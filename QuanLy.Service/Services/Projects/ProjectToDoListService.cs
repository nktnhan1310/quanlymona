using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Utilities;
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
    public class ProjectToDoListService : DomainService<ProjectToDoList, SearchProjectToDoList>, IProjectToDoList
    {
        protected readonly IAppDbContext Context;
        public ProjectToDoListService(IAppUnitOfWork unitOfWork, IMapper mapper
            , IAppDbContext Context) : base(unitOfWork, mapper)
        {
            this.Context = Context;
        }
        public async Task Insert(List<ProjectToDoList> entity, int IdProjectTask, int IdProject)
        {
            if (entity != null && entity.Any())
            {
                foreach (var ItemToDo in entity)
                {
                    ItemToDo.TaskId = IdProjectTask;
                    ItemToDo.ProjectId = IdProject;
                    ItemToDo.ToTime = ItemToDo.FromTime.Value.AddHours(ItemToDo.NumberHours);
                    this.unitOfWork.Repository<ProjectToDoList>().Create(ItemToDo);
                }
                await this.unitOfWork.SaveAsync();
            }
        }
        public async Task Update(List<ProjectToDoList> entity, int IdProjectTask, int IdProject)
        {
            if (entity != null && entity.Any())
            {
                foreach (var ItemToDo in entity)
                {
                    if (ItemToDo.Id == 0)
                    {
                        ItemToDo.TaskId = IdProjectTask;
                        ItemToDo.ProjectId = IdProject;
                        ItemToDo.ToTime = ItemToDo.FromTime.Value.AddHours(ItemToDo.NumberHours);
                        this.unitOfWork.Repository<ProjectToDoList>().Create(ItemToDo);
                    }
                    else
                    {
                        var GetCreatedDate = await this.unitOfWork.Repository<ProjectToDoList>().GetQueryable().FirstOrDefaultAsync(x => x.Id == ItemToDo.Id);
                        ItemToDo.ToTime = ItemToDo.FromTime.Value.AddHours(ItemToDo.NumberHours);
                        ItemToDo.TaskId = IdProjectTask;
                        ItemToDo.ProjectId = IdProject;
                        ItemToDo.Created = GetCreatedDate.Created;
                        ItemToDo.CreatedBy = GetCreatedDate.CreatedBy;
                        this.unitOfWork.Repository<ProjectToDoList>().Update(ItemToDo);
                    }
                }
                await this.unitOfWork.SaveAsync();
            }
        }

        protected override string GetStoreProcName()
        {
            return "Mona_sp_Load_ProjectToDoList_PagingData";
        }
        protected override SqlParameter[] GetSqlParameters(SearchProjectToDoList baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@TaskId", baseSearch.TaskId),
                new SqlParameter("@ProjectId", baseSearch.ProjectId),
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@FromDate", baseSearch.FromDate),
                new SqlParameter("@ToDate", baseSearch.ToDate),
                new SqlParameter("@UID", baseSearch.UserId),
                new SqlParameter("@Priority", baseSearch.Priority),

            };
            return parameters;
        }

        public override async Task<PagedList<ProjectToDoList>> GetPagedListData(SearchProjectToDoList baseSearch)
        {
            PagedList<ProjectToDoList> pagedList = new PagedList<ProjectToDoList>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.PageIndex = baseSearch.PageIndex;
            pagedList.PageSize = baseSearch.PageSize;
            return pagedList;
        }
        private async Task<PagedList<ProjectToDoList>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedList<ProjectToDoList> pagedList = new PagedList<ProjectToDoList>();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = commandText;
                    command.Parameters.AddRange(sqlParameters);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    pagedList.Items = MappingDataTable.ConvertToList<ProjectToDoList>(dataTable);
                    if (pagedList.Items.Any())
                        pagedList.TotalItem = pagedList.Items.FirstOrDefault().TotalPage ?? 0;
                    return pagedList;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }
        public async Task<string> CheckUserExsitsInTask(List<ProjectToDoList> entity, string UserCreate)
        {
            string message = "";
            foreach (var item in entity)
            {
                var GetDataTask = await this.unitOfWork.Repository<ProjectTasks>().GetQueryable().FirstOrDefaultAsync(x => x.Id == item.TaskId);
                if (GetDataTask == null)
                {
                    message = $"Không có dữ liệu cửa Task có Title là : {item.Title}";
                    break;
                }
                var GetUserInTask = await this.unitOfWork.Repository<ProjectUsers>().GetQueryable()
                    .Where(x => x.TaskId == GetDataTask.Id).Select(x => x.UserId).ToListAsync();
                if (!GetUserInTask.Contains(item.UserId))
                {
                    message = $"User không có trong Task có Title là : {item.Title}";
                    break;

                }
                item.ToTime = item.FromTime.Value.AddHours(item.NumberHours);
                item.Created = DateTime.Now;
                item.CreatedBy = UserCreate;
                item.ProjectId = GetDataTask.ProjectId;
                item.Active = true;
            }
            return message;
        }
    }
}
