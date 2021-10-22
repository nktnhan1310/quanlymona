using App.Core.Entities.DomainEntity;
using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities.Newfeed;
using QuanLy.Interface.Services.Newfeed;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service.Services.Newfeed
{
    public class PostCommentService : DomainService<PostComments, BaseSearch>, IPostComments
    {
        protected readonly IAppDbContext Context;
        public PostCommentService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext context) : base(unitOfWork, mapper)
        {
            this.Context = context;
        }
        private  string GetStoreProcNameParentComment()
        {
            return "Mona_sp_load_Parent_PostComment";
        }
        /// <summary>
        /// Lấy danh sách Parent Comment
        /// </summary>
        /// <param name="PageIndex" name="PageSize" name="PostConTentID"></param>
        /// <returns></returns>
        private SqlParameter[] GetSqlParametersParentComment(int PageIndex, int PageSize, int PostConTentID)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", PageIndex),
                new SqlParameter("@PageSize", PageSize),
                new SqlParameter("@PostConTentID", PostConTentID),
            };
            return parameters;
        }

        public async Task<PagedList<PostComments>> GetPagedListDataPanrentComment(int PageIndex, int PageSize, int PostConTentID)
        {
            PagedList<PostComments> pagedList = new PagedList<PostComments>();
            SqlParameter[] parameters = GetSqlParametersParentComment(PageIndex, PageSize, PostConTentID);
            pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcNameParentComment(), parameters);
            pagedList.PageIndex = PageIndex;
            pagedList.PageSize = PageSize;
            return pagedList;
        }

        private string GetStoreProcNameChilComment()
        {
            return "Mona_sp_load_Chil_PostComment";
        }
        /// <summary>
        /// Lấy danh sách Chil Comment
        /// </summary>
        /// <param name="PageIndex" name="PageSize" name="PostConTentID" name ="PostCommentID"></param>
        /// <returns></returns>
        private SqlParameter[] GetSqlParametersChilComment(int PageIndex, int PageSize, int PostConTentID, int PostCommentID)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", PageIndex),
                new SqlParameter("@PageSize", PageSize),
                new SqlParameter("@PostConTentID", PostConTentID),
                new SqlParameter("@PostCommentID", PostCommentID),
            };
            return parameters;
        }

        public async Task<PagedList<PostComments>> GetPagedListDataChilComment(int PageIndex, int PageSize, int PostConTentID, int PostCommentID)
        {
            PagedList<PostComments> pagedList = new PagedList<PostComments>();
            SqlParameter[] parameters = GetSqlParametersChilComment(PageIndex, PageSize, PostConTentID, PostCommentID);
            pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcNameChilComment(), parameters);
            pagedList.PageIndex = PageIndex;
            pagedList.PageSize = PageSize;
            return pagedList;
        }

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        private async Task<PagedList<PostComments>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedList<PostComments> pagedList = new PagedList<PostComments>();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = commandText;
                    command.Parameters.AddRange(sqlParameters);
                    //command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    //pagedList.TotalItem = int.Parse(command.Parameters["@TotalPage"].Value.ToString());
                    pagedList.Items = MappingDataTable.ConvertToList<PostComments>(dataTable);
                    if (pagedList.Items.Any())
                        pagedList.TotalItem = pagedList.Items.FirstOrDefault().TotalPage ?? 0;
                    return pagedList;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }
        public async Task<List<PostComments>> ListPostComment(int PostCommentID)
        {
            return await unitOfWork.Repository<PostComments>().GetQueryable().Where(x => x.PostCommentID == PostCommentID && x.Deleted == false).ToListAsync();
        }

        private SqlParameter[] GetSqlParametersDeTailComment(int ID)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", ID),
            };
            return parameters;
        }
        private string GetStoreProcNameDetailComment()
        {
            return "Mona_sp_load_Detail_PostComment_ByID";
        }
        public async Task<PostComments> GetIdPostComment(int ID)
        {
            SqlParameter[] parameters = GetSqlParametersDeTailComment(ID);
            return await Task.Run(() =>
            {
                PostComments Model = new PostComments();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = this.GetStoreProcNameDetailComment();
                    command.Parameters.AddRange(parameters);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    Model = MappingDataTable.ConvertToList<PostComments>(dataTable).FirstOrDefault();
                    return Model;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }
    }
}
