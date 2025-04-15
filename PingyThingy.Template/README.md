# PingyThingy Secure API Template (NarebaCoder)

Este é um template `dotnet new` para criar rapidamente uma solução de API ASP.NET 9 robusta, baseada no projeto PingyThingy.

**Foco:** Segurança, Observabilidade e Boas Práticas.

## Funcionalidades Incluídas no Template

*   **Estrutura de Solução:** Projeto API (`.Api`) e Core (`.Core`).
*   **Segurança:**
    *   Autenticação JWT Bearer pré-configurada.
    *   Autorização básica.
    *   Rate Limiting (por usuário e global) configurável via `appsettings.json`.
    *   Validação de Entrada com FluentValidation (usando Filtro de Ação).
    *   Middleware de Cabeçalhos de Segurança.
    *   Política CORS configurável.
    *   Redirecionamento HTTPS e HSTS (não-dev).
*   **Observabilidade:**
    *   Integração com OpenTelemetry (Traces, Metrics, Logs via OTLP).
    *   Endpoint de Health Checks (`/healthz`).
    *   Tratamento de Erros com `ProblemDetails` e log de Correlation ID.
*   **Desenvolvimento:**
    *   Swagger/OpenAPI configurado com suporte a autenticação Bearer.
    *   Endpoint de geração de token JWT para desenvolvimento (`/dev/generate-token`).
    *   Configuração centralizada via `appsettings.json` e `ServiceCollectionExtensions.cs`.
    *   Dockerfile e `docker-compose.yml` básicos.

## Como Usar

1.  **Instale o Template:**
    ```powershell
    dotnet new install NarebaCoder.PingyThingy.Api.Template
    ```

2.  **Crie um Novo Projeto:**
    ```powershell
    dotnet new narebacoder-pingyapi -n SeuNovoNomeDeProjeto
    ```
    (Onde `narebacoder-pingyapi` é o nome curto do template e `SeuNovoNomeDeProjeto` é o nome desejado para sua nova API).

3.  **Configure:**
    *   Siga as instruções no `README.md` gerado dentro do seu novo projeto, especialmente sobre a configuração de User Secrets para a chave JWT em desenvolvimento.

---

*Este template foi criado por NarebaCoder.*
*Encontre o código fonte [aqui](https://github.com/eduardofaneli/pingythingy-api-template).*