using APIGerenciamento.DTOs;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGerenciamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : GenericController<Evento, EventoDTO>
    {
        public EventoController(IUnitOfWork unitOfWork, ILogger
            <GenericController<Evento, EventoDTO>> logger, IDTOMapper<EventoDTO, Evento> mapper) : 
            base(unitOfWork, logger, mapper)
        {
        }
    }
    
}
