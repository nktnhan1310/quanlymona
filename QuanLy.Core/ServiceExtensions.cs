using App.Core.Interface.DbContext;
using App.Core.Interface.Repository;
using App.Core.Interface.Services;
using App.Core.Interface.Services.Auth;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using QuanLy.AppDbContext;
using QuanLy.Interface;
using QuanLy.Interface.Services;
using QuanLy.Interface.Services.Catalogue;
using QuanLy.Interface.Services.Newfeed;
using QuanLy.Service;
using QuanLy.Service.Services.Catalogue;
using QuanLy.Service.Services.Newfeed;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLy.Core
{
    public static class ServiceExtensions
    {
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IAppDbContext, MonaDbContext>();
            //services.AddScoped(typeof(IDomainRepository<>), typeof(DomainRepository<>));
            //services.AddScoped(typeof(ICatalogueRepository<>), typeof(CatalogueRepository<>));
            //services.AddScoped(typeof(IAppRepository<>), typeof(AppRepository<>));
            //services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
        }

        public static void ConfigureService(this IServiceCollection services)
        {
            #region Reports

            services.AddScoped<IReportSourceService, ReportSourceService>();

            #endregion

            #region Projects

            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectUserService, ProjectUserService>();
            services.AddScoped<IProjectFileService, ProjectFileService>();
            services.AddScoped<IProjectServiceTypeService, ProjectServiceTypeService>();
            services.AddScoped<IProjectSessionPayService, ProjectSessionPayService>();
            services.AddScoped<IProjectTaskService, ProjectTaskService>();
            services.AddScoped<IProjectCommentService, ProjectCommentService>();
            services.AddScoped<IProjectServiceService, ProjectServiceService>();
            services.AddScoped<IProjectServiceHistories, ProjectServiceHistoriesService>();

            #endregion

            #region Configuration

            services.AddScoped<IDeviceAppService, DeviceAppService>();
            services.AddScoped<IDeviceBrowserService, DeviceBrowserService>();
            services.AddScoped<IHolidayConfigService, HolidayConfigService>();

            #endregion


            #region Authenticate

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserInGroupService, UserInGroupService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IPermitObjectService, PermitObjectService>();

            #endregion

            #region Contact Customer

            services.AddScoped<IContactCustomerService, ContactCustomerService>();
            services.AddScoped<IContactCustomerMappingRequestService, ContactCustomerMappingRequestService>();
            services.AddScoped<IContactCustomerMappingUserService, ContactCustomerMappingUserService>();
            services.AddScoped<IContactCustomerNoteService, ContactCustomerNoteService>();
            services.AddScoped<IContactCustomerFileService, ContactCustomerFileService>();
            services.AddScoped<IContactCustomerSaleRequestService, ContactCustomerSaleRequestService>();
            services.AddScoped<IContactCustomerServiceTypeService, ContactCustomerServiceTypeService>();

            #endregion

            #region Catalogue


            services.AddScoped<IColorTaskService, ColorTaskService>();
            services.AddScoped<ITaskTypeService, TaskTypeService>();
            services.AddScoped<ISourceProjectTypeService, SourceProjectTypeService>();
            services.AddScoped<IProjectTypeService, ProjectTypeService>();
            services.AddScoped<IRequestTypeService, RequestTypeService>();
            services.AddScoped<ISourceTypeService, SourceTypeService>();
            services.AddScoped<ICampaignSourceService, CampaignSourceService>();
            services.AddScoped<ICampaignMediumService, CampaignMediumService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IProjectStatusService, ProjectStatusService>();
            services.AddScoped<IProjectCustomerStatusService, ProjectCustomerStatusService>();
            services.AddScoped<IServiceTypes, ServiceTypeService>();
            services.AddScoped<IPostContents, PostContentService>();
            services.AddScoped<IPostComments, PostCommentService>();
            services.AddScoped<ICategoryMappingAccount, CategoryMappingAccountService>();
            services.AddScoped<IEmailTemplate, EmailTemplateService>();

            #endregion

            #region WEB HOOKS

            services.AddScoped<IWebHookPhoneHistoryService, WebHookPhoneHistoryService>();
            services.AddScoped<IWebHookTawkHistoryService, WebHookTawkHistoryService>();
            services.AddScoped<IWebHookPhoneHistoryService, WebHookPhoneHistoryService>();
            services.AddScoped<IWebHookFormHistoryService, WebHookFormHistoryService>();

            #endregion

            #region NotificationSingle
            services.AddScoped<INotificationSingle, NotificationSingleService>();

            #endregion

            #region ProjectToDoList
            services.AddScoped<IProjectToDoList, ProjectToDoListService>();
            #endregion

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Quan Ly API", Version = "v1" });
            //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        In = ParameterLocation.Header,
            //        Description = "Please insert JWT with Bearer into field",
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.Http,
            //        Scheme = "bearer"
            //    });
            //    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            //       {
            //         new OpenApiSecurityScheme
            //         {
            //           Reference = new OpenApiReference
            //           {
            //             Type = ReferenceType.SecurityScheme,
            //             Id = "Bearer"
            //           }
            //          },
            //          new string[] { }
            //        }
            //      });

            //    var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
            //    foreach (var fi in dir.EnumerateFiles("*.xml"))
            //    {
            //        c.IncludeXmlComments(fi.FullName);
            //    }

            //    c.EnableAnnotations();
            //});


        }

        public static void MigrationDatabase(this IServiceProvider services, IConfiguration configuration)
        {
            using (var context = services.GetRequiredService<MonaDbContext>())
            {
                context.Database.Migrate();
            }
        }
    }
}
