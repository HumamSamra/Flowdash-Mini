namespace Flowdash_Mini.Classes
{
    public static class CookieHandler
    {
        /// <summary>
        /// Saves the requested string to a secure cookie for 1 year.
        /// </summary>
        public static void Set(string c, HttpContext context, string value, int expireDays = 30)
        {
            if (context == null) return;

            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            };

            context.Response.Cookies.Append(c, value, options);
        }

        /// <summary>
        /// Retrieves the requested Cookie. Returns null if not found.
        /// </summary>
        public static string? Get(string c, HttpContext context)
        {
            if (context == null) return null;

            return context.Request.Cookies[c];
        }

        /// <summary>
        /// Deletes the cookie and returns the value that was held (if any).
        /// </summary>
        public static string? Delete(string c, HttpContext context)
        {
            if (context == null) return null;

            var existingValue = context.Request.Cookies[c];

            context.Response.Cookies.Delete(c);

            return existingValue;
        }
    }
}
