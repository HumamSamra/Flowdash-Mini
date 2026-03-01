using AutoMapper;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Services.CaptchaService;
using Flowdash_Mini.Services.MailService;
using Flowdash_Mini.Services.TokenService;
using Flowdash_Mini.ViewModels.Accounts;
using Flowdash_Mini.ViewModels.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Flowdash_Mini.Controllers.API
{
    [Route("API/[controller]"), ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ISmtpService _smtp;
        private readonly IConfiguration _config;
        private readonly ICaptchaService _captcha;
        private readonly IMapper _mapper;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                ISmtpService smtp, IConfiguration config,
                                ICaptchaService captcha, IMapper mapper, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _smtp = smtp;
            _config = config;
            _captcha = captcha;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(CredentialsVM model)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return BadRequest("You are already logged in");
            }

            var user = _userManager.Users.FirstOrDefault(
                x => x.UserName == model.Account || x.Email == model.Account);
            if (user == null)
            {
                return BadRequest("Username or Password is not correct");
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return BadRequest("Username or Password is not correct");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return BadRequest("This user is disabled, and can not login");
            }

            if (!user.EmailConfirmed)
            {
                // Avoid Mail Confirmation Spamming
                if (DateTime.Now <= user.LastEmailVerificationSent.AddMinutes(3))
                {
                    return BadRequest("Email confirmation message has been already sent to your account, please check your inbox");
                }
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

                string link = $"{_config["WebDomain"]}auth/confirmEmail";
                link += $"?userId={user.Id}&token={code}";

                var res = _smtp.SendMail("Email Confirmation",
                        $"<h1>Hello {user.FullName.Split(" ").First()}</h1>" +
                        $"Please <b><a href=\"{link}\">click here</a></b> to confirm your e-mail" +
                        $"<br /><br />Thank you, Flowdash Team",
                        new List<string>() { user.Email! });
                if (!res.Succeeded)
                {
                    return BadRequest("Couldn't send e-mail verification link");
                }

                user.LastEmailVerificationSent = DateTime.Now;
                await _userManager.UpdateAsync(user);

                return BadRequest("Email confirmation message has sent to your account, please check your inbox");
            }

            var userVM = _mapper.Map<ApiUserVM>(user);
            userVM.Token = await _tokenService.CreateAsync(user);

            return Ok(userVM);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterVM model)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return BadRequest("You are already logged in");
            }

            var user = _mapper.Map<AppUser>(model);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string msg = string.Empty;
                foreach (var err in result.Errors)
                {
                    msg += err.Description + Environment.NewLine;
                }
                return BadRequest(msg);
            }
            await _userManager.AddToRoleAsync(user, nameof(UserType.User));

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

            string link = $"{_config["WebDomain"]}auth/confirmEmail";
            link += $"?userId={user.Id}&token={code}";

            var res = _smtp.SendMail("Email Confirmation",
                    $"<h1>Hello {user.FullName.Split(" ").First()}</h1>" +
                    $"Please <b><a href=\"{link}\">click here</a></b> to confirm your e-mail" +
                    $"<br /><br />Thank you, Flowdash Team",
                    new List<string>() { user.Email! });
            if (!res.Succeeded)
            {
                return BadRequest("Couldn't send e-mail verification link");
            }

            return Ok("Account created successfully, please check your inbox to confirm your email");
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            // var reCaptchaToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            // if (!await _captcha.VerifyAsync(reCaptchaToken))
            // {
            //     return Json(new { statusCode = 400, msg = "Invalid reCaptcha token" });
            // }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var confirmationToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

                string link = $"{_config["WebDomain"]}auth/resetpassword";
                link += $"?userId={user.Id}&token={code}";

                _smtp.SendMail("Reset Your Password",
                        $"<h1>Hello {user.FullName.Split(" ").First()}</h1>" +
                        $"Please <b><a href=\"{link}\">click here</a></b> to reset your password" +
                        $"<br /><br />Thank you, Flowdash Team",
                        new List<string>() { user.Email! });
            }
            return Ok("Password reset message has been sent successfully");
        }

        [HttpPost("ResetPassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangePassword(ChangePasswordVM model)
        {
            // var reCaptchaToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            // if (!await _captcha.VerifyAsync(reCaptchaToken))
            // {
            //     ModelState.AddModelError(string.Empty, "Invalid reCaptcha token");
            //     return View(model);
            // }

            var user = await _userManager.GetUserAsync(User);
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
                // Logout everyone with this account
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return Ok("Password has been successfully reset");
        }
    }
}
