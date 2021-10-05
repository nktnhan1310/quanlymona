using App.Core.Entities;
using App.Core.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using QuanLy.Interface.DbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace QuanLy.AppDbContext
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbQuery<TQuery> Query<TQuery>() where TQuery : class
        {
            throw new NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Authentication

            modelBuilder.Entity<Users>(x => x.ToTable("Users"));
            modelBuilder.Entity<UserGroups>(x => x.ToTable("UserGroups"));
            modelBuilder.Entity<UserInGroups>(x => x.ToTable("UserInGroups"));
            modelBuilder.Entity<Permissions>(x => x.ToTable("Permissions"));
            modelBuilder.Entity<PermitObjects>(x => x.ToTable("PermitObjects"));
            modelBuilder.Entity<PermitObjectPermissions>(x => x.ToTable("PermitObjectPermissions"));

            #endregion

            #region Configuration

            modelBuilder.Entity<EmailConfigurations>(x => x.ToTable("EmailConfigurations"));
            modelBuilder.Entity<SMSConfigurations>(x => x.ToTable("SMSConfigurations"));
            modelBuilder.Entity<SMSEmailTemplates>(x => x.ToTable("SMSEmailTemplates"));
            modelBuilder.Entity<OTPHistories>(x => x.ToTable("OTPHistories"));


            #endregion


            base.OnModelCreating(modelBuilder);
        }

        #region Authentication

        public DbSet<Users> Users { get; set; }
        public DbSet<UserGroups> UserGroups { get; set; }
        public DbSet<UserInGroups> UserInGroups { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<PermitObjects> PermitObjects { get; set; }
        public DbSet<PermitObjectPermissions> PermitObjectPermissions { get; set; }

        #endregion

        #region Configuration

        public DbSet<EmailConfigurations> EmailConfigurations { get; set; }
        public DbSet<SMSConfigurations> SMSConfigurations { get; set; }
        public DbSet<SMSEmailTemplates> SMSEmailTemplates { get; set; }
        public DbSet<OTPHistories> OTPHistories { get; set; }

        #endregion

    }
}
