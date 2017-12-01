CREATE TABLE [dbo].[PropertyAlertStatus] (
    [id]                  INT            IDENTITY (1, 1) NOT NULL,
    [propertyAlertStatus] NVARCHAR (200) NOT NULL,
    [severityId]          INT            DEFAULT ('1') NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([severityId]) REFERENCES [dbo].[Severity] ([id])
);

