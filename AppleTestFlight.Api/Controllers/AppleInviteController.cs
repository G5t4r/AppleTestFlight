using AppleTestFlight.Api.Models;
using AppleTestFlight.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace AppleTestFlight.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("CustomCorsRules")]
    public class AppleInviteController : ControllerBase
    {
        [HttpGet]
        public async Task<TestFlightResult> GetSingleInviteUrl(string appid)
        {
            return await Task.Run(() =>
            {
                var data = RedisUtils.DeQueue(appid);

                if (data != null)
                {
                    return new TestFlightResult(1, "获取成功", data.Replace("https://", "itms-beta://"));

                }

                return new TestFlightResult(0, "暂无可用", null);
            });
        }
    }
}
