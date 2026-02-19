using Flowdash_Mini.Models.Accounts;

namespace Flowdash_Mini.Repositories.Invites
{
    public interface IInviteRepo
    {
        IQueryable<UserInvite> GetAll(Guid userId);
        IQueryable<UserInvite> GetAllByProjectCode(string code);
        UserInvite? Get(Guid id);
        void Add(UserInvite invite);
        void Delete(Guid id);
    }
}
