USE [glosaanalyticsdevelopmentdb]
GO

/****** Object: Table [dbo].[GLOSAMonitoringLog] Script Date: 27/01/2018 14:04:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GLOSAMonitoringLog] (
    [id]        NVARCHAR (255)     NOT NULL,
    [createdAt] DATETIMEOFFSET (3) NOT NULL,
    [updatedAt] DATETIMEOFFSET (3) NULL,
    [version]   ROWVERSION         NOT NULL,
    [deleted]   BIT                NULL,
    [URL]       NVARCHAR (MAX)     NULL,
    [Method]    NVARCHAR (MAX)     NULL,
    [Latency]   FLOAT (53)         NULL,
    [Event]     NVARCHAR (MAX)     NULL,
    [Value]     NVARCHAR (MAX)     NULL
);


GO
CREATE CLUSTERED INDEX [createdAt]
    ON [dbo].[GLOSAMonitoringLog]([createdAt] ASC);


GO
ALTER TABLE [dbo].[GLOSAMonitoringLog]
    ADD CONSTRAINT [DF_GLOSAMonitoringLog_id] DEFAULT (CONVERT([nvarchar](255),newid(),(0))) FOR [id];


GO
ALTER TABLE [dbo].[GLOSAMonitoringLog]
    ADD CONSTRAINT [DF_GLOSAMonitoringLog_createdAt] DEFAULT (CONVERT([datetimeoffset](3),sysutcdatetime(),(0))) FOR [createdAt];


