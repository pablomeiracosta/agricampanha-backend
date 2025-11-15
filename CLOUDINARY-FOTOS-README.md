# Implementação de CRUD de Fotos com Cloudinary

## 📋 Resumo

Implementados os endpoints CRUD para gerenciamento de fotos de notícias utilizando o **Cloudinary** como repositório de imagens.

## 🔧 Tecnologias Utilizadas

- **CloudinaryDotNet v1.27.8** - SDK oficial do Cloudinary para .NET
- **ASP.NET Core 7.0** - Framework web
- **Multipart/form-data** - Para upload de arquivos
- **JWT Authentication** - Todos os endpoints requerem autenticação

## 🔐 Credenciais Cloudinary

Configuradas em `appsettings.Development.json`:

```json
{
  "Cloudinary": {
    "CloudName": "dqj3xbzmz",
    "ApiKey": "745372561514137",
    "ApiSecret": "_IIgUsMsAfDbiZCT2a6MMTjRhZc"
  }
}
```

## 📍 Endpoints Implementados

### 1. Upload de Foto Única
**POST** `/api/fotos/upload`

**Content-Type:** `multipart/form-data`

**Parâmetros:**
- `file` (IFormFile) - Arquivo de imagem (obrigatório)
- `folder` (string) - Pasta de destino no Cloudinary (padrão: "noticias")

**Validações:**
- ✅ Formatos aceitos: .jpg, .jpeg, .png, .gif, .webp
- ✅ Tamanho máximo: 10MB
- ✅ Arquivo não pode ser vazio

**Resposta de Sucesso (200 OK):**
```json
{
  "url": "https://res.cloudinary.com/dqj3xbzmz/image/upload/v1699380000/noticias/foto.jpg",
  "fileName": "foto.jpg",
  "size": 125684,
  "uploadedAt": "2025-11-07T19:00:00Z"
}
```

**Erros:**
- `400 Bad Request` - Arquivo inválido ou tipo não permitido
- `401 Unauthorized` - Token JWT não fornecido ou inválido
- `500 Internal Server Error` - Erro no upload para o Cloudinary

---

### 2. Upload de Múltiplas Fotos
**POST** `/api/fotos/upload/multiplas`

**Content-Type:** `multipart/form-data`

**Parâmetros:**
- `files` (List<IFormFile>) - Lista de arquivos de imagem (obrigatório)
- `folder` (string) - Pasta de destino no Cloudinary (padrão: "noticias")

**Validações:**
- ✅ Máximo de 10 arquivos por requisição
- ✅ Mesmas validações de formato e tamanho do upload único
- ✅ Arquivos inválidos são ignorados (não interrompem o processo)

**Resposta de Sucesso (200 OK):**
```json
[
  {
    "url": "https://res.cloudinary.com/dqj3xbzmz/image/upload/v1699380000/noticias/foto1.jpg",
    "fileName": "foto1.jpg",
    "size": 125684,
    "uploadedAt": "2025-11-07T19:00:00Z"
  },
  {
    "url": "https://res.cloudinary.com/dqj3xbzmz/image/upload/v1699380001/noticias/foto2.jpg",
    "fileName": "foto2.jpg",
    "size": 98432,
    "uploadedAt": "2025-11-07T19:00:00Z"
  }
]
```

**Erros:**
- `400 Bad Request` - Nenhum arquivo fornecido ou mais de 10 arquivos
- `401 Unauthorized` - Token JWT não fornecido ou inválido
- `500 Internal Server Error` - Erro no upload

---

### 3. Deletar Foto
**DELETE** `/api/fotos`

**Parâmetros de Query:**
- `imageUrl` (string) - URL completa da imagem no Cloudinary (obrigatório)

**Exemplo:**
```
DELETE /api/fotos?imageUrl=https://res.cloudinary.com/dqj3xbzmz/image/upload/v1699380000/noticias/foto.jpg
```

**Resposta de Sucesso (200 OK):**
```json
{
  "message": "Foto deletada com sucesso"
}
```

**Erros:**
- `400 Bad Request` - URL não fornecida ou não foi possível deletar
- `401 Unauthorized` - Token JWT não fornecido ou inválido
- `500 Internal Server Error` - Erro ao deletar do Cloudinary

---

## 🏗️ Arquitetura

### Controller
**Arquivo:** `Auria.API/Controllers/FotosController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FotosController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly AuriaContext _context;

    // Métodos: Upload, Delete, UploadMultiplas
}
```

### Service Interface
**Arquivo:** `Auria.Bll/Services/Interfaces/ICloudinaryService.cs`

```csharp
public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folder);
    Task<bool> DeleteImageAsync(string imageUrl);
}
```

### Service Implementation
**Arquivo:** `Auria.Bll/Services/CloudinaryService.cs`

- Upload com transformações automáticas (qualidade, formato)
- Extração de public_id da URL para deleção
- Tratamento de erros específicos do Cloudinary

### DTO
**Arquivo:** `Auria.API/Controllers/FotosController.cs` (definido inline)

```csharp
public class FotoUploadResponseDto
{
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
}
```

---

## 🔄 Fluxo de Uso Integrado

### 1. Upload e Criação de Notícia

```bash
# 1. Fazer login
POST /api/Auth/login
Body: { "login": "admin", "senha": "admin123" }

# 2. Upload da foto
POST /api/fotos/upload
Headers: Authorization: Bearer {token}
Body (multipart/form-data):
  - file: [arquivo de imagem]
  - folder: "noticias"

# Resposta: { "url": "https://res.cloudinary.com/...", ... }

# 3. Criar notícia com a URL da foto
POST /api/noticias
Headers: Authorization: Bearer {token}
Body (JSON):
{
  "titulo": "Título da Notícia",
  "subtitulo": "Subtítulo",
  "categoriaId": 1,
  "dataNoticia": "2025-11-07T00:00:00",
  "fonte": "Fonte",
  "texto": "Texto da notícia...",
  "imagemUrl": "https://res.cloudinary.com/..."  // URL retornada no passo 2
}
```

### 2. Atualização de Foto

```bash
# 1. Upload da nova foto
POST /api/fotos/upload
# Recebe nova URL

# 2. (Opcional) Deletar foto antiga
DELETE /api/fotos?imageUrl={url_antiga}

# 3. Atualizar notícia com nova URL
PUT /api/noticias/{id}
Body: { ..., "imagemUrl": "{nova_url}" }
```

---

## 📊 Logging

Todos os endpoints registram logs através do Serilog:

- **Informação:** Início e conclusão de uploads/deleções
- **Warning:** Arquivos ignorados (inválidos ou muito grandes)
- **Erro:** Falhas no upload/deleção

**Exemplo de logs:**
```
[INF] Upload de foto iniciado: foto.jpg, Tamanho: 125684 bytes
[INF] Upload de foto concluído: https://res.cloudinary.com/...
[INF] Upload múltiplo concluído: 3 fotos enviadas
[INF] Foto deletada com sucesso: https://res.cloudinary.com/...
```

---

## 🧪 Testando via Swagger

1. Acesse: **http://localhost:5000/swagger**
2. Faça login no endpoint `/api/Auth/login`
3. Clique em **Authorize** e insira: `Bearer {token}`
4. Teste os endpoints de fotos:
   - **POST /api/fotos/upload** - Selecione um arquivo
   - **POST /api/fotos/upload/multiplas** - Selecione múltiplos arquivos
   - **DELETE /api/fotos** - Informe uma URL do Cloudinary

---

## ✅ Status

- ✅ CloudinaryDotNet package instalado (v1.27.8)
- ✅ Credenciais configuradas
- ✅ FotosController implementado
- ✅ Validações de arquivo implementadas
- ✅ Logging configurado
- ✅ Documentação Swagger atualizada
- ✅ Aplicação rodando em http://localhost:5000

---

## 📝 Observações Importantes

1. **Segurança:**
   - Todos os endpoints requerem autenticação JWT
   - Validação rigorosa de tipos e tamanhos de arquivo
   - Não expõe credenciais do Cloudinary nos responses

2. **Performance:**
   - Upload assíncrono para não bloquear threads
   - Validação ocorre antes do upload (economiza recursos)
   - Múltiplos uploads são processados sequencialmente (pode ser otimizado com Task.WhenAll se necessário)

3. **Manutenibilidade:**
   - Serviço Cloudinary isolado em camada BLL
   - Controller fino, apenas coordena operações
   - DTOs claros e bem definidos

4. **Integração:**
   - URLs retornadas são compatíveis com campo `imagemUrl` de notícias
   - Fotos organizadas em pastas no Cloudinary
   - Possibilidade de usar diferentes pastas para diferentes contextos
