# Demo: custom agents, subagents e handoffs

Este repositório demonstra um fluxo de revisão com capacidades limitadas sobre a
aplicação **Training Catalog**, usando um agente de revisão e um pesquisador
especializado.

O cenário usa o endpoint `GET /api/trainings/count`. O contrato exige resposta
`200 OK` com a quantidade de treinamentos tanto para catálogo vazio quanto
preenchido. A implementação cobre os dois casos, mas existe teste funcional
somente para o catálogo preenchido. Essa lacuna permite que o revisor diferencie:

- comportamento comprovadamente incorreto;
- critério atendido com evidência;
- critério compatível com a implementação, mas sem evidência suficiente.

Nenhum defeito foi introduzido de propósito. A lacuna é exclusivamente de
evidência automatizada e pode ser corrigida com um teste funcional pequeno.

## Custom agent, subagent e handoff

| Recurso | Papel nesta demonstração |
| --- | --- |
| **Custom agent** | Define uma persona persistente, suas instruções, modelo, tools e agentes permitidos. O `Revisor da entrega` coordena e julga a revisão. |
| **Subagent** | Executa uma investigação focada em contexto isolado e devolve um resumo ao coordenador. O `Pesquisador de critérios` relaciona especificação, implementação e testes. |
| **Handoff** | Muda para o agente implementador e preenche a próxima solicitação. Não é execução paralela nem isolada e mantém uma decisão humana entre revisão e correção. |

O isolamento do subagent reduz o volume de investigação no contexto principal,
mas cada chamada consome AI Credits e precisa receber tarefa e contexto
autossuficientes. O agente principal continua responsável por avaliar se as
evidências são suficientes.

## Arquivos relevantes

| Arquivo | Finalidade |
| --- | --- |
| [`.github/agents/revisor-entrega.md`](.github/agents/revisor-entrega.md) | Agente principal read-only que delega a pesquisa, executa validação focada e consolida o relatório. |
| [`.github/agents/pesquisador-criterios.md`](.github/agents/pesquisador-criterios.md) | Subagente oculto da seleção manual, limitado a leitura e busca. |
| [`docs/specs/training-catalog-vertical-slice.md`](docs/specs/training-catalog-vertical-slice.md) | Contrato e critérios de aceitação, incluindo a contagem do catálogo. |
| [`src/Api/Program.cs`](src/Api/Program.cs) | Implementação do endpoint de contagem. |
| [`src/Tests/Api.Tests/TrainingCountTests.cs`](src/Tests/Api.Tests/TrainingCountTests.cs) | Evidência do caso com catálogo preenchido; não cobre catálogo vazio. |

## Configuração dos agentes

### Revisor da entrega

O revisor aparece no seletor e não possui tools de criação, edição ou exclusão.

| Campo | Configuração e efeito |
| --- | --- |
| `name` | Nome exibido: `Revisor da entrega`. |
| `description` | Explica quando usar o agente e seu caráter read-only. |
| `argument-hint` | Orienta a entrada esperada no chat. |
| `target` | Restringe a definição ao VS Code. |
| `model` | Define um modelo preferencial; a disponibilidade depende do plano e das políticas. |
| `tools` | Disponibiliza `read`, `search`, `execute` e `agent`. |
| `agents` | Permite somente `Pesquisador de critérios` como subagent. |
| `user-invocable` | `true`, portanto o revisor aparece para seleção manual. |
| `disable-model-invocation` | `true`, evitando que outros agentes o escolham implicitamente como subagent. |
| `handoffs` | Oferece `Corrigir lacunas` para o agente implementador, com `send: false`. |

A tool `agent` é necessária para invocar subagents. `execute` permite somente
as validações definidas pelas instruções do revisor; ela não transforma o agente
em editor, mas comandos de terminal ainda exigem revisão humana. Um conjunto
read-only reduz o risco de alterações acidentais, porém não elimina riscos de
comandos, interpretação incorreta ou exposição indevida de dados.

### Pesquisador de critérios

O pesquisador tem uma única responsabilidade: extrair critérios e localizar
evidências. Ele não decide se a entrega deve ser aprovada.

| Campo | Configuração e efeito |
| --- | --- |
| `tools` | Somente `read` e `search`; não há terminal nem edição. |
| `agents` | Lista vazia, impedindo nova delegação. |
| `user-invocable` | `false`, ocultando o agente do seletor manual. |
| `disable-model-invocation` | `false`, permitindo que o coordenador autorizado o invoque. |

Cada invocação é independente. Por isso, o revisor deve informar a alteração,
a especificação aplicável, os caminhos relevantes e o formato esperado.

## Executar a demonstração no VS Code

Use uma versão atualizada do VS Code com GitHub Copilot e abra a raiz do
repositório em Codespaces.

1. Execute **Chat: Open Customizations** na Command Palette.
2. Abra **Agents** e inspecione os dois arquivos em `.github/agents`.
3. Confirme que `Revisor da entrega` aparece no seletor de agentes.
4. Confirme que `Pesquisador de critérios` não aparece para seleção manual.
5. Selecione `Revisor da entrega` e envie:

> Revise a implementação de `GET /api/trainings/count` contra a especificação.
> Delegue o levantamento dos critérios e das evidências ao subagente permitido.
> Execute somente a validação focada necessária e não altere arquivos.

Quando o revisor solicitar a tool de terminal, confira o comando na confirmação
nativa do VS Code e autorize apenas o teste focado que deseja observar. O agente
não deve abrir uma etapa conversacional separada apenas para pedir aprovação,
pois isso faria o handoff aparecer antes do relatório final.

## O que observar

1. Expanda a chamada do subagent e confira o nome `Pesquisador de critérios`.
2. Inspecione o prompt recebido pelo subagent e confirme que contém contexto
   suficiente sem depender de todo o histórico do chat principal.
3. Confira que as únicas tools do pesquisador são leitura e busca.
4. Observe o resumo relacionando especificação, endpoint e teste funcional.
5. Confirme que o revisor faz seu próprio julgamento depois da delegação.
6. Inspecione o comando de teste focado e seu resultado.
7. Verifique no painel **Source Control** que a revisão não alterou arquivos.
8. Ao fim da resposta, localize o handoff **Corrigir lacunas**.
9. Selecione o handoff e confirme que o agente muda e o prompt é preenchido,
   mas não enviado automaticamente.

As chamadas de subagents aparecem como seções expansíveis. A apresentação, os
rótulos e o nível de detalhe podem variar entre versões do VS Code.

## Interpretar o relatório

O revisor organiza cada critério em:

1. **Atendido**: implementação e evidência reproduzível sustentam o critério.
2. **Não atendido**: há evidência de comportamento contrário ao contrato.
3. **Não foi possível comprovar**: não há evidência suficiente para concluir,
   mesmo que a implementação pareça compatível.

Para esta entrega, o teste com catálogo preenchido deve sustentar a contagem
positiva. O caso de catálogo vazio exige teste funcional segundo a especificação,
mas não está coberto; portanto, a ausência não deve ser relatada automaticamente
como defeito do endpoint.

## Inspecionar tools e restrições

No chat, use **Configure Tools** para visualizar as tools ativas. No editor de
customizações, compare o front matter dos agentes:

- o revisor tem `execute` e `agent`, mas não tem edição;
- o pesquisador tem somente `read` e `search`;
- `agents` limita o revisor ao pesquisador;
- `agents: []` impede subagents aninhados no pesquisador.

Se uma tool declarada não existir na versão instalada, o VS Code pode ignorá-la.
Confirme os nomes disponíveis no picker antes de apresentar a demonstração.

## Handoff para correção

O handoff **Corrigir lacunas** aponta para o agente implementador local e limita
o prompt aos itens classificados como **Não atendido** ou
**Não foi possível comprovar**. Ele também exige preservar contratos e executar
os testes indicados.

`send: false` é intencional: selecionar o botão apenas prepara a próxima etapa.
O usuário pode revisar, ajustar ou cancelar o prompt antes de autorizar qualquer
edição. Isso distingue uma transição guiada de uma delegação isolada.

## Repetir ou restaurar o estado

A revisão read-only não deve produzir mudanças. Para repeti-la, abra um chat
novo, selecione o revisor e envie novamente a solicitação copiável.

Se o handoff for enviado e gerar alterações, revise o diff no painel
**Source Control** e descarte somente os arquivos produzidos nessa execução.
Não descarte os agentes, a especificação, o endpoint nem o teste preparado que
compõem esta demonstração.

## Validação do repositório

Os comandos documentados são:

```text
dotnet build src/TrainingCatalog.slnx
dotnet test src/TrainingCatalog.slnx
```

Para executar apenas a evidência preparada para contagem:

```text
dotnet test src/Tests/Api.Tests/TrainingCatalog.Api.Tests.csproj --filter FullyQualifiedName~TrainingCountTests
```

## Limitações

- Subagents consomem AI Credits adicionais; o custo depende do modelo e do plano.
- Modelo, tools, campos de front matter e interface variam conforme versão,
  assinatura, extensões instaladas e políticas da organização.
- O modelo principal decide como usar tools; instruções explícitas tornam a
  delegação mais consistente, mas não garantem saídas idênticas.
- Restrições de tools implementam menor privilégio, mas não substituem revisão
  humana de comandos, evidências e conclusões.
- Handoffs são uma conveniência de fluxo, não uma aprovação automática.
- A demonstração foi preparada para VS Code; outras superfícies têm matrizes de
  suporte diferentes.

## Referências

Documentação consultada em **3 de setembro de 2026**:

- [Custom agents no VS Code](https://code.visualstudio.com/docs/agent-customization/custom-agents)
- [Subagents no VS Code](https://code.visualstudio.com/docs/agents/run/subagents)
- [Tools de agentes no VS Code](https://code.visualstudio.com/docs/agents/run/tools)
- [Cheat sheet de customizações do GitHub Copilot](https://docs.github.com/en/copilot/reference/customization-cheat-sheet)
