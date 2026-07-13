# 🚀 FcgUsers API

Serviço core de gerenciamento de usuários da plataforma **FCG Games**. Este microsserviço é responsável pelo ciclo de vida completo do usuário, incluindo registro, autenticação, controle de permissões (RBAC) e propagação assíncrona de eventos de domínio.

---

## 📑 Sumário

- [👁 Visão Geral](#-visão-geral)
- [📋 Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [🏛 Arquitetura](#-arquitetura)
- [📁 Estrutura da Solução](#-estrutura-da-solução)
- [🔄 Fluxo de Eventos (Mensageria)](#-fluxo-de-eventos-mensageria)
- [⚙️ Pré-requisitos](#️-pré-requisitos)
- [▶️ Executando Localmente](#️-executando-localmente)
- [🗄 Banco de Dados](#-banco-de-dados)
- [🔐 Autenticação JWT](#-autenticação-jwt)
- [🧪 Testes](#-testes)
- [🌍 Variáveis de Ambiente](#-variáveis-de-ambiente)

---

## 👁 Visão Geral

O **FcgUsers API** provê a identidade e gestão de perfis dentro do ecossistema FCG Games.

A aplicação utiliza o padrão **CQRS** com **MediatR** para desacoplar comandos de escrita de consultas de leitura, promovendo maior organização, escalabilidade e facilidade de manutenção.

Além disso, adota **DDD (Domain-Driven Design)** e **Clean Architecture**, separando claramente regras de negócio, infraestrutura e apresentação.

---

## 📋 Tecnologias Utilizadas

| Categoria | Tecnologia |
| :--- | :--- |
| **Runtime** | .NET 10 |
| **API** | ASP.NET Core Minimal API |
| **Orquestração** | .NET Aspire (Local e Cloud Ready) |
| **Mensageria** | MassTransit + RabbitMQ |
| **Persistência** | Entity Framework Core + PostgreSQL |
| **Mediação** | MediatR + CQRS + OperationResult |
| **Validação** | FluentValidation (via `ValidationBehavior`) |
| **Segurança** | JWT Bearer + BCrypt |
| **Testes** | xUnit, Shouldly, Respawn e Testcontainers |

---

## 🏛 Arquitetura

O projeto segue os princípios de **Clean Architecture** e **DDD**, organizando o código em camadas bem definidas.

| Camada | Responsabilidade |
| :--- | :--- |
| `FcgUsers.Api` | Endpoints, Middlewares e Filtros de Validação |
| `FcgUsers.Application` | Casos de Uso, Commands, Queries, Handlers e Mappers |
| `FcgUsers.Domain` | Agregados (`User`), Value Objects (`Email`, `Password`), Interfaces e Regras de Negócio |
| `FcgUsers.Infrastructure` | Persistência, EF Core, Migrations e Mensageria |
| `FcgUsers.IoC` | Registro de Dependências |
| `FcgUsers.ServiceDefaults` | Configurações compartilhadas do Aspire (Health Checks, OTEL etc.) |
| `FcgUsers.SharedKernel` | Componentes reutilizáveis, Behaviors, Validators e Configurações |

---

## 📁 Estrutura da Solução

```text
src/
├── FcgUsers.Api              # Entry Point da API
├── FcgUsers.AppHost          # Orquestração com .NET Aspire
├── FcgUsers.Application      # Casos de uso e Handlers
├── FcgUsers.Domain           # Entidades e regras de negócio
├── FcgUsers.Infrastructure   # Persistência e Mensageria
├── FcgUsers.IoC              # Injeção de Dependências
├── FcgUsers.ServiceDefaults  # Configurações compartilhadas do Aspire
└── FcgUsers.SharedKernel     # Código compartilhado

tests/
├── FcgUsers.UnitTests         # Testes de domínio
└── FcgUsers.IntegrationTests  # Testes de integração
```

---

## 🔄 Fluxo de Eventos (Mensageria)

A API atua como um **publicador de eventos de domínio**, permitindo que outros microsserviços reajam às alterações realizadas nos usuários.

### Evento publicado

- `UserCreatedEvent`

### Broker

- RabbitMQ

### Framework

- MassTransit

### Resiliência

A mensageria utiliza **Retry Policy exponencial**, configurada através de `MassTransitSettings`, garantindo reprocessamento automático em caso de falhas temporárias.

---

## ⚙️ Pré-requisitos

Antes de executar a aplicação, instale:

- .NET SDK 10.0 ou superior
- Docker Desktop
- Workload do Aspire

Instalação do Aspire:

```bash
dotnet workload install aspire
```

---

## ▶️ Executando Localmente

O ambiente completo é provisionado automaticamente pelo **.NET Aspire**.

Execute:

```bash
dotnet run --project src/FcgUsers.AppHost
```

Após iniciar a aplicação, o **Aspire Dashboard** será aberto automaticamente, exibindo:

- Logs
- Métricas
- Status dos containers
  - PostgreSQL
  - RabbitMQ

---

## 🗄 Banco de Dados

A persistência utiliza **Entity Framework Core** com PostgreSQL.

A conexão possui estratégia de **Retry on Failure**, aumentando a resiliência em falhas temporárias.

### Criar uma Migration

```bash
dotnet ef migrations add NomeDaMigration -p src/FcgUsers.Infrastructure -s src/FcgUsers.Api
```

---

## 🔐 Autenticação JWT

A API utiliza autenticação baseada em **JWT Bearer**.

As configurações são validadas em tempo de inicialização através do método `ValidateOnStart()`.

### Validações

- Issuer
- SecurityKey

### Política

- `AdminPolicy` — Restringe endpoints administrativos.

### Header

```http
Authorization: Bearer {TOKEN}
```

---

## 🧪 Testes

### Testes Unitários

Responsáveis por validar a lógica das entidades de domínio:

- User
- Password
- Email

Execute:

```bash
dotnet test tests/FcgUsers.UnitTests
```

---

### Testes de Integração

Os testes utilizam:

- IntegrationTestFixture
- Respawn
- Testcontainers

Antes da execução de cada teste, o banco é restaurado automaticamente utilizando **Respawn**, garantindo isolamento total entre os cenários.

Execute:

```bash
dotnet test tests/FcgUsers.IntegrationTests
```

---

## 🌍 Variáveis de Ambiente (Aspire Managed)

| Variável | Descrição |
| :--- | :--- |
| `ConnectionStrings:Default` | String de conexão com PostgreSQL |
| `ConnectionStrings:rabbitmq` | Endereço do RabbitMQ |
| `JwtSettings:SecurityKey` | Chave secreta utilizada para assinatura dos tokens JWT |
| `MassTransit:RetryLimit` | Quantidade máxima de tentativas de reprocessamento (Padrão: **5**) |

---

## 📄 Licença

Projeto desenvolvido para a plataforma **FCG Games**.
