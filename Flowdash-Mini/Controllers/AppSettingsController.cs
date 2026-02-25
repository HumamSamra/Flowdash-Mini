using AutoMapper;
using Flowdash_Mini.Classes;
using Flowdash_Mini.Controllers;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Repositories;
using Flowdash_Mini.Services.MailService;
using Flowdash_Mini.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowdash_Mini.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = nameof(UserType.Admin))]
    public class AppSettingsController : _BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISmtpService _smtp;
        private readonly IMapper _mapper;
        public AppSettingsController(IUnitOfWork unitOfWork, IMapper mapper,
                                    ISmtpService smtp)
        {
            _smtp = smtp;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var settings = _unitOfWork.AppSettings.GetAll().ToList();
            var map = _mapper.Map<AppSettingInfoVM>(settings);
            return View(map);
        }

        [HttpPost]
        public IActionResult Index(AppSettingInfoVM model)
        {
            _unitOfWork.AppSettings.Set(nameof(model.Email), model.Email ?? "");
            _unitOfWork.AppSettings.Set(nameof(model.ContactEmail), model.ContactEmail ?? "");
            _unitOfWork.AppSettings.Set(nameof(model.Tel), model.Tel ?? "");
            return View(model);
        }

        [HttpGet]
        public IActionResult Smtp()
        {
            var settings = _unitOfWork.AppSettings.GetAll().ToArray();
            var map = _mapper.Map<AppSettingSmtpVM>(settings);
            return View(map);
        }

        [HttpPost]
        public IActionResult Smtp(AppSettingSmtpVM model)
        {
            _unitOfWork.AppSettings.Set(nameof(model.SmtpEmail), model.SmtpEmail ?? "");
            _unitOfWork.AppSettings.Set(nameof(model.SmtpName), model.SmtpName ?? "");
            if (!string.IsNullOrWhiteSpace(model.SmtpPassword))
            {
                _unitOfWork.AppSettings.Set(nameof(model.SmtpPassword), Ciphering.Encrypt(model.SmtpPassword ?? ""));
            }
            _unitOfWork.AppSettings.Set(nameof(model.SmtpSsl), model.SmtpSsl.ToString());
            _unitOfWork.AppSettings.Set(nameof(model.SmtpPort), model.SmtpPort.ToString());
            _unitOfWork.AppSettings.Set(nameof(model.SmtpHost), model.SmtpHost ?? "");
            return View(model);
        }

        [HttpGet]
        public IActionResult Socials()
        {
            var settings = _unitOfWork.AppSettings.GetAll().ToArray();
            var map = _mapper.Map<AppSettingSocialVM>(settings);
            return View(map);
        }

        [HttpPost]
        public IActionResult Socials(AppSettingSocialVM model)
        {
            _unitOfWork.AppSettings.Set(nameof(model.WhatsApp), model.WhatsApp ?? "");
            _unitOfWork.AppSettings.Set(nameof(model.Facebook), model.Facebook ?? "");
            _unitOfWork.AppSettings.Set(nameof(model.Instagram), model.Instagram ?? "");
            _unitOfWork.AppSettings.Set(nameof(model.Youtube), model.Youtube ?? "");
            _unitOfWork.AppSettings.Set(nameof(model.LinkedIn), model.LinkedIn ?? "");
            _unitOfWork.AppSettings.Set(nameof(model.Tiktok), model.Tiktok ?? "");
            return View(model);
        }

        [HttpPost]
        public JsonResult SendSmtp(string subject, string email, string message)
        {
            try
            {
                var result = _smtp.SendMail(subject, message, new List<string>() { email });
                if (result.Succeeded)
                {
                    return Json(null);
                }
                else
                {
                    return Json(new { message = result.Message });
                }
            }
            catch (Exception err)
            {
                return Json(new { message = err.Message });
            }

        }
    }
}
