-- Mudar para o banco de dados AgendaPro
USE AgendaPro;
GO

-- 1️ Cria login no SQL Server
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'agendapro')
BEGIN
    CREATE LOGIN agendapro
    WITH PASSWORD = 'senhasecreta';
END
GO

-- 2️ Cria usuário no banco de dados associado ao login
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'agendapro')
BEGIN
    CREATE USER agendapro FOR LOGIN agendapro;
END
GO

-- 3️ Concede permissões totais no banco
ALTER ROLE db_owner ADD MEMBER agendapro;
GO
