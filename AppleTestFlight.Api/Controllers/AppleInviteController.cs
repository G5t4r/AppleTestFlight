using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppleTestFlight.Core;
using AppleTestFlight.Api.Models;
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
        public async Task<TestFlightResult> GetSingleInviteUrl()
        {
            return await Task.Run(() =>
            {
                var data = _taskManager.GetSingleInviteUrl();

                if (data != null)
                {
                    return new TestFlightResult(1, "获取成功", data.Replace("https://", "itms-beta://"));

                }

                return new TestFlightResult(0, "暂无可用", null);
            });
        }
    }
}
