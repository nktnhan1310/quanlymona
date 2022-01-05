using App.Core.Entities.DomainEntity;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class ProjectSessionPayService : DomainService<ProjectSessionPays, SearchProjectSessionPay>, IProjectSessionPayService
    {
        public ProjectSessionPayService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public override async Task<PagedList<ProjectSessionPays>> GetPagedListData(SearchProjectSessionPay baseSearch)
        {
            PagedList<ProjectSessionPays> pagedList = new PagedList<ProjectSessionPays>();

            var getListData = await this.unitOfWork.Repository<ProjectSessionPays>().GetQueryable().Where(
                x => !x.Deleted &&
                x.ProjectId == baseSearch.ProjectId &&
                x.IsDone == baseSearch.IsDone
                ).ToListAsync();
            var Total = getListData.Count();
            if(Total > 0)
            {
                pagedList.Items = getListData;
                pagedList.TotalItem = Total;
                pagedList.PageIndex = 1;
                pagedList.PageSize = 1;
            }
            return pagedList;
        }
        public override async Task<string> GetExistItemMessage(ProjectSessionPays item)
        {
            string message = "";
            if(item.Id != 0) 
            {
                var checkExistPrjectSessionPay = await this.unitOfWork.Repository<ProjectSessionPays>().GetQueryable().Where(x => x.Id == item.Id && !x.Deleted).AnyAsync();
                if (!checkExistPrjectSessionPay)
                    message = "item không tồn tại";
            }
            var checkExistProject = await this.unitOfWork.Repository<Projects>().GetQueryable().Where(x => x.Id == item.ProjectId && !x.Deleted).AnyAsync();
            if (!checkExistProject)
                message = "Dự án không tồn tại";
            return message;
        }
    }
}
