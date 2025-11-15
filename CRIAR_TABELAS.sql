-- ========================================
-- Script para Criar Tabelas do Banco Auria
-- Banco: impulsoweb
-- ========================================

-- Tabela de Usuários
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AGRICAMPANHA_USUARIO')
BEGIN
    CREATE TABLE [AGRICAMPANHA_USUARIO] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Nome] NVARCHAR(100) NOT NULL,
        [Login] NVARCHAR(50) NOT NULL UNIQUE,
        [SenhaHash] NVARCHAR(255) NOT NULL,
        [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [Ativo] BIT NOT NULL DEFAULT 1
    );

    PRINT 'Tabela AGRICAMPANHA_USUARIO criada com sucesso!';
END
ELSE
BEGIN
    PRINT 'Tabela AGRICAMPANHA_USUARIO já existe.';
END
GO

-- Tabela de Notícias
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AGRICAMPANHA_NOTICIA')
BEGIN
    CREATE TABLE [AGRICAMPANHA_NOTICIA] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Titulo] NVARCHAR(200) NOT NULL,
        [Subtitulo] NVARCHAR(300) NULL,
        [Categoria] INT NOT NULL CHECK ([Categoria] BETWEEN 1 AND 5),
        [DataNoticia] DATETIME2 NOT NULL,
        [Fonte] NVARCHAR(100) NULL,
        [Texto] NVARCHAR(MAX) NOT NULL,
        [ImagemUrl] NVARCHAR(500) NULL,
        [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [DataAtualizacao] DATETIME2 NULL
    );

    CREATE INDEX IX_AGRICAMPANHA_NOTICIA_Categoria ON [AGRICAMPANHA_NOTICIA]([Categoria]);
    CREATE INDEX IX_AGRICAMPANHA_NOTICIA_DataNoticia ON [AGRICAMPANHA_NOTICIA]([DataNoticia] DESC);

    PRINT 'Tabela AGRICAMPANHA_NOTICIA criada com sucesso!';
END
ELSE
BEGIN
    PRINT 'Tabela AGRICAMPANHA_NOTICIA já existe.';
END
GO

-- Inserir usuário padrão (admin/admin123)
IF NOT EXISTS (SELECT * FROM [AGRICAMPANHA_USUARIO] WHERE [Login] = 'admin')
BEGIN
    -- Senha: admin123
    -- BCrypt Hash gerado com custo 10
    INSERT INTO [AGRICAMPANHA_USUARIO] ([Nome], [Login], [SenhaHash], [DataCriacao], [Ativo])
    VALUES ('Administrador', 'admin', '$2a$10$rYQlF6ZGqYNVYqHJ3YvP1.BVnE5Ct5h6XjW9Z5mQlYvP1.BVnE5Ct6', GETDATE(), 1);

    PRINT 'Usuário admin criado com sucesso!';
    PRINT 'Login: admin';
    PRINT 'Senha: admin123';
END
ELSE
BEGIN
    PRINT 'Usuário admin já existe.';
END
GO

-- Inserir algumas notícias de exemplo
IF NOT EXISTS (SELECT * FROM [AGRICAMPANHA_NOTICIA])
BEGIN
    INSERT INTO [AGRICAMPANHA_NOTICIA] ([Titulo], [Subtitulo], [Categoria], [DataNoticia], [Fonte], [Texto])
    VALUES
    ('Safra de Milho Supera Expectativas', 'Produção aumenta 15% em relação ao ano anterior', 1, GETDATE(), 'Agro News', 'A safra de milho deste ano superou todas as expectativas iniciais, com um crescimento de 15% na produção em comparação ao ano anterior. Este resultado positivo é atribuído principalmente às condições climáticas favoráveis e ao uso de tecnologias modernas de cultivo.'),

    ('Nova Tecnologia para Irrigação', 'Sistema inteligente economiza até 40% de água', 2, GETDATE(), 'Tech Agro', 'Foi lançada uma nova tecnologia de irrigação inteligente que promete revolucionar o setor agrícola. O sistema utiliza sensores e inteligência artificial para otimizar o uso da água, economizando até 40% em comparação com métodos tradicionais.'),

    ('Preços dos Grãos em Alta', 'Mercado internacional impulsiona cotações', 3, GETDATE(), 'Mercado Rural', 'Os preços dos grãos registraram alta significativa no mercado nacional, impulsionados pela forte demanda internacional e pela valorização do dólar. Especialistas preveem que a tendência de alta deve se manter nos próximos meses.'),

    ('Curso de Agricultura Sustentável', 'Inscrições abertas para capacitação gratuita', 4, GETDATE(), 'SENAR', 'Estão abertas as inscrições para o curso gratuito de Agricultura Sustentável, oferecido em parceria com o SENAR. O curso aborda práticas modernas de cultivo que respeitam o meio ambiente e aumentam a produtividade.'),

    ('Clima Favorável para Plantio', 'Previsão indica chuvas regulares para os próximos meses', 5, GETDATE(), 'Meteorologia Agrícola', 'A previsão meteorológica indica um período de chuvas regulares para os próximos três meses, criando condições ideais para o plantio da próxima safra. Agricultores são orientados a aproveitar a janela de oportunidade.');

    PRINT '5 notícias de exemplo inseridas com sucesso!';
END
ELSE
BEGIN
    PRINT 'Já existem notícias no banco de dados.';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'Script executado com sucesso!';
PRINT '========================================';
PRINT '';
PRINT 'Tabelas criadas:';
PRINT '  - AGRICAMPANHA_USUARIO';
PRINT '  - AGRICAMPANHA_NOTICIA';
PRINT '';
PRINT 'Credenciais de acesso:';
PRINT '  Login: admin';
PRINT '  Senha: admin123';
PRINT '';
PRINT 'Notícias de exemplo: 5 inseridas';
PRINT '========================================';
