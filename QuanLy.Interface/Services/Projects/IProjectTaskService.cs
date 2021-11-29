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
    }
}
