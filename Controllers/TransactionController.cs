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
public class TransactionController(IConfiguration config, ITransactionRepository transactionRepository) : ControllerBase
{
    private readonly AuthHelper _authHelper = new (config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<TransactionWithdrawDto, Transaction>();
        c.CreateMap<TransactionDepositDto, Transaction>();
        c.CreateMap<TransactionTransferDto, Transaction>();
        c.CreateMap<Transaction, TransactionDto>();
    }));
    
    [HttpPost("Withdraw")]
    public async Task<ActionResult<string>> CreateWithdrawTransaction([FromBody] TransactionWithdrawDto transactionWithdrawDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");

        var userDb = await transactionRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        if (userDb.AccountNumber is null || userDb.Account is null) return NotFound("Account not found!");
        if (userDb.Account.Balance < transactionWithdrawDto.Amount) 
            return BadRequest("Insufficient funds to withdraw given amount!");
        
        var destinationAccountDb = await transactionRepository.GetAccountByAccountNumberAsync(transactionWithdrawDto.DestinationAccountNumber);
        if (destinationAccountDb is null)
        {
            await transactionRepository.AddEntityAsync(new Account
            {
                AccountNumber = transactionWithdrawDto.DestinationAccountNumber
            });
        }
        
        var transaction = _mapper.Map<Transaction>(transactionWithdrawDto);
        transaction.SourceAccountNumber = userDb.AccountNumber;
        transaction.Type = "Withdraw";
        
        await transactionRepository.AddEntityAsync(transaction);
        
        userDb.Account.Balance -= transactionWithdrawDto.Amount;
        
        transactionRepository.UpdateEntity(userDb);
        
        return await transactionRepository.SaveChangesAsync() ? Ok(transaction.TransactionId) : Problem("Transaction failed to process!");
    }
    
    [HttpPost("Deposit")]
    public async Task<ActionResult<string>> CreateDepositTransaction([FromBody] TransactionDepositDto transactionDepositDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");

        var userDb = await transactionRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        if (userDb.AccountNumber is null || userDb.Account is null) return NotFound("Account not found!");
        
        var sourceAccountDb = await transactionRepository.GetAccountByAccountNumberAsync(transactionDepositDto.SourceAccountNumber);
        if (sourceAccountDb is null)
        {
            await transactionRepository.AddEntityAsync(new Account
            {
                AccountNumber = transactionDepositDto.SourceAccountNumber
            });
        }
        
        var transaction = _mapper.Map<Transaction>(transactionDepositDto);
        transaction.DestinationAccountNumber = userDb.AccountNumber;
        transaction.Type = "Deposit";
        
        await transactionRepository.AddEntityAsync(transaction);
        
        userDb.Account.Balance += transactionDepositDto.Amount;
        
        transactionRepository.UpdateEntity(userDb);
        
        return await transactionRepository.SaveChangesAsync() ? Ok(transaction.TransactionId) : Problem("Transaction failed to process!");
    }
    
    [HttpPost("Transfer")]
    public async Task<ActionResult<string>> CreateTransferTransaction([FromBody] TransactionTransferDto transactionTransferDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");

        var userDb = await transactionRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        if (userDb.AccountNumber is null || userDb.Account is null) return NotFound("Account not found!");
        if (userDb.Account.Balance < transactionTransferDto.Amount) 
            return BadRequest("Insufficient funds to withdraw given amount!");
        
        var destinationAccountDb = await transactionRepository.GetAccountByAccountNumberAsync(transactionTransferDto.DestinationAccountNumber);
        if (destinationAccountDb is null) return NotFound("Destination Account not found!");
        
        var transaction = _mapper.Map<Transaction>(transactionTransferDto);
        transaction.SourceAccountNumber = userDb.AccountNumber;
        transaction.Type = "Transfer";
        
        await transactionRepository.AddEntityAsync(transaction);
        
        userDb.Account.Balance -= transactionTransferDto.Amount;
        destinationAccountDb.Balance += transactionTransferDto.Amount;
        
        transactionRepository.UpdateEntity(userDb);
        transactionRepository.UpdateEntity(destinationAccountDb);
        
        return await transactionRepository.SaveChangesAsync() ? Ok(transaction.TransactionId) : Problem("Transaction failed to process!");
    }

    [HttpGet("Get/{transactionId}")]
    public async Task<ActionResult<TransactionDto>> GetTransactionById(string transactionId)
    {
        var transactionDb = await transactionRepository.GetTransactionByIdAsync(transactionId);
        if (transactionDb is null) return NotFound();
        
        return Ok(_mapper.Map<TransactionDto>(transactionDb));
    }
}