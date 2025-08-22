using APIGerenciamento.Controllers;
using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Mappings;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciamentoTest.ParticipanteUnitTest
{
    public class DeleteParticipanteTests
    {
        private readonly ParticipantesController _Controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IParticipanteRepository> _mockParticipanteRepository;
        private readonly IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> _mapper;

        public DeleteParticipanteTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockParticipanteRepository = new Mock<IParticipanteRepository>();

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _mockParticipanteRepository.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new Participante { Id = 10, Nome = "Fulano Existente" });

            _mockParticipanteRepository.Setup(r => r.Remove(It.IsAny<Participante>()));

            _mockUnitOfWork.Setup(u => u.Participantes)
                .Returns(_mockParticipanteRepository.Object);

            var participantesService = new EventosService(_mockUnitOfWork.Object);
            var logger = NullLogger<ParticipantesController>.Instance;

            _Controller = new ParticipantesController(
                _mockUnitOfWork.Object,
                logger,
               _mapper
            );
        }

        [Fact]

        public async Task Delete_ShouldReturnNoContent_WhenParticipanteExists()
        {
            // Arrange
            int participanteId = 10;
            // Act
            var result = await _Controller.Delete(participanteId);
            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockParticipanteRepository.Verify(r => r.Remove(It.Is<Participante>(p => p.Id == participanteId)), 
                Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]  

        public async Task Delete_ShouldReturnNotFound_WhenParticipanteDoesNotExist()
        {
            // Arrange
            int participanteId = 999; // ID que não existe
            _mockParticipanteRepository.Setup(r => r.GetByIdAsync(participanteId))
                .ReturnsAsync((Participante)null);
            // Act
            var result = await _Controller.Delete(participanteId);
            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockParticipanteRepository.Verify(r => r.Remove(It.IsAny<Participante>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
