using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
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
        public ParticipanteController(
    IUnitOfWork unitOfWork,
    ILogger<GenericController<Participante, ParticipanteDTO, ParticipantePatchDTO>> logger,
    IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> mapper, EventosService eventosService)
    : base(unitOfWork, logger, mapper, eventosService)
        {
        }
    }
}
