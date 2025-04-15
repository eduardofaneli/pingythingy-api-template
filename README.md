# Código Fonte do Template PingyThingy Secure API (NarebaCoder)

Este repositório contém o código-fonte para o template `dotnet new` **NarebaCoder.PingyThingy.Api.Template**.

O template visa fornecer uma base sólida e segura para a criação de APIs .NET 9, incorporando boas práticas de segurança e observabilidade.

## Pacote NuGet

O template compilado e pronto para uso está disponível como um pacote NuGet:

[![NuGet version](https://img.shields.io/nuget/v/NarebaCoder.PingyThingy.Api.Template.svg)](https://www.nuget.org/packages/NarebaCoder.PingyThingy.Api.Template)

**Link:** [https://www.nuget.org/packages/NarebaCoder.PingyThingy.Api.Template](https://www.nuget.org/packages/NarebaCoder.PingyThingy.Api.Template)

## Como Usar o Template

1.  **Instale o Template:**
    ```powershell
    dotnet new install NarebaCoder.PingyThingy.Api.Template
    ```

2.  **Crie um Novo Projeto:**
    ```powershell
    dotnet new narebacoder-pingyapi -n SeuNovoNomeDeProjeto
    ```
    (Substitua `SeuNovoNomeDeProjeto` pelo nome desejado para sua API).

3.  **Siga as Instruções:** Consulte o `README.md` gerado dentro do seu novo projeto para configurações adicionais (como User Secrets).

## Estrutura do Repositório

*   `./PingyThingy.Template/`: Contém o projeto `.csproj` para empacotar o template como um pacote NuGet.
    *   `./PingyThingy.Template/README.md`: O README específico que é exibido na página do pacote NuGet.
    *   `./PingyThingy.Template/content/`: Contém todo o código-fonte real que será incluído no template (a solução .NET, Dockerfiles, etc.).
*   `./README.md`: Este arquivo, explicando o propósito do repositório.
*   `.gitignore`: Arquivo padrão para ignorar arquivos desnecessários do Git.

## Construindo o Template a Partir do Código Fonte

Se você clonou este repositório e deseja construir e instalar o pacote `.nupkg` localmente:

1.  Navegue até a pasta `PingyThingy.Template`:
    ```powershell
    cd PingyThingy.Template
    ```
2.  Execute o comando `dotnet pack`:
    ```powershell
    dotnet pack --configuration Release
    ```
    Isso gerará o arquivo `.nupkg` dentro de `bin/Release`.
3.  Instale o pacote localmente:
    ```powershell
    dotnet new install .\bin\Release\NarebaCoder.PingyThingy.Api.Template.<versão>.nupkg
    ```
    (Substitua `<versão>` pela versão atual definida no `.csproj`).

## Contribuições

Contribuições são bem-vindas! Sinta-se à vontade para abrir Issues ou Pull Requests.
