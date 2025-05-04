

namespace WebApi.Filters
{

    public class JwtTokenMiddleware
    {

        public JwtTokenMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.Next = next;
            this.Configuration = configuration;
        }


        private RequestDelegate Next { get; }


        private IConfiguration Configuration { get; }


        public async Task Invoke(HttpContext httpContext)
        {
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null && !this.IsCurrentTokenValid(token))
            {
                httpContext.Request.Headers["Authorization"] = string.Empty;
            }

            await this.Next(httpContext);
        }


        private bool IsCurrentTokenValid(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var isTokenValid = true;

            try
            {
                #pragma warning disable SA1117 // Parameters should be on same line or separate lines
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromSeconds(1),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(this.Configuration["JwtToken:SecretKey"]!)),
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = this.Configuration["JwtToken:Audience"],
                    ValidIssuer = this.Configuration["JwtToken:Issuer"],
                }, validatedToken: out SecurityToken validatedToken);
                #pragma warning restore SA1117 // Parameters should be on same line or separate lines
            }
            catch (Exception)
            {
                isTokenValid = false;
            }

            return isTokenValid;
        }
    }
}
