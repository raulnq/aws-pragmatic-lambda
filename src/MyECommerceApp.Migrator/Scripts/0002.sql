GO

CREATE TABLE $schema$.[Clients] (
    [ClientId] UNIQUEIDENTIFIER NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [Address] nvarchar(500) NOT NULL
    CONSTRAINT [PK_Clients] PRIMARY KEY ([ClientId])
);

GO

CREATE TABLE $schema$.[Products] (
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    [Price] decimal(19,4) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [IsEnabled] bit NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId])
);

GO

CREATE TABLE $schema$.[ShoppingCartItems] (
    [ShoppingCartItemId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Quantity] decimal(19,4) NOT NULL,
    [ClientId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ShoppingCartItems] PRIMARY KEY ([ShoppingCartItemId])
);