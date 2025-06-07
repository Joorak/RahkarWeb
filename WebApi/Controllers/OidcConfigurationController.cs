//using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace WebApi.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    public class OidcConfigurationController : Controller
    {

        public OidcConfigurationController(
            //IClientRequestParametersProvider clientRequestParametersProvider,
            ILogger<OidcConfigurationController> logger)
        {
            //this.ClientRequestParametersProvider = clientRequestParametersProvider;
            this.Logger = logger;
        }

        //public IClientRequestParametersProvider ClientRequestParametersProvider { get; }


        private ILogger<OidcConfigurationController> Logger { get; }

        //[HttpGet("_configuration/{clientId}")]
        //public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        //{
        //    var parameters = this.ClientRequestParametersProvider.GetClientParameters(this.HttpContext, clientId);
        //    return this.Ok(parameters);
        //}
    }
}
