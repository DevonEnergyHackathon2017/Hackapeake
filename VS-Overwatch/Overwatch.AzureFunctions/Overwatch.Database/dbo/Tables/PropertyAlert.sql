CREATE TABLE [dbo].[PropertyAlert] (
    [id]                    INT            IDENTITY (1, 1) NOT NULL,
    [imageUrl]              NVARCHAR (MAX) NULL,
    [createTimestamp]       DATETIME       NULL,
    [propertyId]            INT            NULL,
    [propertyAlertStatusId] INT            NOT NULL,
    [predictionJson]        NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([propertyAlertStatusId]) REFERENCES [dbo].[PropertyAlertStatus] ([id]),
    FOREIGN KEY ([propertyId]) REFERENCES [dbo].[Property] ([id])
);

