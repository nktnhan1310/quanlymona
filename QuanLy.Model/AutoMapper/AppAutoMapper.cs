using App.Core.Models.DomainModel;
using App.Core.Utilities;
using AutoMapper;
using QuanLy.Entities;
using QuanLy.Entities.Auth;
using QuanLy.Entities.Catalogue;
using QuanLy.Entities.Newfeed;
using QuanLy.Model.Auth;
using QuanLy.Model.Catalogue;
using QuanLy.Model.Configuration;
using QuanLy.Model.Newfeed;
using QuanLy.Model.RequestModel.Newfeed;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLy.Model
{
    public class AppAutoMapper : Profile
    {
        public AppAutoMapper()
        {
            #region Authenticate

            CreateMap<UserModel, Users>().ReverseMap();
            CreateMap<RequestUserModel, Users>().ReverseMap();
            CreateMap<PagedList<UserModel>, PagedList<Users>>().ReverseMap();

            CreateMap<UserGroupModel, UserGroups>().ReverseMap();
            CreateMap<RequestUserGroupModel, UserGroups>().ReverseMap();
            CreateMap<PagedList<UserGroupModel>, PagedList<UserGroups>>().ReverseMap();

            CreateMap<UserInGroupModel, UserInGroups>().ReverseMap();
            CreateMap<PagedList<UserInGroupModel>, PagedList<UserInGroups>>().ReverseMap();

            #endregion

            #region Configuration

            CreateMap<DeviceAppModel, DeviceApps>().ReverseMap();
            CreateMap<PagedList<DeviceAppModel>, PagedList<DeviceApps>>().ReverseMap();

            CreateMap<DeviceBrowserModel, DeviceBrowsers>().ReverseMap();
            CreateMap<PagedList<DeviceBrowserModel>, PagedList<DeviceBrowsers>>().ReverseMap();

            CreateMap<HolidayConfigModel, HolidayConfigs>().ReverseMap();
            CreateMap<RequestHolidayConfigModel, HolidayConfigs>().ReverseMap();
            CreateMap<PagedList<HolidayConfigModel>, PagedList<HolidayConfigs>>().ReverseMap();

            #endregion

            #region Projects

            CreateMap<ProjectModel, Projects>().ReverseMap();
            CreateMap<RequestProjectModel, Projects>().ReverseMap();
            CreateMap<PagedList<ProjectModel>, PagedList<Projects>>().ReverseMap();

            CreateMap<ProjectCommentModel, ProjectComments>().ReverseMap();
            CreateMap<RequestProjectCommentModel, ProjectComments>().ReverseMap();
            CreateMap<PagedList<ProjectCommentModel>, PagedList<ProjectComments>>().ReverseMap();

            CreateMap<ProjectUserModel, ProjectUsers>().ReverseMap();
            CreateMap<RequestProjectUserModel, ProjectUsers>().ReverseMap();
            CreateMap<PagedList<ProjectUserModel>, PagedList<ProjectUsers>>().ReverseMap();

            #endregion

            #region Contact Customer

            CreateMap<ContactCustomerModel, ContactCustomers>().ReverseMap();
            CreateMap<RequestContactCustomerModel, ContactCustomers>().ReverseMap();
            CreateMap<PagedList<ContactCustomerModel>, PagedList<ContactCustomers>>().ReverseMap();

            CreateMap<ContactCustomerFileModel, ContactCustomerFiles>().ReverseMap();
            CreateMap<RequestContactCustomerFileModel, ContactCustomerFiles>().ReverseMap();
            CreateMap<PagedList<ContactCustomerFileModel>, PagedList<ContactCustomerFiles>>().ReverseMap();

            CreateMap<ContactCustomerNoteModel, ContactCustomerNotes>().ReverseMap();
            CreateMap<RequestContactCustomerNoteModel, ContactCustomerNotes>().ReverseMap();
            CreateMap<PagedList<ContactCustomerNoteModel>, PagedList<ContactCustomerNotes>>().ReverseMap();

            CreateMap<ContactCustomerMappingRequestModel, ContactCustomerMappingRequests>().ReverseMap();
            CreateMap<RequestContactCustomerMappingRequestModel, ContactCustomerMappingRequests>().ReverseMap();
            CreateMap<PagedList<ContactCustomerMappingRequestModel>, PagedList<ContactCustomerMappingRequests>>().ReverseMap();

            CreateMap<ContactCustomerServiceModel, ContactCustomerServices>().ReverseMap();
            CreateMap<RequestContactCustomerServiceModel, ContactCustomerServices>().ReverseMap();
            CreateMap<PagedList<ContactCustomerServiceModel>, PagedList<ContactCustomerServices>>().ReverseMap();

            CreateMap<ContactCustomerSaleRequestModel, ContactCustomerSaleRequests>().ReverseMap();
            CreateMap<RequestContactCustomerSaleRequestModel, ContactCustomerSaleRequests>().ReverseMap();
            CreateMap<PagedList<ContactCustomerSaleRequestModel>, PagedList<ContactCustomerSaleRequests>>().ReverseMap();



            #endregion

            #region Catalogue

            CreateMap<ServiceTypeModel, ServiceTypes>().ReverseMap();
            CreateMap<RequestCoreCatalogueModel, ServiceTypes>().ReverseMap();
            CreateMap<PagedList<ServiceTypeModel>, PagedList<ServiceTypes>>().ReverseMap();

            CreateMap<ColorTaskModel, ColorTasks>().ReverseMap();
            CreateMap<RequestColorTaskModel, ColorTasks>().ReverseMap();
            CreateMap<PagedList<ColorTaskModel>, PagedList<ColorTasks>>().ReverseMap();

            CreateMap<TaskTypeModel, TaskTypes>().ReverseMap();
            CreateMap<RequestTaskTypeModel, TaskTypes>().ReverseMap();
            CreateMap<PagedList<TaskTypeModel>, PagedList<RequestTypes>>().ReverseMap();

            CreateMap<RequestTypeModel, RequestTypes>().ReverseMap();
            CreateMap<RequestCoreCatalogueModel, RequestTypes>().ReverseMap();
            CreateMap<PagedList<RequestTypeModel>, PagedList<RequestTypes>>().ReverseMap();

            CreateMap<ProjectCustomerStatusModel, ProjectCustomerStatuses>().ReverseMap();
            CreateMap<RequestCoreCatalogueModel, ProjectCustomerStatuses>().ReverseMap();
            CreateMap<PagedList<ProjectCustomerStatusModel>, PagedList<ProjectCustomerStatuses>>().ReverseMap();

            CreateMap<ProjectStatusModel, ProjectStatuses>().ReverseMap();
            CreateMap<RequestCatalogueProjectStatusModel, ProjectStatuses>().ReverseMap();
            CreateMap<PagedList<ProjectStatusModel>, PagedList<ProjectStatuses>>().ReverseMap();

            CreateMap<SourceTypeModel, SourceTypes>().ReverseMap();
            CreateMap<RequestCoreCatalogueModel, SourceTypes>().ReverseMap();
            CreateMap<PagedList<SourceTypeModel>, PagedList<SourceTypes>>().ReverseMap();

            CreateMap<ProjectTypeModel, ProjectTypes>().ReverseMap();
            CreateMap<RequestCatalogueProjectTypeModel, ProjectTypes>().ReverseMap();
            CreateMap<PagedList<ProjectTypeModel>, PagedList<ProjectTypes>>().ReverseMap();

            CreateMap<SourceProjectTypeModel, SourceProjectTypes>().ReverseMap();
            CreateMap<RequestCoreCatalogueModel, SourceProjectTypes>().ReverseMap();
            CreateMap<PagedList<SourceProjectTypeModel>, PagedList<SourceProjectTypes>>().ReverseMap();

            CreateMap<CompanyModel, Companies>().ReverseMap();
            CreateMap<RequestCatalogueCompanyModel, Companies>().ReverseMap();
            CreateMap<PagedList<CompanyModel>, PagedList<Companies>>().ReverseMap();

            #endregion

            #region Reports

            CreateMap<ReportSourceListModel, ReportSourceList>().ReverseMap();
            CreateMap<PagedList<ReportSourceListModel>, PagedList<ReportSourceList>>().ReverseMap();


            CreateMap<ReportSourceDetailModel, ReportSourceDetails>().ReverseMap();
            CreateMap<PagedList<ReportSourceDetailModel>, PagedList<ReportSourceDetails>>().ReverseMap();

            #endregion

            #region NewFeed
            CreateMap<PostCommentModel, PostComments>().ReverseMap();
            CreateMap<RequestPostCommentModel, PostComments>().ReverseMap();
            CreateMap<PagedList<PostCommentModel>, PagedList<PostComments>>().ReverseMap();

            CreateMap<PostContentModel, PostContents>().ReverseMap();
            CreateMap<RequestPostContentModel, PostContents>().ReverseMap();
            CreateMap<PagedList<PostContentModel>, PagedList<PostContents>>().ReverseMap();
            #endregion

        }
    }
}
