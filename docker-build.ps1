# Script para build e execução da imagem Docker da API Auria
# PowerShell script

param(
    [Parameter(Mandatory=$false)]
    [string]$Action = "build"
)

Write-Host "=== Auria API - Docker Management ===" -ForegroundColor Cyan
Write-Host ""

switch ($Action.ToLower()) {
    "build" {
        Write-Host "Construindo imagem Docker..." -ForegroundColor Yellow
        docker build -t auria-api:latest .
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Imagem construída com sucesso!" -ForegroundColor Green
        } else {
            Write-Host "Erro ao construir imagem." -ForegroundColor Red
            exit 1
        }
    }
    "run" {
        Write-Host "Iniciando container..." -ForegroundColor Yellow
        docker run -d -p 5000:80 --name auria-api auria-api:latest
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Container iniciado com sucesso!" -ForegroundColor Green
            Write-Host "API disponível em: http://localhost:5000" -ForegroundColor Cyan
            Write-Host "Swagger disponível em: http://localhost:5000/swagger" -ForegroundColor Cyan
        } else {
            Write-Host "Erro ao iniciar container." -ForegroundColor Red
            exit 1
        }
    }
    "stop" {
        Write-Host "Parando container..." -ForegroundColor Yellow
        docker stop auria-api
        docker rm auria-api
        Write-Host "Container parado e removido." -ForegroundColor Green
    }
    "logs" {
        Write-Host "Exibindo logs do container..." -ForegroundColor Yellow
        docker logs -f auria-api
    }
    "compose-up" {
        Write-Host "Iniciando com docker-compose..." -ForegroundColor Yellow
        docker-compose up -d --build
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Serviços iniciados com sucesso!" -ForegroundColor Green
            Write-Host "API disponível em: http://localhost:5000" -ForegroundColor Cyan
            Write-Host "Swagger disponível em: http://localhost:5000/swagger" -ForegroundColor Cyan
        }
    }
    "compose-down" {
        Write-Host "Parando serviços do docker-compose..." -ForegroundColor Yellow
        docker-compose down
        Write-Host "Serviços parados." -ForegroundColor Green
    }
    "push" {
        Write-Host "Enviando imagem para registry..." -ForegroundColor Yellow
        Write-Host "Por favor, faça o tag da imagem primeiro:" -ForegroundColor Yellow
        Write-Host "  docker tag auria-api:latest seu-registry/auria-api:latest" -ForegroundColor White
        Write-Host "  docker push seu-registry/auria-api:latest" -ForegroundColor White
    }
    default {
        Write-Host "Uso: .\docker-build.ps1 [Action]" -ForegroundColor White
        Write-Host ""
        Write-Host "Actions disponíveis:" -ForegroundColor Yellow
        Write-Host "  build         - Constrói a imagem Docker" -ForegroundColor White
        Write-Host "  run           - Inicia o container" -ForegroundColor White
        Write-Host "  stop          - Para e remove o container" -ForegroundColor White
        Write-Host "  logs          - Exibe os logs do container" -ForegroundColor White
        Write-Host "  compose-up    - Inicia com docker-compose" -ForegroundColor White
        Write-Host "  compose-down  - Para serviços do docker-compose" -ForegroundColor White
        Write-Host "  push          - Instruções para enviar ao registry" -ForegroundColor White
        Write-Host ""
        Write-Host "Exemplos:" -ForegroundColor Yellow
        Write-Host "  .\docker-build.ps1 build" -ForegroundColor White
        Write-Host "  .\docker-build.ps1 run" -ForegroundColor White
        Write-Host "  .\docker-build.ps1 compose-up" -ForegroundColor White
    }
}
