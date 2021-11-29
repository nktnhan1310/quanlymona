using App.Core.Interface;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface
{
    public interface INotificationSingle : IDomainService<NotificationSingles, SearchNotification>
    {
        /// <summary>
        /// Tạo thông báo
        /// </summary>
        /// <param name="NotificationId"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="UID"></param>
        /// <param name="type"> 1 Thông báo dịch vụ, 2 Thông báo task, 3 Thông báo dự án, 4 khác</param>
        /// <param name="createby"></param>
        /// <returns></returns>
        Task <int> CreateNotification(int NotificationId, string title, string content, int UID, int type, string createby);

        /// <summary>
        /// Tạo thông báo cho leader khi có bái post
        /// </summary>
        /// <param name="UserName">User Người đăng</param>
        /// <param name="fullName">FullName Người đăng</param>
        /// <param name="DataLink">Link Bài post</param>
        /// <returns></returns>
        Task CreateNotifacationForLeader(string UserName, string fullName, string DataLink, string Content);

        /// <summary>
        /// Tạo thông báo khi User bình luận trên bài viết
        /// </summary>
        /// <param name="UserName">Người comment</param>
        /// <param name="fullName">Full Người Comment </param>
        /// <param name="DataLink">Link Bài Post</param>
        /// <param name="UserPostID">NguoiTaoPost</param>
        /// <returns></returns>
        Task CreateNotificationfeedback(string UserName, string fullName, string DataLink, string Content, string CreatedBy);


        /// <summary>
        /// Tạo thông báo khi User bình luận trên 1 bình luận khác
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="fullName"></param>
        /// <param name="DataLink"></param>
        /// <param name="Content"></param>
        /// <param name="UserPostCommentID"></param>
        /// <returns></returns>
        Task CreateNotificationfeedbackComment(string UserName, string fullName, string DataLink, string Content, int UserPostCommentID);


        /// <summary>
        /// Số thông báo của User
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<int> NumberNotification(int UserId);

        /// <summary>
        /// Cập nhật trạng thái thông báo
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<string> UpdateStaus(int Id, int UID);


    }
}
