# Sistema de Gerenciamento de Aluguel de Motos 🏍️

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Arquitetura](#️-arquitetura)
- [Como Executar](#-como-executar)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Padrões e Tecnologias](#-padrões-e-tecnologias)
- [Funcionalidades Principais](#-funcionalidades-principais)
- [Fluxos de Integração](#-fluxos-de-integração)
- [Endpoints Principais](#-endpoints-principais)
- [Validações de Negócio](#-validações-de-negócio)
- [Cálculo de Custos](#-cálculo-de-custos-de-locação)
- [Solução de Problemas](#-solução-de-problemas)
- [Documentação Adicional](#-documentação-adicional)
- [Contribuindo](#-contribuindo)
- [Licença](#-licença)

## 📋 Visão Geral

Sistema para gerenciamento de aluguel de motos e entregadores, desenvolvido em .NET 8 com arquitetura de microsserviços. O sistema permite cadastro de motos, registro de entregadores e gestão completa de locações com cálculo automático de custos e penalidades.

## 🏗️ Arquitetura

### Microsserviços

- **Rider API** (Porta 5001): Gerenciamento de entregadores
- **Motorbike API** (Porta 5002): Gerenciamento de motos
- **Subscription API** (Porta 5003): Gerenciamento de locações
- **API Gateway** (Porta 5000): Ponto único de entrada usando YARP

### Componentes de Infraestrutura

- **MongoDB**: 3 instâncias (uma por microsserviço)
- **RabbitMQ**: Mensageria assíncrona
- **MinIO**: Armazenamento S3-compatible para imagens CNH
- **gRPC**: Comunicação síncrona entre serviços

## 🚀 Como Executar

### Pré-requisitos

- Docker e Docker Compose
- .NET 8 SDK (para desenvolvimento local)
- Visual Studio 2022 ou VS Code

### Execução com Docker

```bash
# Na pasta Source
docker-compose up -d
```

O sistema estará disponível em:

- API Gateway: http://localhost:5000
- Swagger UI: http://localhost:5000/docs
- RabbitMQ Management: http://localhost:15672 (guest/guest)
- MinIO Console: http://localhost:9001 (minioadmin/minioadmin)

### Execução Local (Desenvolvimento)

```bash
# Subir apenas infraestrutura
docker-compose up -d rider-database motorbike-database subscription-database rabbitmq rider-storage

# Executar cada microsserviço
cd Microservices/Challenge.Microservices.RiderApi
dotnet run

cd Microservices/Challenge.Microservices.MotorbikeApi
dotnet run

cd Microservices/Challenge.Microservices.SubscriptionApi
dotnet run

cd Gateways/Challenge.Gateways.ApiGateway
dotnet run
```

## 📁 Estrutura do Projeto

```
Source/
├── Common/                      # Bibliotecas compartilhadas
│   ├── Core/
│   │   ├── Challenge.Common.Core.Response/    # Padrões de resposta HTTP
│   │   ├── Challenge.Common.Core.Validation/  # FluentValidation extensions
│   │   ├── Challenge.Common.Core.Cqrs/        # Interfaces CQRS
│   │   └── Challenge.Common.Core.Collections/ # Extensions para coleções
│   ├── Data/
│   │   ├── Challenge.Common.Data/             # Interfaces de repositório
│   │   └── Challenge.Common.Data.Mongo/       # Implementação MongoDB
│   └── Messaging/
│       └── Challenge.Common.Messaging.Grpc/   # Contratos gRPC
│
├── Microservices/
│   ├── Challenge.Microservices.RiderApi/      # API de Entregadores
│   ├── Challenge.Microservices.MotorbikeApi/  # API de Motos
│   └── Challenge.Microservices.SubscriptionApi/ # API de Locações
│
├── Gateways/
│   └── Challenge.Gateways.ApiGateway/         # API Gateway (YARP)
│
├── docker-compose.yml
├── docker-compose.override.yml
└── Directory.Packages.props                    # Versionamento centralizado NuGet
```

## 🔧 Padrões e Tecnologias

### Padrões Implementados

- **CQRS (Command Query Responsibility Segregation)**: Separação de comandos e queries
- **Repository Pattern**: Abstração de acesso a dados
- **Dependency Injection**: IoC container nativo do .NET
- **API Versioning**: Versionamento de APIs REST
- **Validation Pattern**: FluentValidation para validação de comandos

### Stack Tecnológica

- **.NET 8**: Framework principal
- **MongoDB**: Banco de dados NoSQL
- **RabbitMQ**: Message broker
- **MinIO**: Object storage (S3 compatible)
- **gRPC**: Comunicação entre serviços
- **YARP**: Reverse proxy para API Gateway
- **FluentValidation**: Validação de dados
- **AutoMapper**: Mapeamento objeto-objeto
- **Docker**: Containerização
- **Serilog**: Logging estruturado

## 📝 Funcionalidades Principais

### Gestão de Entregadores

- Cadastro com validação de CNPJ e CNH
- Upload de imagem da CNH (PNG/BMP)
- Validação de categoria de habilitação (A, B, AB)

### Gestão de Motos

- Cadastro com placa única
- Atualização de placa
- Exclusão (apenas sem histórico de locação)
- Notificação automática para motos ano 2024

### Gestão de Locações

- Planos: 7, 15, 30, 45 ou 50 dias
- Cálculo automático de valores:
  - Diárias conforme plano
  - Multas por devolução antecipada (20% ou 40%)
  - Taxa adicional por atraso (R$ 50/dia)
- Validação de habilitação categoria A

## 🔄 Fluxos de Integração

### Comunicação Síncrona (gRPC)

- SubscriptionApi → RiderApi: Validação de CNH categoria A

### Comunicação Assíncrona (RabbitMQ)

- MotorbikeApi → RabbitMQ: Publicação de eventos de cadastro
- Consumer → MongoDB: Armazenamento de notificações (motos 2024)
- MotorbikeApi ← → SubscriptionApi: Verificação de locações ativas

## 🧪 Endpoints Principais

### Entregadores (`/entregadores`)

- `POST /entregadores` - Cadastrar entregador
- `POST /entregadores/{id}/cnh` - Upload CNH

### Motos (`/motos`)

- `GET /motos` - Listar motos (filtro por placa)
- `GET /motos/{id}` - Buscar moto específica
- `POST /motos` - Cadastrar moto
- `PUT /motos/{id}/placa` - Atualizar placa
- `DELETE /motos/{id}` - Remover moto

### Locações (`/locacao`)

- `POST /locacao` - Criar locação
- `PUT /locacao/{id}/devolucao` - Informar devolução
- `GET /locacao/{id}` - Consultar locação

## 🔒 Validações de Negócio

### Entregadores

- CNPJ único e válido
- CNH única e válida
- Categoria CNH: A, B ou AB
- Imagem CNH: PNG ou BMP, máx 50MB

### Motos

- Placa única (formato brasileiro)
- Ano entre 1900 e ano atual + 1
- Não pode deletar com locações ativas

### Locações

- Entregador deve ter CNH categoria A ou AB
- Data início: dia seguinte à criação
- Moto não pode estar alugada
- Planos fixos: 7, 15, 30, 45 ou 50 dias

## 📊 Cálculo de Custos de Locação

### Valores por Plano

- 7 dias: R$ 30,00/dia
- 15 dias: R$ 28,00/dia
- 30 dias: R$ 22,00/dia
- 45 dias: R$ 20,00/dia
- 50 dias: R$ 18,00/dia

### Penalidades

- **Devolução antecipada**:
  - Plano 7 dias: 20% sobre dias não utilizados
  - Plano 15 dias: 40% sobre dias não utilizados
- **Devolução atrasada**: R$ 50,00 por dia adicional

## 🐛 Solução de Problemas

### Containers não iniciam

```bash
docker-compose down -v
docker-compose up -d
```

### Erro de conexão MongoDB

Verificar se as portas 27017, 27018, 27019 estão livres

### Erro de upload de imagem

Verificar se o bucket MinIO foi criado corretamente

## 📚 Documentação Adicional

- **Swagger UI**: http://localhost:5000/docs
- **Arquitetura Detalhada**: [ARCHITECTURE.md](../Docs/ARCHITECTURE.md)
- **Especificação Original**: [README.md](../README.md)
- **RabbitMQ Management**: http://localhost:15672
- **MinIO Console**: http://localhost:9001

## 📄 Licença

Este projeto é privado e confidencial.
