# Script de inicialização da Auria API
# Execute: .\start.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Auria API - Agricampanha" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se está no diretório correto
if (-not (Test-Path "Auria.sln")) {
    Write-Host "ERRO: Execute este script na raiz do projeto!" -ForegroundColor Red
    exit 1
}

# Verificar versão do .NET
Write-Host "Verificando versão do .NET..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
Write-Host "  .NET SDK: $dotnetVersion" -ForegroundColor Green

if (-not $dotnetVersion.StartsWith("8.")) {
    Write-Host "AVISO: Recomendado .NET 8.0" -ForegroundColor Yellow
}

# Restaurar pacotes
Write-Host ""
Write-Host "Restaurando pacotes NuGet..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha ao restaurar pacotes!" -ForegroundColor Red
    exit 1
}

# Build da solução
Write-Host ""
Write-Host "Compilando solução..." -ForegroundColor Yellow
dotnet build --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERRO: Falha na compilação!" -ForegroundColor Red
    exit 1
}

# Iniciar aplicação
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Iniciando Auria API..." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "URLs da aplicação:" -ForegroundColor Cyan
Write-Host "  - HTTPS: https://localhost:5001" -ForegroundColor White
Write-Host "  - HTTP:  http://localhost:5000" -ForegroundColor White
Write-Host "  - Swagger: https://localhost:5001/swagger" -ForegroundColor Yellow
Write-Host ""
Write-Host "Credenciais padrão:" -ForegroundColor Cyan
Write-Host "  - Login: admin" -ForegroundColor White
Write-Host "  - Senha: admin123" -ForegroundColor White
Write-Host ""
Write-Host "Pressione Ctrl+C para parar o servidor" -ForegroundColor Gray
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

cd Auria.API
dotnet run
