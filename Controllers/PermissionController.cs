using ShopManagementAPI.DTOs.request;
using ShopManagementAPI.DTOs.response;
using ShopManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopManagementAPI.Authorization;

namespace ShopManagementAPI.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    [Produces("application/json")]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionService _service;

        public PermissionController(PermissionService service)
        {
            _service = service;
        }

        [Authorize]
        [RequirePermission(Permissions.GetPermissions)]
        [HttpGet]
        public async Task<ActionResult<List<PermissionResponseDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [Authorize]
        [RequirePermission(Permissions.GetPermissionDetail)]
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionResponseDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }


    }
}
