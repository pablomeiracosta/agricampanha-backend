# Guia de Segurança - Auria API

## Checklist de Segurança

### ✅ Implementado

#### Autenticação e Autorização
- [x] JWT Bearer Token com claims
- [x] Validação de token com issuer/audience
- [x] Expiração configurável de tokens
- [x] Atributo [Authorize] em endpoints sensíveis
- [x] Hash de senhas com BCrypt (11 rounds)
- [x] Senhas nunca armazenadas em plain text

#### Validação de Dados
- [x] FluentValidation para todos os DTOs
- [x] Validação de tipos de arquivo (upload)
- [x] Validação de tamanho de campos
- [x] Validação de formatos de data
- [x] Validação de enums

#### Proteção contra Vulnerabilidades
- [x] Proteção contra SQL Injection (Entity Framework)
- [x] CORS configurável por ambiente
- [x] HTTPS obrigatório (RedirectHttps)
- [x] Validação de input em todos os endpoints

#### Logs e Monitoramento
- [x] Serilog estruturado
- [x] Logs de tentativas de login
- [x] Logs de operações críticas
- [x] Separação de logs por ambiente

### 🔒 Recomendações de Produção

#### 1. Configurações Sensíveis

**❌ NÃO FAZER:**
```json
{
  "Jwt": {
    "SecretKey": "123456"
  }
}
```

**✅ FAZER:**
```json
{
  "Jwt": {
    "SecretKey": "chave-segura-gerada-com-openssl-ou-cryptographyrng-32-chars-minimo"
  }
}
```

Gerar chave segura:
```powershell
# PowerShell
$bytes = New-Object Byte[] 32
[Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

```bash
# Linux/Mac
openssl rand -base64 32
```

#### 2. Connection String

**❌ NÃO FAZER:**
```json
{
  "ConnectionString": "Server=prod;Database=Agricampanha;User Id=sa;Password=123456;"
}
```

**✅ FAZER:**
```json
{
  "ConnectionString": "Server=prod;Database=Agricampanha;User Id=app_user;Password=SenhaForte123!;Encrypt=true;TrustServerCertificate=false;"
}
```

Melhor ainda: Use Azure Key Vault ou AWS Secrets Manager

#### 3. CORS em Produção

**❌ NÃO FAZER:**
```csharp
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

**✅ FAZER:**
```csharp
policy.WithOrigins("https://app.agricampanha.com.br")
      .WithMethods("GET", "POST", "PUT", "DELETE")
      .WithHeaders("Authorization", "Content-Type");
```

#### 4. Ambiente de Produção

Certifique-se de que `ASPNETCORE_ENVIRONMENT` está configurado como `Production`:

```bash
$env:ASPNETCORE_ENVIRONMENT = "Production"
```

## Vulnerabilidades Comuns e Proteções

### 1. SQL Injection
**Status:** ✅ Protegido

O Entity Framework usa queries parametrizadas automaticamente.

**Exemplo seguro:**
```csharp
await _context.Usuarios.FirstOrDefaultAsync(u => u.Login == login);
```

### 2. XSS (Cross-Site Scripting)
**Status:** ✅ Protegido

ASP.NET Core escapa automaticamente output em views. Para API, o frontend deve sanitizar.

**Recomendação frontend:**
```javascript
// Use DOMPurify ou similar
import DOMPurify from 'dompurify';
const clean = DOMPurify.sanitize(dirty);
```

### 3. CSRF (Cross-Site Request Forgery)
**Status:** ✅ Não aplicável

APIs stateless com JWT não precisam de proteção CSRF. Tokens JWT no header são seguros.

### 4. Broken Authentication
**Status:** ✅ Protegido

- Tokens JWT com expiração
- Senhas com BCrypt
- Validação de credenciais

**Melhorias futuras:**
- [ ] Refresh tokens
- [ ] Multi-factor authentication (MFA)
- [ ] Account lockout após tentativas falhas
- [ ] Password complexity requirements

### 5. Sensitive Data Exposure
**Status:** ⚠️ Atenção necessária

**Implementado:**
- HTTPS obrigatório
- Senhas hasheadas
- Logs não contêm senhas

**A fazer:**
- [ ] Criptografar dados sensíveis no banco
- [ ] Usar Azure Key Vault para secrets
- [ ] Implementar data masking em logs

### 6. Broken Access Control
**Status:** ✅ Protegido

Endpoints protegidos com `[Authorize]`. Endpoints públicos explicitamente marcados com `[AllowAnonymous]`.

**Melhorias futuras:**
- [ ] Implementar roles/permissions
- [ ] Policy-based authorization
- [ ] Resource-based authorization

### 7. Security Misconfiguration
**Status:** ⚠️ Revisar

**Checklist:**
- [ ] Remover header "Server" das respostas
- [ ] Configurar Content Security Policy (CSP)
- [ ] Configurar X-Frame-Options
- [ ] Configurar X-Content-Type-Options
- [ ] Desabilitar directory browsing
- [ ] Remover informações de debug em produção

**Implementar:**
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Remove("Server");
    await next();
});
```

### 8. Injection Attacks
**Status:** ✅ Protegido

- Entity Framework previne SQL Injection
- FluentValidation previne invalid input
- File upload validado

**Melhorias:**
- [ ] Validar MIME types de upload
- [ ] Scan de antivírus em uploads
- [ ] Limitar tamanho de arquivos

### 9. Insufficient Logging & Monitoring
**Status:** ✅ Implementado

Serilog registra:
- Tentativas de login
- Operações CRUD
- Erros e exceções

**Melhorias:**
- [ ] Integrar com SIEM
- [ ] Alertas automáticos
- [ ] Métricas de performance
- [ ] Audit trail completo

### 10. Using Components with Known Vulnerabilities
**Status:** ⚠️ Monitoramento contínuo

**Ações:**
```bash
# Verificar vulnerabilidades
dotnet list package --vulnerable

# Atualizar pacotes
dotnet outdated
```

**Automatizar:**
- [ ] Dependabot no GitHub
- [ ] Renovate Bot
- [ ] Snyk

## Rate Limiting (A Implementar)

Protege contra ataques de força bruta e DDoS.

```csharp
// Adicionar ao Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
    });
});

app.UseRateLimiter();
```

Uso:
```csharp
[RateLimit("fixed")]
[HttpPost("login")]
public async Task<ActionResult> Login(...) { }
```

## Account Lockout (A Implementar)

Previne ataques de força bruta.

```csharp
// Em Usuario entity, adicionar:
public int FailedLoginAttempts { get; set; }
public DateTime? LockoutEnd { get; set; }

// No AuthService:
if (usuario.LockoutEnd.HasValue && usuario.LockoutEnd > DateTime.UtcNow)
{
    return new LoginResponseDto
    {
        Success = false,
        Message = "Conta bloqueada. Tente novamente mais tarde."
    };
}

if (senhaInvalida)
{
    usuario.FailedLoginAttempts++;
    if (usuario.FailedLoginAttempts >= 5)
    {
        usuario.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
    }
    await _usuarioRepository.UpdateAsync(usuario);
}
else
{
    usuario.FailedLoginAttempts = 0;
    usuario.LockoutEnd = null;
}
```

## Segurança de Upload de Arquivos

### Validações Implementadas
1. Extensão de arquivo (jpg, jpeg, png, gif)
2. FluentValidation

### Validações a Adicionar

```csharp
public class SecureFileUploadValidator
{
    public static async Task<bool> IsValidImageAsync(IFormFile file)
    {
        // 1. Validar MIME type
        var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
        if (!allowedMimeTypes.Contains(file.ContentType))
            return false;

        // 2. Validar tamanho (max 5MB)
        if (file.Length > 5 * 1024 * 1024)
            return false;

        // 3. Validar assinatura do arquivo (magic numbers)
        using var stream = file.OpenReadStream();
        var header = new byte[8];
        await stream.ReadAsync(header, 0, header.Length);

        // JPEG: FF D8 FF
        // PNG: 89 50 4E 47
        // GIF: 47 49 46 38
        bool isJpeg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
        bool isPng = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;
        bool isGif = header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38;

        return isJpeg || isPng || isGif;
    }
}
```

## Auditoria

### Implementar Tabela de Auditoria

```csharp
public class AuditLog
{
    public int Id { get; set; }
    public string Action { get; set; }
    public string Entity { get; set; }
    public int? EntityId { get; set; }
    public string UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Changes { get; set; } // JSON
}
```

## Compliance

### LGPD (Lei Geral de Proteção de Dados)

**Implementar:**
1. Consent management
2. Right to be forgotten
3. Data portability
4. Privacy policy
5. Terms of service
6. Cookie consent

### OWASP Top 10

Revisão periódica contra: https://owasp.org/www-project-top-ten/

## Backup e Recuperação

### Estratégia de Backup

```sql
-- Backup completo diário
BACKUP DATABASE [Agricampanha]
TO DISK = 'C:\Backups\Full\Agricampanha_Full.bak'
WITH INIT, COMPRESSION;

-- Backup diferencial a cada 6 horas
BACKUP DATABASE [Agricampanha]
TO DISK = 'C:\Backups\Diff\Agricampanha_Diff.bak'
WITH DIFFERENTIAL, COMPRESSION;

-- Backup de log a cada hora
BACKUP LOG [Agricampanha]
TO DISK = 'C:\Backups\Log\Agricampanha_Log.bak'
WITH COMPRESSION;
```

### Teste de Recuperação

Testar recuperação mensalmente:
```sql
RESTORE DATABASE [Agricampanha_Test]
FROM DISK = 'C:\Backups\Full\Agricampanha_Full.bak'
WITH REPLACE, MOVE 'Agricampanha' TO 'C:\Data\Test.mdf';
```

## Incident Response Plan

### Em caso de incidente:

1. **Detectar:** Monitorar logs e alertas
2. **Conter:** Isolar sistema afetado
3. **Erradicar:** Remover ameaça
4. **Recuperar:** Restaurar operações normais
5. **Aprender:** Post-mortem e melhorias

### Contatos de Emergência

- DBA: [contato]
- DevOps: [contato]
- Segurança: [contato]
- Legal: [contato]

## Recursos Adicionais

- OWASP Top 10: https://owasp.org/www-project-top-ten/
- Microsoft Security Best Practices: https://docs.microsoft.com/security
- NIST Cybersecurity Framework: https://www.nist.gov/cyberframework
- CWE Top 25: https://cwe.mitre.org/top25/

## Revisão de Segurança

Realizar revisão de segurança:
- [ ] Semanalmente: Verificar logs de erro
- [ ] Mensalmente: Atualizar dependências
- [ ] Trimestralmente: Penetration testing
- [ ] Anualmente: Security audit completo
