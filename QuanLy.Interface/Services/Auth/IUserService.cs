using App.Core.Entities;
using App.Core.Interface;
using App.Core.Interface.Services;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface.Services
{
    public interface IUserService : IDomainService<Users, BaseSearchUser>
    {
        Task<bool> Verify(string userName, string password);

        Task<bool> HasPermission(int userId, string controller, IList<string> permissions);
        Task<string> CheckCurrentUserPassword(int userId, string password, string newPasssword);
        Task<bool> UpdateUserToken(int userId, string token, bool isLogin = false);
        Task<bool> UpdateUserPassword(int userId, string newPassword);

        Task<bool> IsInUserGroup(int userId, string userGroupCode);
    }
}
