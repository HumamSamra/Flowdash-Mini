namespace Flowdash_Mini.Services.CaptchaService
{
    public interface ICaptchaService
    {
        Task<bool> VerifyAsync(string token);
    }
}
