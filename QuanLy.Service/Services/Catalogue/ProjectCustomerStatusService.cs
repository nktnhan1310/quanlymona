using App.Core.Entities.DomainEntity;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using AutoMapper;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Service
{
    public class ProjectCustomerStatusService : CatalogueService<ProjectCustomerStatuses, BaseSearch>, IProjectCustomerStatusService
    {
        public ProjectCustomerStatusService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
