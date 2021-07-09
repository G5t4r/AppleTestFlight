using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppleTestFlight.Core;
using AppleTestFlight.Api.Models;
using Microsoft.AspNetCore.Cors;

namespace AppleTestFlight.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("CustomCorsRules")]
    public class AppleInviteController : ControllerBase
    {
        private readonly TestFlightTaskManager _taskManager;
        public AppleInviteController()
        {
            _taskManager = new TestFlightTaskManager("1575052758", "0f8002ef-b934-4ac3-862f-45db06e8d938");
        }

        [HttpGet]
        public async Task<TestFlightResult> GetSingleInviteUrl()
        {
            return await Task.Run(() =>
            {
                var data = _taskManager.GetSingleInviteUrlInRedis();

                if (data != null)
                {
                    return new TestFlightResult(1, "获取成功", data.Replace("https://", "itms-beta://"));

                }

                return new TestFlightResult(0, "暂无可用", null);
            });
        }
    }
}
