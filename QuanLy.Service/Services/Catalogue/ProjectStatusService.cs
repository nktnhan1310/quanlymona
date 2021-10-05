﻿using App.Core.Entities.DomainEntity;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using AutoMapper;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Service
{
    public class ProjectStatusService : CatalogueService<ProjectStatuses, BaseSearch>, IProjectStatusService
    {
        public ProjectStatusService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}