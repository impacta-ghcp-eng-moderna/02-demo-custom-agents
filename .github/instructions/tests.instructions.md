---
name: Testes funcionais da API
description: Convenções dos testes xUnit que exercitam a API por HTTP.
applyTo: "src/Tests/**/*.cs"
---

# Convenções dos testes

- Escreva testes funcionais com xUnit e `TrainingCatalogApiFactory`.
- Exercite os endpoints por meio do `HttpClient`; não chame handlers nem o `DbContext` diretamente.
- Crie uma factory e um client por teste para manter o banco SQLite isolado.
- Nomeie os testes em inglês pelo comportamento observável, começando com `Returns`.
- Verifique primeiro o status HTTP e depois o corpo relevante da resposta.
- Ao explicar uma alteração que usa estas regras, inclua uma linha iniciada por `TESTES:`.
