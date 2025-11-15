# Guia de Testes - API Auria Agricampanha

## Índice
1. [Pré-requisitos](#pré-requisitos)
2. [Configuração do Banco de Dados](#configuração-do-banco-de-dados)
3. [Iniciar a Aplicação](#iniciar-a-aplicação)
4. [Testes dos Endpoints](#testes-dos-endpoints)
   - [1. Autenticação](#1-autenticação)
   - [2. CRUD de Categorias](#2-crud-de-categorias)
   - [3. CRUD de Notícias](#3-crud-de-notícias)
   - [4. Paginação](#4-paginação)
   - [5. Filtro por Categoria](#5-filtro-por-categoria)

---

## Pré-requisitos

- SQL Server instalado e rodando (localhost)
- .NET 7.0 SDK instalado
- Ferramenta de teste de API (Postman, Insomnia, ou usar o Swagger)

---

## Configuração do Banco de Dados

### Opção 1: Usar Entity Framework (Recomendado)

```bash
# Navegar até a pasta Auria.Data
cd "c:\Projetos\Auria\clientes\P0004 - Agricampanha\dev\backend\Auria.Data"

# Aplicar a migração
dotnet ef database update --startup-project ../Auria.API
```

### Opção 2: Executar Scripts SQL Manualmente

1. Abrir SQL Server Management Studio (SSMS)
2. Conectar no servidor `localhost`
3. Executar o script: `sql-scripts/create-database.sql`
4. Executar o script: `sql-scripts/insert-test-data.sql`

---

## Iniciar a Aplicação

```bash
# Navegar até a pasta Auria.API
cd "c:\Projetos\Auria\clientes\P0004 - Agricampanha\dev\backend\Auria.API"

# Executar a aplicação
dotnet run --urls "http://localhost:5000"
```

**Acessar Swagger:**
- URL: http://localhost:5000/swagger
- A interface do Swagger permite testar todos os endpoints de forma interativa

---

## Testes dos Endpoints

### 1. Autenticação

#### 1.1. Login (Obter Token JWT)

**Endpoint:** `POST /api/auth/login`

**Request:**
```json
{
  "login": "admin",
  "senha": "Admin@123"
}
```

**Resposta Esperada (200 OK):**
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

**IMPORTANTE:** Copie o token retornado. Ele será necessário para os endpoints protegidos!

#### 1.2. Configurar Autenticação no Swagger

1. Clique no botão **"Authorize"** no canto superior direito
2. Digite: `Bearer SEU_TOKEN_AQUI` (substituindo SEU_TOKEN_AQUI pelo token obtido no login)
3. Clique em "Authorize"

#### 1.3. Configurar Autenticação no Postman/Insomnia

1. Vá para a aba "Authorization"
2. Selecione tipo "Bearer Token"
3. Cole o token no campo "Token"

---

### 2. CRUD de Categorias

#### 2.1. Listar Todas as Categorias

**Endpoint:** `GET /api/categorias`

**Headers:**
- `Authorization: Bearer SEU_TOKEN`

**Resposta Esperada (200 OK):**
```json
[
  {
    "id": 1,
    "nome": "Agricultura",
    "descricao": "Notícias relacionadas a técnicas agrícolas, plantio e colheita",
    "ativo": true,
    "dataCriacao": "2025-11-04T13:00:00"
  },
  {
    "id": 2,
    "nome": "Pecuária",
    "descricao": "Notícias sobre criação de animais e produção pecuária",
    "ativo": true,
    "dataCriacao": "2025-11-04T13:00:00"
  }
]
```

#### 2.2. Listar Apenas Categorias Ativas

**Endpoint:** `GET /api/categorias/ativas`

**Autenticação:** Não requerida (AllowAnonymous)

**Resposta Esperada (200 OK):** Lista apenas categorias com `ativo: true`

#### 2.3. Obter Categoria por ID

**Endpoint:** `GET /api/categorias/{id}`

**Exemplo:** `GET /api/categorias/1`

**Autenticação:** Não requerida (AllowAnonymous)

**Resposta Esperada (200 OK):**
```json
{
  "id": 1,
  "nome": "Agricultura",
  "descricao": "Notícias relacionadas a técnicas agrícolas, plantio e colheita",
  "ativo": true,
  "dataCriacao": "2025-11-04T13:00:00"
}
```

#### 2.4. Criar Nova Categoria

**Endpoint:** `POST /api/categorias`

**Headers:**
- `Authorization: Bearer SEU_TOKEN`
- `Content-Type: application/json`

**Request:**
```json
{
  "nome": "Clima e Meio Ambiente",
  "descricao": "Informações sobre clima, previsões e impactos ambientais",
  "ativo": true
}
```

**Resposta Esperada (201 Created):**
```json
{
  "id": 6,
  "nome": "Clima e Meio Ambiente",
  "descricao": "Informações sobre clima, previsões e impactos ambientais",
  "ativo": true,
  "dataCriacao": "2025-11-04T14:30:00"
}
```

**Validações:**
- Nome é obrigatório
- Nome deve ser único
- Nome máximo 100 caracteres

#### 2.5. Atualizar Categoria

**Endpoint:** `PUT /api/categorias/{id}`

**Exemplo:** `PUT /api/categorias/6`

**Headers:**
- `Authorization: Bearer SEU_TOKEN`
- `Content-Type: application/json`

**Request:**
```json
{
  "id": 6,
  "nome": "Clima e Meteorologia",
  "descricao": "Previsões do tempo e impactos climáticos no agronegócio",
  "ativo": true
}
```

**Resposta Esperada (200 OK):**
```json
{
  "id": 6,
  "nome": "Clima e Meteorologia",
  "descricao": "Previsões do tempo e impactos climáticos no agronegócio",
  "ativo": true,
  "dataCriacao": "2025-11-04T14:30:00"
}
```

#### 2.6. Deletar Categoria

**Endpoint:** `DELETE /api/categorias/{id}`

**Exemplo:** `DELETE /api/categorias/6`

**Headers:**
- `Authorization: Bearer SEU_TOKEN`

**Resposta Esperada (204 No Content)**

**Observação:** Não é possível deletar uma categoria que possui notícias associadas.

---

### 3. CRUD de Notícias

#### 3.1. Listar Todas as Notícias

**Endpoint:** `GET /api/noticias`

**Autenticação:** Não requerida (AllowAnonymous)

**Resposta Esperada (200 OK):**
```json
[
  {
    "id": 1,
    "titulo": "Nova técnica de irrigação aumenta produtividade",
    "subtitulo": "Método desenvolvido por pesquisadores reduz consumo de água em 40%",
    "categoriaId": 1,
    "categoria": {
      "id": 1,
      "nome": "Agricultura",
      "descricao": "Notícias relacionadas a técnicas agrícolas, plantio e colheita",
      "ativo": true,
      "dataCriacao": "2025-11-04T13:00:00"
    },
    "dataNoticia": "2025-11-04T13:00:00",
    "fonte": "Embrapa",
    "texto": "Pesquisadores da Embrapa desenvolveram...",
    "imagemUrl": null,
    "dataCriacao": "2025-11-04T13:00:00",
    "dataAtualizacao": "2025-11-04T13:00:00"
  }
]
```

#### 3.2. Obter Notícia por ID

**Endpoint:** `GET /api/noticias/{id}`

**Exemplo:** `GET /api/noticias/1`

**Autenticação:** Não requerida (AllowAnonymous)

**Resposta Esperada (200 OK):** Objeto da notícia completo

#### 3.3. Criar Nova Notícia

**Endpoint:** `POST /api/noticias`

**Headers:**
- `Authorization: Bearer SEU_TOKEN`
- `Content-Type: multipart/form-data`

**Request (form-data):**
```
Titulo: "Safra de soja bate recorde histórico"
Subtitulo: "Produção aumenta 15% em relação ao ano anterior"
CategoriaId: 1
DataNoticia: "2025-11-04"
Fonte: "Conab"
Texto: "A safra de soja 2024/2025 registrou um recorde histórico de produção..."
Imagem: [arquivo opcional - jpg, jpeg, png, gif]
```

**Resposta Esperada (201 Created):**
```json
{
  "id": 3,
  "titulo": "Safra de soja bate recorde histórico",
  "subtitulo": "Produção aumenta 15% em relação ao ano anterior",
  "categoriaId": 1,
  "categoria": null,
  "dataNoticia": "2025-11-04T00:00:00",
  "fonte": "Conab",
  "texto": "A safra de soja 2024/2025 registrou...",
  "imagemUrl": "https://res.cloudinary.com/...",
  "dataCriacao": "2025-11-04T14:45:00",
  "dataAtualizacao": "2025-11-04T14:45:00"
}
```

**Validações:**
- Título: obrigatório, máximo 200 caracteres
- Subtítulo: obrigatório, máximo 300 caracteres
- CategoriaId: obrigatório, deve existir
- DataNoticia: obrigatória, não pode ser futura
- Fonte: obrigatória, máximo 100 caracteres
- Texto: obrigatório
- Imagem: opcional, formatos permitidos: jpg, jpeg, png, gif

#### 3.4. Atualizar Notícia

**Endpoint:** `PUT /api/noticias/{id}`

**Exemplo:** `PUT /api/noticias/3`

**Headers:**
- `Authorization: Bearer SEU_TOKEN`
- `Content-Type: multipart/form-data`

**Request (form-data):**
```
Id: 3
Titulo: "Safra de soja supera expectativas"
Subtitulo: "Produção aumenta 18% em relação ao ano anterior"
CategoriaId: 1
DataNoticia: "2025-11-04"
Fonte: "Conab"
Texto: "A safra de soja 2024/2025 superou as expectativas..."
Imagem: [arquivo opcional para substituir a imagem]
```

**Resposta Esperada (200 OK):** Objeto da notícia atualizado

#### 3.5. Deletar Notícia

**Endpoint:** `DELETE /api/noticias/{id}`

**Exemplo:** `DELETE /api/noticias/3`

**Headers:**
- `Authorization: Bearer SEU_TOKEN`

**Resposta Esperada (204 No Content)**

**Observação:** Se a notícia tiver uma imagem no Cloudinary, ela será deletada automaticamente.

---

### 4. Paginação

#### 4.1. Listar Notícias Paginadas

**Endpoint:** `GET /api/noticias/paginadas`

**Query Parameters:**
- `pageNumber` (opcional, padrão: 1): Número da página
- `pageSize` (opcional, padrão: 10, min: 1, max: 100): Quantidade de itens por página

**Exemplos:**
- `GET /api/noticias/paginadas` (página 1, 10 itens)
- `GET /api/noticias/paginadas?pageNumber=2&pageSize=5` (página 2, 5 itens)
- `GET /api/noticias/paginadas?pageNumber=1&pageSize=20` (página 1, 20 itens)

**Autenticação:** Não requerida (AllowAnonymous)

**Resposta Esperada (200 OK):**
```json
{
  "items": [
    {
      "id": 1,
      "titulo": "Nova técnica de irrigação aumenta produtividade",
      "subtitulo": "Método desenvolvido por pesquisadores...",
      "categoriaId": 1,
      "categoria": {
        "id": 1,
        "nome": "Agricultura",
        "descricao": "...",
        "ativo": true,
        "dataCriacao": "2025-11-04T13:00:00"
      },
      "dataNoticia": "2025-11-04T13:00:00",
      "fonte": "Embrapa",
      "texto": "...",
      "imagemUrl": null,
      "dataCriacao": "2025-11-04T13:00:00",
      "dataAtualizacao": "2025-11-04T13:00:00"
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 2,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

**Campos de Metadados:**
- `currentPage`: Página atual
- `pageSize`: Tamanho da página
- `totalCount`: Total de registros
- `totalPages`: Total de páginas
- `hasPreviousPage`: Indica se existe página anterior
- `hasNextPage`: Indica se existe próxima página

**Validações:**
- `pageNumber` deve ser >= 1
- `pageSize` deve estar entre 1 e 100

---

### 5. Filtro por Categoria

#### 5.1. Listar Notícias por Categoria

**Endpoint:** `GET /api/noticias/categoria/{categoriaId}`

**Exemplo:** `GET /api/noticias/categoria/1`

**Autenticação:** Não requerida (AllowAnonymous)

**Resposta Esperada (200 OK):**
```json
[
  {
    "id": 1,
    "titulo": "Nova técnica de irrigação aumenta produtividade",
    "subtitulo": "Método desenvolvido por pesquisadores reduz consumo de água em 40%",
    "categoriaId": 1,
    "categoria": {
      "id": 1,
      "nome": "Agricultura",
      "descricao": "Notícias relacionadas a técnicas agrícolas, plantio e colheita",
      "ativo": true,
      "dataCriacao": "2025-11-04T13:00:00"
    },
    "dataNoticia": "2025-11-04T13:00:00",
    "fonte": "Embrapa",
    "texto": "Pesquisadores da Embrapa desenvolveram...",
    "imagemUrl": null,
    "dataCriacao": "2025-11-04T13:00:00",
    "dataAtualizacao": "2025-11-04T13:00:00"
  }
]
```

**Observação:** A categoria é carregada automaticamente (eager loading) para cada notícia.

---

## Checklist de Testes

Use este checklist para garantir que todos os endpoints foram testados:

### Autenticação
- [ ] Login com credenciais corretas retorna token
- [ ] Login com credenciais incorretas retorna erro
- [ ] Token JWT é aceito em endpoints protegidos
- [ ] Acesso sem token retorna 401 Unauthorized

### Categorias
- [ ] GET /api/categorias retorna todas as categorias
- [ ] GET /api/categorias/ativas retorna apenas ativas
- [ ] GET /api/categorias/{id} retorna categoria específica
- [ ] GET /api/categorias/{id} com ID inválido retorna 404
- [ ] POST /api/categorias cria nova categoria
- [ ] POST /api/categorias com nome duplicado retorna erro
- [ ] PUT /api/categorias/{id} atualiza categoria
- [ ] DELETE /api/categorias/{id} deleta categoria sem notícias
- [ ] DELETE categoria com notícias retorna erro

### Notícias
- [ ] GET /api/noticias retorna todas as notícias
- [ ] GET /api/noticias/{id} retorna notícia específica
- [ ] GET /api/noticias/{id} com ID inválido retorna 404
- [ ] POST /api/noticias cria nova notícia sem imagem
- [ ] POST /api/noticias cria nova notícia com imagem
- [ ] POST /api/noticias com categoria inválida retorna erro
- [ ] POST /api/noticias com data futura retorna erro
- [ ] PUT /api/noticias/{id} atualiza notícia
- [ ] PUT /api/noticias/{id} substitui imagem
- [ ] DELETE /api/noticias/{id} deleta notícia

### Paginação
- [ ] GET /api/noticias/paginadas retorna primeira página
- [ ] GET /api/noticias/paginadas?pageNumber=2 retorna segunda página
- [ ] GET /api/noticias/paginadas?pageSize=5 respeita tamanho
- [ ] Metadados de paginação estão corretos (totalPages, hasNextPage, etc)
- [ ] pageNumber=0 retorna erro de validação
- [ ] pageSize=101 retorna erro de validação

### Filtro por Categoria
- [ ] GET /api/noticias/categoria/{id} retorna notícias filtradas
- [ ] GET /api/noticias/categoria/{id} inclui dados da categoria
- [ ] Categoria sem notícias retorna array vazio

---

## Validação de Persistência no Banco

Para verificar se os dados estão sendo persistidos corretamente no banco:

```sql
-- Verificar usuários
SELECT * FROM AGRICAMPANHA_USUARIO;

-- Verificar categorias
SELECT * FROM CategoriasNoticias;

-- Verificar notícias com categorias
SELECT
    n.Id,
    n.Titulo,
    n.Subtitulo,
    c.Nome AS Categoria,
    n.DataNoticia,
    n.Fonte,
    n.DataCriacao,
    n.DataAtualizacao
FROM AGRICAMPANHA_NOTICIA n
INNER JOIN CategoriasNoticias c ON n.CategoriaId = c.Id
ORDER BY n.DataCriacao DESC;

-- Verificar relacionamento (categorias com suas notícias)
SELECT
    c.Nome AS Categoria,
    COUNT(n.Id) AS TotalNoticias
FROM CategoriasNoticias c
LEFT JOIN AGRICAMPANHA_NOTICIA n ON c.Id = n.CategoriaId
GROUP BY c.Nome;
```

---

## Problemas Comuns

### 1. Erro: "Cannot connect to SQL Server"
- Verifique se o SQL Server está rodando
- Confirme a connection string em `appsettings.Development.json`
- Tente executar: `dotnet ef database update --startup-project ../Auria.API`

### 2. Erro: "Unauthorized" (401)
- Verifique se você fez login e obteve o token
- Confirme que o token está sendo enviado no header: `Authorization: Bearer {token}`
- Verifique se o token não expirou (validade: 8 horas)

### 3. Erro: "Categoria não encontrada" ao criar notícia
- Certifique-se de que o `categoriaId` informado existe
- Liste as categorias disponíveis: `GET /api/categorias`

### 4. Erro ao fazer upload de imagem
- Verifique se o formato é permitido (jpg, jpeg, png, gif)
- Confirme que o Cloudinary está configurado corretamente
- Verifique os logs da aplicação para mais detalhes

---

## Conclusão

Após executar todos os testes deste guia, você terá verificado:

1. ✅ A aplicação compila sem erros
2. ✅ A aplicação roda em http://localhost:5000
3. ✅ O banco de dados foi criado corretamente
4. ✅ As tabelas foram criadas com os relacionamentos
5. ✅ A autenticação JWT funciona
6. ✅ O CRUD de categorias está funcional
7. ✅ O CRUD de notícias está funcional
8. ✅ O relacionamento de chave estrangeira está funcionando
9. ✅ A paginação retorna os dados corretos
10. ✅ O filtro por categoria funciona
11. ✅ Os dados são persistidos corretamente no banco
12. ✅ O eager loading da categoria está funcionando

**Documentação Swagger:** http://localhost:5000/swagger
**Health Check:** http://localhost:5000/health (se implementado)

---

**Data de criação:** 04/11/2025
**Versão da API:** 1.0.0
**Framework:** .NET 7.0
