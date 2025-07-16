using Application.Interfaces;
using Application.Models;
using Application.Utils;
using Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SharedUI.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseApiUrl;

        public AccountService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AccountService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _baseApiUrl = _configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException("ApiSettings:BaseUrl");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<RequestResponse> ResetPasswordUserAsync(ResetPasswordRequest resetPassword)
        {
            try
            {
                _logger.LogInformation("Attempting to reset password for user: {Email}", resetPassword.Email);

                var apiUrl = _baseApiUrl + ApiEndpoint.Account.GetVersionedEndpoint(ApiEndpoint.Account.ResetPassword);

                var response = await _httpClient.PostAsJsonAsync(apiUrl, resetPassword, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RequestResponse>(_jsonOptions);
                    _logger.LogInformation("Password reset successful for user: {Email}", resetPassword.Email);
                    return result ?? new RequestResponse { Successful = true };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Password reset failed for user: {Email}. Status: {StatusCode}, Error: {Error}",
                    resetPassword.Email, response.StatusCode, errorContent);

                return new RequestResponse
                {
                    Successful = false,
                    Error = GetErrorMessage(response.StatusCode, errorContent)
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during password reset for user: {Email}", resetPassword.Email);
                return new RequestResponse
                {
                    Successful = false,
                    Error = "خطا در اتصال به سرور. لطفاً دوباره تلاش کنید."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password reset for user: {Email}", resetPassword.Email);
                return new RequestResponse
                {
                    Successful = false,
                    Error = "خطای غیرمنتظره رخ داده است."
                };
            }
        }

        public async Task<RequestResponse> ChangePasswordUserAsync(ChangePasswordRequest changePassword)
        {
            try
            {
                _logger.LogInformation("Attempting to change password for user: {UserId}", changePassword.UserId);

                var apiUrl = _baseApiUrl + ApiEndpoint.Account.GetVersionedEndpoint(ApiEndpoint.Account.ChangePassword);

                var response = await _httpClient.PostAsJsonAsync(apiUrl, changePassword, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RequestResponse>(_jsonOptions);
                    _logger.LogInformation("Password change successful for user: {UserId}", changePassword.UserId);
                    return result ?? new RequestResponse { Successful = true };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Password change failed for user: {UserId}. Status: {StatusCode}, Error: {Error}", changePassword.UserId, response.StatusCode, errorContent);

                return new RequestResponse
                {
                    Successful = false,
                    Error = GetErrorMessage(response.StatusCode, errorContent)
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during password change for user: {UserId}", changePassword.UserId);
                return new RequestResponse
                {
                    Successful = false,
                    Error = "خطا در اتصال به سرور. لطفاً دوباره تلاش کنید."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password change for user: {UserId}", changePassword.UserId);
                return new RequestResponse
                {
                    Successful = false,
                    Error = "خطای غیرمنتظره رخ داده است."
                };
            }
        }

        private async Task<RequestResponse<JwtTokenResponse>> GenerateTokenAsync(JwtTokenRequest jwtTokenRequest)
        {
            try
            {
                //_logger.LogInformation("Generating token for account: {AccountId}, Type: {TokenType}", jwtTokenRequest.AccountId, jwtTokenRequest.TokenType);

                // فعلاً endpoint مخصوص GenerateToken وجود ندارد، از Login استفاده می‌کنیم
                var apiUrl = _baseApiUrl + ApiEndpoint.Account.GetVersionedEndpoint(ApiEndpoint.Account.Login);

                var response = await _httpClient.PostAsJsonAsync(apiUrl, jwtTokenRequest, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RequestResponse<JwtTokenResponse>>(_jsonOptions);

                    if (result != null && result.Successful)
                    {
                        _logger.LogInformation("Token generated successfully for account: {AccountId}", jwtTokenRequest.AccountId);
                        return result;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Token generation failed for account: {AccountId}. Status: {StatusCode}, Error: {Error}",
                    jwtTokenRequest.AccountId, response.StatusCode, errorContent);

                return new RequestResponse<JwtTokenResponse>
                {
                    Successful = false,
                    Error = GetErrorMessage(response.StatusCode, errorContent)
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during token generation for account: {AccountId}", jwtTokenRequest.AccountId);
                return new RequestResponse<JwtTokenResponse>
                {
                    Successful = false,
                    Error = "خطا در اتصال به سرور. لطفاً دوباره تلاش کنید."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token generation for account: {AccountId}", jwtTokenRequest.AccountId);
                return new RequestResponse<JwtTokenResponse>
                {
                    Successful = false,
                    Error = "خطای غیرمنتظره رخ داده است."
                };
            }
        }

        public async Task<RequestResponse> ValidateTokenAsync(string token)
        {
            try
            {
                _logger.LogInformation("Validating token");

                if (string.IsNullOrEmpty(token))
                {
                    return new RequestResponse
                    {
                        Successful = false,
                        Error = "توکن نامعتبر است."
                    };
                }

                var apiUrl = _baseApiUrl + ApiEndpoint.Account.GetVersionedEndpoint(ApiEndpoint.Account.ValidateToken);

                // Add token to Authorization header
                SetAuthorizationHeader(token);

                var response = await _httpClient.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RequestResponse>(_jsonOptions);
                    _logger.LogInformation("Token validation successful");
                    return result ?? new RequestResponse { Successful = true };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Token validation failed. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new RequestResponse
                {
                    Successful = false,
                    Error = GetErrorMessage(response.StatusCode, errorContent)
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during token validation");
                return new RequestResponse
                {
                    Successful = false,
                    Error = "خطا در اتصال به سرور. لطفاً دوباره تلاش کنید."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token validation");
                return new RequestResponse
                {
                    Successful = false,
                    Error = "خطای غیرمنتظره رخ داده است."
                };
            }
            finally
            {
                // Clear authorization header
                ClearAuthorizationHeader();
            }
        }

        public async Task<RequestResponse<JwtTokenResponse>> LoginAsync(LoginRequest login)
        {
            try
            {
                _logger.LogInformation("Attempting login for account: {AccountId}", login.AccountId);

                var apiUrl = _baseApiUrl + ApiEndpoint.Account.GetVersionedEndpoint(ApiEndpoint.Account.Login);

                var response = await _httpClient.PostAsJsonAsync(apiUrl, login, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RequestResponse<JwtTokenResponse>>(_jsonOptions);

                    if (result != null && result.Successful)
                    {
                        _logger.LogInformation("Login successful for account: {AccountId}", login.AccountId);
                        return result;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Login failed for account: {AccountId}. Status: {StatusCode}, Error: {Error}",
                    login.AccountId, response.StatusCode, errorContent);

                return new RequestResponse<JwtTokenResponse>
                {
                    Successful = false,
                    Error = GetErrorMessage(response.StatusCode, errorContent)
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during login for account: {AccountId}", login.AccountId);
                return new RequestResponse<JwtTokenResponse>
                {
                    Successful = false,
                    Error = "خطا در اتصال به سرور. لطفاً دوباره تلاش کنید."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for account: {AccountId}", login.AccountId);
                return new RequestResponse<JwtTokenResponse>
                {
                    Successful = false,
                    Error = "خطای غیرمنتظره رخ داده است."
                };
            }
        }
        public Task<RequestResponse<JwtTokenResponse>> LoginAdminAsync(string accountId)
        {
            throw new NotImplementedException();
        }
        public async Task<RequestResponse<JwtTokenResponse>> RegisterAsync(RegisterRequest register)
        {
            try
            {
                _logger.LogInformation("Attempting registration for account: {AccountId}", register.AccountId);

                // Register endpoint در ApiEndpoint موجود نیست، پس از Base استفاده می‌کنیم
                var apiUrl = _baseApiUrl + ApiEndpoint.Account.GetVersionedEndpoint(ApiEndpoint.Account.Base + "/register");

                var response = await _httpClient.PostAsJsonAsync(apiUrl, register, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RequestResponse<JwtTokenResponse>>(_jsonOptions);

                    if (result != null && result.Successful)
                    {
                        _logger.LogInformation("Registration successful for account: {AccountId}", register.AccountId);
                        return result;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Registration failed for account: {AccountId}. Status: {StatusCode}, Error: {Error}",
                    register.AccountId, response.StatusCode, errorContent);

                return new RequestResponse<JwtTokenResponse>
                {
                    Successful = false,
                    Error = GetErrorMessage(response.StatusCode, errorContent)
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during registration for account: {AccountId}", register.AccountId);
                return new RequestResponse<JwtTokenResponse>
                {
                    Successful = false,
                    Error = "خطا در اتصال به سرور. لطفاً دوباره تلاش کنید."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for account: {AccountId}", register.AccountId);
                return new RequestResponse<JwtTokenResponse>
                {
                    Successful = false,
                    Error = "خطای غیرمنتظره رخ داده است."
                };
            }
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            try
            {
                _logger.LogInformation("Checking password for user: {UserId}", user.Id);

                var apiUrl = _baseApiUrl + ApiEndpoint.Account.GetVersionedEndpoint(ApiEndpoint.Account.Base + "/check-password");

                var request = new
                {
                    UserId = user.Id,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync(apiUrl, request, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<bool>(_jsonOptions);
                    _logger.LogInformation("Password check completed for user: {UserId}, Result: {Result}", user.Id, result);
                    return result;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Password check failed for user: {UserId}. Status: {StatusCode}, Error: {Error}",
                    user.Id, response.StatusCode, errorContent);

                return false;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during password check for user: {UserId}", user.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password check for user: {UserId}", user.Id);
                return false;
            }
        }

        #region SMS Service Methods

        public async Task<bool> SendPassKeyAsync(string mobileNumber, string passKey)
        {
            try
            {
                //_logger.LogInformation("Sending SMS code to: {MobileNumber}", mobileNumber);

                var apiUrl = _baseApiUrl + ApiEndpoint.Account.GetVersionedEndpoint(ApiEndpoint.Account.SendSmsPassKey);

                var smsSendRequest = new SmsSendRequest()
                {
                    MobileNumber = mobileNumber,
                    SmsContent = $"کد ورود شما به برنامه : {passKey}",
                    SendPriority = Domain.Enums.Priority.RealTime
                };

                var response = await _httpClient.PostAsJsonAsync(apiUrl, smsSendRequest, _jsonOptions);

                if (response.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS code to: {MobileNumber}", mobileNumber);
                return false;
            }
        }

        private async Task<bool> SendSmsAsync(string mobileNumber, string message)
        {
            try
            {
                // Integration with SMS service (e.g., Kavenegar, Ghasedak, etc.)
                var smsApiUrl = _configuration["SmsSettings:ApiUrl"];
                var smsApiKey = _configuration["SmsSettings:ApiKey"];

                if (string.IsNullOrEmpty(smsApiUrl) || string.IsNullOrEmpty(smsApiKey))
                {
                    _logger.LogWarning("SMS service not configured properly");
                    return false;
                }

                var smsRequest = new
                {
                    receptor = mobileNumber,
                    message = message,
                    token = smsApiKey
                };

                var response = await _httpClient.PostAsJsonAsync(smsApiUrl, smsRequest);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("SMS sent successfully to: {MobileNumber}", mobileNumber);
                    return true;
                }

                _logger.LogWarning("SMS sending failed for: {MobileNumber}, Status: {StatusCode}",
                    mobileNumber, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SMS service for: {MobileNumber}", mobileNumber);
                return false;
            }
        }

        //private string GenerateRandomSmsCode()
        //{
        //    var random = new Random();
        //    return random.Next(100000, 999999).ToString();
        //}

        #endregion

        #region Helper Methods

        private string GetErrorMessage(System.Net.HttpStatusCode statusCode, string errorContent)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<RequestResponse>(errorContent, _jsonOptions);
                if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Error))
                {
                    return errorResponse.Error;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse error response: {ErrorContent}", errorContent);
            }

            return statusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => "نام کاربری یا رمز عبور اشتباه است.",
                System.Net.HttpStatusCode.Forbidden => "دسترسی مجاز نیست.",
                System.Net.HttpStatusCode.NotFound => "کاربر یافت نشد.",
                System.Net.HttpStatusCode.BadRequest => "اطلاعات وارد شده نامعتبر است.",
                System.Net.HttpStatusCode.InternalServerError => "خطای داخلی سرور رخ داده است.",
                System.Net.HttpStatusCode.ServiceUnavailable => "سرویس در حال حاضر در دسترس نیست.",
                System.Net.HttpStatusCode.RequestTimeout => "زمان درخواست به پایان رسید.",
                _ => "خطای غیرمنتظره رخ داده است."
            };
        }

        private void SetAuthorizationHeader(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private void ClearAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        #endregion
    }
}