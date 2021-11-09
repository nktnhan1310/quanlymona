using App.Core.Entities;
using App.Core.Entities.Configuration;
using App.Core.Interface.DbContext;
using Microsoft.EntityFrameworkCore;
using QuanLy.Entities;
using QuanLy.Entities.Catalogue;
using QuanLy.Entities.Newfeed;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace QuanLy.AppDbContext
{
    public class MonaDbContext : DbContext, IAppDbContext
    {
        public MonaDbContext(DbContextOptions<MonaDbContext> options) : base(options)
        {

        }

        public DbQuery<TQuery> Query<TQuery>() where TQuery : class
        {
            throw new NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Authentication

            modelBuilder.Entity<UserCores>(x => x.ToTable("Users"));
            modelBuilder.Entity<UserFileCores>(x => x.ToTable("UserFiles"));
            modelBuilder.Entity<UserGroupCores>(x => x.ToTable("UserGroups"));
            modelBuilder.Entity<UserInGroupCores>(x => x.ToTable("UserInGroups"));
            modelBuilder.Entity<PermissionCores>(x => x.ToTable("Permissions"));
            modelBuilder.Entity<PermitObjectCores>(x => x.ToTable("PermitObjects"));
            modelBuilder.Entity<PermitObjectPermissionCores>(x => x.ToTable("PermitObjectPermissions"));

            #endregion

            #region Configuration

            modelBuilder.Entity<EmailConfigurationCores>(x => x.ToTable("EmailConfigurations"));
            modelBuilder.Entity<SMSConfigurationCores>(x => x.ToTable("SMSConfigurations"));
            modelBuilder.Entity<SMSEmailTemplateCores>(x => x.ToTable("SMSEmailTemplates"));
            modelBuilder.Entity<OTPHistoryCores>(x => x.ToTable("OTPHistories"));


            #endregion

            #region Catalogue

            modelBuilder.Entity<CountryCores>(x => x.ToTable("Countries"));
            modelBuilder.Entity<DistrictCores>(x => x.ToTable("Districts"));
            modelBuilder.Entity<NationCores>(x => x.ToTable("Nations"));
            modelBuilder.Entity<CityCores>(x => x.ToTable("Cities"));
            modelBuilder.Entity<WardCores>(x => x.ToTable("Wards"));
            modelBuilder.Entity<CampaignMediums>(x => x.ToTable("CampaignMediums"));
            modelBuilder.Entity<CampaignSources>(x => x.ToTable("CampaignSources"));
            modelBuilder.Entity<ColorTasks>(x => x.ToTable("ColorTasks"));
            modelBuilder.Entity<Companies>(x => x.ToTable("Companies"));
            modelBuilder.Entity<ProjectStatuses>(x => x.ToTable("ProjectStatuses"));
            modelBuilder.Entity<ProjectTypes>(x => x.ToTable("ProjectTypes"));
            modelBuilder.Entity<RequestTypes>(x => x.ToTable("RequestTypes"));
            modelBuilder.Entity<SourceProjectTypes>(x => x.ToTable("SourceProjectTypes"));
            modelBuilder.Entity<SourceTypes>(x => x.ToTable("SourceTypes"));
            modelBuilder.Entity<TaskTypes>(x => x.ToTable("TaskTypes"));
            modelBuilder.Entity<HolidayConfigs>(x => x.ToTable("HolidayConfigs"));
            modelBuilder.Entity<ProjectCustomerStatuses>(x => x.ToTable("ProjectCustomerStatuses"));
            modelBuilder.Entity<ServiceTypes>(x => x.ToTable("ServiceTypes"));

            #endregion

            modelBuilder.Entity<ContactCustomers>(x => x.ToTable("ContactCustomers"));
            modelBuilder.Entity<ContactCustomerFiles>(x => x.ToTable("ContactCustomerFiles"));
            modelBuilder.Entity<ContactCustomerNotes>(x => x.ToTable("ContactCustomerNotes"));
            modelBuilder.Entity<ContactCustomerMappingRequests>(x => x.ToTable("ContactCustomerMappingRequests"));
            modelBuilder.Entity<ContactCustomerSaleRequests>(x => x.ToTable("ContactCustomerSaleRequests"));
            modelBuilder.Entity<ContactCustomerServices>(x => x.ToTable("ContactCustomerServices"));
            modelBuilder.Entity<FeedBacks>(x => x.ToTable("FeedBacks"));
            modelBuilder.Entity<ProjectComments>(x => x.ToTable("ProjectComments"));
            modelBuilder.Entity<ProjectDesignFiles>(x => x.ToTable("ProjectDesignFiles"));
            modelBuilder.Entity<ProjectFiles>(x => x.ToTable("ProjectFiles"));
            modelBuilder.Entity<Projects>(x => x.ToTable("Projects"));
            modelBuilder.Entity<ProjectServiceHistories>(x => x.ToTable("ProjectServiceHistories"));
            modelBuilder.Entity<ProjectServices>(x => x.ToTable("ProjectServices"));
            modelBuilder.Entity<ProjectSessionPays>(x => x.ToTable("ProjectSessionPays"));
            modelBuilder.Entity<ProjectTasks>(x => x.ToTable("ProjectTasks"));
            modelBuilder.Entity<ProjectUsers>(x => x.ToTable("ProjectUsers"));
            modelBuilder.Entity<SalaryMonths>(x => x.ToTable("SalaryMonths"));
            modelBuilder.Entity<WebHookFormHistories>(x => x.ToTable("WebHookFormHistories"));
            modelBuilder.Entity<WebHookPhoneHistories>(x => x.ToTable("WebHookPhoneHistories"));
            modelBuilder.Entity<WebHookTawkHistories>(x => x.ToTable("WebHookTawkHistories"));
            modelBuilder.Entity<ProjectToDoList>(x => x.ToTable("ProjectToDoList"));
            modelBuilder.Entity<ContactCustomerMappingUsers>(x => x.ToTable("ContactCustomerMappingUsers"));

            #region Configuration

            modelBuilder.Entity<DeviceApps>(x => x.ToTable("DeviceApps"));
            modelBuilder.Entity<DeviceBrowsers>(x => x.ToTable("DeviceBrowsers"));

            #endregion

            #region NewFeed
            modelBuilder.Entity<PostComments>(x => x.ToTable("PostComments"));
            modelBuilder.Entity<PostContents>(x => x.ToTable("PostContents"));
            #endregion

            #region Notification
            modelBuilder.Entity<NotificationSingles>(x => x.ToTable("NotificationSingles"));
            #endregion
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<UserFileCores> UserFiles { get; set; }

        public DbSet<ContactCustomerMappingUsers> ContactCustomerMappingUsers { get; set; }
        public DbSet<ContactCustomers> ContactCustomers { get; set; }
        public DbSet<ContactCustomerFiles> ContactCustomerFiles { get; set; }
        public DbSet<ContactCustomerNotes> ContactCustomerNotes { get; set; }
        public DbSet<ContactCustomerMappingRequests> ContactCustomerMappingRequests { get; set; }
        public DbSet<ContactCustomerSaleRequests> ContactCustomerSaleRequests { get; set; }
        public DbSet<ContactCustomerServices> ContactCustomerServices { get; set; }
        public DbSet<FeedBacks> FeedBacks { get; set; }
        public DbSet<ProjectComments> ProjectComments { get; set; }
        public DbSet<ProjectDesignFiles> ProjectDesignFiles { get; set; }
        public DbSet<ProjectFiles> ProjectFiles { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<ProjectServiceHistories> ProjectServiceHistories { get; set; }
        public DbSet<ProjectServices> ProjectServices { get; set; }
        public DbSet<ProjectSessionPays> ProjectSessionPays { get; set; }
        public DbSet<ProjectTasks> ProjectTasks { get; set; }
        public DbSet<ProjectUsers> ProjectUsers { get; set; }
        public DbSet<SalaryMonths> SalaryMonths { get; set; }
        public DbSet<ProjectToDoList> ProjectToDoList { get; set; }
        public DbSet<WebHookFormHistories> WebHookFormHistories { get; set; }
        public DbSet<WebHookPhoneHistories> WebHookPhoneHistories { get; set; }
        public DbSet<WebHookTawkHistories> WebHookTawkHistories { get; set; }


        #region Authentication

        public DbSet<UserCores> Users { get; set; }
        public DbSet<UserGroupCores> UserGroups { get; set; }
        public DbSet<UserInGroupCores> UserInGroups { get; set; }
        public DbSet<PermissionCores> Permissions { get; set; }
        public DbSet<PermitObjectCores> PermitObjects { get; set; }
        public DbSet<PermitObjectPermissionCores> PermitObjectPermissions { get; set; }

        #endregion

        #region Configuration

        public DbSet<EmailConfigurationCores> EmailConfigurations { get; set; }
        public DbSet<SMSConfigurationCores> SMSConfigurations { get; set; }
        public DbSet<SMSEmailTemplateCores> SMSEmailTemplates { get; set; }
        public DbSet<OTPHistoryCores> OTPHistories { get; set; }
        public DbSet<DeviceApps> DeviceApps { get; set; }
        public DbSet<DeviceBrowsers> DeviceBrowsers { get; set; }

        #endregion

        #region Catalogues

        public DbSet<ProjectCustomerStatuses> ProjectCustomerStatuses { get; set; }
        public DbSet<HolidayConfigs> HolidayConfigs { get; set; }
        public DbSet<CountryCores> Countries { get; set; }
        public DbSet<DistrictCores> Districts { get; set; }
        public DbSet<NationCores> Nations { get; set; }
        public DbSet<CityCores> Cities { get; set; }
        public DbSet<WardCores> Wards { get; set; }
        public DbSet<CampaignMediums> CampaignMediums { get; set; }
        public DbSet<CampaignSources> CampaignSources { get; set; }
        public DbSet<ColorTasks> ColorTasks { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<ProjectStatuses> ProjectStatuses { get; set; }
        public DbSet<ProjectTypes> ProjectTypes { get; set; }
        public DbSet<RequestTypes> RequestTypes { get; set; }
        public DbSet<SourceProjectTypes> SourceProjectTypes { get; set; }
        public DbSet<SourceTypes> SourceTypes { get; set; }
        public DbSet<TaskTypes> TaskTypes { get; set; }
        public DbSet<ServiceTypes> ServiceTypes { get; set; }
        #endregion

        #region NewFeed

        public DbSet<PostComments> PostComments { get; set; }
        public DbSet<PostContents> PostContents { get; set; }

        #endregion

        #region
        public DbSet<NotificationSingles> NotificationSingles { get; set; }
        #endregion

    }
}
