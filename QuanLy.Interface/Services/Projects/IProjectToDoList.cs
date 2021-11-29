using App.Core.Interface;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface
{
    public interface IProjectToDoList : IDomainService<ProjectToDoList, SearchProjectToDoList>
    {
        Task Insert(List<ProjectToDoList> entity, int IdProjectTask, int IdProject);
        Task Update(List<ProjectToDoList> entity, int IdProjectTask, int IdProject);
        Task <string> CheckUserExsitsInTask(List<ProjectToDoList> entity, string UserCreate);
    }
}
