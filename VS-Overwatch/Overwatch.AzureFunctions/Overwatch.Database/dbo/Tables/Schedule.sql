CREATE TABLE [dbo].[Schedule] (
    [scheduleID]         INT           NOT NULL,
    [propertyNumber]     INT           NOT NULL,
    [arrivalTime]        DATETIME      NULL,
    [departureTime]      DATETIME      NULL,
    [responsiblePerson]  VARCHAR (255) NULL,
    [responsibleCompany] VARCHAR (255) NULL,
    [reasonForVisit]     INT           NOT NULL,
    [typeOfVehicle]      INT           NOT NULL,
    [numberOfVehicles]   INT           NULL,
    [numberOfPersonnel]  INT           NULL,
    PRIMARY KEY CLUSTERED ([scheduleID] ASC),
    FOREIGN KEY ([propertyNumber]) REFERENCES [dbo].[Property] ([id]),
    FOREIGN KEY ([reasonForVisit]) REFERENCES [dbo].[VisitReason] ([visitID]),
    FOREIGN KEY ([typeOfVehicle]) REFERENCES [dbo].[VehicleType] ([vehicleID])
);

