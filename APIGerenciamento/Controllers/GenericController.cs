using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace APIGerenciamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<TEntity, TDto, TPatchDto> : ControllerBase where TEntity : class, IEntidade
    where TDto : class where TPatchDto : class
    {
        private readonly IRepository<TEntity> _repository;
        private readonly ILogger<GenericController<TEntity, TDto, TPatchDto>> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDTOMapper<TDto, TEntity, TPatchDto> _mapper;

        public GenericController(IUnitOfWork unitOfWork, ILogger<GenericController<TEntity, TDto, TPatchDto>> logger, 
            IDTOMapper<TDto, TEntity, TPatchDto> mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

            _repository = typeof(TEntity).Name switch
            {
                nameof(Evento) => (IRepository<TEntity>)_unitOfWork.Eventos,
                nameof(Participante) => (IRepository<TEntity>)_unitOfWork.Participantes,
                nameof(Inscricao) => (IRepository<TEntity>)_unitOfWork.Inscricoes,
                _ => throw new NotSupportedException($"Repositório não encontrado para {typeof(TEntity).Name}")
            };
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _repository.GetAllAsync();
            var dtos = entities.Select(e => _mapper.ToDto(e)).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(_mapper.ToDto(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var entity = _mapper.ToEntity(dto);
                await _repository.AddAsync(entity);
                await _unitOfWork.CommitAsync();

                var dtoCreated = _mapper.ToDto(entity);
                return CreatedAtAction(nameof(GetById), new { id = GetEntityId(entity) }, dtoCreated);
            }
            catch (Exception)
            {
                _logger.LogError("❌ Erro ao criar {Entity}", typeof(TEntity).Name);
                return StatusCode(500, "Erro interno ao criar o recurso.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var updatedEntity = _mapper.ToEntity(dto);
            _repository.Update(updatedEntity);
            await _unitOfWork.CommitAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<TPatchDto> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var patchDto = _mapper.ToPatchDto(entity);

            patchDoc.ApplyTo(patchDto, ModelState);

            if (!TryValidateModel(patchDto)) return BadRequest(ModelState);

            _mapper.PatchToEntity(patchDto, entity);

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

        private object GetEntityId(object entity)
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
