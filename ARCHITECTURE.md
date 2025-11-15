# Arquitetura do Projeto Auria - Agricampanha

## Visão Geral

O projeto Auria é uma aplicação backend desenvolvida em .NET 8 seguindo os princípios de **Clean Architecture** e **separação de responsabilidades**. A solução está dividida em 5 projetos principais, cada um com sua responsabilidade específica.

## Estrutura de Camadas

```
┌─────────────────────────────────────────────────────┐
│                   AURIA.API                          │
│         (Presentation Layer / Controllers)           │
│  - Controllers (AuthController, NoticiasController)  │
│  - Validators (FluentValidation)                     │
│  - Mappings (AutoMapper Profiles)                    │
│  - Program.cs (Configuração e DI)                    │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│                   AURIA.BLL                          │
│              (Business Logic Layer)                  │
│  - Services (AuthService, NoticiaService, etc.)      │
│  - Business Rules & Validations                      │
│  - DTOs transformation                               │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│                   AURIA.DATA                         │
│              (Data Access Layer)                     │
│  - DbContext (Entity Framework)                      │
│  - Entities (Database Models)                        │
│  - Repositories (Data Access)                        │
│  - Migrations                                        │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
                [SQL Server]
```

```
┌─────────────────────────────────────────────────────┐
│                  AURIA.DTO                           │
│          (Data Transfer Objects)                     │
│  Objetos compartilhados entre todas as camadas       │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│               AURIA.STRUCTURE                        │
│         (Cross-Cutting Concerns)                     │
│  - AuriaContext (Application Context)                │
│  - Configuration (AppSettings)                       │
│  - Serilog Configuration                             │
│  - Helpers & Utilities                               │
└─────────────────────────────────────────────────────┘
```

## Descrição Detalhada dos Projetos

### 1. Auria.API (Presentation Layer)

**Responsabilidade:** Expor os endpoints HTTP e gerenciar requisições/respostas.

**Componentes:**
- **Controllers:** Expõem endpoints REST
  - `AuthController`: Login e autenticação
  - `NoticiasController`: CRUD de notícias

- **Validators:** Validação de entrada usando FluentValidation
  - `LoginRequestValidator`
  - `NoticiaCreateValidator`
  - `NoticiaUpdateValidator`

- **Mappings:** Configuração do AutoMapper
  - `MappingProfile`: Mapeamento entre Entities e DTOs

- **Program.cs:** Configuração de DI, middleware, JWT, CORS, Swagger

**Dependências:**
- Auria.Bll
- Auria.Dto
- Auria.Structure

### 2. Auria.Bll (Business Logic Layer)

**Responsabilidade:** Implementar regras de negócio e orquestrar operações.

**Componentes:**
- **Services:** Implementam lógica de negócio
  - `AuthService`: Autenticação e geração de JWT
  - `NoticiaService`: Gerenciamento de notícias
  - `CloudinaryService`: Upload de imagens

**Características:**
- Valida regras de negócio
- Orquestra chamadas aos repositórios
- Transforma dados entre camadas
- Registra logs de operações importantes

**Dependências:**
- Auria.Data
- Auria.Dto
- Auria.Structure

### 3. Auria.Data (Data Access Layer)

**Responsabilidade:** Acesso e persistência de dados.

**Componentes:**
- **Context:** DbContext do Entity Framework
  - `AuriaDbContext`: Configuração do banco de dados

- **Entities:** Modelos de domínio
  - `Usuario`: Usuários do sistema
  - `Noticia`: Notícias

- **Repositories:** Padrão Repository para acesso a dados
  - `Repository<T>`: Repository genérico base
  - `UsuarioRepository`: Repository específico de usuários
  - `NoticiaRepository`: Repository específico de notícias

**Características:**
- Abstração do banco de dados
- Uso do padrão Repository
- Migrations do Entity Framework
- Índices para otimização de queries

**Dependências:**
- Auria.Dto
- Auria.Structure

### 4. Auria.Dto (Data Transfer Objects)

**Responsabilidade:** Definir objetos de transferência de dados.

**Componentes:**
- **Enums:** Enumeradores compartilhados
  - `CategoriaNoticia`: Categorias de notícias

- **Login:**
  - `LoginRequestDto`: Request de login
  - `LoginResponseDto`: Response de login
  - `UsuarioDto`: Dados do usuário

- **Noticias:**
  - `NoticiaDto`: Notícia completa
  - `NoticiaCreateDto`: Criação de notícia
  - `NoticiaUpdateDto`: Atualização de notícia

**Características:**
- Sem dependências de outros projetos
- Apenas objetos de transferência
- Validações via FluentValidation na API

### 5. Auria.Structure (Cross-Cutting Concerns)

**Responsabilidade:** Funcionalidades transversais e configurações.

**Componentes:**
- **AuriaContext:** Singleton que centraliza:
  - Configurações (IConfiguration)
  - Settings (AppSettings)
  - Logger (Serilog)
  - Mapper (AutoMapper)

- **Configuration:**
  - `AppSettings`: Configurações da aplicação
  - `JwtSettings`: Configurações JWT
  - `CloudinarySettings`: Configurações Cloudinary
  - `SerilogSettings`: Configurações de log

**Características:**
- Singleton compartilhado
- Centralização de configurações
- Logger configurado e pronto para uso
- Sem lógica de negócio

## Fluxo de Dados

### Exemplo: Criar uma Notícia

```
1. CLIENT → POST /api/noticias (com JWT token)
             ↓
2. API → NoticiasController.Create()
   - Valida JWT (Middleware)
   - Valida dados (FluentValidation)
             ↓
3. BLL → NoticiaService.CreateAsync()
   - Aplica regras de negócio
   - Faz upload da imagem (CloudinaryService)
   - Mapeia DTO → Entity
             ↓
4. DATA → NoticiaRepository.AddAsync()
   - Persiste no banco de dados
   - Retorna Entity criada
             ↓
5. BLL → Mapeia Entity → DTO
   - Loga operação
             ↓
6. API → Retorna 201 Created com DTO
             ↓
7. CLIENT ← Response JSON
```

## Padrões Utilizados

### 1. Repository Pattern
- Abstração da camada de dados
- Facilita testes unitários
- Centraliza lógica de acesso a dados

### 2. Dependency Injection
- Inversão de controle
- Facilita manutenção e testes
- Configurado no Program.cs

### 3. DTO Pattern
- Separação entre entidades de domínio e objetos de transferência
- Evita exposição do modelo de dados
- Flexibilidade nas APIs

### 4. Service Layer Pattern
- Lógica de negócio centralizada
- Reutilização de código
- Separação de responsabilidades

### 5. Singleton Pattern
- AuriaContext é singleton
- Garante única instância de configurações

## Segurança Implementada

### 1. Autenticação JWT
- Token baseado em claims
- Expiração configurável
- Validação de issuer e audience

### 2. Hash de Senhas
- BCrypt com salt automático
- Custo de trabalho: 11 rounds
- Senhas nunca armazenadas em plain text

### 3. Validações
- FluentValidation para entrada de dados
- Validação de tipos de arquivo (upload)
- Proteção contra injeção SQL (Entity Framework)

### 4. CORS
- Configurável por ambiente
- Controle de origens permitidas

## Logs e Monitoramento

### Serilog
- Logs estruturados
- Salvos em arquivo com rotação diária
- Console output em desenvolvimento
- Níveis configuráveis (Debug, Information, Warning, Error)

### Estrutura de Logs
```
[Timestamp] [Level] Message
- Timestamp: Data/hora com timezone
- Level: DEBUG, INFO, WARN, ERROR
- Message: Mensagem estruturada com contexto
```

## Escalabilidade

### Estratégias de Escalabilidade:

1. **Horizontal:**
   - Stateless design (JWT)
   - Múltiplas instâncias da API
   - Load balancer

2. **Vertical:**
   - Connection pooling (EF Core)
   - Async/await em todas operações I/O
   - Índices otimizados no banco

3. **Cache (futuro):**
   - Redis para cache distribuído
   - Cache de notícias frequentes
   - Cache de configurações

4. **CDN:**
   - Cloudinary para imagens
   - Distribuição global de assets

## Testes (Próximos Passos)

### Recomendações:
1. **Unit Tests:**
   - Services (Bll)
   - Repositories (Data)

2. **Integration Tests:**
   - Controllers com banco em memória
   - Fluxo completo da API

3. **Performance Tests:**
   - Load testing com JMeter/K6
   - Stress testing

## Manutenibilidade

### Boas Práticas Implementadas:
- ✅ Separação clara de responsabilidades
- ✅ Código limpo e legível
- ✅ Naming conventions consistentes
- ✅ Logging adequado
- ✅ Documentação inline (XML comments)
- ✅ Tratamento de erros centralizado
- ✅ Configurações externalizadas

### Futuras Melhorias:
- [ ] Implementar CQRS para operações complexas
- [ ] Adicionar cache distribuído (Redis)
- [ ] Implementar rate limiting
- [ ] Adicionar health checks
- [ ] Implementar circuit breaker
- [ ] Adicionar testes automatizados
- [ ] Implementar versionamento de API
- [ ] Adicionar métricas (Prometheus/Grafana)

## Documentação da API

### Swagger/OpenAPI
- Disponível em: `/swagger`
- Documentação interativa
- Teste direto dos endpoints
- Schemas de request/response

## Deployment

### Ambientes Sugeridos:
1. **Desenvolvimento:** localhost
2. **Staging:** Azure App Service / AWS Elastic Beanstalk
3. **Produção:** Azure App Service / AWS Elastic Beanstalk

### Checklist de Deploy:
- [ ] Configurar connection string de produção
- [ ] Configurar chave JWT segura (32+ caracteres)
- [ ] Configurar credenciais Cloudinary
- [ ] Configurar CORS para domínios específicos
- [ ] Configurar SSL/TLS
- [ ] Executar migrations
- [ ] Configurar backup automático do banco
- [ ] Configurar monitoramento (Application Insights)
- [ ] Configurar alertas de erro
- [ ] Revisar logs de produção

## Contato e Suporte

Para dúvidas ou suporte técnico, entre em contato com a equipe de desenvolvimento.
