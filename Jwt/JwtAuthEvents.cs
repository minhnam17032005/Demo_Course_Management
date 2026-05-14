namespace Demo_Course_Management.Jwt
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text.Json;

    public class JwtAuthEvents : JwtBearerEvents
    {
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            context.NoResult();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var message = context.Exception switch
            {
                SecurityTokenExpiredException =>
                    "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.",

                SecurityTokenInvalidSignatureException =>
                    "Token không hợp lệ. Vui lòng đăng nhập lại.",

                _ =>
                    "Xác thực thất bại. Vui lòng đăng nhập lại."
            };

            var result = JsonSerializer.Serialize(new
            {
                status = 401,
                message
            });

            return context.Response.WriteAsync(result);
        }

        public override Task Challenge(JwtBearerChallengeContext context)
        {
            // tránh ASP.NET tự ghi response
            context.HandleResponse();

            // response đã ghi rồi thì thôi
            if (context.Response.HasStarted)
                return Task.CompletedTask;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                status = 401,
                message = "Bạn chưa đăng nhập."
            });

            return context.Response.WriteAsync(result);
        }
    }
}
