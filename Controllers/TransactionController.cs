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
public class TransactionController(
    IConfiguration config, 
    ITransactionRepository transactionRepository,
    IUserRepository userRepository,
    IAccountRepository accountRepository
    ) : ControllerBase
{
    private readonly AuthHelper _authHelper = new (config);
    
    [HttpPost("Withdraw")]
    public async Task<ActionResult<string>> CreateWithdrawTransaction([FromBody] TransactionWithdrawDto transactionWithdrawDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");

        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        if (userDb.AccountNumber is null || userDb.Account is null) return NotFound("Account not found!");
        if (userDb.Account.Balance < transactionWithdrawDto.Amount) 
            return Conflict("Insufficient funds to withdraw given amount!");

        if (transactionWithdrawDto.DestinationAccountNumber is null or "")
        {
            var destinationAccount = new Account();
            await transactionRepository.AddEntityAsync(destinationAccount);
            transactionWithdrawDto.DestinationAccountNumber = destinationAccount.AccountNumber;
        }
        else
        {
            var destinationAccountDb = await accountRepository.GetAccountByAccountNumberAsync(transactionWithdrawDto.DestinationAccountNumber);
            if (destinationAccountDb is null)
            {
                await transactionRepository.AddEntityAsync(new Account
                {
                    AccountNumber = transactionWithdrawDto.DestinationAccountNumber
                });
            }
        }

        var transaction = new Transaction
        {
            Type = "Withdraw",
            SourceAccountNumber = userDb.AccountNumber,
            DestinationAccountNumber = transactionWithdrawDto.DestinationAccountNumber
        };
        
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

        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        if (userDb.AccountNumber is null || userDb.Account is null) return NotFound("Account not found!");
        
        if (transactionDepositDto.SourceAccountNumber is null or "")
        {
            var sourceAccount = new Account();
            await transactionRepository.AddEntityAsync(sourceAccount);
            transactionDepositDto.SourceAccountNumber = sourceAccount.AccountNumber;
        }
        else
        {
            var sourceAccountDb = await accountRepository.GetAccountByAccountNumberAsync(transactionDepositDto.SourceAccountNumber);
            if (sourceAccountDb is null)
            {
                await transactionRepository.AddEntityAsync(new Account
                {
                    AccountNumber = transactionDepositDto.SourceAccountNumber
                });
            }
        }
        
        var transaction = new Transaction
        {
            Type = "Deposit",
            SourceAccountNumber = transactionDepositDto.SourceAccountNumber,
            DestinationAccountNumber = userDb.AccountNumber
        };
        
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

        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        if (userDb.AccountNumber is null || userDb.Account is null) return NotFound("Account not found!");
        if (userDb.Account.Balance < transactionTransferDto.Amount) 
            return Conflict("Insufficient funds to withdraw given amount!");
        
        var destinationAccountDb = await accountRepository.GetAccountByAccountNumberAsync(transactionTransferDto.DestinationAccountNumber);
        if (destinationAccountDb is null) return NotFound("Destination Account not found!");
        
        var transaction = new Transaction
        {
            Type = "Transfer",
            SourceAccountNumber = userDb.AccountNumber,
            DestinationAccountNumber = transactionTransferDto.DestinationAccountNumber
        };
        
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

        var transaction = new TransactionDto
        {
            TransactionId = transactionDb.TransactionId,
            Amount = transactionDb.Amount,
            Date = transactionDb.Date,
            Type = transactionDb.Type,
            Status = transactionDb.Status,
            SourceAccountNumber = transactionDb.SourceAccountNumber,
            DestinationAccountNumber = transactionDb.DestinationAccountNumber
        };
        
        return Ok(transaction);
    }
}