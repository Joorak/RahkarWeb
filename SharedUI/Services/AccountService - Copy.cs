using Application.Interfaces;
using Application.Models;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharedUI.Extensions;
using System.Text;
using System.Threading.Tasks;

namespace SharedUI.Services
{
    public class AccountService_ : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly IAccessTokenService _tokenService;
        private readonly NavigationManager _navigationManager;


        // In-memory storage for demo purposes (replace with Redis in production)
        private static readonly Dictionary<string, SmsSession> _smsSessions = new();
        private static readonly Dictionary<string, QRSession> _qrSessions = new();

        public AccountService_(HttpClient httpClient, IAccessTokenService tokenService, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _navigationManager = navigationManager;
        }

        public async Task<RequestResponse> ChangePasswordUserAsync(ChangePasswordRequest command)
        {
            var token = await _tokenService.GetToken();
           
            return await _httpClient.PostAsync<RequestResponse>(ApiEndpoint.Account.ChangePassword, command, token);
        }

        public async Task<RequestResponse> ResetPasswordUserAsync(ResetPasswordRequest command)
        {
            var token = await _tokenService.GetToken();
            return await _httpClient.PostAsync<RequestResponse>(ApiEndpoint.Account.ResetPassword, command, token);
        }

        public async Task<RequestResponse<JwtTokenResponse>> LoginAsync(LoginRequest login)
        {
            return await _httpClient.PostAsync<RequestResponse<JwtTokenResponse>>(ApiEndpoint.Account.Login, login, null);
        }

        public async Task<RequestResponse> ValidateTokenAsync(string token)
        {
            return await _httpClient.GetAsync<RequestResponse>(ApiEndpoint.Account.ValidateToken, token);
        }

        public Task<RequestResponse<JwtTokenResponse>> GenerateTokenAsync(JwtTokenRequest jwtTokenRequest)
        {
            throw new NotImplementedException();
        }

        public Task<RequestResponse<JwtTokenResponse>> RegisterAsync(RegisterRequest register)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            throw new NotImplementedException();
        }





        public async Task<LoginResult> LoginAsync(LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "نام کاربری یا کلمه عبور اشتباه است"
                    };
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "نام کاربری یا کلمه عبور اشتباه است"
                    };
                }

                var token = await GenerateJwtTokenAsync(user);
                var userInfo = await GetUserInfoAsync(user);

                return new LoginResult
                {
                    Success = true,
                    Message = "ورود موفقیت آمیز بود",
                    Token = token,
                    UserInfo = userInfo
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "خطا در عملیات ورود"
                };
            }
        }

        public async Task<SmsResult> SendSmsCodeAsync(string phoneNumber, string role)
        {
            try
            {
                // For demo purposes, we'll create a user if it doesn't exist
                var user = await _userManager.FindByNameAsync(phoneNumber);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = phoneNumber,
                        PhoneNumber = phoneNumber,
                        PhoneNumberConfirmed = true,
                        Email = $"{phoneNumber}@example.com",
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(user, "123456"); // Default password
                    if (!createResult.Succeeded)
                    {
                        return new SmsResult
                        {
                            Success = false,
                            Message = "خطا در ایجاد کاربر"
                        };
                    }

                    // Add role
                    await _userManager.AddToRoleAsync(user, role);
                }

                // Generate SMS code
                var smsCode = GenerateSmsCode();
                var sessionToken = Guid.NewGuid().ToString();

                // Store session
                _smsSessions[sessionToken] = new SmsSession
                {
                    PhoneNumber = phoneNumber,
                    Code = smsCode,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(2),
                    IsUsed = false
                };

                // For demo purposes, we'll just log the code (in production, send SMS)
                Console.WriteLine($"SMS Code for {phoneNumber}: {smsCode}");

                return new SmsResult
                {
                    Success = true,
                    Message = "کد پیامک ارسال شد",
                    SessionToken = sessionToken
                };
            }
            catch (Exception ex)
            {
                return new SmsResult
                {
                    Success = false,
                    Message = "خطا در ارسال پیامک"
                };
            }
        }

        public async Task<LoginResult> VerifySmsCodeAsync(string phoneNumber, string smsCode, string sessionToken, string role)
        {
            try
            {
                if (!_smsSessions.ContainsKey(sessionToken))
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "جلسه منقضی شده است"
                    };
                }

                var session = _smsSessions[sessionToken];
                if (session.IsUsed || session.ExpiresAt < DateTime.UtcNow)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "کد منقضی شده است"
                    };
                }

                if (session.PhoneNumber != phoneNumber || session.Code != smsCode)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "کد وارد شده اشتباه است"
                    };
                }

                // Mark as used
                session.IsUsed = true;

                var user = await _userManager.FindByNameAsync(phoneNumber);
                if (user == null)
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "کاربر یافت نشد"
                    };
                }

                var token = await GenerateJwtTokenAsync(user);
                var userInfo = await GetUserInfoAsync(user);

                return new LoginResult
                {
                    Success = true,
                    Message = "ورود موفقیت آمیز بود",
                    Token = token,
                    UserInfo = userInfo
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "خطا در تایید کد"
                };
            }
        }

        public async Task<bool> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserInfo> GetUserInfoFromTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);

                var userId = jsonToken.Claims.First(x => x.Type == "sub").Value;
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    return await GetUserInfoAsync(user);
                }

                return new UserInfo();
            }
            catch
            {
                return new UserInfo();
            }
        }

        public async Task<QRCodeResult> GenerateQRCodeAsync(string sessionToken)
        {
            try
            {
                var qrToken = Guid.NewGuid().ToString();
                var qrData = $"{_configuration["BaseUrl"]}/api/auth/qr-login?token={qrToken}";

                _qrSessions[qrToken] = new QRSession
                {
                    SessionToken = sessionToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    IsScanned = false,
                    Token = null
                };

                return new QRCodeResult
                {
                    Success = true,
                    QRData = qrData,
                    QRToken = qrToken
                };
            }
            catch (Exception ex)
            {
                return new QRCodeResult
                {
                    Success = false,
                    Message = "خطا در تولید QR Code"
                };
            }
        }

        public async Task<QRStatusResult> CheckQRCodeStatusAsync(string qrToken)
        {
            try
            {
                if (!_qrSessions.ContainsKey(qrToken))
                {
                    return new QRStatusResult
                    {
                        Success = false,
                        Message = "QR Code منقضی شده است"
                    };
                }

                var session = _qrSessions[qrToken];
                if (session.ExpiresAt < DateTime.UtcNow)
                {
                    return new QRStatusResult
                    {
                        Success = false,
                        Message = "QR Code منقضی شده است"
                    };
                }

                return new QRStatusResult
                {
                    Success = true,
                    IsScanned = session.IsScanned,
                    Token = session.Token
                };
            }
            catch (Exception ex)
            {
                return new QRStatusResult
                {
                    Success = false,
                    Message = "خطا در بررسی وضعیت QR Code"
                };
            }
        }

        public Task<RequestResponse<JwtTokenResponse>> LoginAdminAsync(string accountId)
        {
            throw new NotImplementedException();
        }
    }
}