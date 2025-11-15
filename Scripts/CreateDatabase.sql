-- Script de criação do banco de dados Agricampanha
-- Execute este script caso não queira usar as migrations do Entity Framework

USE master;
GO

-- Cria o banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Agricampanha')
BEGIN
    CREATE DATABASE Agricampanha;
    PRINT 'Banco de dados Agricampanha criado com sucesso';
END
ELSE
BEGIN
    PRINT 'Banco de dados Agricampanha já existe';
END
GO

USE Agricampanha;
GO

-- Tabela de Usuários
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AGRICAMPANHA_USUARIO]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AGRICAMPANHA_USUARIO] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Nome] NVARCHAR(100) NOT NULL,
        [Login] NVARCHAR(50) NOT NULL UNIQUE,
        [SenhaHash] NVARCHAR(255) NOT NULL,
        [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [Ativo] BIT NOT NULL DEFAULT 1
    );

    CREATE INDEX IX_AGRICAMPANHA_USUARIO_Login ON [dbo].[AGRICAMPANHA_USUARIO] ([Login]);

    PRINT 'Tabela AGRICAMPANHA_USUARIO criada com sucesso';
END
GO

-- Tabela de Notícias
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AGRICAMPANHA_NOTICIA]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AGRICAMPANHA_NOTICIA] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Titulo] NVARCHAR(200) NOT NULL,
        [Subtitulo] NVARCHAR(300) NOT NULL,
        [Categoria] INT NOT NULL CHECK ([Categoria] BETWEEN 1 AND 5),
        [DataNoticia] DATETIME2 NOT NULL,
        [Fonte] NVARCHAR(100) NOT NULL,
        [Texto] NVARCHAR(MAX) NOT NULL,
        [ImagemUrl] NVARCHAR(500) NULL,
        [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [DataAtualizacao] DATETIME2 NULL
    );

    CREATE INDEX IX_AGRICAMPANHA_NOTICIA_DataNoticia ON [dbo].[AGRICAMPANHA_NOTICIA] ([DataNoticia]);
    CREATE INDEX IX_AGRICAMPANHA_NOTICIA_Categoria ON [dbo].[AGRICAMPANHA_NOTICIA] ([Categoria]);

    PRINT 'Tabela AGRICAMPANHA_NOTICIA criada com sucesso';
END
GO

-- Inserir usuário padrão (admin/admin123)
-- Senha hash gerada com BCrypt para "admin123"
IF NOT EXISTS (SELECT 1 FROM [dbo].[AGRICAMPANHA_USUARIO] WHERE [Login] = 'admin')
BEGIN
    INSERT INTO [dbo].[AGRICAMPANHA_USUARIO] ([Nome], [Login], [SenhaHash], [DataCriacao], [Ativo])
    VALUES ('Administrador', 'admin', '$2a$11$rGxQZKxZ8qVXJ7pJ.4mAEuGmxK7LVvKxM6hF4Jj.cKjHxJLZ0xK2C', GETDATE(), 1);

    PRINT 'Usuário admin criado com sucesso';
    PRINT 'Login: admin';
    PRINT 'Senha: admin123';
END
GO

-- Inserir notícias de exemplo
IF NOT EXISTS (SELECT 1 FROM [dbo].[AGRICAMPANHA_NOTICIA])
BEGIN
    INSERT INTO [dbo].[AGRICAMPANHA_NOTICIA]
        ([Titulo], [Subtitulo], [Categoria], [DataNoticia], [Fonte], [Texto], [DataCriacao])
    VALUES
        ('Inauguração da Nova Sede', 'Agricampanha inaugura novas instalações com tecnologia de ponta', 2, GETDATE(), 'Assessoria de Imprensa',
         'A Agricampanha tem o prazer de anunciar a inauguração de sua nova sede, equipada com tecnologia de última geração para melhor atender nossos associados.',
         GETDATE()),

        ('Workshop de Agricultura Sustentável', 'Evento gratuito aborda práticas sustentáveis no campo', 1, DATEADD(day, 7, GETDATE()), 'Departamento de Eventos',
         'Participe do nosso workshop gratuito sobre agricultura sustentável. Inscrições abertas até o final do mês.',
         GETDATE()),

        ('Confraternização de Final de Ano', 'Associados celebram conquistas de 2024', 3, DATEADD(month, -1, GETDATE()), 'Comunicação Social',
         'A tradicional confraternização da Agricampanha reuniu centenas de associados para celebrar as conquistas do ano.',
         GETDATE()),

        ('Mercado de Commodities em Alta', 'Análise das tendências do mercado agrícola', 4, GETDATE(), 'Departamento Técnico',
         'O mercado de commodities agrícolas apresenta tendência de alta para o próximo trimestre, segundo análises de especialistas.',
         GETDATE()),

        ('Nova Técnica de Irrigação', 'Tecnologia economiza até 40% de água', 5, DATEADD(day, -5, GETDATE()), 'Centro de Pesquisa',
         'Pesquisadores desenvolvem nova técnica de irrigação que promete revolucionar o uso consciente de recursos hídricos na agricultura.',
         GETDATE());

    PRINT 'Notícias de exemplo inseridas com sucesso';
END
GO

PRINT '';
PRINT '=================================================';
PRINT 'Database setup concluído com sucesso!';
PRINT '=================================================';
PRINT 'Credenciais de acesso:';
PRINT 'Login: admin';
PRINT 'Senha: admin123';
PRINT '=================================================';
GO
