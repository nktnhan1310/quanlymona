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
    public class PermitObjectService : PermitObjectCoreService, IPermitObjectService
    {
        public PermitObjectService(IAppUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
        }
    }
}
