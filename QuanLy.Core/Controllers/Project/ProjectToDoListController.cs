using App.Core.Controllers;
using App.Core.Extensions;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuanLy.Entities;
using QuanLy.Interface;
using QuanLy.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QuanLy.Core.Controllers
{
    [Route("api/project-to-do")]
    [ApiController]
    [Description("Quản lý các công việc của task ")]
    [Authorize]
    public class ProjectToDoListController : BaseController<ProjectToDoList, ProjectToDoListModel, RequestProjectToDoListModel, SearchProjectToDoList>
    {
        protected IProjectToDoList ProjectToDoList;
        public ProjectToDoListController(IServiceProvider serviceProvider, ILogger<BaseController<ProjectToDoList, ProjectToDoListModel, RequestProjectToDoListModel, SearchProjectToDoList>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.ProjectToDoList = serviceProvider.GetRequiredService<IProjectToDoList>();
            this.domainService = serviceProvider.GetRequiredService<IProjectToDoList>();
        }

        /// <summary>
        /// Thêm to do list
        /// </summary>
        /// <returns></returns>
        [HttpPost("add-mutiple-to-do")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> AddListToDo([FromBody] List<RequestProjectToDoListModel> model)
        {
            if (!ModelState.IsValid) throw new AppException(ModelState.GetErrorMessage());
            bool success = false;
            AppDomainResult appDomainResult = new AppDomainResult();
            var itemUpdate = mapper.Map<List<ProjectToDoList>>(model);
            var User = LoginContext.Instance.CurrentUser.UserName;
            var Message = await this.ProjectToDoList.CheckUserExsitsInTask(itemUpdate, User);
            if (string.IsNullOrEmpty(Message))
            {
                success = true;
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                await this.domainService.CreateAsync(itemUpdate);
            }
            else
                throw new KeyNotFoundException(Message);
            appDomainResult.Success = success;
            return appDomainResult;
        }
    }
}
