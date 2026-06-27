using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tp_Programacion.Enums;
using Tp_Programacion.Models.Role;
using Tp_Programacion.Models.Role.Dto;
using Tp_Programacion.Services;
using Tp_Programacion.Utils;

namespace Tp_Programacion.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Roles = ROLES.Admin)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status500InternalServerError)]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }


        [HttpGet]
        [Authorize(Roles = $"{ROLES.Admin}")]
        [ProducesResponseType(typeof(List<Role>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Role>>> GetAll()
        {
            var r = await _roleService.GetAll();
            return Ok(r);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{ROLES.Admin}")]
        [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Role>> GetOneById(int id)
        {
            try
            {
                var rol = await _roleService.GetOneById(id);
                return Ok(rol);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Role), StatusCodes.Status201Created)]
        public async Task<ActionResult<Role>> CreateOne([FromBody] RoleDTO createRol)
        {
            try
            {
                var rol = await _roleService.CreateOne(createRol);
                return Created("POST api/Roles", rol);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Role>> UpdateOneById(int id, [FromBody] RoleDTO updateRol)
        {
            try
            {
                var rol = await _roleService.UpdateOneById(id, updateRol);
                return Ok(rol);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteOneById(int id)
        {
            try
            {
                await _roleService.DeleteOneById(id);
                ResponseMessage msg = new ResponseMessage($"Role con id = {id} ha sido eliminada");
                //return NoContent();

                return Ok(msg);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }
    }
}
