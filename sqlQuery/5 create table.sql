CREATE TABLE schedulerReportLog
(
[Id] [int] IDENTITY(1,1) NOT NULL,
[ReportType] NVARCHAR(255),
[SendDate] DATETIME,
[contain] NVARCHAR(max)
)