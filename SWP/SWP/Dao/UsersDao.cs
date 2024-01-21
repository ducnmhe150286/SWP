using Microsoft.AspNetCore.Identity;
using SWP.Dto;
using SWP.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System;

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
                //var data = context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
                //if (data == null)
                //{
                //    return null;
                //}
                //return data;
                var user = context.Users.FirstOrDefault(x => x.Email == email);
                if (user == null)
                {
                    return null;
                }
                string hashedInputPassword = HashPassword(password);

                if (hashedInputPassword == user.Password)
                {

                    return user;
                }

                return null;
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
                //user.Password = registerModel.Password;
                user.Password = HashPassword(registerModel.Password);
                user.PhoneNumber = registerModel.PhoneNumber;
                user.FirstName = registerModel.FirstName;
                user.LastName = registerModel.LastName;
                user.RoleId = registerModel.RoleId??2;
                user.CreatedDate = DateTime.Now;
                user.Status = 1;
                var check = context.Users.FirstOrDefault(x => x.Email == registerModel.Email);
                if (check != null)
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

        public async Task<bool> ForgotPassword(string email)
        {
            try
            {
                string otp = RandomString(6);
                var data = context.Users.FirstOrDefault(x => x.Email == email);

                if(data == null)
                {
                    return false;
                }
                data.Otp = otp;
                data.OtpExpired = DateTime.Now.AddMinutes(5);
                context.SaveChanges();

                await SendEmailAsync(email, "Mã xác nhận", $"<p>Mã xác nhận của bạn - <b>{otp}</b>.</p>");
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public string Confirm(string email, string otp)
        {
            try
            {

                var data = context.Users.FirstOrDefault(x => x.Email == email);

                if (data == null)
                {
                    return "Email không tồn tại!";
                }
                if (data.OtpExpired < DateTime.Now) return "Mã xác nhận hết hạn!";
                if (data.Otp != otp) return "Mã xác nhận không đúng!";
               
                return "";
            }
            catch (Exception)
            {

                throw;
            }

        }

        public string resetPassword(string email, string password)
        {
            try
            {

                var data = context.Users.FirstOrDefault(x => x.Email == email);

                if (data == null)
                {
                    return "Email không tồn tại!";
                }
                string hashedInputPassword = HashPassword(password);
                data.Password = hashedInputPassword;
                context.SaveChanges();

                return "";
            }
            catch (Exception)
            {

                throw;
            }

        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string content)
        {
            try
            {
                string from = "phucnvhe1@gmail.com";
                string pass = "sqch zqjq mruf kjog";
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                mail.IsBodyHtml = true;
                mail.To.Add(toEmail);
                mail.From = new MailAddress(from);
                mail.Subject = subject;
                mail.Body = "Boxing Shop:" + content;
                smtp.EnableSsl = true;
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(from, pass);
                await smtp.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static List<User> GetAllUser()
        {
            try
            {
                using(var connection = new SWPContext())
                {
                    var list = connection.Users.ToList();
                    return list;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            
        }
        public static User GetUserById(int id)
        {
            try
            {
                using(var connection = new SWPContext())
                {
                    var user = connection.Users.Where(x => x.UserId== id).FirstOrDefault();
                    return user;
                }
            }catch(Exception ex)
            {
                return null;
            }
        }
        public static void SaveUser(User user)
        {
            try
            {
                using(var context = new SWPContext())
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void UpdateUser(User user)
        {
            try
            {
                using (var context = new SWPContext())
                {
                    context.Entry<User>(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void DeleteUser(User user)
        {
            try
            {
                using (var context = new SWPContext())
                {
                   var p1 = context.Users.SingleOrDefault(c=> c.UserId == user.UserId);
                    context.Users.Remove(p1);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        

    }
}
