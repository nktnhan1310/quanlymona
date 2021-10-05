using App.Core.Entities.DomainEntity;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
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
    public class ContactCustomerMappingRequestService : BaseContactCustomerService<ContactCustomerMappingRequests>, IContactCustomerMappingRequestService
    {
        public ContactCustomerMappingRequestService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<string> GetExistItemMessage(ContactCustomerMappingRequests item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;

            bool isExistMappingRequest = await this.unitOfWork.Repository<ContactCustomerMappingRequests>().GetQueryable()
                .AnyAsync(e => !e.Deleted && e.Active && e.Id != item.Id && e.RequestId == item.RequestId && e.SourceId == item.SourceId);
            if (isExistMappingRequest)
                messages.Add("Yêu cầu đã tồn tại");

            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

    }
}
