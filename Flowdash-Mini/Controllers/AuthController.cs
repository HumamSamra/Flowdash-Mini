using AutoMapper;
using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Services.CaptchaService;
using Flowdash_Mini.Services.MailService;
using Flowdash_Mini.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Flowdash_Mini.Controllers
{
    public class AuthController : _BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ISmtpService _smtp;
        private readonly IConfiguration _config;
        private readonly ICaptchaService _captcha;
        private readonly IMapper _mapper;

        public SignInManager<AppUser> SignInManager => _signInManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                ISmtpService smtp, IConfiguration config,
                                ICaptchaService captcha, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _smtp = smtp;
            _config = config;
            _captcha = captcha;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "")
        {
            if (User.Identity!.IsAuthenticated)
            {
                return Redirect("/");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = "")
        {
            if (User.Identity!.IsAuthenticated)
            {
                return Redirect("/");
            }

            var reCaptchaToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            if (!await _captcha.VerifyAsync(reCaptchaToken))
            {
                ModelState.AddModelError(string.Empty, "Invalid reCaptcha token");
                return View(model);
            }

            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userManager.Users.FirstOrDefault(
                x => x.UserName == model.Account || x.Email == model.Account);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username or Password is not correct");
                return View(model);
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Username or Password is not correct");
                return View(model);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError(string.Empty, "This user is disabled, and can not login");
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                // Avoid Mail Confirmation Spamming
                if (DateTime.Now <= user.LastEmailVerificationSent.AddMinutes(3))
                {
                    ViewBag.Succeeded = true;
                    ViewBag.Email = Tools.MaskEmail(user.Email!);
                    return View(model);
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
                    ModelState.AddModelError(string.Empty, "Couldn't send e-mail verification link");
                    return View(model);
                }

                user.LastEmailVerificationSent = DateTime.Now;
                await _userManager.UpdateAsync(user);

                ViewBag.Succeeded = true;
                ViewBag.Email = Tools.MaskEmail(user.Email!);

                return View(model);
            }

            await _signInManager.SignInAsync(user, model.SaveLogin);
            return Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return Redirect("/");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (User.Identity!.IsAuthenticated)
            {
                return Redirect("/");
            }

            var reCaptchaToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            if (!await _captcha.VerifyAsync(reCaptchaToken))
            {
                ModelState.AddModelError(string.Empty, "Invalid reCaptcha token");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _mapper.Map<AppUser>(model);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
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
                ModelState.AddModelError(string.Empty, "Couldn't send e-mail verification link");
                return View(model);
            }

            ViewBag.Succeeded = true;
            ViewBag.Email = Tools.MaskEmail(user.Email!);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ForgotPassword(string email)
        {
            var reCaptchaToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            if (!await _captcha.VerifyAsync(reCaptchaToken))
            {
                return Json(new { statusCode = 400, msg = "Invalid reCaptcha token" });
            }

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
            return Json(new { statusCode = 200 });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token)
        {
            var model = new ResetPasswordVM()
            {
                Token = token,
                UserId = new Guid(userId)
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            var reCaptchaToken = HttpContext.Request.Form["g-recaptcha-response"].ToString();
            if (!await _captcha.VerifyAsync(reCaptchaToken))
            {
                ModelState.AddModelError(string.Empty, "Invalid reCaptcha token");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Token))
            {
                ModelState.AddModelError(string.Empty, "The token provided is not valid");
                return View(model);
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Logout everyone with this account
            await _userManager.UpdateSecurityStampAsync(user);
            ViewBag.Succeeded = true;

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (userId == null || user == null || string.IsNullOrEmpty(token))
            {
                return View(false);
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/auth/login");
        }
    }
}
