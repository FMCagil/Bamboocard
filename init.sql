-- /init.sql
USE [master];
GO
CREATE DATABASE [nop-sql];
GO

CREATE LOGIN [nopuser] WITH PASSWORD=N'UserStrongPwd123', CHECK_POLICY=ON, CHECK_EXPIRATION=OFF;
GO

USE [nop-sql];
GO
CREATE USER [nopuser] FOR LOGIN [nopuser];
ALTER ROLE [db_owner] ADD MEMBER [nopuser];
GO
