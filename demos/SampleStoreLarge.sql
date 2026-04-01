----------------------------------------------------------
-- SampleStoreLarge - Large Demo Database (~70k rows)
-- Author: UkrGuru / Alexander
-- Purpose: Reliable large dataset for testing, profiling,
--          perf benchmarks, demos, UkrGuru.Sql examples
----------------------------------------------------------

----------------------------------------------------------
-- Recreate DB
----------------------------------------------------------
IF DB_ID('SampleStoreLarge') IS NOT NULL
    DROP DATABASE SampleStoreLarge;
GO

CREATE DATABASE SampleStoreLarge;
GO

USE SampleStoreLarge;
GO

----------------------------------------------------------
-- Tables
----------------------------------------------------------

CREATE TABLE Customers (
    CustomerId INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) UNIQUE NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE Products (
    ProductId INT IDENTITY PRIMARY KEY,
    ProductName NVARCHAR(200) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Orders (
    OrderId INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    OrderDate DATETIME2 NOT NULL,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId)
        REFERENCES Customers(CustomerId)
);

CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId)
        REFERENCES Orders(OrderId),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId)
        REFERENCES Products(ProductId)
);

----------------------------------------------------------
-- Generate Customers (10,000)
----------------------------------------------------------
;WITH N AS (
    SELECT TOP (10000) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects a CROSS JOIN sys.all_objects b
)
INSERT INTO Customers (FullName, Email)
SELECT
    CONCAT('Customer ', n),
    CONCAT('customer', n, '@example.com')
FROM N;

----------------------------------------------------------
-- Generate Products (500)
----------------------------------------------------------
;WITH P AS (
    SELECT TOP (500) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects
)
INSERT INTO Products (ProductName, Price)
SELECT
    CONCAT('Product ', n),
    CAST((RAND(CHECKSUM(NEWID())) * 500 + 5) AS DECIMAL(10,2))
FROM P;

----------------------------------------------------------
-- Generate Orders (20,000)
----------------------------------------------------------
;WITH O AS (
    SELECT TOP (20000)
        ABS(CHECKSUM(NEWID())) % 10000 + 1 AS CustomerId
    FROM sys.all_objects a CROSS JOIN sys.all_objects b
)
INSERT INTO Orders (CustomerId, OrderDate)
SELECT
    CustomerId,
    DATEADD(day, -ABS(CHECKSUM(NEWID())) % 365, SYSDATETIME())
FROM O;

----------------------------------------------------------
-- Generate OrderItems (~40,000, avg 2 items/order)
----------------------------------------------------------

;WITH Base AS (
    SELECT OrderId,
           (ABS(CHECKSUM(NEWID())) % 2) + 1 AS ItemCount
    FROM Orders
),
Nums AS (
    SELECT TOP (5) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects
)
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
SELECT
    b.OrderId,
    ABS(CHECKSUM(NEWID())) % 500 + 1 AS ProductId,
    ABS(CHECKSUM(NEWID())) % 5 + 1 AS Quantity,
    CAST((RAND(CHECKSUM(NEWID())) * 500 + 5) AS DECIMAL(10,2))
FROM Base b
JOIN Nums n ON n.n <= b.ItemCount;

----------------------------------------------------------
-- Indexes for relational columns
----------------------------------------------------------

CREATE INDEX IX_Orders_CustomerId 
    ON Orders (CustomerId);

CREATE INDEX IX_OrderItems_OrderId 
    ON OrderItems (OrderId);

CREATE INDEX IX_OrderItems_ProductId 
    ON OrderItems (ProductId);

----------------------------------------------------------
-- Optional performance indexes
----------------------------------------------------------

--CREATE INDEX IX_Products_IsActive  ON Products (IsActive);
--CREATE INDEX IX_Orders_OrderDate   ON Orders (OrderDate);

----------------------------------------------------------
-- Summary
----------------------------------------------------------
SELECT 
    (SELECT COUNT(*) FROM Customers)  AS Customers,
    (SELECT COUNT(*) FROM Products)   AS Products,
    (SELECT COUNT(*) FROM Orders)     AS Orders,
    (SELECT COUNT(*) FROM OrderItems) AS OrderItems;

PRINT 'SampleStoreLarge database successfully created.';
