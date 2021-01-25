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


insert into JobStatus  values ('17','ประกบ')
GO