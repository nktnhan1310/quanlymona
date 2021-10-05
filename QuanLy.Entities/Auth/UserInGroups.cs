using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuanLy.Entities.Auth
{
    public class UserInGroups : AppCoreDomain
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
        [NotMapped]
        public Users Users { get; set; }

        /// <summary>
        /// Lấy thông tin Nhóm người dùng
        /// </summary>
        [NotMapped]
        public UserGroups UserGroups { get; set; }

        #endregion
    }
}
