using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using AppleTestFlight.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppleTestFlight.Api.Filters
{
    public class LimitFilter : IActionFilter
    {
        private readonly string _limitAccessorTime;

        public LimitFilter(string limitAccessorTime)
        {
            _limitAccessorTime = limitAccessorTime;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string ip = new HttpContextAccessor().HttpContext.Connection.RemoteIpAddress.ToString();

            //如果该IP存在
            if (RedisUtils.KeyExits(ip))
            {
                context.Result = new JsonResult(new
                {
                    code = 0,
                    message = "重复请求过多，请" + _limitAccessorTime + "秒后尝试",
                    data = ""
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            string ip = new HttpContextAccessor().HttpContext.Connection.RemoteIpAddress.ToString();
            RedisUtils.InsertString(ip, "限制IP", double.Parse(_limitAccessorTime));
        }
    }
}
