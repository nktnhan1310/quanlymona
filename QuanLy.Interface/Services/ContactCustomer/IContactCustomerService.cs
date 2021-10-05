using App.Core.Interface;
using QuanLy.Entities;
using QuanLy.Entities.Search;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface.Services
{
    public interface IContactCustomerService : IDomainService<ContactCustomers, SearchContactCustomer>
    {
        /// <summary>
        /// Lấy thông tin liên hệ theo email/phone
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<ContactCustomers> GetExistContactCustomer(string phone, string email);
        /// <summary>
        /// Lấy tổng số liên hệ khách hàng
        /// </summary>
        /// <returns></returns>
        Task<int> GetTotalContactCustomer();

        /// <summary>
        /// Merge thông tin liên hệ
        /// </summary>
        /// <param name="contactCustomerIds"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        Task<bool> MergeContactCustomers(List<int> contactCustomerIds, string createdBy);

        /// <summary>
        /// Đếm tổng số liên hệ
        /// </summary>
        /// <returns></returns>
        Task<int> CountTotalContact();
    }
}
