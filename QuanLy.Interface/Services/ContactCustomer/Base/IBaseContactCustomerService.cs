using App.Core.Interface;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Interface
{
    public interface IBaseContactCustomerService<E, F>: IDomainService<E, F>
        where E: BaseContactCustomer
        where F : BaseSearchContactCustomer
    {
    }
}
