using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Interface;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class ProjectServiceHistoriesService : DomainService<ProjectServiceHistories, SearchProject>, IProjectServiceHistories
    {
        protected readonly IAppDbContext Context;
        public ProjectServiceHistoriesService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext context) : base(unitOfWork, mapper)
        {
            this.Context = context;
        }
        public async Task<List<ProjectServiceHistories>> ListProjectServiceHistories(int ProjectService)
        {
            return await this.unitOfWork.Repository<ProjectServiceHistories>().GetQueryable().Where(x => x.ProjectServiceId == ProjectService).ToListAsync();
        }

        public async Task<ProjectServiceHistories> DetailProjectServiceHistories(int Id)
        {
            SqlParameter[] parameters = GetSqlParametersProjectServiceHistories(Id);
            return await Task.Run(() =>
            {
                ProjectServiceHistories Model = new ProjectServiceHistories();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = this.GetStoreProcName();
                    command.Parameters.AddRange(parameters);
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    Model = MappingDataTable.ConvertToList<ProjectServiceHistories>(dataTable).FirstOrDefault();
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
        protected override string GetStoreProcName()
        {
            return "Mona_Load_Detail_ProjectServiceHistories_ByID";
        }

        private SqlParameter[] GetSqlParametersProjectServiceHistories(int ID)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", ID),

            };
            return parameters;
        }
    }
}
