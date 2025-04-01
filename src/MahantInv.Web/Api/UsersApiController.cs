using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Dtos.User;
using MahantInv.Infrastructure.Identity;
using MahantInv.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MahantInv.Web.Api
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UsersApiController : BaseApiController
    {
        private readonly MIDbContext _context;
        private readonly UserManager<MIIdentityUser> _userManager;
        public UsersApiController(UserManager<MIIdentityUser> userManager, IMapper mapper, MIDbContext context) : base(mapper)
        {
            _context = context;
            _userManager = userManager;
        }
        private bool IsSystemUser()
        {
            return User.Identity.Name.Equals("msystem", System.StringComparison.OrdinalIgnoreCase) || User.Identity.Name.Equals("system", System.StringComparison.OrdinalIgnoreCase);
        }
        [HttpGet("all")]
        public async Task<IActionResult> Users()
        {
            if (IsSystemUser())
            {
                var users = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .Where(u => u.UserName != "msystem")
                    .Select(u => new UserListDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        Status = u.IsActive ? "Active" : "Inactive",
                        IsMfaEnabled = u.IsMfaEnabled ? "Enable" : "Disable",
                        Role = u.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(users);
            }
            return Unauthorized();
        }
        [HttpPost("resetmfa/{Id}")]
        public async Task<IActionResult> ResetMFA(string Id)
        {
            if (IsSystemUser())
            {
                var user = await _context.Users.FindAsync(Id);
                if (user == null)
                {
                    return BadRequest("User not found");
                }
                if (user.UserName.Equals("msystem", System.StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { success = false });
                }
                user.IsMfaEnabled = false;
                user.AuthenticatorKey = null;
                _context.SaveChanges();
                return Ok(new { success = true });
            }
            return Unauthorized();
        }
        [HttpPost("save")]
        public async Task<IActionResult> SaveUser([FromBody] AddUserDto request)
        {
            if (IsSystemUser())
            {
                if (!ModelState.IsValid)
                {
                    List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
                    return BadRequest(new { success = false, errors = errors.SelectMany(e => e.Select(r => r.ErrorMessage)) });
                }
                List<ModelErrorCollection> newErrors = new();
                var userName = await _userManager.FindByNameAsync(request.UserName);
                if (userName != null)
                {
                    ModelState.AddModelError(nameof(request.UserName), "User Name already exists!");
                }
                var email = await _userManager.FindByEmailAsync(request.Email);
                if (email != null)
                {
                    ModelState.AddModelError(nameof(request.Email), "Email address already exists!");
                }
                if (!ModelState.IsValid)
                {
                    List<ModelErrorCollection> errors = ModelState
                          .Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
                    return BadRequest(new { success = false, errors = errors.SelectMany(e => e.Select(r => r.ErrorMessage)) });
                }
                MIIdentityUser user = new()
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    EmailConfirmed = true,
                    IsActive = true,
                };
                var userResult = await _userManager.CreateAsync(user, request.Password);

                if (userResult.Succeeded)
                {
                    var r = await _userManager.AddToRoleAsync(user, request.Role);

                    return Ok(new { success = true });
                }
                return BadRequest(new { success = false, errors = userResult.Errors.Select(e => $"{e.Code}:{e.Description}") });

            }
            return Unauthorized();
        }
    }
}
