namespace Flowdash_Mini.Classes
{
    public static class Tools
    {
        public static string MaskEmail(string email)
        {
            var parts = email.Split('@');
            if (parts.Length != 2) return email;

            string name = parts[0];
            string domain = parts[1];

            if (name.Length <= 2)
                name = name.Substring(0, 1) + "****";
            else
                name = name.Substring(0, 2) + "****";

            return name + "@" + domain;
        }

        public static string GenerateRandomString(int length = 8)
        {
            const string chars = "ABCDEFGHKLMNOQRSTUVWXYZabcdefghklmnoqrstuvwxyz1234567890";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
