# Como Usar o Swagger - Auria API

## Passo 1: Acessar o Swagger
Abra o navegador e acesse: **http://localhost:5000/swagger**

## Passo 2: Fazer Login e Obter o Token

1. No Swagger, localize o endpoint **`POST /api/Auth/login`**
2. Clique em **"Try it out"**
3. No campo **Request body**, cole exatamente este JSON:
```json
{
  "login": "admin",
  "senha": "admin123"
}
```
4. Clique em **"Execute"**
5. Na resposta (Response body), você verá algo assim:
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
6. **COPIE TODO O VALOR DO CAMPO `token`** (o texto que começa com eyJ...)

## Passo 3: Autenticar no Swagger

1. No topo da página do Swagger, você verá um botão verde **"Authorize"** (com um cadeado)
2. Clique nele
3. Uma janela popup vai abrir com um campo chamado **"Value"**
4. Digite neste campo: `Bearer ` (com espaço depois) e cole o token que você copiou
   - Exemplo completo: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
   - **IMPORTANTE:** Não esqueça da palavra `Bearer` e do espaço antes do token!
5. Clique em **"Authorize"**
6. Clique em **"Close"**

## Passo 4: Criar uma Notícia

Agora você está autenticado! Vamos criar uma notícia:

1. Localize o endpoint **`POST /api/noticias`**
2. Clique em **"Try it out"**
3. No campo **Request body**, cole este JSON:
```json
{
  "titulo": "Nova Técnica de Irrigação",
  "subtitulo": "Método revolucionário economiza 40% de água",
  "categoriaId": 1,
  "dataNoticia": "2025-11-05T00:00:00",
  "fonte": "Embrapa",
  "texto": "Pesquisadores da Embrapa desenvolveram uma técnica inovadora que promete transformar a agricultura brasileira...",
  "imagemUrl": "https://example.com/irrigacao.jpg"
}
```
4. Clique em **"Execute"**
5. Você deve receber um **201 Created** com os dados da notícia criada!

## Categorias Disponíveis

Use estes IDs no campo `categoriaId`:
- **1** - Agricultura
- **2** - Pecuária
- **3** - Tecnologia no Campo
- **4** - Sustentabilidade
- **5** - Mercado e Economia

## Outros Endpoints Disponíveis

### Listar Categorias (Público - não requer autenticação)
- **GET /api/categorias** - Lista todas as categorias

### Listar Notícias (Público - não requer autenticação)
- **GET /api/noticias** - Lista todas as notícias
- **GET /api/noticias/paginadas?pageNumber=1&pageSize=10** - Lista com paginação
- **GET /api/noticias/categoria/{categoriaId}** - Filtra por categoria

### Criar Categoria (Requer autenticação)
- **POST /api/categorias**
```json
{
  "nome": "Nova Categoria",
  "descricao": "Descrição da categoria",
  "ativo": true
}
```

### Atualizar Notícia (Requer autenticação)
- **PUT /api/noticias/{id}**
```json
{
  "id": 1,
  "titulo": "Título Atualizado",
  "subtitulo": "Subtítulo atualizado",
  "categoriaId": 1,
  "dataNoticia": "2025-11-05T00:00:00",
  "fonte": "Fonte",
  "texto": "Texto atualizado",
  "imagemUrl": "https://example.com/nova-imagem.jpg"
}
```

### Deletar Notícia (Requer autenticação)
- **DELETE /api/noticias/{id}** - Substitua {id} pelo ID da notícia

### Upload de Foto (Requer autenticação)
- **POST /api/fotos/upload**
  1. Selecione um arquivo de imagem (jpg, jpeg, png, gif, webp)
  2. Defina a pasta (padrão: "noticias")
  3. Receba a URL da foto hospedada no Cloudinary
  4. Use a URL retornada no campo `imagemUrl` ao criar/atualizar notícias

### Upload Múltiplo de Fotos (Requer autenticação)
- **POST /api/fotos/upload/multiplas**
  - Selecione até 10 arquivos de imagem
  - Receba um array com URLs de todas as fotos enviadas

### Deletar Foto (Requer autenticação)
- **DELETE /api/fotos?imageUrl={url}**
  - Informe a URL completa da foto no Cloudinary
  - A foto será deletada permanentemente

## Solução de Problemas

### Erro 401 Unauthorized
- **Causa:** Token não foi enviado ou está inválido/expirado
- **Solução:**
  1. Verifique se clicou em "Authorize" e colou o token com `Bearer ` na frente
  2. Faça login novamente para obter um novo token (tokens expiram em 8 horas)
  3. Certifique-se de que o cadeado no endpoint está "fechado" (verde)

### Erro 400 Bad Request
- **Causa:** JSON inválido ou campos obrigatórios faltando
- **Solução:** Verifique se todos os campos obrigatórios estão preenchidos:
  - `titulo` (obrigatório)
  - `categoriaId` (obrigatório, deve ser > 0)
  - `dataNoticia` (obrigatório)
  - `texto` (obrigatório)

### Erro 404 Not Found
- **Causa:** Recurso não encontrado
- **Solução:** Verifique se o ID existe antes de tentar atualizar ou deletar

## Credenciais de Acesso

- **Login:** admin
- **Senha:** admin123

## URLs Importantes

- **API:** http://localhost:5000
- **Swagger:** http://localhost:5000/swagger
- **Banco de Dados:** mssql.impulsoweb.uni5.net (database: impulsoweb)
