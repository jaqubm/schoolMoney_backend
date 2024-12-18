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
public class ClassController(IConfiguration config, IClassRepository classRepository) : ControllerBase
{
    private readonly AuthHelper _authHelper = new (config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<Class, ClassDto>();
        c.CreateMap<Class, ClassListDto>();
        c.CreateMap<User, UserInClassDto>();
        c.CreateMap<Child, ChildInClassDto>();
    }));

    [HttpPost("Create")]
    public async Task<ActionResult<string>> CreateClass([FromBody] ClassCreatorDto classCreatorDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");

        var newClass = new Class
        {
            Name = classCreatorDto.Name,
            SchoolName = classCreatorDto.SchoolName,
            TreasurerId = userId
        };
        
        await classRepository.AddEntityAsync(newClass);
        
        return await classRepository.SaveChangesAsync() ? Ok(newClass.ClassId) : Problem("Failed to create class!");
    }

    [HttpGet("Get/{classId}")]
    public async Task<ActionResult<ClassDto>> GetClass([FromRoute] string classId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return BadRequest("Invalid Token!");
        
        var classDb = await classRepository.GetClassByIdAsync(classId);
        if (classDb is null) return NotFound("Class not found!");
        if (!classDb.TreasurerId.Equals(userId) && (classDb.Children is not null && classDb.Children.Any(c => c.ParentId.Equals(userId))))
            return Unauthorized("You don't have permission to view this class!");
        
        var classDto = _mapper.Map<ClassDto>(classDb);
        classDto.IsTreasurer = classDb.TreasurerId.Equals(userId);

        foreach (var child in classDto.Children)
        {
            var parentDb = await classRepository.GetUserByIdAsync(child.ParentId);
            
            if (parentDb is null) continue;
            
            child.ParentName = parentDb.Name;
            child.ParentSurname = parentDb.Surname;
        }
        
        return Ok(classDto);
    }

    [HttpGet("Search/{className}")]
    public async Task<ActionResult<List<ClassListDto>>> GetClasses([FromRoute] string className)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return BadRequest("Invalid Token!");
        
        var classListDb = await classRepository.SearchClassesByNameAsync(className);
        var classList = _mapper.Map<List<ClassListDto>>(classListDb);
        classList.ForEach(c =>
        {
            c.IsTreasurer =
                classListDb
                    .FirstOrDefault(cl => 
                        cl.ClassId.Equals(c.ClassId))!.TreasurerId.Equals(userId);
        });
        
        return Ok(classList);
    }

    [HttpPut("Update/{classId}")]
    public async Task<ActionResult<string>> UpdateClass([FromRoute] string classId, [FromBody] ClassCreatorDto classCreatorDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return BadRequest("Invalid Token!");
        
        var classDb = await classRepository.GetClassByIdAsync(classId);
        if (classDb is null) return NotFound("Class not found!");
        if (!classDb.TreasurerId.Equals(userId)) 
            return Unauthorized("You don't have permission to update this class!");
        
        classDb.Name = classCreatorDto.Name;
        classDb.SchoolName = classCreatorDto.SchoolName;
        
        classRepository.UpdateEntity(classDb);
        
        return await classRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update class!");
    }

    [HttpDelete("Delete/{classId}")]
    public async Task<ActionResult<string>> DeleteClass([FromRoute] string classId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return BadRequest("Invalid Token!");
        
        var classDb = await classRepository.GetClassByIdAsync(classId);
        if (classDb is null) return NotFound("Class not found!");
        if (!classDb.TreasurerId.Equals(userId)) 
            return Unauthorized("You don't have permission to delete this class!");
        
        classRepository.DeleteEntity(classDb);
        
        return await classRepository.SaveChangesAsync() ? Ok() : Problem("Failed to delete class!");
    }
}