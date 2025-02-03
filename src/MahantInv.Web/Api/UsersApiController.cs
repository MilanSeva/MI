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
                    .Where(u=>u.UserName != "msystem")
                    .Select(u => new UserListDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        Status = u.IsActive ? "Active" : "Inactive",
                        IsMfaEnabled = u.IsMfaEnabled ? "Enable" : "Disable"
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
                    return BadRequest(errors);
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
                    List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
                    return BadRequest(errors);
                }
                var user = await _userManager.CreateAsync(new MIIdentityUser
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    EmailConfirmed = true,
                    IsActive = true,
                }, request.Password);

                return Ok(new { success = true });

            }
            return Unauthorized();
        }
    }
}
