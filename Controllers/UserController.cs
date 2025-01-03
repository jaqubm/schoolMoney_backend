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
public class UserController(IConfiguration config, IUserRepository userRepository) : ControllerBase
{
    private readonly AuthHelper _authHelper = new (config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<User, UserDto>();
        c.CreateMap<Account, AccountDto>();
        c.CreateMap<UserUpdateDto, User>();
        c.CreateMap<Child, ChildDto>();
        c.CreateMap<Class, ClassListDto>();
        c.CreateMap<User, UserInClassDto>();
        c.CreateMap<Fundraise, FundraiseListDto>();
        c.CreateMap<Transaction, TransactionDto>();
    }));
    
    [HttpGet("Get")]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        
        return Ok(_mapper.Map<UserDto>(userDb));
    }

    [HttpPut("Update")]
    public async Task<ActionResult<string>> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        
        _mapper.Map(userUpdateDto, userDb);
        
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

        await userRepository.AddEntityAsync(child);
        
        return await userRepository.SaveChangesAsync() ? Ok(child.ChildId) : Problem("Failed to create child profile!");
    }

    [HttpGet("GetChildProfile/{childId}")]
    public async Task<ActionResult<ChildDto>> GetChildProfile([FromRoute] string childId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var childDb = await userRepository.GetChildByIdAsync(childId);
        if (childDb is null) return NotFound("Child not found!");
        if (!childDb.ParentId.Equals(userId)) return Unauthorized("You cannot update child profile which you are not owner!");
        
        var childDto = _mapper.Map<ChildDto>(childDb);
        childDto.ClassName = childDb.Class?.Name;
        
        return Ok(childDto);
    }
    
    [HttpPut("UpdateChildProfile/{childId}")]
    public async Task<ActionResult<string>> UpdateChildProfile([FromRoute] string childId, [FromBody] ChildCreatorDto childCreatorDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var childDb = await userRepository.GetChildByIdAsync(childId);
        if (childDb is null) return NotFound("Child not found!");
        if (!childDb.ParentId.Equals(userId)) return Unauthorized("You cannot update child profile which you are not owner!");
        
        childDb.Name = childCreatorDto.Name;
        childDb.ClassId = childCreatorDto.ClassId;

        userRepository.UpdateEntity(childDb);
        
        return await userRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update child profile!");
    }

    [HttpDelete("DeleteChildProfile/{childId}")]
    public async Task<ActionResult<string>> DeleteChildProfile([FromRoute] string childId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var childDb = await userRepository.GetChildByIdAsync(childId);
        if (childDb is null) return NotFound("Child not found!");
        if (!childDb.ParentId.Equals(userId)) return Unauthorized("You cannot update child profile which you are not owner!");
        
        userRepository.DeleteEntity(childDb);
        
        return await userRepository.SaveChangesAsync() ? Ok() : Problem("Failed to delete child profile!");
    }
    
    [HttpGet("GetClasses")]
    public async Task<ActionResult<List<ClassListDto>>> GetClasses()
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return BadRequest("Invalid Token!");
        
        var classListDb = await userRepository.GetClassListByTreasurerIdAsync(userId);
        
        var classList = _mapper.Map<List<ClassListDto>>(classListDb);
        classList.ForEach(c => c.IsTreasurer = true);
        
        return Ok(classList);
    }

    [HttpGet("GetFundraises")]
    public async Task<ActionResult<List<FundraiseListDto>>> GetFundraises()
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return BadRequest("Invalid Token!");
        
        var userDb = await userRepository.GetUserByIdAsync(userId);
        if (userDb is null) return NotFound("User not found!");
        
        var fundraises = new List<FundraiseListDto>();
        
        if (userDb.ClassesAsTreasurer is not null)
        {
            foreach (var classAsTreasurer in userDb.ClassesAsTreasurer)
            {
                var classDb = await userRepository.GetClassByIdAsync(classAsTreasurer.ClassId);
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
                var childDb = await userRepository.GetChildByIdAsync(child.ChildId);
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
        if (userDb.AccountNumber is null) return BadRequest("Account number not found!");
        
        var accountDb = await userRepository.GetAccountByAccountNumberAsync(userDb.AccountNumber);
        if (accountDb is null) return BadRequest("Account number not found!");

        var transactionHistory = new List<TransactionDto>();
        
        if (accountDb.SourceTransactions is not null)
            transactionHistory.AddRange(accountDb.SourceTransactions
                .Select(st => _mapper.Map<TransactionDto>(st)));
        
        if (accountDb.DestinationTransactions is not null)
            transactionHistory.AddRange(accountDb.DestinationTransactions
                .Select(dt => _mapper.Map<TransactionDto>(dt)));
        
        return Ok(transactionHistory);
    }
}