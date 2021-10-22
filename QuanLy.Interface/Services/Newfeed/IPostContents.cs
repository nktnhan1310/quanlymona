using App.Core.Entities.DomainEntity;
using App.Core.Interface;
using QuanLy.Entities.Newfeed;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Interface.Services.Newfeed
{
    public interface IPostContents : IDomainService<PostContents, BaseSearch>
    {
    }
}
