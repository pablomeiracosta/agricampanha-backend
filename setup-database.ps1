# Script para criar o banco de dados
# Execute: .\setup-database.ps1

param(
    [switch]$Reset
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Setup do Banco de Dados" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se está no diretório correto
if (-not (Test-Path "Auria.sln")) {
    Write-Host "ERRO: Execute este script na raiz do projeto!" -ForegroundColor Red
    exit 1
}

cd Auria.API

if ($Reset) {
    Write-Host "ATENÇÃO: Você está prestes a DELETAR o banco de dados!" -ForegroundColor Red
    $confirm = Read-Host "Digite 'SIM' para confirmar"

    if ($confirm -eq "SIM") {
        Write-Host ""
        Write-Host "Deletando banco de dados..." -ForegroundColor Yellow
        dotnet ef database drop --force --project ..\Auria.Data

        if ($LASTEXITCODE -ne 0) {
            Write-Host "ERRO: Falha ao deletar banco!" -ForegroundColor Red
            exit 1
        }
        Write-Host "Banco deletado com sucesso!" -ForegroundColor Green
    } else {
        Write-Host "Operação cancelada." -ForegroundColor Yellow
        exit 0
    }
}

Write-Host ""
Write-Host "Verificando migrations existentes..." -ForegroundColor Yellow
$migrations = dotnet ef migrations list --project ..\Auria.Data 2>&1

if ($migrations -match "No migrations") {
    Write-Host "Nenhuma migration encontrada. Criando migration inicial..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate --project ..\Auria.Data

    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERRO: Falha ao criar migration!" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "Aplicando migrations no banco de dados..." -ForegroundColor Yellow
dotnet ef database update --project ..\Auria.Data

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  Banco de dados configurado!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Banco de dados: Agricampanha" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Tabelas criadas:" -ForegroundColor Cyan
    Write-Host "  - AGRICAMPANHA_USUARIO" -ForegroundColor White
    Write-Host "  - AGRICAMPANHA_NOTICIA" -ForegroundColor White
    Write-Host ""
    Write-Host "Usuário padrão criado:" -ForegroundColor Cyan
    Write-Host "  - Login: admin" -ForegroundColor White
    Write-Host "  - Senha: admin123" -ForegroundColor White
    Write-Host ""
    Write-Host "Próximo passo: Execute .\start.ps1 para iniciar a API" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "ERRO: Falha ao aplicar migrations!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifique:" -ForegroundColor Yellow
    Write-Host "  1. SQL Server está rodando" -ForegroundColor White
    Write-Host "  2. Connection string em appsettings.json está correta" -ForegroundColor White
    Write-Host "  3. Você tem permissões para criar o banco de dados" -ForegroundColor White
    exit 1
}

cd ..
