# JobMinerSolution

## Fontes

- Consulte o Atlas no projeto `jobminer` e confirme o estado no Git.
- Arquitetura atual: `JobMiner5.sln` e projetos em `src/`.
- Fluxo: `docs/adr/0001-adocao-policy-v4.md`.

## Projeto

- Trunk: `master`.
- Stack: .NET 8 e WinForms.
- `JobMiner.sln` está vazia e não é gate.
- Não altere contratos entre Core, Data, Matching e Providers sem atualizar todos os consumidores.
- Vagas, currículos, e-mails e dados de candidatos são PII; use fixtures sintéticas e logs redigidos.

## Gate

```powershell
dotnet build JobMiner5.sln --configuration Release
```

Não há suíte nem CI confiável documentada. Mudança visual exige smoke manual do WinForms e registro dessa limitação.

Preserve WIP. Uma tarefa usa issue, branch e PR. Só conclua após build, smoke aplicável, review, mergeabilidade, merge, limpeza e atualização do Atlas.
