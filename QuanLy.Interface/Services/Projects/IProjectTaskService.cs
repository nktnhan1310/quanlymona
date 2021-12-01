using App.Core.Entities.DomainEntity;
using App.Core.Interface;
using App.Core.Utilities;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface
{
    public interface IProjectTaskService : IDomainService<ProjectTasks, SearchProjectTask>
    {
        Task UpdateTaskEffect(ProjectTasks item, bool isUpdated = false, bool isDeleted = false);

        Task<PagedList<ProjectTasks>> GetPagedListTaskUser(int UserId, SearchProjectTask searchProject);
        /// <summary>
        /// Cập nhật status của projectask theo định kỳ 1 ngày
        /// </summary>
        /// <returns></returns>
        Task JobProjectTask();
        /// <summary>
        /// Update Status Task
        /// </summary>
        /// <param name="TaskId"></param>
        /// <param name="StatusDay">2 đang làm , 1 xog , 3 trễ</param>
        /// <returns></returns>
        Task UpdateStatusTask(int TaskId, int StatusDay);

        Task <string> UpdateTaskInfinity(int TaskId, bool Infinity);
    }
}
