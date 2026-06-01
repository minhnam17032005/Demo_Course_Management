using ShopManagementAPI.DTOs.request;
using ShopManagementAPI.DTOs.response;
using ShopManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopManagementAPI.Authorization;

namespace ShopManagementAPI.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Produces("application/json")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _service;

        public RoleController(RoleService service)
        {
                _service = service;
        }

        [Authorize]
        [RequirePermission(Permissions.GetRoles)]
        [HttpGet]
        public async Task<ActionResult<List<RoleResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [Authorize]
        [RequirePermission(Permissions.GetRoleDetail)]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleResponseDTO>> GetById(int id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [Authorize]
        [RequirePermission(Permissions.AddRolePermissions)]
        [HttpPost("{id}/permissions")]
        public async Task<ActionResult<RolePermissionResponseDTO>> AddPermissions(int id,[FromBody] RolePermissionRequestDTO request)
        {
            return Ok(await _service.AddPermissionsAsync(id, request.PermissionIds));
        }

        [Authorize]
        [RequirePermission(Permissions.RemoveRolePermissions)]
        [HttpDelete("{id}/permissions")]
        public async Task<ActionResult<RolePermissionResponseDTO>> RemovePermissions(int id,[FromBody] RolePermissionRequestDTO request)
        {
            return Ok(await _service.RemovePermissionsAsync(id, request.PermissionIds));
        }




    }
}
