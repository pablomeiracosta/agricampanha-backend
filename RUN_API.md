# Como Executar a API - Passo a Passo

## ✅ Opção 1: PowerShell (RECOMENDADO)

Abra o **PowerShell** e execute:

```powershell
# 1. Ir para a pasta da API
cd "c:\Projetos\Auria\clientes\P0004 - Agricampanha\dev\backend\Auria.API"

# 2. Executar a aplicação
dotnet run --urls "http://localhost:5000"
```

**IMPORTANTE:** Deixe essa janela do PowerShell ABERTA!

### O que você deve ver:

```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Application started. Press Ctrl+C to shut down.
```

### Quando ver essas mensagens:

1. **Abra outro navegador**
2. **Acesse:** http://localhost:5000/swagger
3. **Pronto!** O Swagger deve carregar

---

## ✅ Opção 2: CMD

Abra o **Prompt de Comando** (cmd) e execute:

```cmd
cd c:\Projetos\Auria\clientes\P0004 - Agricampanha\dev\backend\Auria.API
dotnet run --urls http://localhost:5000
```

---

## ✅ Opção 3: Visual Studio Code

1. Abra o VS Code na pasta do projeto
2. Pressione `F5` ou vá em **Run > Start Debugging**
3. Aguarde compilar
4. Acesse http://localhost:5000/swagger

---

## ✅ Opção 4: Visual Studio

1. Abra a solução `Auria.sln`
2. Defina `Auria.API` como projeto de inicialização
3. Pressione `F5` ou clique em **▶ Start**
4. Aguarde compilar
5. O navegador deve abrir automaticamente

---

## 🐛 Se Houver ERROS

### Erro 1: "Unable to bind to http://localhost:5000"

**Causa:** Porta em uso

**Solução:**
```powershell
# Ver o que está usando a porta
netstat -ano | findstr :5000

# Matar o processo (substitua PID pelo número encontrado)
taskkill /PID <PID> /F

# Tentar novamente
dotnet run --urls "http://localhost:5000"
```

### Erro 2: "A network-related or instance-specific error"

**Causa:** SQL Server não está rodando ou connection string incorreta

**Solução:**
1. Inicie o SQL Server
2. Ou edite `appsettings.json` e corrija a connection string

### Erro 3: "The type initializer for 'Serilog.Log' threw an exception"

**Causa:** Pasta de logs não existe

**Solução:**
```powershell
mkdir logs
dotnet run --urls "http://localhost:5000"
```

### Erro 4: Build falha

**Causa:** Projeto não compilou

**Solução:**
```powershell
# Limpar e rebuild
dotnet clean
dotnet build

# Se compilar sem erros, executar
dotnet run --urls "http://localhost:5000"
```

---

## ✅ Verificar se Está Funcionando

### Teste 1: Aplicação está rodando?

No PowerShell/CMD onde executou `dotnet run`, você DEVE ver:
```
Now listening on: http://localhost:5000
```

### Teste 2: Swagger está acessível?

Abra o navegador e acesse:
```
http://localhost:5000/swagger
```

Você DEVE ver a página do Swagger UI com:
- Título: **Auria API - Agricampanha**
- Seções: **Auth** e **Noticias**

### Teste 3: API está respondendo?

Em outro PowerShell, execute:
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/swagger/v1/swagger.json" -UseBasicParsing
```

**Resultado esperado:** `StatusCode : 200`

---

## 📋 Checklist

Antes de acessar o Swagger:

- [ ] PowerShell/CMD está aberto
- [ ] Executei `dotnet run --urls "http://localhost:5000"`
- [ ] Vi a mensagem "Now listening on: http://localhost:5000"
- [ ] NÃO fechei a janela do PowerShell/CMD
- [ ] Abri o navegador
- [ ] Acessei http://localhost:5000/swagger

---

## 🎯 Dica Final

**A aplicação DEVE ficar rodando!**

Não feche a janela do PowerShell/CMD onde executou `dotnet run`. Ela deve permanecer aberta enquanto você usa a API.

Para **PARAR** a aplicação:
- Pressione `Ctrl+C` no PowerShell/CMD
- Ou feche a janela

Para **INICIAR** novamente:
- Execute `dotnet run --urls "http://localhost:5000"` novamente

---

## 📞 Ainda com Problemas?

Se após seguir TODOS os passos acima o Swagger ainda não funcionar:

1. **Copie TODO o texto** que aparece no PowerShell/CMD
2. **Tire um print** do erro no navegador
3. **Verifique** se há mensagens em vermelho no console
4. **Informe** qual dos 3 testes acima falhou

---

**URL DO SWAGGER:** http://localhost:5000/swagger

**Não esqueça:** A aplicação deve estar RODANDO (PowerShell aberto com "Now listening...")
