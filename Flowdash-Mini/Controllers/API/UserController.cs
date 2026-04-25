using AutoMapper;
using Flowdash_Mini.Dtos.Accounts;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.ViewModels.Accounts;
using Flowdash_Mini.ViewModels.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flowdash_Mini.Controllers.API
{
    [Route("API/[controller]"), ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult<UserVM>> GetUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return _mapper.Map<UserVM>(user);
        }

        [HttpPost("Update")]
        public async Task<ActionResult<UserVM>> Update([FromBody] UserDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User was not found");
            }

            user.UserName = model.UserName;
            user.FullName = model.FullName;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                string msg = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                return BadRequest(msg);
            }

            return Ok("User updated successfully");
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
            if (!result.Succeeded)
            {
                string msg = "";
                foreach (var error in result.Errors)
                {
                    msg += error.Description + Environment.NewLine;
                }
                return BadRequest(msg);
            }

            if (model.LogEveryoneOut)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return Ok("Password has been successfully reset");
        }
    }
}
