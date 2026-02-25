using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Flowdash_Mini.Extensions
{
    public static class DateFormatter
    {
        public static void ConfigureDateFormat(this WebApplication app, string format)
        {
            var defaultDateCulture = "en-GB";
            var ci = new CultureInfo(defaultDateCulture);

            ci.DateTimeFormat.ShortDatePattern = format;
            ci.DateTimeFormat.DateSeparator = "/";

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ci),
                SupportedCultures = new List<CultureInfo> { ci },
                SupportedUICultures = new List<CultureInfo> { ci }
            });
        }
    }
}
