GO

CREATE TABLE $schema$.[ClientRequests] (
    [ClientRequestId] UNIQUEIDENTIFIER NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [Address] nvarchar(500) NOT NULL,
    [RegisteredAt] datetimeoffset NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [ApprovedAt] datetimeoffset NULL,
    [RejectedAt] datetimeoffset NULL,
    CONSTRAINT [PK_ClientRequests] PRIMARY KEY ([ClientRequestId]),
);

GO