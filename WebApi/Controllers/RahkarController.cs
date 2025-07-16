    
namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RahkarController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;
        public RahkarController(IConfiguration configuration, IAccountService accountService)
        {
            this._configuration = configuration;
            this._accountService = accountService;
        }

        [HttpGet]
        public ContentResult Welcome()
        {

            //var html = System.IO.File.ReadAllText(@"./welcome.html");
            var html = $@"<!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='utf-8' />
                            <title>Welcome</title>
                        </head>
                        <body>
                            <h1>Welcome to RahkarApi</h1>
                            <p>Database Host : <strong>{_configuration["ConnectionStrings:Default"]!.Split('=')[1]}</strong></p>
                            
                        </body>
                        </html>";
            //html = html.Replace("{{name}}", name ?? "");

            return base.Content(html, "text/html");
        }

        [HttpGet("{accountId}/{password}")]
        public async Task<ContentResult> GetAdminToken([FromQuery] string accountId, string password)
        {
            string? token = string.Empty;
            //var loginRequest = new LoginRequest() { AccountId = accountId, PassKey = password };
            if (accountId != _configuration["AdminSeedModel:AccountId"]!.ToString() || password != _configuration["AdminSeedModel:Password"]!.ToString())
                token = null;
            else
            {
                var response = await _accountService.LoginAdminAsync(accountId);
                if (response.Successful)
                    token = response.Item!.AccessToken!;
            }

            var html = $@"<!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='utf-8' />
                            <title>Welcome</title>
                        </head>
                        <body>
                            <h1>Welcome to Rahkar</h1>
                            <p>Your Token : <strong>{token ?? "user not found!!!"}</strong></p>
                        </body>
                        </html>";
            //html = html.Replace("{{name}}", name ?? "");

            return base.Content(html, "text/html");
        }

        [Authorize]
        [HttpGet("GetConnectionString")]
        public async Task<IActionResult> GetConnectionString()
        {
            RequestResult<string> response = new RequestResult<string>();
            try
            {
                response.Item = await Task.Run(() => _configuration["ConnectionStrings:Default"]!.ToString());
                response.Successful = true;
            }
            catch (Exception ex)
            {
                response.Item = null;
                response.Successful = false;
                response.Error = ex.Message;
            }
            if (response.Successful)
                return Ok(response);
            else
                return BadRequest(response);
        }

    }
}