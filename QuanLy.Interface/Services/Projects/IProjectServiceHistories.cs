using App.Core.Interface;
using App.Core.Utilities;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface
{
    public interface IProjectServiceHistories : IDomainService<Entities.ProjectServiceHistories, SearchProject>
    {
        /// <summary>
        /// Get item ProjectServiceHistories
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        Task<ProjectServiceHistories> DetailProjectServiceHistories(int Id);

        /// <summary>
        /// List Project Serivice Histories By ProjectServiceID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        Task<List<ProjectServiceHistories>> ListProjectServiceHistories(int ProjectService);
        
        
    }
}
