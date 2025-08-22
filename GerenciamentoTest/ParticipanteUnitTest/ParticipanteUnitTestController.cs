using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Mappings;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.UnitOfWork;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciamentoTest.ParticipanteUnitTest
{
    public class ParticipanteUnitTestController
    {
        public Mock<IUnitOfWork> MockUnitOfWork { get; }
        public Mock<IParticipanteRepository> MockParticipanteRepo { get; }

        public IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> Mapper { get; }

        public ParticipanteUnitTestController()
        {
            MockUnitOfWork = new Mock<IUnitOfWork>();
            MockParticipanteRepo = new Mock<IParticipanteRepository>();
            Mapper = new ParticipanteMapper();

            // Configuração do Mock para o repositório de participantes
            MockUnitOfWork.Setup(u => u.Participantes).Returns(MockParticipanteRepo.Object);
        }
    }
}
