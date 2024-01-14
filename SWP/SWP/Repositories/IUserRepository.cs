using SWP.Models;

namespace SWP.Repositories
{
    public interface IUserRepository
    {
        List<User> GetAllUser();
        User GetUserById(int id);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
    }
}
