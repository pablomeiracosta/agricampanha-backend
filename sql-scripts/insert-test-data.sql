-- =====================================================
-- Script de Dados Iniciais para Testes
-- API Auria - Agricampanha
-- =====================================================

USE Agricampanha;
GO

-- =====================================================
-- 1. INSERIR USUÁRIO ADMINISTRADOR
-- Login: admin
-- Senha: Admin@123
-- Hash BCrypt gerado para a senha Admin@123
-- =====================================================

IF NOT EXISTS (SELECT 1 FROM AGRICAMPANHA_USUARIO WHERE Login = 'admin')
BEGIN
    INSERT INTO AGRICAMPANHA_USUARIO (Nome, Login, SenhaHash, DataCriacao, Ativo)
    VALUES (
        'Administrador',
        'admin',
        '$2a$11$JZlqGZK3VLhXMD1QVk5mGO8zN6J1P3xQT.YLvYnxF4kZqDx5PQRVe', -- Senha: Admin@123
        GETDATE(),
        1
    );
    PRINT 'Usuário admin criado com sucesso!';
END
ELSE
BEGIN
    PRINT 'Usuário admin já existe.';
END
GO

-- =====================================================
-- 2. INSERIR CATEGORIAS DE NOTÍCIAS
-- =====================================================

-- Categoria: Agricultura
IF NOT EXISTS (SELECT 1 FROM CategoriasNoticias WHERE Nome = 'Agricultura')
BEGIN
    INSERT INTO CategoriasNoticias (Nome, Descricao, Ativo, DataCriacao)
    VALUES (
        'Agricultura',
        'Notícias relacionadas a técnicas agrícolas, plantio e colheita',
        1,
        GETDATE()
    );
    PRINT 'Categoria Agricultura criada!';
END

-- Categoria: Pecuária
IF NOT EXISTS (SELECT 1 FROM CategoriasNoticias WHERE Nome = 'Pecuária')
BEGIN
    INSERT INTO CategoriasNoticias (Nome, Descricao, Ativo, DataCriacao)
    VALUES (
        'Pecuária',
        'Notícias sobre criação de animais e produção pecuária',
        1,
        GETDATE()
    );
    PRINT 'Categoria Pecuária criada!';
END

-- Categoria: Tecnologia no Campo
IF NOT EXISTS (SELECT 1 FROM CategoriasNoticias WHERE Nome = 'Tecnologia no Campo')
BEGIN
    INSERT INTO CategoriasNoticias (Nome, Descricao, Ativo, DataCriacao)
    VALUES (
        'Tecnologia no Campo',
        'Inovações tecnológicas aplicadas ao agronegócio',
        1,
        GETDATE()
    );
    PRINT 'Categoria Tecnologia no Campo criada!';
END

-- Categoria: Sustentabilidade
IF NOT EXISTS (SELECT 1 FROM CategoriasNoticias WHERE Nome = 'Sustentabilidade')
BEGIN
    INSERT INTO CategoriasNoticias (Nome, Descricao, Ativo, DataCriacao)
    VALUES (
        'Sustentabilidade',
        'Práticas sustentáveis e preservação ambiental no agronegócio',
        1,
        GETDATE()
    );
    PRINT 'Categoria Sustentabilidade criada!';
END

-- Categoria: Mercado e Economia
IF NOT EXISTS (SELECT 1 FROM CategoriasNoticias WHERE Nome = 'Mercado e Economia')
BEGIN
    INSERT INTO CategoriasNoticias (Nome, Descricao, Ativo, DataCriacao)
    VALUES (
        'Mercado e Economia',
        'Cotações, mercado e economia do agronegócio',
        1,
        GETDATE()
    );
    PRINT 'Categoria Mercado e Economia criada!';
END

GO

-- =====================================================
-- 3. INSERIR NOTÍCIAS DE EXEMPLO
-- =====================================================

DECLARE @CategoriaAgriculturaId INT;
DECLARE @CategoriaTecnologiaId INT;

SELECT @CategoriaAgriculturaId = Id FROM CategoriasNoticias WHERE Nome = 'Agricultura';
SELECT @CategoriaTecnologiaId = Id FROM CategoriasNoticias WHERE Nome = 'Tecnologia no Campo';

-- Notícia 1
IF NOT EXISTS (SELECT 1 FROM AGRICAMPANHA_NOTICIA WHERE Titulo = 'Nova técnica de irrigação aumenta produtividade')
BEGIN
    INSERT INTO AGRICAMPANHA_NOTICIA (
        Titulo,
        Subtitulo,
        CategoriaId,
        DataNoticia,
        Fonte,
        Texto,
        DataCriacao,
        DataAtualizacao
    )
    VALUES (
        'Nova técnica de irrigação aumenta produtividade',
        'Método desenvolvido por pesquisadores reduz consumo de água em 40%',
        @CategoriaAgriculturaId,
        GETDATE(),
        'Embrapa',
        'Pesquisadores da Embrapa desenvolveram uma nova técnica de irrigação por gotejamento que promete revolucionar o setor agrícola. O método utiliza sensores inteligentes que monitoram a umidade do solo em tempo real, aplicando água apenas quando necessário. Os testes realizados em lavouras de soja demonstraram uma redução de 40% no consumo de água, mantendo os mesmos níveis de produtividade.',
        GETDATE(),
        GETDATE()
    );
    PRINT 'Notícia sobre irrigação criada!';
END

-- Notícia 2
IF NOT EXISTS (SELECT 1 FROM AGRICAMPANHA_NOTICIA WHERE Titulo = 'Drones revolucionam monitoramento de lavouras')
BEGIN
    INSERT INTO AGRICAMPANHA_NOTICIA (
        Titulo,
        Subtitulo,
        CategoriaId,
        DataNoticia,
        Fonte,
        Texto,
        DataCriacao,
        DataAtualizacao
    )
    VALUES (
        'Drones revolucionam monitoramento de lavouras',
        'Tecnologia permite identificar pragas e doenças com precisão',
        @CategoriaTecnologiaId,
        GETDATE(),
        'AgroTech Brasil',
        'O uso de drones equipados com câmeras multiespectrais está transformando a forma como os agricultores monitoram suas lavouras. A tecnologia permite identificar áreas afetadas por pragas, doenças e deficiências nutricionais antes mesmo que os sintomas sejam visíveis a olho nu. Segundo especialistas, a detecção precoce pode aumentar a eficiência das aplicações em até 60%.',
        GETDATE(),
        GETDATE()
    );
    PRINT 'Notícia sobre drones criada!';
END

GO

-- =====================================================
-- 4. VERIFICAR DADOS INSERIDOS
-- =====================================================

PRINT '';
PRINT '========================================';
PRINT 'RESUMO DOS DADOS INSERIDOS';
PRINT '========================================';
PRINT '';

SELECT
    'Usuários' AS Tabela,
    COUNT(*) AS Total
FROM AGRICAMPANHA_USUARIO

UNION ALL

SELECT
    'Categorias' AS Tabela,
    COUNT(*) AS Total
FROM CategoriasNoticias

UNION ALL

SELECT
    'Notícias' AS Tabela,
    COUNT(*) AS Total
FROM AGRICAMPANHA_NOTICIA;

GO

PRINT '';
PRINT '========================================';
PRINT 'CREDENCIAIS DE TESTE';
PRINT '========================================';
PRINT 'Login: admin';
PRINT 'Senha: Admin@123';
PRINT '========================================';
PRINT '';
PRINT 'Script executado com sucesso!';
PRINT 'Agora você pode testar a API em http://localhost:5000/swagger';
GO
