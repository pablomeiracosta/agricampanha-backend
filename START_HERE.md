# 🚀 COMECE AQUI - Auria API

## ✅ Correção do Swagger Aplicada!

O Swagger foi corrigido e agora funciona em todos os ambientes.

## 📋 Início Rápido (3 Passos)

### 1️⃣ Configurar Connection String

Edite `Auria.API/appsettings.json`:

```json
{
  "ConnectionString": "Server=localhost;Database=Agricampanha;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2️⃣ Criar Banco de Dados

**Opção A - Script PowerShell (Mais Fácil):**
```powershell
.\setup-database.ps1
```

**Opção B - Manual:**
```bash
cd Auria.API
dotnet ef migrations add InitialCreate --project ..\Auria.Data
dotnet ef database update --project ..\Auria.Data
```

### 3️⃣ Iniciar a API

**Opção A - Script PowerShell:**
```powershell
.\start.ps1
```

**Opção B - Manual:**
```bash
cd Auria.API
dotnet run
```

## 🌐 Acessar o Swagger

Abra seu navegador em:
```
https://localhost:5001/swagger
```

## 🔐 Credenciais Padrão

- **Login:** admin
- **Senha:** admin123

## 📚 Documentação Completa

| Documento | Para que serve |
|-----------|---------------|
| [SWAGGER_SETUP.md](SWAGGER_SETUP.md) | Guia completo do Swagger e troubleshooting |
| [QUICK_START.md](QUICK_START.md) | Guia rápido de inicialização |
| [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md) | Visão geral do projeto |
| [README.md](README.md) | Documentação completa da API |
| [INDEX.md](INDEX.md) | Índice de toda documentação |

## ❓ Problemas Comuns

### Swagger não abre
1. Verifique se a aplicação está rodando
2. Acesse: https://localhost:5001/swagger
3. Se problema persistir, veja [SWAGGER_SETUP.md](SWAGGER_SETUP.md)

### Erro de certificado SSL
```bash
dotnet dev-certs https --trust
```

### Porta em uso
```powershell
netstat -ano | findstr :5001
taskkill /PID <PID> /F
```

### Erro ao conectar no banco
1. Verifique se SQL Server está rodando
2. Confira a connection string em appsettings.json

## 🎯 Próximos Passos

1. ✅ Criar banco de dados
2. ✅ Iniciar aplicação
3. ✅ Acessar Swagger
4. ✅ Fazer login no Swagger
5. ✅ Testar endpoints
6. 📖 Ler documentação completa
7. 🔒 Revisar [SECURITY.md](SECURITY.md) antes de produção

## 💡 Dicas

- Use o Swagger para testar todos os endpoints
- Consulte [COMMANDS.md](COMMANDS.md) para comandos úteis
- Veja [ARCHITECTURE.md](ARCHITECTURE.md) para entender a estrutura
- Leia [SECURITY.md](SECURITY.md) antes do deploy

---

**Desenvolvido para Agricampanha**
**Versão: 1.0.0**
