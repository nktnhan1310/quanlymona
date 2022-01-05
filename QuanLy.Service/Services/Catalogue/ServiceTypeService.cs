using App.Core.Entities.DomainEntity;
using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface.Services.Catalogue;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service.Services.Catalogue
{
    public class ServiceTypeService : CatalogueService<ServiceTypes, BaseSearch>, IServiceTypes
    {
        protected readonly IAppDbContext Context;
        public ServiceTypeService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext context) : base(unitOfWork, mapper)
        {
            this.Context = context;
            this.IsUseStore = true;
        }
        protected override string GetStoreProcName()
        {
            return "Mona_Load_ServiceType";
        }
        public override async Task<string> GetExistItemMessage(ServiceTypes item)
        {
            string result = string.Empty;
            if(string.IsNullOrEmpty(item.Code) || string.IsNullOrEmpty(item.Name))
            {
                return "Thông tin không được để trống";
            }
            bool isExistCode = await this.unitOfWork.Repository<ServiceTypes>().GetQueryable().AnyAsync(x => x.Id != item.Id 
            && x.Code == item.Code && !x.Deleted && x.Active);
            if (isExistCode)
                return "Mã đã tồn tại!";
            return result;
        }

        public async Task<bool> CheckServiceTypeExistProjectService(int id)
        {
            return await this.unitOfWork.Repository<ProjectServices>().GetQueryable().AnyAsync(x => x.ServiceTypeId == id && !x.Deleted && x.Active);
        }
    }
}
