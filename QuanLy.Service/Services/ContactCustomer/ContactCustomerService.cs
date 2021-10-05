using App.Core.Extensions;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Entities.Search;
using QuanLy.Interface.Services;
using QuanLy.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class ContactCustomerService : DomainService<ContactCustomers, SearchContactCustomer>, IContactCustomerService
    {
        public ContactCustomerService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "ContactCustomer_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(SearchContactCustomer baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),

                new SqlParameter("@Status", baseSearch.Status),
                new SqlParameter("@SaleStatus", baseSearch.SaleStatus),
                new SqlParameter("@SourceId", (baseSearch.SourceIds != null && baseSearch.SourceIds.Any()) ? string.Join(',', baseSearch.SourceIds) : string.Empty),
                new SqlParameter("@FromDate", baseSearch.FromDate),
                new SqlParameter("@ToDate", baseSearch.ToDate),
                new SqlParameter("@SaleIds", (baseSearch.SaleIds != null && baseSearch.SaleIds.Any()) ? string.Join(',', baseSearch.SaleIds) : string.Empty),



                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        public override async Task<bool> CreateAsync(ContactCustomers item)
        {
            bool result = false;
            if (item != null)
            {
                await this.unitOfWork.Repository<ContactCustomers>().CreateAsync(item);
                await unitOfWork.SaveAsync();
                // Thêm mới mapping yêu cầu
                if (item.ContactCustomerMappingRequests != null && item.ContactCustomerMappingRequests.Any())
                {
                    foreach (var mappingRequest in item.ContactCustomerMappingRequests)
                    {
                        mappingRequest.ContactCustomerId = item.Id;
                        mappingRequest.Created = DateTime.UtcNow.AddHours(7);
                        mappingRequest.CreatedBy = item.CreatedBy;
                        await this.unitOfWork.Repository<ContactCustomerMappingRequests>().CreateAsync(mappingRequest);
                    }
                }
                // Thêm mới dịch vụ
                if (item.ContactCustomerServices != null && item.ContactCustomerServices.Any())
                {
                    foreach (var customerService in item.ContactCustomerServices)
                    {
                        customerService.ContactCustomerId = item.Id;
                        customerService.Created = DateTime.UtcNow.AddHours(7);
                        customerService.CreatedBy = item.CreatedBy;
                        await this.unitOfWork.Repository<ContactCustomerServices>().CreateAsync(customerService);
                    }
                }
                // Thêm mới ghi chú
                if (item.ContactCustomerNotes != null && item.ContactCustomerNotes.Any())
                {
                    foreach (var customerNote in item.ContactCustomerNotes)
                    {
                        customerNote.ContactCustomerId = item.Id;
                        customerNote.Created = DateTime.UtcNow.AddHours(7);
                        customerNote.CreatedBy = item.CreatedBy;
                        await this.unitOfWork.Repository<ContactCustomerNotes>().CreateAsync(customerNote);
                    }
                }
                // Thêm mới yêu cầu liên hệ
                if (item.ContactCustomerSaleRequests != null && item.ContactCustomerSaleRequests.Any())
                {
                    foreach (var customerSaleRequest in item.ContactCustomerSaleRequests)
                    {
                        customerSaleRequest.ContactCustomerId = item.Id;
                        customerSaleRequest.Created = DateTime.UtcNow.AddHours(7);
                        customerSaleRequest.CreatedBy = item.CreatedBy;
                        await this.unitOfWork.Repository<ContactCustomerSaleRequests>().CreateAsync(customerSaleRequest);
                    }
                }
                // Thêm mới file thông tin liên hệ
                if (item.ContactCustomerFiles != null && item.ContactCustomerFiles.Any())
                {
                    foreach (var customerFile in item.ContactCustomerFiles)
                    {
                        customerFile.ContactCustomerId = item.Id;
                        customerFile.Created = DateTime.UtcNow.AddHours(7);
                        customerFile.CreatedBy = item.CreatedBy;
                        await this.unitOfWork.Repository<ContactCustomerFiles>().CreateAsync(customerFile);
                    }
                }

                await unitOfWork.SaveAsync();
                result = true;
            }

            return result;
            //return base.CreateAsync(item);
        }

        /// <summary>
        /// Cập nhật thông tin liên hệ
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(ContactCustomers item)
        {
            bool result = false;
            var existItem = await this.unitOfWork.Repository<ContactCustomers>().GetQueryable().Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem != null)
            {
                var currentCustomerDate = existItem.Created;
                var currentCustomerCreatedBy = existItem.CreatedBy;
                existItem = mapper.Map<ContactCustomers>(item);
                existItem.Created = currentCustomerDate;
                existItem.CreatedBy = currentCustomerCreatedBy;
                this.unitOfWork.Repository<ContactCustomers>().Update(existItem);

                // Cập nhật thông tin yêu cầu
                if (item.ContactCustomerMappingRequests != null && item.ContactCustomerMappingRequests.Any())
                {
                    foreach (var mappingRequest in item.ContactCustomerMappingRequests)
                    {
                        var existMappingRequest = await this.unitOfWork.Repository<ContactCustomerMappingRequests>().GetQueryable().Where(e => e.Id == mappingRequest.Id).FirstOrDefaultAsync();
                        if (existMappingRequest != null)
                        {
                            var currentMappingRequestDate = existMappingRequest.Created;
                            var currentMappingRequestCreatedBy = existMappingRequest.CreatedBy;
                            existMappingRequest = mapper.Map<ContactCustomerMappingRequests>(mappingRequest);
                            existMappingRequest.Created = currentMappingRequestDate;
                            existMappingRequest.CreatedBy = currentMappingRequestCreatedBy;
                            
                            existMappingRequest.Updated = DateTime.UtcNow.AddHours(7);
                            existMappingRequest.UpdatedBy = item.UpdatedBy;
                            existMappingRequest.ContactCustomerId = item.Id;
                            this.unitOfWork.Repository<ContactCustomerMappingRequests>().Update(existMappingRequest);
                        }
                        else
                        {
                            mappingRequest.Created = DateTime.UtcNow.AddHours(7);
                            mappingRequest.CreatedBy = item.UpdatedBy;
                            mappingRequest.ContactCustomerId = item.Id;
                            await this.unitOfWork.Repository<ContactCustomerMappingRequests>().CreateAsync(mappingRequest);
                        }
                    }
                }

                // Cập nhật thông tin file
                if (item.ContactCustomerFiles != null && item.ContactCustomerFiles.Any())
                {
                    foreach (var contactCustomerFile in item.ContactCustomerFiles)
                    {
                        var existContactCustomerFile = await this.unitOfWork.Repository<ContactCustomerFiles>().GetQueryable().Where(e => e.Id == contactCustomerFile.Id).FirstOrDefaultAsync();
                        if (existContactCustomerFile != null)
                        {
                            var currentContactCustomerDate = existContactCustomerFile.Created;
                            var currentContactCustomerCreatedBy = existContactCustomerFile.CreatedBy;
                            existContactCustomerFile = mapper.Map<ContactCustomerFiles>(contactCustomerFile);
                            existContactCustomerFile.Created = currentContactCustomerDate;
                            existContactCustomerFile.CreatedBy = currentContactCustomerCreatedBy;
                            existContactCustomerFile.Updated = DateTime.UtcNow.AddHours(7);
                            existContactCustomerFile.UpdatedBy = item.UpdatedBy;
                            existContactCustomerFile.ContactCustomerId = item.Id;
                            this.unitOfWork.Repository<ContactCustomerFiles>().Update(existContactCustomerFile);
                        }
                        else
                        {
                            contactCustomerFile.Created = DateTime.UtcNow.AddHours(7);
                            contactCustomerFile.CreatedBy = item.UpdatedBy;
                            contactCustomerFile.ContactCustomerId = item.Id;
                            await this.unitOfWork.Repository<ContactCustomerFiles>().CreateAsync(contactCustomerFile);
                        }
                    }
                }

                // Cập nhật thông tin dịch vụ
                if (item.ContactCustomerServices != null && item.ContactCustomerServices.Any())
                {
                    foreach (var contactCustomerService in item.ContactCustomerServices)
                    {
                        var existContactCustomerService = await this.unitOfWork.Repository<ContactCustomerServices>().GetQueryable().Where(e => e.Id == contactCustomerService.Id).FirstOrDefaultAsync();
                        if (existContactCustomerService != null)
                        {
                            var currentContactCustomerDate = existContactCustomerService.Created;
                            var currentContactCustomerCreatedBy = existContactCustomerService.CreatedBy;
                            existContactCustomerService = mapper.Map<ContactCustomerServices>(contactCustomerService);
                            existContactCustomerService.Created = currentContactCustomerDate;
                            existContactCustomerService.CreatedBy = currentContactCustomerCreatedBy;

                            existContactCustomerService.Updated = DateTime.UtcNow.AddHours(7);
                            existContactCustomerService.UpdatedBy = item.UpdatedBy;
                            existContactCustomerService.ContactCustomerId = item.Id;
                            this.unitOfWork.Repository<ContactCustomerServices>().Update(existContactCustomerService);
                        }
                        else
                        {
                            contactCustomerService.Created = DateTime.UtcNow.AddHours(7);
                            contactCustomerService.CreatedBy = item.UpdatedBy;
                            contactCustomerService.ContactCustomerId = item.Id;
                            await this.unitOfWork.Repository<ContactCustomerServices>().CreateAsync(contactCustomerService);
                        }
                    }
                }

                // Cập nhật thông tin ghi chú
                if (item.ContactCustomerNotes != null && item.ContactCustomerNotes.Any())
                {
                    foreach (var contactCustomerNote in item.ContactCustomerNotes)
                    {
                        var existContactCustomerNote = await this.unitOfWork.Repository<ContactCustomerNotes>().GetQueryable().Where(e => e.Id == contactCustomerNote.Id).FirstOrDefaultAsync();
                        if (existContactCustomerNote != null)
                        {
                            var currentContactCustomerDate = existContactCustomerNote.Created;
                            var currentContactCustomerCreatedBy = existContactCustomerNote.CreatedBy;
                            existContactCustomerNote = mapper.Map<ContactCustomerNotes>(contactCustomerNote);
                            existContactCustomerNote.Created = currentContactCustomerDate;
                            existContactCustomerNote.CreatedBy = currentContactCustomerCreatedBy;
                            existContactCustomerNote.Updated = DateTime.UtcNow.AddHours(7);
                            existContactCustomerNote.UpdatedBy = item.UpdatedBy;
                            existContactCustomerNote.ContactCustomerId = item.Id;
                            this.unitOfWork.Repository<ContactCustomerNotes>().Update(existContactCustomerNote);
                        }
                        else
                        {
                            contactCustomerNote.Created = DateTime.UtcNow.AddHours(7);
                            contactCustomerNote.CreatedBy = item.UpdatedBy;
                            contactCustomerNote.ContactCustomerId = item.Id;
                            await this.unitOfWork.Repository<ContactCustomerNotes>().CreateAsync(contactCustomerNote);
                        }
                    }
                }

                // Cập nhật thông tin yêu cầu liên hệ
                if (item.ContactCustomerSaleRequests != null && item.ContactCustomerSaleRequests.Any())
                {
                    foreach (var contactCustomerSaleRequest in item.ContactCustomerSaleRequests)
                    {
                        var existContactCustomerSaleRequest = await this.unitOfWork.Repository<ContactCustomerSaleRequests>().GetQueryable().Where(e => e.Id == contactCustomerSaleRequest.Id).FirstOrDefaultAsync();
                        if (existContactCustomerSaleRequest != null)
                        {
                            var currentContactCustomerDate = existContactCustomerSaleRequest.Created;
                            var currentContactCustomerCreatedBy = existContactCustomerSaleRequest.CreatedBy;
                            existContactCustomerSaleRequest = mapper.Map<ContactCustomerSaleRequests>(contactCustomerSaleRequest);
                            existContactCustomerSaleRequest.Created = currentContactCustomerDate;
                            existContactCustomerSaleRequest.CreatedBy = currentContactCustomerCreatedBy;
                            existContactCustomerSaleRequest.Updated = DateTime.UtcNow.AddHours(7);
                            existContactCustomerSaleRequest.UpdatedBy = item.UpdatedBy;
                            existContactCustomerSaleRequest.ContactCustomerId = item.Id;
                            this.unitOfWork.Repository<ContactCustomerSaleRequests>().Update(existContactCustomerSaleRequest);
                        }
                        else
                        {
                            contactCustomerSaleRequest.Created = DateTime.UtcNow.AddHours(7);
                            contactCustomerSaleRequest.CreatedBy = item.UpdatedBy;
                            contactCustomerSaleRequest.ContactCustomerId = item.Id;
                            await this.unitOfWork.Repository<ContactCustomerSaleRequests>().CreateAsync(contactCustomerSaleRequest);
                        }
                    }
                }

                await this.unitOfWork.SaveAsync();
                result = true;
            }
            return result;
            //return base.UpdateAsync(item);
        }

        /// <summary>
        /// Lấy thông tin liên hệ theo phone/email
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ContactCustomers> GetExistContactCustomer(string phone, string email)
        {
            ContactCustomers contactCustomers = null;
            contactCustomers = await this.unitOfWork.Repository<ContactCustomers>().GetQueryable()
                .Where(e => !e.Deleted && e.Active
                && (string.IsNullOrEmpty(phone) || e.Phone == phone)
                && (string.IsNullOrEmpty(email) || e.Email.ToLower() == email.ToLower())
                && (e.Created.HasValue && e.Created.Value.Date == DateTime.UtcNow.AddHours(7).Date)
                ).FirstOrDefaultAsync();
            return contactCustomers;
        }

        /// <summary>
        /// Đếm tổng số lượng contact customer
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotalContactCustomer()
        {
            return await this.unitOfWork.Repository<ContactCustomers>().GetQueryable().Where(e => !e.Deleted && e.Active).CountAsync();
        }

        /// <summary>
        /// Merge thông tin liên hệ
        /// </summary>
        /// <param name="contactCustomerIds"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public async Task<bool> MergeContactCustomers(List<int> contactCustomerIds, string createdBy)
        {
            bool result = false;
            var contactCustomerInfos = await this.unitOfWork.Repository<ContactCustomers>().GetQueryable().Where(e => contactCustomerIds.Contains(e.Id)).ToListAsync();
            if (contactCustomerInfos != null && contactCustomerInfos.Any())
            {
                var groupPhoneContacts = contactCustomerInfos.GroupBy(e => e.Phone).ToList();
                // Kiểm tra thông tin liên hệ có số điện thoại giống nhau chưa?
                if (groupPhoneContacts == null || groupPhoneContacts.Count >= 2)
                    throw new AppException("Vui lòng chọn liên hệ có số điện thoại giống nhau");
                // Lưu thông tin liên hệ mới sau khi gộp
                ContactCustomers contactCustomer = groupPhoneContacts
                    .Select(e => new ContactCustomers()
                    {
                        CreatedBy = createdBy,
                        Updated = DateTime.UtcNow.AddHours(7),
                        Created = DateTime.UtcNow.AddHours(7),
                        UpdatedBy = createdBy,
                        FullName = e.FirstOrDefault().FullName,
                        Phone = e.Key,
                        Email = e.FirstOrDefault().Email,
                        Status = (int)CatalogueEnums.ContactCustomerStatus.UnConnected,
                        SaleStatus = (int)CatalogueEnums.SaleContactCustomerStatus.UnConnected,
                        Deleted = false,
                        Active = true,
                        ExpectedMoney = e.Where(x => x.ExpectedMoney.HasValue).FirstOrDefault() != null ? e.Where(x => x.ExpectedMoney.HasValue).FirstOrDefault().ExpectedMoney : null,
                        CompanyId = e.Where(x => x.CompanyId.HasValue).FirstOrDefault() != null ? e.Where(x => x.CompanyId.HasValue).FirstOrDefault().CompanyId : null,
                        ParentCompanyId = e.Where(x => x.ParentCompanyId.HasValue).FirstOrDefault() != null ? e.Where(x => x.ParentCompanyId.HasValue).FirstOrDefault().ParentCompanyId : null,
                        UserId = e.Where(x => x.UserId.HasValue).FirstOrDefault() != null ? e.Where(x => x.UserId.HasValue).FirstOrDefault().UserId : null,
                        Address = e.Where(x => !string.IsNullOrEmpty(x.Address)).FirstOrDefault() != null ? e.Where(x => !string.IsNullOrEmpty(x.Address)).FirstOrDefault().Address : string.Empty,
                        BirthDate = e.Where(x => x.BirthDate.HasValue).FirstOrDefault() != null ? e.Where(x => x.BirthDate.HasValue).FirstOrDefault().BirthDate : null,
                        Gender = e.Where(x => x.Gender.HasValue).FirstOrDefault() != null ? e.Where(x => x.Gender.HasValue).FirstOrDefault().Gender : null,
                        JobDescription = e.Where(x => !string.IsNullOrEmpty(x.JobDescription)).FirstOrDefault() != null ? e.Where(x => !string.IsNullOrEmpty(x.JobDescription)).FirstOrDefault().JobDescription : string.Empty,
                        SaleId = e.Where(x => x.SaleId.HasValue).FirstOrDefault() != null ? e.Where(x => x.SaleId.HasValue).FirstOrDefault().SaleId : null,
                    }).FirstOrDefault();

                await this.unitOfWork.Repository<ContactCustomers>().CreateAsync(contactCustomer);
                await this.unitOfWork.SaveAsync();
                // Ẩn liên hệ cũ
                foreach (var contactCustomerInfo in contactCustomerInfos)
                {
                    contactCustomerInfo.Deleted = true;
                    contactCustomerInfo.Updated = DateTime.UtcNow.AddHours(7);
                    contactCustomerInfo.UpdatedBy = createdBy;
                    Expression<Func<ContactCustomers, object>>[] includeProperties = new Expression<Func<ContactCustomers, object>>[]
                    {
                        e => e.Deleted,
                        e => e.Updated,
                        e => e.UpdatedBy
                    };
                    await this.unitOfWork.Repository<ContactCustomers>().UpdateFieldsSaveAsync(contactCustomerInfo, includeProperties);
                }
                // Lấy tất cả mapping yêu cầu để gộp
                var mappingRequestOlds = await this.unitOfWork.Repository<ContactCustomerMappingRequests>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active && e.ContactCustomerId.HasValue
                    && contactCustomerIds.Contains(e.ContactCustomerId.Value)
                    ).ToListAsync();
                // Group lại theo nguồn và yêu cầu tổng hợp ra danh sách liên hệ mới
                if (mappingRequestOlds != null && mappingRequestOlds.Any())
                {
                    var mappingRequestNews = mappingRequestOlds.GroupBy(e => new
                    {
                        e.RequestId,
                        e.SourceId,
                    }).Select(e => new ContactCustomerMappingRequests()
                    {
                        RequestId = e.Key.RequestId,
                        SourceId = e.Key.SourceId,
                        SourceName = e.FirstOrDefault().SourceName,
                        ContactCustomerId = contactCustomer.Id,
                        Deleted = false,
                        Active = true,
                        CreatedBy = createdBy,
                        Created = DateTime.UtcNow.AddHours(7)
                    }).ToList();
                    // Thêm thông tin liên hệ mới
                    await this.unitOfWork.Repository<ContactCustomerMappingRequests>().CreateAsync(mappingRequestNews);

                    // Ẩn tất cả mapping yêu cầu từ liên hệ cũ
                    foreach (var mappingRequestOld in mappingRequestOlds)
                    {
                        mappingRequestOld.Deleted = true;
                        mappingRequestOld.Updated = DateTime.UtcNow.AddHours(7);
                        mappingRequestOld.UpdatedBy = createdBy;

                        Expression<Func<ContactCustomerMappingRequests, object>>[] includeProperties = new Expression<Func<ContactCustomerMappingRequests, object>>[]
                        {
                            e => e.Deleted,
                            e => e.Updated,
                            e => e.UpdatedBy
                        };
                        await this.unitOfWork.Repository<ContactCustomerMappingRequests>().UpdateFieldsSaveAsync(mappingRequestOld, includeProperties);
                    }

                }
                // Lấy ra tất cả lịch sử web hook gộp vào thông tin liên hệ mới
                // ----------------- LỊCH SỬ WEB HOOK TAWK
                var webHookTawkHistories = await this.unitOfWork.Repository<WebHookTawkHistories>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active
                    && e.ContactCustomerId.HasValue
                    && contactCustomerIds.Contains(e.ContactCustomerId.Value)
                    ).ToListAsync();
                // Cập nhật lại id cho liên hệ mới
                if (webHookTawkHistories != null && webHookTawkHistories.Any())
                {
                    foreach (var webHookTawkHistory in webHookTawkHistories)
                    {
                        webHookTawkHistory.ContactCustomerId = contactCustomer.Id;
                        Expression<Func<WebHookTawkHistories, object>>[] includeProperties = new Expression<Func<WebHookTawkHistories, object>>[]
                        {
                            e => e.ContactCustomerId
                        };
                        await this.unitOfWork.Repository<WebHookTawkHistories>().UpdateFieldsSaveAsync(webHookTawkHistory, includeProperties);
                    }
                }
                // ----------------- LỊCH SỬ WEB HOOK PHONE
                var webHookPhoneHistories = await this.unitOfWork.Repository<WebHookPhoneHistories>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active
                    && e.ContactCustomerId.HasValue
                    && contactCustomerIds.Contains(e.ContactCustomerId.Value)
                    ).ToListAsync();
                // Cập nhật lại id cho liên hệ mới
                if (webHookPhoneHistories != null && webHookPhoneHistories.Any())
                {
                    foreach (var webHookPhoneHistory in webHookPhoneHistories)
                    {
                        webHookPhoneHistory.ContactCustomerId = contactCustomer.Id;
                        Expression<Func<WebHookPhoneHistories, object>>[] includeProperties = new Expression<Func<WebHookPhoneHistories, object>>[]
                        {
                            e => e.ContactCustomerId
                        };
                        await this.unitOfWork.Repository<WebHookPhoneHistories>().UpdateFieldsSaveAsync(webHookPhoneHistory, includeProperties);
                    }
                }

                // ----------------- LỊCH SỬ WEB HOOK PHONE
                var webHookFormHistories = await this.unitOfWork.Repository<WebHookFormHistories>().GetQueryable()
                    .Where(e => !e.Deleted && e.Active
                    && e.ContactCustomerId.HasValue
                    && contactCustomerIds.Contains(e.ContactCustomerId.Value)
                    ).ToListAsync();
                // Cập nhật lại id cho liên hệ mới
                if (webHookFormHistories != null && webHookFormHistories.Any())
                {
                    foreach (var webHookFormHistory in webHookFormHistories)
                    {
                        webHookFormHistory.ContactCustomerId = contactCustomer.Id;
                        Expression<Func<WebHookFormHistories, object>>[] includeProperties = new Expression<Func<WebHookFormHistories, object>>[]
                        {
                            e => e.ContactCustomerId
                        };
                        await this.unitOfWork.Repository<WebHookFormHistories>().UpdateFieldsSaveAsync(webHookFormHistory, includeProperties);
                    }
                }
                await this.unitOfWork.SaveAsync();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Đếm tổng số liên hệ
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountTotalContact()
        {
            return await this.unitOfWork.Repository<ContactCustomers>().GetQueryable().Where(e => !e.Deleted && e.Active).CountAsync();
        }
    }
}
