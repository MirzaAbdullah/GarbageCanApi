using GarbageCanApi.Models;
using GarbageCanApi.Models.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Interfaces
{
    public interface ISecurity
    {
        string GenerateJSONWebToken(User userModel);
        User AuthenticateUser(UserViewModel userModel);
        bool VerifyUser(string userEmailId, int verificationCode);
        bool SendVerificationEmail(string userEmail);
        bool ChangePassword(string userId, string newPassword);
        bool ForgetPassword(string userEmail);
        User CreateUser(UserViewModel userModel);
        IEnumerable<UserViewModel> GetAllUsers();
        UserViewModel GetUserById(string userId);
        UserViewModel GetUserByRoleId(int roleId);
        bool UpdateUserDetails(User userModel);
        bool IsUserEmailExists(string userEmailId);
        bool IsUserNameExists(string userName);
        bool IsPasswordValid(string userId, string password);
    }
}
