# PingyThingy API

API para receber e processar webhooks de forma segura e observável.

## Visão Geral

Esta API .NET 9 foi construída com foco em segurança, resiliência e observabilidade. Ela serve como ponto de entrada para webhooks externos, validando-os, aplicando políticas de segurança e preparando-os para processamento pela lógica de negócio principal (a ser implementada no projeto `PingyThingy.Core`).

## Funcionalidades Implementadas

*   **Autenticação:** Proteção via JWT Bearer Token.
*   **Autorização:** Controle de acesso aos endpoints.
*   **Rate Limiting:** Limites de requisição configuráveis (por usuário e global) para prevenir abuso.
*   **Validação de Entrada:** Validação automática de payloads usando FluentValidation.
*   **Cabeçalhos de Segurança:** Adição automática de cabeçalhos HTTP para mitigar vulnerabilidades comuns.
*   **CORS:** Política configurável para permitir acesso de frontends específicos.
*   **Observabilidade:**
    *   Integração com OpenTelemetry para Traces, Metrics e Logs (exportação via OTLP).
    *   Endpoint de Health Check (`/healthz`).
    *   Uso de Correlation IDs (TraceId) para rastreamento.
*   **Tratamento de Erros:** Handler global para capturar exceções não tratadas e retornar respostas `ProblemDetails` padronizadas e seguras.
*   **Configuração Centralizada:** Uso de `appsettings.json` para configurações chave (JWT, Rate Limiting, CORS).
*   **Documentação API:** Geração automática de documentação via Swagger/OpenAPI.
*   **Endpoint de Desenvolvimento:** Geração de token JWT para testes locais (`/dev/generate-token`, apenas em Debug).

## Começando

### Pré-requisitos

*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) ou superior.
*   (Opcional) Docker e Docker Compose para execução em contêiner.
*   (Opcional) Um backend de observabilidade compatível com OTLP (ex: Jaeger, Prometheus, SigNoz, Datadog) para visualizar telemetria.

### Configuração

1.  **Clonar o Repositório:**
    ```bash
    git clone <url-do-repositorio>
    cd PingyThingy
    ```

2.  **Configurar Segredos (Desenvolvimento):**
    A API usa JWT para autenticação. Para desenvolvimento, configure a chave secreta usando User Secrets:
    ```powershell
    # Navegue até o diretório src/PingyThingy.Api
    cd src/PingyThingy.Api

    # Inicialize User Secrets (se ainda não foi feito)
    dotnet user-secrets init

    # Defina a chave secreta JWT (substitua por uma chave forte)
    dotnet user-secrets set "Jwt:Key" "SUA_CHAVE_SECRETA_FORTE_PARA_DESENVOLVIMENTO_AQUI_5a8f3b9e7d1c4a0b8e9f2a1b3c4d5e6f"

    # (Opcional) Configure Issuer e Audience se diferente dos padrões no appsettings.json
    # dotnet user-secrets set "Jwt:Issuer" "seu-issuer-dev"
    # dotnet user-secrets set "Jwt:Audience" "sua-audience-dev"

    cd ../.. # Voltar para a raiz do projeto
    ```
    *Nota: Para produção, a chave JWT **NÃO** deve estar no `appsettings.json` ou User Secrets. Use Variáveis de Ambiente ou um serviço como Azure Key Vault.*

3.  **Ajustar `appsettings.json`:**
    *   Revise as configurações em `src/PingyThingy.Api/appsettings.json`, especialmente:
        *   `Jwt:Issuer` e `Jwt:Audience`: Devem corresponder ao que seus tokens JWT usarão.
        *   `RateLimiting`: Ajuste os limites conforme necessário.
        *   `Cors:AllowedOrigins`: Adicione as URLs dos seus frontends que precisam acessar a API.
        *   `Logging`: Ajuste os níveis de log se necessário.
        *   `OTEL_EXPORTER_OTLP_ENDPOINT` (Variável de Ambiente ou configuração): Se estiver usando um backend de observabilidade, configure o endpoint OTLP aqui ou via variável de ambiente.

### Executando a API

*   **Via .NET CLI:**
    ```powershell
    # A partir da raiz do repositório
    dotnet run --project src/PingyThingy.Api/PingyThingy.Api.csproj
    ```
    A API estará disponível (por padrão) em `https://localhost:PORTA_HTTPS` e `http://localhost:PORTA_HTTP`. As portas são definidas em `src/PingyThingy.Api/Properties/launchSettings.json`.

*   **Via Docker Compose:**
    ```bash
    # A partir da raiz do repositório
    docker-compose up --build
    ```
    (Verifique o `docker-compose.yml` para as portas expostas).

## Endpoints da API

Após iniciar a API, acesse a interface do Swagger para ver a lista completa de endpoints e interagir com eles:

*   **Swagger UI:** `https://localhost:PORTA_HTTPS/swagger`

Endpoints principais:

*   `POST /api/webhooks`: Recebe o payload do webhook (requer autenticação JWT).
*   `GET /healthz`: Verifica a saúde da aplicação.
*   `GET /dev/generate-token`: (Apenas em Debug) Gera um token JWT para teste.

## Segurança

*   **Autenticação:** Use um token JWT válido no cabeçalho `Authorization: Bearer <token>`.
*   **Rate Limiting:** Esteja ciente dos limites de requisição configurados. Respostas `429 Too Many Requests` indicarão que o limite foi excedido.
*   **Validação:** Payloads enviados para `/api/webhooks` são validados. Respostas `400 Bad Request` com `ProblemDetails` indicarão erros de validação.
*   **Cabeçalhos:** A API adiciona cabeçalhos de segurança para proteção adicional.
*   **CORS:** Apenas origens configuradas em `Cors:AllowedOrigins` podem fazer requisições via browser.

## Observabilidade

*   **Traces, Metrics, Logs:** Exportados via OTLP para um backend configurado.
*   **Health Checks:** Disponível em `/healthz`.
*   **Correlation ID:** Incluído (`traceId`) nas respostas de erro para facilitar o rastreamento.

## Roadmap

Consulte o arquivo [roadmap.md](roadmap.md) para ver os próximos passos planejados para o desenvolvimento.
