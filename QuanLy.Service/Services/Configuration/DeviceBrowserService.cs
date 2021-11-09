using App.Core.Entities.DomainEntity;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class DeviceBrowserService : DomainService<DeviceBrowsers, BaseSearch>, IDeviceBrowserService
    {
        public DeviceBrowserService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task UpdateHide(string PlayerId)
        {
            var entity = this.unitOfWork.Repository<DeviceBrowsers>().GetQueryable().FirstOrDefault(x => x.PlayerId == PlayerId);
            entity.Deleted = true;
            entity.Updated = DateTime.Now;
            entity.UpdatedBy = "Auto";
            this.unitOfWork.Repository<DeviceBrowsers>().Update(entity);
            await this.unitOfWork.SaveAsync();
            
        }
    }
}
