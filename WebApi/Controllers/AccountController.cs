using Application.Models;
using Application.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    //[Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        //private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserService _userService;
        //private readonly ISmsService _smsService;
        private readonly ILogger<AccountController> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IExternalApiService _externalApi;

        public AccountController(
            IAccountService accountService,
            //IJwtTokenService jwtTokenService,
            IUserService userService,
            //ISmsService smsService,
            ILogger<AccountController> logger,
            IPasswordHasher<User> passwordHasher,
            IExternalApiService externalApi)
        {
            _accountService = accountService;
            //_jwtTokenService = jwtTokenService;
            _userService = userService;
            //_smsService = smsService;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _externalApi = externalApi;
        }

        /// <summary>
        /// ورود کاربر به سیستم
        /// </summary>
        /// <param name="loginRequest">اطلاعات ورود</param>
        /// <returns>توکن JWT در صورت موفقیت</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(RequestResponse<JwtTokenResponse>), 200)]
        [ProducesResponseType(typeof(RequestResponse), 400)]
        [ProducesResponseType(typeof(RequestResponse), 401)]
        public async Task<IActionResult> Login([FromBody] Application.Models.LoginRequest loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new RequestResponse
                    {
                        Successful = false,
                        Error = "اطلاعات وارد شده نامعتبر است."
                    });
                }

                _logger.LogInformation("Login attempt for account: {AccountId}", loginRequest.AccountId);

                var result = await _accountService.LoginAsync(loginRequest);

                if (result.Successful && result.Item != null)
                {
                    _logger.LogInformation("Login successful for account: {AccountId}", loginRequest.AccountId);
                    return Ok(result);
                }

                _logger.LogWarning("Login failed for account: {AccountId}", loginRequest.AccountId);
                return Unauthorized(new RequestResponse
                {
                    Successful = false,
                    Error = "نام کاربری یا رمز عبور اشتباه است."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for account: {AccountId}", loginRequest.AccountId);
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }

        /// <summary>
        /// ثبت نام کاربر جدید
        /// </summary>
        /// <param name="registerRequest">اطلاعات ثبت نام</param>
        /// <returns>توکن JWT در صورت موفقیت</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RequestResponse<JwtTokenResponse>), 200)]
        [ProducesResponseType(typeof(RequestResponse), 400)]
        [ProducesResponseType(typeof(RequestResponse), 409)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new RequestResponse
                    {
                        Successful = false,
                        Error = "اطلاعات وارد شده نامعتبر است."
                    });
                }

                _logger.LogInformation("Registration attempt for account: {AccountId}", registerRequest.AccountId);

                var result = await _accountService.RegisterAsync(registerRequest);

                if (result.Successful && result.Item != null)
                {
                    _logger.LogInformation("Registration successful for account: {AccountId}", registerRequest.AccountId);
                    return Ok(result);
                }

                if (result.Error?.Contains("موجود") == true)
                {
                    return Conflict(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for account: {AccountId}", registerRequest.AccountId);
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }

        /// <summary>
        /// اعتبارسنجی توکن JWT
        /// </summary>
        /// <returns>نتیجه اعتبارسنجی</returns>
        [HttpPost("validate-token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(RequestResponse), 200)]
        [ProducesResponseType(typeof(RequestResponse), 401)]
        public async Task<IActionResult> ValidateToken()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token)) return Unauthorized(RequestResponse.Failure("توکن یافت نشد"));

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = await Task.Run(() => handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken);
                //jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role) == StringRoleResources.
                if (jwtToken == null || jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return Unauthorized(RequestResponse.Failure("توکن نامعتبر یا منقضی شده"));
                }

                return Ok(RequestResponse.Success());
            }
            catch (Exception)
            {
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }

        /// <summary>
        /// تولید توکن JWT
        /// </summary>
        /// <param name="tokenRequest">درخواست تولید توکن</param>
        /// <returns>توکن JWT</returns>
        [HttpPost("generate-token")]
        [ProducesResponseType(typeof(RequestResponse<JwtTokenResponse>), 200)]
        [ProducesResponseType(typeof(RequestResponse), 400)]
        public async Task<IActionResult> GenerateToken([FromBody] JwtTokenRequest tokenRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new RequestResponse
                    {
                        Successful = false,
                        Error = "اطلاعات وارد شده نامعتبر است."
                    });
                }

                _logger.LogInformation("Token generation request for account: {AccountId}, Type: {TokenType}", 
                    tokenRequest.AccountId, tokenRequest.TokenType);

                await Task.Delay(1000);
                return Ok();

                //var result = await _accountService.GenerateToken(tokenRequest);

                //if (result.Successful && result.Item != null)
                //{
                //    _logger.LogInformation("Token generated successfully for account: {AccountId}", tokenRequest.AccountId);
                //    return Ok(result);
                //}

                //return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token generation for account: {AccountId}", tokenRequest.AccountId);
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }

        /// <summary>
        /// تغییر رمز عبور
        /// </summary>
        /// <param name="changePasswordRequest">درخواست تغییر رمز عبور</param>
        /// <returns>نتیجه تغییر رمز عبور</returns>
        [HttpPost("change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(RequestResponse), 200)]
        [ProducesResponseType(typeof(RequestResponse), 400)]
        [ProducesResponseType(typeof(RequestResponse), 401)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new RequestResponse
                    {
                        Successful = false,
                        Error = "اطلاعات وارد شده نامعتبر است."
                    });
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new RequestResponse
                    {
                        Successful = false,
                        Error = "کاربر احراز هویت نشده است."
                    });
                }

                // بررسی اینکه آیا کاربر می‌تواند رمز عبور این کاربر را تغییر دهد
                if (currentUserId != changePasswordRequest.UserId.ToString())
                {
                    var isAdmin = User.IsInRole("Admin");
                    if (!isAdmin)
                    {
                        return Forbid();
                    }
                }

                _logger.LogInformation("Password change request for user: {UserId}", changePasswordRequest.UserId);

                var result = await _accountService.ChangePasswordUserAsync(changePasswordRequest);

                if (result.Successful)
                {
                    _logger.LogInformation("Password changed successfully for user: {UserId}", changePasswordRequest.UserId);
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change for user: {UserId}", changePasswordRequest.UserId);
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }

        /// <summary>
        /// بازیابی رمز عبور
        /// </summary>
        /// <param name="resetPasswordRequest">درخواست بازیابی رمز عبور</param>
        /// <returns>نتیجه بازیابی رمز عبور</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(RequestResponse), 200)]
        [ProducesResponseType(typeof(RequestResponse), 400)]
        [ProducesResponseType(typeof(RequestResponse), 404)]
        public async Task<IActionResult> ResetPassword([FromBody] Application.Models.ResetPasswordRequest resetPasswordRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new RequestResponse
                    {
                        Successful = false,
                        Error = "اطلاعات وارد شده نامعتبر است."
                    });
                }

                _logger.LogInformation("Password reset request for email: {Email}", resetPasswordRequest.Email);

                var result = await _accountService.ResetPasswordUserAsync(resetPasswordRequest);

                if (result.Successful)
                {
                    _logger.LogInformation("Password reset successful for email: {Email}", resetPasswordRequest.Email);
                    return Ok(result);
                }

                if (result.Error?.Contains("یافت نشد") == true)
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for email: {Email}", resetPasswordRequest.Email);
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }

        /// <summary>
        /// بررسی صحت رمز عبور
        /// </summary>
        /// <param name="checkPasswordRequest">درخواست بررسی رمز عبور</param>
        /// <returns>نتیجه بررسی رمز عبور</returns>
        [HttpPost("check-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(RequestResponse), 400)]
        [ProducesResponseType(typeof(RequestResponse), 401)]
        public async Task<IActionResult> CheckPassword([FromBody] CheckPasswordRequest checkPasswordRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new RequestResponse
                    {
                        Successful = false,
                        Error = "اطلاعات وارد شده نامعتبر است."
                    });
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new RequestResponse
                    {
                        Successful = false,
                        Error = "کاربر احراز هویت نشده است."
                    });
                }

                // بررسی اینکه آیا کاربر می‌تواند رمز عبور این کاربر را بررسی کند
                if (currentUserId != checkPasswordRequest.UserId.ToString())
                {
                    var isAdmin = User.IsInRole("Admin");
                    if (!isAdmin)
                    {
                        return Forbid();
                    }
                }

                _logger.LogInformation("Password check request for user: {UserId}", checkPasswordRequest.UserId);

                var user = _userService.GetUserById(checkPasswordRequest.UserId);
                if (user == null)
                {
                    return NotFound(new RequestResponse
                    {
                        Successful = false,
                        Error = "کاربر یافت نشد."
                    });
                }

                var result = await _accountService.CheckPasswordAsync(user, checkPasswordRequest.Password);
                
                _logger.LogInformation("Password check completed for user: {UserId}, Result: {Result}", 
                    checkPasswordRequest.UserId, result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password check for user: {UserId}", checkPasswordRequest.UserId);
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }

        /// <summary>
        /// ارسال کد تایید پیامکی
        /// </summary>
        /// <param name="sendSmsRequest">درخواست ارسال پیامک</param>
        /// <returns>نتیجه ارسال پیامک</returns>
        [HttpPost("send-sms-passkey")]
        [ProducesResponseType(typeof(RequestResponse<string>), 200)]
        [ProducesResponseType(typeof(RequestResponse), 400)]
        public async Task<IActionResult> SendSmsCode([FromBody] SmsSendRequest smsSendRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new RequestResponse
                    {
                        Successful = false,
                        Error = "اطلاعات وارد شده نامعتبر است."
                    });
                }

                //_logger.LogInformation("SMS code request for mobile: {MobileNumber}", sendSmsRequest.MobileNumber);
                var response = await _externalApi.PostAsync<SmsSendRequest, NullResponse>("HTTPS://OUR_SMS_PROVIDER.COM", smsSendRequest);
                //var result = await _smsService.SendVerificationCodeAsync(mobileNumber);
                if(response.Successful)
                    return Ok(response);
                else
                    return BadRequest(response);
            }
            catch (Exception)
            {
                //_logger.LogError(ex, "Error sending SMS code to: {MobileNumber}", mobileNumber);
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = $"خطا در ارسال پیامک به شماره موبایل : {smsSendRequest.MobileNumber}"
                });
            }
        }

        /// <summary>
        /// تایید کد پیامکی
        /// </summary>
        /// <param name="verifySmsRequest">درخواست تایید کد پیامکی</param>
        /// <returns>نتیجه تایید کد</returns>
        [HttpPost("verify-sms-code")]
        [ProducesResponseType(typeof(RequestResponse<JwtTokenResponse>), 200)]
        [ProducesResponseType(typeof(RequestResponse), 400)]
        public async Task<IActionResult> VerifySmsCode([FromBody] VerifySmsCodeRequest verifySmsRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new RequestResponse
                    {
                        Successful = false,
                        Error = "اطلاعات وارد شده نامعتبر است."
                    });
                }

                _logger.LogInformation("SMS code verification for mobile: {MobileNumber}", verifySmsRequest.MobileNumber);

                await Task.Delay(1000);
                return Ok();

                //var result = await _smsService.VerifyCodeAsync(verifySmsRequest.MobileNumber, verifySmsRequest.Code);

                //if (result.Successful)
                //{
                //    _logger.LogInformation("SMS code verified successfully for mobile: {MobileNumber}", verifySmsRequest.MobileNumber);

                //    // تولید توکن JWT برای کاربر
                //    var tokenRequest = new JwtTokenRequest
                //    {
                //        AccountId = verifySmsRequest.MobileNumber,
                //        Role = "Customer", // یا بر اساس نوع کاربر
                //        TokenType = "SMS"
                //    };

                //    var tokenResult = await _accountService.GenerateTokenAsync(tokenRequest);
                //    return Ok(tokenResult);
                //}

                //return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying SMS code for mobile: {MobileNumber}", verifySmsRequest.MobileNumber);
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }

        /// <summary>
        /// بازیابی توکن (Refresh Token)
        /// </summary>
        /// <param name="refreshTokenRequest">درخواست بازیابی توکن</param>
        /// <returns>توکن جدید</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(RequestResponse<JwtTokenResponse>), 200)]
        [ProducesResponseType(typeof(RequestResponse), 400)]
        [ProducesResponseType(typeof(RequestResponse), 401)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new RequestResponse
                    {
                        Successful = false,
                        Error = "اطلاعات وارد شده نامعتبر است."
                    });
                }

                _logger.LogInformation("Refresh token request");
                await Task.Delay(1000);
                return Ok();

                //var result = await _jwtTokenService.RefreshTokenAsync(refreshTokenRequest.RefreshToken);

                //if (result.Successful && result.Item != null)
                //{
                //    _logger.LogInformation("Token refreshed successfully");
                //    return Ok(result);
                //}

                //return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }

        /// <summary>
        /// خروج از سیستم
        /// </summary>
        /// <returns>نتیجه خروج</returns>
        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(RequestResponse), 200)]
        [ProducesResponseType(typeof(RequestResponse), 401)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                //var tokenId = User.FindFirst("jti")?.Value;
                //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //if (string.IsNullOrEmpty(tokenId) || string.IsNullOrEmpty(userId))
                //{
                //    return Unauthorized(new RequestResponse
                //    {
                //        Successful = false,
                //        Error = "کاربر احراز هویت نشده است."
                //    });
                //}

                //_logger.LogInformation("Logout request for user: {UserId}", userId);

                //await _jwtTokenService.RevokeTokenAsync(tokenId);

                //_logger.LogInformation("Logout successful for user: {UserId}", userId);
                await Task.Delay(1000);
                return Ok(RequestResponse.Success());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new RequestResponse
                {
                    Successful = false,
                    Error = "خطای داخلی سرور رخ داده است."
                });
            }
        }
    }

    // Request Models
    public class SendSmsCodeRequest
    {
        [Required(ErrorMessage = "شماره موبایل الزامی است.")]
        [Phone(ErrorMessage = "شماره موبایل نامعتبر است.")]
        public string MobileNumber { get; set; } = string.Empty;
    }

    public class VerifySmsCodeRequest
    {
        [Required(ErrorMessage = "شماره موبایل الزامی است.")]
        [Phone(ErrorMessage = "شماره موبایل نامعتبر است.")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "کد تایید الزامی است.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "کد تایید باید 6 رقم باشد.")]
        public string Code { get; set; } = string.Empty;
    }

    public class CheckPasswordRequest
    {
        [Required(ErrorMessage = "شناسه کاربر الزامی است.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh Token الزامی است.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}