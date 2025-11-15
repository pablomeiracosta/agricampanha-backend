# Guia de Setup do Swagger

## ✅ Correção Aplicada

O Swagger foi configurado para funcionar em **todos os ambientes** (não apenas desenvolvimento).

## 🌐 Como Acessar

Após iniciar a aplicação, acesse:

### Opção 1: URL Direta
```
https://localhost:5001/swagger
```

### Opção 2: URL Alternativa
```
http://localhost:5000/swagger
```

### Opção 3: Raiz (se configurado)
```
https://localhost:5001/
```

## 🚀 Passo a Passo para Testar

### 1. Iniciar a Aplicação

```bash
cd Auria.API
dotnet run
```

Você deverá ver algo como:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### 2. Abrir o Swagger no Navegador

Abra seu navegador e acesse: `https://localhost:5001/swagger`

### 3. Interface do Swagger

Você verá:
- **Título:** Auria API - Agricampanha
- **Versão:** v1
- **Descrição:** API para gerenciamento de notícias e autenticação
- **Seções:**
  - Auth (Login)
  - Noticias (CRUD completo)

## 🔐 Como Autenticar no Swagger

### Passo 1: Fazer Login

1. Expanda o endpoint `POST /api/auth/login`
2. Clique em "Try it out"
3. Preencha o corpo:
```json
{
  "login": "admin",
  "senha": "admin123"
}
```
4. Clique em "Execute"
5. Copie o valor do campo `token` da resposta

### Passo 2: Autorizar

1. Clique no botão **"Authorize"** no topo da página (ícone de cadeado)
2. Na janela que abrir, cole o token no formato:
```
Bearer seu-token-aqui
```
   **Importante:** Deve começar com a palavra "Bearer " seguida de espaço!
3. Clique em "Authorize"
4. Clique em "Close"

### Passo 3: Testar Endpoints Protegidos

Agora você pode testar os endpoints que requerem autenticação:
- POST /api/noticias
- PUT /api/noticias/{id}
- DELETE /api/noticias/{id}

## 🐛 Problemas Comuns

### Problema 1: "Página não encontrada" (404)

**Causa:** Aplicação não está rodando ou URL incorreta

**Solução:**
```bash
# Verificar se a aplicação está rodando
cd Auria.API
dotnet run

# Aguardar mensagem "Now listening on: https://localhost:5001"
# Então acessar: https://localhost:5001/swagger
```

### Problema 2: Certificado SSL não confiável

**Causa:** Certificado de desenvolvimento não está instalado

**Solução:**
```bash
dotnet dev-certs https --trust
```

Ou acesse via HTTP:
```
http://localhost:5000/swagger
```

### Problema 3: "Unable to load swagger.json"

**Causa:** Erro na configuração do Swagger ou controladores

**Solução:**
1. Verificar logs em `logs/auria-dev-YYYYMMDD.log`
2. Verificar se os controllers estão corretamente anotados com `[ApiController]` e `[Route]`
3. Rebuild do projeto:
```bash
dotnet clean
dotnet build
dotnet run
```

### Problema 4: Endpoints não aparecem

**Causa:** Controllers não estão sendo carregados

**Solução:**
1. Verificar se os arquivos estão na pasta `Controllers/`
2. Verificar namespace correto
3. Rebuild:
```bash
dotnet clean
dotnet build
```

### Problema 5: "401 Unauthorized" ao testar endpoint protegido

**Causa:** Token não foi configurado ou expirou

**Solução:**
1. Fazer login novamente
2. Copiar novo token
3. Clicar em "Authorize" e colar com "Bearer " na frente
4. Tentar novamente

### Problema 6: Swagger em branco ou não carrega

**Causa:** Erro JavaScript ou conflito de extensões do navegador

**Solução:**
1. Limpar cache do navegador (Ctrl + Shift + Del)
2. Testar em modo anônimo/privado
3. Testar em outro navegador
4. Desabilitar extensões (AdBlock, etc)

### Problema 7: Porta já em uso

**Erro:**
```
Failed to bind to address https://127.0.0.1:5001: address already in use.
```

**Solução:**

Windows (PowerShell):
```powershell
# Ver o que está usando a porta
netstat -ano | findstr :5001

# Matar processo (substitua PID pelo número encontrado)
taskkill /PID <PID> /F
```

Linux/Mac:
```bash
# Ver o que está usando a porta
lsof -i :5001

# Matar processo
kill -9 <PID>
```

Ou alterar porta em `launchSettings.json`:
```json
"applicationUrl": "https://localhost:5002;http://localhost:5003"
```

## 📝 Testando Endpoints no Swagger

### Teste 1: Login (Não requer autenticação)

1. Expanda `POST /api/auth/login`
2. Click "Try it out"
3. Use:
```json
{
  "login": "admin",
  "senha": "admin123"
}
```
4. Execute
5. Deve retornar status **200** com token

### Teste 2: Listar Notícias (Não requer autenticação)

1. Expanda `GET /api/noticias`
2. Click "Try it out"
3. Execute
4. Deve retornar status **200** com array de notícias

### Teste 3: Criar Notícia (Requer autenticação)

1. Primeiro, faça login e autorize (ver seção acima)
2. Expanda `POST /api/noticias`
3. Click "Try it out"
4. Preencha os campos:
   - titulo: "Minha Notícia de Teste"
   - subtitulo: "Subtítulo da notícia"
   - categoria: 1
   - dataNoticia: Data atual no formato ISO (ex: 2024-01-15T10:00:00)
   - fonte: "Redação"
   - texto: "Texto completo da notícia..."
   - imagem: (opcional) selecione uma imagem
5. Execute
6. Deve retornar status **201** com a notícia criada

### Teste 4: Upload de Imagem

Para testar upload:
1. Autorize com token válido
2. Use `POST /api/noticias`
3. No campo "imagem", clique em "Choose File"
4. Selecione uma imagem (jpg, png, gif)
5. Preencha os outros campos
6. Execute

**Nota:** Upload de imagem só funciona se Cloudinary estiver configurado!

## ⚙️ Configuração Avançada

### Desabilitar Swagger em Produção

Se quiser desabilitar em produção, edite `Program.cs`:

```csharp
// Habilitar apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auria API v1");
    });
}
```

### Customizar URL do Swagger

Em `Program.cs`:

```csharp
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auria API v1");
    c.RoutePrefix = "api-docs"; // Acessa em /api-docs ao invés de /swagger
});
```

### Adicionar Comentários XML

1. Edite `Auria.API.csproj`:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

2. Em `Program.cs`, adicione:
```csharp
builder.Services.AddSwaggerGen(c =>
{
    // ... configuração existente ...

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
```

3. Adicione comentários nos controllers:
```csharp
/// <summary>
/// Cria uma nova notícia
/// </summary>
/// <param name="noticiaDto">Dados da notícia</param>
/// <returns>Notícia criada</returns>
/// <response code="201">Notícia criada com sucesso</response>
/// <response code="400">Dados inválidos</response>
[HttpPost]
[ProducesResponseType(typeof(NoticiaDto), StatusCodes.Status201Created)]
public async Task<ActionResult<NoticiaDto>> Create([FromForm] NoticiaCreateDto noticiaDto)
```

## 📱 Acessar Swagger de Outro Dispositivo

Para testar de celular ou tablet na mesma rede:

1. Encontre seu IP local:
```powershell
# Windows
ipconfig

# Linux/Mac
ifconfig
```

2. Configure URLs em `launchSettings.json`:
```json
"applicationUrl": "https://localhost:5001;https://192.168.1.100:5001"
```

3. Acesse de outro dispositivo:
```
https://192.168.1.100:5001/swagger
```

**Nota:** Pode ter aviso de certificado, pois é certificado de desenvolvimento.

## 🎨 Tema Escuro no Swagger

O Swagger UI usa tema claro por padrão. Para mudar:

```csharp
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auria API v1");
    c.RoutePrefix = "swagger";
    c.DefaultModelsExpandDepth(-1); // Ocultar schemas por padrão
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Colapsar tudo
});
```

## ✅ Checklist de Verificação

- [ ] Aplicação está rodando (`dotnet run`)
- [ ] Sem erros no console
- [ ] URL correta: https://localhost:5001/swagger
- [ ] Navegador atualizado
- [ ] Cache limpo se necessário
- [ ] Certificado SSL confiável (`dotnet dev-certs https --trust`)
- [ ] Firewall não está bloqueando

## 📞 Ainda com Problemas?

1. **Verifique os logs:**
```bash
# Ver logs em tempo real
Get-Content "logs/auria-dev-$(Get-Date -Format 'yyyyMMdd').log" -Wait
```

2. **Rebuild completo:**
```bash
dotnet clean
dotnet restore
dotnet build
cd Auria.API
dotnet run
```

3. **Verificar versão do .NET:**
```bash
dotnet --version
# Deve ser 8.0.x
```

4. **Testar endpoint diretamente:**
```bash
curl https://localhost:5001/swagger/v1/swagger.json -k
# Deve retornar JSON com definição da API
```

Se após tudo isso ainda não funcionar, verifique os logs detalhados e o console onde a aplicação está rodando para mensagens de erro específicas.
