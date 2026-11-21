-- Run only in a NEW/test database. Adapt to existing schema before production.
CREATE TABLE Customers(CustomerId int IDENTITY PRIMARY KEY, Name nvarchar(150) NOT NULL, Phone nvarchar(30) NULL, Email nvarchar(256) NULL, Address nvarchar(500) NULL, OpeningBalance decimal(18,2) NOT NULL DEFAULT 0);
CREATE TABLE Suppliers(SupplierId int IDENTITY PRIMARY KEY, Name nvarchar(150) NOT NULL, Phone nvarchar(30) NULL, Email nvarchar(256) NULL, Address nvarchar(500) NULL, OpeningBalance decimal(18,2) NOT NULL DEFAULT 0);
CREATE TABLE Products(ProductId int IDENTITY PRIMARY KEY, ProductName nvarchar(200) NOT NULL, SKU nvarchar(80) NULL UNIQUE, PurchasePrice decimal(18,2) NOT NULL DEFAULT 0, SalePrice decimal(18,2) NOT NULL DEFAULT 0, StockQuantity decimal(18,3) NOT NULL DEFAULT 0, ReorderLevel decimal(18,3) NOT NULL DEFAULT 0);
CREATE TABLE Transactions(TransactionId int IDENTITY PRIMARY KEY, CustomerId int NULL REFERENCES Customers(CustomerId), SupplierId int NULL REFERENCES Suppliers(SupplierId), TransactionType nvarchar(30) NOT NULL, Amount decimal(18,2) NOT NULL CHECK(Amount>=0), TransactionDate datetime2 NOT NULL, Description nvarchar(500) NULL, CONSTRAINT CK_Transactions_OneParty CHECK ((CustomerId IS NOT NULL AND SupplierId IS NULL) OR (CustomerId IS NULL AND SupplierId IS NOT NULL)));
CREATE TABLE Payments(PaymentId int IDENTITY PRIMARY KEY, CustomerId int NULL REFERENCES Customers(CustomerId), SupplierId int NULL REFERENCES Suppliers(SupplierId), Amount decimal(18,2) NOT NULL CHECK(Amount>0), PaymentType nvarchar(20) NOT NULL CHECK(PaymentType IN ('Received','Paid')), PaymentDate datetime2 NOT NULL, Description nvarchar(500) NULL, CONSTRAINT CK_Payments_OneParty CHECK ((CustomerId IS NOT NULL AND SupplierId IS NULL) OR (CustomerId IS NULL AND SupplierId IS NOT NULL)));
CREATE TABLE Sales(SaleId int IDENTITY PRIMARY KEY, CustomerId int NULL REFERENCES Customers(CustomerId), SaleDate datetime2 NOT NULL, TotalAmount decimal(18,2) NOT NULL DEFAULT 0);
CREATE TABLE Purchases(PurchaseId int IDENTITY PRIMARY KEY, SupplierId int NULL REFERENCES Suppliers(SupplierId), PurchaseDate datetime2 NOT NULL, TotalAmount decimal(18,2) NOT NULL DEFAULT 0);
CREATE INDEX IX_Transactions_CustomerDate ON Transactions(CustomerId,TransactionDate);
CREATE INDEX IX_Transactions_SupplierDate ON Transactions(SupplierId,TransactionDate);
CREATE INDEX IX_Payments_Date ON Payments(PaymentDate);
CREATE INDEX IX_Sales_Date ON Sales(SaleDate);
CREATE INDEX IX_Purchases_Date ON Purchases(PurchaseDate);
