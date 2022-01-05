using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using QuanLy.Entities;
using QuanLy.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace QuanLy.Service
{
    public abstract class ReportCoreService<R, T> : IReportCoreService<R, T> where R : ReportAppDomain where T : BaseReportSearch, new()
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        protected readonly IAppDbContext Context;
        public ReportCoreService(IUnitOfWork unitOfWork, IMapper mapper, IAppDbContext context)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.Context = context;
        }

        /// <summary>
        /// Lấy danh sách báo cáo
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<R>> GetPagedListReport(T baseSearch)
        {
            PagedList<R> pagedList = new PagedList<R>();
            if (baseSearch.IsExport)
            {
                baseSearch.PageIndex = 1;
                baseSearch.PageSize = int.MaxValue;
            }
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.PageIndex = baseSearch.PageIndex;
            pagedList.PageSize = baseSearch.PageSize;
            return pagedList;
        }

        /// <summary>
        /// Lấy tên store procedure
        /// </summary>
        /// <returns></returns>
        protected virtual string GetStoreProcName()
        {
            return string.Empty;
        }

        /// <summary>
        /// Lấy thông tin sql parameter
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        protected virtual SqlParameter[] GetSqlParameters(T baseSearch)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            foreach (PropertyInfo prop in baseSearch.GetType().GetProperties())
            {
                if (prop.Name == "IsExport") continue;
                sqlParameters.Add(new SqlParameter(prop.Name, prop.GetValue(baseSearch, null)));
            }
            SqlParameter[] parameters = sqlParameters.ToArray();
            return parameters;
        }

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<R>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedList<R> pagedList = new PagedList<R>();
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
                    pagedList.Items = MappingDataTable.ConvertToList<R>(dataTable);
                    if (pagedList.Items != null && pagedList.Items.Any())
                        pagedList.TotalItem = pagedList.Items.FirstOrDefault().TotalItem;
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

    }
}
