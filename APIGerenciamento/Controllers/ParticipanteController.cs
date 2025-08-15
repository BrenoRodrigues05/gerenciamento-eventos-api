using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace APIGerenciamento.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de participantes.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    public class ParticipantesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ParticipantesController> _logger;
        private readonly IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> _mapper;

        /// <summary>
        /// Construtor do controller de participantes.
        /// </summary>
        public ParticipantesController(IUnitOfWork unitOfWork,
            ILogger<ParticipantesController> logger,
            IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Retorna todos os participantes cadastrados.
        /// </summary>
        /// <returns>Lista de ParticipanteDTO</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var participantes = await _unitOfWork.Participantes.GetAllAsync();
            var dtos = participantes.Select(p => _mapper.ToDto(p));
            return Ok(dtos);
        }

        /// <summary>
        /// Retorna um participante pelo ID.
        /// </summary>
        /// <param name="id">ID do participante</param>
        /// <returns>ParticipanteDTO correspondente</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var participante = await _unitOfWork.Participantes.GetByIdAsync(id);
            if (participante == null) return NotFound();
            return Ok(_mapper.ToDto(participante));
        }

        /// <summary>
        /// Cria um novo participante.
        /// </summary>
        /// <param name="dto">Dados do participante a ser criado</param>
        /// <returns>Participante criado</returns>
        /// <response code="201">Participante criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno ao criar participante</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ParticipanteDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var participante = _mapper.ToEntity(dto);
                await _unitOfWork.Participantes.AddAsync(participante);
                await _unitOfWork.CommitAsync();

                var createdDto = _mapper.ToDto(participante);
                return CreatedAtAction(nameof(GetById), new { id = participante.Id }, createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar participante");
                return StatusCode(500, "Erro interno ao criar participante.");
            }
        }

        /// <summary>
        /// Atualiza todos os dados de um participante existente.
        /// </summary>
        /// <param name="id">ID do participante</param>
        /// <param name="dto">Dados atualizados</param>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ParticipanteDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _unitOfWork.Participantes.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var updated = _mapper.ToEntity(dto);
            updated.Id = id;

            _unitOfWork.Participantes.Update(updated);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        /// <summary>
        /// Atualiza parcialmente os dados de um participante.
        /// </summary>
        /// <param name="id">ID do participante</param>
        /// <param name="patchDoc">Documento JSON Patch contendo as alterações</param>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<ParticipantePatchDTO> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var participante = await _unitOfWork.Participantes.GetByIdAsync(id);
            if (participante == null) return NotFound();

            var patchDto = _mapper.ToPatchDto(participante);
            patchDoc.ApplyTo(patchDto, ModelState);

            if (!TryValidateModel(patchDto)) return BadRequest(ModelState);

            _mapper.PatchToEntity(patchDto, participante);
            _unitOfWork.Participantes.Update(participante);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        /// <summary>
        /// Remove um participante existente.
        /// </summary>
        /// <param name="id">ID do participante</param>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var participante = await _unitOfWork.Participantes.GetByIdAsync(id);
            if (participante == null) return NotFound();

            _unitOfWork.Participantes.Remove(participante);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
