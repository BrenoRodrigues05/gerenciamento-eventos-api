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
    /// Controller responsável pelo gerenciamento de eventos.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EventosController> _logger;
        private readonly IDTOMapper<EventoDTO, Evento, EventoPatchDTO> _mapper;
        private readonly EventosService _eventosService;
        private readonly EventosCacheService _eventosCacheService;

        /// <summary>
        /// Construtor do controller de eventos.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work para persistência</param>
        /// <param name="logger">Logger para rastreamento de erros</param>
        /// <param name="mapper">Mapper para conversão entre DTOs e entidades</param>
        /// <param name="eventosService">Serviço de regras de negócio para eventos</param>
        public EventosController(IUnitOfWork unitOfWork,
            ILogger<EventosController> logger,
            IDTOMapper<EventoDTO, Evento, EventoPatchDTO> mapper,
            EventosService eventosService,
            EventosCacheService eventosCacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _eventosService = eventosService;
            _eventosCacheService = eventosCacheService;
        }

        /// <summary>
        /// Retorna todos os eventos.
        /// </summary>
        /// <returns>Lista de eventos em formato DTO.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var dtos = await _eventosCacheService.GetAllAsync();
            return Ok(dtos);
        }

        /// <summary>
        /// Retorna um evento pelo ID.
        /// </summary>
        /// <param name="id">ID do evento</param>
        /// <returns>Evento correspondente ao ID informado.</returns>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evento = await _eventosCacheService.GetEventoByIdAsync(id);
            if (evento == null) return NotFound();
            return Ok(evento);
        }

        /// <summary>
        /// Cria um novo evento.
        /// </summary>
        /// <param name="dto">DTO contendo os dados do evento</param>
        /// <returns>Evento criado com sucesso.</returns>
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

                _eventosCacheService.InvalidateCache();

                var createdDto = _mapper.ToDto(evento);
                return CreatedAtAction(nameof(GetById), new { id = evento.Id }, createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar evento");
                return StatusCode(500, "Erro interno ao criar evento.");
            }
        }

        /// <summary>
        /// Atualiza um evento existente completamente.
        /// </summary>
        /// <param name="id">ID do evento a ser atualizado</param>
        /// <param name="dto">DTO com os dados atualizados do evento</param>
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EventoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _unitOfWork.Eventos.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Titulo = dto.Titulo;
            existing.Data = dto.Data;
            existing.Local = dto.Local;
            existing.Vagas = dto.Vagas;
            existing.Cidade = dto.Cidade;
            existing.Entrada = dto.Entrada;
            existing.Descricao = dto.Descricao;

            _unitOfWork.Eventos.Update(existing);
            await _unitOfWork.CommitAsync();

            _eventosCacheService.InvalidateCache(id);

            return NoContent();
        }

        /// <summary>
        /// Aplica alterações parciais a um evento existente.
        /// </summary>
        /// <param name="id">ID do evento a ser alterado</param>
        /// <param name="patchDoc">Documento JSON Patch com as alterações</param>
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

            _eventosCacheService.InvalidateCache(id);

            return NoContent();
        }

        /// <summary>
        /// Remove um evento existente.
        /// </summary>
        /// <param name="id">ID do evento a ser removido</param>
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var evento = await _unitOfWork.Eventos.GetByIdAsync(id);
            if (evento == null) return NotFound();

            _unitOfWork.Eventos.Remove(evento);
            await _unitOfWork.CommitAsync();

            _eventosCacheService.InvalidateCache(id);

            return NoContent();
        }
    }
}
