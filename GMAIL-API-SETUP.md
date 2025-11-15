# Configuração do Gmail API para Envio de Emails

## 📋 Visão Geral

O sistema utiliza a **Gmail API** (não SMTP) para envio de emails. Isso oferece várias vantagens:
- ✅ Maior segurança (OAuth 2.0)
- ✅ Melhor taxa de entrega
- ✅ Sem necessidade de habilitar "aplicativos menos seguros"
- ✅ Controle granular de permissões

## 🔧 Passo a Passo para Configuração

### 1. Criar Projeto no Google Cloud Console

1. Acesse: https://console.cloud.google.com
2. Clique em **"Criar Projeto"** ou selecione um projeto existente
3. Nomeie o projeto (ex: "Auria Email Service")
4. Clique em **"Criar"**

### 2. Habilitar a Gmail API

1. No menu lateral, vá em **"APIs e serviços"** > **"Biblioteca"**
2. Pesquise por **"Gmail API"**
3. Clique em **"Gmail API"**
4. Clique em **"Ativar"**

### 3. Criar Credenciais OAuth 2.0

1. No menu lateral, vá em **"APIs e serviços"** > **"Credenciais"**
2. Clique em **"Criar credenciais"** > **"ID do cliente OAuth"**
3. Se solicitado, configure a tela de consentimento OAuth:
   - Escolha **"Externo"** como tipo de usuário
   - Preencha os campos obrigatórios:
     - Nome do aplicativo: "Auria API"
     - Email de suporte: seu-email@gmail.com
     - Email do desenvolvedor: seu-email@gmail.com
   - Clique em **"Salvar e continuar"**
   - Em **"Escopos"**, clique em **"Adicionar ou remover escopos"**
   - Adicione o escopo: `https://www.googleapis.com/auth/gmail.send`
   - Clique em **"Salvar e continuar"**
   - Em **"Usuários de teste"**, adicione o email que enviará os emails
   - Clique em **"Salvar e continuar"**

4. Volte para **"Credenciais"** e clique novamente em **"Criar credenciais"** > **"ID do cliente OAuth"**
5. Escolha **"Aplicativo para computador"**
6. Nomeie (ex: "Auria Desktop Client")
7. Clique em **"Criar"**

### 4. Baixar o Arquivo de Credenciais

1. Após criar, clique no ícone de **download** (seta para baixo) ao lado das credenciais criadas
2. Salve o arquivo JSON (será algo como `client_secret_XXXXX.json`)
3. Renomeie para `gmail-credentials.json`

### 5. Gerar Token de Acesso (Primeira Vez)

Como a Gmail API usa OAuth 2.0, você precisa autorizar o aplicativo uma vez. Existem duas opções:

#### Opção A: Usar o Arquivo JSON Diretamente (Recomendado para Produção)

Para usar uma conta de serviço (service account), siga estes passos:

1. No Google Cloud Console, vá em **"IAM e administrador"** > **"Contas de serviço"**
2. Clique em **"Criar conta de serviço"**
3. Preencha:
   - Nome: "Auria Email Service"
   - ID: auria-email-service
4. Clique em **"Criar e continuar"**
5. Selecione a função: **"Editor de projeto"** (ou crie uma função personalizada)
6. Clique em **"Concluído"**
7. Clique na conta de serviço criada
8. Vá na aba **"Chaves"**
9. Clique em **"Adicionar chave"** > **"Criar nova chave"**
10. Escolha **JSON**
11. O arquivo JSON será baixado automaticamente

**IMPORTANTE:** Para contas de serviço funcionarem com Gmail API, você precisa configurar a delegação de domínio no Google Workspace (se aplicável).

#### Opção B: Usar OAuth com Conta de Usuário (Mais Simples para Desenvolvimento)

Para desenvolvimento, é mais fácil usar uma conta de usuário normal. Você precisará fazer a autenticação interativa uma vez.

**Nota:** O código atual está preparado para usar o JSON de credenciais. Para autenticação interativa, seria necessário implementar um fluxo adicional.

### 6. Configurar no appsettings.Development.json

Abra o arquivo `appsettings.Development.json` e configure:

```json
{
  "Gmail": {
    "FromEmail": "seu-email@gmail.com",
    "CredentialsPath": "caminho/para/gmail-credentials.json",
    "CredentialsJson": ""
  }
}
```

**OU** se preferir colocar o JSON diretamente na configuração (não recomendado para produção):

```json
{
  "Gmail": {
    "FromEmail": "seu-email@gmail.com",
    "CredentialsPath": "",
    "CredentialsJson": "{\"type\":\"service_account\",\"project_id\":\"seu-projeto\", ...}"
  }
}
```

### 7. Estrutura de Arquivos

```
backend/
├── Auria.API/
│   ├── appsettings.Development.json   ← Configuração aqui
│   └── gmail-credentials.json         ← Credenciais aqui (não commitar!)
```

**IMPORTANTE:** Adicione `gmail-credentials.json` ao `.gitignore` para não commitar credenciais!

## 📍 Endpoints Disponíveis

### 1. Enviar Email para Um Destinatário
```http
POST /api/email/send
Authorization: Bearer {token}
Content-Type: application/json

{
  "to": "destinatario@example.com",
  "subject": "Assunto do Email",
  "body": "<h1>Email HTML</h1><p>Conteúdo do email</p>",
  "isHtml": true
}
```

### 2. Enviar Email para Múltiplos Destinatários
```http
POST /api/email/send/multiple
Authorization: Bearer {token}
Content-Type: application/json

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

### 3. Enviar Email de Teste
```http
POST /api/email/test?to=seu-email@example.com
Authorization: Bearer {token}
```

## 🔒 Segurança

1. **Nunca commite** o arquivo `gmail-credentials.json` no repositório
2. **Use variáveis de ambiente** em produção para armazenar credenciais
3. **Limite os escopos** da Gmail API apenas ao necessário (gmail.send)
4. **Monitore o uso** através do Google Cloud Console
5. **Proteja os endpoints** com autenticação JWT (já implementado)

## ⚠️ Limitações da Gmail API

- **Limite de envio:** 100-150 emails por dia para contas gratuitas do Gmail
- **Limite de envio:** 2.000 emails por dia para Google Workspace
- **Taxa de envio:** Máximo de 10 emails por segundo (recomendado: 1-2 por segundo)

Se precisar enviar mais emails, considere usar serviços como:
- SendGrid
- AWS SES
- Mailgun
- Postmark

## 🧪 Testando a Integração

1. **Faça login** na API:
```bash
curl -X POST http://localhost:5000/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"login":"admin","senha":"admin123"}'
```

2. **Copie o token** retornado

3. **Envie um email de teste**:
```bash
curl -X POST "http://localhost:5000/api/email/test?to=seu-email@gmail.com" \
  -H "Authorization: Bearer {SEU_TOKEN}"
```

## 📝 Exemplo de Uso no Código

```csharp
// Injetar o serviço
private readonly IEmailService _emailService;

// Enviar email simples
await _emailService.SendEmailAsync(
    "destinatario@example.com",
    "Bem-vindo ao Sistema",
    "<h1>Olá!</h1><p>Seja bem-vindo ao nosso sistema.</p>",
    isHtml: true
);

// Enviar para múltiplos destinatários
await _emailService.SendEmailAsync(
    new List<string> { "email1@example.com", "email2@example.com" },
    "Newsletter Semanal",
    "<h1>Novidades desta semana</h1>",
    isHtml: true
);
```

## 🆘 Solução de Problemas

### Erro: "Invalid Credentials"
- Verifique se o arquivo JSON está no caminho correto
- Confirme que o email em `FromEmail` corresponde à conta autorizada
- Verifique se a Gmail API está habilitada no projeto

### Erro: "Insufficient Permission"
- Certifique-se de que adicionou o escopo `gmail.send`
- Reautorize a aplicação se mudou os escopos

### Erro: "User Rate Limit Exceeded"
- Você atingiu o limite de envio diário
- Aguarde 24 horas ou use uma conta Google Workspace

### Emails não chegam
- Verifique a pasta de Spam/Lixo Eletrônico
- Confirme que o email "From" está verificado
- Verifique os logs da aplicação para erros

## 📚 Recursos Adicionais

- [Gmail API Documentation](https://developers.google.com/gmail/api)
- [OAuth 2.0 Overview](https://developers.google.com/identity/protocols/oauth2)
- [Gmail API Quotas](https://developers.google.com/gmail/api/reference/quota)
