using Flowdash_Mini.Context;
using Flowdash_Mini.Models.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Flowdash_Mini.Repositories.Invites
{
    public class InviteRepo : IInviteRepo
    {
        private readonly AppDbContext _context;
        public InviteRepo(AppDbContext context)
        {
            _context = context;
        }

        public void Add(UserInvite invite)
        {
            invite.CreatedAt = DateTime.UtcNow;
            _context.UserInvites.Add(invite);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var item = Get(id);
            if (item != null)
            {
                _context.UserInvites.Remove(item);
                _context.SaveChanges();
            }
        }

        public UserInvite? Get(Guid id)
            => _context.UserInvites
            .Include(e => e.User)
            .Include(e => e.Project)
            .FirstOrDefault(i => i.Id == id);

        public IQueryable<UserInvite> GetAll(Guid userId)
            => _context.UserInvites
            .Include(e => e.User)
            .Include(e => e.Project)
            .Where(i => i.UserId == userId);

        public IQueryable<UserInvite> GetAllByProjectCode(string code)
            => _context.UserInvites
                .Include(e => e.User)
                .Include(e => e.Project)
                .Where(i => i.Project.ProjectCode == code);
    }
}
