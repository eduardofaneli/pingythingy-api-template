# PingyThingy API Roadmap

Este documento resume o progresso atual e os próximos passos planejados para o desenvolvimento da API PingyThingy.

## Concluído ✅

*   **Estrutura e Configuração:**
    *   Refatoração da configuração do `Program.cs` para `ServiceCollectionExtensions.cs`.
    *   Configurações (JWT, Rate Limiting, CORS) movidas para `appsettings.json`.
*   **Segurança:**
    *   Autenticação JWT Bearer.
    *   Autorização básica (`[Authorize]` no `WebhooksController`).
    *   Rate Limiting configurável (por usuário e global).
    *   Validação de Entrada com FluentValidation (`WebhookPayloadDto`).
    *   Middleware de Cabeçalhos de Segurança (`X-Content-Type-Options`, `X-Frame-Options`, etc.).
    *   Política CORS configurável.
    *   Redirecionamento HTTPS e HSTS (não-dev).
*   **Observabilidade:**
    *   Configuração base do OpenTelemetry (Traces, Metrics, Logs via OTLP).
    *   Endpoint de Health Checks (`/healthz`).
    *   Uso de Correlation ID (`TraceId`) nos logs de erro.
*   **Tratamento de Erros:**
    *   Middleware `UseExceptionHandler` direcionando para `ErrorsController`.
    *   `ErrorsController` para log detalhado e resposta `ProblemDetails` padronizada.
*   **Desenvolvimento:**
    *   Endpoint de geração de token JWT (`/dev/generate-token` no `DevelopmentController`, apenas em Debug).
    *   Configuração do Swagger/OpenAPI.
    *   `UseDeveloperExceptionPage` para depuração em desenvolvimento.
*   **Organização:**
    *   Endpoints de erro e desenvolvimento movidos para Controllers dedicados (`ErrorsController`, `DevelopmentController`).

## Roadmap 🚀

1.  **Implementar Lógica Principal (`PingyThingy.Core`):**
    *   [ ] Definir interfaces e classes no Core.
    *   [ ] Implementar processamento do `WebhookPayloadDto` (validação de assinatura, transformação, persistência, enfileiramento, etc.).
    *   [ ] Integrar a lógica do Core com o `WebhooksController`.
    
2.  **Gerenciamento de Segredos:**
    *   [ ] Usar "User Secrets" para `Jwt:Key` em desenvolvimento.
    *   [ ] Configurar leitura de `Jwt:Key` de Variáveis de Ambiente ou Azure Key Vault para produção.
3.  **Escrever Testes:**
    *   [ ] Adicionar Testes Unitários para `PingyThingy.Core`.
    *   [ ] Adicionar Testes de Integração para a API (`WebApplicationFactory`).
4.  **Aprimorar Logging:**
    *   [ ] Verificar/Garantir logs estruturados (JSON).
    *   [ ] Assegurar inclusão de `TraceId`/`SpanId` em todos os logs.
    *   [ ] Adicionar logs contextuais na lógica de negócio.
5.  **Configurar Backend de Observabilidade:**
    *   [ ] Escolher e configurar uma stack (ex: Prometheus/Grafana/Tempo/Loki, SigNoz, Datadog).
    *   [ ] Apontar o OTLP Exporter da API para o backend.
    *   [ ] Criar dashboards e alertas para métricas chave.
6.  **Implementar Resiliência com Polly (Quando Necessário):**
    *   [ ] Adicionar Polly a chamadas HTTP/DB no Core quando integrações externas forem implementadas.
7.  **Refinar Configuração de Deployment:**
    *   [ ] Otimizar `Dockerfile`.
    *   [ ] Configurar pipeline CI/CD.

*(Use `[x]` para marcar itens como concluídos)*
