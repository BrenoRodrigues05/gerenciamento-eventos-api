using APIGerenciamento.Controllers;
using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Mappings;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GerenciamentoTest.EventoUnitTest
{
    public class PostEventosTests
    {
        private readonly EventosController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IDTOMapper<EventoDTO, Evento, EventoPatchDTO> _mapper;
        
        public PostEventosTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mapper = new EventoMapper();

            // Mock do repositório de eventos
            var mockRepo = new Mock<IEventoRepository>();
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Evento>()))
                    .Returns(Task.CompletedTask); // Simula AddAsync sem alterar banco

            _mockUnitOfWork.Setup(u => u.Eventos).Returns(mockRepo.Object);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Instancia real do service
            var eventosService = new EventosService(_mockUnitOfWork.Object);
            var fakeCache = new FakeEventosCacheService(_mockUnitOfWork.Object, _mapper);

            // Controller com logger nulo e service real
            _controller = new EventosController(
                _mockUnitOfWork.Object,
                NullLogger<EventosController>.Instance,
                _mapper,
                eventosService,
                fakeCache
            );
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenDtoIsValid()
        {
            // Arrange
            var dto = new EventoDTO
            {
                Titulo = "Evento Teste",
                Data = DateTime.Now.AddDays(1),
                Local = "Auditório",
                Descricao = "Descrição do evento teste",
                Vagas = 100,
                Cidade = "São Paulo",
                Entrada = "Gratuita"
            };

            // Act
            var result = await _controller.Create(dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.Value.Should().NotBeNull();

            var createdDto = createdResult.Value as EventoDTO;
            createdDto.Titulo.Should().Be(dto.Titulo);
            createdDto.Data.Should().BeCloseTo(dto.Data, TimeSpan.FromSeconds(1));
            createdDto.Local.Should().Be(dto.Local);
            createdDto.Descricao.Should().Be(dto.Descricao);
            createdDto.Vagas.Should().Be(dto.Vagas);
            createdDto.Cidade.Should().Be(dto.Cidade);
            createdDto.Entrada.Should().Be(dto.Entrada);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var dto = new EventoDTO(); // DTO vazio, inválido
            _controller.ModelState.AddModelError("Titulo", "Título é obrigatório");

            // Act
            var result = await _controller.Create(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
