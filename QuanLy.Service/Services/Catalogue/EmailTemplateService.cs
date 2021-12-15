using App.Core.Entities.DomainEntity;
using App.Core.Interface.DbContext;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLy.Service
{
    public class EmailTemplateService : CatalogueService<EmailTemplates, BaseSearch>, IEmailTemplate
    {
        protected readonly IAppDbContext Context;
        public EmailTemplateService(IAppUnitOfWork unitOfWork, IMapper mapper
            , IAppDbContext Context) : base(unitOfWork, mapper)
        {
            this.Context = Context;
            this.IsUseStore = true;
        }

        public override async Task<string> GetExistItemMessage(EmailTemplates item)
        {
            var message = "";
            var isExists = await this.unitOfWork.Repository<EmailTemplates>().GetQueryable()
                .Where(x =>
                x.Id != item.Id &&
                x.Code == item.Code)
                .AnyAsync();
            if (isExists)
            {
                message = "Mã email đã tồn tại";
            }
            return message;
        }

        protected override string GetStoreProcName()
        {
            return "Mona_sp_Load_Email_Template_PagingData";
        }
    }
}
