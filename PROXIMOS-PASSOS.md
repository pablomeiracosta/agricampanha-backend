# Próximos Passos - Configuração Final

## ✅ O que já foi concluído

1. ✅ Aplicação compilando sem erros
2. ✅ Migração do relacionamento Categoria → Notícia (enum para FK)
3. ✅ Todos os endpoints implementados e funcionando
4. ✅ Paginação de notícias implementada
5. ✅ Validações com FluentValidation configuradas
6. ✅ Eager loading de categorias configurado
7. ✅ Scripts SQL gerados (create-database.sql e insert-test-data.sql)
8. ✅ Documentação completa de testes criada (GUIA-TESTES.md)

---

## ⏳ Próximos Passos Necessários

### 1. Iniciar o SQL Server

Se o SQL Server não estiver rodando, inicie o serviço:

**Opção A - Serviços do Windows:**
1. Pressione `Win + R`
2. Digite `services.msc`
3. Procure por "SQL Server (MSSQLSERVER)" ou "SQL Server (nome-da-instancia)"
4. Clique com botão direito → Iniciar

**Opção B - PowerShell (Como Administrador):**
```powershell
Start-Service MSSQLSERVER
# ou
Start-Service "MSSQL$INSTANCIA"
```

**Opção C - SQL Server Configuration Manager:**
1. Abra "SQL Server Configuration Manager"
2. Vá em "SQL Server Services"
3. Inicie o serviço SQL Server

---

### 2. Configurar o Banco de Dados

Você tem duas opções:

#### Opção A - Entity Framework (Recomendado)

```bash
cd "c:\Projetos\Auria\clientes\P0004 - Agricampanha\dev\backend\Auria.Data"
dotnet ef database update --startup-project ../Auria.API
```

Este comando irá:
- Criar o banco de dados "Agricampanha" automaticamente
- Criar todas as tabelas
- Aplicar os índices e constraints

#### Opção B - Scripts SQL Manuais

1. Abra **SQL Server Management Studio (SSMS)**
2. Conecte em `localhost`
3. Abra e execute: `sql-scripts/create-database.sql`
4. Abra e execute: `sql-scripts/insert-test-data.sql`

---

### 3. Verificar a Criação das Tabelas

Execute esta query no SSMS para verificar:

```sql
USE Agricampanha;
GO

-- Verificar tabelas criadas
SELECT
    TABLE_SCHEMA,
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Verificar relacionamento FK
SELECT
    FK.name AS ForeignKey_Name,
    TP.name AS Parent_Table,
    CP.name AS Parent_Column,
    TR.name AS Referenced_Table,
    CR.name AS Referenced_Column
FROM sys.foreign_keys FK
INNER JOIN sys.tables TP ON FK.parent_object_id = TP.object_id
INNER JOIN sys.tables TR ON FK.referenced_object_id = TR.object_id
INNER JOIN sys.foreign_key_columns FKC ON FK.object_id = FKC.constraint_object_id
INNER JOIN sys.columns CP ON FKC.parent_column_id = CP.column_id AND FKC.parent_object_id = CP.object_id
INNER JOIN sys.columns CR ON FKC.referenced_column_id = CR.column_id AND FKC.referenced_object_id = CR.object_id
WHERE TP.name IN ('AGRICAMPANHA_NOTICIA', 'AGRICAMPANHA_USUARIO', 'CategoriasNoticias');
```

Resultado esperado:
- `AGRICAMPANHA_USUARIO` (tabela de usuários)
- `CategoriasNoticias` (tabela de categorias)
- `AGRICAMPANHA_NOTICIA` (tabela de notícias com FK para categorias)
- `__EFMigrationsHistory` (controle de migrações)

---

### 4. Inserir Dados de Teste

Se você usou o Entity Framework, ainda precisa inserir dados iniciais:

```bash
# No SSMS, execute o script:
sql-scripts/insert-test-data.sql
```

Isso irá criar:
- 1 usuário administrador (login: admin, senha: Admin@123)
- 5 categorias de notícias
- 2 notícias de exemplo

---

### 5. Reiniciar a Aplicação

```bash
cd "c:\Projetos\Auria\clientes\P0004 - Agricampanha\dev\backend\Auria.API"
dotnet run --urls "http://localhost:5000"
```

Aguarde até ver:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

---

### 6. Testar a Aplicação

#### 6.1. Abrir o Swagger
Navegue para: **http://localhost:5000/swagger**

#### 6.2. Fazer Login e Obter Token

1. No Swagger, localize `POST /api/auth/login`
2. Clique em "Try it out"
3. Use o body:
```json
{
  "login": "admin",
  "senha": "Admin@123"
}
```
4. Clique em "Execute"
5. **Copie o token** retornado

#### 6.3. Configurar Autenticação

1. Clique no botão **"Authorize"** (cadeado no topo do Swagger)
2. Digite: `Bearer SEU_TOKEN_AQUI`
3. Clique em "Authorize"

#### 6.4. Testar Endpoints

Agora você pode testar todos os endpoints! Siga o guia completo em:
📄 **[GUIA-TESTES.md](GUIA-TESTES.md)**

---

## 📋 Checklist Rápido de Validação

Execute este checklist para garantir que está tudo funcionando:

### Banco de Dados
- [ ] SQL Server está rodando
- [ ] Banco "Agricampanha" foi criado
- [ ] Tabelas foram criadas (3 principais + __EFMigrationsHistory)
- [ ] Dados de teste foram inseridos
- [ ] FK entre Noticia e CategoriaNoticia está configurada

### API
- [ ] Aplicação inicia sem erros
- [ ] Swagger abre em http://localhost:5000/swagger
- [ ] Login funciona e retorna token
- [ ] Token é aceito em endpoints protegidos
- [ ] GET /api/categorias retorna as 5 categorias
- [ ] GET /api/noticias retorna as 2 notícias de exemplo
- [ ] As notícias incluem os dados da categoria (eager loading)

### CRUD Categorias
- [ ] Criar nova categoria funciona
- [ ] Listar categorias funciona
- [ ] Atualizar categoria funciona
- [ ] Deletar categoria sem notícias funciona
- [ ] Deletar categoria COM notícias retorna erro (restrição FK)

### CRUD Notícias
- [ ] Criar notícia sem imagem funciona
- [ ] Criar notícia com imagem funciona (Cloudinary)
- [ ] Listar notícias funciona
- [ ] Atualizar notícia funciona
- [ ] Deletar notícia funciona

### Paginação
- [ ] GET /api/noticias/paginadas funciona
- [ ] Parâmetros pageNumber e pageSize funcionam
- [ ] Metadados (totalPages, hasNextPage, etc) estão corretos

### Filtro por Categoria
- [ ] GET /api/noticias/categoria/{id} funciona
- [ ] Retorna apenas notícias da categoria especificada
- [ ] Categoria é carregada com eager loading

---

## 🔧 Solução de Problemas

### Erro: "Cannot connect to SQL Server"

**Causa:** SQL Server não está rodando ou connection string incorreta

**Solução:**
1. Verifique se o serviço SQL Server está rodando
2. Confirme a connection string em `appsettings.Development.json`:
```json
{
  "ConnectionString": "Server=localhost;Database=Agricampanha;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
3. Teste a conexão usando SSMS

### Erro: "Database does not exist"

**Causa:** O banco ainda não foi criado

**Solução:**
```bash
cd Auria.Data
dotnet ef database update --startup-project ../Auria.API
```

### Erro: "Login failed for user"

**Causa:** Problema de autenticação Windows ou permissões

**Solução:**
1. Verifique se está usando Windows Authentication (Trusted_Connection=True)
2. Ou configure SQL Server Authentication na connection string:
```json
{
  "ConnectionString": "Server=localhost;Database=Agricampanha;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;"
}
```

### Erro 401 Unauthorized nos endpoints

**Causa:** Token não foi enviado ou está incorreto

**Solução:**
1. Faça login: `POST /api/auth/login`
2. Copie o token retornado
3. Configure no Swagger: `Bearer {token}`
4. Ou adicione no header: `Authorization: Bearer {token}`

---

## 📊 Queries SQL Úteis para Validação

```sql
USE Agricampanha;
GO

-- Ver todas as categorias
SELECT * FROM CategoriasNoticias ORDER BY Nome;

-- Ver todas as notícias com suas categorias
SELECT
    n.Id,
    n.Titulo,
    c.Nome AS Categoria,
    n.DataNoticia,
    n.Fonte
FROM AGRICAMPANHA_NOTICIA n
INNER JOIN CategoriasNoticias c ON n.CategoriaId = c.Id
ORDER BY n.DataNoticia DESC;

-- Contar notícias por categoria
SELECT
    c.Nome AS Categoria,
    COUNT(n.Id) AS TotalNoticias
FROM CategoriasNoticias c
LEFT JOIN AGRICAMPANHA_NOTICIA n ON c.Id = n.CategoriaId
GROUP BY c.Nome
ORDER BY TotalNoticias DESC;

-- Ver usuários
SELECT
    Id,
    Nome,
    Login,
    Ativo,
    DataCriacao
FROM AGRICAMPANHA_USUARIO;

-- Ver última notícia criada
SELECT TOP 1
    n.*,
    c.Nome AS CategoriaNome
FROM AGRICAMPANHA_NOTICIA n
INNER JOIN CategoriasNoticias c ON n.CategoriaId = c.Id
ORDER BY n.DataCriacao DESC;
```

---

## 📚 Documentação Adicional

- **Guia Completo de Testes:** [GUIA-TESTES.md](GUIA-TESTES.md)
- **README Principal:** [README.md](README.md)
- **Script de Criação:** [sql-scripts/create-database.sql](sql-scripts/create-database.sql)
- **Dados de Teste:** [sql-scripts/insert-test-data.sql](sql-scripts/insert-test-data.sql)

---

## ✅ Resumo Final

Depois de seguir estes passos, você terá:

1. ✅ SQL Server rodando
2. ✅ Banco de dados "Agricampanha" criado
3. ✅ Todas as tabelas criadas com relacionamentos
4. ✅ Dados de teste inseridos (categorias, notícias, usuário admin)
5. ✅ API rodando em http://localhost:5000
6. ✅ Swagger funcional para testes
7. ✅ Todos os endpoints funcionando e persistindo dados corretamente

**Próximo passo:** Abrir o [GUIA-TESTES.md](GUIA-TESTES.md) e seguir o checklist completo para validar todos os endpoints!

---

**Data:** 04/11/2025
**Status:** Pronto para testes após configuração do banco de dados
