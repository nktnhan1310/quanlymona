using App.Core.Entities.DomainEntity;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class TaskTypeService : CatalogueService<TaskTypes, BaseSearch>, ITaskTypeService
    {
        public TaskTypeService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override Expression<Func<TaskTypes, bool>> GetExpression(BaseSearch baseSearch)
        {
            return e => !e.Deleted
            && (string.IsNullOrEmpty(baseSearch.SearchContent)
                || (e.Code.ToLower().Contains(baseSearch.SearchContent.ToLower())
                || e.Name.ToLower().Contains(baseSearch.SearchContent.ToLower())
                || e.ColorCode.ToLower().Contains(baseSearch.SearchContent.ToLower())
                || e.Description.ToLower().Contains(baseSearch.SearchContent.ToLower()))
                );
        }

        public override async Task<string> GetExistItemMessage(TaskTypes item)
        {
            var message = "";
            var isExists = await this.unitOfWork.Repository<TaskTypes>().GetQueryable()
                .Where(x =>
                x.Id != item.Id &&
                x.Code == item.Code)
                .AnyAsync();
            if (isExists)
            {
                message = "Loại task đã tồn tại";
            }
            return message;
        }
    }
}
