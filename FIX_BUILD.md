# Como Resolver o Problema de Compilação

## Problema Identificado

O projeto foi configurado para .NET 8, mas você tem .NET 7 instalado. Ajustei todos os projetos para .NET 7, mas há um problema de pastas duplicadas que precisa ser resolvido.

## Solução Rápida

Execute os seguintes comandos no PowerShell:

```powershell
# 1. Limpar todo o build
cd "c:\Projetos\Auria\clientes\P0004 - Agricampanha\dev\backend"
dotnet clean

# 2. Remover todas as pastas bin e obj
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force

# 3. Restaurar pacotes
dotnet restore

# 4. Compilar
dotnet build

# 5. Se tudo compilar, executar
cd Auria.API
dotnet run
```

## Ou Use o Script de Fix

Execute:

```powershell
.\fix-build.ps1
```

## URLs da Aplicação

Após compilar e executar:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- **Swagger: https://localhost:5001/swagger**

## Credenciais

- Login: admin
- Senha: admin123

## Se o Swagger Não Abrir

1. Verifique se a aplicação está rodando
2. Veja se há erros no console
3. Tente acessar: http://localhost:5000/swagger
4. Confie no certificado: `dotnet dev-certs https --trust`

## Alternativa: Instalar .NET 8

Se preferir usar .NET 8 (versão original):

1. Baixe em: https://dotnet.microsoft.com/download/dotnet/8.0
2. Instale o SDK
3. Reverta as alterações:
   - global.json: voltar para "8.0.0"
   - Todos os .csproj: voltar para "net8.0"
   - Pacotes NuGet: voltar para versões 8.0.0

## Suporte

Se o problema persistir:
1. Verifique se o SQL Server está rodando
2. Confira a connection string em appsettings.json
3. Veja os logs em logs/auria-dev-*.log
