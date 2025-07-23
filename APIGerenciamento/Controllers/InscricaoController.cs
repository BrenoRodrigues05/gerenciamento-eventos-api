using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGerenciamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscricaoController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public InscricaoController(IUnitOfWork u) => _uow = u;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
        Ok(await _uow.Inscricoes.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Inscricao ins)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var evt = await _uow.Eventos.GetByIdAsync(ins.EventoId);
            if (evt is null) return BadRequest("Evento não encontrado.");

            var inscricoes = (await _uow.Inscricoes.GetAllAsync())
                               .Count(i => i.EventoId == ins.EventoId);
            if (inscricoes >= evt.Vagas)
                return BadRequest("Evento lotado.");

            var part = await _uow.Participantes.GetByIdAsync(ins.ParticipanteId);
            if (part is null) return BadRequest("Participante não encontrado.");

            bool exists = (await _uow.Inscricoes.GetAllAsync())
                           .Any(i => i.EventoId == ins.EventoId && i.ParticipanteId == ins.ParticipanteId);
            if (exists)
                return BadRequest("Participante já inscrito.");

            await _uow.Inscricoes.AddAsync(ins);
            await _uow.CommitAsync();
            return CreatedAtAction(nameof(Create), new { id = ins.Id }, ins);
        }
    }
}
