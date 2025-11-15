# Serviço de Envio de Email - Gmail API

## ✅ Implementação Concluída

O sistema de envio de emails foi implementado usando a **Gmail API** (não SMTP), oferecendo maior segurança e confiabilidade.

## 📁 Arquivos Criados

### 1. Services e Interfaces
- **[IEmailService.cs](Auria.Bll/Services/Interfaces/IEmailService.cs)** - Interface do serviço de email
- **[GmailService.cs](Auria.Bll/Services/GmailService.cs)** - Implementação usando Gmail API

### 2. DTOs
- **[EmailSendDto.cs](Auria.Dto/Email/EmailSendDto.cs)** - DTOs para envio de emails

### 3. Controllers
- **[EmailController.cs](Auria.API/Controllers/EmailController.cs)** - Endpoints para envio de emails

### 4. Configurações
- **[AppSettings.cs](Auria.Structure/Configuration/AppSettings.cs)** - Adicionado `GmailSettings`
- **[appsettings.Development.json](Auria.API/appsettings.Development.json)** - Configuração do Gmail

### 5. Documentação
- **[GMAIL-API-SETUP.md](GMAIL-API-SETUP.md)** - Guia completo de configuração do Gmail API

## 📍 Endpoints Disponíveis

### 1. POST /api/email/send
Envia um email para um destinatário.

**Autenticação:** JWT Bearer Token

**Request:**
```json
{
  "to": "destinatario@example.com",
  "subject": "Assunto do Email",
  "body": "<h1>Email HTML</h1><p>Conteúdo do email</p>",
  "isHtml": true
}
```

**Response (200 OK):**
```json
{
  "message": "Email enviado com sucesso",
  "to": "destinatario@example.com"
}
```

---

### 2. POST /api/email/send/multiple
Envia um email para múltiplos destinatários.

**Autenticação:** JWT Bearer Token

**Request:**
```json
{
  "toList": [
    "pessoa1@example.com",
    "pessoa2@example.com",
    "pessoa3@example.com"
  ],
  "subject": "Assunto do Email",
  "body": "<h1>Email em Massa</h1>",
  "isHtml": true
}
```

**Response (200 OK):**
```json
{
  "message": "Email enviado com sucesso",
  "recipients": 3,
  "toList": ["pessoa1@example.com", "pessoa2@example.com", "pessoa3@example.com"]
}
```

---

### 3. POST /api/email/test
Envia um email de teste com template HTML pré-definido.

**Autenticação:** JWT Bearer Token

**Request:**
```http
POST /api/email/test?to=seu-email@example.com
```

**Response (200 OK):**
```json
{
  "message": "Email de teste enviado com sucesso",
  "to": "seu-email@example.com"
}
```

## 🔧 Tecnologias Utilizadas

- **Google.Apis.Gmail.v1** (v1.70.0.3833) - SDK oficial do Gmail
- **OAuth 2.0** - Autenticação segura
- **MIME Format** - Formato padrão de email
- **Base64 URL-safe** - Codificação necessária pela Gmail API

## ⚙️ Configuração Necessária

### 1. Obter Credenciais do Gmail API

Siga o guia completo em **[GMAIL-API-SETUP.md](GMAIL-API-SETUP.md)** que detalha:

1. Como criar projeto no Google Cloud Console
2. Como habilitar a Gmail API
3. Como criar credenciais OAuth 2.0
4. Como baixar o arquivo de credenciais
5. Como configurar no projeto

### 2. Atualizar appsettings.Development.json

```json
{
  "Gmail": {
    "FromEmail": "seu-email@gmail.com",
    "CredentialsPath": "caminho/para/gmail-credentials.json",
    "CredentialsJson": ""
  }
}
```

**Opções de Configuração:**
- **CredentialsPath:** Caminho para o arquivo JSON de credenciais (recomendado)
- **CredentialsJson:** JSON direto na configuração (não recomendado para produção)

## 🔒 Segurança

✅ Todos os endpoints requerem autenticação JWT
✅ Validação de emails com System.Net.Mail.MailAddress
✅ Logging completo de todas as operações
✅ Credenciais não são expostas nos logs ou responses
✅ Suporte a OAuth 2.0 (mais seguro que SMTP)

## 📊 Funcionalidades

✅ Envio de email para destinatário único
✅ Envio de email para múltiplos destinatários
✅ Suporte a HTML e texto plano
✅ Validação de formato de email
✅ Endpoint de teste com template HTML
✅ Logging detalhado com Serilog
✅ Tratamento de erros e exceptions

## 🚀 Como Usar

### Exemplo 1: Enviar Email Simples

```csharp
// No código C#
await _emailService.SendEmailAsync(
    "cliente@example.com",
    "Bem-vindo ao Sistema Auria",
    "<h1>Olá!</h1><p>Seja bem-vindo!</p>",
    isHtml: true
);
```

```bash
# Via API
curl -X POST "http://localhost:5000/api/email/send" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "to": "cliente@example.com",
    "subject": "Bem-vindo ao Sistema Auria",
    "body": "<h1>Olá!</h1><p>Seja bem-vindo!</p>",
    "isHtml": true
  }'
```

### Exemplo 2: Enviar para Múltiplos Destinatários

```bash
curl -X POST "http://localhost:5000/api/email/send/multiple" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "toList": ["email1@example.com", "email2@example.com"],
    "subject": "Newsletter Semanal",
    "body": "<h1>Novidades desta semana</h1>",
    "isHtml": true
  }'
```

### Exemplo 3: Enviar Email de Teste

```bash
curl -X POST "http://localhost:5000/api/email/test?to=seu-email@example.com" \
  -H "Authorization: Bearer {token}"
```

## ⚠️ Limitações

- **Gmail gratuito:** 100-150 emails por dia
- **Google Workspace:** 2.000 emails por dia
- **Taxa de envio:** Máximo de 10 emails/segundo (recomendado: 1-2/segundo)

Para volumes maiores, considere:
- SendGrid
- AWS SES
- Mailgun
- Postmark

## 🧪 Testando a Implementação

### 1. Login

```bash
curl -X POST http://localhost:5000/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"login":"admin","senha":"admin123"}'
```

### 2. Copiar o Token

Copie o valor do campo `token` da resposta.

### 3. Enviar Email de Teste

```bash
curl -X POST "http://localhost:5000/api/email/test?to=seu-email@gmail.com" \
  -H "Authorization: Bearer {SEU_TOKEN_AQUI}"
```

### 4. Verificar Logs

Os logs estarão disponíveis em `logs/auria-dev-.log` com informações detalhadas sobre o envio.

## 📝 Logs

Todos os envios de email são registrados:

```
[INF] Solicitação de envio de email para: cliente@example.com
[INF] Iniciando envio de email para 1 destinatário(s)
[INF] Email enviado com sucesso. ID: 18f23a4b5c6d7e8f
[INF] Email enviado com sucesso para: cliente@example.com
```

Em caso de erro:

```
[ERR] Erro ao enviar email para: cliente@example.com
System.Exception: Erro ao fazer upload da imagem: Invalid Credentials
```

## 🔄 Integração com Outros Módulos

O serviço de email pode ser facilmente integrado com:

- **Notícias:** Enviar notificações de novas publicações
- **Usuários:** Emails de boas-vindas, recuperação de senha
- **Newsletters:** Envio em massa para assinantes
- **Alertas:** Notificações automáticas do sistema

## 📚 Próximos Passos

Para usar o serviço:

1. ✅ Leia o [GMAIL-API-SETUP.md](GMAIL-API-SETUP.md) para configurar as credenciais
2. ✅ Configure o `appsettings.Development.json` com suas credenciais
3. ✅ Teste usando o endpoint `/api/email/test`
4. ✅ Integre o `IEmailService` nos seus controllers ou services

## 🆘 Suporte

Em caso de problemas:

1. Verifique se a Gmail API está habilitada no Google Cloud Console
2. Confirme que as credenciais estão corretas
3. Verifique os logs em `logs/auria-dev-.log`
4. Consulte o [GMAIL-API-SETUP.md](GMAIL-API-SETUP.md) para troubleshooting detalhado
