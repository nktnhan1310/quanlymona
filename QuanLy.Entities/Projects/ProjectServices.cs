using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace QuanLy.Entities
{
    /// <summary>
    /// Dịch vụ của dự án
    /// </summary>
    public class ProjectServices : AppCoreDomain
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Tên dịch vụ
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ngày bắt đầu dịch vụ
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// Ngày kết thúc dịch vụ
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Giá dịch vụ
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Hạn sử dụng
        /// </summary>
        [StringLength(500)]
        public string DeadlineName { get; set; }

        public DateTime? DatePush { get; set; }

        public int? StillInUse { get; set; }
        /// <summary>
        /// ghi chú
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Danh mục
        /// </summary>
        public int? ServiceTypeId { get; set; }

        #region Extension Properties

        /// <summary>
        /// Tên dự án
        /// </summary>
        [NotMapped]
        public string ProjectName { get; set; }

        /// <summary>
        /// Tên dịch vụ sử dụng
        /// </summary>
        [NotMapped]
        public string ServiceName { get; set; }


        [NotMapped]
        public string ListPlayerId { get; set; }

        [NotMapped]
        public IList<ListDataPlayerId> dataPlayerIds
        {
            get
            {
                if (!string.IsNullOrEmpty(ListPlayerId))
                {
                    var DataSplit = ListPlayerId.Split(';').ToArray();
                    if (DataSplit != null && DataSplit.Any())
                    {
                        List<ListDataPlayerId> result = new List<ListDataPlayerId>();
                        foreach (var item in DataSplit)
                        {
                            result.Add(new ListDataPlayerId()
                            {
                                PlayerId = item
                            });
                        }
                        return result;
                    }
                }
                return null;
            }
        }
        [NotMapped]
        public string ListUserId { get; set; }

        [NotMapped]
        public IList<ListDataUserId> dataUserIds
        {
            get
            {
                if (!string.IsNullOrEmpty(ListUserId))
                {
                    var DataSplit = ListUserId.Split(';').ToArray();
                    if (DataSplit != null && DataSplit.Any())
                    {
                        List<ListDataUserId> result = new List<ListDataUserId>();
                        foreach (var item in DataSplit)
                        {
                            var itemProperties = item.Split('_').ToArray();
                            if (itemProperties != null && itemProperties.Any())
                            {
                                result.Add(new ListDataUserId()
                                {
                                    Gmail = itemProperties[0],
                                    FullName = itemProperties[1]
                                }); ;
                            }
                        }
                        return result;
                    }
                }
                return null;
            }
        }

        #endregion
    }

    public class ListDataPlayerId
    {
        public string PlayerId { get; set; }
    }
    public class ListDataUserId
    {
        public string Gmail { get; set; }
        public string FullName { get; set; }
    }
}
