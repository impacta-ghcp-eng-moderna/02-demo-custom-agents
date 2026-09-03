# Training Catalog

O código da solução fica em `src`:

- `Api` hospeda os endpoints ASP.NET Core.
- `Application` contém os contratos do domínio.
- `Infrastructure` contém Entity Framework Core e SQLite.
- `Client` contém a interface Blazor WebAssembly.
- `Tests/Api.Tests` contém os testes funcionais.

Use `dotnet build src/TrainingCatalog.slnx` para compilar e
`dotnet test src/TrainingCatalog.slnx` para executar os testes.

Não altere arquivos em `src/Infrastructure/Migrations` nem o banco
`src/Api/training-catalog.db`, exceto quando a tarefa exigir explicitamente
uma mudança de esquema.
