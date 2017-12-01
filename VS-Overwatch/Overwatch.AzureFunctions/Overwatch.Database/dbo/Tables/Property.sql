CREATE TABLE [dbo].[Property] (
    [id]                     INT            IDENTITY (1, 1) NOT NULL,
    [propertyNumber]         NVARCHAR (100) NOT NULL,
    [propertyName]           NVARCHAR (100) NOT NULL,
    [latitude]               FLOAT (53)     NULL,
    [longitude]              FLOAT (53)     NULL,
    [sourceName]             NVARCHAR (100) NULL,
    [approximateNumOfPeople] FLOAT (53)     NULL,
    [propertyStatusId]       INT            NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([propertyStatusId]) REFERENCES [dbo].[PropertyStatus] ([id]),
    CONSTRAINT [UniqueKeyPropertyNumber] UNIQUE NONCLUSTERED ([propertyNumber] ASC)
);

