using App.Core.Entities;
using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Utilities;
using AutoMapper;
using Azure.Core;
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
    public class NotificationSingleService : DomainService<NotificationSingles, SearchNotification>, INotificationSingle
    {
        protected readonly IAppDbContext Context;
        protected readonly IUserService userService;
        protected readonly IHubContext<CommentHub> _hubContext;
        protected readonly IDeviceBrowserService deviceBrowserService;
        protected readonly IConfiguration configuration;
        public NotificationSingleService(IAppUnitOfWork unitOfWork, IMapper mapper
            , IUserService userService
            , IDeviceBrowserService deviceBrowserService
            , IHubContext<CommentHub> _hubContext
            , IConfiguration configuration
            , IAppDbContext context) : base(unitOfWork, mapper)
        {
            this.configuration = configuration;
            this.userService = userService;
            this.deviceBrowserService = deviceBrowserService;
            this._hubContext = _hubContext;
            this.Context = context;
        }

        public async Task<int> CreateNotification(int NotificationId, string title, string content, int UID, int type, string createby)
        {
            NotificationSingles entity = new NotificationSingles();
            entity.NotificationID = NotificationId;
            entity.NotificationTitle = title;
            entity.notifacationContent = content;
            entity.UID = UID;
            entity.TypeNoti = type;
            entity.Created = DateTime.UtcNow.AddHours(7);
            entity.CreatedBy = createby;
            entity.Active = true;
            entity.Deleted = false;
            this.unitOfWork.Repository<NotificationSingles>().Create(entity);
            await this.unitOfWork.SaveAsync();
            return entity.UID;
        }

        public async Task CreateNotifacationForLeader(string UserName, string FullName, string DataLink, string Content)
        {
            var AppId = Guid.Parse(configuration.GetSection("OneSignal:AppId").Value.ToString());
            var AppSecret = configuration.GetSection("OneSignal:AppSecret").Value.ToString();
            var DataLeaders = await this.userService.Mona_sp_LoadUser_Role_Leader();
            foreach (var leader in DataLeaders)
            {
                var DataNotification = await this.CreateNotification(0, Content + FullName, DataLink, leader.Id, 4, UserName);
                await _hubContext.Clients.All.SendAsync(Contants.SR_NOTIFICATION, DataNotification);
                if (leader.dataPlayerIds != null)
                {
                    foreach (var PlayerIdLeader in leader.dataPlayerIds)
                    {
                        List<string> ListData = new List<string>();
                        ListData.Add(PlayerIdLeader.PlayerId);
                        string Url = "https://zingnews.vn/";
                        var DataPlayerId = await OneSignalPushNotification.PushNotification("Mona.media",Content + FullName, ListData, AppId, AppSecret, Url);
                        if (string.IsNullOrEmpty(DataPlayerId))
                        {
                            await this.deviceBrowserService.UpdateHide(PlayerIdLeader.PlayerId);
                        }
                    }
                }
            }
        }
        public async Task CreateNotificationfeedback(string UserName, string FullName, string DataLink, string Content, string UserNamePostContent)
        {
            var AppId = Guid.Parse(configuration.GetSection("OneSignal:AppId").Value.ToString());
            var AppSecret = configuration.GetSection("OneSignal:AppSecret").Value.ToString();
            var GetDataUser = await this.unitOfWork.Repository<UserCores>().GetQueryable().FirstOrDefaultAsync(x => x.UserName == UserNamePostContent);
            if (GetDataUser != null)
            {
                var DataNotification = await this.CreateNotification(0, FullName + Content, DataLink, GetDataUser.Id, 4, UserName);
                await _hubContext.Clients.All.SendAsync(Contants.SR_NOTIFICATION, DataNotification);
                var GetListPlayerId = await this.unitOfWork.Repository<DeviceBrowsers>().GetQueryable().Where(x => x.UserId == GetDataUser.Id).Select(x => x.PlayerId).ToListAsync();
                foreach (var item in GetListPlayerId)
                {
                    List<string> ListData = new List<string>();
                    ListData.Add(item);
                    string Url = "https://zingnews.vn/";
                    var DataPlayerId = await OneSignalPushNotification.PushNotification("Mona.media", FullName + Content , ListData, AppId, AppSecret, Url);
                    if (string.IsNullOrEmpty(DataPlayerId))
                    {
                        await this.deviceBrowserService.UpdateHide(item);
                    }
                }
            }
        }

        public async Task CreateNotificationfeedbackComment(string UserName, string FullName, string DataLink, string Content, int UserPostCommentID)
        {
            var AppId = Guid.Parse(configuration.GetSection("OneSignal:AppId").Value.ToString());
            var AppSecret = configuration.GetSection("OneSignal:AppSecret").Value.ToString();
            var DataNotification = await this.CreateNotification(0, FullName + Content, DataLink, UserPostCommentID, 4, UserName);
            await _hubContext.Clients.All.SendAsync(Contants.SR_NOTIFICATION, DataNotification);
            var GetListPlayerId = await this.unitOfWork.Repository<DeviceBrowsers>().GetQueryable().Where(x => x.UserId == UserPostCommentID).Select(x => x.PlayerId).ToListAsync();
            foreach (var item in GetListPlayerId)
            {
                List<string> ListData = new List<string>();
                ListData.Add(item);
                string Url = "https://zingnews.vn/";
                var DataPlayerId = await OneSignalPushNotification.PushNotification("Mona.media",FullName + Content, ListData, AppId, AppSecret, Url);
                if (string.IsNullOrEmpty(DataPlayerId))
                {
                    await this.deviceBrowserService.UpdateHide(item);
                }
            }
        }

        public async Task<int> NumberNotification(int UserId)
        {
            return await this.unitOfWork.Repository<NotificationSingles>().GetQueryable().Where(x => x.UID == UserId && x.Status == 0 && x.Deleted == false).CountAsync();
        }

        protected override string GetStoreProcName()
        {
            return "Mona_sp_Load_NotificationSingle_User_PagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchNotification baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@UID", baseSearch.UID),
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@FromDate", baseSearch.FromDate),
                new SqlParameter("@ToDate", baseSearch.ToDate),
                new SqlParameter("@Status", baseSearch.Status),

            };
            return parameters;
        }

        public override async Task<PagedList<NotificationSingles>> GetPagedListData(SearchNotification baseSearch)
        {
            PagedList<NotificationSingles> pagedList = new PagedList<NotificationSingles>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.PageIndex = baseSearch.PageIndex;
            pagedList.PageSize = baseSearch.PageSize;
            return pagedList;
        }

        private async Task<PagedList<NotificationSingles>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedList<NotificationSingles> pagedList = new PagedList<NotificationSingles>();
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
                    pagedList.Items = MappingDataTable.ConvertToList<NotificationSingles>(dataTable);
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

        public async Task<string> UpdateStaus(int Id,int UID)
        {
            string Message = "";
            var Model = await this.unitOfWork.Repository<NotificationSingles>().GetQueryable().FirstOrDefaultAsync(x => x.Id == Id && x.UID == UID);
            if (Model != null)
            {
                Model.Status = 1;
                this.unitOfWork.Repository<NotificationSingles>().Update(Model);
                await this.unitOfWork.SaveAsync();
            }
            else
                Message = "Không dữ liệu thông báo";

            return Message;
        }
    }
}
