using App.Core.Models.AuthModel;
using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model.Auth
{
    public class UserInGroupModel : AppCoreDomainModel
    {
        /// <summary>
        /// Người dùng
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        public int UserGroupId { get; set; }


        #region Extension Properties

        /// <summary>
        /// Lấy thông tin Người dùng
        /// </summary>
        public UserModel Users { get; set; }

        /// <summary>
        /// Lấy thông tin Nhóm người dùng
        /// </summary>
        public UserGroupModel UserGroups { get; set; }

        #endregion
    }
}
