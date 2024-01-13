using Microsoft.AspNetCore.Identity;
using SWP.Dto;
using SWP.Models;
using System.Security.Cryptography;
using System.Text;

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
                var data = context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
                if (data == null)
                {
                    return null;
                }
                return data;           
            //var user = context.Users.FirstOrDefault(x => x.Email == email);
            //if (user == null)
            //{
            //    return null;
            //}
            //string hashedInputPassword = HashPassword(password);

            //if (hashedInputPassword == user.Password)
            //{

            //    return user;
            //}

            //return null; 
        }
            catch (Exception)
            {

                throw;
            }
            
        }

        public bool Register(RegisterModel registerModel)
        {
            try
            {
                var user = new User();
                user.Email = registerModel.Email;
                user.Password = registerModel.Password;
                //user.Password = HashPassword(registerModel.Password);
                user.PhoneNumber = registerModel.PhoneNumber;
                user.FirstName = registerModel.FirstName;
                user.LastName = registerModel.LastName;
                user.RoleId = 2;
                user.CreatedDate = DateTime.Now;
                var check = context.Users.FirstOrDefault(x=>x.Email == registerModel.Email);
                if(check != null)
                {
                    return false;
                }
                context.Users.Add(user);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        
    }
}
