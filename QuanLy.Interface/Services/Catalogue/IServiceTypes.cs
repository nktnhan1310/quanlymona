using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services.Base;
using App.Core.Utilities;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface.Services.Catalogue
{
    public interface IServiceTypes : ICatalogueService<ServiceTypes, BaseSearch>
    {
    }
}
