using App.Core.Entities;
using App.Core.Interface.Services;
using QuanLy.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface
{
    public interface IUserInGroupService : IUserInGroupCoreService
    {
        Task<UserInGroupCores> GetUserInGroupByUserId(int UserId,int RoleId);
    }
}
