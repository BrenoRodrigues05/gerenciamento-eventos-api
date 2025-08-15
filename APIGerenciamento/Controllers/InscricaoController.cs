using APIGerenciamento.DTOs;
using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGerenciamento.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class InscricaoController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public InscricaoController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var inscricoes = await _uow.Inscricoes.GetAllAsync();

            var resposta = inscricoes.Select(i => new InscricaoDTO
            {
                Id = i.Id,
                EventoId = i.EventoId,
                ParticipanteId = i.ParticipanteId,
                DataInscricao = i.DataInscricao,
                NomeEvento = i.Evento?.Titulo,
                NomeParticipante = i.Participante?.Nome
            });

            return Ok(resposta);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InscricaoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var evento = await _uow.Eventos.GetByIdAsync(dto.EventoId);
            if (evento is null)
                return BadRequest("Evento não encontrado.");

            var participante = await _uow.Participantes.GetByIdAsync(dto.ParticipanteId);
            if (participante is null)
                return BadRequest("Participante não encontrado.");

            var inscricoesEvento = (await _uow.Inscricoes.GetAllAsync())
                .Count(i => i.EventoId == dto.EventoId);

            if (inscricoesEvento >= evento.Vagas)
                return BadRequest("Evento lotado.");

            var jaInscrito = (await _uow.Inscricoes.GetAllAsync())
                .Any(i => i.EventoId == dto.EventoId && i.ParticipanteId == dto.ParticipanteId);

            if (jaInscrito)
                return BadRequest("Participante já inscrito.");

            var novaInscricao = new Inscricao
            {
                EventoId = dto.EventoId,
                ParticipanteId = dto.ParticipanteId,
                DataInscricao = DateTime.Now
            };

            await _uow.Inscricoes.AddAsync(novaInscricao);

            evento.Vagas -= 1;
            _uow.Eventos.Update(evento);

            await _uow.CommitAsync();

            dto.Id = novaInscricao.Id;
            dto.DataInscricao = novaInscricao.DataInscricao;
            dto.NomeEvento = evento.Titulo;
            dto.NomeParticipante = participante.Nome;

            return CreatedAtAction(nameof(Create), new { id = dto.Id }, dto);
        }
    }
}
