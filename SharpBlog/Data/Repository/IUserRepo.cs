using SharpBlog.Models;

namespace SharpBlog.Data.Repository;

public interface IUserRepo
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User> GetUserByEmail(string email);
    Task<User> GetUserById(int id);
    Task UpdateUser(User user);
    Task CreateUser(User user);
    Task DeleteUser(User user);
}
