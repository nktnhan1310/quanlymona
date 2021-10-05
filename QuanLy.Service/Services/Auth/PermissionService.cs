using App.Core.Interface.Services;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Service
{
    public class PermissionService : PermissionCoreService, IPermissionService
    {
        public PermissionService(IAppUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
        }
    }
}
