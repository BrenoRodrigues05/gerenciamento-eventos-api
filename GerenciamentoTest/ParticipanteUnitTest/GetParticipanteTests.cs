using APIGerenciamento.Controllers;
using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Mappings;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GerenciamentoTest.ParticipanteUnitTest
{
    public class GetParticipanteTests
    {
        private readonly ParticipantesController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IParticipanteRepository> _mockRepo;
        private readonly ParticipanteMapper _mapper;
        private readonly Mock<EventosService> _mockService;

        public GetParticipanteTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IParticipanteRepository>();
            _mapper = new ParticipanteMapper();
            
            _mockService = new Mock<EventosService>(
                _mockUnitOfWork.Object
          
            );

            var mockLogger = new List<Participante>
            {
                new Participante
                {
                    Id = 1,
                    Nome = "Participante Teste",
                    Email = "fulano@teste.com",
                    Telefone = "123456789"
                },
                 new Participante{
                    Id = 2,
                    Nome = "Participante Teste 2",
                    Email = "fulano2@teste.com",
                    Telefone = "987654321"

                 }
            };
            // Configurações do mock
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(mockLogger);
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(mockLogger[0]);
            _mockRepo.Setup(r => r.GetByIdAsync(It.Is<int>(id => id != 1))).ReturnsAsync((Participante)null);

            _mockUnitOfWork.Setup(u => u.Participantes).Returns(_mockRepo.Object);

            _controller = new ParticipantesController(
                _mockUnitOfWork.Object,
                 Mock.Of<ILogger<ParticipantesController>>(),
                 _mapper
            );
           
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllParticipantes()
        {
            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().NotBeNull();

            var participantes = okResult.Value as IEnumerable<ParticipanteDTO>;

            participantes.Should().NotBeNull();
            participantes.Count().Should().BeGreaterThanOrEqualTo(2);
           
        }

        [Fact]

        public async Task GetById_ShouldReturnParticipante_WhenIdExists()
        {
            var result = await _controller.GetById(1);

            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;

            okResult.Value.Should().NotBeNull();
            var participante = okResult.Value as ParticipanteDTO;

            participante.Should().NotBeNull();
            participante.Id.Should().Be(1);
            participante.Nome.Should().Be("Participante Teste");
        }

        [Fact]

        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var result = await _controller.GetById(999);

            result.Should().BeOfType<NotFoundResult>();
        }
    }

}
