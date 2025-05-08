using SharpBlog.Models;

namespace SharpBlog.Data.Repository;

public interface IUserRepo
{
    void Create(User user);
    void Delete(User user);
    User GetUser(string username);
    IEnumerable<User> GetAll();
    void Save();
}
