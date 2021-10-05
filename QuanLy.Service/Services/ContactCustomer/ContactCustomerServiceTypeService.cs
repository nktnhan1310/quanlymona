using App.Core.Entities.DomainEntity;
using App.Core.Interface;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Service
{
    public class ContactCustomerServiceTypeService : BaseContactCustomerService<ContactCustomerServices>, IContactCustomerServiceTypeService
    {
        public ContactCustomerServiceTypeService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
