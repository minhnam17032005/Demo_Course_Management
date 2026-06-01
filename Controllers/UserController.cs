using Azure;
using ShopManagementAPI.DTOs.request;
using ShopManagementAPI.DTOs.response;
using ShopManagementAPI.Models;
using ShopManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopManagementAPI.Authorization;

namespace ShopManagementAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [Authorize]
        [RequirePermission(Permissions.CreateUser)]
        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> Create([FromBody] CreateUserReqDTO dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),           
                new { id = result.Id },
                result
            );
        }

        [Authorize]
        [RequirePermission(Permissions.UpdateUserProfile)]
        [HttpPut("profile")]
        public async Task<ActionResult<UserResponseDTO>> ChangeProfile( ChangeProfileReqDTO dto)
        {
            var result = await _service.ChangeProfileAsync(dto);
            return Ok(result);
        }

        [Authorize]
        [RequirePermission(Permissions.AddUserRoles)]
        [HttpPost("{id}/roles")]
        public async Task<ActionResult<UserResponseDTO>> AddRoles(
            int id,
            [FromBody] ChangeRolesReqDTO dto)
        {
            var result = await _service.AddRolesAsync(id, dto.RoleIds);

            return Ok(new
            {
                message = "Thêm vai trò cho người dùng thành công.",
                data = result
            });
        }

        [Authorize]
        [RequirePermission(Permissions.RemoveUserRoles)]
        [HttpDelete("{id}/roles")]
        public async Task<ActionResult<UserResponseDTO>> RemoveRoles(
            int id,
            [FromBody] ChangeRolesReqDTO dto)
        {
            var result = await _service.RemoveRolesAsync(id, dto.RoleIds);

            return Ok(new {
                message = "Xóa vai trò khỏi người dùng thành công.",
                data = result
            });
        }

        [Authorize]
        [RequirePermission(Permissions.GetUsers)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [Authorize]
        [RequirePermission(Permissions.GetUserDetail)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize]
        [RequirePermission(Permissions.LockUser)]
        [HttpPatch("{id}/lock")]
        public async Task<ActionResult<StatusResponseDTO>> Lock(int id)
        {
            var response = await _service.LockAsync(id);
            return Ok(response);
        }

        [Authorize]
        [RequirePermission(Permissions.UnlockUser)]
        [HttpPatch("{id}/unlock")]
        public async Task<ActionResult<StatusResponseDTO>> Unlock(int id)
        {
            var response = await _service.UnlockAsync(id);
            return Ok(response);
        }
    }
}
