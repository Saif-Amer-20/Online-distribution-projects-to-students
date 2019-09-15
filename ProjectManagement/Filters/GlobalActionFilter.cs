using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Filters
{
    public class GlobalActionFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public GlobalActionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("");
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("start executing");
            _logger.LogWarning(context.ModelState.IsValid + "OnActionExecuting");
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                _logger.LogError(context.Result.ToString());
            }
            base.OnActionExecuted(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
        }
    }
}
