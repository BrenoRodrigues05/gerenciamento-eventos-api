using APIGerenciamento.Controllers;
using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Mappings;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.UnitOfWork;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciamentoTest.ParticipanteUnitTest
{
    public class PostParticipanteUnitTests
    {
        private readonly ParticipantesController _participantesController;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> _mapper;

        public PostParticipanteUnitTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mapper = new ParticipanteMapper();

            var mockParticipanteRepo = new Mock<IParticipanteRepository>();

            mockParticipanteRepo = new Mock<IParticipanteRepository>();
            mockParticipanteRepo.Setup(r => r.AddAsync(It.IsAny<Participante>()))
            .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.Participantes).Returns(mockParticipanteRepo.Object);
            _mockUnitOfWork.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            _participantesController = new ParticipantesController(
             _mockUnitOfWork.Object,
             NullLogger<ParticipantesController>.Instance,
               _mapper
           );
        }

        [Fact]

        public async Task Create_ShouldReturnCreated_WhenValidParticipante()
        {
            // Arrange
            var dto = new ParticipanteDTO
            {
                Nome = "Fulano de Tal",
                Email = "fulano@teste.com",
                Telefone = "123456789"
            };

            //act

            var result = await _participantesController.Create(dto);

            // Assert

            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.Value.Should().NotBeNull();

            var createdDto = createdResult.Value as ParticipanteDTO;
            createdDto.Nome.Should().Be(dto.Nome);
            createdDto.Email.Should().Be(dto.Email);
            createdDto.Telefone.Should().Be(dto.Telefone);


        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalidModel()
        {
            // Arrange
            _participantesController.ModelState.AddModelError("Nome", "O campo Nome é obrigatório.");
            var dto = new ParticipanteDTO();

            _participantesController.ModelState.AddModelError("Nome", "O campo Nome é obrigatório.");

            //Act

            var result = await _participantesController.Create(dto);

            //Assert

            result.Should().BeOfType<BadRequestObjectResult>();
}   }   }   