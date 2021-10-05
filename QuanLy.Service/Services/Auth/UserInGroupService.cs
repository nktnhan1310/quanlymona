using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Service
{
    public class UserInGroupService : UserInGroupCoreService, IUserInGroupService
    {
        public UserInGroupService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
