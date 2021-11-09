using App.Core.Entities.DomainEntity;
using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using QuanLy.Interface.Services.Catalogue;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service.Services.Catalogue
{
    public class ServiceTypeService : CatalogueService<ServiceTypes, BaseSearch>, IServiceTypes
    {
        protected readonly IAppDbContext Context;
        public ServiceTypeService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext context) : base(unitOfWork, mapper)
        {
            this.Context = context;
        }
        protected override string GetStoreProcName()
        {
            return "Mona_Load_ServiceType";
        }

        protected override SqlParameter[] GetSqlParameters(BaseSearch baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
            };
            return parameters;
        }

        public override async Task<string> GetExistItemMessage(ServiceTypes item)
        {
            string result = string.Empty;
            if(string.IsNullOrEmpty(item.Code) || string.IsNullOrEmpty(item.Name))
            {
                return "Thông tin không được để trống";
            }
            bool isExistCode = await this.unitOfWork.Repository<ServiceTypes>().GetQueryable().AnyAsync(x => x.Id != item.Id 
            && x.Code == item.Code && !x.Deleted && x.Active);
            if (isExistCode)
                return "Mã đã tồn tại!";
            return result;
        }

        public async Task<bool> CheckServiceTypeExistProjectService(int id)
        {
            return await this.unitOfWork.Repository<ProjectServices>().GetQueryable().AnyAsync(x => x.Id == id && !x.Deleted && x.Active);
        }
        /// <summary>
        /// Lấy danh sách
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        public override async Task<PagedList<ServiceTypes>> GetPagedListData(BaseSearch baseSearch)
        {
            PagedList<ServiceTypes> pagedList = new PagedList<ServiceTypes>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await ExcuteQueryPagingAsync(this.GetStoreProcName(), parameters);
            pagedList.PageIndex = baseSearch.PageIndex;
            pagedList.PageSize = baseSearch.PageSize;
            return pagedList;
        }

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        private async Task<PagedList<ServiceTypes>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return await Task.Run(() =>
            {
                PagedList<ServiceTypes> pagedList = new PagedList<ServiceTypes>();
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
                    pagedList.Items = MappingDataTable.ConvertToList<ServiceTypes>(dataTable);
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
    }
}
