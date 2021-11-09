using App.Core.Controllers;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities.Newfeed;
using QuanLy.Interface;
using QuanLy.Interface.Services;
using QuanLy.Interface.Services.Newfeed;
using QuanLy.Model.Newfeed;
using QuanLy.Model.RequestModel.Newfeed;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers.Newfeed
{
    [Route("api/post-comment")]
    [ApiController]
    [Authorize]
    [Description("Quản lý comment của newfeed ")]
    public class PostCommentController  : BaseController<PostComments, PostCommentModel, RequestPostCommentModel, BaseSearch>
    {
        private IHubContext<CommentHub> _hubContext;
        protected IPostContents postContents;
        protected IConfiguration configuration;
        protected IPostComments PostComments;
        protected INotificationSingle notificationSingle;
        protected IUserService userService;
        public PostCommentController(IServiceProvider serviceProvider, ILogger<BaseController<PostComments, PostCommentModel, RequestPostCommentModel, BaseSearch>> logger, IWebHostEnvironment env
            , IHubContext<CommentHub> hubContext
            , IConfiguration configuration) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPostComments>();
            this.postContents = serviceProvider.GetRequiredService<IPostContents>();
            this.PostComments = serviceProvider.GetRequiredService<IPostComments>();
            this.notificationSingle = serviceProvider.GetRequiredService<INotificationSingle>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this._hubContext = hubContext;

        }

        public class SearchParentPostComment : BaseSearch
        {
            public int PostContentID { get; set; }
        }

        public class SearchChilPostComment : BaseSearch
        {
            public int PostCommentID { get; set; }
            public int PostContentID { get; set; }
        }

        /// <summary>
        /// Lấy danh sách item phân trang Parent Comment của content
        /// </summary>
        /// <param name="searchParent"></param>
        /// <returns></returns>
        [HttpGet("get-parent-comment")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]

        public async Task<AppDomainResult> GetParentComment ([FromQuery] SearchParentPostComment searchParent)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                PagedList<PostComments> pagedData = await this.PostComments.GetPagedListDataPanrentComment(searchParent.PageIndex, searchParent.PageSize, searchParent.PostContentID);
                PagedList<PostCommentModel> pagedDataModel = mapper.Map<PagedList<PostCommentModel>>(pagedData);
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách item phân trang Chil Comment của content
        /// </summary>
        /// <param name="searchChil"></param>
        /// <returns></returns>
        [HttpGet("get-child-comment")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetChilComment([FromQuery] SearchChilPostComment searchChil)
        {

            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                PagedList<PostComments> pagedData = await this.PostComments.GetPagedListDataChilComment(searchChil.PageIndex,searchChil.PageSize,searchChil.PostContentID,searchChil.PostCommentID);
                PagedList<PostCommentModel> pagedDataModel = mapper.Map<PagedList<PostCommentModel>>(pagedData);
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới comment
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] RequestPostCommentModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<PostComments>(itemModel);
            item.Created = DateTime.UtcNow.AddHours(7);
            item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
            item.Active = true;
            item.UID = LoginContext.Instance.CurrentUser.UserId;
            var FullName = LoginContext.Instance.CurrentUser.FullName;
            var DataPostContent = this.postContents.GetById(item.PostContentID);
            if(DataPostContent != null)
            {
                string datalink = "/Admin/NewFeed/NewFeed/" + DataPostContent.Id;
                if (item.PostCommentID != 0)
                {
                    var DataParentPostComment = this.domainService.GetById(item.PostCommentID);
                    if(DataParentPostComment == null)
                    {
                        throw new KeyNotFoundException("Item Parent Comment không tồn tại");
                    }

                    success = await this.domainService.CreateAsync(item);
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;

                    //Người phản hồi bình luận
                    if (success && DataParentPostComment.UID != item.UID)
                    {
                        await this.notificationSingle.CreateNotificationfeedbackComment(item.CreatedBy, FullName, datalink, " đã phản hồi bình luận của bạn", DataParentPostComment.UID);
                    }
                }
                else
                {
                    success = await this.domainService.CreateAsync(item);
                    appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                }

                //Người bình luận bài viết
                if (success && DataPostContent.CreatedBy != item.CreatedBy)
                {
                    await this.notificationSingle.CreateNotificationfeedback(item.CreatedBy, FullName, datalink, " đã bình luận về bài viết của bạn", DataPostContent.CreatedBy);
                }

            }
            else
            {
                throw new KeyNotFoundException("Item Content không tồn tại");
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin comment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestPostCommentModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<PostComments>(itemModel);
            item.Updated = DateTime.UtcNow.AddHours(7);
            item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
            item.Active = true;
            var UID = LoginContext.Instance.CurrentUser.UserId;
            var DataPostContent = this.postContents.GetById(item.PostContentID);
            if (DataPostContent != null)
            {
                var DataPostComment = this.domainService.GetById(item.Id);
                if(DataPostComment == null)
                {
                    throw new KeyNotFoundException("Item Parent Comment không tồn tại");
                }
                else if (DataPostComment.Id == item.PostCommentID)
                {
                    throw new KeyNotFoundException("Không được sửa của Parent Comment ");
                }
                else if(DataPostComment.UID != UID)
                {
                    throw new KeyNotFoundException("Không được sửa của User khác");
                }
                else if (item.PostCommentID != 0)
                {
                    var DataPostParentComment = this.domainService.GetById(item.PostCommentID);
                    if (DataPostParentComment == null)
                    {
                        throw new KeyNotFoundException("Item Parent Comment không tồn tại");
                    }
                }
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                item.UID = UID;
                success = await this.domainService.UpdateAsync(item);
                await _hubContext.Clients.All.SendAsync(Contants.SR_POST_COMMENT, item.Id);
            }
            else
            {
                throw new KeyNotFoundException("Item Content không tồn tại");
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public override async Task<AppDomainResult> DeleteItem(int id)
        {
            bool success = false;
            AppDomainResult appDomainResult = new AppDomainResult();
            var DataExist = this.domainService.GetById(id);
            if (DataExist != null)
            {
                var UID = LoginContext.Instance.CurrentUser.UserId;
                var userCreateFeed = DataExist.UID;
                if (userCreateFeed == UID)
                {

                    if(DataExist.PostCommentID == 0)
                    {
                       // Nếu là Parent comment thì xóa tất cả những chil  comment
                       var ListDataChilComment = await this.PostComments.ListPostComment(DataExist.Id);
                        foreach(var item in ListDataChilComment)
                        {
                            await this.domainService.DeleteAsync(item.Id);
                        }
                    }
                    success = await this.domainService.DeleteAsync(id);
                    await _hubContext.Clients.All.SendAsync(Contants.SR_POST_COMMENT_DELETE, DataExist);
                    if (success)
                    {
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    }

                }
                else
                {
                    throw new KeyNotFoundException("Không có quyền xóa của User khác");
                }
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            appDomainResult.Success = success;
            return appDomainResult;
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.PostComments.GetIdPostComment(id);
            if (item != null)
            {
                var itemModel = mapper.Map<PostCommentModel>(item);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = itemModel,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }
    }
}
