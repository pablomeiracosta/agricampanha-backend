# Auria API - Agricampanha

Backend desenvolvido em .NET 8 com arquitetura em camadas para gerenciamento de notícias e autenticação.

## Estrutura do Projeto

```
Auria.sln
├── Auria.API           # Camada de apresentação (Controllers, Validators, Mappings)
├── Auria.Bll           # Camada de lógica de negócio (Services)
├── Auria.Data          # Camada de persistência (Entity Framework, Repositories)
├── Auria.Dto           # Objetos de transferência de dados
└── Auria.Structure     # Classes de contexto, helpers e configurações
```

## Tecnologias Utilizadas

- **.NET 8**
- **Entity Framework Core 8** (SQL Server)
- **AutoMapper** - Mapeamento de objetos
- **Serilog** - Logging estruturado
- **FluentValidation** - Validação de dados
- **JWT Bearer** - Autenticação
- **Cloudinary** - Upload de imagens
- **BCrypt** - Hash de senhas
- **Swagger/OpenAPI** - Documentação da API

## Configuração Inicial

### 1. Configurar appsettings.json

Edite o arquivo `Auria.API/appsettings.json` e configure:

```json
{
  "ConnectionString": "Server=SEU_SERVIDOR;Database=Agricampanha;Trusted_Connection=True;TrustServerCertificate=True;",
  "Jwt": {
    "SecretKey": "sua-chave-secreta-muito-segura-aqui-com-pelo-menos-32-caracteres",
    "Issuer": "AuriaAPI",
    "Audience": "AuriaClient",
    "ExpirationMinutes": 480
  },
  "Cloudinary": {
    "CloudName": "seu-cloud-name",
    "ApiKey": "sua-api-key",
    "ApiSecret": "seu-api-secret"
  }
}
```

### 2. Criar o Banco de Dados

Execute os comandos no Package Manager Console ou Terminal:

```bash
# Navegar até o diretório da API
cd Auria.API

# Criar a migration inicial
dotnet ef migrations add InitialCreate --project ..\Auria.Data --startup-project .

# Aplicar a migration no banco de dados
dotnet ef database update --project ..\Auria.Data --startup-project .
```

### 3. Executar a Aplicação

```bash
dotnet run --project Auria.API
```

A API estará disponível em:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: https://localhost:5001/swagger

## Credenciais Iniciais

Após executar a aplicação pela primeira vez, um usuário padrão será criado:

- **Login:** admin
- **Senha:** admin123

## Estrutura do Banco de Dados

### Tabelas

#### AGRICAMPANHA_USUARIO
- Id (int, PK)
- Nome (nvarchar(100))
- Login (nvarchar(50), unique)
- SenhaHash (nvarchar(255))
- DataCriacao (datetime2)
- Ativo (bit)

#### AGRICAMPANHA_NOTICIA
- Id (int, PK)
- Titulo (nvarchar(200))
- Subtitulo (nvarchar(300))
- Categoria (int) - 1: Eventos, 2: Institucional, 3: Social, 4: Mercado, 5: Técnico
- DataNoticia (datetime2)
- Fonte (nvarchar(100))
- Texto (nvarchar(max))
- ImagemUrl (nvarchar(500), nullable)
- DataCriacao (datetime2)
- DataAtualizacao (datetime2, nullable)

## Endpoints da API

### Autenticação

#### POST /api/auth/login
Realiza o login do usuário e retorna o token JWT.

**Request:**
```json
{
  "login": "admin",
  "senha": "admin123"
}
```

**Response:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login realizado com sucesso",
  "usuario": {
    "id": 1,
    "nome": "Administrador",
    "login": "admin"
  }
}
```

### Notícias

#### GET /api/noticias
Obtém todas as notícias (não requer autenticação).

#### GET /api/noticias/{id}
Obtém uma notícia específica por ID (não requer autenticação).

#### GET /api/noticias/categoria/{categoria}
Obtém notícias por categoria (não requer autenticação).
- Categorias: 1-Eventos, 2-Institucional, 3-Social, 4-Mercado, 5-Técnico

#### POST /api/noticias
Cria uma nova notícia (requer autenticação).

**Request (multipart/form-data):**
- titulo: string
- subtitulo: string
- categoria: int (1-5)
- dataNoticia: datetime
- fonte: string
- texto: string
- imagem: file (opcional, jpg/jpeg/png/gif)

#### PUT /api/noticias/{id}
Atualiza uma notícia existente (requer autenticação).

**Request (multipart/form-data):**
- id: int
- titulo: string
- subtitulo: string
- categoria: int (1-5)
- dataNoticia: datetime
- fonte: string
- texto: string
- imagem: file (opcional, jpg/jpeg/png/gif)

#### DELETE /api/noticias/{id}
Deleta uma notícia (requer autenticação).

## Autenticação JWT

Para endpoints protegidos, inclua o token JWT no header:

```
Authorization: Bearer {seu-token-jwt}
```

## Logs

Os logs são salvos em:
- Desenvolvimento: `logs/auria-dev-YYYYMMDD.log`
- Produção: `logs/auria-YYYYMMDD.log`

## Upload de Imagens

As imagens são armazenadas no Cloudinary. Configure suas credenciais no `appsettings.json`:
- Obtenha uma conta gratuita em: https://cloudinary.com
- As imagens são armazenadas na pasta "noticias"
- Formatos aceitos: jpg, jpeg, png, gif

## Comandos Úteis do Entity Framework

```bash
# Adicionar nova migration
dotnet ef migrations add NomeDaMigration --project Auria.Data --startup-project Auria.API

# Atualizar banco de dados
dotnet ef database update --project Auria.Data --startup-project Auria.API

# Remover última migration
dotnet ef migrations remove --project Auria.Data --startup-project Auria.API

# Listar migrations
dotnet ef migrations list --project Auria.Data --startup-project Auria.API
```

## Validações Implementadas

### Login
- Login: obrigatório, máx. 50 caracteres
- Senha: obrigatória, mín. 6 caracteres

### Notícia
- Título: obrigatório, máx. 200 caracteres
- Subtítulo: obrigatório, máx. 300 caracteres
- Categoria: obrigatória, valores válidos (1-5)
- Data da Notícia: obrigatória, não pode ser futura
- Fonte: obrigatória, máx. 100 caracteres
- Texto: obrigatório
- Imagem: opcional, formatos: jpg, jpeg, png, gif

## Tratamento de Erros

Todos os endpoints retornam mensagens de erro apropriadas:
- 200: Sucesso
- 201: Criado com sucesso
- 204: Deletado com sucesso
- 400: Requisição inválida
- 401: Não autorizado
- 404: Não encontrado
- 500: Erro interno do servidor

## Contribuição

Este projeto foi desenvolvido para a Agricampanha utilizando as melhores práticas de desenvolvimento .NET.
