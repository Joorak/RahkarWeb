

using Microsoft.Win32;
using Serilog;

namespace WebApi.Controllers
{

    [ApiController]
    public class AccountsController : ControllerBase
    {

        public AccountsController(
            IConfiguration configuration,
            IEmailService emailService,
            IAccountService accountService)
            : base()
        {
            this.Configuration = configuration;
            this.EmailService = emailService;
            this.AccountService = accountService;
        }


        private IConfiguration Configuration { get; }


        private IEmailService EmailService { get; }
        private IAccountService AccountService { get; }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromForm] LoginRequest login)
        {
            var result = await this.AccountService.LoginAsync(login);

            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterRequest register)
        {
            var result = await this.AccountService.RegisterAsync(register).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePasswordUser([FromBody] ChangePasswordRequest changePassword)
        {
            var result = await this.AccountService.ChangePasswordUserAsync(changePassword).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }


        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordUser([FromForm] ResetPasswordRequest resetPassword)
        {
            var emailSettings = new EmailSettings
            {
                Host = this.Configuration["BusinessEmail:Host"],
                Port = int.Parse(this.Configuration["BusinessEmail:Port"]!),
                Subject = this.Configuration["BusinessEmail:Subject"],
                Message = this.Configuration["BusinessEmail:Message"],
                Username = this.Configuration["BusinessEmail:Username"],
                Password = this.Configuration["BusinessEmail:Password"],
            };

            await EmailService.SendEmail(resetPassword.Email, emailSettings).ConfigureAwait(false);
            var result = await this.AccountService.ResetPasswordUserAsync(resetPassword).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }
    }
}
