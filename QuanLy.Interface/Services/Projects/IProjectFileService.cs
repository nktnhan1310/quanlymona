using App.Core.Entities.DomainEntity;
using App.Core.Interface;
using QuanLy.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Interface
{
    public interface IProjectFileService : IDomainService<ProjectFiles, BaseSearch>
    {
    }
}
