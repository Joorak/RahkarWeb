using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace WebAPI.Filters
{

    public class LogActionFilter : IActionFilter
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null || context.HttpContext == null) return;
            if (context.HttpContext.Request.Method.ToLower() == "post" && context.HttpContext.Request.Path.Value!.ToLower().StartsWith("/NOT_LOG_PATH")) return;
            context.HttpContext.Request.HttpContext.Items.Add("StartTime", DateTime.Now);
            context.HttpContext.Request.HttpContext.Items.Add("ActionArguments", context.ActionArguments);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context == null || context.HttpContext == null) return;
            if (context.HttpContext.Request.Method.ToLower() == "post" && context.HttpContext.Request.Path.Value!.ToLower().StartsWith("/NOT_LOG_PATH")) return;
            var StartTime = (DateTime)context.HttpContext.Request.HttpContext.Items.Where(i => i.Key.ToString() == "StartTime").FirstOrDefault().Value!;
            var ActionArguments = context.HttpContext.Request.HttpContext.Items.Where(i => i.Key.ToString() == "ActionArguments").FirstOrDefault().Value!;
            _logger.LogCritical("{Date}\t{Time}\t{UserId}\t{Method}\t{Path}\t{@logparam}\t{ElapsedTime}", PersianCalendarService.Now.ToString("yyyy/MM/dd"), StartTime.ToString("HH:mm:ss.fff"), context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), context.HttpContext.Request.Method, context.HttpContext.Request.Path.Value, ActionArguments, DateTime.Now.Subtract(StartTime).ToString(@"dd\.hh\:mm\:ss\.fff"));
        }
    }
}
