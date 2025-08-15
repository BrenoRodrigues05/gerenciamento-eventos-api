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
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EventosController> _logger;
        private readonly IDTOMapper<EventoDTO, Evento, EventoPatchDTO> _mapper;
        private readonly EventosService _eventosService;

        public EventosController(IUnitOfWork unitOfWork,
            ILogger<EventosController> logger,
            IDTOMapper<EventoDTO, Evento, EventoPatchDTO> mapper,
            EventosService eventosService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _eventosService = eventosService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventos = await _unitOfWork.Eventos.GetAllAsync();
            var dtos = eventos.Select(e => _mapper.ToDto(e));
            return Ok(dtos);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evento = await _unitOfWork.Eventos.GetByIdAsync(id);
            if (evento == null) return NotFound();
            return Ok(_mapper.ToDto(evento));
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var evento = _mapper.ToEntity(dto);
                await _unitOfWork.Eventos.AddAsync(evento);
                await _unitOfWork.CommitAsync();

                var createdDto = _mapper.ToDto(evento);
                return CreatedAtAction(nameof(GetById), new { id = evento.Id }, createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar evento");
                return StatusCode(500, "Erro interno ao criar evento.");
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EventoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _unitOfWork.Eventos.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var updated = _mapper.ToEntity(dto);
            updated.Id = id;

            _unitOfWork.Eventos.Update(updated);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<EventoPatchDTO> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var evento = await _unitOfWork.Eventos.GetByIdAsync(id);
            if (evento == null) return NotFound();

            var patchDto = _mapper.ToPatchDto(evento);
            patchDoc.ApplyTo(patchDto, ModelState);

            if (!TryValidateModel(patchDto)) return BadRequest(ModelState);

            _mapper.PatchToEntity(patchDto, evento);
            _unitOfWork.Eventos.Update(evento);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var evento = await _unitOfWork.Eventos.GetByIdAsync(id);
            if (evento == null) return NotFound();

            _unitOfWork.Eventos.Remove(evento);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
