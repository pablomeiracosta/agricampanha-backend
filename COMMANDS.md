# Comandos Úteis - Auria API

## Gerenciamento de Pacotes NuGet

### Restaurar Pacotes
```bash
dotnet restore
```

### Adicionar Pacote a um Projeto
```bash
dotnet add Auria.API package NomeDoPacote --version X.X.X
```

### Atualizar Todos os Pacotes
```bash
dotnet list package --outdated
dotnet add package NomeDoPacote
```

### Remover Pacote
```bash
dotnet remove Auria.API package NomeDoPacote
```

## Build e Execução

### Build da Solução
```bash
# Build em modo Debug
dotnet build

# Build em modo Release
dotnet build --configuration Release

# Build de projeto específico
dotnet build Auria.API/Auria.API.csproj
```

### Executar a API
```bash
# Desenvolvimento (com hot reload)
cd Auria.API
dotnet run

# Modo watch (recompila automaticamente)
dotnet watch run

# Especificar ambiente
dotnet run --environment Production
```

### Publicar para Deploy
```bash
# Publicar para pasta
dotnet publish -c Release -o ./publish

# Publicar self-contained (inclui runtime)
dotnet publish -c Release -r win-x64 --self-contained
```

## Entity Framework Core

### Migrations

#### Criar Nova Migration
```bash
cd Auria.API
dotnet ef migrations add NomeDaMigration --project ..\Auria.Data
```

#### Aplicar Migrations
```bash
# Aplicar todas as migrations pendentes
dotnet ef database update --project ..\Auria.Data

# Aplicar até uma migration específica
dotnet ef database update NomeDaMigration --project ..\Auria.Data

# Reverter todas as migrations
dotnet ef database update 0 --project ..\Auria.Data
```

#### Listar Migrations
```bash
dotnet ef migrations list --project ..\Auria.Data
```

#### Remover Última Migration
```bash
# Remove migration que ainda não foi aplicada
dotnet ef migrations remove --project ..\Auria.Data
```

#### Gerar Script SQL das Migrations
```bash
# Gerar script de todas as migrations
dotnet ef migrations script --project ..\Auria.Data -o migration.sql

# Gerar script entre migrations específicas
dotnet ef migrations script MigrationA MigrationB --project ..\Auria.Data -o script.sql
```

### Database

#### Dropar Banco de Dados
```bash
dotnet ef database drop --project ..\Auria.Data
```

#### Criar Banco de Dados
```bash
dotnet ef database update --project ..\Auria.Data
```

## Limpeza

### Limpar Artifacts de Build
```bash
# Limpar outputs de build
dotnet clean

# Limpar bin e obj manualmente
Get-ChildItem -Include bin,obj -Recurse | Remove-Item -Force -Recurse
```

### Remover Pacotes Não Utilizados
```bash
dotnet restore
```

## Testes (quando implementados)

### Executar Todos os Testes
```bash
dotnet test
```

### Executar Testes com Cobertura
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Executar Testes de Projeto Específico
```bash
dotnet test Auria.Tests/Auria.Tests.csproj
```

## Inspeção de Código

### Verificar Avisos
```bash
dotnet build /warnaserror
```

### Análise de Código
```bash
dotnet format
```

## Logs

### Ver Logs em Tempo Real (Windows)
```powershell
Get-Content "Auria.API/logs/auria-dev-$(Get-Date -Format 'yyyyMMdd').log" -Wait
```

### Ver Logs em Tempo Real (Linux/Mac)
```bash
tail -f Auria.API/logs/auria-dev-$(date +%Y%m%d).log
```

### Limpar Logs Antigos (PowerShell)
```powershell
Get-ChildItem -Path "Auria.API/logs" -Filter "*.log" |
    Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-30) } |
    Remove-Item
```

## Docker (Futuro)

### Construir Imagem
```bash
docker build -t auria-api:latest .
```

### Executar Container
```bash
docker run -d -p 5000:5000 -p 5001:5001 --name auria-api auria-api:latest
```

### Ver Logs do Container
```bash
docker logs -f auria-api
```

## SQL Server

### Backup do Banco
```sql
BACKUP DATABASE [Agricampanha]
TO DISK = 'C:\Backups\Agricampanha_Backup.bak'
WITH FORMAT, MEDIANAME = 'Agricampanha_Backup', NAME = 'Full Backup of Agricampanha';
```

### Restaurar Banco
```sql
RESTORE DATABASE [Agricampanha]
FROM DISK = 'C:\Backups\Agricampanha_Backup.bak'
WITH REPLACE;
```

### Verificar Tamanho do Banco
```sql
USE Agricampanha;
GO
EXEC sp_spaceused;
```

### Ver Conexões Ativas
```sql
USE Agricampanha;
GO
SELECT * FROM sys.dm_exec_sessions WHERE database_id = DB_ID('Agricampanha');
```

## Git

### Inicializar Repositório
```bash
git init
git add .
git commit -m "Initial commit - Auria API Backend"
```

### Branches
```bash
# Criar e mudar para nova branch
git checkout -b feature/nova-funcionalidade

# Voltar para main
git checkout main

# Merge de branch
git merge feature/nova-funcionalidade
```

### Tags
```bash
# Criar tag de versão
git tag -a v1.0.0 -m "Versão 1.0.0"

# Push de tags
git push origin --tags
```

## Cloudinary

### Testar Configuração (PowerShell)
```powershell
$headers = @{
    "Authorization" = "Basic {BASE64_API_KEY:API_SECRET}"
}
Invoke-RestMethod -Uri "https://api.cloudinary.com/v1_1/{CLOUD_NAME}/resources/image" -Headers $headers
```

## Performance

### Medir Tempo de Resposta
```bash
# Usando curl
curl -w "@curl-format.txt" -o /dev/null -s "https://localhost:5001/api/noticias"

# curl-format.txt:
# time_namelookup: %{time_namelookup}\n
# time_connect: %{time_connect}\n
# time_starttransfer: %{time_starttransfer}\n
# time_total: %{time_total}\n
```

### Load Testing com Apache Bench
```bash
ab -n 1000 -c 10 https://localhost:5001/api/noticias
```

## Segurança

### Gerar Chave JWT Segura (PowerShell)
```powershell
$bytes = New-Object Byte[] 32
[Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

### Gerar Chave JWT Segura (Linux/Mac)
```bash
openssl rand -base64 32
```

### Verificar Certificados SSL
```bash
dotnet dev-certs https --check
dotnet dev-certs https --trust
```

## Monitoramento

### Verificar Uso de Memória
```powershell
# Windows
Get-Process -Name "Auria.API" | Select-Object Name, CPU, WS

# Linux
ps aux | grep Auria.API
```

### Verificar Porta em Uso
```powershell
# Windows
netstat -ano | findstr :5001

# Linux
lsof -i :5001
```

## Utilitários

### Gerar Hash BCrypt para Senha
Use o seguinte código C#:
```csharp
var hash = BCrypt.Net.BCrypt.HashPassword("senha123");
Console.WriteLine(hash);
```

### Validar Token JWT Online
https://jwt.io/

### Testar Regex
https://regex101.com/

### Validar JSON
https://jsonlint.com/

## Troubleshooting

### Limpar Cache do NuGet
```bash
dotnet nuget locals all --clear
```

### Resetar Entity Framework
```bash
# Dropar banco e recriar
dotnet ef database drop --force --project ..\Auria.Data
dotnet ef database update --project ..\Auria.Data
```

### Verificar Versão do .NET
```bash
dotnet --version
dotnet --list-sdks
dotnet --list-runtimes
```

### Verificar Porta do SQL Server
```powershell
Test-NetConnection -ComputerName localhost -Port 1433
```

## Scripts de Produtividade

### Script PowerShell: Build e Run
```powershell
# build-run.ps1
dotnet build
if ($LASTEXITCODE -eq 0) {
    cd Auria.API
    dotnet run
}
```

### Script PowerShell: Reset Database
```powershell
# reset-db.ps1
cd Auria.API
dotnet ef database drop --force --project ..\Auria.Data
dotnet ef database update --project ..\Auria.Data
Write-Host "Database resetado com sucesso!"
```

### Script PowerShell: Criar Nova Migration
```powershell
# new-migration.ps1
param([string]$name)
if ([string]::IsNullOrEmpty($name)) {
    Write-Host "Uso: .\new-migration.ps1 NomeDaMigration"
    exit
}
cd Auria.API
dotnet ef migrations add $name --project ..\Auria.Data
```

## Ambiente de Desenvolvimento

### Variáveis de Ambiente Úteis
```bash
# Windows (PowerShell)
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "https://localhost:5001;http://localhost:5000"

# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS="https://localhost:5001;http://localhost:5000"
```

### appsettings por Ambiente
- `appsettings.json` - Base
- `appsettings.Development.json` - Desenvolvimento (override)
- `appsettings.Production.json` - Produção (override)

## Dicas de Desenvolvimento

1. **Use watch mode durante desenvolvimento:**
   ```bash
   dotnet watch run
   ```

2. **Sempre teste migrations antes de aplicar em produção:**
   ```bash
   dotnet ef migrations script -o preview.sql
   ```

3. **Use o Swagger para documentar e testar:**
   - Acessível em: https://localhost:5001/swagger

4. **Configure o Serilog para diferentes níveis por ambiente**

5. **Mantenha as migrations organizadas e com nomes descritivos**

6. **Faça backup antes de migrations destrutivas**

7. **Use User Secrets para dados sensíveis em desenvolvimento:**
   ```bash
   dotnet user-secrets init --project Auria.API
   dotnet user-secrets set "ConnectionString" "sua-string" --project Auria.API
   ```
