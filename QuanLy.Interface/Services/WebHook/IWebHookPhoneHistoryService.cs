using App.Core.Entities.DomainEntity;
using App.Core.Interface;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface
{
    public interface IWebHookPhoneHistoryService : IDomainService<WebHookPhoneHistories, BaseSearch>
    {
        Task<WebHookPhoneHistories> GetExistWebHookPhoneAsync(string phone, string status);
    }
}
