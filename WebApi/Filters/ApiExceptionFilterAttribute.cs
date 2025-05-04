

namespace WebApi.Filters
{

    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {

        public ApiExceptionFilterAttribute()
        {
            this.ExceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), this.HandleValidationException },
            };
        }


        private IDictionary<Type, Action<ExceptionContext>> ExceptionHandlers { get; }


        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            this.HandleException(context);
        }


        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (this.ExceptionHandlers.ContainsKey(type))
            {
                this.ExceptionHandlers[type].Invoke(context);
                return;
            }

            if (!context.ModelState.IsValid)
            {
                this.HandleInvalidModelStateException(context);
                return;
            }

            this.HandleUnknownException(context);
        }


        private void HandleValidationException(ExceptionContext context)
        {
            var exception = (ValidationException)context.Exception;
            var response = new RequestResponse
            {
                Successful = false,
                Error = $"Fluent Handlers Validations. {exception.Message}. {exception.InnerException?.Message}",
            };
            foreach (var item in exception.Errors)
            {
                foreach (var value in item.Value)
                {
                    response.Error += $"{value}. ";
                }
            }

            context.Result = new BadRequestObjectResult(response);
            context.ExceptionHandled = true;
        }


        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            var response = new RequestResponse
            {
                Successful = false,
                Error = $"Invalid Model State. {context.Exception.Message}. {context.Exception?.InnerException?.Message}",
            };

            context.Result = new BadRequestObjectResult(response);
            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var response = new RequestResponse
            {
                Successful = false,
                Error = $"An error occurred while processing your request. {context.Exception.Message}. {context.Exception?.InnerException?.Message}",
            };

            context.Result = new BadRequestObjectResult(response);
            context.ExceptionHandled = true;
        }
    }
}
