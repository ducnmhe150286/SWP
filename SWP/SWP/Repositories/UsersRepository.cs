using SWP.Dao;
using SWP.Models;

namespace SWP.Repositories
{
    public class UsersRepository : IUserRepository
    {
        public void AddUser(User user) => UsersDao.SaveUser(user);
        public void DeleteUser(User user) => UsersDao.DeleteUser(user);
        public void UpdateUser(User user) => UsersDao.UpdateUser(user);
        public User GetUserById(int id) => UsersDao.GetUserById(id);
        public List<User> GetAllUser() => UsersDao.GetAllUser();
    }
}
