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
    public class EventoController : GenericController<Evento, EventoDTO, EventoPatchDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EventoController(
     IUnitOfWork unitOfWork,
     ILogger<GenericController<Evento, EventoDTO, EventoPatchDTO>> logger,
     IDTOMapper<EventoDTO, Evento, EventoPatchDTO> mapper, EventosService eventosService)
     : base(unitOfWork, logger, mapper, eventosService)
        {
            _unitOfWork = unitOfWork;
        }
    
    [HttpGet("filtro-título")]
       
        public async Task<IActionResult> GetFiltroTitulosEventosAsync([FromQuery] FiltroEventoTitulo filtro)
        {
            if (filtro.PageNumber < 1 || filtro.PageSize < 1)
                return BadRequest(new { message = "PageNumber e PageSize devem ser maiores que zero." });

            var pagedEventos = await _unitOfWork.EventoRepository.GetTituloEventoAsync(filtro);

            if (pagedEventos == null || !pagedEventos.Items.Any())
                return NotFound(new { message = "Nenhum evento encontrado com o título informado." });

            
            var eventosDTO = pagedEventos.Items.Select(e => new EventoDTO
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Data = e.Data,
                Local = e.Local,
              
            }).ToList();

            var response = new
            {
                Items = eventosDTO,
                pagedEventos.TotalItems,
                pagedEventos.PageNumber,
                pagedEventos.PageSize
            };

            Response.Headers["X-Total-Count"] = pagedEventos.TotalItems.ToString();
            Response.Headers["X-Page-Number"] = pagedEventos.PageNumber.ToString();
            Response.Headers["X-Page-Size"] = pagedEventos.PageSize.ToString();

            return Ok(response);
        }
}   }    
