using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Mappings;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.UnitOfWork;
using Moq;

namespace GerenciamentoTest.EventoUnitTest
{
    public class EventoUnitTestController
    {
        public Mock<IUnitOfWork> MockUnitOfWork { get; }
        public Mock<IEventoRepository> MockEventoRepository { get; }
        public IDTOMapper<EventoDTO, Evento, EventoPatchDTO> Mapper { get; }

        public EventoUnitTestController()
        {
            MockUnitOfWork = new Mock<IUnitOfWork>();
            MockEventoRepository = new Mock<IEventoRepository>();
            Mapper = new EventoMapper();

            // Conecta UnitOfWork com o repo mockado
            MockUnitOfWork.Setup(u => u.EventoRepository).Returns(MockEventoRepository.Object);
        }
    }
}
