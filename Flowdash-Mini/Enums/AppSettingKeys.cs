using System.ComponentModel.DataAnnotations;

namespace Flowdash_Mini.Enums
{
    public enum AppSettingKeys
    {
        [Display(Name = "Smtp Email")]
        SmtpEmail = 0,

        [Display(Name = "Smtp Name")]
        SmtpName = 1,

        [Display(Name = "Smtp Host")]
        SmtpHost = 2,

        [Display(Name = "Smtp Port")]
        SmtpPort = 3,

        [Display(Name = "Smtp Ssl")]
        SmtpSsl = 4,

        [Display(Name = "Smtp Password")]
        SmtpPassword = 5,

        [Display(Name = "Tel")]
        Tel = 6,

        [Display(Name = "Email")]
        Email = 7,

        [Display(Name = "ContactEmail")]
        ContactEmail = 8,

        [Display(Name = "WhatsApp")]
        WhatsApp = 9,

        [Display(Name = "Facebook")]
        Facebook = 10,

        [Display(Name = "Instagram")]
        Instagram = 11,

        [Display(Name = "Youtube")]
        Youtube = 12,

        [Display(Name = "LinkedIn")]
        LinkedIn = 13,

        [Display(Name = "Tiktok")]
        Tiktok = 14
    }
}
