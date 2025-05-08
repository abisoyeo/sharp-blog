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

        public void Create(User user)
        {
            _db.Users.Add(user);
            Save();
        }

        public void Delete(User user)
        {
            _db.Users.Remove(user);
            Save();
        }

        public User GetUser(string username)
        {
            return _db.Users.FirstOrDefault(u => u.Email == username);
        }

        public IEnumerable<User> GetAll()
        {
            return _db.Users.ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
