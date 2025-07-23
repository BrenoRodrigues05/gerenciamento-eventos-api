using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGerenciamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : GenericController<Evento>
    {
        public EventoController(IUnitOfWork unitOfWork, ILogger
            <GenericController<Evento>> logger) : base(unitOfWork, logger)
        {
        }
    }
    
}
