# Demo: uma tarefa reutilizável com prompt files

Este repositório demonstra como transformar uma solicitação recorrente em um
**prompt file** executável no VS Code. A aplicação Training Catalog, suas
especificações e demais customizações fornecem um contexto real para criar ou
ajustar um único endpoint sem copiar todas as regras para o prompt.

O exemplo está em
[`.github/prompts/criar-endpoint-treinamento.prompt.md`](.github/prompts/criar-endpoint-treinamento.prompt.md).
Arquivos `*.prompt.md` são templates Markdown invocados explicitamente no chat,
normalmente por um comando `/`. Eles podem receber entradas e selecionar o
agente, o modelo e as tools adequadas para uma tarefa focada.

## Quando usar cada customização

| Necessidade | Use | Como entra em ação |
| --- | --- | --- |
| Repetir uma tarefa focada com parâmetros diferentes | **Prompt file** | Invocação explícita pelo usuário |
| Aplicar padrões e restrições em todo pedido ou em arquivos específicos | **Custom instructions** | Aplicação automática conforme o escopo, ou anexo manual |
| Disponibilizar um procedimento especializado com scripts ou recursos auxiliares | **Skill** | O agente carrega quando identifica que a tarefa é relevante |
| Manter uma persona, fluxo e conjunto de capacidades especializados | **Custom agent** | Seleção explícita do agente ou referência por outra customização |

Regra prática: instructions dizem continuamente **como trabalhar neste
contexto**; prompt files empacotam **qual tarefa executar agora**; skills
oferecem **como realizar um procedimento especializado sob demanda**. Neste
repositório, os mecanismos são complementares, não alternativas mutuamente
exclusivas.

## Estrutura do prompt

O front matter controla a execução:

| Campo | Uso nesta demo |
| --- | --- |
| `name` | Define `/endpoint-treinamento`, nome curto usado para invocar o prompt |
| `description` | Resume a finalidade exibida na interface |
| `argument-hint` | Orienta o preenchimento da solicitação |
| `agent` | Seleciona o agente local `agent`, capaz de implementar a mudança |
| `model` | Seleciona `Claude Opus 4.8 (fast mode) (Preview) (copilot)` como exemplo explícito; a disponibilidade depende do plano e das políticas da organização |
| `tools` | Limita as capacidades a `search`, `edit` e `execute` para inspecionar, alterar e validar |

> [!CAUTION]
> **O que acontece se eu não tiver acesso a esse modelo?**
>
> O VS Code usará o modelo mais adequado disponível ou o que estiver selecionado.
> Este exemplo demonstra um caso de risco: o modelo especificado pode não estar visível para o usuário. O prompt não garante que o mesmo modelo será usado em todos os ambientes.


As tools declaradas no prompt têm prioridade sobre as tools do agente
referenciado. Sem `tools` no prompt, seriam usadas as tools do custom agent e,
na ausência delas, as tools padrão do agente selecionado. Se uma tool declarada
não estiver disponível no ambiente, o VS Code a ignora.

O corpo usa quatro variáveis `${input:...}`:

- `operation`: método HTTP;
- `route`: rota do endpoint;
- `contract`: entrada, sucesso, erros e regras de negócio;
- `validation`: cenários e comandos que produzirão evidências.

As regras duráveis não são repetidas integralmente. O prompt referencia
explicitamente apenas a
[especificação do catálogo](docs/specs/training-catalog-vertical-slice.md),
fonte do contrato aprovado. As
[instructions gerais](.github/copilot-instructions.md), as
[instructions da API](.github/instructions/api.instructions.md), as
[instructions dos testes](.github/instructions/tests.instructions.md) e o
[`AGENTS.md`](AGENTS.md) são descobertos e aplicados pelo VS Code conforme seus
escopos. Permanecem no prompt apenas o fluxo e os limites específicos da
tarefa.

## Executar a demo no VS Code

Abra a raiz deste repositório em uma versão atualizada do VS Code com GitHub
Copilot. Inicie um chat novo para evitar influência de conversas anteriores.
Se `Claude Sonnet 4.5` não estiver disponível na assinatura do participante,
substitua o valor de `model` por um modelo exibido no picker ou remova o campo
para usar o modelo atualmente selecionado, inclusive `Auto`.
Execute o prompt por uma destas opções:

1. Digite `/endpoint-treinamento` no chat e selecione o comando.
2. Execute **Chat: Run Prompt** na Command Palette e selecione o arquivo.
3. Abra o arquivo `.prompt.md` e use o botão de execução no editor.

Quando solicitado, use estas entradas:

| Entrada | Valor copiável |
| --- | --- |
| Operação | `GET` |
| Rota | `/api/trainings/count` |
| Contrato | `Retornar HTTP 200 com um corpo JSON contendo a quantidade de treinamentos cadastrados, sem alterar os contratos existentes.` |
| Validação | `Adicionar um teste funcional focado e executar o menor comando dotnet test que cubra o comportamento.` |

Esse endpoint adicional é compatível com a especificação atual porque possui
contrato explícito e não altera comportamentos já aprovados.

## O que observar

Antes de aprovar edições ou comandos, revise a proposta do agente. Durante e
após a execução:

1. Expanda as referências do chat e confirme a especificação, o `AGENTS.md` e
   as instructions aplicáveis. Os marcadores `GERAL:`, `API:` e `TESTES:`
   ajudam na demonstração, mas as referências carregadas são a evidência mais
   confiável.
2. Expanda as chamadas de tools e observe o uso restrito de busca, edição e
   terminal.
3. Revise o diff no **Source Control** e confirme que somente o endpoint e seu
   teste focado foram alterados.
4. Confira o comando e o resultado dos testes apresentados pelo agente.

O prompt orienta o agente, mas não garante resultados idênticos entre
execuções. A seleção explícita de modelo também não torna a saída
determinística. A revisão das mudanças, das permissões de tools e das
evidências continua sendo responsabilidade humana.

Para repetir a demonstração, descarte apenas as alterações geradas na execução
pelo painel **Source Control** e inicie um novo chat. Não descarte as
customizações versionadas que compõem esta demo.

## Limitações

Prompt files funcionam com agentes locais executados pela extensão do VS Code.
Agentes executados no **Agent Host** não os utilizam; para esse ambiente, a
documentação recomenda converter o fluxo em uma skill. O suporte varia entre
IDE, GitHub.com e Copilot CLI, portanto confirme a
[matriz vigente do GitHub Copilot](https://docs.github.com/en/copilot/reference/customization-cheat-sheet).
Modelos e tools também podem variar conforme versão, plano, extensões
instaladas e políticas da organização.

## Referências

- [Prompt files no VS Code](https://code.visualstudio.com/docs/agent-customization/prompt-files)
- [Custom instructions no VS Code](https://code.visualstudio.com/docs/agent-customization/custom-instructions)
- [Tools de agentes no VS Code](https://code.visualstudio.com/docs/agents/run/tools)
- [Cheat sheet de customizações do GitHub Copilot](https://docs.github.com/en/copilot/reference/customization-cheat-sheet)
