# Script para resolver problemas de compilação

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Fix Build - Auria API" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se está no diretório correto
if (-not (Test-Path "Auria.sln")) {
    Write-Host "ERRO: Execute este script na raiz do projeto!" -ForegroundColor Red
    exit 1
}

Write-Host "1. Limpando build anterior..." -ForegroundColor Yellow
dotnet clean

Write-Host ""
Write-Host "2. Removendo todas as pastas bin e obj..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force

Write-Host ""
Write-Host "3. Restaurando pacotes NuGet..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERRO: Falha ao restaurar pacotes!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "4. Compilando solução..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "=====================================" -ForegroundColor Green
    Write-Host "  Build realizado com sucesso!" -ForegroundColor Green
    Write-Host "=====================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Próximo passo:" -ForegroundColor Cyan
    Write-Host "  1. Configure o banco: .\setup-database.ps1" -ForegroundColor White
    Write-Host "  2. Inicie a API: .\start.ps1" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "=====================================" -ForegroundColor Red
    Write-Host "  Falha na compilação!" -ForegroundColor Red
    Write-Host "=====================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Possíveis soluções:" -ForegroundColor Yellow
    Write-Host "  1. Instale .NET 7 SDK se não tiver" -ForegroundColor White
    Write-Host "  2. Ou instale .NET 8 SDK (versão original)" -ForegroundColor White
    Write-Host "  3. Verifique os erros acima" -ForegroundColor White
    Write-Host ""
    Write-Host "Para mais ajuda, veja: FIX_BUILD.md" -ForegroundColor Cyan
    exit 1
}
