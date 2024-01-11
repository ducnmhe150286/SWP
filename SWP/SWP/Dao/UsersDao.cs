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

    }
}
