-- Script para inserir dados de teste no banco Impulso
-- Senha do admin: Admin@123

-- Inserir usuário admin
IF NOT EXISTS (SELECT 1 FROM AGRICAMPANHA_USUARIO WHERE Login = 'admin')
BEGIN
    INSERT INTO AGRICAMPANHA_USUARIO (Nome, Login, SenhaHash, DataCriacao, Ativo)
    VALUES ('Administrador', 'admin', '$2a$11$JZlqGZK3VLhXMD1QVk5mGO8zN6J1P3xQT.YLvYnxF4kZqDx5PQRVe', GETDATE(), 1);
    PRINT 'Usuário admin criado!';
END
ELSE
    PRINT 'Usuário admin já existe!';

-- Inserir categorias
IF NOT EXISTS (SELECT 1 FROM CategoriasNoticias WHERE Nome = 'Agricultura')
BEGIN
    INSERT INTO CategoriasNoticias (Nome, Descricao, Ativo, DataCriacao) VALUES
    ('Agricultura', 'Notícias relacionadas a técnicas agrícolas, plantio e colheita', 1, GETDATE()),
    ('Pecuária', 'Notícias sobre criação de animais e produção pecuária', 1, GETDATE()),
    ('Tecnologia no Campo', 'Inovações tecnológicas aplicadas ao agronegócio', 1, GETDATE()),
    ('Sustentabilidade', 'Práticas sustentáveis e preservação ambiental no agronegócio', 1, GETDATE()),
    ('Mercado e Economia', 'Cotações, mercado e economia do agronegócio', 1, GETDATE());
    PRINT 'Categorias criadas!';
END
ELSE
    PRINT 'Categorias já existem!';

-- Verificar dados inseridos
SELECT COUNT(*) AS TotalUsuarios FROM AGRICAMPANHA_USUARIO;
SELECT COUNT(*) AS TotalCategorias FROM CategoriasNoticias;

PRINT '========================================';
PRINT 'CREDENCIAIS DE TESTE';
PRINT '========================================';
PRINT 'Login: admin';
PRINT 'Senha: Admin@123';
PRINT '========================================';
