using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Pagination;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGerenciamento.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipanteController : GenericController<Participante, ParticipanteDTO, ParticipantePatchDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ParticipanteController(
    IUnitOfWork unitOfWork,
    ILogger<GenericController<Participante, ParticipanteDTO, ParticipantePatchDTO>> logger,
    IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> mapper, EventosService eventosService)
    : base(unitOfWork, logger, mapper, eventosService)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("filtro-nome")]

        public async Task<IActionResult> GetFiltroNomesParticipantesAsync([FromQuery] FiltroNomeParticipante filtro)
        {
            if (filtro.PageNumber < 1 || filtro.PageSize < 1)
                return BadRequest("Os parâmetros 'PageNumber' e 'PageSize' devem ser maiores que zero.");

            var resultado = await _unitOfWork.ParticipanteRepository.GetFiltroNomesParticipantesAsync(filtro);

            if (resultado.Items == null || !resultado.Items.Any())
                return NotFound("Nenhum participante encontrado com os critérios informados.");

            return Ok(resultado);
        }
    }
}
