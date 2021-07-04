using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppleTestFlight.Core;
namespace AppleTestFlight.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppleInviteController : ControllerBase
    {
        private readonly TaskManager _taskManager;
        public AppleInviteController()
        {
            _taskManager = new TaskManager();
        }

        [HttpGet]
        public async Task<object> GetSingleInviteUrl()
        {
            return await Task.Run(() =>
             {

                 var data = _taskManager.GetSingleInviteUrl();

                 if (data != null)
                 {
                     return new
                     {
                         code = 1,
                         message = "获取成功",
                         data = data.Replace("https://", "itms-beta://")
                     };
                 }
                 return new
                 {
                     code = 0,
                     message = "暂无库存",
                     data = ""
                 };
             });
        }
    }
}
