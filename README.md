# Saggezza.Microservices

Monorepo de microserviços em **.NET 8** usando **ASP.NET Core**, **Entity Framework Core** e **SQL Server**, com autenticação **JWT** e infraestrutura para execução via **Docker Compose**.

## Visão geral

Este repositório contém dois serviços principais:

- **AuthService**: cadastro/login e gestão de status do usuário (ativo/inativo/bloqueado) e emissão de token JWT.
- **SupplierService**: gestão de **Fornecedores**, **Produtos** e **Entregas**. Todas as rotas são protegidas por JWT e o serviço consulta o AuthService (middleware) para validar se o usuário está ativo.

Cada serviço segue uma separação em camadas (Domain / Application / Infrastructure / API) e aplica migrations automaticamente ao iniciar (útil para containers/dev).

## Regras de negócio (resumo)

### AuthService
- **Autenticação** via **JWT**.
- **Status do usuário** impacta o acesso:
  - usuários **inativos** podem ser barrados por middleware no pipeline.
  - existem operações para **ativar/desativar** e listar usuários por status (ativos/inativos/bloqueados).

> Observação: os detalhes (campos/fluxos) podem ser verificados no Swagger do AuthService e nos UseCases em `AuthService.Application`.

### SupplierService
- **Acesso protegido**: todas as rotas exigem JWT (`[Authorize]`).
- **Operações sensíveis exigem role Admin** (ex.: criar/atualizar/excluir em alguns controllers).
- **Regra de usuário ativo**: o serviço executa um middleware (`UserStatusMiddleware`) que consulta o AuthService para validar se o usuário do token está **ativo** antes de permitir o processamento.

#### Fornecedores (Supplier)
- **CNPJ obrigatório e válido**.
- **CNPJ deve ser único** (não pode existir outro fornecedor com o mesmo CNPJ).
- **Email obrigatório e válido**.
- **Telefone obrigatório** e deve conter apenas dígitos (8–20).
- Update permite atualizar parcial (campos ausentes mantêm o valor atual), mas:
  - se trocar o CNPJ, ele continua precisando ser **válido** e **único**.

#### Produtos (Product)
- **Preço deve ser maior que zero**.
- **SKU deve ser único** (case-insensitive: `SKU-001` e `sku-001` contam como o mesmo SKU).
- Update permite atualizar parcial, mas mantém as validações (preço > 0 e SKU único quando alterado).

## Arquitetura (projetos)

### AuthService
- **AuthService.API**: endpoints HTTP, autenticação JWT, Swagger, middlewares.
- **AuthService.Application**: casos de uso (UseCases), DTOs, validações.
- **AuthService.Domain**: entidades, regras de domínio e contratos.
- **AuthService.Infrastructure**: EF Core DbContext, migrations, repositórios e serviços.
- **AuthService.Tests**: testes unitários (xUnit, Moq, FluentAssertions).

### SupplierService
- **SupplierService.API**: endpoints HTTP, autenticação JWT, Swagger, middlewares.
- **SupplierService.Application**: casos de uso, DTOs, validações.
- **SupplierService.Domain**: entidades, VOs e contratos.
- **SupplierService.Infrastructure**: EF Core DbContext, migrations, repositórios.
- **SupplierServicee.Test**: testes unitários (xUnit, Moq, FluentAssertions).

### SharedKernel
Biblioteca compartilhada para utilitários/contratos comuns (quando aplicável).

## Tecnologias

- .NET 8 (ASP.NET Core)
- Entity Framework Core (SQL Server)
- JWT Bearer Authentication
- FluentValidation
- Swagger / OpenAPI
- Docker / Docker Compose
- xUnit + Moq + FluentAssertions

## Pré-requisitos

- **.NET SDK 8** (para rodar localmente sem Docker)
- **Docker** e **Docker Compose** (recomendado)

## Como executar com Docker Compose

O compose sobe:

- `sqlserver` (SQL Server)
- `authservice` (porta **5001** no host)
- `supplierservice` (porta **5002** no host)

Configuração principal em `docker-compose.yml`:

- SQL Server exposto em `1433`
- ConnectionStrings apontam para `Server=sqlserver,1433` (rede do compose)
- JWT configurado por variáveis de ambiente

Suba o ambiente:

1. Build + up
   - `docker compose up --build`

2. Acesse o Swagger:
   - AuthService: `http://localhost:5001/swagger`
   - SupplierService: `http://localhost:5002/swagger`

### Observações importantes

- **Migrations automáticas**: ambos os serviços aplicam `db.Database.MigrateAsync()` ao iniciar.
- **HTTPS local**: quando rodando em container, a API expõe **HTTP** em `:8080` internamente. Fora de container, o redirecionamento HTTPS pode ocorrer.

## Como executar localmente (sem Docker)

1. Garanta um SQL Server acessível (local, container ou remoto).
2. Configure `ConnectionStrings:DefaultConnection` e as chaves JWT no `appsettings.*` (ou via variáveis de ambiente).
3. Rode os projetos de API:

- `AuthService/AuthService.API`
- `SupplierSevice/SupplierService.API`

> Dica: como o repositório já aplica migrations no startup, basta garantir que a connection string está correta e que o SQL Server está disponível.

## Autenticação (JWT)

- O token é emitido pelo **AuthService**.
- O **SupplierService** exige `Authorization: Bearer <token>` para acessar seus endpoints.
- Algumas rotas exigem **role Admin** (ex.: criar/atualizar/excluir em alguns controllers).

No Swagger, use o botão **Authorize** e informe:

`Bearer {seu_token}`

## Endpoints (alto nível)

Os endpoints exatos podem variar, mas em geral:

### AuthService (base)
- Cadastro de usuário
- Login (gera JWT)
- Consultas de usuários (ativos/inativos/bloqueados)
- Ativar/Desativar/Desbloquear usuários

Swagger: `http://localhost:5001/swagger`

### SupplierService (base)
- `GET/POST/PUT/DELETE /api/Supplier`
- `GET/POST/PUT/DELETE /api/Product`
- `GET/POST/PUT/DELETE /api/Delivery`

Swagger: `http://localhost:5002/swagger`

## Banco de dados

Ao subir via Docker Compose, são criados (via migrations) bancos separados:

- `AuthServiceDb`
- `SupplierServiceDb`

## Testes

Projetos de teste:

- `AuthService.Tests`
- `SupplierServicee.Test`

Execute os testes pela sua IDE ou via CLI.

## Estrutura de pastas (resumo)

```text
.
	AuthService/
		AuthService.API/
		AuthService.Application/
		AuthService.Domain/
		AuthSerrvice.Infrastructure/
		AuthService.Tests/
	SupplierSevice/
		SupplierService.API/
		SupplierService.Application/
		SupplierService.Domain/
		SupplierService.Infrastructure/
		SupplierServicee.Test/
	SharedKernel/
	docker-compose.yml
	docker-compose.override.yml
```

## Troubleshooting

- **Portas em uso**: ajuste `5001:8080` e `5002:8080` no `docker-compose.yml`.
- **SQL Server demorando para subir**: aguarde alguns segundos e reinicie os serviços caso necessário.
- **Falha ao aplicar migrations**: verifique credenciais/connection string e se o container `sqlserver` está saudável.

## Licença

Defina aqui a licença do projeto (ex.: MIT) caso aplicável.