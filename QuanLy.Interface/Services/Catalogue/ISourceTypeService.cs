using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services.Base;
using QuanLy.Entities.Catalogue;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Interface
{
    public interface ISourceTypeService : ICatalogueService<SourceTypes, BaseSearch>
    {
    }
}
