using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service.Services.Newfeed
{
    public class PostContentService : DomainService<PostContents, BaseSearch>, IPostContents
    {
        protected readonly IAppDbContext Context;
        public PostContentService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext context) : base(unitOfWork, mapper)
        {
            this.Context = context;
        }
        protected override string GetStoreProcName()
        {
            return "Mona_sp_Load_PostContent";
        }

        ///// <summary>
        ///// Lấy danh sách
        ///// </summary>
        ///// <param name="baseSearch"></param>
        ///// <returns></returns>
        //protected override SqlParameter[] GetSqlParameters(BaseSearch baseSearch)
        //{
        //    SqlParameter[] parameters =
        //    {
        //        new SqlParameter("@PageIndex", baseSearch.PageIndex),
        //        new SqlParameter("@PageSize", baseSearch.PageSize),
        //        new SqlParameter("@SearchContent", baseSearch.SearchContent),
        //        new SqlParameter("@OrderBy", baseSearch.OrderBy),
        //    };
        //    return parameters;
        //}
        //public override async Task<PagedList<PostContents>> GetPagedListData(BaseSearch baseSearch)
        //{
        //    PagedList<PostContents> pagedList = new PagedList<PostContents>();
        //    SqlParameter[] parameters = GetSqlParameters(baseSearch);
        //    pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
        //    pagedList.PageIndex = baseSearch.PageIndex;
        //    pagedList.PageSize = baseSearch.PageSize;
        //    return pagedList;
        //}

        ///// <summary>
        ///// Lấy danh sách phân trang
        ///// </summary>
        ///// <param name="commandText"></param>
        ///// <param name="sqlParameters"></param>
        ///// <returns></returns>
        //private async Task<PagedList<PostContents>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        //{
        //    return await Task.Run(() =>
        //    {
        //        PagedList<PostContents> pagedList = new PagedList<PostContents>();
        //        DataTable dataTable = new DataTable();
        //        SqlConnection connection = null;
        //        SqlCommand command = null;
        //        try
        //        {
        //            connection = (SqlConnection)Context.Database.GetDbConnection();
        //            command = connection.CreateCommand();
        //            connection.Open();
        //            command.CommandText = commandText;
        //            command.Parameters.AddRange(sqlParameters);
        //            //command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;
        //            command.CommandType = CommandType.StoredProcedure;
        //            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
        //            sqlDataAdapter.Fill(dataTable);
        //            //pagedList.TotalItem = int.Parse(command.Parameters["@TotalPage"].Value.ToString());
        //            pagedList.Items = MappingDataTable.ConvertToList<PostContents>(dataTable);
        //            if (pagedList.Items.Any())
        //                pagedList.TotalItem = pagedList.Items.FirstOrDefault().TotalItem;
        //            return pagedList;
        //        }
        //        finally
        //        {
        //            if (connection != null && connection.State == System.Data.ConnectionState.Open)
        //                connection.Close();

        //            if (command != null)
        //                command.Dispose();
        //        }
        //    });
        //}
    }
}
