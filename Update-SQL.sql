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
	[น้ำหนักส่งประกบ] [decimal](18, 2) NULL,
	[น้ำหนักหลังประกบ] [decimal](18, 2) NULL,
	[เศษสร้อย] [decimal](18, 2) NULL,
	[น้ำหนักwire_roll] [decimal](18, 2) NULL,
	[น้ำหนักroll] [decimal](18, 2) NULL,
	[น้ำหนักwire] [decimal](18, 2) NULL,
	[คืนwire_roll] [decimal](18, 2) NULL,
	[คืนเศษwire] [decimal](18, 2) NULL,
	[สรุปหาย] [decimal](18, 2) NULL,
	[รวมใช้นส] [decimal](18, 2) NULL,
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
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (0, N'ลงทอง')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (1, N'ชักลวด')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (2, N'ตัดห่วง')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (3, N'ส่งร้อย')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (4, N'แต่งสร้อย')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (12, N'ลดรู')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (13, N'ทอ')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (14, N'เชื่อม')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (15, N'เลื่อย')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (17, N'ประกบ')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (20, N'ลงเตา')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (21, N'ตั้งเวลา')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (22, N'เสร็จสิ้น')
GO
INSERT [dbo].[JobStatus] ([Status], [Descr]) VALUES (99, N'งานใหม่')
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
                  CASE dbo.ProcessKiln.LastStatus WHEN 0 THEN 'ใหม่' WHEN 1 THEN 'ตั้งเวลา' ELSE 'เสร็จสิ้น' END AS LastStatus, dbo.ProcessKiln.UserID, dbo.ProcessKiln.UserUpdate, dbo.Jobs.JobType, dbo.Jobs.Barcode, 
                  dbo.Jobs.LastStatus AS JobLastStatus, dbo.Jobs.CreateUser AS JobCreateUser, dbo.Jobs.CreateUpdate AS JobCreateUpdate, dbo.Jobs.UserID AS JobUserID, dbo.Jobs.UserUpdate AS JobUserUpdate, dbo.Barcode.BARCODE_SCAN, 
                  dbo.Barcode.BARCODE_NAME, dbo.ProcessTimer.Date, dbo.ProcessTimer.HH, dbo.ProcessTimer.MM, dbo.ProcessTimer.SS, dbo.Jobs.BarcodeText, dbo.JobStatus.Descr AS StatusName, dbo.ProcessKiln.Process12
FROM     dbo.Jobs INNER JOIN
                  dbo.ProcessKiln ON dbo.Jobs.Lot = dbo.ProcessKiln.Lot AND dbo.Jobs.Barcode = dbo.ProcessKiln.Barcode AND dbo.Jobs.BarcodeText = dbo.ProcessKiln.BarcodeText INNER JOIN
                  dbo.Barcode ON dbo.Jobs.Barcode = dbo.Barcode.Barcode INNER JOIN
                  dbo.Lot ON dbo.Jobs.Lot = dbo.Lot.Lot INNER JOIN
                  dbo.JobStatus ON dbo.Jobs.LastStatus = dbo.JobStatus.Status LEFT OUTER JOIN
                  dbo.ProcessTimer ON dbo.ProcessKiln.Timer = dbo.ProcessTimer.Name
GO


