using Microsoft.AspNetCore.Authorization;

namespace ShopManagementAPI.Authorization
{
    //đọc các nhãn quyền từ permissions 
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public string Permission { get; }

        public RequirePermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }

}
