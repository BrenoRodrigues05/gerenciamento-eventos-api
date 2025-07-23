using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace APIGerenciamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : ControllerBase where T : class, IEntidade
    {
        private readonly IRepository<T> _repository;
        private readonly ILogger<GenericController<T>> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public GenericController(IUnitOfWork unitOfWork, ILogger<GenericController<T>> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            _repository = typeof(T).Name switch
            {
                nameof(Evento) => (IRepository<T>)_unitOfWork.Eventos,
                nameof(Participante) => (IRepository<T>)_unitOfWork.Participantes,
                nameof(Inscricao) => (IRepository<T>)_unitOfWork.Inscricoes,
                _ => throw new NotSupportedException($"Repositório não encontrado para {typeof(T).Name}")
            };
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repository.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] T entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _repository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = GetEntityId(entity) }, entity);
            }
            catch (Exception)
            {
                _logger.LogError("❌ Erro ao criar {Entity}", typeof(T).Name);
                return StatusCode(500, "Erro interno ao criar o recurso.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] T entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _repository.Remove(entity);
            await _unitOfWork.CommitAsync();
            return NoContent();
        }

        private object GetEntityId(T entity)
        {
            var prop = entity?.GetType().GetProperty("Id");
            if (prop == null)
            {
                throw new InvalidOperationException("A entidade precisa ter uma propriedade pública chamada 'Id'");
            }
            return prop.GetValue(entity)!;
        }
    }
}
