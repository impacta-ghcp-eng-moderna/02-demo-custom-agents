---
name: API do Training Catalog
description: Convenções dos endpoints ASP.NET Core Minimal API e de seus contratos HTTP.
applyTo: "src/Api/**/*.cs"
---

# Convenções da API

- Mantenha os endpoints como Minimal APIs em `src/Api/Program.cs`.
- Preserve o prefixo de rota `/api/trainings`.
- Retorne erros de validação no formato `{ errors: { campo: string[] } }`, com mensagens em português do Brasil.
- Declare os status possíveis de cada endpoint com chamadas `.Produces`.
- Use as APIs assíncronas do Entity Framework Core e `AsNoTracking` em consultas somente leitura.
- Ao explicar uma alteração que usa estas regras, inclua uma linha iniciada por `API:`.
