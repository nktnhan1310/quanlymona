using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class ProjectUserService : DomainService<ProjectUsers, BaseSearch>, IProjectUserService
    {
        public ProjectUserService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        /// <summary>
        /// Thêm mới thông tin user của dự án
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(ProjectUsers item)
        {
            bool result = false;
            if (item == null) throw new AppException("Không tìm thấy item");
            bool isExistUser = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable().AnyAsync(e => !e.Deleted && e.Active
            && e.ProjectId == item.ProjectId && e.UserId == item.Id && e.Id != item.Id);
            if (isExistUser) throw new AppException("Thông tin user đã tồn tại trong dự án");
            this.unitOfWork.Repository<ProjectUsers>().Create(item);
            await this.unitOfWork.SaveAsync();
            result = true;
            return result;
        }

        public async Task<string> UpdateStatusTaskOfUser(int UserId, int TaskId)
        {
            string message = "";
            var GetData = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable().FirstOrDefaultAsync(
                x => x.TaskId == TaskId
                && x.UserId == UserId);
            var GetDatatask = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable().FirstOrDefaultAsync(
                x => x.Id == TaskId);
            if (GetData == null)
                message = "Không có dữ liệu của task";
            else if(GetDatatask == null)
                message = "Không có dữ liệu của task";
            else if(GetDatatask.StatusDay == 1)
                message = "Task đã hoàn thành, không được cập nhật nữa";
            else
            {
                GetDatatask.StatusDay = 1;
                GetDatatask.IsDone = true;
                GetDatatask.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                GetDatatask.Updated = DateTime.UtcNow.AddHours(7);
                this.unitOfWork.Repository<ProjectTasks>().Update(GetDatatask);
                await this.unitOfWork.SaveAsync();
            }
            return message;
        }
    }
}
