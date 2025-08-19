using APIGerenciamento.Controllers;
using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Mappings;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GerenciamentoTest.EventoUnitTest
{
    public class GetEventosTests
    {
        private readonly EventosController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEventoRepository> _mockRepo;
        private readonly EventoMapper _mapper;
        private readonly Mock<EventosService> _mockService;

        public GetEventosTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IEventoRepository>();
            _mapper = new EventoMapper();

            // Mock do service
            _mockService = new Mock<EventosService>(_mockUnitOfWork.Object);

            // Dados fake
            var eventosFake = new List<Evento>
            {
                new Evento { Id = 1, Titulo = "Evento 1", Data = System.DateTime.Now, Local = "Local 1", 
                    Descricao = "Desc 1", Vagas = 10, Cidade = "Cidade 1", Entrada = "Gratuita" },
                new Evento { Id = 2, Titulo = "Evento 2", Data = System.DateTime.Now, Local = "Local 2", 
                    Descricao = "Desc 2", Vagas = 20, Cidade = "Cidade 2", Entrada = "Pago" }
            };

            // Setup do repositório
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(eventosFake);
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(eventosFake[0]);
            _mockRepo.Setup(r => r.GetByIdAsync(It.Is<int>(id => id != 1))).ReturnsAsync((Evento)null);

            _mockUnitOfWork.Setup(u => u.Eventos).Returns(_mockRepo.Object);

            // Instancia do controller com todos os parâmetros
            _controller = new EventosController(
                _mockUnitOfWork.Object,
                Mock.Of<ILogger<EventosController>>(),
                _mapper,
                _mockService.Object
            );
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithListOfEventos()
        {
            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().NotBeNull();

            var listaEventos = okResult.Value as IEnumerable<EventoDTO>;
            listaEventos.Should().NotBeNull();
            listaEventos.Count().Should().BeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenEventoExists()
        {
            var existingId = 1;
            var result = await _controller.GetById(existingId);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var eventoDto = okResult.Value as EventoDTO;

            eventoDto.Should().NotBeNull();
            eventoDto.Id.Should().Be(existingId);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenEventoDoesNotExist()
        {
            var nonExistingId = 999;
            var result = await _controller.GetById(nonExistingId);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
