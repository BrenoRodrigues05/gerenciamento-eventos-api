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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace GerenciamentoTest.EventoUnitTest
{
    public class PatchEventosTests
    {
        private readonly EventosController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEventoRepository> _mockRepo;
        private readonly EventoMapper _mapper;
        private readonly EventosService _eventosService;

        public PatchEventosTests()
        {
            // Mocks
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IEventoRepository>();
            _mapper = new EventoMapper();

            // Setup do UnitOfWork
            _mockUnitOfWork.Setup(u => u.Eventos).Returns(_mockRepo.Object);

            // Serviço real usando UnitOfWork mockado
            _eventosService = new EventosService(_mockUnitOfWork.Object);

            // Controller
            _controller = new EventosController(
                _mockUnitOfWork.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<EventosController>>(),
                _mapper,
                _eventosService
            );

            // Necessário para TryValidateModel
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Patch_ShouldReturnBadRequest_WhenPatchDocIsNull()
        {
            // Arrange
            JsonPatchDocument<EventoPatchDTO> patchDoc = null;

            // Act
            var result = await _controller.Patch(1, patchDoc);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Patch_ShouldReturnNotFound_WhenEventoDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                     .ReturnsAsync((Evento)null);

            var patchDoc = new JsonPatchDocument<EventoPatchDTO>();

            // Act
            var result = await _controller.Patch(1, patchDoc);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Patch_ShouldReturnNoContent_WhenPatchIsAppliedSuccessfully()
        {
            // Arrange
            var evento = new Evento { Id = 1, Titulo = "Evento Teste" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(evento);

            var patchDoc = new JsonPatchDocument<EventoPatchDTO>();
            patchDoc.Replace(e => e.Titulo, "Evento Atualizado");

            // Configura ControllerContext e ObjectValidator para evitar NullReference
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            var mockValidator = new Mock<IObjectModelValidator>();
            _controller.ObjectValidator = mockValidator.Object;

            // Act
            var result = await _controller.Patch(1, patchDoc);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            (result as NoContentResult).StatusCode.Should().Be(204);

            // Verifica se Update e Commit foram chamados
            _mockRepo.Verify(r => r.Update(evento), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
