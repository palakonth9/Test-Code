
  use Goldsmith
  go

ALTER TABLE ProcessKiln
ADD Process12 decimal(18, 2) NOT NULL DEFAULT(0)
GO


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


