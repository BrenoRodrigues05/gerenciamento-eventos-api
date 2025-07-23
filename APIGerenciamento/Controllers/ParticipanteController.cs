using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGerenciamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipanteController : GenericController<Participante>
    {
        public ParticipanteController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
