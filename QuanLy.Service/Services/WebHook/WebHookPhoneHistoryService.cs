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
    public class WebHookPhoneHistoryService : DomainService<WebHookPhoneHistories, BaseSearch>, IWebHookPhoneHistoryService
    {
        public WebHookPhoneHistoryService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        /// <summary>
        /// Lấy ra danh sách web hook trong 1p của tổng đài
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<WebHookPhoneHistories> GetExistWebHookPhoneAsync(string phone, string status)
        {
            DateTime ruleData = Convert.ToDateTime(DateTime.Now).Date;
            WebHookPhoneHistories webHookPhoneHistory = null;
            var webHookPhoneHistoryList = await this.unitOfWork.Repository<WebHookPhoneHistories>().GetQueryable().Where(e => !e.Deleted && e.Active
            && e.CallNumber == phone
            && e.Status == status
            && (e.Created.HasValue && e.Created.Value.Date == DateTime.Now.Date) 
            ).ToListAsync();
            DateTime currentDate = DateTime.UtcNow.AddHours(7);
            if (webHookPhoneHistoryList != null && webHookPhoneHistoryList.Any())
            {
                foreach (var item in webHookPhoneHistoryList)
                {
                    if (item.Created.HasValue && item.Created.Value.Date == currentDate.Date
                        && item.Created.Value.AddMinutes(1) >= currentDate
                        )
                    {
                        webHookPhoneHistory = item;
                        break;
                    }

                }
            }
            return webHookPhoneHistory;
        }

    }
}
