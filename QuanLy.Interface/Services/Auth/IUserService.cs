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

        /// <summary>
        /// Lấy danh sach user Leader
        /// </summary>
        /// <returns></returns>
        Task<List<Users>> Mona_sp_LoadUser_Role_Leader();

        /// <summary>
        /// Lấy danh sach user chăm sóc khách hàng
        /// </summary>
        /// <returns></returns>
        Task<List<Users>> Mona_sp_LoadUser_Role_CSKH();

        /// <summary>
        /// Láy danh sách User Leader và Manager
        /// </summary>
        /// <returns></returns>
        Task<List<Users>> Mona_sp_LoadUser_Role_LeaderAndManager();
    }
}
