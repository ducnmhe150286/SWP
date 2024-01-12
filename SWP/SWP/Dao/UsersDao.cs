using SWP.Dto;
using SWP.Models;

namespace SWP.Dao
{
    public class UsersDao
    {
        SWPContext context;
        public UsersDao()
        {
            context = new SWPContext();

        }
        public User login(string email, string password)
        {
            try
            {
                var data = context.Users.FirstOrDefault(x=>x.Email == email && x.Password == password);
                if(data == null)
                {
                    return null;
                }
                return data;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public User Register(RegisterModel registerModel)
        {
            try
            {
                var user = new User();
                user.Email = registerModel.Email;
                user.Password = registerModel.Password;
                user.PhoneNumber = registerModel.PhoneNumber;
                user.FirstName = registerModel.FirstName;
                user.LastName = registerModel.LastName;

                context.Users.Add(user);
                context.SaveChanges();
                return user;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
