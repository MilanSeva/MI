using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Dtos.User;
using MahantInv.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
            return User.Identity.Name.Equals("system", System.StringComparison.OrdinalIgnoreCase);
        }
        [HttpGet("all")]
        public async Task<IActionResult> Users()
        {
            if (IsSystemUser())
            {
                var users = await _context.Users
                    .Select(u => new UserListDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        IsMfaEnabled = string.IsNullOrWhiteSpace(u.AuthenticatorKey) ? "Disable" : "Enable"
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
                user.IsMfaEnabled = true;
                user.AuthenticatorKey = null;
                _context.SaveChanges();
                return Ok();
            }
            return Unauthorized();
        }
        [HttpPost("save")]
        public async Task<IActionResult> SaveUser([FromBody] UserAddEditDto request)
        {
            if (IsSystemUser())
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(request.Id))
                    {
                        var user = await _userManager.CreateAsync(new MIIdentityUser
                        {
                            Email = request.Email,
                            UserName = request.Email,
                            EmailConfirmed = true
                        }, request.Password);
                    }
                    else
                    {

                    }
                    return Ok();
                }
            }
            return Unauthorized();
        }
    }
}
