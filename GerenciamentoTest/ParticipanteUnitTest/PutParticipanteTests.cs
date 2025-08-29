using APIGerenciamento.Controllers;
using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Mappings;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace GerenciamentoTest.ParticipanteUnitTest;

public class PutParticipanteTests
{
    private readonly ParticipantesController _Controller;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IParticipanteRepository> _mockParticipanteRepository;
    private readonly IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> _mapper;
    private readonly Mock<ILogger<ParticipantesController>> _mockLogger;
    private readonly ParticipanteCacheService _mockCacheService;
    public PutParticipanteTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockParticipanteRepository = new Mock<IParticipanteRepository>();
        _mockLogger = new Mock<ILogger<ParticipantesController>>();
        _mapper = new ParticipanteMapper();

        _mockUnitOfWork.Setup(u => u.Participantes)
            .Returns(_mockParticipanteRepository.Object);
        _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var fakeCache = new FakeParticipantesCacheService(_mockUnitOfWork.Object, _mapper);

        _Controller = new ParticipantesController(
            _mockUnitOfWork.Object,
            _mockLogger.Object,
            _mapper,
            fakeCache
        );
    }

    [Fact]

    public async Task Update_ShouldNoContent_WhenValidParticipante()
    {
        // Arrange
        var existingParticipante = new Participante
        {
            Id = 1,
            Nome = "Fulano de Tal",
            Email = "fulano@email.com",
            Telefone = "123456789"

        };

        var dto = new ParticipanteDTO
        {
            Nome = "Fulano de Tal 2",
            Email = "fulano2@email.com",
            Telefone = "987654321"
        };

        _mockParticipanteRepository.Setup(r => r.GetByIdAsync(existingParticipante.Id))
      .ReturnsAsync(existingParticipante);

        // Act
        var result = await _Controller.Update(existingParticipante.Id, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        _mockParticipanteRepository.Verify(r => r.Update(It.Is<Participante>(
            p => p.Id == existingParticipante.Id &&
                 p.Nome == dto.Nome &&
                 p.Email == dto.Email &&
                 p.Telefone == dto.Telefone
        )), Times.Once);

        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var dto = new ParticipanteDTO();
        _Controller.ModelState.AddModelError("Nome", "O nome é obrigatório");
        // Act
        var result = await _Controller.Update(1, dto);
        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

    }
    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenParticipanteDoesNotExist()
    {
        // Arrange
        var dto = new ParticipanteDTO
        {
            Nome = "Fulano de tal existente",
            Email = "fulanoexistente@teste.com",
            Telefone = "123456789"
        };

        var nonExistentId = 999; // ID que não existe no banco de dados

        _mockParticipanteRepository.Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Participante?)null);

        // Act

        var result = await _Controller.Update(nonExistentId, dto);

        // Assert

        result.Should().BeOfType<NotFoundResult>();
    }

}