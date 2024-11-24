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
public class FundraiseController(IConfiguration config, IFundraiseRepository fundraiseRepository) : ControllerBase
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
        if (fundraiseDb.Class is null) return NotFound("Class not found!");
        if (fundraiseDb.Account is null) return NotFound("Account not found!");
        
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
        fundraise.CanEdit = fundraiseDb.Class.TreasurerId.Equals(userId);

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
        fundraiseDb.GoalAmount = fundraiseCreatorDto.GoalAmount;
        fundraiseDb.StartDate = fundraiseCreatorDto.StartDate;
        fundraiseDb.EndDate = fundraiseCreatorDto.EndDate;
        fundraiseDb.ClassId = fundraiseCreatorDto.ClassId;
        
        fundraiseRepository.UpdateEntity(fundraiseDb);
        
        return await fundraiseRepository.SaveChangesAsync() ? Ok(fundraiseDb.FundraiseId) : Problem("Failed to update fundraise!");
    }
}