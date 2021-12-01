using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Interface;
using QuanLy.Utilities;
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
            entity.UpdatedBy = Contants.UPDATE_BY_SEVER;
            this.unitOfWork.Repository<DeviceBrowsers>().Update(entity);
            await this.unitOfWork.SaveAsync();

        }

        public async Task<string> InsertOrUpdatePlayerId(DeviceBrowsers entity)
        {
            var message = "";
            var getData = await this.unitOfWork.Repository<DeviceBrowsers>().GetQueryable()
                .Where(x => 
                x.UserId == entity.UserId &&
                x.PlayerId == entity.PlayerId)
                .FirstOrDefaultAsync();
            if (getData != null)
            {
                getData.Deleted = false;
                getData.Updated = DateTime.Now;
                getData.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                this.unitOfWork.Repository<DeviceBrowsers>().Update(getData);
            }
            else this.unitOfWork.Repository<DeviceBrowsers>().Create(entity);
            await this.unitOfWork.SaveAsync();
            return message;
        }
    }
}
