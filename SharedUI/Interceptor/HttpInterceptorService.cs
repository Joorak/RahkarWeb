


namespace SharedUI.Interceptor
{

    public class HttpInterceptorService
    {

        public HttpInterceptorService(HttpClientInterceptor interceptor, NavigationManager navManager)
        {
            this.Interceptor = interceptor;
            this.NavManager = navManager;
        }

        private HttpClientInterceptor Interceptor { get; }

        private NavigationManager NavManager { get; }

        #pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        public void RegisterEvent() => this.Interceptor.AfterSend += this.InterceptResponse;
        #pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

        #pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        public void DisposeEvent() => this.Interceptor.AfterSend -= this.InterceptResponse;
        #pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

        protected void InterceptResponse(object sender, HttpClientInterceptorEventArgs e)
        {
            if (!e.Response.IsSuccessStatusCode)
            {
                var statusCode = e.Response.StatusCode;

                string message;
                switch (statusCode)
                {
                    case HttpStatusCode.NotFound:
                        this.NavManager.NavigateTo("/not-found");
                        message = "The requested resource was not found.";
                        break;
                    case HttpStatusCode.Unauthorized:
                        this.NavManager.NavigateTo("/unauthorized");
                        message = "User is not authorized";
                        break;
                    case HttpStatusCode.Forbidden:
                        this.NavManager.NavigateTo("/unauthorized");
                        message = "User is not authorized";
                        break;
                    default:
                        this.NavManager.NavigateTo("/server-error");
                        message = "Something went wrong, please contact Administrator";
                        break;
                }

                throw new HttpResponseException(message);
            }
        }
    }
}
