-- Veritabanını oluştur
CREATE DATABASE [nop-sql];

-- Bağlantıyı tek kullanıcıya al
ALTER DATABASE [nop-sql] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

-- Yedek dosyasını restore et
RESTORE DATABASE [nop-sql]
FROM DISK = N'/init/nop-sql.bak'
WITH
    MOVE N'nop-sql' TO N'/var/opt/mssql/data/nop-sql.mdf',
    MOVE N'nop-sql_log' TO N'/var/opt/mssql/data/nop-sql_log.ldf',
    REPLACE,
    RECOVERY;

-- Normal moda geri dön
ALTER DATABASE [nop-sql] SET MULTI_USER;
