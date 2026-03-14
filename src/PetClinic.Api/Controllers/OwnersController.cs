using AutoMapper;  
using Microsoft.AspNetCore.Authorization;  
using Microsoft.AspNetCore.Mvc;  
using PetClinic.Application;  
using PetClinic.Infrastructure;  
  
namespace PetClinic.Api.Controllers;  
  
[ApiController]  
[Route("api/v1/owners")]  
[Authorize]  
public class OwnersController : ControllerBase  
{  
    private readonly PetClinicDbContext _context;  
    private readonly IUserContextService _userContext;  
    private readonly IMapper _mapper;  
  
    public OwnersController(PetClinicDbContext context, IUserContextService userContext, IMapper mapper)  
    {  
        _context = context;  
        _userContext = userContext;  
        _mapper = mapper;  
    }  
  
    [HttpGet("me")]  
