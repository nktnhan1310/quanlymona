using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Interface;
using App.Core.Interface.Services;
using App.Core.Interface.Services.Base;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Interface
{
    public interface IUserGroupService : ICatalogueService<UserGroups, BaseSearchUserInGroup>
    {
    }
}
