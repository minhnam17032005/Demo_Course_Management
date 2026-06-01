using System.Text.Json;
using System.Text.RegularExpressions;
using ShopManagementAPI.Authorization;
using ShopManagementAPI.Jwt;
using ShopManagementAPI.Repositories;

namespace ShopManagementAPI.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            CurrentUserService currentUser,
            UserRepository userRepository,
            PermissionCacheService permissionCacheService
        )
        {
            //lấy ra endpoint 
            var endpoint = context.GetEndpoint();

            var permissionAttribute =
                endpoint?.Metadata
                    .GetMetadata<RequirePermissionAttribute>();

            // api không cần permission
            if (permissionAttribute == null)
            {
                await _next(context);
                return;
            }

            // lấy permissions từ cache service
            var permissions =
                await permissionCacheService
                    .GetPermissionsAsync(currentUser.UserId);

            // check permission
            var hasPermission = permissions.Contains(
                    permissionAttribute.Permission
                );

            // không có quyền
            if (!hasPermission)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode =StatusCodes.Status403Forbidden;

                var forbiddenResponse = new
                {
                    success = false,
                    statusCode = StatusCodes.Status403Forbidden,
                    code = "FORBIDDEN",
                    message = "Bạn không có quyền truy cập",
                    timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        forbiddenResponse
                    )
                );

                return;
            }

            await _next(context);
        }
    }

}
