﻿using Microsoft.AspNetCore.Mvc;
using SWP.Dto;
using SWP.Models;
using System.Security.Cryptography;
using System.Text;

namespace SWP.Dao
{
    public class ManageUsersDao
    {
        SWPContext context;
        public ManageUsersDao()
        {
            context = new SWPContext();
        }

        public List<User> GetAllUsers()
        {
            try
            {
                var data = context.Users.Where(x=>x.RoleId == 2).ToList();
                return data;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public User getUser(int id)
        {
            try
            {
                var data = context.Users.FirstOrDefault(x => x.UserId==id);
                return data;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public User getUserByEmail(string email)
        {
            try
            {
                var data = context.Users.FirstOrDefault(x => x.Email == email);
                return data;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public User updateProfile(UserProfileModel userProfile,string email)
        {
            try
            {
                var data = context.Users.FirstOrDefault(x => x.Email == email);
                if(data == null)
                {
                    return null;
                }
                data.FirstName = userProfile.FirstName;
                data.LastName = userProfile.LastName;
                data.PhoneNumber = userProfile.PhoneNumber;
                data.Address = userProfile.Address;
                data.Gender = userProfile.Gender;
                data.UpdatedBy = email;
                context.SaveChanges();
                return data;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public bool checkAddImage(string image, string email)
        {
            try
            {
                var data = context.Users.FirstOrDefault(x => x.Email == email);
                if (data == null)
                {
                    return false;
                }
               
                data.Image = image;
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public string changePassWord(ChangePassWordModel passWordModel,string email)
        {
            try
            {

                var data = context.Users.FirstOrDefault(x => x.Email == email);
                if(passWordModel.Password == null || data.Password!= HashPassword(passWordModel.Password))
                {
                    return "Mật khẩu cũ không dúng";
                }
                if (data == null)
                {
                    return "Email không tồn tại!";
                }
                string hashedInputPassword = HashPassword(passWordModel.NewPassword);
                data.Password = hashedInputPassword;
                context.SaveChanges();

                return "";
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
        public string BlockUser(int? id,int? block,string updateby)
		{
			try
			{
				var data = context.Users.FirstOrDefault(x => x.UserId == id);
                if(data==null)
                {
                    return "Không có giá trị nào.";

                }
                if (block == 1)
                {
                    data.Status = 2;
                    data.UpdatedBy = updateby.ToString();
                    context.SaveChanges();
                    return "Chặn thành công.";
                }
                else
                {
                    data.Status = 1;

                    data.UpdatedBy = updateby.ToString();
                    context.SaveChanges();
                    return "Bỏ chặn thành công.";

                }
               
			}
			catch (Exception ex)
			{

				return ex.Message;
			}
		}
		public bool CheckAdmin(string email)
        {
            try
            {
                var data = context.Users.FirstOrDefault(x => x.Email == email);

                if(data.RoleId !=1||data==null ) {

                    return false;
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
