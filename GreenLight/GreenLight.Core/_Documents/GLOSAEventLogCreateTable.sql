USE [glosaanalyticsdevelopmentdb]
GO

/****** Object: Table [dbo].[GLOSAEventLog] Script Date: 28/01/2018 22:33:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GLOSAEventLog] (
    [id]                  NVARCHAR (255)     NOT NULL,
    [createdAt]           DATETIMEOFFSET (3) NOT NULL,
    [updatedAt]           DATETIMEOFFSET (3) NULL,
    [version]             ROWVERSION         NOT NULL,
    [deleted]             BIT                NULL,
    [Longitude]           FLOAT (53)         NULL,
    [Latitude]            FLOAT (53)         NULL,
    [VehicleId]           NVARCHAR (MAX)     NULL,
    [DeviceTime]          DATETIMEOFFSET (3) NULL,
    [TimeOffset]          FLOAT (53)         NULL,
    [IntersectionId]      NVARCHAR (MAX)     NULL,
    [Event]               NVARCHAR (MAX)     NULL,
    [GPSHistory]          NVARCHAR (MAX)     NULL,
    [Distance]            FLOAT (53)         NULL,
    [Value]               NVARCHAR (MAX)     NULL,
    [AdvisoryEnabled]     BIT                NULL,
    [RouteId]             NVARCHAR (MAX)     NULL,
    [RouteSession]        NVARCHAR (MAX)     NULL,
    [Speed]               FLOAT (53)         NULL,
    [CalculationAdvisory] NVARCHAR (MAX)     NULL,
    [SPAT]                NVARCHAR (MAX)     NULL,
    [Latency]             FLOAT (53)         NULL
);


GO
CREATE CLUSTERED INDEX [createdAt]
    ON [dbo].[GLOSAEventLog]([createdAt] ASC);


GO
ALTER TABLE [dbo].[GLOSAEventLog]
    ADD CONSTRAINT [DF_GLOSAEventLog_id] DEFAULT (CONVERT([nvarchar](255),newid(),(0))) FOR [id];


GO
ALTER TABLE [dbo].[GLOSAEventLog]
    ADD CONSTRAINT [DF_GLOSAEventLog_createdAt] DEFAULT (CONVERT([datetimeoffset](3),sysutcdatetime(),(0))) FOR [createdAt];


