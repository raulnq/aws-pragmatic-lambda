GO

CREATE TABLE $schema$.[Orders] (
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    [ClientId] UNIQUEIDENTIFIER NOT NULL,
    [Address] nvarchar(500) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [PaymentMethod] nvarchar(20) NOT NULL,
    [DeliveryDate] datetimeoffset NOT NULL,
    [Total] decimal(19,4) NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId])
);

GO

CREATE TABLE $schema$.[OrderItems] (
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Quantity] decimal(19,4) NOT NULL,
    [Price] decimal(19,4) NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([OrderId],[ProductId])
);