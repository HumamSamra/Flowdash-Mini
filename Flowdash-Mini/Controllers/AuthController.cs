using AutoMapper;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Models.Accounts;
using Flowdash_Mini.Services.CaptchaService;
using Flowdash_Mini.Services.MailService;
using Flowdash_Mini.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Al_Madinah_Reparing.Controllers
{
    public class AuthController : Controller
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
                ModelState.AddModelError("", "Invalid reCaptcha token");
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
                ModelState.AddModelError("", "Username or Password is not correct");
                return View(model);
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Username or Password is not correct");
                return View(model);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError("", "This account is disabled, please contact us for more info");
                return View(model);
            }

            await SignInManager.SignInAsync(user, model.SaveLogin);
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
                ModelState.AddModelError("", "Invalid reCaptcha token");
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
                    ModelState.AddModelError("", err.Description);
                }
                return View(model);
            }
            await _userManager.AddToRoleAsync(user, nameof(UserType.User));

            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
