# Guia Completo - Testando a API no Swagger

## ✅ Status Atual da API

Todos os endpoints estão funcionando com **JSON puro** (application/json):
- ✅ **POST** - Criar notícia (JSON via `[FromBody]`)
- ✅ **PUT** - Atualizar notícia (JSON via `[FromBody]`)
- ✅ **GET** - Listar e consultar (público)
- ✅ **DELETE** - Deletar notícia (requer autenticação)

## 🔐 Passo 1: Autenticação

### 1.1 Fazer Login
1. Acesse: **http://localhost:5000/swagger**
2. Localize: **POST /api/Auth/login**
3. Clique em **"Try it out"**
4. Cole no Request body:
```json
{
  "login": "admin",
  "senha": "admin123"
}
```
5. Clique em **Execute**
6. **COPIE o token** da resposta (campo `token`)

### 1.2 Autenticar no Swagger
1. Clique no botão **"Authorize"** (cadeado verde no topo)
2. Digite: **`Bearer SEU_TOKEN_AQUI`** (substitua SEU_TOKEN_AQUI pelo token copiado)
   - **IMPORTANTE:** Não esqueça da palavra `Bearer` e do espaço!
   - Exemplo: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. Clique em **"Authorize"**
4. Clique em **"Close"**

## 📝 Passo 2: Criar Notícia (POST)

1. Localize: **POST /api/noticias**
2. Clique em **"Try it out"**
3. Cole este JSON no Request body:
```json
{
  "titulo": "Tecnologia 5G chega ao campo brasileiro",
  "subtitulo": "Conectividade avancada permite automacao total da producao",
  "categoriaId": 3,
  "dataNoticia": "2025-11-06T08:00:00",
  "fonte": "TechAgro News",
  "texto": "A tecnologia 5G esta revolucionando o agronegocio brasileiro. Com a nova infraestrutura, fazendas conseguem implementar sistemas de automacao completa, desde tratores autonomos ate monitoramento em tempo real de cada planta. Especialistas estimam aumento de 35% na eficiencia operacional.",
  "imagemUrl": "https://images.unsplash.com/photo-1581092160562-40aa08e78837?w=800"
}
```
4. Clique em **Execute**
5. Resposta esperada: **201 Created** com os dados da notícia criada

## ✏️ Passo 3: Atualizar Notícia (PUT)

1. Primeiro, liste as notícias para pegar um ID:
   - Use **GET /api/noticias**
   - Anote o `id` de uma notícia que deseja atualizar

2. Localize: **PUT /api/noticias/{id}**
3. Clique em **"Try it out"**
4. No campo `id`, digite o ID da notícia (exemplo: 2)
5. Cole este JSON no Request body (ajuste o `id` para corresponder):
```json
{
  "id": 2,
  "titulo": "Nova Tecnica de Irrigacao - VERSAO 2.0",
  "subtitulo": "Sistema aprimorado com inteligencia artificial",
  "categoriaId": 1,
  "dataNoticia": "2025-11-06T09:00:00",
  "fonte": "Embrapa Tech",
  "texto": "A Embrapa lancou a versao 2.0 do sistema de irrigacao inteligente. Agora com IA, o sistema consegue prever necessidades hidricas com 95% de precisao, economizando ate 50% de agua. A tecnologia ja esta sendo testada em 100 fazendas piloto.",
  "imagemUrl": "https://images.unsplash.com/photo-1625246333195-78d9c38ad449?w=800"
}
```
6. Clique em **Execute**
7. Resposta esperada: **200 OK** com os dados atualizados

## 📋 Passo 4: Listar Notícias (GET - Público)

### 4.1 Listar Todas
1. Localize: **GET /api/noticias**
2. Clique em **"Try it out"**
3. Clique em **Execute**
4. Resposta: Lista com todas as notícias

### 4.2 Listar com Paginação
1. Localize: **GET /api/noticias/paginadas**
2. Clique em **"Try it out"**
3. Defina:
   - `pageNumber`: 1
   - `pageSize`: 5
4. Clique em **Execute**
5. Resposta: Página com 5 notícias + informações de paginação

### 4.3 Filtrar por Categoria
1. Localize: **GET /api/noticias/categoria/{categoriaId}**
2. Clique em **"Try it out"**
3. No campo `categoriaId`, digite: **1** (Agricultura)
4. Clique em **Execute**
5. Resposta: Apenas notícias da categoria Agricultura

### 4.4 Buscar por ID
1. Localize: **GET /api/noticias/{id}**
2. Clique em **"Try it out"**
3. No campo `id`, digite o ID desejado (exemplo: 2)
4. Clique em **Execute**
5. Resposta: Dados da notícia específica

## 🗑️ Passo 5: Deletar Notícia (DELETE)

⚠️ **ATENÇÃO:** Esta ação é irreversível!

1. Localize: **DELETE /api/noticias/{id}**
2. Clique em **"Try it out"**
3. No campo `id`, digite o ID da notícia a deletar
4. Clique em **Execute**
5. Resposta esperada: **204 No Content** (deletado com sucesso)

## 📂 Passo 6: Gerenciar Categorias

### 6.1 Listar Categorias (Público)
1. Localize: **GET /api/categorias**
2. Clique em **"Try it out"**
3. Clique em **Execute**
4. Resposta: Lista de categorias disponíveis

### 6.2 Criar Categoria (Requer autenticação)
1. Localize: **POST /api/categorias**
2. Clique em **"Try it out"**
3. Cole este JSON:
```json
{
  "nome": "Inovacao e Startups",
  "descricao": "Noticias sobre startups e inovacao no agronegocio",
  "ativo": true
}
```
4. Clique em **Execute**
5. Resposta: **201 Created** com a categoria criada

## 📸 Passo 7: Gerenciar Fotos (Cloudinary)

### 7.1 Upload de Foto Única (Requer autenticação)
1. Localize: **POST /api/fotos/upload**
2. Clique em **"Try it out"**
3. No campo **file**, clique em **"Choose File"** e selecione uma imagem (jpg, jpeg, png, gif ou webp, máx 10MB)
4. No campo **folder**, digite: **noticias** (ou deixe o padrão)
5. Clique em **Execute**
6. Resposta: **200 OK** com:
```json
{
  "url": "https://res.cloudinary.com/...",
  "fileName": "imagem.jpg",
  "size": 125684,
  "uploadedAt": "2025-11-07T19:00:00Z"
}
```
7. **COPIE a URL** para usar no campo `imagemUrl` ao criar/atualizar notícias

### 7.2 Deletar Foto (Requer autenticação)
1. Localize: **DELETE /api/fotos**
2. Clique em **"Try it out"**
3. No campo **imageUrl**, cole a URL completa da foto no Cloudinary
   - Exemplo: `https://res.cloudinary.com/dqj3xbzmz/image/upload/v123456/noticias/foto.jpg`
4. Clique em **Execute**
5. Resposta: **200 OK** - Foto deletada com sucesso

### 7.3 Upload de Múltiplas Fotos (Requer autenticação)
1. Localize: **POST /api/fotos/upload/multiplas**
2. Clique em **"Try it out"**
3. No campo **files**, clique em **"Choose Files"** e selecione até 10 imagens
4. No campo **folder**, digite: **noticias**
5. Clique em **Execute**
6. Resposta: **200 OK** com array de URLs:
```json
[
  {
    "url": "https://res.cloudinary.com/.../foto1.jpg",
    "fileName": "foto1.jpg",
    "size": 125684,
    "uploadedAt": "2025-11-07T19:00:00Z"
  },
  {
    "url": "https://res.cloudinary.com/.../foto2.jpg",
    "fileName": "foto2.jpg",
    "size": 98432,
    "uploadedAt": "2025-11-07T19:00:00Z"
  }
]
```

**Validações de Upload:**
- ✅ Formatos aceitos: .jpg, .jpeg, .png, .gif, .webp
- ✅ Tamanho máximo: 10MB por arquivo
- ✅ Máximo de 10 arquivos simultâneos (upload múltiplo)
- ✅ Arquivos inválidos são ignorados (não travam o processo)

## 📊 Categorias Disponíveis

Use estes IDs no campo `categoriaId`:

| ID | Nome | Descrição |
|----|------|-----------|
| 1 | Agricultura | Técnicas agrícolas, plantio e colheita |
| 2 | Pecuária | Criação de animais e produção pecuária |
| 3 | Tecnologia no Campo | Inovações tecnológicas no agronegócio |
| 4 | Sustentabilidade | Práticas sustentáveis e preservação ambiental |
| 5 | Mercado e Economia | Cotações e economia do agronegócio |

## ❌ Solução de Problemas

### Erro 401 - Unauthorized
**Causa:** Token não enviado ou expirado
**Solução:**
1. Verifique se clicou em "Authorize" e inseriu `Bearer TOKEN`
2. Faça login novamente para obter novo token
3. Certifique-se que o cadeado está verde (autenticado)

### Erro 400 - Bad Request
**Causa:** JSON inválido ou campos obrigatórios faltando
**Solução:** Verifique os campos obrigatórios:
- `titulo` ✅
- `categoriaId` ✅ (deve ser > 0)
- `dataNoticia` ✅
- `texto` ✅
- `fonte` ✅

### Erro 404 - Not Found
**Causa:** Recurso não existe
**Solução:** Verifique se o ID informado existe usando GET primeiro

### Erro 500 - Internal Server Error
**Causa:** Erro no servidor
**Solução:**
1. Verifique os logs da aplicação
2. Certifique-se que o banco de dados está acessível
3. Verifique se a `categoriaId` existe

## 🎯 Exemplos de Teste Completo

### Fluxo Completo de Teste:

1. **Login** → Obter token ✅
2. **Authorize** → Autenticar no Swagger ✅
3. **GET /api/categorias** → Ver categorias disponíveis ✅
4. **POST /api/fotos/upload** → Upload de foto para Cloudinary ✅
5. **POST /api/noticias** → Criar notícia (com URL da foto) ✅
6. **GET /api/noticias** → Listar todas e pegar ID ✅
7. **PUT /api/noticias/{id}** → Atualizar notícia ✅
8. **GET /api/noticias/{id}** → Verificar atualização ✅
9. **GET /api/noticias/categoria/1** → Filtrar por categoria ✅
10. **DELETE /api/noticias/{id}** → Deletar notícia ✅
11. **DELETE /api/fotos?imageUrl=...** → Deletar foto do Cloudinary ✅

### Fluxo de Upload de Fotos:

1. **Login e Authorize** → Autenticar ✅
2. **POST /api/fotos/upload** → Fazer upload da imagem ✅
3. **Copiar URL retornada** → Ex: https://res.cloudinary.com/... ✅
4. **POST /api/noticias** → Criar notícia usando a URL no campo `imagemUrl` ✅
5. **Acessar URL da foto** → Verificar que está disponível no Cloudinary ✅

## 📌 Informações Importantes

- **URL da API:** http://localhost:5000
- **Swagger:** http://localhost:5000/swagger
- **Login:** admin
- **Senha:** admin123
- **Banco:** mssql.impulsoweb.uni5.net (impulsoweb)
- **Token expira em:** 8 horas
- **Formato aceito:** JSON (application/json)

## ✅ Confirmação de Funcionamento

Todos os endpoints foram testados e estão funcionando:
- ✅ POST /api/noticias - Criação via JSON
- ✅ PUT /api/noticias/{id} - Atualização via JSON
- ✅ GET /api/noticias - Listagem
- ✅ DELETE /api/noticias/{id} - Remoção
- ✅ POST /api/fotos/upload - Upload de foto única
- ✅ POST /api/fotos/upload/multiplas - Upload de múltiplas fotos
- ✅ DELETE /api/fotos - Deleção de foto
- ✅ Autenticação JWT - Funcionando
- ✅ Validações - Ativas
- ✅ Integração Cloudinary - Configurada

**Última atualização:** Endpoints de fotos implementados com Cloudinary!
