using App.Core.Interface.DbContext;
using App.Core.Interface.Services.Configuration;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuanLy.Entities;
using QuanLy.Interface;
using QuanLy.Interface.Services;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class ProjectServiceService : DomainService<ProjectServices, SearchProject>, IProjectServiceService
    {
        protected readonly IConfiguration configuration;
        protected readonly IAppDbContext Context;
        protected readonly INotificationSingle notificationSingle;
        protected readonly IEmailConfigurationCoreService emailConfigurationCoreService;
        protected readonly IDeviceBrowserService deviceBrowserService;
        protected readonly IHubContext<CommentHub> _hubContext;
        protected readonly IUserService userService;
        public ProjectServiceService(IAppUnitOfWork unitOfWork, IMapper mapper
            , IConfiguration configuration
            , IAppDbContext context
            , INotificationSingle notificationSingle
            , IDeviceBrowserService deviceBrowserService
            , IHubContext<CommentHub> _hubContext
            , IUserService userService
            , IEmailConfigurationCoreService EmailConfigurationCoreService) : base(unitOfWork, mapper)
        {
            this.Context = context;
            this.emailConfigurationCoreService = EmailConfigurationCoreService;
            this.configuration = configuration;
            this.notificationSingle = notificationSingle;
            this.deviceBrowserService = deviceBrowserService;
            this.userService = userService;

            this._hubContext = _hubContext;
        }

        protected override string GetStoreProcName()
        {
            return "Mona_Load_ProjectService";
        }

        protected override SqlParameter[] GetSqlParameters(SearchProject baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@FromDate", baseSearch.FromDate),
                new SqlParameter("@ToDate", baseSearch.ToDate),
                new SqlParameter("@Status", baseSearch.Status),
                new SqlParameter("@ServiceTypeId", baseSearch.CategoryProjectId),

            };
            return parameters;
        }
        //public override async Task<PagedList<ProjectServices>> GetPagedListData(SearchProject baseSearch)
        //{
        //    PagedList<ProjectServices> pagedList = new PagedList<ProjectServices>();
        //    SqlParameter[] parameters = GetSqlParameters(baseSearch);
        //    pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
        //    pagedList.PageIndex = baseSearch.PageIndex;
        //    pagedList.PageSize = baseSearch.PageSize;
        //    return pagedList;
        //}

        //private async Task<PagedList<ProjectServices>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        //{
        //    return await Task.Run(() =>
        //    {
        //        PagedList<ProjectServices> pagedList = new PagedList<ProjectServices>();
        //        DataTable dataTable = new DataTable();
        //        SqlConnection connection = null;
        //        SqlCommand command = null;
        //        try
        //        {
        //            connection = (SqlConnection)Context.Database.GetDbConnection();
        //            command = connection.CreateCommand();
        //            connection.Open();
        //            command.CommandText = commandText;
        //            command.Parameters.AddRange(sqlParameters);
        //            //command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;
        //            command.CommandType = CommandType.StoredProcedure;
        //            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
        //            sqlDataAdapter.Fill(dataTable);
        //            //pagedList.TotalItem = int.Parse(command.Parameters["@TotalPage"].Value.ToString());
        //            pagedList.Items = MappingDataTable.ConvertToList<ProjectServices>(dataTable);
        //            if (pagedList.Items.Any())
        //                pagedList.TotalItem = pagedList.Items.FirstOrDefault().TotalPage ?? 0;
        //            return pagedList;
        //        }
        //        finally
        //        {
        //            if (connection != null && connection.State == System.Data.ConnectionState.Open)
        //                connection.Close();

        //            if (command != null)
        //                command.Dispose();
        //        }
        //    });
        //}
        private async Task<List<ProjectServices>> ListProjectServiceExprireDate()
        {
            return await unitOfWork.Repository<ProjectServices>().GetQueryable().Where(x => x.EndDate.Value.Date >= DateTime.Now.Date && DateTime.Now.AddDays(30).Date > x.EndDate.Value.Date && x.Status != 3 && x.StillInUse != 2).ToListAsync();
        }

        private async Task UpdateStatusService(ProjectServices entity)
        {
            if (entity.EndDate.Value.Date <= DateTime.Now.Date && entity.Status == 2)
            {
                entity.Status = 3;
            }
            else if ((entity.EndDate.Value.Date - DateTime.Now.Date).Days <= 14 && entity.Status == 1)
            {
                entity.Status = 2;
            }
            entity.DatePush = DateTime.Now;
            entity.Updated = DateTime.Now;
            entity.UpdatedBy = Contants.UPDATE_BY_SEVER;
            this.unitOfWork.Repository<ProjectServices>().Update(entity);
            await this.unitOfWork.SaveAsync();
        }

        public async Task UpdateServiceExprireDate()
        {
            var DataServiceProject = await this.Mona_sp_Load_ProjectService_EndDate();
            var DataLeader = await this.userService.Mona_sp_LoadUser_Role_Leader();
            var AppId = Guid.Parse(configuration.GetSection("OneSignal:AppId").Value.ToString());
            var AppSecret = configuration.GetSection("OneSignal:AppSecret").Value.ToString();
            foreach (var item in DataServiceProject)
            {
                int day = (item.EndDate.Value.Date - DateTime.Now.Date).Days;
                if (day % 3 == 0)
                {
                    //Tạo thông báo và Onesignal leader
                    foreach (var leader in DataLeader)
                    {
                        var DataNotification = await this.notificationSingle.CreateNotification(0, "Thông báo gia hạn dịch vụ", $"/Admin/Services/ServiceList?search= { item.ProjectId}", leader.Id, 1, Contants.UPDATE_BY_SEVER);
                        await _hubContext.Clients.All.SendAsync(Contants.SR_NOTIFICATION, DataNotification);
                        if(leader.dataPlayerIds != null)
                        {
                            foreach (var PlayerIdLeader in leader.dataPlayerIds)
                            {
                                List<string> ListData = new List<string>();
                                ListData.Add(PlayerIdLeader.PlayerId);
                                string Url = "https://zingnews.vn/";
                                var DataPlayerId = await OneSignalPushNotification.PushNotification("Mona.media", "Dịch vụ đến hạn", ListData, AppId, AppSecret, Url);
                                if (string.IsNullOrEmpty(DataPlayerId))
                                {
                                    await this.deviceBrowserService.UpdateHide(PlayerIdLeader.PlayerId);
                                }
                            }
                        }
                    }

                    // Gửi Mail Khách
                    foreach (var dataUser in item.dataUserIds)
                    {
                        if (!string.IsNullOrEmpty(dataUser.Gmail)) 
                        {
                            string[] ListEmail = new[] { dataUser.Gmail };
                            await this.emailConfigurationCoreService.Send("Mona Media - Thông tin dịch vụ", EmailTemplate.ServiceExpires(dataUser.FullName, item.ServiceName + "/" + item.ProjectName,
                                                    String.Format("{0:0,0 đ}", item.Price).Replace(",", "."), item.EndDate.Value.AddDays(1).ToString("dd/MM/yyyy"),
                                                    item.StartDate.Value.AddDays(1).ToString("dd/MM/yyyy"), item.EndDate.Value.AddDays(1).ToString("dd/MM/yyyy")), ListEmail);
                        }
                    }

                    // OneSignal Khách hàng
                    foreach (var dataPlayerID in item.dataPlayerIds)
                    {
                        List<string> ListData = new List<string>();
                        ListData.Add(dataPlayerID.PlayerId);
                        string Url = "https://zingnews.vn/";
                        var DataPlayerId = await OneSignalPushNotification.PushNotification("Mona.media", "Dịch vụ đến hạn", ListData, AppId, AppSecret, Url);
                        if (string.IsNullOrEmpty(DataPlayerId))
                        {
                            await this.deviceBrowserService.UpdateHide(dataPlayerID.PlayerId);
                        }
                    }

                    // Update status 
                    await this.UpdateStatusService(item);
                }
            }

        }

        

        public async Task<List<ProjectServices>> Mona_sp_Load_ProjectService_EndDate()
        {
            return await Task.Run(() =>
            {
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    List<ProjectServices> Model = new List<ProjectServices>();
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "Mona_sp_Load_ProjectService_EndDate";
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    Model = MappingDataTable.ConvertToList<ProjectServices>(dataTable);
                    return Model;
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
    }
}
