
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/28/2014 17:27:57
-- Generated from EDMX file: C:\Users\N000148583\Desktop\BarCode_MVC4\BarCode_MVC4\Models\BarCode.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [BarCode];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Customer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customer];
GO
IF OBJECT_ID(N'[dbo].[PackagingMaterial]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PackagingMaterial];
GO
IF OBJECT_ID(N'[dbo].[Srnm]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Srnm];
GO
IF OBJECT_ID(N'[dbo].[Staff]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Staff];
GO
IF OBJECT_ID(N'[dbo].[Zone]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Zone];
GO
IF OBJECT_ID(N'[BarCodeModelStoreContainer].[Inventory_View]', 'U') IS NOT NULL
    DROP TABLE [BarCodeModelStoreContainer].[Inventory_View];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Customer'
CREATE TABLE [dbo].[Customer] (
    [CustomerNo] nvarchar(4)  NOT NULL,
    [CustomerName] nvarchar(18)  NULL
);
GO

-- Creating table 'PackagingMaterial'
CREATE TABLE [dbo].[PackagingMaterial] (
    [PackNo] nvarchar(4)  NOT NULL,
    [PackName] nvarchar(8)  NULL
);
GO

-- Creating table 'Staff'
CREATE TABLE [dbo].[Staff] (
    [Account] nvarchar(50)  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Password] nvarchar(50)  NOT NULL,
    [Role] nvarchar(10)  NULL
);
GO

-- Creating table 'Zone'
CREATE TABLE [dbo].[Zone] (
    [ZoneID] nvarchar(1)  NOT NULL,
    [ZoneNo] nvarchar(4)  NOT NULL
);
GO

-- Creating table 'Srnm'
CREATE TABLE [dbo].[Srnm] (
    [SrnmType] nvarchar(10)  NOT NULL,
    [SrnmName] nvarchar(20)  NULL
);
GO

-- Creating table 'Inventory_View'
CREATE TABLE [dbo].[Inventory_View] (
    [Type] nvarchar(12)  NOT NULL,
    [Prod] nvarchar(10)  NOT NULL,
    [Thickness] nvarchar(50)  NULL,
    [Widt] nvarchar(4)  NULL,
    [Leng] int  NULL,
    [Splice] nvarchar(1)  NULL,
    [NewWeight] int  NULL,
    [GrossWeight] int  NULL,
    [Ptno] nvarchar(24)  NULL,
    [ProductDate] datetime  NULL,
    [CustomerNO] nvarchar(4)  NULL,
    [PackNo] nvarchar(4)  NULL,
    [StaffID] nvarchar(50)  NULL,
    [ZoneID] nvarchar(8)  NULL,
    [TransactionDate] datetime  NULL,
    [SrnmType] nvarchar(1)  NULL,
    [SrnmName] nvarchar(20)  NULL,
    [CtlNo1] nvarchar(10)  NULL,
    [CtlName1] nvarchar(20)  NULL,
    [CtlNo2] nvarchar(10)  NULL,
    [CtlName2] nvarchar(20)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [CustomerNo] in table 'Customer'
ALTER TABLE [dbo].[Customer]
ADD CONSTRAINT [PK_Customer]
    PRIMARY KEY CLUSTERED ([CustomerNo] ASC);
GO

-- Creating primary key on [PackNo] in table 'PackagingMaterial'
ALTER TABLE [dbo].[PackagingMaterial]
ADD CONSTRAINT [PK_PackagingMaterial]
    PRIMARY KEY CLUSTERED ([PackNo] ASC);
GO

-- Creating primary key on [Account] in table 'Staff'
ALTER TABLE [dbo].[Staff]
ADD CONSTRAINT [PK_Staff]
    PRIMARY KEY CLUSTERED ([Account] ASC);
GO

-- Creating primary key on [ZoneID] in table 'Zone'
ALTER TABLE [dbo].[Zone]
ADD CONSTRAINT [PK_Zone]
    PRIMARY KEY CLUSTERED ([ZoneID] ASC);
GO

-- Creating primary key on [SrnmType] in table 'Srnm'
ALTER TABLE [dbo].[Srnm]
ADD CONSTRAINT [PK_Srnm]
    PRIMARY KEY CLUSTERED ([SrnmType] ASC);
GO

-- Creating primary key on [Type], [Prod] in table 'Inventory_View'
ALTER TABLE [dbo].[Inventory_View]
ADD CONSTRAINT [PK_Inventory_View]
    PRIMARY KEY CLUSTERED ([Type], [Prod] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------