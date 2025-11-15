# Guia Rápido de Inicialização

## Passo 1: Restaurar Pacotes

```bash
cd "c:\Projetos\Auria\clientes\P0004 - Agricampanha\dev\backend"
dotnet restore
```

## Passo 2: Configurar Connection String

Edite o arquivo `Auria.API/appsettings.json` e ajuste a connection string para seu SQL Server:

```json
"ConnectionString": "Server=localhost;Database=Agricampanha;Trusted_Connection=True;TrustServerCertificate=True;"
```

## Passo 3: Configurar Cloudinary (Opcional)

Se for usar upload de imagens, configure no `appsettings.json`:

```json
"Cloudinary": {
  "CloudName": "seu-cloud-name",
  "ApiKey": "sua-api-key",
  "ApiSecret": "seu-api-secret"
}
```

Obtenha credenciais gratuitas em: https://cloudinary.com

## Passo 4: Criar o Banco de Dados

### Opção A: Usando Entity Framework Migrations (Recomendado)

```bash
cd Auria.API
dotnet ef migrations add InitialCreate --project ..\Auria.Data
dotnet ef database update --project ..\Auria.Data
```

### Opção B: Usando Script SQL

Execute o script `Scripts/CreateDatabase.sql` no SQL Server Management Studio ou Azure Data Studio.

## Passo 5: Executar a Aplicação

```bash
cd Auria.API
dotnet run
```

A API estará disponível em:
- https://localhost:5001/swagger (Swagger UI)
- http://localhost:5000

## Passo 6: Testar a API

### 6.1 Fazer Login

**Endpoint:** POST https://localhost:5001/api/auth/login

**Body (JSON):**
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

Copie o valor do campo `token` para usar nas próximas requisições.

### 6.2 Listar Notícias (não requer autenticação)

**Endpoint:** GET https://localhost:5001/api/noticias

### 6.3 Criar Notícia (requer autenticação)

**Endpoint:** POST https://localhost:5001/api/noticias

**Headers:**
```
Authorization: Bearer {seu-token}
Content-Type: multipart/form-data
```

**Body (Form Data):**
```
titulo: Minha Primeira Notícia
subtitulo: Subtítulo da notícia
categoria: 1
dataNoticia: 2024-01-15T10:00:00
fonte: Redação
texto: Texto completo da notícia aqui...
imagem: (arquivo de imagem - opcional)
```

## Estrutura de Categorias

- 1 = Eventos
- 2 = Institucional
- 3 = Social
- 4 = Mercado
- 5 = Técnico

## Testando com Swagger

1. Acesse: https://localhost:5001/swagger
2. Clique em "Authorize" no topo da página
3. Cole o token no formato: `Bearer {seu-token}`
4. Clique em "Authorize" e depois "Close"
5. Agora você pode testar todos os endpoints protegidos

## Testando com cURL

```bash
# Login
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"login\":\"admin\",\"senha\":\"admin123\"}" \
  -k

# Listar notícias
curl -X GET "https://localhost:5001/api/noticias" -k

# Criar notícia (substitua {TOKEN} pelo seu token)
curl -X POST "https://localhost:5001/api/noticias" \
  -H "Authorization: Bearer {TOKEN}" \
  -F "titulo=Nova Notícia" \
  -F "subtitulo=Subtítulo" \
  -F "categoria=1" \
  -F "dataNoticia=2024-01-15T10:00:00" \
  -F "fonte=Redação" \
  -F "texto=Texto da notícia" \
  -k
```

## Testando com Postman

1. Importe a collection do Postman (se disponível)
2. Configure a variável de ambiente `baseUrl` como `https://localhost:5001`
3. Execute o request de Login
4. O token será automaticamente salvo e usado nas próximas requisições

## Verificar Logs

Os logs da aplicação são salvos em:
```
Auria.API/logs/auria-dev-YYYYMMDD.log
```

## Problemas Comuns

### Erro de Certificado SSL
Se encontrar problemas com certificado SSL, você pode:
1. Confiar no certificado de desenvolvimento: `dotnet dev-certs https --trust`
2. Ou usar HTTP: http://localhost:5000

### Erro de Connection String
Verifique se:
- O SQL Server está rodando
- A connection string está correta
- Você tem permissões para criar o banco de dados

### Cloudinary não configurado
Se não configurar o Cloudinary:
- A API funcionará normalmente
- Upload de imagens retornará erro
- Configure quando for usar a funcionalidade de imagens

## Próximos Passos

1. Altere a senha padrão do usuário admin
2. Configure o Cloudinary para upload de imagens
3. Ajuste o tempo de expiração do JWT no appsettings.json
4. Configure CORS de acordo com seu frontend
5. Crie usuários adicionais conforme necessário
