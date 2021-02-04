USE [Goldsmith]
GO

/****** Object:  Table [dbo].[ProcessJoint]    Script Date: 1/23/2021 11:45:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProcessJoint](
	[Lot] [bigint] NOT NULL,
	[Barcode] [bigint] NULL,
	[BarcodeText] [varchar](15) NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[CreateUpdate] [datetime] NULL,
	[LastStatus] [int] NULL,
	[UserID] [nvarchar](10) NULL,
	[UserUpdate] [datetime] NULL,
	[���˹ѡ�觻�С�] [decimal](18, 2) NULL,
	[���˹ѡ��ѧ��С�] [decimal](18, 2) NULL,
	[��������] [decimal](18, 2) NULL,
	[���˹ѡwire_roll] [decimal](18, 2) NULL,
	[���˹ѡroll] [decimal](18, 2) NULL,
	[���˹ѡwire] [decimal](18, 2) NULL,
	[�׹wire_roll] [decimal](18, 2) NULL,
	[�׹���wire] [decimal](18, 2) NULL,
	[��ػ���] [decimal](18, 2) NULL,
	[������] [decimal](18, 2) NULL,
 CONSTRAINT [PK_ProcessJoint] PRIMARY KEY CLUSTERED 
(
	[Lot] ASC,
	[BarcodeText] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProcessJoint] ADD  CONSTRAINT [DF_ProcessJoint_LastStatus]  DEFAULT ((0)) FOR [LastStatus]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[JobStatus]') AND type in (N'U'))
DROP TABLE [dbo].[JobStatus]
GO

CREATE TABLE [dbo].[JobStatus](
	[Status] [int] NOT NULL,
	[Descr] [nvarchar](50) NULL,
 CONSTRAINT [PK_JobStatus] PRIMARY KEY CLUSTERED 
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (0, N'ŧ�ͧ')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (1, N'�ѡ�Ǵ')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (2, N'�Ѵ��ǧ')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (3, N'������')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (4, N'�������')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (12, N'Ŵ��')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (13, N'��')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (14, N'�����')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (15, N'������')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (17, N'��С�')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (20, N'ŧ��')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (21, N'�������')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (22, N'�������')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (99, N'�ҹ����')
GO

GO


ALTER TABLE ProcessKiln
ADD Process12 decimal(18, 2) NOT NULL DEFAULT(0)
GO


--update ProcessKiln set Process12 = 0  where Process12 is null

ALTER VIEW [dbo].[vProcessKiln]
AS
SELECT dbo.ProcessKiln.Lot, dbo.Lot.Descr AS LotName, dbo.ProcessKiln.Timer, dbo.ProcessKiln.CreateUser, dbo.ProcessKiln.CreateUpdate, dbo.ProcessKiln.Process1, dbo.ProcessKiln.Process2, dbo.ProcessKiln.Process3, 
                  dbo.ProcessKiln.Process4, dbo.ProcessKiln.Process5, dbo.ProcessKiln.Process6, dbo.ProcessKiln.Process7, dbo.ProcessKiln.Process8, dbo.ProcessKiln.Process9, dbo.ProcessKiln.Process10, dbo.ProcessKiln.Process11, 
                  CASE dbo.ProcessKiln.LastStatus WHEN 0 THEN '����' WHEN 1 THEN '�������' ELSE '�������' END AS LastStatus, dbo.ProcessKiln.UserID, dbo.ProcessKiln.UserUpdate, dbo.Jobs.JobType, dbo.Jobs.Barcode, 
                  dbo.Jobs.LastStatus AS JobLastStatus, dbo.Jobs.CreateUser AS JobCreateUser, dbo.Jobs.CreateUpdate AS JobCreateUpdate, dbo.Jobs.UserID AS JobUserID, dbo.Jobs.UserUpdate AS JobUserUpdate, dbo.Barcode.BARCODE_SCAN, 
                  dbo.Barcode.BARCODE_NAME, dbo.ProcessTimer.Date, dbo.ProcessTimer.HH, dbo.ProcessTimer.MM, dbo.ProcessTimer.SS, dbo.Jobs.BarcodeText, dbo.JobStatus.Descr AS StatusName, dbo.ProcessKiln.Process12
FROM     dbo.Jobs INNER JOIN
                  dbo.ProcessKiln ON dbo.Jobs.Lot = dbo.ProcessKiln.Lot AND dbo.Jobs.Barcode = dbo.ProcessKiln.Barcode AND dbo.Jobs.BarcodeText = dbo.ProcessKiln.BarcodeText INNER JOIN
                  dbo.Barcode ON dbo.Jobs.Barcode = dbo.Barcode.Barcode INNER JOIN
                  dbo.Lot ON dbo.Jobs.Lot = dbo.Lot.Lot INNER JOIN
                  dbo.JobStatus ON dbo.Jobs.LastStatus = dbo.JobStatus.Status LEFT OUTER JOIN
                  dbo.ProcessTimer ON dbo.ProcessKiln.Timer = dbo.ProcessTimer.Name
GO


