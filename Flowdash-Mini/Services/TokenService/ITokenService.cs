using Flowdash_Mini.Models.Accounts;

namespace Flowdash_Mini.Services.TokenService
{
    public interface ITokenService
    {
        Task<string> CreateAsync(AppUser user);
    }
}
