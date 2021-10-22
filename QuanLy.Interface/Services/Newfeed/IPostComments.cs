using App.Core.Entities.DomainEntity;
using App.Core.Interface;
using App.Core.Utilities;
using QuanLy.Entities.Newfeed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Interface.Services.Newfeed
{
    public interface IPostComments  : IDomainService<PostComments, BaseSearch>
    {

        /// <summary>
        /// Lấy Store Parent Comment
        /// </summary>
        /// <returns></returns>
        Task<PagedList<PostComments>> GetPagedListDataPanrentComment(int PageIndex, int PageSize, int PostConTentID);

        /// <summary>
        /// Lấy Store Chil Comment
        /// </summary>
        /// <returns></returns>
        Task<PagedList<PostComments>> GetPagedListDataChilComment(int PageIndex, int PageSize, int PostConTentID, int PostCommentID);
        Task<List<PostComments>> ListPostComment(int PostCommentID);
        Task<PostComments> GetIdPostComment(int ID);

    }
}
