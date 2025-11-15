# Troubleshooting do Swagger

## Checklist Rápido

### 1. A aplicação está rodando?

No console onde você executou `dotnet run`, você deve ver:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

**Se NÃO vê essas mensagens:**
- Há erros no console?
- A aplicação travou?
- Tente fechar e executar novamente

### 2. Teste as URLs corretas

Tente EXATAMENTE estas URLs (uma de cada vez):

```
http://localhost:5000/swagger
https://localhost:5001/swagger
http://localhost:5000/swagger/index.html
https://localhost:5001/swagger/index.html
```

### 3. Verifique se há erros no console

**Erros comuns:**

#### Erro: "Failed to bind to address"
```
Failed to bind to address https://127.0.0.1:5001: address already in use
```

**Solução:** Porta em uso, mate o processo:
```powershell
netstat -ano | findstr :5001
taskkill /PID <número> /F
```

#### Erro: Connection String
```
A network-related or instance-specific error occurred
```

**Solução:** SQL Server não está rodando ou connection string incorreta

#### Erro: Serilog/Logging
```
The type initializer for 'Serilog.Log' threw an exception
```

**Solução:** Problema com caminho do log, crie a pasta:
```powershell
mkdir logs
```

### 4. Certificado SSL

Se o navegador mostrar aviso de certificado:

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Ou simplesmente use HTTP: http://localhost:5000/swagger

### 5. Firewall/Antivírus

Verifique se o Firewall ou Antivírus não está bloqueando a porta 5000 ou 5001.

**Windows Firewall:**
```powershell
# Adicionar regra para porta 5000
netsh advfirewall firewall add rule name="Auria API HTTP" dir=in action=allow protocol=TCP localport=5000

# Adicionar regra para porta 5001
netsh advfirewall firewall add rule name="Auria API HTTPS" dir=in action=allow protocol=TCP localport=5001
```

### 6. Navegador

- Limpe o cache (Ctrl + Shift + Del)
- Tente modo anônimo/privado
- Tente outro navegador
- Desabilite extensões (AdBlock, etc.)

### 7. Verificar se Swagger está configurado

Execute na pasta do projeto:

```powershell
cd Auria.API
dotnet run --urls "http://localhost:5000"
```

Então acesse: http://localhost:5000/swagger

## Testes Manuais

### Teste 1: API está respondendo?

```powershell
Invoke-WebRequest -Uri "http://localhost:5000/swagger/v1/swagger.json" -UseBasicParsing
```

**Resultado esperado:** StatusCode 200

### Teste 2: Swagger UI está acessível?

```powershell
Invoke-WebRequest -Uri "http://localhost:5000/swagger/index.html" -UseBasicParsing
```

**Resultado esperado:** HTML do Swagger UI

### Teste 3: Endpoint simples

Se o Swagger não funcionar, teste se a API está respondendo:

```powershell
Invoke-WebRequest -Uri "http://localhost:5000/api/noticias" -UseBasicParsing
```

## Erros Específicos

### "This site can't be reached"

**Causa:** Aplicação não está rodando ou porta errada

**Solução:**
1. Verifique se `dotnet run` está executando
2. Veja se não há erros no console
3. Confirme as portas em `launchSettings.json`

### "Your connection is not private" (certificado)

**Causa:** Certificado SSL não confiável

**Solução:**
1. Clique em "Advanced" → "Proceed to localhost (unsafe)"
2. Ou confie no certificado: `dotnet dev-certs https --trust`
3. Ou use HTTP: http://localhost:5000/swagger

### Página em branco

**Causa:** JavaScript não carregou

**Solução:**
1. Abra Developer Tools (F12)
2. Veja erros no Console
3. Verifique se há bloqueio de scripts (CSP)
4. Limpe cache e tente novamente

### 404 Not Found

**Causa:** Rota incorreta ou Swagger não configurado

**Solução:**
1. Verifique se digitou: /swagger (não /swagger-ui)
2. Verifique Program.cs tem UseSwagger() e UseSwaggerUI()
3. Tente: http://localhost:5000/index.html

## Diagnóstico Completo

Execute este script para diagnóstico completo:

```powershell
Write-Host "=== DIAGNÓSTICO SWAGGER ===" -ForegroundColor Cyan

# 1. Verificar .NET
Write-Host "`n1. Versão do .NET:" -ForegroundColor Yellow
dotnet --version

# 2. Verificar se está compilando
Write-Host "`n2. Compilando projeto:" -ForegroundColor Yellow
dotnet build --no-restore

# 3. Verificar portas
Write-Host "`n3. Portas em uso:" -ForegroundColor Yellow
netstat -ano | findstr ":5000 :5001"

# 4. Testar conexão
Write-Host "`n4. Testando conexão:" -ForegroundColor Yellow
Test-NetConnection -ComputerName localhost -Port 5000

# 5. Ver logs
Write-Host "`n5. Últimas linhas do log:" -ForegroundColor Yellow
Get-Content "Auria.API\logs\auria-dev-*.log" -Tail 20 -ErrorAction SilentlyContinue
```

## Última Alternativa

Se NADA funcionar, recrie o projeto Program.cs com configuração mínima:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
```

Salve como `ProgramMinimal.cs` e teste se funciona.

## Contato para Suporte

Se após TODOS esses passos o Swagger ainda não funcionar:

1. Copie TODO o output do console (incluindo erros)
2. Copie o erro do navegador (F12 → Console)
3. Informe qual teste funcionou/não funcionou
4. Anexe screenshot do erro

---

**Na maioria dos casos, o problema é:**
1. ✅ Aplicação não está rodando
2. ✅ URL incorreta (faltou /swagger)
3. ✅ Certificado SSL não confiável
4. ✅ Firewall bloqueando porta
5. ✅ Cache do navegador

**Tente primeiro os itens acima!**
