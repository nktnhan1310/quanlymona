using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuanLy.Entities
{
    public class ReportSourceList : ReportAppDomain
    {
        /// <summary>
        /// Tên hiển thị
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ngày tạo nếu có
        /// </summary>
        public DateTime? Created { get; set; }
        /// <summary>
        /// Chi tiết báo cáo
        /// </summary>
        public string DetailReports { get; set; }

        /// <summary>
        ///  Tổng số nguồn từ tawk
        /// </summary>
        public int? TotalTawks
        {
            get
            {
                if (ReportTotalSourceDetails != null && ReportTotalSourceDetails.Any())
                {
                    var tawkSourceTotals = ReportTotalSourceDetails.Where(e => e.SourceCode == CatalogueEnums.ContactCustomerSourceCatalogue.TAWK.ToString()).Count();
                    return tawkSourceTotals;
                }

                return 0;
            }
        }

        /// <summary>
        ///  Tổng số nguồn từ điện thoại
        /// </summary>
        public int? TotalPhones
        {
            get
            {
                if (ReportTotalSourceDetails != null && ReportTotalSourceDetails.Any())
                {
                    var phoneSourceTotals = ReportTotalSourceDetails.Where(e => e.SourceCode == CatalogueEnums.ContactCustomerSourceCatalogue.PHONE.ToString()).Count();
                    return phoneSourceTotals;
                }

                return 0;
            }
        }

        /// <summary>
        ///  Tổng số nguồn từ form
        /// </summary>
        public int? TotalForms
        {
            get
            {
                if (ReportTotalSourceDetails != null && ReportTotalSourceDetails.Any())
                {
                    var formSourceTotals = ReportTotalSourceDetails.Where(e => e.SourceCode == CatalogueEnums.ContactCustomerSourceCatalogue.FORM.ToString()).Count();
                    return formSourceTotals;
                }

                return 0;
            }
        }

        /// <summary>
        ///  Tổng số nguồn từ khác
        /// </summary>
        public int? TotalOthers
        {
            get
            {
                if (ReportTotalSourceDetails != null && ReportTotalSourceDetails.Any())
                {
                    var otherSourceTotals = ReportTotalSourceDetails.Where(e => e.SourceCode == CatalogueEnums.ContactCustomerSourceCatalogue.OTHER.ToString()).Count();
                    return otherSourceTotals;
                }

                return 0;
            }
        }


        public IList<ReportSourceDetails> ReportTotalSourceDetails
        {
            get
            {
                IList<ReportSourceDetails> reportDetails = new List<ReportSourceDetails>();
                if (!string.IsNullOrEmpty(DetailReports))
                {
                    string[] arrayDetails = DetailReports.Split(';').ToArray();
                    if (arrayDetails != null && arrayDetails.Any())
                    {
                        foreach (var arrayDetail in arrayDetails)
                        {
                            string[] sourceDetails = arrayDetail.Split('_').ToArray();
                            if (sourceDetails != null && sourceDetails.Any())
                            {
                                int sourceId = 0;
                                int contactCustomerId = 0;

                                if (sourceDetails[0] != null && !string.IsNullOrEmpty(sourceDetails[0]))
                                    int.TryParse(sourceDetails[0], out sourceId);
                                
                                string soureCode = sourceDetails[1];
                                if (sourceDetails[2] != null && !string.IsNullOrEmpty(sourceDetails[2]))
                                    int.TryParse(sourceDetails[2], out contactCustomerId);

                                ReportSourceDetails reportTotalSourceDetailModel = new ReportSourceDetails()
                                {
                                    SourceId = sourceId,
                                    SourceCode = soureCode,
                                    ContactCustomerId = contactCustomerId,
                                };
                                reportDetails.Add(reportTotalSourceDetailModel);
                            }
                        }

                        if (reportDetails != null && reportDetails.Any())
                        {
                            reportDetails = reportDetails.GroupBy(e => new
                            {
                                e.ContactCustomerId,
                                e.SourceCode,
                                e.SourceId
                            }).Select(e => new ReportSourceDetails()
                            {
                                SourceId = e.Key.SourceId,
                                SourceCode = e.Key.SourceCode,
                                ContactCustomerId = e.Key.ContactCustomerId
                            }).ToList();
                            return reportDetails;
                        }
                    }
                }
                return null;
            }
        }
    }

    public class ReportSourceDetails
    {
        public int? SourceId { get; set; }
        public string SourceCode { get; set; }
        public int? ContactCustomerId { get; set; }
    }

}
