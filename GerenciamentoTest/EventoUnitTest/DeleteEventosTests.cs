using APIGerenciamento.Controllers;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace GerenciamentoTest.EventoUnitTest
{
    public class DeleteEventosTests
    {
        private readonly EventosController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEventoRepository> _mockEventoRepo;

        public DeleteEventosTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEventoRepo = new Mock<IEventoRepository>();

            // Configura ControllerContext para consistência
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Mock de retorno de evento existente
            _mockEventoRepo.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new Evento { Id = 10, Titulo = "Evento Existente" });

            // Mock de remoção (não altera nada no banco)
            _mockEventoRepo.Setup(r => r.Remove(It.IsAny<Evento>()));

            // Configura o UnitOfWork para retornar o repo mockado
            _mockUnitOfWork.Setup(u => u.Eventos).Returns(_mockEventoRepo.Object);

            var eventosService = new EventosService(_mockUnitOfWork.Object);
            var logger = NullLogger<EventosController>.Instance;

            _controller = new EventosController(
                _mockUnitOfWork.Object,
                logger,
                new APIGerenciamento.DTOs.Mappings.EventoMapper(),
                eventosService
            )
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenEventoExists()
        {
            // Act
            var result = await _controller.Delete(10);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            (result as NoContentResult).StatusCode.Should().Be(204);

            // Verifica se Remove e Commit foram chamados
            _mockEventoRepo.Verify(r => r.Remove(It.IsAny<Evento>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenEventoDoesNotExist()
        {
            // Arrange
            _mockEventoRepo.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Evento)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
