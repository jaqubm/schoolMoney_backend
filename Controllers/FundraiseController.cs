using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using schoolMoney_backend.Dtos;
using schoolMoney_backend.Helpers;
using schoolMoney_backend.Models;
using schoolMoney_backend.Repositories;

namespace schoolMoney_backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class FundraiseController(IConfiguration config, IFundraiseRepository fundraiseRepository, ITransactionRepository transactionRepository) : ControllerBase
{
    private readonly AuthHelper _authHelper = new (config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<Fundraise, FundraiseDto>();
    }));

    [HttpPost("Create")]
    public async Task<ActionResult<string>> CreateFundraise([FromBody] FundraiseCreatorDto fundraiseCreatorDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var classDb = await fundraiseRepository.GetClassByIdAsync(fundraiseCreatorDto.ClassId);
        if (classDb is null) return NotFound("Class not found!");
        if (!classDb.TreasurerId.Equals(userId)) return BadRequest("Only treasurers can create fundraise!");
        
        var fundraiseAccount = new Account();
        var fundraise = new Fundraise
        {
            Title = fundraiseCreatorDto.Title,
            Description = fundraiseCreatorDto.Description,
            ImageIndex = fundraiseCreatorDto.ImageIndex,
            GoalAmount = fundraiseCreatorDto.GoalAmount,
            StartDate = fundraiseCreatorDto.StartDate,
            EndDate = fundraiseCreatorDto.EndDate,
            ClassId = fundraiseCreatorDto.ClassId,
            Account = fundraiseAccount
        };
        
        await fundraiseRepository.AddEntityAsync(fundraise);

        return await fundraiseRepository.SaveChangesAsync() ? Ok(fundraise.FundraiseId) : Problem("Failed to create fundraise!");
    }

    [HttpGet("Get/{fundraiseId}")]
    public async Task<ActionResult<FundraiseDto>> GetFundraise([FromRoute] string fundraiseId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return BadRequest("Invalid Token!");
        
        var fundraiseDb = await fundraiseRepository.GetFundraiseByIdAsync(fundraiseId);
        if (fundraiseDb is null) return NotFound("Fundraise not found!");
        if (fundraiseDb.Class is null) return NotFound("Class of fundraise not found!");
        if (fundraiseDb.Account is null) return NotFound("Account of fundraise not found!");
        
        var fundraise = _mapper.Map<Fundraise, FundraiseDto>(fundraiseDb);
        
        fundraise.RaisedAmount = fundraiseDb.Account.Balance;
        
        if (fundraiseDb.Account.DestinationTransactions is not null)
        {
            fundraise.TotalSupporters = fundraiseDb
                .Account
                .DestinationTransactions
                .Where(t => t.DestinationAccountNumber == fundraiseDb.Account.AccountNumber)
                .Select(t => t.SourceAccountNumber)
                .Distinct()
                .Count();
        }
        
        fundraise.ClassName = fundraiseDb.Class.Name;
        fundraise.SchoolName = fundraiseDb.Class.SchoolName;
        fundraise.IsTreasurer = fundraiseDb.Class.TreasurerId.Equals(userId);

        return Ok(fundraise);
    }

    [HttpPut("Update/{fundraiseId}")]
    public async Task<ActionResult<string>> UpdateFundraise([FromRoute] string fundraiseId, [FromBody] FundraiseCreatorDto fundraiseCreatorDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var classDb = await fundraiseRepository.GetClassByIdAsync(fundraiseCreatorDto.ClassId);
        if (classDb is null) return NotFound("Class not found!");
        if (!classDb.TreasurerId.Equals(userId)) return BadRequest("Only treasurers can create fundraise!");
        
        var fundraiseDb = await fundraiseRepository.GetFundraiseByIdAsync(fundraiseId);
        if (fundraiseDb is null) return NotFound("Fundraise not found!");
        if (!(fundraiseDb.Class is not null && fundraiseDb.Class.TreasurerId.Equals(userId))) 
            return BadRequest("Only treasurers can update fundraise!");
        
        fundraiseDb.Title = fundraiseCreatorDto.Title;
        fundraiseDb.Description = fundraiseCreatorDto.Description;
        fundraiseDb.ImageIndex = fundraiseCreatorDto.ImageIndex;
        fundraiseDb.GoalAmount = fundraiseCreatorDto.GoalAmount;
        fundraiseDb.StartDate = fundraiseCreatorDto.StartDate;
        fundraiseDb.EndDate = fundraiseCreatorDto.EndDate;
        fundraiseDb.ClassId = fundraiseCreatorDto.ClassId;
        
        fundraiseRepository.UpdateEntity(fundraiseDb);
        
        return await fundraiseRepository.SaveChangesAsync() ? Ok(fundraiseDb.FundraiseId) : Problem("Failed to update fundraise!");
    }
    
    [HttpDelete("Delete/{fundraiseId}")]
    public async Task<ActionResult<string>> DeleteFundraise([FromRoute] string fundraiseId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return BadRequest("Invalid Token!");
        
        var fundraiseDb = await fundraiseRepository.GetFundraiseByIdAsync(fundraiseId);
        if (fundraiseDb is null) return NotFound("Fundraise not found!");
        if (fundraiseDb.Class is null) return NotFound("Class of fundraise not found!");
        if (fundraiseDb.Account is null) return NotFound("Account of fundraise not found!");
        if (fundraiseDb.Account.Balance > decimal.Zero) 
            return BadRequest("You need to withdraw money from the fundraise in order to delete it!");
        
        fundraiseRepository.DeleteEntity(fundraiseDb);

        return await fundraiseRepository.SaveChangesAsync() ? Ok(fundraiseDb.FundraiseId) : Problem("Failed to delete fundraise!");
    }

    [HttpGet("GetTransactionHistory/{fundraiseId}")]
    public async Task<ActionResult<List<TransactionDto>>> GetFundraiseTransactionHistory([FromRoute] string fundraiseId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");

        var fundraise = await fundraiseRepository.GetFundraiseByIdAsync(fundraiseId);
        if (fundraise is null) return NotFound("Fundraise not found!");
        if (fundraise.Account is null) return NotFound("Fundraise account not found!");

        if (fundraise.Class?.TreasurerId != userId)
            return Unauthorized("You are not authorized to view this transaction history!");

        var account = await transactionRepository.GetAccountByAccountNumberAsync(fundraise.Account.AccountNumber);
        if (account is null) return NotFound("Account not found!");

        var transactions = new List<TransactionDto>();

        if (account.SourceTransactions is not null)
            transactions.AddRange(account.SourceTransactions.Select(t => new TransactionDto
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                Date = t.Date,
                Type = t.Type,
                Status = t.Status,
                SourceAccountNumber = t.SourceAccountNumber,
                DestinationAccountNumber = t.DestinationAccountNumber
            }));

        if (account.DestinationTransactions is not null)
            transactions.AddRange(account.DestinationTransactions.Select(t => new TransactionDto
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                Date = t.Date,
                Type = t.Type,
                Status = t.Status,
                SourceAccountNumber = t.SourceAccountNumber,
                DestinationAccountNumber = t.DestinationAccountNumber
            }));

        transactions = transactions.OrderByDescending(t => t.Date).ToList();

        return Ok(transactions);
    }
}