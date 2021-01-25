using GarbageCanApi.Interfaces;
using GarbageCanApi.Models;
using GarbageCanApi.Models.ViewModels;
using GarbageCanApi.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GarbageCanApi.Implementations
{
    public class ImplSecurity : ISecurity
    {
        private readonly databaseIEContext DbContext;
        private readonly IConfiguration configuration;
        private readonly Email email;
        public ImplSecurity(databaseIEContext DbContext, IConfiguration configuration, Email email)
        {
            this.DbContext = DbContext;
            this.configuration = configuration;
            this.email = email;
        }

        public User AuthenticateUser(UserViewModel userModel)
        {
            if (userModel == null)
            {
                return null;
            }

            return DbContext.Users.Where(user => user.Email == userModel.Email && user.Password == Encryption.Encrypt(userModel.Password)).SingleOrDefault();
        }

        public bool ChangePassword(string userId, string newPassword)
        {
            var user = DbContext.Users.Where(user => user.IdUser == userId).SingleOrDefault();

            if (user != null)
            {
                //Set new password for user
                user.Password = Encryption.Encrypt(newPassword);

                //Update in DB
                DbContext.Users.Update(user);
                DbContext.SaveChanges();

                return true;
            }

            return false;
        }

        public User CreateUser(UserViewModel userModel)
        {
            try
            {
                //Adding new Id
                var id = Guid.NewGuid().ToString("N");
                userModel.IdUser = id;

                //Encrypting Password
                userModel.Password = Encryption.Encrypt(userModel.Password);

                //Set to false because user will verify through email
                userModel.IsVerified = false;

                //Adding Role as Admin & Date
                userModel.CreatedDate = DateTime.Now;

                var syncedModel = SyncUserAndUserViewModel(userModel);

                //Adding to database
                DbContext.Users.Add(syncedModel);
                var count = DbContext.SaveChanges();

                if (count > 0)
                {
                    //Send Welcome Email
                    var body = @"Congratulations, you are now a member of GarbageCAN (Pvt.) Ltd. <br />
                             We are a sustainable waste management company. We want to take Karachi into a new era of waste management with a focus on environmentally friendly waste management practices such as reduce, reuse, and recycle. For us, sustainability is key. <br />
                             We at GarbageCAN believe that the garbage CAN be cleaned, and the garbage CAN be recycled. In the wise words of Oscar the Grouch from Sesame Street: ‘Its called Garbage CAN, not Garbage CANNOT.’";

                    EmailViewModel emailObj = new EmailViewModel()
                    {
                        to = userModel.Email,
                        subject = "Welcome",
                        body = email.htmlEmailBody(body)
                    };

                    email.sendEmail(emailObj);

                    return DbContext.Users.Where(user => user.IdUser == id).SingleOrDefault();
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private User SyncUserAndUserViewModel(UserViewModel userViewModel)
        {
            return new User
            {
                IdUser = userViewModel.IdUser,
                Email = userViewModel.Email,
                Password = userViewModel.Password,
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                Name = userViewModel.Name,
                PhoneNo = userViewModel.PhoneNo,
                CreatedDate = userViewModel.CreatedDate,
                IdRole = userViewModel.IdRole,
                IsVerified = userViewModel.IsVerified,
                CodeVerification = userViewModel.VerificationCode
            };
        }

        public bool ForgetPassword(string userEmail)
        {
            var user = DbContext.Users.Where(user => user.Email == userEmail).SingleOrDefault();

            //If user Exists, send email
            if (user != null)
            {
                //Create Random number as temporary password
                var rand = new Random().Next(0, 1000000).ToString();

                //update password
                user.Password = Encryption.Encrypt(rand);
                DbContext.Users.Update(user);
                DbContext.SaveChanges();

                //Send Email
                EmailViewModel emailObj = new EmailViewModel()
                {
                    to = userEmail,
                    subject = "Temporary Password",
                    body = email.htmlEmailBody(string.Format("Your temporary password is : {0} , please change your password within 2 days.", rand))
                };

                return email.sendEmail(emailObj);
            }

            return false;
        }

        public string GenerateJSONWebToken(User userModel)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, userModel.Email),
                new Claim("userId", userModel.IdUser),
                new Claim("userName", userModel.Name),
                new Claim("uRoleId", userModel.IdRole.ToString())
            };

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
              configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public IEnumerable<UserViewModel> GetAllUsers()
        {
            return DbContext
                .Users
                .Include(user => user.IdRoleNavigation)
                .Where(user => user.IsVerified == true && user.IdRole != (int) EnumRoles.Roles.Admin)
                .Select(user => new UserViewModel
                {
                    IdUser = user.IdUser,
                    Email = user.Email,
                    Name = user.Name,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = string.Format("{0} {1}", user.FirstName, user.LastName),
                    PhoneNo = user.PhoneNo,
                    IsVerified = user.IsVerified,
                    CreatedDate = user.CreatedDate,
                    IdRole = user.IdRole,
                    NameRole = user.IdRoleNavigation.RoleName
                });
        }

        public UserViewModel GetUserById(string userId)
        {
            return DbContext
                .Users
                .Include(user => user.IdRoleNavigation)
                .Where(user => user.IsVerified == true && user.IdRole != (int)EnumRoles.Roles.Admin)
                .Select(user => new UserViewModel
                {
                    IdUser = user.IdUser,
                    Email = user.Email,
                    Name = user.Name,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = string.Format("{0} {1}", user.FirstName, user.LastName),
                    PhoneNo = user.PhoneNo,
                    IsVerified = user.IsVerified,
                    CreatedDate = user.CreatedDate,
                    IdRole = user.IdRole,
                    NameRole = user.IdRoleNavigation.RoleName
                }).SingleOrDefault();
        }

        public bool SendVerificationEmail(string userEmail)
        {
            var user = DbContext.Users.Where(user => user.Email == userEmail).SingleOrDefault();

            //If User Account is approved
            if (user != null)
            {
                //Create Random number
                var rand = new Random().Next(0, 1000000);

                //Update against user record
                user.CodeVerification = rand;
                DbContext.Users.Update(user);
                DbContext.SaveChanges();

                //Send Email
                EmailViewModel emailObj = new EmailViewModel()
                {
                    to = userEmail,
                    subject = "Verify your account",
                    body = email.htmlEmailBody(string.Format("Verification Code: {0}", rand))
                };

                return email.sendEmail(emailObj);
            }

            return false;
        }

        public bool UpdateUserDetails(User userModel)
        {
            var user = DbContext.Users.Where(user => user.IdUser == userModel.IdUser).SingleOrDefault();

            if (user != null)
            {
                //Update in DB
                DbContext.Users.Update(user);
                DbContext.SaveChanges();

                return true;
            }

            return false;
        }

        public bool VerifyUser(string userEmailId, int verificationCode)
        {
            var user = DbContext.Users.Where(user => user.Email == userEmailId && user.CodeVerification == verificationCode).SingleOrDefault();

            if (user != null)
            {
                //Set isUserVerified Flag to true
                user.IsVerified = true;

                //Update the User Records in DB
                DbContext.Users.Update(user);
                DbContext.SaveChanges();

                //Send Welcome Email
                EmailViewModel emailObj = new EmailViewModel()
                {
                    to = user.Email,
                    subject = "Account Verified",
                    body = email.htmlEmailBody("Congratulations. Your account is verified successfully. We hope you will add values to GarbageCan.")
                };

                email.sendEmail(emailObj);

                return true;
            }

            return false;
        }

        public bool IsUserEmailExists(string userEmailId)
        {
            var isUserExists = DbContext.Users.Where(user => user.Email == userEmailId.ToLower().Trim()).SingleOrDefault();

            if (isUserExists != null)
            {
                return true;
            }

            return false;
        }

        public bool IsUserNameExists(string userName)
        {
            var isUserNameExists = DbContext.Users.Where(user => user.Name == userName.ToLower().Trim()).SingleOrDefault();

            if (isUserNameExists != null)
            {
                return true;
            }

            return false;
        }

        public UserViewModel GetUserByRoleId(int roleId)
        {
            return DbContext
                .Users
                .Include(user => user.IdRoleNavigation)
                .Where(user => user.IsVerified == true && user.IdRole == roleId)
                .Select(user => new UserViewModel
                {
                    IdUser = user.IdUser,
                    Email = user.Email,
                    Name = user.Name,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = string.Format("{0} {1}", user.FirstName, user.LastName),
                    PhoneNo = user.PhoneNo,
                    IsVerified = user.IsVerified,
                    CreatedDate = user.CreatedDate,
                    IdRole = user.IdRole,
                    NameRole = user.IdRoleNavigation.RoleName
                }).SingleOrDefault();
        }
    }
}
