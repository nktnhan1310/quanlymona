using App.Core.Entities.DomainEntity;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using AutoMapper;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

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
            && (string.IsNullOrEmpty(baseSearch.SearchContent.ToLower())
                || (e.Code.ToLower().Contains(baseSearch.SearchContent.ToLower())
                || e.Name.ToLower().Contains(baseSearch.SearchContent.ToLower())
                || e.ColorCode.ToLower().Contains(baseSearch.SearchContent.ToLower())
                || e.Description.ToLower().Contains(baseSearch.SearchContent.ToLower()))
                );
        }
    }
}
