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
    public class ParticipanteController : GenericController<Participante, ParticipanteDTO>
    {
        public ParticipanteController(IUnitOfWork unitOfWork, ILogger
            <GenericController<Participante, ParticipanteDTO>> logger, IDTOMapper<ParticipanteDTO, Participante
                >mapper) : base(unitOfWork, logger, mapper)
        {
        }
    }
}
