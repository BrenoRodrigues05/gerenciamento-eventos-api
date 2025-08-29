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

namespace GerenciamentoTest.ParticipanteUnitTest
{
    public class PatchParticipanteTests
    {
        private readonly ParticipantesController _controller;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IParticipanteRepository> _mockRepo;
        private readonly Mock<IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO>> _mockMapper;
       
        public PatchParticipanteTests()
        {
            // Mocks
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IParticipanteRepository>();
            _mockMapper = new Mock<IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO>>();

            // Setup do UnitOfWork
            _mockUnitOfWork.Setup(u => u.Participantes).Returns(_mockRepo.Object);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Setup do Mapper
            _mockMapper.Setup(m => m.ToPatchDto(It.IsAny<Participante>()))
                       .Returns<Participante>(p => new ParticipantePatchDTO
                       {
                           Nome = p.Nome,
                           Email = p.Email,
                           Telefone = p.Telefone
                       });
            _mockMapper.Setup(m => m.PatchToEntity(It.IsAny<ParticipantePatchDTO>(), It.IsAny<Participante>()))
                       .Callback<ParticipantePatchDTO, Participante>((dto, entity) =>
                       {
                           if (dto.Nome != null) entity.Nome = dto.Nome;
                           if (dto.Email != null) entity.Email = dto.Email;
                           if (dto.Telefone != null) entity.Telefone = dto.Telefone;
                       });

            var fakeCache = new FakeParticipantesCacheService(_mockUnitOfWork.Object, _mockMapper.Object);

            // Controller
            _controller = new ParticipantesController(
                _mockUnitOfWork.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ParticipantesController>>(),
                _mockMapper.Object,
                fakeCache
            );

            // Necessário para TryValidateModel funcionar
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            var mockValidator = new Mock<IObjectModelValidator>();
            _controller.ObjectValidator = mockValidator.Object;
        }

        [Fact]
        public async Task Patch_ShouldReturnBadRequest_WhenPatchDocIsNull()
        {
            JsonPatchDocument<ParticipantePatchDTO> patchDoc = null;
            var result = await _controller.Patch(1, patchDoc);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Patch_ShouldReturnNotFound_WhenParticipanteDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Participante)null);
            var patchDoc = new JsonPatchDocument<ParticipantePatchDTO>();
            var result = await _controller.Patch(1, patchDoc);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Patch_ShouldReturnNoContent_WhenPatchIsAppliedSuccessfully()
        {
            var participante = new Participante
            {
                Id = 1,
                Nome = "Participante Teste",
                Email = "teste@email.com",
                Telefone = "123456789"
            };
            _mockRepo.Setup(r => r.GetByIdAsync(participante.Id)).ReturnsAsync(participante);

            var patchDoc = new JsonPatchDocument<ParticipantePatchDTO>();
            patchDoc.Replace(p => p.Nome, "Participante Atualizado");
            patchDoc.Replace(p => p.Email, "novo@email.com");

            var result = await _controller.Patch(participante.Id, patchDoc);

            result.Should().BeOfType<NoContentResult>();
            participante.Nome.Should().Be("Participante Atualizado");
            participante.Email.Should().Be("novo@email.com");

            _mockRepo.Verify(r => r.Update(participante), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
