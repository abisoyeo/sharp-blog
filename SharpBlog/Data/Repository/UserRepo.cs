using Microsoft.EntityFrameworkCore;
using SharpBlog.Models;

namespace SharpBlog.Data.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly BlogDbContext _db;

        public UserRepo(BlogDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserById(int id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task CreateUser(User user)
        {
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteUser(User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }
}
