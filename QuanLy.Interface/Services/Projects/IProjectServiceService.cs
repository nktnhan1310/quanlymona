using App.Core.Interface;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface
{
    public interface IProjectServiceService : IDomainService<ProjectServices, SearchProject>
    {
        /// <summary>
        /// Cập nhật dịch vụ gần hết hạn
        /// </summary>
        /// <returns></returns>
        Task UpdateServiceExprireDate();
        Task<List<ProjectServices>> Mona_sp_Load_ProjectService_EndDate();
    }
}
