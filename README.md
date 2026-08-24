# FCG.CatalogAPI

Microsserviço de **catálogo de jogos** da plataforma FIAP Cloud Games (FCG).

Responsável pelo CRUD de jogos, aplicação de descontos e aquisição de jogos pelos usuários. Ao adquirir um jogo, publica o evento `OrderPlacedEvent` no RabbitMQ para o serviço de pagamentos. Também consome o `UserCreatedEvent` do serviço de usuários.

## Arquitetura

Clean Architecture / DDD em 4 camadas:

```
src/
├── FCG.CatalogAPI.Domain          # Entidades (Game, UserGame), interfaces
├── FCG.CatalogAPI.Application     # Commands/Queries (MediatR), eventos, consumers
├── FCG.CatalogAPI.Infrastructure  # EF Core, repositórios
└── FCG.CatalogAPI.API             # Controllers, configuração, Swagger
```

- **.NET 8** / ASP.NET Core
- **EF Core 8** + SQL Server
- **MassTransit** + RabbitMQ (mensageria)
- **MediatR** (CQRS)
- **JWT Bearer** (valida tokens emitidos pelo FCG.UsersAPI)

## Endpoints

| Método | Rota | Autorização | Descrição |
|---|---|---|---|
| GET | `/api/games` | Pública | Lista jogos ativos |
| GET | `/api/games/{id}` | Pública | Detalha um jogo |
| POST | `/api/games` | Admin | Cadastra jogo |
| PUT | `/api/games/{id}` | Admin | Atualiza jogo |
| DELETE | `/api/games/{id}` | Admin | Desativa jogo |
| PATCH | `/api/games/{id}/discount` | Admin | Aplica desconto (0–100%) |
| POST | `/api/games/{id}/acquire` | Autenticado | Adquire jogo e publica `OrderPlacedEvent` |

## Eventos

| Evento | Direção | Descrição |
|---|---|---|
| `UserCreatedEvent` | Consome | Novo usuário registrado (fila `catalog-user-created`) |
| `OrderPlacedEvent` | Publica | Pedido criado na aquisição de um jogo (OrderId, UserId, GameId, Amount) |

## Variáveis de ambiente

| Variável | Descrição | Exemplo |
|---|---|---|
| `ConnectionStrings__Default` | Connection string do SQL Server | `Server=localhost\SQLEXPRESS;Database=FCG_CatalogDB;Trusted_Connection=True;TrustServerCertificate=True` |
| `Jwt__Secret` | Mesma chave do FCG.UsersAPI | — |
| `Jwt__Issuer` | `FCG.UsersAPI` | — |
| `Jwt__Audience` | `FCG` | — |
| `RabbitMQ__Host` | Host do RabbitMQ | `localhost` |
| `RabbitMQ__Username` | Usuário do RabbitMQ | `guest` |
| `RabbitMQ__Password` | Senha do RabbitMQ | `guest` |

## Como executar

### Local

Pré-requisitos: .NET 8 SDK, SQL Server, RabbitMQ.

```bash
dotnet run --project src/FCG.CatalogAPI.API
```

As migrations são aplicadas automaticamente na inicialização.

### Docker

```bash
docker build -t fcg-catalogapi .
docker run -p 5002:8080 \
  -e ConnectionStrings__Default="..." \
  -e Jwt__Secret="..." \
  -e RabbitMQ__Host="rabbitmq" \
  fcg-catalogapi
```

### Kubernetes

```bash
kubectl apply -f k8s/
```

Os manifests incluem Deployment, Service, ConfigMap e Secret.

## Testes

Os testes unitários (xUnit + NSubstitute) estão na branch `feature/testes-unitarios`:

```bash
dotnet test
```
