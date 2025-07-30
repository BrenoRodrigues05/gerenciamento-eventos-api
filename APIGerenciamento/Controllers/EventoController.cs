using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGerenciamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : GenericController<Evento, EventoDTO, EventoPatchDTO>
    {
        public EventoController(
     IUnitOfWork unitOfWork,
     ILogger<GenericController<Evento, EventoDTO, EventoPatchDTO>> logger,
     IDTOMapper<EventoDTO, Evento, EventoPatchDTO> mapper, EventosService eventosService)
     : base(unitOfWork, logger, mapper, eventosService)
        {
           
        }
    }
    
}
