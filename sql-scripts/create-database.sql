IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251104163442_InitialCreate')
BEGIN
    CREATE TABLE [AGRICAMPANHA_USUARIO] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [Login] nvarchar(50) NOT NULL,
        [SenhaHash] nvarchar(255) NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [Ativo] bit NOT NULL,
        CONSTRAINT [PK_AGRICAMPANHA_USUARIO] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251104163442_InitialCreate')
BEGIN
    CREATE TABLE [CategoriasNoticias] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(450) NOT NULL,
        [Descricao] nvarchar(max) NULL,
        [Ativo] bit NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        CONSTRAINT [PK_CategoriasNoticias] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251104163442_InitialCreate')
BEGIN
    CREATE TABLE [AGRICAMPANHA_NOTICIA] (
        [Id] int NOT NULL IDENTITY,
        [Titulo] nvarchar(200) NOT NULL,
        [Subtitulo] nvarchar(300) NOT NULL,
        [CategoriaId] int NOT NULL,
        [DataNoticia] datetime2 NOT NULL,
        [Fonte] nvarchar(100) NOT NULL,
        [Texto] nvarchar(max) NOT NULL,
        [ImagemUrl] nvarchar(500) NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataAtualizacao] datetime2 NULL,
        CONSTRAINT [PK_AGRICAMPANHA_NOTICIA] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AGRICAMPANHA_NOTICIA_CategoriasNoticias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [CategoriasNoticias] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251104163442_InitialCreate')
BEGIN
    CREATE INDEX [IX_AGRICAMPANHA_NOTICIA_CategoriaId] ON [AGRICAMPANHA_NOTICIA] ([CategoriaId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251104163442_InitialCreate')
BEGIN
    CREATE INDEX [IX_AGRICAMPANHA_NOTICIA_DataNoticia] ON [AGRICAMPANHA_NOTICIA] ([DataNoticia]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251104163442_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_AGRICAMPANHA_USUARIO_Login] ON [AGRICAMPANHA_USUARIO] ([Login]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251104163442_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CategoriasNoticias_Nome] ON [CategoriasNoticias] ([Nome]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251104163442_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251104163442_InitialCreate', N'7.0.14');
END;
GO

COMMIT;
GO

