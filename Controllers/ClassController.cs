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
public class ClassController(
    IConfiguration config, 
    IClassRepository classRepository
    ) : ControllerBase
{
    private readonly AuthHelper _authHelper = new (config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<Class, ClassListDto>();
    }));

    [HttpPost("Create")]
    public async Task<ActionResult<string>> CreateClass([FromBody] ClassCreatorDto classCreatorDto)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var classWithGivenSchoolAndClassNameExists = await classRepository.ClassWithGivenSchoolAndClassNameExistsAsync(classCreatorDto.SchoolName, classCreatorDto.Name);
        if (classWithGivenSchoolAndClassNameExists) return Conflict("Class with given name in this school already exists!");

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
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var classDb = await classRepository.GetClassByIdAsync(classId);
        if (classDb is null) return NotFound("Class not found!");
        if (classDb.Treasurer is null) return NotFound("Treasurer of the class not found!");
        if (!classDb.TreasurerId.Equals(userId) && (classDb.Children is not null && classDb.Children.Any(c => c.ParentId.Equals(userId))))
            return Unauthorized("You don't have permission to view this class!");

        var classDto = new ClassDto
        {
            Name = classDb.Name,
            SchoolName = classDb.SchoolName,
            IsTreasurer = classDb.TreasurerId.Equals(userId),
            Treasurer = new UserInClassDto
            {
                Email = classDb.Treasurer.Email,
                Name = classDb.Treasurer.Name,
                Surname = classDb.Treasurer.Surname
            },
            Children = classDb.Children?.Select<Child, ChildInClassDto>(child =>
            {
                var childInClassDto = new ChildInClassDto
                {
                    ChildId = child.ChildId,
                    Name = child.Name,
                    ParentId = child.ParentId,
                    ParentName = child.Parent?.Name,
                    ParentSurname = child.Parent?.Surname
                };

                return childInClassDto;
            })
        };
        
        return Ok(classDto);
    }

    [HttpGet("Search/{className}")]
    public async Task<ActionResult<List<ClassListDto>>> GetClasses([FromRoute] string className)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var classListDb = await classRepository.GetClassListByNameThatStartsWithAsync(className);
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
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var classDb = await classRepository.GetClassByIdAsync(classId);
        if (classDb is null) return NotFound("Class not found!");
        if (!classDb.TreasurerId.Equals(userId)) return Unauthorized("You don't have permission to update this class!");

        var classWithGivenSchoolAndClassNameExists = await classRepository.ClassWithGivenSchoolAndClassNameExistsAsync(classCreatorDto.SchoolName, classCreatorDto.Name);
        if (classWithGivenSchoolAndClassNameExists) return Conflict("Class with given name in this school already exists!");
        
        classDb.Name = classCreatorDto.Name;
        classDb.SchoolName = classCreatorDto.SchoolName;
        
        classRepository.UpdateEntity(classDb);
        
        return await classRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update class!");
    }

    [HttpDelete("Delete/{classId}")]
    public async Task<ActionResult<string>> DeleteClass([FromRoute] string classId)
    {
        var userId = await _authHelper.GetUserIdFromToken(HttpContext);
        if (userId is null) return Unauthorized("Invalid Token!");
        
        var classDb = await classRepository.GetClassByIdAsync(classId);
        if (classDb is null) return NotFound("Class not found!");
        if (!classDb.TreasurerId.Equals(userId)) return Unauthorized("You don't have permission to delete this class!");
        
        classRepository.DeleteEntity(classDb);
        
        return await classRepository.SaveChangesAsync() ? Ok() : Problem("Failed to delete class!");
    }
}
