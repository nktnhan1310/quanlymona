using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class ProjectService : DomainService<Projects, SearchProject>, IProjectService
    {
        public ProjectService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Project_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchProject baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@FromDate", baseSearch.FromDate),
                new SqlParameter("@ToDate", baseSearch.ToDate),
                new SqlParameter("@Status", baseSearch.Status),
                new SqlParameter("@CategoryProjectId", baseSearch.CategoryProjectId),
                new SqlParameter("@IsDone", baseSearch.isDone),

                new SqlParameter("@UserIds", (baseSearch.UserIds != null && baseSearch.UserIds.Any()) ? string.Join(',', baseSearch.UserIds) : string.Empty),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Thêm mới dự án
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(Projects item)
        {
            bool result = false;
            if (item == null) throw new AppException("Item không tồn tại");
            this.unitOfWork.Repository<Projects>().Create(item);
            await this.unitOfWork.SaveAsync();

            // Thêm vào thông tin file dự án
            if (item.ProjectFiles != null && item.ProjectFiles.Any())
            {
                foreach (var file in item.ProjectFiles)
                {
                    this.unitOfWork.Repository<ProjectFiles>().Create(file);
                }

            }
            // Thêm user dự án
            if (item.ProjectUsers != null && item.ProjectUsers.Any())
            {
                foreach (var user in item.ProjectUsers)
                {
                    user.Created = DateTime.UtcNow.AddHours(7);
                    user.CreatedBy = item.CreatedBy;
                    this.unitOfWork.Repository<ProjectUsers>().Create(user);
                }
            }
            await this.unitOfWork.SaveAsync();
            result = true;
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin dự án
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Projects item)
        {
            bool result = false;
            var existItem = await this.unitOfWork.Repository<Projects>().GetQueryable().Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem == null) throw new AppException("Không tìm thấy thông tin dự án");

            var currentCreated = existItem.Created;
            var currentCreatedBy = existItem.CreatedBy;
            existItem = mapper.Map<Projects>(item);
            existItem.Created = currentCreated;
            existItem.CreatedBy = currentCreatedBy;
            this.unitOfWork.Repository<Projects>().Update(existItem);

            // Cập nhật thông tin user của dự án
            if (item.ProjectUsers != null && item.ProjectUsers.Any())
            {
                foreach (var user in item.ProjectUsers)
                {
                    var existUser = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable().Where(e => e.Id == user.Id).FirstOrDefaultAsync();
                    if (existUser == null)
                    {
                        user.Created = DateTime.UtcNow.AddHours(7);
                        user.CreatedBy = item.CreatedBy;
                        user.ProjectId = item.Id;
                        this.unitOfWork.Repository<ProjectUsers>().Create(user);
                    }
                    else
                    {
                        var currentItemCreated = existUser.Created;
                        var currentItemCreatedBy = existUser.CreatedBy;
                        existUser = mapper.Map<ProjectUsers>(user);
                        existUser.Created = currentItemCreated;
                        existUser.CreatedBy = currentItemCreatedBy;

                        existUser.Updated = DateTime.UtcNow.AddHours(7);
                        existUser.UpdatedBy = item.UpdatedBy;
                        existUser.ProjectId = item.Id;
                        this.unitOfWork.Repository<ProjectUsers>().Update(existUser);
                    }
                }
            }

            // Cập nhật thông tin file của dự án
            if (item.ProjectFiles != null && item.ProjectFiles.Any())
            {
                foreach (var file in item.ProjectFiles)
                {
                    var existFile = await this.unitOfWork.Repository<ProjectFiles>().GetQueryable().Where(e => e.Id == file.Id).FirstOrDefaultAsync();
                    if(existFile == null)
                    {
                        file.Created = DateTime.UtcNow.AddHours(7);
                        file.CreatedBy = item.CreatedBy;
                        file.ProjectId = item.Id;
                        this.unitOfWork.Repository<ProjectFiles>().Create(file);
                    }
                    else
                    {
                        var currentItemCreated = existFile.Created;
                        var currentItemCreatedBy = existFile.CreatedBy;
                        existFile = mapper.Map<ProjectFiles>(file);
                        existFile.Created = currentItemCreated;
                        existFile.CreatedBy = currentItemCreatedBy;
                        existFile.ProjectId = item.Id;
                        this.unitOfWork.Repository<ProjectFiles>().Update(existFile);
                    }
                }
            }
            // Cập nhật thông tin phiên thanh toán của dự án
            if (item.ProjectSessionPays != null && item.ProjectSessionPays.Any())
            {
                foreach (var sessionPay in item.ProjectSessionPays)
                {
                    var existSessionPay = await this.unitOfWork.Repository<ProjectSessionPays>().GetQueryable().Where(e => e.Id == sessionPay.Id).FirstOrDefaultAsync();
                    if (existSessionPay == null)
                    {
                        sessionPay.Created = DateTime.UtcNow.AddHours(7);
                        sessionPay.CreatedBy = item.CreatedBy;
                        sessionPay.ProjectId = item.Id;
                        this.unitOfWork.Repository<ProjectSessionPays>().Create(sessionPay);
                    }
                    else
                    {
                        var currentItemCreated = existSessionPay.Created;
                        var currentItemCreatedBy = existSessionPay.CreatedBy;
                        existSessionPay = mapper.Map<ProjectSessionPays>(sessionPay);
                        existSessionPay.Created = currentItemCreated;
                        existSessionPay.CreatedBy = currentItemCreatedBy;

                        existSessionPay.Updated = DateTime.UtcNow.AddHours(7);
                        existSessionPay.UpdatedBy = item.UpdatedBy;
                        existSessionPay.ProjectId = item.Id;
                        this.unitOfWork.Repository<ProjectSessionPays>().Update(existSessionPay);
                    }
                }
            }


            await this.unitOfWork.SaveAsync();
            result = true;

            return result;
        }

    }
}
