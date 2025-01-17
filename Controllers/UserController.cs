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
public class UserController(
    IConfiguration config, 
    IUserRepository userRepository, 
    IChildRepository childRepository, 
    IAccountRepository accountRepository,
    IClassRepository classRepository
    ) : ControllerBase
{
    private readonly AuthHelper _authHelper = new (config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<Class, ClassListDto>();
        c.CreateMap<Fundraise, FundraiseListDto>();
    }));
    
    [HttpGet("Get")]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        if (userDb.AccountNumber is null || userDb.Account is null) return NotFound("Account not found!");

        var userDto = new UserDto
        {
            Email = userDb.Email,
            Name = userDb.Name,
            Surname = userDb.Surname,
            CreatedAt = userDb.CreatedAt,
            Account = new AccountDto
            {
                AccountNumber = userDb.AccountNumber,
                Balance = userDb.Account.Balance
            }
        };

        if (userDb.Children is null) return Ok(userDto);
        
        foreach (var child in userDb.Children)
        {
            var childDb = await childRepository.GetChildByIdAsync(child.ChildId);
            if (childDb is null) return NotFound("Child not found!");

            var childDto = new ChildDto
            {
                ChildId = childDb.ChildId,
                Name = childDb.Name,
                ClassName = childDb.Class?.Name,
                SchoolName = childDb.Class?.SchoolName
            };

            userDto.Children.Add(childDto);
        }

        return Ok(userDto);
    }

    [HttpPut("Update")]
    public async Task<ActionResult<string>> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");

        userDb.Email = userUpdateDto.Email;
        userDb.Name = userUpdateDto.Name;
        userDb.Surname = userUpdateDto.Surname;
        
        userRepository.UpdateEntity(userDb);
        
        return await userRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update user!");
    }

    [HttpPost("CreateChildProfile")]
    public async Task<ActionResult<string>> CreateChildProfile([FromBody] ChildCreatorDto childCreatorDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");

        var child = new Child
        {
            Name = childCreatorDto.Name,
            ParentId = userId,
            ClassId = childCreatorDto.ClassId,
        };

        await childRepository.AddEntityAsync(child);
        
        return await childRepository.SaveChangesAsync() ? Ok(child.ChildId) : Problem("Failed to create child profile!");
    }

    [HttpGet("GetChildProfile/{childId}")]
    public async Task<ActionResult<ChildDto>> GetChildProfile([FromRoute] string childId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var childDb = await childRepository.GetChildByIdAsync(childId);
        if (childDb is null) return NotFound("Child not found!");
        if (!childDb.ParentId.Equals(userId)) return Unauthorized("You cannot get child profile which you are not owner!");

        var childDto = new ChildDto
        {
            ChildId = childDb.ChildId,
            Name = childDb.Name,
            ClassName = childDb.Class?.Name,
            SchoolName = childDb.Class?.SchoolName
        };
        
        return Ok(childDto);
    }
    
    [HttpPut("UpdateChildProfile/{childId}")]
    public async Task<ActionResult<string>> UpdateChildProfile([FromRoute] string childId, [FromBody] ChildCreatorDto childCreatorDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var childDb = await childRepository.GetChildByIdAsync(childId);
        if (childDb is null) return NotFound("Child not found!");
        if (!childDb.ParentId.Equals(userId)) return Unauthorized("You cannot update child profile which you are not owner!");
        
        childDb.Name = childCreatorDto.Name;
        childDb.ClassId = childCreatorDto.ClassId;

        childRepository.UpdateEntity(childDb);
        
        return await childRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update child profile!");
    }

    [HttpDelete("DeleteChildProfile/{childId}")]
    public async Task<ActionResult<string>> DeleteChildProfile([FromRoute] string childId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var childDb = await childRepository.GetChildByIdAsync(childId);
        if (childDb is null) return NotFound("Child not found!");
        if (!childDb.ParentId.Equals(userId)) return Unauthorized("You cannot update child profile which you are not owner!");
        
        childRepository.DeleteEntity(childDb);
        
        return await childRepository.SaveChangesAsync() ? Ok() : Problem("Failed to delete child profile!");
    }
    
    [HttpGet("GetClasses")]
    public async Task<ActionResult<List<ClassListDto>>> GetClasses()
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var classListDb = await classRepository.GetClassListByTreasurerIdAsync(userId);
        
        var classList = _mapper.Map<List<ClassListDto>>(classListDb);
        classList.ForEach(c => c.IsTreasurer = true);
        
        return Ok(classList);
    }

    [HttpGet("GetFundraises")]
    public async Task<ActionResult<List<FundraiseListDto>>> GetFundraises()
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        
        var fundraises = new List<FundraiseListDto>();
        
        if (userDb.ClassesAsTreasurer is not null)
        {
            foreach (var classAsTreasurer in userDb.ClassesAsTreasurer)
            {
                var classDb = await classRepository.GetClassByIdAsync(classAsTreasurer.ClassId);
                if (classDb is null) return NotFound("Class not found!");
                if (classDb.Fundraises is null) continue;
            
                fundraises
                    .AddRange(classDb
                        .Fundraises
                        .Select(classFundraise =>
                        {
                            var fundraise = _mapper.Map<FundraiseListDto>(classFundraise);
                            fundraise.ClassName = classDb.Name;
                            fundraise.SchoolName = classDb.SchoolName;
                            fundraise.IsTreasurer = true;
                            return fundraise;
                        })
                    );
            }
        }

        if (userDb.Children is not null)
        {
            foreach (var child in userDb.Children)
            {
                var childDb = await childRepository.GetChildByIdAsync(child.ChildId);
                if (childDb is null) return NotFound("Child not found!");
                if (childDb.Class?.Fundraises is null) continue;

                fundraises
                    .AddRange(childDb
                        .Class
                        .Fundraises
                        .Select(classFundraise =>
                        {
                            var fundraise = _mapper.Map<FundraiseListDto>(classFundraise);
                            fundraise.ClassName = childDb.Class.Name;
                            fundraise.SchoolName = childDb.Class.SchoolName;
                            return fundraise;
                        })
                    );
            }
        }
        
        fundraises = fundraises
            .DistinctBy(f => f.FundraiseId) // Remove duplicates based on FundraiseId
            .ToList();
        
        return Ok(fundraises);
    }
    
    [HttpGet("GetTransactionHistory")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactionHistory()
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        if (userDb.AccountNumber is null) return NotFound("Account number not found!");
        
        var accountDb = await accountRepository.GetAccountByAccountNumberAsync(userDb.AccountNumber);
        if (accountDb is null) return NotFound("Account number not found!");

        var transactionHistory = new List<TransactionDto>();
        
        if (accountDb.SourceTransactions is not null)
            transactionHistory.AddRange(accountDb.SourceTransactions
                .Select(st => new TransactionDto
                {
                    TransactionId = st.TransactionId,
                    Title = st.Title,
                    Amount = st.Amount,
                    Date = st.Date,
                    Type = st.Type,
                    Status = st.Status,
                    SourceAccountNumber = st.SourceAccountNumber,
                    DestinationAccountNumber = st.DestinationAccountNumber
                }));
        
        if (accountDb.DestinationTransactions is not null)
            transactionHistory.AddRange(accountDb.DestinationTransactions
                .Select(dt => new TransactionDto
                {
                    TransactionId = dt.TransactionId,
                    Title = dt.Title,
                    Amount = dt.Amount,
                    Date = dt.Date,
                    Type = dt.Type,
                    Status = dt.Status,
                    SourceAccountNumber = dt.SourceAccountNumber,
                    DestinationAccountNumber = dt.DestinationAccountNumber
                }));
        
        return Ok(transactionHistory);
    }
}