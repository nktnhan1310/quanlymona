using App.Core.Entities;
using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities.Auth;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class UserInGroupService : UserInGroupCoreService, IUserInGroupService
    {
        protected IAppDbContext context;
        public UserInGroupService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext context) : base(unitOfWork, mapper)
        {
            this.context = context;
        }

        public async Task<UserInGroupCores> GetUserInGroupByUserId(int UserId)
        {
            return  await unitOfWork.Repository<UserInGroupCores>().GetQueryable().FirstOrDefaultAsync(x=>x.UserId == UserId);
        }
    }
}
