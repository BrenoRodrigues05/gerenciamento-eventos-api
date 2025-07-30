Gerenciamento de Eventos API 

API REST desenvolvida em ASP.NET Core para gerenciamento de eventos, participantes e inscrições. Ideal para organização de eventos com controle de vagas, datas e participantes.

## 🔧 Tecnologias Utilizadas ##

- ASP.NET Core 8
- Entity Framework Core
- AutoMapper
- FluentValidation
- Repository + Unit of Work Pattern
- Swagger (Swashbuckle)
- SQL Server

## 🚀 Como Executar ##

### Pré-requisitos ##

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server (local ou container)
- (Opcional) Docker

1. Clone o repositório:

```bash
git clone https://github.com/BrenoRodrigues05/gerenciamento-eventos-api.git
cd gerenciamento-eventos-api/APIGerenciamento

2. Configure o appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GerenciamentoEventosDb;Trusted_Connection=True;"
}

3. Execute as migrations e rode a aplicação:

dotnet ef database update
dotnet run

4. Acesse via:

https://localhost:5001/swagger

📌 Funcionalidades

1- CRUD completo de eventos
2- Cadastro e listagem de participantes
3- Inscrição e cancelamento de inscrição em eventos
4- Paginação com filtros nos endpoints
5- Regras de negócio específicas implementadas em serviços

 Regras de Negócio:

1- Datas de início não podem ser posteriores à data final
2- Eventos possuem número máximo de vagas
3- Participantes não podem se inscrever duas vezes no mesmo evento

Autor : Breno Rodrigues 
