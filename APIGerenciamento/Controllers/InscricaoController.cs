using APIGerenciamento.DTOs;
using APIGerenciamento.Models;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIGerenciamento.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de inscrições em eventos.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class InscricaoController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly InscricaoCacheService _inscricaoCacheService;

        /// <summary>
        /// Construtor do controller de inscrições.
        /// </summary>
        /// <param name="uow">Unit of Work para acesso aos repositórios</param>
        public InscricaoController(IUnitOfWork uow, InscricaoCacheService inscricaoCacheService)
        {
            _uow = uow;
            _inscricaoCacheService = inscricaoCacheService;
        }

        /// <summary>
        /// Retorna todas as inscrições cadastradas.
        /// </summary>
        /// <remarks>
        /// Necessita permissão de Admin ou SuperAdmin.
        /// </remarks>
        /// <returns>Lista de inscrições com informações do evento e participante.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var inscricoes = await _inscricaoCacheService.GetAllAsync();

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

        /// <summary>
        /// Cria uma nova inscrição para um participante em um evento.
        /// </summary>
        /// <param name="dto">DTO com os IDs do evento e do participante</param>
        /// <returns>Inscrição criada com sucesso ou mensagem de erro.</returns>
        /// <response code="201">Inscrição criada com sucesso.</response>
        /// <response code="400">Erro de validação, evento ou participante não encontrados, evento lotado ou participante já inscrito.</response>
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

            _inscricaoCacheService.InvalidateCache();

            await _uow.CommitAsync();

            dto.Id = novaInscricao.Id;
            dto.DataInscricao = novaInscricao.DataInscricao;
            dto.NomeEvento = evento.Titulo;
            dto.NomeParticipante = participante.Nome;

            return CreatedAtAction(nameof(Create), new { id = dto.Id }, dto);
        }
    }
}
