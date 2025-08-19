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
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GerenciamentoTest.EventoUnitTest
{
    public class PutEventosTests
    {
        private readonly EventosController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEventoRepository> _mockRepo;
        private readonly Mock<ILogger<EventosController>> _mockLogger;
        private readonly IDTOMapper<EventoDTO, Evento, EventoPatchDTO> _mapper;

        public PutEventosTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IEventoRepository>();
            _mockLogger = new Mock<ILogger<EventosController>>();
            _mapper = new EventoMapper();

            _mockUnitOfWork.Setup(u => u.Eventos).Returns(_mockRepo.Object);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            _controller = new EventosController(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mapper,
                null // Sem serviço para PUT
            );
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenEventoExists()
        {
            // Arrange
            var existingEvento = new Evento
            {
                Id = 6,
                Titulo = "Evento Antigo",
                Data = DateTime.Now,
                Local = "Auditório Antigo",
                Vagas = 50,
                Cidade = "São Paulo",
                Entrada = "Gratuita",
                Descricao = "Descrição antiga"
            };

            var dto = new EventoDTO
            {
                Titulo = "Evento Atualizado",
                Data = DateTime.Now.AddDays(5),
                Local = "Auditório",
                Vagas = 100,
                Cidade = "São Paulo",
                Entrada = "Gratuita",
                Descricao = "Descrição do evento atualizado"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(existingEvento.Id))
                     .ReturnsAsync(existingEvento);

            // Act
            var result = await _controller.Update(existingEvento.Id, dto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            existingEvento.Titulo.Should().Be(dto.Titulo);
            existingEvento.Data.Should().Be(dto.Data);
            existingEvento.Local.Should().Be(dto.Local);
            existingEvento.Vagas.Should().Be(dto.Vagas);
            existingEvento.Cidade.Should().Be(dto.Cidade);
            existingEvento.Entrada.Should().Be(dto.Entrada);
            existingEvento.Descricao.Should().Be(dto.Descricao);

            _mockRepo.Verify(r => r.Update(existingEvento), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var dto = new EventoDTO(); // DTO inválido
            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");
            var id = 1;

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenEventoDoesNotExist()
        {
            // Arrange
            var dto = new EventoDTO
            {
                Titulo = "Evento Inexistente",
                Data = DateTime.Now.AddDays(5),
                Local = "Auditório",
                Descricao = "Descrição do evento inexistente",
                Vagas = 50,
                Cidade = "Rio de Janeiro",
                Entrada = "Gratuita"
            };

            var nonExistingId = 9999; // ID que não existe

            _mockRepo.Setup(r => r.GetByIdAsync(nonExistingId))
                     .ReturnsAsync((Evento)null);

            // Act
            var result = await _controller.Update(nonExistingId, dto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
