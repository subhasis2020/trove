--USE [master]
--GO
--/****** Object:  Database [TroveProduction]    Script Date: 7/30/2020 7:55:17 AM ******/
--CREATE DATABASE [TroveProduction]
-- CONTAINMENT = NONE
-- ON  PRIMARY 
--( NAME = N'FoundryDev', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\TroveProduction.mdf' , SIZE = 404480KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
-- LOG ON 
--( NAME = N'FoundryDev_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\TroveProduction_log.ldf' , SIZE = 1655488KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
--GO
--ALTER DATABASE [TroveProduction] SET COMPATIBILITY_LEVEL = 120
--GO
--IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
--begin
--EXEC [TroveProduction].[dbo].[sp_fulltext_database] @action = 'enable'
--end
--GO
--ALTER DATABASE [TroveProduction] SET ANSI_NULL_DEFAULT OFF 
--GO
--ALTER DATABASE [TroveProduction] SET ANSI_NULLS OFF 
--GO
--ALTER DATABASE [TroveProduction] SET ANSI_PADDING OFF 
--GO
--ALTER DATABASE [TroveProduction] SET ANSI_WARNINGS OFF 
--GO
--ALTER DATABASE [TroveProduction] SET ARITHABORT OFF 
--GO
--ALTER DATABASE [TroveProduction] SET AUTO_CLOSE OFF 
--GO
--ALTER DATABASE [TroveProduction] SET AUTO_SHRINK OFF 
--GO
--ALTER DATABASE [TroveProduction] SET AUTO_UPDATE_STATISTICS ON 
--GO
--ALTER DATABASE [TroveProduction] SET CURSOR_CLOSE_ON_COMMIT OFF 
--GO
--ALTER DATABASE [TroveProduction] SET CURSOR_DEFAULT  GLOBAL 
--GO
--ALTER DATABASE [TroveProduction] SET CONCAT_NULL_YIELDS_NULL OFF 
--GO
--ALTER DATABASE [TroveProduction] SET NUMERIC_ROUNDABORT OFF 
--GO
--ALTER DATABASE [TroveProduction] SET QUOTED_IDENTIFIER OFF 
--GO
--ALTER DATABASE [TroveProduction] SET RECURSIVE_TRIGGERS OFF 
--GO
--ALTER DATABASE [TroveProduction] SET  DISABLE_BROKER 
--GO
--ALTER DATABASE [TroveProduction] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
--GO
--ALTER DATABASE [TroveProduction] SET DATE_CORRELATION_OPTIMIZATION OFF 
--GO
--ALTER DATABASE [TroveProduction] SET TRUSTWORTHY OFF 
--GO
--ALTER DATABASE [TroveProduction] SET ALLOW_SNAPSHOT_ISOLATION OFF 
--GO
--ALTER DATABASE [TroveProduction] SET PARAMETERIZATION SIMPLE 
--GO
--ALTER DATABASE [TroveProduction] SET READ_COMMITTED_SNAPSHOT OFF 
--GO
--ALTER DATABASE [TroveProduction] SET HONOR_BROKER_PRIORITY OFF 
--GO
--ALTER DATABASE [TroveProduction] SET RECOVERY FULL 
--GO
--ALTER DATABASE [TroveProduction] SET  MULTI_USER 
--GO
--ALTER DATABASE [TroveProduction] SET PAGE_VERIFY CHECKSUM  
--GO
--ALTER DATABASE [TroveProduction] SET DB_CHAINING OFF 
--GO
--ALTER DATABASE [TroveProduction] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
--GO
--ALTER DATABASE [TroveProduction] SET TARGET_RECOVERY_TIME = 0 SECONDS 
--GO
--ALTER DATABASE [TroveProduction] SET DELAYED_DURABILITY = DISABLED 
--GO
--ALTER DATABASE [TroveProduction] SET QUERY_STORE = OFF
--GO
USE [TroveProduction]
GO
--/****** Object:  User [TroveProductionUser]    Script Date: 7/30/2020 7:55:17 AM ******/
--//CREATE USER [FoundryStage1] FOR LOGIN [FoundryStage1] WITH DEFAULT_SCHEMA=[dbo]
--GO
--/****** Object:  User [FoundryLog1]    Script Date: 7/30/2020 7:55:17 AM ******/
----CREATE USER [FoundryLog1] FOR LOGIN [FoundryLog1] WITH DEFAULT_SCHEMA=[dbo]
----GO
----/****** Object:  User [FoundryLog]    Script Date: 7/30/2020 7:55:17 AM ******/
----CREATE USER [FoundryLog] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
--GO
--ALTER ROLE [db_owner] ADD MEMBER [FoundryStage1]
--GO
--ALTER ROLE [db_datareader] ADD MEMBER [FoundryLog1]
--GO
--ALTER ROLE [db_owner] ADD MEMBER [FoundryLog]
GO
/****** Object:  UserDefinedTableType [dbo].[BinFile]    Script Date: 7/30/2020 7:55:17 AM ******/
CREATE TYPE [dbo].[BinFile] AS TABLE(
	[BinStart] [bigint] NULL,
	[BinEnd] [bigint] NULL,
	[code] [varchar](50) NULL
)
GO
/****** Object:  UserDefinedFunction [dbo].[ConvertStringToTable]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ConvertStringToTable](@input AS nvarchar(max) )

RETURNS

      @Result TABLE(Value BIGINT)

AS

BEGIN

      DECLARE @str VARCHAR(20)

      DECLARE @ind Int

      IF(@input is not null)

      BEGIN

            SET @ind = CharIndex(',',@input)

            WHILE @ind > 0

            BEGIN

                  SET @str = SUBSTRING(@input,1,@ind-1)

                  SET @input = SUBSTRING(@input,@ind+1,LEN(@input)-@ind)

                  INSERT INTO @Result values (@str)

                  SET @ind = CharIndex(',',@input)

            END

            SET @str = @input

            INSERT INTO @Result values (@str)

      END

      RETURN

END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_getClosingStatus]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_getClosingStatus] 
(
	-- Add the parameters for the function here
	@OrganisationId int
)
RETURNS varchar(50)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @status varchar(50)='Closed Now'

	DECLARE @dt datetime = GETDATE(), @currentday varchar(20),@orgOpenTime time, @orgClosingTime time, @diffhr int, @orgClosingDate datetime,
	@orgOpenDate datetime;
	Select @currentday=DATENAME(weekday, @dt)
	Select @orgClosingTime = ClosedTime from OrganisationSchedule WHERE OrganisationId = @OrganisationId and WorkingDay = @currentday and IsActive = 1 and IsDeleted = 0
	Select @orgOpenTime = OpenTime from OrganisationSchedule WHERE OrganisationId = @OrganisationId and WorkingDay = @currentday and IsActive = 1 and IsDeleted = 0
	select @orgOpenDate = CONVERT(DATETIME, CONVERT(CHAR(8), GETDATE(), 112) + ' ' + CONVERT(CHAR(8), @orgOpenTime, 108))
	select @orgClosingDate = CONVERT(DATETIME, CONVERT(CHAR(8), GETDATE(), 112) + ' ' + CONVERT(CHAR(8), @orgClosingTime, 108))
	Select @diffhr = DATEDIFF(hour, @dt, @orgClosingDate)
	if(@dt>=@orgOpenDate and @dt<= @orgClosingDate and @diffhr > 1)
		select @status = 'Open'
	else if(@dt>=@orgOpenDate and @dt<= @orgClosingDate and @diffhr > 0 and @diffhr <=1)
		select @status = 'Closing Soon'
	-- Return the result of the function
	RETURN @status

END


GO
/****** Object:  UserDefinedFunction [dbo].[fn_getDistance]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Select  [dbo].[fn_getDistance]('-122.609617, 49.572645', '-122.5611372, 49.2128279') 
-- Select  [dbo].[fn_getDistance]('75.857277, 30.900965', '-122.5611372, 49.2128279')  
CREATE FUNCTION [dbo].[fn_getDistance]   
(  
 @latlongUser1 varchar(300), @latlongMerchant1 varchar(300)
)  
RETURNS float  
AS  
BEGIN  

DECLARE @distance float = 0.0,@geo1 geography,@geo2 geography , @RoughDistance float
DECLARE @Userlong NVARCHAR(30), @Userlat NVARCHAR(30), @MerLong NVARCHAR(30), @MerLat NVARCHAR(30)
		SET @Userlong = SUBSTRING(@latlongUser1, 0, CHARINDEX (',', @latlongUser1))
		SET @Userlat = LTRIM(SUBSTRING(@latlongUser1, CHARINDEX (',', @latlongUser1) +1, LEN(@latlongUser1)))
		--SELECT @long as Long, @lat as Lat

SET @geo1= geography::Point(Convert(float,@Userlat), Convert(float,@Userlong), 4326)
SET @MerLong = SUBSTRING(@latlongMerchant1, 0, CHARINDEX (',', @latlongMerchant1))
	SET	 @MerLat = LTRIM(SUBSTRING(@latlongMerchant1, CHARINDEX (',', @latlongMerchant1) +1, LEN(@latlongMerchant1)))


SET @geo2= geography::Point(Convert(float,@MerLat), Convert(float,@MerLong), 4326)


select @distance=ROUND(@geo1.STDistance(@geo2)/1000, 2)
 return @distance    
--DECLARE @UserLatLong TABLE(Id INT IDENTITY(1, 1),
--                   SValue VARCHAR(4000))
-- -- Declare the return variable here  
-- DECLARE @distance float = 0,@geo1 geography,@geo2 geography  
-- set @geo1 = geography::STPointFromText('POINT(' + Replace(@latlon1,',',' ') + ')', 4326)  
-- set @geo2 = geography::STPointFromText('POINT(' + Replace(@latlon2,',',' ') + ')', 4326)  
-- select @distance=@geo1.STDistance(@geo2)/1000  
-- return @distance    
END  
  
  
GO
/****** Object:  UserDefinedFunction [dbo].[fn_getDistanceCheckDummy]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Select  [dbo].[fn_getDistanceCheckDummy]('-122.567817,49.216759', '-122.561116, 49.212884') 
--Select  [dbo].[fn_getDistanceCheckDummy]('49.21237','-122.5612666','49.2128279', '-122.5611372')  -- (First Parameter UserLocation, Second PArameter Merchant)

CREATE FUNCTION [dbo].[fn_getDistanceCheckDummy]   
(  
 @latlongUser1 varchar(300), @latlongMerchant1 varchar(300)
)  
RETURNS Float  
AS  
BEGIN  

DECLARE @distance float = 0.0,@geo1 geography,@geo2 geography 
DECLARE @Userlong NVARCHAR(30), @Userlat NVARCHAR(30), @MerLong NVARCHAR(30), @MerLat NVARCHAR(30)
		SET @Userlong = SUBSTRING(@latlongUser1, 0, CHARINDEX (',', @latlongUser1))
		SET @Userlat = LTRIM(SUBSTRING(@latlongUser1, CHARINDEX (',', @latlongUser1) +1, LEN(@latlongUser1)))
		--SELECT @long as Long, @lat as Lat

SET @geo1= geography::Point(@Userlat, @Userlong, 4326)
SET @MerLong = SUBSTRING(@latlongMerchant1, 0, CHARINDEX (',', @latlongMerchant1))
	SET	 @MerLat = LTRIM(SUBSTRING(@latlongMerchant1, CHARINDEX (',', @latlongMerchant1) +1, LEN(@latlongMerchant1)))


SET @geo2= geography::Point(@MerLat, @MerLong, 4326)

select @distance=@geo1.STDistance(@geo2)--/1000

 

 -- Declare the return variable here  
 -- 
 --set @geo1 = geography::STPointFromText('POINT(' + Replace(@latlon1,',',' ') + ')', 4326)  
 --set @geo2 = geography::STPointFromText('POINT(' + Replace(@latlon2,',',' ') + ')', 4326)  
 --select @distance=@geo1.STDistance(@geo2)/1000  
 return @distance    
END  
  
GO
/****** Object:  UserDefinedFunction [dbo].[fn_getMerchantActivity]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[fn_getMerchantActivity] 
(
	@OrganisationId int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @activityStatus int, @closingStatus varchar(50), @lastTransactionTime datetime

	SELECT @closingStatus = dbo.fn_getClosingStatus(@OrganisationId)

	SELECT @lastTransactionTime = transactionDate from UserTransactionInfo WHERE creditUserId = @OrganisationId 

	IF(@closingStatus = 'Open' AND DATEDIFF(second, @lastTransactionTime, GETDATE()) / 3600.0 < 1)
	BEGIN
		SET @activityStatus = 1
	END
	ELSE IF(@closingStatus = 'Open' AND DATEDIFF(second, @lastTransactionTime, GETDATE()) / 3600.0 >= 1 AND DATEDIFF(second, @lastTransactionTime, GETDATE()) / 3600.0 < 4)
	BEGIN
		SET @activityStatus = 2
	END
	ELSE
	BEGIN
		SET @activityStatus = 0
	END
	-- Return the result of the function
	RETURN @activityStatus

END



GO
/****** Object:  UserDefinedFunction [dbo].[fn_getOfferValue]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_getOfferValue]
(
	@offerId int,@offerBannerId int 
)
RETURNS varchar(200)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @offerValue varchar(200)=''
	
	SELECT @offerValue = 
	(CASE 
		WHEN @offerBannerId = 1 THEN Cast(DiscountInPercentage AS varchar(200))
		WHEN @offerBannerId = 2 THEN Cast(DiscountInPercentage AS varchar(200))
		WHEN @offerBannerId = 3 THEN Cast(DiscountInPercentage AS varchar(200))
		ELSE ''
	END)
	FROM Offer WHERE Id = @offerId

	RETURN @offerValue

END
GO
/****** Object:  UserDefinedFunction [dbo].[fnGetFormattedNthDate]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnGetFormattedNthDate]
(
@Date Datetime=NULL
)
Returns NVARCHAR(50)
AS
BEGIN
IF(@Date IS NOT NULL)
BEGIN
RETURN FORMAT(@Date,'MMMM '
+'d'
+IIF(DAY(@Date) IN (1,21,31),'''st'''
,IIF(DAY(@Date) IN (2,22),'''nd'''
,IIF(DAY(@Date) IN (3,23),'''rd''','''th''')))
+' yyyy') 
END
RETURN ''
END
GO
/****** Object:  UserDefinedFunction [dbo].[fnGetUserBalanceDetail]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnGetUserBalanceDetail]
(
@userId int,
@userType int,
@planId int,
@accountId int,
@accountTypeId int
)
RETURNS DECIMAL(18,2)
	AS
	BEGIN
		DECLARE @CreditedAmount Decimal(18,2),@DebitedAmount Decimal(18,2),@Output Decimal  -- AND tuf.PlanId=@planId 
	SET @CreditedAmount= (SELECT ISNULL((SELECT ISNULL(SUM(tuf.transactionAmount),0) FROM UserTransactionInfo tuf Where tuf.CreditUserId=@userId AND tuf.CreditTransactionUserType=@userType AND tuf.IsActive=1 AND tuf.IsDeleted=0 AND (@accountId=0 OR tuf.ProgramAccountId=@accountId) AND tuf.accountTypeId=@accountTypeId
	AND (@planId = 0 OR tuf.PlanId=@planId) Group BY tuf.transactionAmount),0))
	SET @DebitedAmount= (SELECT ISNULL((SELECT ISNULL(SUM(tuf.transactionAmount),0) FROM UserTransactionInfo tuf Where tuf.DebitUserID=@userId  AND tuf.DebitTransactionUserType=@userType AND tuf.IsActive=1 AND tuf.IsDeleted=0 AND (@accountId=0 OR tuf.ProgramAccountId=@accountId) AND tuf.accountTypeId=@accountTypeId
	AND (@planId=0 OR tuf.PlanId=@planId ) Group BY tuf.transactionAmount),0))
		--SET @CreditedAmount=@CreditedAmount
		--SET @DebitedAmount=CAST(@DebitedAmount AS DECIMAL(8, 2))
	SET @Output=(@CreditedAmount-@DebitedAmount)
	Return @Output
	END


	
GO
/****** Object:  UserDefinedFunction [dbo].[fnGetUserBalanceDetailWithProgram]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnGetUserBalanceDetailWithProgram]
(
@userId int,
@userType int,
@planId int,
@accountId int,
@accountTypeId int,
@programId int
)
RETURNS DECIMAL(18,2)
	AS
	BEGIN
		DECLARE @CreditedAmount Decimal(18,2),@DebitedAmount Decimal(18,2),@Output Decimal  -- AND tuf.PlanId=@planId 
	SET @CreditedAmount= (SELECT ISNULL((SELECT ISNULL(SUM(tuf.transactionAmount),0) FROM UserTransactionInfo tuf Where tuf.CreditUserId=@userId 
	AND tuf.CreditTransactionUserType=@userType AND tuf.IsActive=1 AND tuf.IsDeleted=0 AND (@accountId=0 OR tuf.ProgramAccountId=@accountId) AND tuf.accountTypeId=@accountTypeId
	AND (@planId = 0 OR tuf.PlanId=@planId) AND (@programId = 0 OR tuf.programId=@programId) Group BY tuf.CreditUserId),0))
	SET @DebitedAmount= (SELECT ISNULL((SELECT ISNULL(SUM(tuf.transactionAmount),0) FROM UserTransactionInfo tuf Where tuf.DebitUserID=@userId 
	 AND tuf.DebitTransactionUserType=@userType AND tuf.IsActive=1 AND tuf.IsDeleted=0 AND (@accountId=0 OR tuf.ProgramAccountId=@accountId) AND tuf.accountTypeId=@accountTypeId
	AND (@planId=0 OR tuf.PlanId=@planId ) AND (@programId=0 OR tuf.programId=@programId) Group BY tuf.DebitUserID),0))
	
	SET @Output=(@CreditedAmount-@DebitedAmount)
	Return @Output
	END


GO
/****** Object:  UserDefinedFunction [dbo].[GetCurrentWeekDates]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


 
CREATE FUNCTION [dbo].[GetCurrentWeekDates] ( @minDate_Pass Datetime, @maxDate_Pass Datetime)

RETURNS  @Result TABLE(S_No INT,WeekDates datetime NOT NULL, WeekDayName NVARCHAR(30) NOT NULL)

AS

begin

    --DECLARE @minDate DATETIME, @maxDate DATETIME
    --SET @minDate = CONVERT(Datetime, @minDate_Str,103)
    --SET @maxDate = CONVERT(Datetime, @maxDate_Str,103)


    INSERT INTO @Result(S_No,WeekDates, WeekDayName )
    SELECT 1,@minDate_Pass, CONVERT(NVARCHAR(30),DATENAME(dw,@minDate_Pass))


	Declare @i INT;
	SET @i=2
    WHILE @maxDate_Pass > @minDate_Pass
    BEGIN
		
        SET @minDate_Pass = (SELECT DATEADD(dd,1,@minDate_Pass))
        INSERT INTO @Result(S_No,WeekDates, WeekDayName )
        SELECT @i,@minDate_Pass, CONVERT(NVARCHAR(30),DATENAME(dw,@minDate_Pass))
		SET @i=@i+1;
    END
    return
end   
GO
/****** Object:  UserDefinedFunction [dbo].[GetOpenCloseTimeForOrganisation]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetOpenCloseTimeForOrganisation](@OrganizationId int)  
RETURNS NVARCHAR(100)   
AS   
-- Returns the open/close time of organization.
BEGIN  
    DECLARE @openCloseTime NVARCHAR(50);  
	DECLARE @chkTime TIME;
	Set @chkTime= CONVERT(time, GETUTCDATE())
	
			IF EXISTS(SELECT * FROM OrganisationSchedule Where IsHoliday=1 AND CONVERT(Date,HolidayDate)=CONVERT(Date,GetUTCDATE()) AND OrganisationId=@OrganizationId)
				BEGIN
					SELECT Top(1) @openCloseTime=Convert(nvarchar(15),OpenTime,100)+'-'+Convert(nvarchar(15),ClosedTime,100) FROM OrganisationSchedule 
					WHERE organisationid = @OrganizationId 
					AND IsHoliday=1 
					AND CONVERT(Date,HolidayDate)=CONVERT(Date,GetUTCDATE()) 
					AND DATENAME(weekday, HolidayDate) = DATENAME(weekday, GetUTCDATE())
					ORDER BY ABS(DATEDIFF(MINUTE, OpenTime, @chkTime)) --DESC
				END
			ELSE 
				BEGIN
					SELECT Top(1) @openCloseTime=Convert(nvarchar(15),OpenTime,100)+'-'+Convert(nvarchar(15),ClosedTime,100) FROM OrganisationSchedule 
					WHERE organisationid = @OrganizationId
					AND WorkingDay = DATENAME(weekday, GetUTCDATE())
					AND IsHoliday=0
					ORDER BY ABS(DATEDIFF(MINUTE, OpenTime, @chkTime)) --DESC
				END
	
    RETURN @openCloseTime;  
END; 

GO
/****** Object:  UserDefinedFunction [dbo].[GetOpenCloseTimeForOrganisation1]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetOpenCloseTimeForOrganisation1](@OrganizationId int)  
RETURNS NVARCHAR(100)   
AS   
-- Returns the open/close time of organization.
BEGIN  
    DECLARE @openCloseTime NVARCHAR(50);  
	DECLARE @chkTime TIME;
	Set @chkTime= '13:03:20.6200000'--CONVERT(time, GETUTCDATE())
	
			IF EXISTS(SELECT * FROM OrganisationSchedule Where IsHoliday=1 AND CONVERT(Date,HolidayDate)=CONVERT(Date,GetUTCDATE()) AND OrganisationId=@OrganizationId)
				BEGIN
					SELECT Top(1) @openCloseTime=Convert(nvarchar(15),OpenTime,100)+'-'+Convert(nvarchar(15),ClosedTime,100) FROM OrganisationSchedule 
					WHERE organisationid = @OrganizationId 
					AND IsHoliday=1 
					AND CONVERT(Date,HolidayDate)=CONVERT(Date,GetUTCDATE()) 
					AND DATENAME(weekday, HolidayDate) = DATENAME(weekday, GetUTCDATE())
					ORDER BY ABS(DATEDIFF(MINUTE, OpenTime, @chkTime)) --DESC
				END
			ELSE 
				BEGIN
					SELECT Top(1) @openCloseTime=Convert(nvarchar(15),OpenTime,100)+'-'+Convert(nvarchar(15),ClosedTime,100) FROM OrganisationSchedule 
					WHERE organisationid = @OrganizationId
					AND WorkingDay = DATENAME(weekday, GetUTCDATE())
					AND IsHoliday=0
					ORDER BY ABS(DATEDIFF(MINUTE, OpenTime, @chkTime)) --DESC
				END
	
    RETURN @openCloseTime;  
END; 

GO
/****** Object:  Table [dbo].[AccountMerchantRules]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountMerchantRules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[programAccountID] [int] NOT NULL,
	[merchantId] [int] NOT NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[accountTypeId] [int] NULL,
 CONSTRAINT [PK_AccountMerchantRules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccountMerchantRulesDetail]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountMerchantRulesDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[accountMerchantRuleId] [int] NOT NULL,
	[mealPeriod] [int] NOT NULL,
	[maxPassUsage] [decimal](18, 0) NOT NULL,
	[minPassValue] [decimal](18, 0) NULL,
	[maxPassValue] [decimal](18, 0) NULL,
	[transactionLimit] [decimal](18, 0) NULL,
 CONSTRAINT [PK_AccountMerchantRulesDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccountType]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[accountType] [varchar](200) NOT NULL,
	[description] [varchar](700) NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminProgramAccess]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminProgramAccess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ProgramTypeId] [int] NULL,
 CONSTRAINT [PK_AdminProgramAccess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenefactorProgram]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenefactorProgram](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[benefactorId] [int] NOT NULL,
	[programId] [int] NOT NULL,
	[programPackageId] [int] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_BenefactorProgram] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenefactorUsersLinking]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenefactorUsersLinking](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[benefactorId] [int] NOT NULL,
	[relationshipId] [int] NULL,
	[linkedDateTime] [datetime] NULL,
	[canViewTransaction] [bit] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[IsRequestAccepted] [bit] NULL,
	[IsInvitationSent] [bit] NULL,
 CONSTRAINT [PK_BenefactorsMapping] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BinData]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BinData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BinNumberStart] [bigint] NULL,
	[BinNumberEnd] [bigint] NULL,
	[CountryCode] [nvarchar](255) NULL,
	[Delete] [bit] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BusinessType]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](200) NOT NULL,
	[iconPath] [varchar](300) NOT NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_BusinessType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CardHolderAgreement]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CardHolderAgreement](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[programID] [int] NOT NULL,
	[cardHoldrAgreementContent] [nvarchar](max) NOT NULL,
	[versionNo] [nvarchar](200) NOT NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_CardHolderAgreement] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[User] [nvarchar](100) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY NONCLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmailTemplate]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Subject] [nvarchar](500) NOT NULL,
	[CCEmail] [nvarchar](200) NULL,
	[BCCEmail] [nvarchar](200) NULL,
	[CreatedOn] [datetime] NULL,
 CONSTRAINT [PK_EmailTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ErrorMessagesDetail]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorMessagesDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ErrorParameterType] [nvarchar](50) NOT NULL,
	[ErrorMessages] [nvarchar](200) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_ErrorMessagesDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FiservPaymentTransactionLog]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FiservPaymentTransactionLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[debitUserId] [int] NOT NULL,
	[creditUserId] [int] NOT NULL,
	[FiservRequestDate] [datetime] NULL,
	[FiservResponseDate] [datetime] NULL,
	[FiservRequestContent] [nvarchar](max) NULL,
	[FiservResponseContent] [nvarchar](max) NULL,
 CONSTRAINT [PK_FiservPaymentTransactionLog] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GatewayCardWebHookToken]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GatewayCardWebHookToken](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[creditUserId] [int] NULL,
	[debitUserId] [int] NULL,
	[Token] [nvarchar](200) NULL,
	[maskedLastDigitCard] [nvarchar](30) NULL,
	[nameOnCard] [nvarchar](100) NULL,
	[expiryMonthYear] [nvarchar](200) NULL,
	[cardBrand] [nvarchar](50) NULL,
	[last4digits] [nvarchar](10) NULL,
	[TokenReceivedDate] [datetime] NULL,
	[Nonce] [nvarchar](50) NULL,
	[ClientToken] [nvarchar](200) NULL,
	[IsCardToSave] [bit] NULL,
	[ipgFirstTransactionId] [nvarchar](50) NULL,
	[nickName] [varchar](100) NULL,
	[Bin] [bigint] NULL,
	[IsCardValid] [bit] NULL,
	[schemetransactionID] [nvarchar](100) NULL,
 CONSTRAINT [PK_GatewayCardWebHookToken] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GatewayRequestResponseLog]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GatewayRequestResponseLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[debitUserId] [int] NOT NULL,
	[creditUserId] [int] NOT NULL,
	[WebhookResponse] [nvarchar](max) NULL,
	[webhookReceivedDate] [datetime] NULL,
	[Nonce] [nvarchar](50) NULL,
	[ClientToken] [nvarchar](200) NULL,
 CONSTRAINT [PK_GatewayRequestResponseLog] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GeneralSetting]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeneralSetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KeyName] [varchar](255) NOT NULL,
	[Value] [varchar](2000) NOT NULL,
	[keyGroup] [nvarchar](50) NULL,
 CONSTRAINT [PK_GeneralSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Group]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Group](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](300) NOT NULL,
	[description] [varchar](700) NOT NULL,
	[groupType] [int] NOT NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GroupType]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[groupType] [varchar](200) NOT NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_GroupType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[I2CAccountDetail]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[I2CAccountDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CustomerId] [nvarchar](255) NULL,
	[CardNumber] [nvarchar](255) NULL,
	[ExpiryMonth] [nvarchar](10) NULL,
	[ExpiryYear] [nvarchar](5) NULL,
	[CVV2] [nvarchar](50) NULL,
	[AccountHolderUniqueId] [nvarchar](30) NULL,
	[Balance] [decimal](18, 2) NULL,
	[CardStatus] [nvarchar](30) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[NameOnCard] [nvarchar](100) NULL,
	[ReferenceId] [nvarchar](255) NULL,
 CONSTRAINT [PK_I2CAccountDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[i2cBank2CardTransfer]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[i2cBank2CardTransfer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[BankAccountId] [int] NULL,
	[CardNumber] [nvarchar](50) NULL,
	[TransferId] [nvarchar](20) NULL,
	[Amount] [numeric](18, 2) NULL,
	[TransId] [nvarchar](20) NULL,
	[Status] [int] NULL,
	[TransferComments] [nvarchar](max) NULL,
	[TransferFrequency] [nvarchar](10) NULL,
	[TransferDate] [datetime] NULL,
	[TransferEndDate] [datetime] NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_i2cBank2CardTransfer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[i2cCardBankAccount]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[i2cCardBankAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[I2cAccountDetailId] [int] NULL,
	[AccountNumber] [nvarchar](255) NULL,
	[AccountTitle] [nvarchar](100) NULL,
	[AccountType] [nvarchar](50) NULL,
	[AccountNickName] [nvarchar](100) NULL,
	[ACHType] [nvarchar](50) NULL,
	[BankName] [nvarchar](100) NULL,
	[RoutingNumber] [nvarchar](150) NULL,
	[Comments] [nvarchar](max) NULL,
	[Status] [bit] NULL,
	[UserId] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[AccountSrNo] [nvarchar](30) NULL,
 CONSTRAINT [PK_i2cCardBankAccount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[I2CLog]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[I2CLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[AccountHolderUniqueId] [nvarchar](30) NULL,
	[ApiName] [nvarchar](100) NULL,
	[ApiUrl] [nvarchar](250) NULL,
	[Request] [nvarchar](max) NULL,
	[Response] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
	[IpAddress] [nvarchar](30) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_I2CLog_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invitation]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invitation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NOT NULL,
	[PhoneNumber] [nvarchar](20) NOT NULL,
	[ImagePath] [nvarchar](500) NULL,
	[InvitationType] [int] NULL,
	[relationshipId] [int] NULL,
	[programId] [int] NULL,
	[IsRequestAccepted] [bit] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[UserAccessRoleId] [int] NULL,
 CONSTRAINT [PK_Invitation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JPOSCallLog]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JPOSCallLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[entity] [nvarchar](500) NULL,
	[httpMethod] [nvarchar](50) NULL,
	[referenceId] [nvarchar](50) NULL,
	[requestJPOSContent] [nvarchar](max) NULL,
	[responseJPOSContent] [nvarchar](max) NULL,
	[responseStatus] [bit] NULL,
	[requestDateTime] [datetime] NULL,
	[responseDateTime] [datetime] NULL,
	[ClientIpAddress] [nvarchar](500) NULL,
 CONSTRAINT [PK_JPOSCallLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantAdmins]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantAdmins](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[adminUserId] [int] NULL,
	[merchantId] [int] NULL,
 CONSTRAINT [PK_MerchantAdmins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MerchantTerminal]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MerchantTerminal](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[terminalId] [nvarchar](20) NULL,
	[terminalName] [varchar](200) NULL,
	[terminalType] [int] NULL,
	[organisationId] [int] NOT NULL,
	[Jpos_TerminalId] [nvarchar](200) NULL,
 CONSTRAINT [PK_MerchantTerminal] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotificationSettings]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationSettings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](300) NOT NULL,
	[description] [varchar](700) NOT NULL,
	[colorCode] [nvarchar](50) NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isActive] [bit] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_NotificationSettings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Offer]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Offer](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](150) NOT NULL,
	[description] [nvarchar](max) NULL,
	[programId] [int] NULL,
	[offerTypeId] [int] NOT NULL,
	[offerSubTypeId] [int] NOT NULL,
	[buyQuantity] [int] NULL,
	[getQuantity] [int] NULL,
	[visitNumber] [int] NULL,
	[freeQuantity] [int] NULL,
	[discountInPercentage] [decimal](18, 0) NULL,
	[discountInCash] [decimal](18, 0) NULL,
	[isCouponValid] [bit] NULL,
	[couponCode] [varchar](100) NULL,
	[offerValidFrom] [datetime] NULL,
	[offerValidTill] [datetime] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[OfferDayName] [nvarchar](100) NULL,
	[MerchantId] [int] NULL,
 CONSTRAINT [PK_Offer] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfferCode]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfferCode](
	[id] [int] NOT NULL,
	[offerName] [varchar](100) NOT NULL,
	[offerIconPath] [varchar](300) NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_OfferCode] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfferGroup]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfferGroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[offerId] [int] NOT NULL,
	[groupId] [int] NOT NULL,
 CONSTRAINT [PK_OfferGroup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfferMerchant]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfferMerchant](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[offerId] [int] NOT NULL,
	[merchantId] [int] NULL,
	[offerLeftDate] [datetime] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_OfferMerchant] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfferSubType]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfferSubType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](300) NOT NULL,
	[description] [varchar](700) NULL,
	[offerTypeId] [int] NOT NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[OfferCodeId] [int] NULL,
 CONSTRAINT [PK_OfferSubType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfferType]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfferType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[offerType] [varchar](300) NOT NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_OfferType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organisation]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organisation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](300) NULL,
	[addressLine1] [nvarchar](500) NOT NULL,
	[addressLine2] [nvarchar](500) NULL,
	[location] [nvarchar](300) NULL,
	[organisationType] [int] NOT NULL,
	[emailAddress] [nvarchar](100) NOT NULL,
	[contactNumber] [nvarchar](17) NULL,
	[isMaster] [bit] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[maxCapacity] [int] NULL,
	[description] [nvarchar](max) NULL,
	[websiteURL] [varchar](300) NULL,
	[getHelpEmail] [nvarchar](1000) NULL,
	[getHelpPhone1] [nvarchar](17) NULL,
	[getHelpPhone2] [nvarchar](17) NULL,
	[facebookURL] [varchar](300) NULL,
	[twitterURL] [varchar](300) NULL,
	[skypeHandle] [varchar](300) NULL,
	[ContactName] [nvarchar](100) NULL,
	[ContactTitle] [nvarchar](100) NULL,
	[MerchantId] [varchar](10) NULL,
	[country] [varchar](100) NULL,
	[state] [varchar](100) NULL,
	[city] [varchar](100) NULL,
	[zip] [varchar](100) NULL,
	[isMapVisible] [bit] NULL,
	[isClosed] [bit] NULL,
	[businessTypeId] [int] NULL,
	[OrganisationSubTitle] [nvarchar](100) NULL,
	[dwellTime] [int] NULL,
	[InstagramHandle] [nvarchar](200) NULL,
	[isTrafficChartVisible] [bit] NULL,
	[JPOS_MerchantId] [nvarchar](20) NULL,
 CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganisationGroup]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganisationGroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[organisationId] [int] NOT NULL,
	[groupId] [int] NULL,
 CONSTRAINT [PK_OrganisationGroup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganisationMapping]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganisationMapping](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[organisationId] [int] NOT NULL,
	[parentOrganisationId] [int] NULL,
 CONSTRAINT [PK_OrganisationMapping] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganisationMealPeriod]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganisationMealPeriod](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](100) NOT NULL,
	[organisationId] [int] NULL,
	[openTime] [time](7) NULL,
	[closeTime] [time](7) NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isSelected] [bit] NULL,
	[days] [varchar](100) NULL,
 CONSTRAINT [PK_OrganisationMealPeriod] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganisationProgram]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganisationProgram](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[organisationId] [int] NOT NULL,
	[programId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[IsPrimaryAssociation] [bit] NULL,
 CONSTRAINT [PK_OrganisationProgram] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganisationProgramType]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganisationProgramType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrganisationId] [int] NOT NULL,
	[ProgramTypeId] [int] NULL,
 CONSTRAINT [PK_OrganisationProgramType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganisationSchedule]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganisationSchedule](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[organisationId] [int] NOT NULL,
	[workingDay] [varchar](20) NOT NULL,
	[openTime] [time](7) NULL,
	[closedTime] [time](7) NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[isHoliday] [bit] NULL,
	[holidayDate] [datetime] NULL,
	[HolidayName] [nvarchar](50) NULL,
	[IsForHolidayNameToShow] [bit] NULL,
 CONSTRAINT [PK_OrganisationSchedule] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrgLoyalityGlobalSettings]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrgLoyalityGlobalSettings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[organisationId] [int] NOT NULL,
	[loyalityThreshhold] [decimal](18, 2) NULL,
	[globalReward] [decimal](18, 2) NULL,
	[globalRatePoints] [decimal](18, 2) NULL,
	[bitePayRatio] [decimal](18, 2) NULL,
	[dcbFlexRatio] [decimal](18, 2) NULL,
	[userStatusVipRatio] [decimal](18, 2) NULL,
	[userStatusRegularRatio] [decimal](18, 2) NULL,
	[createdDate] [datetime] NULL,
	[modifiedDate] [datetime] NULL,
	[FirstTransactionBonus] [decimal](18, 2) NULL,
 CONSTRAINT [PK_OrgLoyalityGlobalSettings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PartnerNotificationsLog]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartnerNotificationsLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[ApiName] [nvarchar](250) NULL,
	[ApiUrl] [nvarchar](250) NULL,
	[Request] [nvarchar](max) NULL,
	[Response] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Photo]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Photo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[photoPath] [nvarchar](max) NULL,
	[entityId] [int] NULL,
	[photoType] [int] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NOT NULL,
	[updatedDate] [datetime] NULL,
 CONSTRAINT [PK_Photo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanProgramAccountsLinking]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanProgramAccountsLinking](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[planId] [int] NOT NULL,
	[programAccountId] [int] NOT NULL,
 CONSTRAINT [PK_PlanProgramAccountsLinking] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Program]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Program](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](400) NOT NULL,
	[description] [nvarchar](max) NULL,
	[organisationId] [int] NULL,
	[logoPath] [int] NULL,
	[colorCode] [nvarchar](30) NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[timeZone] [varchar](500) NULL,
	[ProgramCodeId] [nvarchar](15) NULL,
	[ProgramTypeId] [int] NULL,
	[website] [nvarchar](150) NULL,
	[address] [nvarchar](max) NULL,
	[country] [nvarchar](50) NULL,
	[state] [nvarchar](50) NULL,
	[city] [nvarchar](50) NULL,
	[zipcode] [nvarchar](20) NULL,
	[customName] [nvarchar](50) NULL,
	[customInputMask] [nvarchar](50) NULL,
	[customErrorMessaging] [nvarchar](100) NULL,
	[customInstructions] [nvarchar](40) NULL,
	[programCustomFields] [nvarchar](max) NULL,
	[AccountHolderGroups] [nvarchar](200) NULL,
	[AccountHolderUniqueId] [nvarchar](30) NULL,
	[IsAllNotificationShow] [bit] NULL,
	[JPOS_IssuerId] [nvarchar](50) NULL,
	[IsRewardsShowInApp] [bit] NULL,
 CONSTRAINT [PK_Program] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProgramAccountLinking]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramAccountLinking](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[programId] [int] NOT NULL,
	[accountTypeId] [int] NOT NULL,
 CONSTRAINT [PK_ProgramAccountLinking] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProgramAccounts]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramAccounts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[accountName] [varchar](150) NOT NULL,
	[accountTypeId] [int] NOT NULL,
	[programId] [int] NULL,
	[passType] [int] NULL,
	[intialBalanceCount] [decimal](18, 0) NULL,
	[resetPeriodType] [int] NULL,
	[resetDay] [int] NULL,
	[resetTime] [time](7) NULL,
	[maxPassUsage] [int] NULL,
	[isPassExchangeEnabled] [bit] NULL,
	[exchangePassValue] [int] NULL,
	[exchangeResetPeriodType] [int] NULL,
	[exchangeResetDay] [int] NULL,
	[exchangeResetTime] [time](7) NULL,
	[isRollOver] [bit] NULL,
	[flexEndDate] [datetime] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[ProgramAccountId] [nvarchar](10) NULL,
	[maxPassPerWeek] [int] NULL,
	[maxPassPerMonth] [int] NULL,
	[resetDateOfMonth] [date] NULL,
	[flexMaxSpendPerDay] [int] NULL,
	[flexMaxSpendPerWeek] [int] NULL,
	[flexMaxSpendPerMonth] [int] NULL,
	[exchangeResetDateOfMonth] [date] NULL,
	[vplMaxBalance] [decimal](18, 2) NULL,
	[vplMaxAddValueAmount] [decimal](18, 2) NULL,
	[vplMaxNumberOfTransaction] [int] NULL,
	[Jpos_ProgramAccountId] [nvarchar](200) NULL,
 CONSTRAINT [PK_ProgramAccounts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProgramAdmins]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramAdmins](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[adminUserId] [int] NULL,
	[programId] [int] NULL,
 CONSTRAINT [PK_ProgramAdmins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProgramBranding]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramBranding](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[programAccountID] [int] NOT NULL,
	[accountName] [nvarchar](150) NOT NULL,
	[programId] [int] NULL,
	[brandingColor] [nvarchar](15) NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[accountTypeId] [int] NULL,
	[cardNumber] [varchar](20) NULL,
 CONSTRAINT [PK_ProgramBranding] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProgramGroup]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramGroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[programId] [int] NOT NULL,
	[groupId] [int] NULL,
 CONSTRAINT [PK_ProgramGroup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProgramMerchant]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramMerchant](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[programId] [int] NOT NULL,
	[organisationId] [int] NOT NULL,
 CONSTRAINT [PK_ProgramMerchant] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProgramMerchantAccountType]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramMerchantAccountType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[organisationId] [int] NOT NULL,
	[programAccountLinkingId] [int] NULL,
 CONSTRAINT [PK_ProgramMerchantAccountType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProgramPackage]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramPackage](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](300) NOT NULL,
	[programId] [int] NOT NULL,
	[noOfMealPasses] [int] NULL,
	[noOfFlexPoints] [int] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[startDate] [datetime] NULL,
	[endDate] [datetime] NULL,
	[startTime] [time](7) NULL,
	[endTime] [time](7) NULL,
	[description] [nvarchar](500) NULL,
	[clientId] [nvarchar](20) NULL,
	[planId] [nvarchar](20) NULL,
	[isActive] [bit] NULL,
	[Jpos_PlanId] [nvarchar](200) NULL,
 CONSTRAINT [PK_ProgramPackage] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProgramType]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProgramTypeName] [varchar](200) NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ProgramType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Promotion]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Promotion](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[offerTypeId] [int] NULL,
	[offerSubTypeId] [int] NULL,
	[name] [varchar](150) NULL,
	[description] [nvarchar](max) NULL,
	[bannerTypeId] [int] NULL,
	[bannerDescription] [varchar](50) NULL,
	[startDate] [date] NULL,
	[startTime] [time](7) NULL,
	[endDate] [date] NULL,
	[endTime] [time](7) NULL,
	[promotionDay] [varchar](50) NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[MerchantId] [int] NULL,
	[noOfVisits] [int] NULL,
	[amounts] [decimal](18, 0) NULL,
	[businessTypeId] [int] NULL,
	[backgroundColor] [varchar](50) NULL,
	[firstGradiantColor] [varchar](50) NULL,
	[secondGradiantColor] [varchar](50) NULL,
	[IsPublished] [bit] NULL,
 CONSTRAINT [PK_Promotion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReloadBalanceRequest]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReloadBalanceRequest](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[benefactorUserId] [int] NULL,
	[isRequestAccepted] [bit] NULL,
	[requestedAmount] [decimal](18, 0) NULL,
	[programId] [int] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[Message] [nvarchar](500) NULL,
 CONSTRAINT [PK_ReloadBalanceRequest] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReloadRules]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReloadRules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[benefactorUserId] [int] NULL,
	[isAutoReloadAmount] [bit] NULL,
	[userDroppedAmount] [decimal](18, 0) NULL,
	[reloadAmount] [decimal](18, 0) NULL,
	[programId] [int] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[i2cBankAccountId] [nvarchar](50) NULL,
	[CardId] [nvarchar](255) NULL,
 CONSTRAINT [PK_ReloadRules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResetUserPassword]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResetUserPassword](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NULL,
	[inviteeId] [int] NULL,
	[resetToken] [nvarchar](20) NULL,
	[validTill] [datetime] NULL,
	[isPasswordReset] [bit] NULL,
	[createdDate] [datetime] NOT NULL,
	[updatedDate] [datetime] NULL,
 CONSTRAINT [PK_ResetUserPassword] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[RoleType] [int] NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleClaim]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleClaim](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_RoleClaim] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SiteLevelOverrideSettings]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiteLevelOverrideSettings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[programId] [int] NOT NULL,
	[siteLevelBitePayRatio] [decimal](18, 2) NULL,
	[siteLevelDcbFlexRatio] [decimal](18, 2) NULL,
	[siteLevelUserStatusVipRatio] [decimal](18, 2) NULL,
	[siteLevelUserStatusRegularRatio] [decimal](18, 2) NULL,
	[createdDate] [datetime] NULL,
	[modifiedDate] [datetime] NULL,
	[FirstTransactionBonus] [decimal](18, 2) NULL,
 CONSTRAINT [PK_SiteLevelOverrideSettings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMSTemplate]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMSTemplate](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
	[body] [nvarchar](max) NULL,
	[createdOn] [datetime] NULL,
 CONSTRAINT [PK_SMSTemplate] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[OrganisationId] [int] NULL,
	[Address] [nvarchar](700) NULL,
	[Location] [nvarchar](300) NULL,
	[UserCode] [nvarchar](30) NULL,
	[Custom1] [nvarchar](300) NULL,
	[Custom2] [nvarchar](300) NULL,
	[Custom3] [nvarchar](300) NULL,
	[Custom4] [nvarchar](300) NULL,
	[Custom5] [nvarchar](300) NULL,
	[Custom6] [nvarchar](300) NULL,
	[Custom7] [nvarchar](300) NULL,
	[Custom8] [nvarchar](300) NULL,
	[Custom9] [nvarchar](300) NULL,
	[Custom10] [nvarchar](300) NULL,
	[Custom11] [nvarchar](300) NULL,
	[Custom12] [nvarchar](300) NULL,
	[UserDeviceId] [nvarchar](300) NULL,
	[UserDeviceType] [nvarchar](50) NULL,
	[SessionId] [nvarchar](50) NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[IsAdmin] [bit] NULL,
	[ProgramId] [int] NULL,
	[secondaryEmail] [nvarchar](150) NULL,
	[genderId] [int] NULL,
	[dateOfBirth] [date] NULL,
	[customInfo] [nvarchar](max) NULL,
	[IsMobileRegistered] [bit] NULL,
	[InvitationStatus] [int] NULL,
	[JPOS_AccoutHolderId] [nvarchar](20) NULL,
	[AgreementVersionNo] [nvarchar](200) NULL,
	[IsAgreementRead] [bit] NULL,
	[AgreementReadDateTime] [datetime] NULL,
	[Jpos_AccountHolderId] [nvarchar](30) NULL,
	[PartnerUserId] [nvarchar](50) NULL,
	[PartnerId] [int] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAgreementHistory]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAgreementHistory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NULL,
	[cardHolderAgreementVersionNo] [nvarchar](100) NULL,
	[programId] [int] NULL,
	[dateAccepted] [datetime] NULL,
	[createdOn] [datetime] NULL,
	[createdBy] [int] NULL,
	[modifiedOn] [datetime] NULL,
	[modifiedBy] [int] NULL,
 CONSTRAINT [PK_UserAgreementHistory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserClaim]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserClaim](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserClaim] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserFavorites]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserFavorites](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[orgnisationId] [int] NOT NULL,
	[isFavorite] [bit] NOT NULL,
 CONSTRAINT [PK_UserFavorites] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserGroup]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserGroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[groupId] [int] NOT NULL,
 CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLogin]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogin](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_UserLogin] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLoyaltyPointsHistory]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLoyaltyPointsHistory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NULL,
	[transactionId] [nvarchar](200) NULL,
	[transactionAmount] [decimal](18, 2) NULL,
	[pointsEarned] [decimal](18, 2) NULL,
	[totalPoints] [decimal](18, 2) NULL,
	[rewardAmount] [decimal](18, 2) NULL,
	[leftOverPoints] [decimal](18, 2) NULL,
	[isThresholdReached] [bit] NULL,
	[transactionDate] [datetime] NULL,
	[createdDate] [datetime] NULL,
	[TranlogID] [bigint] NULL,
 CONSTRAINT [PK_UserLoyaltyPointsHistory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserNotificationSettings]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserNotificationSettings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[notificationId] [int] NOT NULL,
	[IsNotificationEnabled] [bit] NULL,
 CONSTRAINT [PK_UserNotificationSettings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPlans]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPlans](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[programPackageId] [int] NULL,
 CONSTRAINT [PK_UserPlans] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProgram]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProgram](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[programId] [int] NOT NULL,
	[programPackageId] [int] NULL,
	[userEmailAddress] [nvarchar](100) NULL,
	[isLinkedProgram] [bit] NULL,
	[linkAccountVerificationCode] [nvarchar](10) NULL,
	[verificationCodeValidTill] [datetime] NULL,
	[isVerificationCodeDone] [bit] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_UserProgram] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPushedNotifications]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPushedNotifications](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[notificationType] [int] NOT NULL,
	[notificationTitle] [nvarchar](200) NULL,
	[notificationMessage] [nvarchar](500) NULL,
	[referenceId] [int] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[ProgramId] [int] NULL,
	[NotificationSubType] [nvarchar](30) NULL,
	[CustomReferenceId] [int] NULL,
	[IsRedirect] [bit] NULL,
	[PushedNotificationType] [int] NULL,
 CONSTRAINT [PK_UserPushedNotifications] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPushedNotificationsStatus]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPushedNotificationsStatus](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[userDeviceId] [nvarchar](max) NOT NULL,
	[userDeviceType] [nvarchar](40) NOT NULL,
	[notificationId] [int] NOT NULL,
	[IsReadTillId] [bit] NULL,
	[notificationReadDate] [datetime] NULL,
 CONSTRAINT [PK_UserPushedNotificationsStatus] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRelations]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRelations](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[relationName] [varchar](200) NOT NULL,
	[description] [varchar](700) NULL,
	[displayOrder] [int] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
 CONSTRAINT [PK_UserRelations] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRewardsProgressLinking]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRewardsProgressLinking](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[rewardId] [int] NOT NULL,
	[programId] [int] NULL,
	[rewardProgressAchieved] [float] NOT NULL,
	[isRedeemed] [bit] NULL,
	[isRewardOfVisitNumType] [bit] NULL,
 CONSTRAINT [PK_UserRewardsProgressLinking] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserToken]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserToken](
	[UserId] [int] NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserToken] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserTransactionInfo]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTransactionInfo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[debitUserId] [int] NOT NULL,
	[creditUserId] [int] NULL,
	[accountTypeId] [int] NULL,
	[transactionAmount] [decimal](18, 0) NULL,
	[periodRemark] [nvarchar](100) NULL,
	[transactionDate] [datetime] NULL,
	[programId] [int] NULL,
	[isActive] [bit] NULL,
	[createdBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedBy] [int] NULL,
	[modifiedDate] [datetime] NULL,
	[isDeleted] [bit] NULL,
	[CreditTransactionUserType] [int] NULL,
	[organisationId] [int] NULL,
	[planId] [int] NULL,
	[programAccountId] [int] NULL,
	[merchantId] [int] NULL,
	[transactionStatus] [int] NULL,
	[DebitTransactionUserType] [int] NULL,
	[TerminalId] [int] NULL,
	[TransactionId] [varchar](20) NULL,
	[TransactionPaymentGateway] [nvarchar](50) NULL,
 CONSTRAINT [PK_UserTransactionInfo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserWallet]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserWallet](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[userProgramId] [int] NULL,
	[accountTypeId] [int] NOT NULL,
	[value] [decimal](18, 0) NULL,
	[expirationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_UserWallet] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AccountMerchantRules] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[AccountMerchantRules] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[AccountMerchantRules] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[AccountMerchantRules] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[AccountType] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[AccountType] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[AccountType] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[BenefactorProgram] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[BenefactorProgram] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[BenefactorProgram] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[BenefactorProgram] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] ADD  CONSTRAINT [DF_BenefactorUsersLinking_canViewTransaction]  DEFAULT ((1)) FOR [canViewTransaction]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] ADD  CONSTRAINT [DF_BenefactorUsersLinking_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] ADD  CONSTRAINT [DF_BenefactorUsersLinking_createdDate]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] ADD  CONSTRAINT [DF_BenefactorUsersLinking_modifiedDate]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] ADD  CONSTRAINT [DF_BenefactorUsersLinking_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] ADD  DEFAULT ((0)) FOR [IsRequestAccepted]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] ADD  DEFAULT ((0)) FOR [IsInvitationSent]
GO
ALTER TABLE [dbo].[BinData] ADD  CONSTRAINT [DF_BinData_CreatedDate]  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[BusinessType] ADD  CONSTRAINT [DF_BusinessType_createdDate]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[BusinessType] ADD  CONSTRAINT [DF_BusinessType_modifiedDate]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[CardHolderAgreement] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[CardHolderAgreement] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[CardHolderAgreement] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[CardHolderAgreement] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[ELMAH_Error] ADD  CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()) FOR [ErrorId]
GO
ALTER TABLE [dbo].[EmailTemplate] ADD  DEFAULT ('') FOR [CCEmail]
GO
ALTER TABLE [dbo].[EmailTemplate] ADD  DEFAULT ('') FOR [BCCEmail]
GO
ALTER TABLE [dbo].[ErrorMessagesDetail] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ErrorMessagesDetail] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[GatewayCardWebHookToken] ADD  DEFAULT ((0)) FOR [IsCardToSave]
GO
ALTER TABLE [dbo].[Group] ADD  CONSTRAINT [DF__Group__createdDa__35BCFE0A]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[Group] ADD  CONSTRAINT [DF__Group__modifiedD__36B12243]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[Group] ADD  CONSTRAINT [DF__Group__isDeleted__37A5467C]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[GroupType] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[GroupType] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[GroupType] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[I2CAccountDetail] ADD  CONSTRAINT [DF_I2CAccountDetail_CreatedDate]  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[i2cBank2CardTransfer] ADD  CONSTRAINT [DF_i2cBank2CardTransfer_CreatedDate]  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[i2cBank2CardTransfer] ADD  CONSTRAINT [DF_i2cBank2CardTransfer_UpdatedDate]  DEFAULT (getutcdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[i2cCardBankAccount] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[I2CLog] ADD  CONSTRAINT [DF_I2CLog_CreatedDate]  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Invitation] ADD  DEFAULT ((0)) FOR [IsRequestAccepted]
GO
ALTER TABLE [dbo].[Invitation] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Invitation] ADD  DEFAULT (getutcdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Invitation] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Invitation] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Invitation] ADD  DEFAULT (NULL) FOR [UserAccessRoleId]
GO
ALTER TABLE [dbo].[NotificationSettings] ADD  CONSTRAINT [DF__NotificationSettings__createdDa__35BCFE0A]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[NotificationSettings] ADD  CONSTRAINT [DF__NotificationSettings__modifiedD__36B12243]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[NotificationSettings] ADD  CONSTRAINT [DF__NotificationSettings__isActive__37A5467C]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[NotificationSettings] ADD  CONSTRAINT [DF__NotificationSettings__isDeleted__37A5467C]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF_Offer_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF__Offer__createdDa__6B24EA82]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF__Offer__modifiedD__6C190EBB]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[Offer] ADD  CONSTRAINT [DF__Offer__isDeleted__6D0D32F4]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[Offer] ADD  DEFAULT (NULL) FOR [OfferDayName]
GO
ALTER TABLE [dbo].[Offer] ADD  DEFAULT (NULL) FOR [MerchantId]
GO
ALTER TABLE [dbo].[OfferCode] ADD  CONSTRAINT [DF_OfferCode_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[OfferCode] ADD  CONSTRAINT [DF_OfferCode_createdDate]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[OfferCode] ADD  CONSTRAINT [DF_OfferCode_modifiedDate]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[OfferCode] ADD  CONSTRAINT [DF_OfferCode_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[OfferMerchant] ADD  CONSTRAINT [DF_OfferMerchant_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[OfferMerchant] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[OfferMerchant] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[OfferMerchant] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[OfferSubType] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[OfferSubType] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[OfferSubType] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[OfferSubType] ADD  DEFAULT (NULL) FOR [OfferCodeId]
GO
ALTER TABLE [dbo].[OfferType] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[OfferType] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[OfferType] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[Organisation] ADD  CONSTRAINT [DF_Organisation_isMaster]  DEFAULT ((0)) FOR [isMaster]
GO
ALTER TABLE [dbo].[Organisation] ADD  CONSTRAINT [DF__Organisat__isAct__2A4B4B5E]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[Organisation] ADD  CONSTRAINT [DF__Organisat__creat__2B3F6F97]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[Organisation] ADD  CONSTRAINT [DF__Organisat__modif__2C3393D0]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[Organisation] ADD  CONSTRAINT [DF__Organisat__isDel__2D27B809]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[Organisation] ADD  CONSTRAINT [DF__Organisat__Conta__799DF262]  DEFAULT (NULL) FOR [ContactName]
GO
ALTER TABLE [dbo].[Organisation] ADD  CONSTRAINT [DF__Organisat__Conta__7A92169B]  DEFAULT (NULL) FOR [ContactTitle]
GO
ALTER TABLE [dbo].[Organisation] ADD  CONSTRAINT [DF__Organisat__Organ__15460CD7]  DEFAULT (NULL) FOR [OrganisationSubTitle]
GO
ALTER TABLE [dbo].[Organisation] ADD  DEFAULT ((0)) FOR [isTrafficChartVisible]
GO
ALTER TABLE [dbo].[OrganisationMealPeriod] ADD  CONSTRAINT [DF_OrganisationMealPeriod_createdDate]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[OrganisationMealPeriod] ADD  CONSTRAINT [DF_OrganisationMealPeriod_modifiedDate]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[OrganisationProgram] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[OrganisationProgram] ADD  DEFAULT (getutcdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[OrganisationProgram] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[OrganisationProgram] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[OrganisationProgram] ADD  DEFAULT ((1)) FOR [IsPrimaryAssociation]
GO
ALTER TABLE [dbo].[OrganisationSchedule] ADD  CONSTRAINT [DF_OrganisationSchedule_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[OrganisationSchedule] ADD  CONSTRAINT [DF_OrganisationSchedule_createdDate]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[OrganisationSchedule] ADD  CONSTRAINT [DF_OrganisationSchedule_modifiedDate]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[OrganisationSchedule] ADD  CONSTRAINT [DF_OrganisationSchedule_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[OrganisationSchedule] ADD  DEFAULT (NULL) FOR [HolidayName]
GO
ALTER TABLE [dbo].[OrganisationSchedule] ADD  DEFAULT ((0)) FOR [IsForHolidayNameToShow]
GO
ALTER TABLE [dbo].[PartnerNotificationsLog] ADD  CONSTRAINT [DF_PartnerNotificationsLog_CreatedDate]  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Photo] ADD  CONSTRAINT [DF_Photo_createdDate]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[Photo] ADD  CONSTRAINT [DF_Photo_updatedDate]  DEFAULT (getutcdate()) FOR [updatedDate]
GO
ALTER TABLE [dbo].[Program] ADD  CONSTRAINT [DF__Program__isActiv__76969D2E]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[Program] ADD  CONSTRAINT [DF__Program__created__778AC167]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[Program] ADD  CONSTRAINT [DF__Program__modifie__787EE5A0]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[Program] ADD  CONSTRAINT [DF__Program__isDelet__797309D9]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[Program] ADD  DEFAULT (NULL) FOR [ProgramCodeId]
GO
ALTER TABLE [dbo].[Program] ADD  DEFAULT (NULL) FOR [ProgramTypeId]
GO
ALTER TABLE [dbo].[Program] ADD  DEFAULT ((1)) FOR [IsAllNotificationShow]
GO
ALTER TABLE [dbo].[Program] ADD  DEFAULT ((1)) FOR [IsRewardsShowInApp]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [programId]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [passType]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [intialBalanceCount]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [resetPeriodType]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [resetDay]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [resetTime]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [maxPassUsage]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT ((0)) FOR [isPassExchangeEnabled]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [exchangePassValue]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [exchangeResetPeriodType]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [exchangeResetDay]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (NULL) FOR [exchangeResetTime]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT ((0)) FOR [isRollOver]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[ProgramAccounts] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[ProgramBranding] ADD  DEFAULT (NULL) FOR [programId]
GO
ALTER TABLE [dbo].[ProgramBranding] ADD  DEFAULT (NULL) FOR [brandingColor]
GO
ALTER TABLE [dbo].[ProgramBranding] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[ProgramBranding] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[ProgramBranding] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[ProgramBranding] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[ProgramPackage] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[ProgramPackage] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[ProgramPackage] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[ProgramPackage] ADD  CONSTRAINT [plan_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[ProgramType] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ProgramType] ADD  DEFAULT (getutcdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[ProgramType] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ProgramType] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Promotion] ADD  CONSTRAINT [DF_Promotion_createdDate]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[Promotion] ADD  CONSTRAINT [DF_Promotion_modifiedDate]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[Promotion] ADD  CONSTRAINT [DF_Promotion_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[Promotion] ADD  CONSTRAINT [DF_Promotion_MerchantId]  DEFAULT (NULL) FOR [MerchantId]
GO
ALTER TABLE [dbo].[Promotion] ADD  DEFAULT ((0)) FOR [IsPublished]
GO
ALTER TABLE [dbo].[ReloadBalanceRequest] ADD  CONSTRAINT [DF__ReloadBal__isAct__00750D23]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[ReloadBalanceRequest] ADD  CONSTRAINT [DF__ReloadBal__creat__0169315C]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[ReloadBalanceRequest] ADD  CONSTRAINT [DF__ReloadBal__modif__025D5595]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[ReloadBalanceRequest] ADD  CONSTRAINT [DF__ReloadBal__isDel__035179CE]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[ReloadRules] ADD  CONSTRAINT [DF__ReloadRul__isAct__35A7EF71]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[ReloadRules] ADD  CONSTRAINT [DF__ReloadRul__creat__369C13AA]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[ReloadRules] ADD  CONSTRAINT [DF__ReloadRul__modif__379037E3]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[ReloadRules] ADD  CONSTRAINT [DF__ReloadRul__isDel__38845C1C]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[ResetUserPassword] ADD  CONSTRAINT [DF_ResetUserPassword_isPasswordReset]  DEFAULT ((0)) FOR [isPasswordReset]
GO
ALTER TABLE [dbo].[ResetUserPassword] ADD  CONSTRAINT [DF_ResetUserPassword_createdDate]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[ResetUserPassword] ADD  CONSTRAINT [DF_ResetUserPassword_updatedDate]  DEFAULT (getutcdate()) FOR [updatedDate]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_isActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_createdDate]  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_modifiedDate]  DEFAULT (getutcdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_isDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (NULL) FOR [RoleType]
GO
ALTER TABLE [dbo].[SMSTemplate] ADD  CONSTRAINT [DF_SMSTemplate_createdOn]  DEFAULT (getutcdate()) FOR [createdOn]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_isActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__DateCreate__145C0A3F]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__DateUpdate__15502E78]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_isDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__IsAdmin__7D6E8346]  DEFAULT (NULL) FOR [IsAdmin]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsMobileRegistered]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT (NULL) FOR [InvitationStatus]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsAgreementRead]
GO
ALTER TABLE [dbo].[UserAgreementHistory] ADD  DEFAULT (getutcdate()) FOR [dateAccepted]
GO
ALTER TABLE [dbo].[UserAgreementHistory] ADD  DEFAULT (getutcdate()) FOR [createdOn]
GO
ALTER TABLE [dbo].[UserAgreementHistory] ADD  DEFAULT (getutcdate()) FOR [modifiedOn]
GO
ALTER TABLE [dbo].[UserNotificationSettings] ADD  DEFAULT ((1)) FOR [IsNotificationEnabled]
GO
ALTER TABLE [dbo].[UserProgram] ADD  CONSTRAINT [DF_UserProgram_isLinkedProgram]  DEFAULT ((0)) FOR [isLinkedProgram]
GO
ALTER TABLE [dbo].[UserProgram] ADD  CONSTRAINT [DF_UserProgram_isVerificationCodeDone]  DEFAULT ((0)) FOR [isVerificationCodeDone]
GO
ALTER TABLE [dbo].[UserProgram] ADD  CONSTRAINT [DF_UserProgram_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[UserProgram] ADD  CONSTRAINT [DF_UserProgram_createdDate]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[UserProgram] ADD  CONSTRAINT [DF_UserProgram_modifiedDate]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[UserProgram] ADD  CONSTRAINT [DF_UserProgram_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[UserPushedNotifications] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[UserPushedNotifications] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[UserPushedNotifications] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[UserPushedNotifications] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[UserPushedNotifications] ADD  DEFAULT ((0)) FOR [IsRedirect]
GO
ALTER TABLE [dbo].[UserPushedNotificationsStatus] ADD  DEFAULT ((1)) FOR [IsReadTillId]
GO
ALTER TABLE [dbo].[UserPushedNotificationsStatus] ADD  DEFAULT (getutcdate()) FOR [notificationReadDate]
GO
ALTER TABLE [dbo].[UserRelations] ADD  CONSTRAINT [DF__UserRelat__isAct__703EA55A]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[UserRelations] ADD  CONSTRAINT [DF__UserRelat__creat__7132C993]  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[UserRelations] ADD  CONSTRAINT [DF__UserRelat__modif__7226EDCC]  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[UserRelations] ADD  CONSTRAINT [DF__UserRelat__isDel__731B1205]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[UserRewardsProgressLinking] ADD  DEFAULT ((0)) FOR [isRedeemed]
GO
ALTER TABLE [dbo].[UserRewardsProgressLinking] ADD  DEFAULT ((0)) FOR [isRewardOfVisitNumType]
GO
ALTER TABLE [dbo].[UserTransactionInfo] ADD  DEFAULT (getutcdate()) FOR [transactionDate]
GO
ALTER TABLE [dbo].[UserTransactionInfo] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[UserTransactionInfo] ADD  DEFAULT (getutcdate()) FOR [createdDate]
GO
ALTER TABLE [dbo].[UserTransactionInfo] ADD  DEFAULT (getutcdate()) FOR [modifiedDate]
GO
ALTER TABLE [dbo].[UserTransactionInfo] ADD  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[UserTransactionInfo] ADD  DEFAULT (NULL) FOR [DebitTransactionUserType]
GO
ALTER TABLE [dbo].[AccountMerchantRules]  WITH CHECK ADD  CONSTRAINT [FK_AccountMerchantRules_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[AccountMerchantRules] CHECK CONSTRAINT [FK_AccountMerchantRules_ModifiedUser]
GO
ALTER TABLE [dbo].[AccountMerchantRules]  WITH CHECK ADD  CONSTRAINT [FK_AccountMerchantRules_Organisation] FOREIGN KEY([merchantId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[AccountMerchantRules] CHECK CONSTRAINT [FK_AccountMerchantRules_Organisation]
GO
ALTER TABLE [dbo].[AccountMerchantRules]  WITH CHECK ADD  CONSTRAINT [FK_AccountMerchantRules_ProgramAccounts] FOREIGN KEY([programAccountID])
REFERENCES [dbo].[ProgramAccounts] ([id])
GO
ALTER TABLE [dbo].[AccountMerchantRules] CHECK CONSTRAINT [FK_AccountMerchantRules_ProgramAccounts]
GO
ALTER TABLE [dbo].[AccountMerchantRules]  WITH CHECK ADD  CONSTRAINT [FK_AccountMerchantRules_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[AccountMerchantRules] CHECK CONSTRAINT [FK_AccountMerchantRules_User]
GO
ALTER TABLE [dbo].[AccountMerchantRulesDetail]  WITH CHECK ADD  CONSTRAINT [FK_AccountMerchantRules_AccountMerchantRulesDetail] FOREIGN KEY([accountMerchantRuleId])
REFERENCES [dbo].[AccountMerchantRules] ([id])
GO
ALTER TABLE [dbo].[AccountMerchantRulesDetail] CHECK CONSTRAINT [FK_AccountMerchantRules_AccountMerchantRulesDetail]
GO
ALTER TABLE [dbo].[AccountType]  WITH CHECK ADD  CONSTRAINT [FK_AccountType_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[AccountType] CHECK CONSTRAINT [FK_AccountType_ModifiedUser]
GO
ALTER TABLE [dbo].[AdminProgramAccess]  WITH CHECK ADD  CONSTRAINT [FK_AdminProgramAccess_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[AdminProgramAccess] CHECK CONSTRAINT [FK_AdminProgramAccess_User]
GO
ALTER TABLE [dbo].[BenefactorProgram]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorProgram_CreatedUser] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BenefactorProgram] CHECK CONSTRAINT [FK_BenefactorProgram_CreatedUser]
GO
ALTER TABLE [dbo].[BenefactorProgram]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorProgram_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BenefactorProgram] CHECK CONSTRAINT [FK_BenefactorProgram_ModifiedUser]
GO
ALTER TABLE [dbo].[BenefactorProgram]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorProgram_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[BenefactorProgram] CHECK CONSTRAINT [FK_BenefactorProgram_Program]
GO
ALTER TABLE [dbo].[BenefactorProgram]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorProgram_ProgramPackage] FOREIGN KEY([programPackageId])
REFERENCES [dbo].[ProgramPackage] ([id])
GO
ALTER TABLE [dbo].[BenefactorProgram] CHECK CONSTRAINT [FK_BenefactorProgram_ProgramPackage]
GO
ALTER TABLE [dbo].[BenefactorProgram]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorProgram_User] FOREIGN KEY([benefactorId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BenefactorProgram] CHECK CONSTRAINT [FK_BenefactorProgram_User]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorUsersLinking_BenefactorUser] FOREIGN KEY([benefactorId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] CHECK CONSTRAINT [FK_BenefactorUsersLinking_BenefactorUser]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorUsersLinking_CreatedUser] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] CHECK CONSTRAINT [FK_BenefactorUsersLinking_CreatedUser]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorUsersLinking_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] CHECK CONSTRAINT [FK_BenefactorUsersLinking_ModifiedUser]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorUsersLinking_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] CHECK CONSTRAINT [FK_BenefactorUsersLinking_User]
GO
ALTER TABLE [dbo].[BenefactorUsersLinking]  WITH CHECK ADD  CONSTRAINT [FK_BenefactorUsersLinking_UserRelations] FOREIGN KEY([relationshipId])
REFERENCES [dbo].[UserRelations] ([id])
GO
ALTER TABLE [dbo].[BenefactorUsersLinking] CHECK CONSTRAINT [FK_BenefactorUsersLinking_UserRelations]
GO
ALTER TABLE [dbo].[BusinessType]  WITH CHECK ADD  CONSTRAINT [FK_BusinessType_User1] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[BusinessType] CHECK CONSTRAINT [FK_BusinessType_User1]
GO
ALTER TABLE [dbo].[Group]  WITH CHECK ADD  CONSTRAINT [FK_Group_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Group] CHECK CONSTRAINT [FK_Group_ModifiedUser]
GO
ALTER TABLE [dbo].[GroupType]  WITH CHECK ADD  CONSTRAINT [FK_GroupType_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[GroupType] CHECK CONSTRAINT [FK_GroupType_ModifiedUser]
GO
ALTER TABLE [dbo].[I2CAccountDetail]  WITH CHECK ADD  CONSTRAINT [FK_I2CAccountDetail_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[I2CAccountDetail] CHECK CONSTRAINT [FK_I2CAccountDetail_User]
GO
ALTER TABLE [dbo].[i2cCardBankAccount]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[i2cCardBankAccount]  WITH CHECK ADD  CONSTRAINT [FK_i2cCardBankAccount_I2CAccountDetail] FOREIGN KEY([I2cAccountDetailId])
REFERENCES [dbo].[I2CAccountDetail] ([Id])
GO
ALTER TABLE [dbo].[i2cCardBankAccount] CHECK CONSTRAINT [FK_i2cCardBankAccount_I2CAccountDetail]
GO
ALTER TABLE [dbo].[I2CLog]  WITH CHECK ADD  CONSTRAINT [FK_I2CLog_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[I2CLog] CHECK CONSTRAINT [FK_I2CLog_User]
GO
ALTER TABLE [dbo].[MerchantTerminal]  WITH CHECK ADD  CONSTRAINT [FK_MerchantTerminal_Organisation] FOREIGN KEY([organisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[MerchantTerminal] CHECK CONSTRAINT [FK_MerchantTerminal_Organisation]
GO
ALTER TABLE [dbo].[NotificationSettings]  WITH CHECK ADD  CONSTRAINT [FK_NotificationSettings_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[NotificationSettings] CHECK CONSTRAINT [FK_NotificationSettings_ModifiedUser]
GO
ALTER TABLE [dbo].[NotificationSettings]  WITH CHECK ADD  CONSTRAINT [FK_NotificationSettings_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[NotificationSettings] CHECK CONSTRAINT [FK_NotificationSettings_User]
GO
ALTER TABLE [dbo].[Offer]  WITH CHECK ADD  CONSTRAINT [FK__Offer__MerchantI__741A2336] FOREIGN KEY([MerchantId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[Offer] CHECK CONSTRAINT [FK__Offer__MerchantI__741A2336]
GO
ALTER TABLE [dbo].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Offer] CHECK CONSTRAINT [FK_Offer_ModifiedUser]
GO
ALTER TABLE [dbo].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_OfferSubType] FOREIGN KEY([offerSubTypeId])
REFERENCES [dbo].[OfferSubType] ([id])
GO
ALTER TABLE [dbo].[Offer] CHECK CONSTRAINT [FK_Offer_OfferSubType]
GO
ALTER TABLE [dbo].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_OfferType] FOREIGN KEY([offerTypeId])
REFERENCES [dbo].[OfferType] ([id])
GO
ALTER TABLE [dbo].[Offer] CHECK CONSTRAINT [FK_Offer_OfferType]
GO
ALTER TABLE [dbo].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[Offer] CHECK CONSTRAINT [FK_Offer_Program]
GO
ALTER TABLE [dbo].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Offer] CHECK CONSTRAINT [FK_Offer_User]
GO
ALTER TABLE [dbo].[OfferCode]  WITH CHECK ADD  CONSTRAINT [FK_OfferCode_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OfferCode] CHECK CONSTRAINT [FK_OfferCode_User]
GO
ALTER TABLE [dbo].[OfferCode]  WITH CHECK ADD  CONSTRAINT [FK_OfferCode_User1] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OfferCode] CHECK CONSTRAINT [FK_OfferCode_User1]
GO
ALTER TABLE [dbo].[OfferGroup]  WITH CHECK ADD  CONSTRAINT [FK_OfferGroup_Group] FOREIGN KEY([groupId])
REFERENCES [dbo].[Group] ([id])
GO
ALTER TABLE [dbo].[OfferGroup] CHECK CONSTRAINT [FK_OfferGroup_Group]
GO
ALTER TABLE [dbo].[OfferGroup]  WITH CHECK ADD  CONSTRAINT [FK_OfferGroup_Offer] FOREIGN KEY([offerId])
REFERENCES [dbo].[Offer] ([id])
GO
ALTER TABLE [dbo].[OfferGroup] CHECK CONSTRAINT [FK_OfferGroup_Offer]
GO
ALTER TABLE [dbo].[OfferMerchant]  WITH CHECK ADD  CONSTRAINT [FK_OfferMerchant_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OfferMerchant] CHECK CONSTRAINT [FK_OfferMerchant_ModifiedUser]
GO
ALTER TABLE [dbo].[OfferMerchant]  WITH CHECK ADD  CONSTRAINT [FK_OfferMerchant_Offer] FOREIGN KEY([offerId])
REFERENCES [dbo].[Offer] ([id])
GO
ALTER TABLE [dbo].[OfferMerchant] CHECK CONSTRAINT [FK_OfferMerchant_Offer]
GO
ALTER TABLE [dbo].[OfferMerchant]  WITH CHECK ADD  CONSTRAINT [FK_OfferMerchant_Organisation] FOREIGN KEY([merchantId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[OfferMerchant] CHECK CONSTRAINT [FK_OfferMerchant_Organisation]
GO
ALTER TABLE [dbo].[OfferMerchant]  WITH CHECK ADD  CONSTRAINT [FK_OfferMerchant_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OfferMerchant] CHECK CONSTRAINT [FK_OfferMerchant_User]
GO
ALTER TABLE [dbo].[OfferSubType]  WITH CHECK ADD FOREIGN KEY([OfferCodeId])
REFERENCES [dbo].[OfferCode] ([id])
GO
ALTER TABLE [dbo].[OfferSubType]  WITH CHECK ADD  CONSTRAINT [FK_OfferSubType_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OfferSubType] CHECK CONSTRAINT [FK_OfferSubType_ModifiedUser]
GO
ALTER TABLE [dbo].[OfferSubType]  WITH CHECK ADD  CONSTRAINT [FK_OfferSubType_OfferType] FOREIGN KEY([offerTypeId])
REFERENCES [dbo].[OfferType] ([id])
GO
ALTER TABLE [dbo].[OfferSubType] CHECK CONSTRAINT [FK_OfferSubType_OfferType]
GO
ALTER TABLE [dbo].[OfferSubType]  WITH CHECK ADD  CONSTRAINT [FK_OfferSubType_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OfferSubType] CHECK CONSTRAINT [FK_OfferSubType_User]
GO
ALTER TABLE [dbo].[OfferType]  WITH CHECK ADD  CONSTRAINT [FK_OfferType_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OfferType] CHECK CONSTRAINT [FK_OfferType_ModifiedUser]
GO
ALTER TABLE [dbo].[OfferType]  WITH CHECK ADD  CONSTRAINT [FK_OfferType_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OfferType] CHECK CONSTRAINT [FK_OfferType_User]
GO
ALTER TABLE [dbo].[Organisation]  WITH CHECK ADD  CONSTRAINT [FK_Organisation_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Organisation] CHECK CONSTRAINT [FK_Organisation_ModifiedUser]
GO
ALTER TABLE [dbo].[Organisation]  WITH CHECK ADD  CONSTRAINT [FK_Organisation_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Organisation] CHECK CONSTRAINT [FK_Organisation_User]
GO
ALTER TABLE [dbo].[OrganisationGroup]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationGroup_Group] FOREIGN KEY([groupId])
REFERENCES [dbo].[Group] ([id])
GO
ALTER TABLE [dbo].[OrganisationGroup] CHECK CONSTRAINT [FK_OrganisationGroup_Group]
GO
ALTER TABLE [dbo].[OrganisationGroup]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationGroup_Organisation] FOREIGN KEY([organisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[OrganisationGroup] CHECK CONSTRAINT [FK_OrganisationGroup_Organisation]
GO
ALTER TABLE [dbo].[OrganisationMapping]  WITH CHECK ADD  CONSTRAINT [FK_Organisation_OrganisationMapping] FOREIGN KEY([organisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[OrganisationMapping] CHECK CONSTRAINT [FK_Organisation_OrganisationMapping]
GO
ALTER TABLE [dbo].[OrganisationMapping]  WITH CHECK ADD  CONSTRAINT [FK_Organisation_OrganisationMappingParent] FOREIGN KEY([parentOrganisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[OrganisationMapping] CHECK CONSTRAINT [FK_Organisation_OrganisationMappingParent]
GO
ALTER TABLE [dbo].[OrganisationMealPeriod]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationMealPeriod_Organisation] FOREIGN KEY([organisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[OrganisationMealPeriod] CHECK CONSTRAINT [FK_OrganisationMealPeriod_Organisation]
GO
ALTER TABLE [dbo].[OrganisationMealPeriod]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationMealPeriod_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OrganisationMealPeriod] CHECK CONSTRAINT [FK_OrganisationMealPeriod_User]
GO
ALTER TABLE [dbo].[OrganisationMealPeriod]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationMealPeriod_User1] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OrganisationMealPeriod] CHECK CONSTRAINT [FK_OrganisationMealPeriod_User1]
GO
ALTER TABLE [dbo].[OrganisationProgram]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationProgram_Organisation] FOREIGN KEY([organisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[OrganisationProgram] CHECK CONSTRAINT [FK_OrganisationProgram_Organisation]
GO
ALTER TABLE [dbo].[OrganisationProgram]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationProgram_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[OrganisationProgram] CHECK CONSTRAINT [FK_OrganisationProgram_Program]
GO
ALTER TABLE [dbo].[OrganisationProgramType]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationProgramType_Organisation] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[OrganisationProgramType] CHECK CONSTRAINT [FK_OrganisationProgramType_Organisation]
GO
ALTER TABLE [dbo].[OrganisationProgramType]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationProgramType_ProgramType] FOREIGN KEY([ProgramTypeId])
REFERENCES [dbo].[ProgramType] ([Id])
GO
ALTER TABLE [dbo].[OrganisationProgramType] CHECK CONSTRAINT [FK_OrganisationProgramType_ProgramType]
GO
ALTER TABLE [dbo].[OrganisationSchedule]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationSchedule_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OrganisationSchedule] CHECK CONSTRAINT [FK_OrganisationSchedule_ModifiedUser]
GO
ALTER TABLE [dbo].[OrganisationSchedule]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationSchedule_Organisation] FOREIGN KEY([organisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[OrganisationSchedule] CHECK CONSTRAINT [FK_OrganisationSchedule_Organisation]
GO
ALTER TABLE [dbo].[OrganisationSchedule]  WITH CHECK ADD  CONSTRAINT [FK_OrganisationSchedule_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OrganisationSchedule] CHECK CONSTRAINT [FK_OrganisationSchedule_User]
GO
ALTER TABLE [dbo].[PartnerNotificationsLog]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[PlanProgramAccountsLinking]  WITH CHECK ADD  CONSTRAINT [FK_PlanProgramAccountsLinking_ProgramAccounts] FOREIGN KEY([programAccountId])
REFERENCES [dbo].[ProgramAccounts] ([id])
GO
ALTER TABLE [dbo].[PlanProgramAccountsLinking] CHECK CONSTRAINT [FK_PlanProgramAccountsLinking_ProgramAccounts]
GO
ALTER TABLE [dbo].[PlanProgramAccountsLinking]  WITH CHECK ADD  CONSTRAINT [FK_PlanProgramAccountsLinking_ProgramPackage] FOREIGN KEY([planId])
REFERENCES [dbo].[ProgramPackage] ([id])
GO
ALTER TABLE [dbo].[PlanProgramAccountsLinking] CHECK CONSTRAINT [FK_PlanProgramAccountsLinking_ProgramPackage]
GO
ALTER TABLE [dbo].[Program]  WITH CHECK ADD  CONSTRAINT [FK_Program_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Program] CHECK CONSTRAINT [FK_Program_ModifiedUser]
GO
ALTER TABLE [dbo].[Program]  WITH CHECK ADD  CONSTRAINT [FK_Program_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Program] CHECK CONSTRAINT [FK_Program_User]
GO
ALTER TABLE [dbo].[ProgramAccountLinking]  WITH CHECK ADD  CONSTRAINT [FK_ProgramAccountLinking_AccountType] FOREIGN KEY([accountTypeId])
REFERENCES [dbo].[AccountType] ([id])
GO
ALTER TABLE [dbo].[ProgramAccountLinking] CHECK CONSTRAINT [FK_ProgramAccountLinking_AccountType]
GO
ALTER TABLE [dbo].[ProgramAccountLinking]  WITH CHECK ADD  CONSTRAINT [FK_ProgramAccountLinking_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[ProgramAccountLinking] CHECK CONSTRAINT [FK_ProgramAccountLinking_Program]
GO
ALTER TABLE [dbo].[ProgramAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProgramAccounts_AccountType] FOREIGN KEY([accountTypeId])
REFERENCES [dbo].[AccountType] ([id])
GO
ALTER TABLE [dbo].[ProgramAccounts] CHECK CONSTRAINT [FK_ProgramAccounts_AccountType]
GO
ALTER TABLE [dbo].[ProgramAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProgramAccounts_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProgramAccounts] CHECK CONSTRAINT [FK_ProgramAccounts_ModifiedUser]
GO
ALTER TABLE [dbo].[ProgramAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProgramAccounts_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[ProgramAccounts] CHECK CONSTRAINT [FK_ProgramAccounts_Program]
GO
ALTER TABLE [dbo].[ProgramAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProgramAccounts_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProgramAccounts] CHECK CONSTRAINT [FK_ProgramAccounts_User]
GO
ALTER TABLE [dbo].[ProgramBranding]  WITH CHECK ADD  CONSTRAINT [FK_ProgramBranding_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProgramBranding] CHECK CONSTRAINT [FK_ProgramBranding_ModifiedUser]
GO
ALTER TABLE [dbo].[ProgramBranding]  WITH CHECK ADD  CONSTRAINT [FK_ProgramBranding_ProgramAccounts] FOREIGN KEY([programAccountID])
REFERENCES [dbo].[ProgramAccounts] ([id])
GO
ALTER TABLE [dbo].[ProgramBranding] CHECK CONSTRAINT [FK_ProgramBranding_ProgramAccounts]
GO
ALTER TABLE [dbo].[ProgramBranding]  WITH CHECK ADD  CONSTRAINT [FK_ProgramBranding_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProgramBranding] CHECK CONSTRAINT [FK_ProgramBranding_User]
GO
ALTER TABLE [dbo].[ProgramGroup]  WITH CHECK ADD  CONSTRAINT [FK_ProgramGroup_Group] FOREIGN KEY([groupId])
REFERENCES [dbo].[Group] ([id])
GO
ALTER TABLE [dbo].[ProgramGroup] CHECK CONSTRAINT [FK_ProgramGroup_Group]
GO
ALTER TABLE [dbo].[ProgramGroup]  WITH CHECK ADD  CONSTRAINT [FK_ProgramGroup_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[ProgramGroup] CHECK CONSTRAINT [FK_ProgramGroup_Program]
GO
ALTER TABLE [dbo].[ProgramMerchant]  WITH CHECK ADD  CONSTRAINT [FK_ProgramMerchant_Organisation] FOREIGN KEY([organisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[ProgramMerchant] CHECK CONSTRAINT [FK_ProgramMerchant_Organisation]
GO
ALTER TABLE [dbo].[ProgramMerchant]  WITH CHECK ADD  CONSTRAINT [FK_ProgramMerchant_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[ProgramMerchant] CHECK CONSTRAINT [FK_ProgramMerchant_Program]
GO
ALTER TABLE [dbo].[ProgramMerchantAccountType]  WITH CHECK ADD  CONSTRAINT [FK_ProgramMerchantAccountType_Organisation] FOREIGN KEY([organisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[ProgramMerchantAccountType] CHECK CONSTRAINT [FK_ProgramMerchantAccountType_Organisation]
GO
ALTER TABLE [dbo].[ProgramMerchantAccountType]  WITH CHECK ADD  CONSTRAINT [FK_ProgramMerchantAccountType_ProgramAccountLinking] FOREIGN KEY([programAccountLinkingId])
REFERENCES [dbo].[ProgramAccountLinking] ([id])
GO
ALTER TABLE [dbo].[ProgramMerchantAccountType] CHECK CONSTRAINT [FK_ProgramMerchantAccountType_ProgramAccountLinking]
GO
ALTER TABLE [dbo].[ProgramPackage]  WITH CHECK ADD  CONSTRAINT [FK_ProgramPackage_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProgramPackage] CHECK CONSTRAINT [FK_ProgramPackage_ModifiedUser]
GO
ALTER TABLE [dbo].[ProgramPackage]  WITH CHECK ADD  CONSTRAINT [FK_ProgramPackage_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[ProgramPackage] CHECK CONSTRAINT [FK_ProgramPackage_Program]
GO
ALTER TABLE [dbo].[ProgramPackage]  WITH CHECK ADD  CONSTRAINT [FK_ProgramPackage_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProgramPackage] CHECK CONSTRAINT [FK_ProgramPackage_User]
GO
ALTER TABLE [dbo].[ProgramType]  WITH CHECK ADD  CONSTRAINT [FK_ProgramType_ModifiedUser] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProgramType] CHECK CONSTRAINT [FK_ProgramType_ModifiedUser]
GO
ALTER TABLE [dbo].[ProgramType]  WITH CHECK ADD  CONSTRAINT [FK_ProgramType_User] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProgramType] CHECK CONSTRAINT [FK_ProgramType_User]
GO
ALTER TABLE [dbo].[Promotion]  WITH CHECK ADD  CONSTRAINT [FK_Promotion_BusinessType] FOREIGN KEY([businessTypeId])
REFERENCES [dbo].[BusinessType] ([id])
GO
ALTER TABLE [dbo].[Promotion] CHECK CONSTRAINT [FK_Promotion_BusinessType]
GO
ALTER TABLE [dbo].[Promotion]  WITH CHECK ADD  CONSTRAINT [FK_Promotion_OfferCode] FOREIGN KEY([bannerTypeId])
REFERENCES [dbo].[OfferCode] ([id])
GO
ALTER TABLE [dbo].[Promotion] CHECK CONSTRAINT [FK_Promotion_OfferCode]
GO
ALTER TABLE [dbo].[Promotion]  WITH CHECK ADD  CONSTRAINT [FK_Promotion_OfferSubType] FOREIGN KEY([offerSubTypeId])
REFERENCES [dbo].[OfferSubType] ([id])
GO
ALTER TABLE [dbo].[Promotion] CHECK CONSTRAINT [FK_Promotion_OfferSubType]
GO
ALTER TABLE [dbo].[Promotion]  WITH CHECK ADD  CONSTRAINT [FK_Promotion_OfferType] FOREIGN KEY([offerTypeId])
REFERENCES [dbo].[OfferType] ([id])
GO
ALTER TABLE [dbo].[Promotion] CHECK CONSTRAINT [FK_Promotion_OfferType]
GO
ALTER TABLE [dbo].[Promotion]  WITH CHECK ADD  CONSTRAINT [FK_Promotion_Organisation] FOREIGN KEY([MerchantId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[Promotion] CHECK CONSTRAINT [FK_Promotion_Organisation]
GO
ALTER TABLE [dbo].[Promotion]  WITH CHECK ADD  CONSTRAINT [FK_Promotion_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Promotion] CHECK CONSTRAINT [FK_Promotion_User]
GO
ALTER TABLE [dbo].[Promotion]  WITH CHECK ADD  CONSTRAINT [FK_Promotion_User1] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Promotion] CHECK CONSTRAINT [FK_Promotion_User1]
GO
ALTER TABLE [dbo].[ReloadBalanceRequest]  WITH CHECK ADD  CONSTRAINT [FK_ReloadBalanceRequest_BenefactorLink] FOREIGN KEY([benefactorUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ReloadBalanceRequest] CHECK CONSTRAINT [FK_ReloadBalanceRequest_BenefactorLink]
GO
ALTER TABLE [dbo].[ReloadBalanceRequest]  WITH CHECK ADD  CONSTRAINT [FK_ReloadBalanceRequest_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ReloadBalanceRequest] CHECK CONSTRAINT [FK_ReloadBalanceRequest_ModifiedUser]
GO
ALTER TABLE [dbo].[ReloadBalanceRequest]  WITH CHECK ADD  CONSTRAINT [FK_ReloadBalanceRequest_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ReloadBalanceRequest] CHECK CONSTRAINT [FK_ReloadBalanceRequest_User]
GO
ALTER TABLE [dbo].[ReloadBalanceRequest]  WITH CHECK ADD  CONSTRAINT [FK_ReloadBalanceRequest_UserLink] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ReloadBalanceRequest] CHECK CONSTRAINT [FK_ReloadBalanceRequest_UserLink]
GO
ALTER TABLE [dbo].[ReloadRules]  WITH CHECK ADD  CONSTRAINT [FK_ReloadRules_BenefactorLink] FOREIGN KEY([benefactorUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ReloadRules] CHECK CONSTRAINT [FK_ReloadRules_BenefactorLink]
GO
ALTER TABLE [dbo].[ReloadRules]  WITH CHECK ADD  CONSTRAINT [FK_ReloadRules_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ReloadRules] CHECK CONSTRAINT [FK_ReloadRules_ModifiedUser]
GO
ALTER TABLE [dbo].[ReloadRules]  WITH CHECK ADD  CONSTRAINT [FK_ReloadRules_User] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ReloadRules] CHECK CONSTRAINT [FK_ReloadRules_User]
GO
ALTER TABLE [dbo].[ReloadRules]  WITH CHECK ADD  CONSTRAINT [FK_ReloadRules_UserLink] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ReloadRules] CHECK CONSTRAINT [FK_ReloadRules_UserLink]
GO
ALTER TABLE [dbo].[ResetUserPassword]  WITH CHECK ADD  CONSTRAINT [FK_ResetUserPassword_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ResetUserPassword] CHECK CONSTRAINT [FK_ResetUserPassword_User]
GO
ALTER TABLE [dbo].[Role]  WITH CHECK ADD  CONSTRAINT [FK_Role_ModifiedUser] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Role] CHECK CONSTRAINT [FK_Role_ModifiedUser]
GO
ALTER TABLE [dbo].[Role]  WITH CHECK ADD  CONSTRAINT [FK_Role_User] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Role] CHECK CONSTRAINT [FK_Role_User]
GO
ALTER TABLE [dbo].[RoleClaim]  WITH CHECK ADD  CONSTRAINT [FK_RoleClaim_Role_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoleClaim] CHECK CONSTRAINT [FK_RoleClaim_Role_RoleId]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_ModifiedUser] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_ModifiedUser]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Organisation] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Organisation]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_User] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_User]
GO
ALTER TABLE [dbo].[UserClaim]  WITH CHECK ADD  CONSTRAINT [FK_UserClaim_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserClaim] CHECK CONSTRAINT [FK_UserClaim_User_UserId]
GO
ALTER TABLE [dbo].[UserFavorites]  WITH CHECK ADD  CONSTRAINT [FK_UserFavorites_Organisation] FOREIGN KEY([orgnisationId])
REFERENCES [dbo].[Organisation] ([id])
GO
ALTER TABLE [dbo].[UserFavorites] CHECK CONSTRAINT [FK_UserFavorites_Organisation]
GO
ALTER TABLE [dbo].[UserFavorites]  WITH CHECK ADD  CONSTRAINT [FK_UserFavorites_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserFavorites] CHECK CONSTRAINT [FK_UserFavorites_User]
GO
ALTER TABLE [dbo].[UserGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserGroup_Group] FOREIGN KEY([groupId])
REFERENCES [dbo].[Group] ([id])
GO
ALTER TABLE [dbo].[UserGroup] CHECK CONSTRAINT [FK_UserGroup_Group]
GO
ALTER TABLE [dbo].[UserGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserGroup_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserGroup] CHECK CONSTRAINT [FK_UserGroup_User]
GO
ALTER TABLE [dbo].[UserLogin]  WITH CHECK ADD  CONSTRAINT [FK_UserLogin_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserLogin] CHECK CONSTRAINT [FK_UserLogin_User_UserId]
GO
ALTER TABLE [dbo].[UserNotificationSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserNotificationSettings_NotificationSettings] FOREIGN KEY([notificationId])
REFERENCES [dbo].[NotificationSettings] ([id])
GO
ALTER TABLE [dbo].[UserNotificationSettings] CHECK CONSTRAINT [FK_UserNotificationSettings_NotificationSettings]
GO
ALTER TABLE [dbo].[UserNotificationSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserNotificationSettings_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserNotificationSettings] CHECK CONSTRAINT [FK_UserNotificationSettings_User]
GO
ALTER TABLE [dbo].[UserPlans]  WITH CHECK ADD  CONSTRAINT [FK_UserPlans_ProgramPackage] FOREIGN KEY([programPackageId])
REFERENCES [dbo].[ProgramPackage] ([id])
GO
ALTER TABLE [dbo].[UserPlans] CHECK CONSTRAINT [FK_UserPlans_ProgramPackage]
GO
ALTER TABLE [dbo].[UserPlans]  WITH CHECK ADD  CONSTRAINT [FK_UserPlans_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserPlans] CHECK CONSTRAINT [FK_UserPlans_User]
GO
ALTER TABLE [dbo].[UserProgram]  WITH CHECK ADD  CONSTRAINT [FK_UserProgram_CreatedUser] FOREIGN KEY([createdBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserProgram] CHECK CONSTRAINT [FK_UserProgram_CreatedUser]
GO
ALTER TABLE [dbo].[UserProgram]  WITH CHECK ADD  CONSTRAINT [FK_UserProgram_ModifiedUser] FOREIGN KEY([modifiedBy])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserProgram] CHECK CONSTRAINT [FK_UserProgram_ModifiedUser]
GO
ALTER TABLE [dbo].[UserProgram]  WITH CHECK ADD  CONSTRAINT [FK_UserProgram_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[UserProgram] CHECK CONSTRAINT [FK_UserProgram_Program]
GO
ALTER TABLE [dbo].[UserProgram]  WITH CHECK ADD  CONSTRAINT [FK_UserProgram_ProgramPackage] FOREIGN KEY([programPackageId])
REFERENCES [dbo].[ProgramPackage] ([id])
GO
ALTER TABLE [dbo].[UserProgram] CHECK CONSTRAINT [FK_UserProgram_ProgramPackage]
GO
ALTER TABLE [dbo].[UserProgram]  WITH CHECK ADD  CONSTRAINT [FK_UserProgram_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserProgram] CHECK CONSTRAINT [FK_UserProgram_User]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role_RoleId]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User_UserId]
GO
ALTER TABLE [dbo].[UserToken]  WITH CHECK ADD  CONSTRAINT [FK_UserToken_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserToken] CHECK CONSTRAINT [FK_UserToken_User_UserId]
GO
ALTER TABLE [dbo].[UserTransactionInfo]  WITH CHECK ADD  CONSTRAINT [FK_UserTransactionInfo_AccountType] FOREIGN KEY([accountTypeId])
REFERENCES [dbo].[AccountType] ([id])
GO
ALTER TABLE [dbo].[UserTransactionInfo] CHECK CONSTRAINT [FK_UserTransactionInfo_AccountType]
GO
ALTER TABLE [dbo].[UserTransactionInfo]  WITH CHECK ADD  CONSTRAINT [FK_UserTransactionInfo_Program] FOREIGN KEY([programId])
REFERENCES [dbo].[Program] ([id])
GO
ALTER TABLE [dbo].[UserTransactionInfo] CHECK CONSTRAINT [FK_UserTransactionInfo_Program]
GO
ALTER TABLE [dbo].[UserWallet]  WITH CHECK ADD  CONSTRAINT [FK_UserWallet_AccountType] FOREIGN KEY([accountTypeId])
REFERENCES [dbo].[AccountType] ([id])
GO
ALTER TABLE [dbo].[UserWallet] CHECK CONSTRAINT [FK_UserWallet_AccountType]
GO
ALTER TABLE [dbo].[UserWallet]  WITH CHECK ADD  CONSTRAINT [FK_UserWallet_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserWallet] CHECK CONSTRAINT [FK_UserWallet_User]
GO
ALTER TABLE [dbo].[UserWallet]  WITH CHECK ADD  CONSTRAINT [FK_UserWallet_UserProgram] FOREIGN KEY([userProgramId])
REFERENCES [dbo].[UserProgram] ([id])
GO
ALTER TABLE [dbo].[UserWallet] CHECK CONSTRAINT [FK_UserWallet_UserProgram]
GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorsXml]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_GetErrorsXml]  
  
(  
    @Application NVARCHAR(60),  
    @PageIndex INT = 0,  
    @PageSize INT = 15,  
    @TotalCount INT OUTPUT  
  
)  
  
AS  
  
SET NOCOUNT ON  
  
DECLARE @FirstTimeUTC DATETIME  
DECLARE @FirstSequence INT  
DECLARE @StartRow INT  
DECLARE @StartRowIndex INT  
SELECT  
  
@TotalCount = COUNT(1)  
  
FROM  
  
    [ELMAH_Error]  
  
WHERE  
  
    [Application] = @Application  
SET @StartRowIndex = @PageIndex * @PageSize + 1  
IF @StartRowIndex <= @TotalCount  
  
BEGIN  
  
SET ROWCOUNT @StartRowIndex  
  
SELECT  
  
@FirstTimeUTC = [TimeUtc],  
  
    @FirstSequence = [Sequence]  
  
FROM  
  
    [ELMAH_Error]  
  
WHERE  
  
    [Application] = @Application  
  
ORDER BY  
  
    [TimeUtc] DESC,  
    [Sequence] DESC  
  
END  
  
ELSE  
  
BEGIN  
  
SET @PageSize = 0  
  
END  
  
SET ROWCOUNT @PageSize  
  
SELECT  
  
errorId = [ErrorId],  
  
    application = [Application],  
    host = [Host],  
    type = [Type],  
    source = [Source],  
    message = [Message],  
    [user] = [User],  
    statusCode = [StatusCode],  
    time = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'  
  
FROM  
  
    [ELMAH_Error] error  
  
WHERE  
  
    [Application] = @Application  
  
AND  
  
    [TimeUtc] <= @FirstTimeUTC  
  
AND  
  
    [Sequence] <= @FirstSequence  
  
ORDER BY  
  
    [TimeUtc] DESC,  
  
    [Sequence] DESC  
  
FOR  
  
XML AUTO 





GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorXml]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE[dbo].[ELMAH_GetErrorXml]  
  
(  
  
    @Application NVARCHAR(60),  
    @ErrorId UNIQUEIDENTIFIER  
  
)  
  
AS  
  
SET NOCOUNT ON  
SELECT  
  
    [AllXml]  
FROM  
  
    [ELMAH_Error]  
WHERE  
  
    [ErrorId] = @ErrorId  
AND  
    [Application] = @Application  



GO
/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE[dbo].[ELMAH_LogError]  
  
(  
  
    @ErrorId UNIQUEIDENTIFIER,    
    @Application NVARCHAR(60),    
    @Host NVARCHAR(30),    
    @Type NVARCHAR(100),  
    @Source NVARCHAR(60),    
    @Message NVARCHAR(500),  
    @User NVARCHAR(50),   
    @AllXml NTEXT,    
    @StatusCode INT,   
    @TimeUtc DATETIME  
  
)  
  
AS  
  
SET NOCOUNT ON  
  
INSERT  
  
INTO  
  
    [ELMAH_Error]
(  
  
    [ErrorId],   
    [Application],   
    [Host],  
    [Type],  
    [Source],  
    [Message],    
    [User],    
    [AllXml],    
    [StatusCode],    
    [TimeUtc]  
  
)  
  
VALUES  
  
    (  
  
    @ErrorId,  
    @Application,    
    @Host,    
    @Type,    
    @Source,   
    @Message,    
    @User,   
    @AllXml,   
    @StatusCode,   
    @TimeUtc  
  
) 





GO
/****** Object:  StoredProcedure [dbo].[GetAccountAuditReport]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAccountAuditReport]  --exec GetAccountAuditReport 119,227,'07/04/2019','08/19/2019'
	-- Add the parameters for the stored procedure here
	@programId int,
	@accountId int,
	@StartDate DateTime,
	@EndDate DateTime
AS
BEGIN
	SELECT
p.id AS ProgramId,
p.Name AS ProgramName,
u.id AS UserId,
u.userCode AS AccountHolderID,
u.firstName AS FirstName,
u.LAStName AS LAStName,
uti.transactionDate AS DateANDTimeOfScan,
CASt(uti.transactionDate AS VARCHAR(10)) AS SettleDate,
'Auth' AS [Type], --Need to make it dynamic once Jpost AND I2c Db is confirmed
pa.accountname AS AccountName,
uti.TransactionId AS TxnId,
(CASE WHEN pa.accountTypeId = 3 THEN ISNULL(uti.transactionAmount,0) ELSE 0 END) AS Points,
(CASE WHEN pa.accountTypeId != 3 THEN ISNULL(uti.transactionAmount,0) ELSE 0 END) AS Units,
uti.periodRemark AS Period,
(CASE WHEN pa.accountTypeId != 3 THEN ISNULL(uti.transactionAmount,0) ELSE 0 END) AS Equiv,--Need to make it dynamic once Jpost AND I2c Db is confirmed
1 AS Total, --Need to make it dynamic once Jpost AND I2c Db is confirmed
pp.id AS PlanId, pp.name AS PlanName 
 from programAccounts AS pa
INNER JOIN program AS p on pa.programid = p.id
INNER JOIN planProgramAccountsLinking AS ppal on pa.id = ppal.programAccountId
INNER JOIN programPackage AS pp on ppal.planId = pp.id
INNER JOIN userplans AS up on pp.id = up.programPackageId
INNER JOIN [user] AS u on up.userid = u.id
INNER JOIN userTransactionInfo AS uti on (u.id = uti.debituserid OR u.id = uti.credituserid) 
 WHERE pa.id =@accountId AND pa.isdeleted = 0 AND pa.isactive = 1 AND CONVERT(VARCHAR(10), uti.transactionDate, 111) BETWEEN CONVERT(VARCHAR(10), @StartDate, 111) 
AND CONVERT(VARCHAR(10), @EndDate, 111) AND pa.programid = @programId
 GROUP BY p.id, p.name, pa.accountname,pp.id,pp.name,u.userCode,uti.transactionAmount,
 u.id,u.firstName,u.LAStName,transactionDate
 , uti.TransactionId,pa.accountTypeId,uti.periodRemark
END

GO
/****** Object:  StoredProcedure [dbo].[GetAccountBalanceDetailReport]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAccountBalanceDetailReport]
(
@accountId int
)
AS
BEGIN

SELECT 
 u.Id,
 u.UserCode as UserID,
 u.FirstName,
 u.LastName,
 ISNULL(dbo.fnGetUserBalanceDetail(u.Id,1,0,@accountId,pa.AccountTypeId),0) as Balance,  --,@planId
 LastUsed=MAX(transactionDate),
 ac.accountType
 FROM PlanProgramAccountsLinking ppal
 INNER JOIN ProgramAccounts pa ON pa.Id=ppal.ProgramAccountId AND pa.IsActive=1 AND pa.IsDeleted=0
INNER JOIN ProgramPackage pp on pp.Id=ppal.PlanId AND pp.IsActive=1 AND pp.IsDeleted=0
INNER JOIN UserPlans up ON ppal.PlanId=up.ProgramPackageId
INNER JOIN [User] u ON u.Id=up.UserId AND u.IsActive=1 AND u.IsDeleted=0
INNER JOIN AccountType ac On ac.Id=pa.AccountTypeId
INNER JOIN UserTransactionInfo AS utf ON utf.debitUserId=u.Id AND utf.DebitTransactionUserType=1 AND utf.IsActive=1 AND utf.IsDeleted=0 AND utf.ProgramAccountId=@accountId
WHERE ppal.ProgramAccountId=@accountId 
GROUP BY u.Id, u.UserCode, u.FirstName, u.LastName, pa.AccountTypeId, ac.accountType
END
GO
/****** Object:  StoredProcedure [dbo].[GetAccountHolders]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/***********************************************************************************************************************************************  
SP Name: GetAccountHolders  
Description: This stored procedure will fetch the list of Account holders (users) based on the search value and server side paging.  
Test Result Run : Exec GetAccountHolders 44,17,'',1,10,'DateAdded','desc',1,NULL,0  
************************************************************************************************************************************************/  
CREATE PROCEDURE [dbo].[GetAccountHolders]  
(  
@OrganisationId int,  
@ProgramId int,  
@SearchValue nvarchar(50)='',  
@PageNumber int=1,  
@PageSize int=10,  
@SortColumnName nvarchar(20)='AccountHolderID',  
@SortOrderDirection nvarchar(10)='ASC',  
@PhotoTypeDetail INT=1,  
@DefaultUserPhotoPath nvarchar(150)=NULL,  
@PlanId INT=NULL  
)  
AS  
BEGIN  
   
 SET NOCOUNT ON;  
  
 DECLARE @StartRow INT  
 DECLARE @EndRow INT  
  
 -- calculate the starting and ending of records.  
  
 SET @SortColumnName = LOWER(ISNULL(@SortColumnName,''))  
 SET @SortOrderDirection = LOWER(ISNULL(@SortOrderDirection,''))  
  
 SET @StartRow=(@PageNumber - 1) * @PageSize  
 SET @EndRow=(@PageNumber * @PageSize) + 1  
  
  Print  @StartRow
  Print @EndRow
 ;WITH CTEResult AS(  
SELECT ROW_NUMBER() OVER( ORDER BY  
CASE WHEN (@SortColumnName='Name' AND @SortOrderDirection='asc') THEN Name END ASC,  
CASE WHEN (@SortColumnName='Name' AND @SortOrderDirection='desc') THEN Name END DESC,  
CASE WHEN (@SortColumnName='AccountHolderID' AND @SortOrderDirection='asc') THEN Name END ASC,  
CASE WHEN (@SortColumnName='AccountHolderID' AND @SortOrderDirection='desc') THEN Name END DESC,  
CASE WHEN (@SortColumnName='Email' AND @SortOrderDirection='asc') THEN Name END ASC,  
CASE WHEN (@SortColumnName='Email' AND @SortOrderDirection='desc') THEN Name END DESC,  
CASE WHEN (@SortColumnName='DateAdded' AND @SortOrderDirection='asc') THEN Name END ASC,  
CASE WHEN (@SortColumnName='DateAdded' AND @SortOrderDirection='desc') THEN Name END DESC,  
CASE WHEN (@SortColumnName='Status' AND @SortOrderDirection='asc') THEN Name END ASC,  
CASE WHEN (@SortColumnName='Status' AND @SortOrderDirection='desc') THEN Name END DESC,  
CASE WHEN (@SortColumnName='PlanName' AND @SortOrderDirection='asc') THEN Name END ASC,  
CASE WHEN (@SortColumnName='PlanName' AND @SortOrderDirection='desc') THEN Name END DESC  
) AS RowNumber,  
COUNT(*) OVER() AS TotalCount,  
 usr.Id ,  
 usr.FirstName,  
 usr.LastName,  
 (usr.FirstName + ' '+usr.LastName) AS Name,  
 usr.Email,  
 FORMAT(usr.CreatedDate, 'dd MMMM yyyy') AS DateAdded,  
 usr.UserCode AS AccountHolderID,  
 ISNULL(userPic.PhotoPath,@DefaultUserPhotoPath) AS UserImagePath,  
 [Status]=(CASE  
                                                                        WHEN usr.EmailConfirmed IS NULL OR  usr.EmailConfirmed=0 THEN 3  -- Red  
                                                                        WHEN usr.EmailConfirmed=1 AND uprg.IsLinkedProgram=0 THEN 2  -- Orange  
                                                                        WHEN usr.EmailConfirmed=1 AND uprg.IsLinkedProgram=1 THEN 1  -- Green                                                                         
                                                                        END  
                                                                        ),  
[InvitationStatus]=(CASE  
                                                                  WHEN usr.InvitationStatus=1 THEN 1  -- Invite  
                                                                  WHEN usr.InvitationStatus=2 THEN 2  -- Reinvite  
                                                                  WHEN usr.InvitationStatus=3 THEN 3  -- Invitation Accepted                                                                         
                                                                  END  
                                                      ),  
 PlanName = STUFF((  
                   SELECT N', ' + pp.Name FROM ProgramPackage pp  
                   INNER JOIN UserPlans up ON up.ProgramPackageId=pp.Id  
                   WHERE up.UserId = usr.Id AND pp.ProgramId=@ProgramId AND ((ISNULL(@PlanId,'')='') OR pp.Id=@PlanId )  
                   FOR XML PATH(''), TYPE).value(N'.[1]', N'nvarchar(max)'), 1, 2, N''),  
PlanId = STUFF((  
                   SELECT N', ' +convert(varchar(20), pp.Id, 120)  FROM ProgramPackage pp  
                   INNER JOIN UserPlans up ON up.ProgramPackageId=pp.Id  
                   WHERE up.UserId = usr.Id AND pp.ProgramId=@ProgramId   
                   AND ((ISNULL(@PlanId,'')='') OR pp.Id=@PlanId )  
                   FOR XML PATH(''), TYPE).value(N'.[1]', N'nvarchar(max)'), 1, 2, N'')  
 FROM [User] usr  
  INNER JOIN UserRole urole ON uRole.UserId=usr.Id AND usr.OrganisationId=@OrganisationId  
  INNER JOIN [Role] rl on rl.Id=uRole.RoleId AND rl.Name='Basic User'  
  LEFT JOIN Photo userPic ON userPic.EntityId=usr.Id AND userPic.PhotoType=@PhotoTypeDetail  
  INNER JOIN UserProgram uprg ON usr.Id=uprg.UserId and uprg.ProgramId=@ProgramId  
  INNER JOIN OrganisationProgram op ON op.ProgramId=@ProgramId and op.OrganisationId=@OrganisationId  
  WHERE (usr.IsAdmin IS NULL OR usr.IsAdmin=0 ) AND usr.IsActive=1 and usr.IsDeleted=0  
  AND (  
   (ISNULL(@SearchValue,'')='' OR usr.UserCode LIKE '%'+@SearchValue+'%')  
   OR (ISNULL(@SearchValue,'')='' OR  usr.FirstName LIKE '%'+@SearchValue+'%')  
   OR (ISNULL(@SearchValue,'')='' OR usr.LastName LIKE '%'+@SearchValue+'%')  
   OR (ISNULL(@SearchValue,'')='' OR usr.Email LIKE '%'+@SearchValue+'%')  
 OR (ISNULL(@SearchValue,'')='' OR FORMAT(usr.CreatedDate, 'dd MMMM yyyy') LIKE '%'+@SearchValue+'%')  
   
  )  
  )  
  
  SELECT RowNumber,TotalCount,Id,FirstName,LastName,Name,Email,DateAdded,AccountHolderID,UserImagePath,[Status],[InvitationStatus],PlanName   
  FROM CTEResult  
  WHERE (RowNumber>@StartRow AND RowNumber<@EndRow) AND (ISNULL(@PlanId,'')='' OR @PlanId=0 OR @PlanId IN (PlanId))  
  END  
GO
/****** Object:  StoredProcedure [dbo].[GetAdminTransaction]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAdminTransaction]
@organisationId int,
@programId int,
@StartDate Datetime,
@EndDate Datetime
AS
BEGIN

DECLARE @Temp TABLE(
debitUserid int,
AdminName varchar(100)
)
INSERT INTO @Temp SELECT DISTINCT debitUserId, u.firstName + ' ' + u.lastName AS AdminName FROM userTransactioninfo AS uti
INNER JOIN [User] AS u ON uti.debitUserId = u.id
WHERE (uti.DebitTransactionUserType in (5,6,7)) AND uti.ProgramId = @programId AND uti.organisationId = @organisationId

SELECT u.id AS UserId, u.UserCode AS AccountHolderId,u.firstName AS FirstName,u.lastName AS LastName,
CONVERT(VARCHAR(10),uti.transactionDate,111) AS TransactionDate,
CAST(uti.transactionDate AS TIME) [Time],
'Credit' AS [Type],
pa.accountName AS AccountName,
t.AdminName AS AdminName,
uti.transactionAmount AS Amount
FROM UserTransactionInfo AS uti 
INNER JOIN [User] AS u ON uti.creditUserId = u.id
INNER JOIN ProgramAccounts AS pa ON uti.ProgramAccountId = pa.id
INNER JOIN @Temp AS t ON uti.debitUserId = t.debitUserId
WHERE u.IsDeleted = 0 AND CONVERT(VARCHAR(10), uti.transactionDate, 111) BETWEEN CONVERT(VARCHAR(10), @StartDate, 111) 
AND CONVERT(VARCHAR(10), @EndDate, 111)

END


GO
/****** Object:  StoredProcedure [dbo].[GetCardHolderStatement]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
   Stored Procecdure Name : GetCardHolderStatement
   Execution Process To Run: exec GetCardHolderStatement '8273'
*/

CREATE PROCEDURE [dbo].[GetCardHolderStatement] --exec GetCardHolderStatement '365,368',236,0,'2019-03-21 11:23:43.050','2019-12-21 11:23:43.050'
(
@UserIds int,
@organisationId int = 0,
@programId int = 0,
@StartDate Datetime,
@EndDate DateTime
)
AS
BEGIN

;WITH CardHolderCte(UserId,[Date],[Time],AccountName,transactionAmount,TransactionType,LocationUserType) AS
(
Select
uti.CreditUserId as UserId,
 DATEADD(dd, 0, DATEDIFF(dd, 0, uti.transactionDate)) as [Date],
CONVERT(VARCHAR(8), uti.transactionDate,114) AS [Time],
ISNULL(pa.accountName,NULL) as AccountName,
uti.transactionAmount,
'Credit' as TransactionType,
LocationUserType=(CASE
                                                                           When uti.DebitTransactionUserType = 1 Then 'Self'
																		   When uti.DebitTransactionUserType = 2 Then 'Benefactor'
																		   When uti.DebitTransactionUserType = 3 Then 'Merchant'
                                                                           ELSE 'Admin' END)
 from UserTransactionInfo uti
 LEFT JOIN ProgramAccounts pa on pa.Id=uti.ProgramAccountId 
WHERE (uti.CreditUserId IN (SELECT VALUE FROM dbo.ConvertStringToTable(@UserIds))) AND uti.CreditTransactionUserType=1 AND uti.IsActive=1 AND uti.IsDeleted=0 

UNION

Select
uti.DebitUserId as UserId,
 DATEADD(dd, 0, DATEDIFF(dd, 0, uti.transactionDate)) as [Date],
CONVERT(VARCHAR(8), uti.transactionDate,114) AS [Time],
ISNULL(pa.accountName,NULL) as AccountName,
uti.transactionAmount,
'Debit' as TransactionType,
LocationUserType=(CASE
                                                                           When uti.CreditTransactionUserType = 1 Then 'Self'
																		   When uti.CreditTransactionUserType = 2 Then 'Benefactor'
																		   When uti.CreditTransactionUserType = 3 Then 'Merchant'
                                                                           ELSE 'Admin' END)
 from UserTransactionInfo uti
 LEFT JOIN ProgramAccounts pa on pa.Id=uti.ProgramAccountId 
WHERE (uti.DebitUserId IN (SELECT VALUE FROM dbo.ConvertStringToTable(@UserIds))) AND uti.DebitTransactionUserType=1 AND uti.IsActive=1 AND uti.IsDeleted=0 
)
SELECT chcte.* FROM CardHolderCte chcte WHERE CONVERT(VARCHAR(10), chcte.[Date], 111) BETWEEN CONVERT(VARCHAR(10), @StartDate, 111) 
AND CONVERT(VARCHAR(10), @EndDate, 111)

--SELECT usr.id AS UserId, usr.UserCode,usr.FirstName,usr.LastName FROM [User] usr 
--INNER JOIN UserRole AS ur ON usr.id = ur.userId AND ur.RoleId = 1
--WHERE (usr.organisationId = @organisationId OR @organisationId = 0) AND (usr.programId = @programId OR @programId = 0)

END
GO
/****** Object:  StoredProcedure [dbo].[GetLocationByPlanReport]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetLocationByPlanReport]
	@organisationId int,
	@programId int,
	@merchantId int
AS
BEGIN

SELECT
uti.MerchantId AS MerchantId, 
pp.id AS PlanId,
pp.name PlanName,
COUNT(uti.id) AS TransactionCount,
(CASE WHEN uti.accountTypeId != 3 THEN SUM(uti.transactionAmount) ELSE 0 END) AS Units,
(CASE WHEN uti.accountTypeId = 3 THEN SUM(uti.transactionAmount) ELSE 0 END) AS Points,
0 AS Equivalency ,--Need to make it dynamic once cleared from client
(COUNT(uti.id) * (CASE WHEN uti.accountTypeId != 3 THEN SUM(uti.transactionAmount) ELSE 0 END)) AS Total --Need to make it dynamic once cleared from client
FROM UserTransactionInfo AS uti
INNER JOIN programPackage AS pp ON uti.planId = pp.id
WHERE uti.merchantId = @merchantId AND uti.programId = @programId AND uti.organisationId = @organisationId AND uti.transactionStatus = 1
GROUP BY uti.MerchantId, pp.id, pp.name, uti.accountTypeId

END

GO
/****** Object:  StoredProcedure [dbo].[GetMealPlanActivityReport]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec [GetMealPlanActivityReport] 1,1,9,'2019-03-21 11:23:43.050','2019-12-21 11:23:43.050' 
CREATE PROCEDURE [dbo].[GetMealPlanActivityReport]
@programAccountId varchar(max),
@organisationId int,
@programId int,
@StartDate Datetime,
@EndDate Datetime
AS
BEGIN
--	SELECT DISTINCT [MerchantId],[MerchantName],[AccountName],PERIODREMARK,SUM(CNT) CNT 
--INTO #NEWTABLE
--FROM 
--(
--    SELECT DISTINCT 
--    uti.MerchantId AS [MerchantId], m.name AS [MerchantName], pa.accountName AS [AccountName], uti.PERIODREMARK,
--    CASE WHEN uti.MerchantId IS NULL THEN NULL ELSE SUM(uti.transactionAmount) OVER(PARTITION BY uti.MerchantId, m.name, pa.accountName, uti.PERIODREMARK) END CNT  
--    FROM usertransactioninfo AS uti
--	INNER JOIN organisation AS m ON uti.MerchantId = m.id
--	INNER JOIN programAccounts AS pa ON uti.ProgramAccountId = pa.id  
--	WHERE m.IsActive = 1 AND m.IsDeleted = 0 AND m.organisationType = 3 
--	AND uti.programAccountId IN (SELECT value FROM dbo.ConvertStringToTable(@programAccountId)) 
--	AND uti.organisationId = @organisationId AND uti.programId = @programId
--	AND CONVERT(VARCHAR(10), uti.transactionDate, 111) BETWEEN CONVERT(VARCHAR(10), @StartDate, 111) 
--AND CONVERT(VARCHAR(10), @EndDate, 111) AND uti.accountTypeId = 1
--)TAB
--GROUP BY [MerchantId], [MerchantName], [AccountName], PERIODREMARK
--WITH ROLLUP
--DECLARE @cols NVARCHAR (MAX)
--DECLARE @NullToZeroCols NVARCHAR (MAX)


--SET @cols = SUBSTRING((SELECT DISTINCT ',['+PERIODREMARK+']' FROM #NEWTABLE GROUP BY PERIODREMARK  FOR XML PATH('')),2,8000)
--SET @NullToZeroCols = SUBSTRING((SELECT DISTINCT ',ISNULL(['+PERIODREMARK+'],0) AS ['+PERIODREMARK+']' 
--FROM #NEWTABLE GROUP BY PERIODREMARK FOR XML PATH('')),2,8000)
--DECLARE @query NVARCHAR(MAX)
--SET @query = 'SELECT P.MerchantId, P.MerchantName, P.AccountName,' + @NullToZeroCols + ',T2.CNT TOTAL FROM 
--             (
--                 SELECT DISTINCT [MerchantId], [MerchantName], [AccountName], PERIODREMARK,CNT FROM #NEWTABLE
--             ) x
--             PIVOT 
--             (
--                 SUM(CNT)
--                 FOR [PERIODREMARK] IN (' + @cols + ')
--            ) p
--            JOIN #NEWTABLE T2 ON P.[MERCHANTID]=T2.[MERCHANTID] 
--            WHERE P.MerchantId IS NOT NULL AND P.MerchantName IS NOT NULL AND T2.PERIODREMARK IS NULL AND T2.[MERCHANTID] IS NOT NULL 
--			AND T2.[MERCHANTNAME] IS NOT NULL AND T2.[ACCOUNTNAME] IS NOT NULL AND P.AccountName IS NOT NULL;'

--EXEC SP_EXECUTESQL @query
--DROP TABLE #NEWTABLE
SELECT DISTINCT 
    uti.MerchantId AS [MerchantId], m.name AS [MerchantName], pa.accountName AS [AccountName], uti.PERIODREMARK,
    CASE WHEN uti.MerchantId IS NULL THEN NULL ELSE SUM(uti.transactionAmount) OVER(PARTITION BY uti.MerchantId, m.name, pa.accountName, uti.PERIODREMARK) END CNT  
    FROM usertransactioninfo AS uti
	INNER JOIN organisation AS m ON uti.MerchantId = m.id
	INNER JOIN programAccounts AS pa ON uti.ProgramAccountId = pa.id  
	WHERE m.IsActive = 1 AND m.IsDeleted = 0 AND m.organisationType = 3 
	AND uti.programAccountId IN (SELECT value FROM dbo.ConvertStringToTable(@programAccountId)) 
	AND uti.organisationId = @organisationId AND uti.programId = @programId
	 AND uti.accountTypeId = 1
END


GO
/****** Object:  StoredProcedure [dbo].[GetMerchantAuditReport]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--  exec GetMerchantAuditReport 119,500,'08/04/2019','08/22/2019'
CREATE PROCEDURE [dbo].[GetMerchantAuditReport]   
 -- Add the parameters for the stored procedure here  
 @programId int,  
 @merchantId int,  
 @StartDate DateTime,  
 @EndDate DateTime  
AS  
BEGIN  
 SELECT  

p.id As ProgramId,  
p.name AS programName,  
pa.id AS AccountId,  
pa.accountName AS AccountName,  
u.id AS UserId,  
u.userCode AS AccountHolderID,  
u.firstName AS FirstName,  
u.LAStName AS LAStName,  
uti.transactionDate AS DateANDTimeOfScan,  
CASt(uti.transactionDate AS VARCHAR(10)) AS SettleDate,  
'Auth' AS [Type], --Need to make it dynamic once Jpost AND I2c Db is confirmed  
o.name AS MerchantName,  
pa.accountname AS AccountName,  
mt.terminalId AS TID,  
uti.TransactionId AS TxnId,  
(CASE WHEN pa.accountTypeId = 3 THEN ISNULL(uti.transactionAmount,0) ELSE 0 END) AS Points,  
(CASE WHEN pa.accountTypeId != 3 THEN ISNULL(uti.transactionAmount,0) ELSE 0 END) AS Units,  
uti.periodRemark AS Period,  
(CASE WHEN pa.accountTypeId != 3 THEN ISNULL(uti.transactionAmount,0) ELSE 0 END) AS Equiv,--Need to make it dynamic once Jpost AND I2c Db is confirmed  
1 AS Total, --Need to make it dynamic once Jpost AND I2c Db is confirmed  
pp.id AS PlanId, pp.name AS PlanName  
 FROM organisation AS o   
INNER JOIN organisationProgram AS op ON o.id = op.organisationId  
INNER JOIN program AS p ON op.programId = p.id  
INNER JOIN programAccounts AS pa ON p.id = pa.programId  
INNER JOIN planProgramAccountsLinking AS ppal on pa.id = ppal.programAccountId  
INNER JOIN programPackage AS pp on ppal.planId = pp.id  
INNER JOIN userplans AS up on pp.id = up.programPackageId  
INNER JOIN [user] AS u on up.userid = u.id  
LEFT JOIN userTransactionInfo AS uti on (u.id = uti.debituserid OR u.id = uti.credituserid)  
LEFT JOIN MerchantTerminal AS mt ON uti.TerminalId = mt.id AND o.id = mt.organisationId   
WHERE o.id = @merchantId AND o.organisationType = 3 AND o.IsActive = 1 AND o.IsDeleted = 0   
AND CONVERT(VARCHAR(10), uti.transactionDate, 111) BETWEEN CONVERT(VARCHAR(10), @StartDate, 111)   
AND CONVERT(VARCHAR(10), @EndDate, 111) AND op.programid = @programId  
GROUP BY p.id, p.name,pa.id, pa.accountname,pp.id,pp.name,u.userCode,uti.transactionAmount,  
 u.id,u.firstName,u.LAStName,transactionDate,o.name,mt.terminalId,uti.TransactionId,pa.accountTypeId,uti.periodRemark  
END  
GO
/****** Object:  StoredProcedure [dbo].[GetOrgScheduleHours]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************************************************
SP Name: GetOrgScheduleHours
Description: Get the organisation's schedule based on its setting for schedule and holiday hours.
Test Result Run : Exec GetOrgScheduleHours 34,1,0
***************************************************************************************************************************/
CREATE PROCEDURE [dbo].[GetOrgScheduleHours]
(
	@Id INT,
	@IsActive BIT,
	@IsDeleted BIT
)
AS
 BEGIN
	DECLARE @StartWeekDate DATETIME,@EndWeekDate DATETIME
 
	SET @StartWeekDate=DATEADD(DAY, 2 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
	SET @EndWeekDate=DATEADD(DAY, 8 - DATEPART(WEEKDAY, GETDATE()), CAST(GETDATE() AS DATE))
 PRINT @StartWeekDate
 PRINT @EndWeekDate
	SELECT * INTO #WeekDates FROM [dbo].GetCurrentWeekDates(@StartWeekDate,@EndWeekDate) 
 
	CREATE TABLE #ResultContent  (
	S_No INT,
	id INT,
	organisationId INT,
	workingDay VARCHAR(20), 
	openTime TIME(7),
	closedTime TIME(7),
	isActive BIT,
	createdBy INT,
	createdDate DATETIME,
	modifiedBy INT,
	modifiedDate DATETIME,
	isDeleted BIT,
	isHoliday BIT,
	HolidayName VARCHAR(20) NULL,
	IsHolidayNameToShow BIT
	)
 
    SELECT 
	 Id,
     OrganisationId,
     WorkingDay,
     OpenTime,
     ClosedTime,
	 HolidayDate,
	 IsHoliday, 
	 IsActive,
	 CreatedBy ,
	 CreatedDate,
	 ModifiedBy,
	 ModifiedDate,
	 IsDeleted,
	 HolidayName,
	 IsForHolidayNameToShow
	INTO #tmpScheduleHours 
	 FROM OrganisationSchedule 
	WHERE (IsActive = @IsActive OR IsActive is null) 
	      AND OrganisationId = @Id AND (IsDeleted = @IsDeleted OR IsDeleted is null) 
 
 
    --SELECT * FROM #tmpScheduleHours
    --Select * from #WeekDates
    DECLARE @RowCount INT
    DECLARE @WeekDateSelect DATETIME
    DECLARE @WeekDayNameSelect NVARCHAR(15)

    SET @RowCount = 1;
 
    WHILE @RowCount <= (SELECT MAX(S_NO) FROM #WeekDates)
      BEGIN
        SET @WeekDateSelect=(SELECT WeekDates FROM #WeekDates WHERE S_No=@RowCount) 
        SET @WeekDayNameSelect=(SELECT WeekDayName FROM #WeekDates WHERE S_No=@RowCount) 
     
        IF EXISTS(SELECT CONVERT(Date, HolidayDate) FROM #tmpScheduleHours WHERE CONVERT(Date, HolidayDate) IN ( CONVERT(Date, @WeekDateSelect)) AND IsHoliday=1)
         BEGIN
		
           INSERT INTO #ResultContent(S_No,id,organisationId,workingDay,openTime,closedTime,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,isHoliday)
                    SELECT @RowCount,id,organisationId,DATENAME(weekday,HolidayDate),openTime,closedTime,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,isHoliday
       			  FROM #tmpScheduleHours WHERE CONVERT(Date, HolidayDate) IN (CONVERT(Date, @WeekDateSelect)) AND IsHoliday=1 AND DATENAME(weekday,HolidayDate)=@WeekDayNameSelect
         END
        ELSE
         BEGIN
		
           INSERT INTO #ResultContent(S_No,id,organisationId,workingDay,openTime,closedTime,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,isHoliday)
                    SELECT @RowCount,id,organisationId,workingDay,openTime,closedTime,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,isHoliday 
      			  FROM #tmpScheduleHours WHERE (IsHoliday=0  OR IsHoliday IS NULL) AND workingDay=@WeekDayNameSelect
         END
        SET @RowCount=@RowCount+1;
       
      END
    Select * FROM #ResultContent

	Select OrganisationId,HolidayName,HolidayDate,IsForHolidayNameToShow from #tmpScheduleHours Where CONVERT(Date, HolidayDate) BETWEEN CONVERT(Date, @StartWeekDate) AND CONVERT(Date, @EndWeekDate) and IsHoliday=1 Order By HolidayDate ASC

    DROP TABLE #tmpScheduleHours
    DROP TABLE #WeekDates
    DROP TABLE #ResultContent
 END


GO
/****** Object:  StoredProcedure [dbo].[GetPlanBalanceDetailReport]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
  SP Name: GetAccountBalanceDetailReport
  Execution Process: exec GetAccountBalanceDetailReport 40,10
*/

CREATE PROCEDURE [dbo].[GetPlanBalanceDetailReport] --exec GetPlanBalanceDetailReport 276,'227,228,229',119,479,'08/04/2019','08/22/2019'
(
@planId int,
@accountId varchar(max),
@programId int,
@organisationId int,
@startDate DateTime,
@endDate DateTime
)
AS
BEGIN
--Declare @temp Table
--(
--	Id int,
--	UserId varchar(20),
--	AccountName varchar(100),
--	[Type] varchar(10),
--	FirstName varchar(100),
--	LastName varchar(100),
--	Balance decimal(18,2),
--	LastUsed DateTime,
--	AccountType varchar(100)
--)
--INSERT INTO @temp 
SELECT 
 u.Id,
 u.UserCode as UserID,
 pa.AccountName,
 (CASE WHEN pa.accountTypeId = 1 THEN 'Unit' WHEN pa.accountTypeId = 2 THEN 'Points' WHEN pa.accountTypeId = 3 THEN '' End) AS Type,
 u.FirstName,
 u.LastName,
 ISNULL(dbo.fnGetUserBalanceDetail(u.Id,1,@planId,pa.id,pa.AccountTypeId),0) as Balance,
 LastUsed=MAX(utf.transactionDate),
 ac.accountType
FROM PlanProgramAccountsLinking ppal
INNER JOIN ProgramAccounts pa ON pa.Id=ppal.ProgramAccountId AND pa.IsActive=1 AND pa.IsDeleted=0
INNER JOIN ProgramPackage pp on pp.Id=ppal.PlanId AND pp.IsActive=1 AND pp.IsDeleted=0
Inner JOIN UserPlans up ON ppal.PlanId=up.ProgramPackageId
INNER JOIN [User] u ON u.Id=up.UserId AND u.IsActive=1 AND u.IsDeleted=0
INNER JOIN AccountType ac On ac.Id=pa.AccountTypeId
INNER JOIN UserTransactionInfo AS utf ON utf.debitUserId=u.Id AND utf.DebitTransactionUserType=1 AND utf.IsActive=1 AND utf.IsDeleted=0 AND utf.planId=@planId
WHERE ppal.planId=@planId AND pa.programId = @programId AND pa.id in (SELECT value FROM dbo.ConvertStringToTable(@accountId))
GROUP BY u.Id, u.UserCode, pa.AccountName, pa.accountTypeId, u.FirstName,
u.LastName, pa.id, ac.accountType 
HAVING CONVERT(VARCHAR(10), MAX(utf.transactionDate), 111) BETWEEN CONVERT(VARCHAR(10), @startDate, 111) 
AND CONVERT(VARCHAR(10), @endDate, 111)


--SELECT * FROM @temp WHERE CONVERT(VARCHAR(10), LastUsed, 111) BETWEEN CONVERT(VARCHAR(10), @startDate, 111) 
--AND CONVERT(VARCHAR(10), @endDate, 111)




END
GO
/****** Object:  StoredProcedure [dbo].[GetPlanBalanceSummaryReport]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPlanBalanceSummaryReport] 
	-- Add the parameters for the stored procedure here
	@programId int,
	@planId varchar(max),
	@status int,
	@StartDate DateTime,
	@EndDate DateTime
AS
BEGIN
	select pa.id, pa.accountName,ppal.planid,Count(up.userid) AS countOfAccountHolder,
		TotalBalance = ISNULL((SELECT SUM(transactionAmount) from [UserTransactionInfo] WHERE credituserid in (select userid from userplans sup where sup.programPackageId = ppal.planId) AND CreditTransactionUserType = 1 AND programAccountId = pa.id AND transactionStatus = 1) - 
						(SELECT SUM(transactionAmount) from [UserTransactionInfo] WHERE debituserid in (select userid from userplans sup where sup.programPackageId = ppal.planId) AND DebitTransactionUserType = 1 AND programAccountId = pa.id AND transactionStatus = 1), 0) 
	    from programAccounts pa 
INNER join [PlanProgramAccountsLinking] ppal on pa.id = ppal.programAccountid 
INNER JOIN [UserPlans] AS up ON up.programPackageId = ppal.planId
INNER JOIN [ProgramPackage] AS p ON ppal.planId = p.id
where pa.programid = @programid AND (ppal.planId in (select Value from dbo.ConvertStringToTable(@planid)) or @planid = '0')
AND pa.IsDeleted = 0 AND (pa.IsActive = @status OR @status = -1 ) AND CONVERT(VARCHAR(10), p.CreatedDate, 111) BETWEEN CONVERT(VARCHAR(10), @StartDate, 111) 
AND CONVERT(VARCHAR(10), @EndDate, 111)
group by pa.id, pa.accountName, ppal.planid, pa.accountTypeId,pa.intialBalanceCount
END

GO
/****** Object:  StoredProcedure [dbo].[GetSodexhoUserTypebyPartnerUserId]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE	 PROCEDURE [dbo].[GetSodexhoUserTypebyPartnerUserId]  
(  
 @PartnerId INT, @PartnerUserId  NVARCHAR(50))  
 as  
 begin
 DECLARE
 @id int
  
  SELECT @id= id  FROM dbo.[User]  WHERE IsDeleted=0 AND IsActive =1 AND  PartnerId = @PartnerId AND PartnerUserId = @PartnerUserId


  EXECUTE dbo.sp_GetSodexhoUserType @Id =@id
 
 end
  
GO
/****** Object:  StoredProcedure [dbo].[InserAndCalculateUserLoyaltyPoints]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InserAndCalculateUserLoyaltyPoints]  
(  @PartnerUserId  NVARCHAR(50) ,
 @transactionId  NVARCHAR(200) ,
 @transactionAmount FLOAT ,
@transactionDate  DATETIME,
@TranlogID BIGINT ,
@usertype VARCHAR(50)=NULL
     )  
 as  
 BEGIN
 
  DECLARE
 @UserID INT
 ,@pointsEarned FLOAT ,
  @totalPoints  FLOAT ,
  @rewardAmount  FLOAT,
  @leftOverPoints  FLOAT ,
  @isThresholdReached  BIT,
 @VipRatio  FLOAT,
 @bitePayRatio FLOAT,
 @dcbFlexRatio FLOAT,
 @RegularRatio FLOAT,
 @loyalityThreshhold FLOAT,
 @globalReward FLOAT,
 @isFirsttran bit	=0,
 @firstTranBonus FLOAT=0
	  -- get user id
SELECT @UserID= id  FROM dbo.[User]  WHERE IsDeleted=0 AND IsActive =1 AND  PartnerId = 1 AND PartnerUserId = @PartnerUserId

SELECT   @loyalityThreshhold =gs.loyalityThreshhold ,@globalReward = globalReward
FROM dbo.[User] JOIN  OrgLoyalityGlobalSettings gs ON gs.organisationId = [User].OrganisationId
WHERE  [User].Id =@UserID

-- get loyalty calculation Settings

IF EXISTS(SELECT * FROM  dbo.[User] INNER JOIN
    SiteLevelOverrideSettings ON [User].ProgramId = SiteLevelOverrideSettings.programId
				where [User].Id=@UserID
 )
 BEGIN
  SELECT @bitePayRatio = siteLevelBitePayRatio , @VipRatio =siteLevelUserStatusVipRatio , @dcbFlexRatio =siteLevelDcbFlexRatio
  ,@RegularRatio =siteLevelUserStatusRegularRatio ,@firstTranBonus = FirstTransactionBonus
    FROM  dbo.[User] INNER JOIN
    SiteLevelOverrideSettings ON [User].ProgramId = SiteLevelOverrideSettings.programId
				where [User].Id=@UserID

END
ELSE IF EXISTS( SELECT * FROM dbo.[User] JOIN  OrgLoyalityGlobalSettings gs ON gs.organisationId = [User].OrganisationId
WHERE  [User].Id =@UserID ) 
BEGIN 

SELECT 
@bitePayRatio = gs.bitePayRatio , @VipRatio =gs.userStatusVipRatio , @dcbFlexRatio = gs.dcbFlexRatio
  ,@RegularRatio =gs.userStatusRegularRatio ,@firstTranBonus = FirstTransactionBonus
FROM dbo.[User] JOIN  OrgLoyalityGlobalSettings gs ON gs.organisationId = [User].OrganisationId
WHERE  [User].Id =@UserID

END
ELSE 
BEGIN
SET @bitePayRatio =0 
SET @VipRatio =0 
SET @dcbFlexRatio = 0
SET @RegularRatio =0
SET @firstTranBonus = 0
END

---- calculate points


IF ( @usertype ='vip' )
BEGIN
 SET @pointsEarned = (@VipRatio * @bitePayRatio * @transactionAmount)
END
ELSE IF  (@usertype ='bite pay' )
BEGIN
SET @pointsEarned = @RegularRatio * @bitePayRatio * @transactionAmount
END
ELSE
BEGIN 
SET @pointsEarned = @RegularRatio * @dcbFlexRatio *  @transactionAmount
END

DECLARE  @prevleftOverPoints FLOAT , @prevtotalPoints FLOAT ,@previsThresholdReached BIT , @iee INT 


IF EXISTS (
SELECT TOP 1 * from UserLoyaltyPointsHistory Where userId=@UserID )
BEGIN 
Select top 1 @iee=id,  @prevleftOverPoints =leftOverPoints, @prevtotalPoints = totalPoints,@previsThresholdReached = isThresholdReached   from UserLoyaltyPointsHistory Where userId=@UserID order by id DESC
SET @isFirsttran=0
SET @firstTranBonus=0
END
ELSE
BEGIN

SET @pointsEarned= @pointsEarned+@firstTranBonus
SET @isFirsttran=1
SET @iee=0
  SET @prevleftOverPoints =0
  SET @prevtotalPoints = 0
  SET @previsThresholdReached =0
end

IF( @previsThresholdReached =0)
BEGIN
SET @totalPoints = @pointsEarned + @prevtotalPoints;

END
else
BEGIN
SET @totalPoints = @pointsEarned + @prevleftOverPoints;
END


IF(@loyalityThreshhold <=@totalPoints )
BEGIN

--SET   @numberofThresholdReached= FLOOR(@totalPoints/ @loyalityThreshhold) 
set  @leftOverPoints =CAST((@totalPoints) AS DECIMAL(18,2))  % CONVERT(DECIMAL(18,2), @loyalityThreshhold) 

 SET @isThresholdReached =1 
--  SET @leftOverPoints =@totalPoints - @loyalityThreshhold
  SET @rewardAmount = @globalReward * FLOOR(@totalPoints/ @loyalityThreshhold) 
END
ELSE
BEGIN
 SET @isThresholdReached =0
 SET @leftOverPoints =0
 SET @globalReward =0
 SET @rewardAmount =0
end




	INSERT INTO dbo.UserLoyaltyPointsHistory
        ( userId ,
          transactionId ,
          transactionAmount ,
          pointsEarned ,
          totalPoints ,
          rewardAmount ,
          leftOverPoints ,
          isThresholdReached ,
          transactionDate ,
          createdDate ,
          TranlogID
        )
VALUES  ( @UserID , -- userId - int
         @transactionId , -- transactionId - nvarchar(200)
          @transactionAmount , -- transactionAmount - decimal
          @pointsEarned , -- pointsEarned - decimal
          @totalPoints , -- totalPoints - decimal
          @rewardAmount , -- rewardAmount - decimal
          @leftOverPoints , -- leftOverPoints - decimal
          @isThresholdReached , -- isThresholdReached - bit
          @transactionDate , -- transactionDate - datetime
          GETDATE() , -- createdDate - datetime
          @TranlogID  -- TranlogID - bigint
        )
SELECT @pointsEarned pointsEarned,@isThresholdReached isThresholdReached , @totalPoints totalPoints , @rewardAmount rewardAmount , @loyalityThreshhold loyalityThreshhold  , ROUND( (@totalPoints*100)/@loyalityThreshhold ,2)PercentProgress ,@leftOverPoints leftOverPoints, @isFirsttran isFirsttran, @UserID UserID

 	
end
GO
/****** Object:  StoredProcedure [dbo].[InsertDummyData]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*********************************************************************************************************************************************************************    
    
SP Name: InsertDummyData    
Test Executable Statement: Exec InsertDummyData    
    
**********************************************************************************************************************************************************************/    
    
CREATE PROCEDURE [dbo].[InsertDummyData]    
AS    
BEGIN    
DECLARE @NumOrganisation INT,@NumProgram INT;     
DECLARE @ProgramAutoId NVARCHAR(30), @MerchantIdsConcat NVARCHAR(50)    
       
--CREATE TABLE #tempBenefactorIds    
--(    
--ID int identity(1,1) not null,    
--BenefactorId int not null    
--)    
    
    
--------------------------------------------------------------------------  Organisation -------------------------------------------------------------------------------------------    
    
SET @NumOrganisation=1;    
PRINT 'Start Organisation'    
WHILE @NumOrganisation <= 100     
BEGIN    ------ Organisation Loop    
INSERT INTO Organisation(name,addressLine1,addressLine2,location,organisationType,emailAddress,contactNumber,isMaster,isActive,createdBy,createdDate,modifiedBy,modifiedDate,    
isDeleted,maxCapacity,[description],websiteURL,getHelpEmail,getHelpPhone1,getHelpPhone2,facebookURL,twitterURL,skypeHandle,ContactName,ContactTitle,    
MerchantId,country,[state],city,zip,isMapVisible,isClosed,businessTypeId,OrganisationSubTitle,dwellTime)     
VALUES(CONCAT('OrganisationData-',@NumOrganisation),'#245, Toronto, Canada',null,null,2,CONCAT('adminnew',@NumOrganisation,'@aprio.com'),'(123) 345 6787',0,1,null,GetUTCDATE(),NULL,GETUTCDATE(),    
0,NULL,'Aprip University Lorem ipsum dolor sit amet',NULL,NULL,NULL,NULL,NULL,NULL,NULL,CONCAT('Admin Aprio',@NumOrganisation),'Mr.',    
NULL,'Canada','Toronto','Toronto','647833',NULL,NULL,NULL,'APU',NULL)    
    
DECLARE @OrganisationId INT;    
SET @OrganisationId=IDENT_CURRENT('Organisation')      
    
--------------------------------------------------------------------------  Program ---------------------------------------------------------------------------------------------------------------------------    
DECLARE @ProgramId INT,@ProgramMaxId INT    
SET @NumProgram=1    
PRINT 'Start Program'    
while @NumProgram <= 100     
begin    ------ Program Loop    
Create Table #tempMerchantIds    
(    
ID int identity(1,1) not null,    
MerchantId int not null    
)    
    
CREATE TABLE #tempPlanIds    
(    
ID int identity(1,1) not null,    
PlanId int not null    
)    
    
CREATE TABLE #tempProgramAccountIds    
(    
ID int identity(1,1) not null,    
ProgramAccountId int not null    
)    
SET @ProgramMaxId=(SELECT MAX(ID) From Program)    
    
INSERT INTO Program(name,[description],organisationId,logoPath,colorCode,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,timeZone,ProgramCodeId,ProgramTypeId,website,[address],country,[state],city,zipcode,customName,customInputMask,customErrorMessaging,customInstructions,programCustomFields,AccountHolderGroups,AccountHolderUniqueId)    
Values(CONCAT('ProgramData-',@OrganisationId,'-',@NumProgram),'Lorem ipsum dolor sit amet',NULL,NULL,NULL,1,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0,'Eastern Standard Time',CONCAT('P1000-',@ProgramMaxId+1),3,NULL,'Edmonton International Airport (YEG), Airpor
t Road, Edmonton International Airport, AB, Canada','Canada','Alberta',NULL,'T9E 0V3','UserID','XXX-XXX-XXXX','Please enter  user id.',NULL,'[]','Test',CONCAT('AC1000-',@ProgramMaxId+1))      
SET @ProgramId=IDENT_CURRENT('Program')     
    
----------------------------------------------------------------------- Organisation Program (Organisation) ---------------------------------------------------------------------------------------------------    
    
INSERT INTO OrganisationProgram(organisationId, programId,CreatedDate,ModifiedDate,IsActive,IsDeleted)    
VALUES(@OrganisationId,@ProgramId,GETUTCDATE(),GETUTCDATE(),1,0)    
    
---------------------------------------------------------------------------------------   Merchant    -------------------------------------------------------------------------------------------    
    
DECLARE @NumMerchant INT=1;    
PRINT 'Start Merchant'    
while @NumMerchant <= 100     
BEGIN   ------ Merchant Loop    
    
DECLARE @MerchantMaxId INT, @MerchantId INT    
    
   SET @MerchantMaxId=(SELECT MAX(ID) From Program)    
    INSERT into Organisation(name,addressLine1,addressLine2,location,organisationType,emailAddress,contactNumber,isMaster,isActive,createdBy,createdDate,modifiedBy,modifiedDate,    
isDeleted,maxCapacity,[description],websiteURL,getHelpEmail,getHelpPhone1,getHelpPhone2,facebookURL,twitterURL,skypeHandle,ContactName,ContactTitle,    
MerchantId,country,[state],city,zip,isMapVisible,isClosed,businessTypeId,OrganisationSubTitle,dwellTime)     
Values(CONCAT('MerchantData-',@OrganisationId,'-',@ProgramId,'-',@NumMerchant),'#245, Toronto, Canada',null,'-125.26824210000001, 50.2057635',3,CONCAT('merchant',@NumMerchant,'@aprio.com'),'(123) 345 6787',0,1,null,GetUTCDATE(),NULL,GETUTCDATE(),    
0,NULL,'Aprip University Lorem ipsum dolor sit amet',NULL,NULL,NULL,NULL,NULL,NULL,NULL,CONCAT('merchant title',@NumMerchant),'Mr.',    
CONCAT('M1000-',@MerchantMaxId+1),'Canada','Toronto','Toronto','647833',NULL,NULL,NULL,'APU',15)     
    
    
SET @MerchantId=IDENT_CURRENT('Organisation')     
INSERT INTO #tempMerchantIds(MerchantId)VALUES(@MerchantId)    
INSERT INTO OrganisationProgram(organisationId, programId,CreatedDate,ModifiedDate,IsActive,IsDeleted)    
VALUES(@MerchantId,@ProgramId,GETUTCDATE(),GETUTCDATE(),1,0)    
Select * from #tempMerchantIds    
      
  --------------------------------------------------------------------------------------- Program Account Linking  ----------------------------------------------------------------------    
  DECLARE @NumProgramAccountLinkingType INT=1;    
  PRINT 'Start ProgramAccountLinking'    
  while @NumProgramAccountLinkingType <= 3     
BEGIN   ------ Program Account Linking Loop    
    
DECLARE @ProgramAccountLinkId INT    
    
      
 INSERT INTO ProgramAccountLinking(programId,accountTypeId) VALUES(@ProgramId,@NumProgramAccountLinkingType)    
    
 SET @ProgramAccountLinkId=IDENT_CURRENT('ProgramAccountLinking')     
    
 INSERT INTO ProgramMerchantAccountType(organisationId,programAccountLinkingId) VALUES(@MerchantId,@ProgramAccountLinkId)    
    
     
   SET @NumProgramAccountLinkingType = @NumProgramAccountLinkingType + 1;    
    
    
END   ------ Program Account Linking Loop    
PRINT 'End ProgramAccountLinking'    
SET @NumMerchant=@NumMerchant+1    
END   ----- Merchant Loop    
    
PRINT 'End Merchant'    
    
----------------------------------------------------------------------------------  Program Accounts  --------------------------------------------------------------------------------    
  DECLARE @ProgramAccountMaxId INT,@ProgramAccountId INT,@NumProgramAccount INT=1;    
  PRINT 'Start ProgramAccounts'    
  while @NumProgramAccount <= 40     
BEGIN   ------ Program Accounts Loop    
    
SET @ProgramAccountMaxId=(SELECT MAX(ID) From ProgramAccounts)    
IF(@NumProgramAccount=1)    
BEGIN    
INSERT INTO ProgramAccounts(accountName,accountTypeId,programId,passType,intialBalanceCount,resetPeriodType,resetDay,resetTime,    
maxPassUsage,isPassExchangeEnabled,exchangePassValue,exchangeResetPeriodType,exchangeResetDay,exchangeResetTime,isRollOver,flexEndDate,isActive,    
createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,ProgramAccountId,maxPassPerWeek,maxPassPerMonth,resetDateOfMonth,flexMaxSpendPerDay,    
flexMaxSpendPerWeek,flexMaxSpendPerMonth,exchangeResetDateOfMonth,vplMaxBalance,vplMaxAddValueAmount,vplMaxNumberOfTransaction)    
VALUES(CONCAT('AccountData-',@OrganisationId,'-',@ProgramId,'-',@NumProgramAccount),@NumProgramAccount,@ProgramId,5,30,2,6,NULL,2,1,1,1,NULL,NULL,0,NULL,1,NULL,GetUTCDATE(),NULL,GETUTCDATE(),0, CONCAT('PA1000-',@ProgramAccountMaxId+1),    
 NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL)    
 END    
 ELSE IF(@NumProgramAccountLinkingType=2)BEGIN    
 INSERT INTO ProgramAccounts(accountName,accountTypeId,programId,passType,intialBalanceCount,resetPeriodType,resetDay,resetTime,    
maxPassUsage,isPassExchangeEnabled,exchangePassValue,exchangeResetPeriodType,exchangeResetDay,exchangeResetTime,isRollOver,flexEndDate,isActive,    
createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,ProgramAccountId,maxPassPerWeek,maxPassPerMonth,resetDateOfMonth,flexMaxSpendPerDay,    
flexMaxSpendPerWeek,flexMaxSpendPerMonth,exchangeResetDateOfMonth,vplMaxBalance,vplMaxAddValueAmount,vplMaxNumberOfTransaction)    
VALUES(CONCAT('AccountData-',@OrganisationId,'-',@ProgramId,'-',@NumProgramAccount),@NumProgramAccount,@ProgramId,5,45,2,6,NULL,2,1,1,1,NULL,NULL,0,NULL,1,NULL,GetUTCDATE(),NULL,GETUTCDATE(),0, CONCAT('PA1000-',@ProgramAccountMaxId+1),    
 NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL)    
 END    
 ELSE    
 BEGIN    
 INSERT INTO ProgramAccounts(accountName,accountTypeId,programId,passType,intialBalanceCount,resetPeriodType,resetDay,resetTime,    
maxPassUsage,isPassExchangeEnabled,exchangePassValue,exchangeResetPeriodType,exchangeResetDay,exchangeResetTime,isRollOver,flexEndDate,isActive,    
createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,ProgramAccountId,maxPassPerWeek,maxPassPerMonth,resetDateOfMonth,flexMaxSpendPerDay,    
flexMaxSpendPerWeek,flexMaxSpendPerMonth,exchangeResetDateOfMonth,vplMaxBalance,vplMaxAddValueAmount,vplMaxNumberOfTransaction)    
VALUES(CONCAT('AccountData-',@OrganisationId,'-',@ProgramId,'-',@NumProgramAccount),@NumProgramAccount,@ProgramId,NULL,NULL,NULL,NULL,NULL,2,1,1,1,NULL,NULL,0,NULL,1,NULL,GetUTCDATE(),NULL,GETUTCDATE(),0, CONCAT('PA1000-',@ProgramAccountMaxId+1),    
 NULL,NULL,NULL,NULL,NULL,NULL,NULL,12.00,12.00,12)    
 END    
SET @ProgramAccountId=IDENT_CURRENT('ProgramAccounts')    
    
  --------------------------------------------------------------------------  AccountMerchantRules -------------------------------------------------------------------------------    
  DECLARE @AccountMerchantRuleMerchantCount INT = 1,     
   @AccountMerchantRuleMerchantMaxCount INT, @AccountMerchantRuleId INT    
  -- Select * from #tempMerchantIds    
 SET @AccountMerchantRuleMerchantMaxCount=(SELECT COUNT(*)    
 FROM #tempMerchantIds)    
 PRINT @AccountMerchantRuleMerchantMaxCount    
  PRINT 'Start AccountMerchantRules'    
 WHILE(@AccountMerchantRuleMerchantCount <= @AccountMerchantRuleMerchantMaxCount)    
   BEGIN  ---- Account Merchant Rules Loop    
      
   DECLARE @MerchantIdSelect INT    
         
   SELECT TOP(1) @MerchantIdSelect=MerchantId FROM #tempMerchantIds WHERE ID=@AccountMerchantRuleMerchantCount    
    
   INSERT INTO AccountMerchantRules(programAccountID,merchantId,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,accountTypeId)    
   VALUES(@ProgramAccountId,@MerchantIdSelect,1,NULL,GETUTCDATE(),NULL,GetUTCDATE(),0,@NumProgramAccountLinkingType)    
    
   SET @AccountMerchantRuleId=IDENT_CURRENT('AccountMerchantRules')    
   DECLARE @NumAccountMerchantRuleDetail INT=1;    
   WHILE(@NumAccountMerchantRuleDetail <= 4)    
   BEGIN  ----  Account Merchant Rules Detail Loop    
      
      
   INSERT INTO AccountMerchantRulesDetail(accountMerchantRuleId,mealPeriod,maxPassUsage,minPassValue,maxPassValue,transactionLimit)    
   VALUES(@AccountMerchantRuleId,@NumAccountMerchantRuleDetail,1,0,1,NULL)    
    
  SET @NumAccountMerchantRuleDetail=@NumAccountMerchantRuleDetail+1      
    
   END  --  Account Merchant Rules Detail Loop    
   PRINT 'End AccountMerchantRulesDetail'    
   SET @AccountMerchantRuleMerchantCount=@AccountMerchantRuleMerchantCount+1    
   END  ---- Account Merchant Rules Loop    
    
PRINT 'End AccountMerchantRules'    
    
    
INSERT INTO #tempProgramAccountIds(ProgramAccountId)VALUES(@ProgramAccountId)    
    
SET @NumProgramAccount=@NumProgramAccount+1    
END ------ Program Accounts Loop    
PRINT 'End ProgramAccounts'    
-----------------------------------------------------------------------------------  ProgramPackage (Plans)  ------------------------------------------------------------------------------------------------------------    
DECLARE @PlanMaxId INT,@PlanId INT,@NumPlans INT=1    
while @NumPlans <= 50     
BEGIN      
SET @PlanMaxId=(SELECT MAX(ID) From ProgramPackage)    
INSERT INTO ProgramPackage(name,programId,noOfMealPasses,noOfFlexPoints,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,startDate,    
endDate,startTime,endTime,[description],clientId,planId,isActive)    
VALUES(CONCAT('PlanData-',@OrganisationId,'-',@ProgramId,'-',@NumPlans),@ProgramId,23,23,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0,'2019-09-06 10:57:10.130',    
'2019-10-06 10:57:10.130','10:45:00.0000000','23:45:00.0000000','Lorem ipsum dolor sit amet',CONCAT('P1000-',@PlanMaxId+1),NULL,1)    
    
 SET @PlanId=IDENT_CURRENT('ProgramPackage')    
    
   DECLARE @NumPlanProgramAccountLinking INT=1,    
   @PlanProgramAccountLinkingtMaxCount INT    
 SET @PlanProgramAccountLinkingtMaxCount=(SELECT COUNT(*)    
 FROM #tempProgramAccountIds)    
  PRINT 'Start PlanProgramAccountLinking'    
 WHILE(@NumPlanProgramAccountLinking <= @PlanProgramAccountLinkingtMaxCount)    
   BEGIN   ---- PlanProgramAccountLinking Loop    
      
   DECLARE @AccountIdSelect INT    
       
    
   SELECT TOP(1) @AccountIdSelect=ProgramAccountId FROM #tempProgramAccountIds WHERE ID=@NumPlanProgramAccountLinking    
    
  INSERT INTO PlanProgramAccountsLinking(planId,programAccountId) VALUES(@PlanId,@AccountIdSelect)    
    
 SET @NumPlanProgramAccountLinking=@NumPlanProgramAccountLinking+1    
    
   END  ---- PlanProgramAccountLinking Loop     
   PRINT 'End PlanProgramAccountLinking'    
   Print CONCAT('Plan ID: ',@PlanId)    
  INSERT INTO #tempPlanIds(PlanId) VALUES(@PlanId)       
    
 SET @NumPlans=@NumPlans+1;    
END    
    
--------------------------------------------------------------------------------    User  ------------------------------------------------------------------------------------------------------------------    
 DECLARE @UserMaxId INT,@UserId INT,@NumUser INT=1    
  PRINT 'Start User (Account Holder)'    
while @NumUser <= 500   -------- User (Account Holder) Loop    
BEGIN      
SET @UserMaxId=(SELECT MAX(ID) From [User])    
    
INSERT INTO [User](UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,    
PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,FirstName,MiddleName,LastName,OrganisationId,    
[Address],Location,UserCode,Custom1,Custom2,Custom3,Custom4,Custom5,Custom6,Custom7,Custom8,Custom9,Custom10,Custom11,Custom12,UserDeviceId,    
UserDeviceType,SessionId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,IsAdmin,ProgramId,secondaryEmail,genderId,dateOfBirth,    
customInfo,IsMobileRegistered,InvitationStatus)    
VALUES(CONCAT('User-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('User-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),CONCAT('User-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('Us
  
er-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),1,'AQAAAAEAACcQAAAAEDM5iXo1pgIQ9iooLw4JjuYSVEB2qWSpUpxgHgxiGzePUXvpqdco2jJ6IYQjFVLnJA==',    
NULL,NULL,'6575547843',1,0,NULL,0,0,CONCAT('User-FirstName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),NULL,CONCAT('User-LastName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),@OrganisationId,NULL,'30.7093802,76.6952725',    
CONCAT('AHD1000-',@UserMaxId+1),NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NEWID(),1,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0,0,@ProgramId,NULL,    
NULL,NULL,NULL,1,3)    
 SET @UserId=IDENT_CURRENT('[User]')    
    
 ----------------------------------------------------------------------------     User Program    -----------------------------------------------------------------------------------    
    
 INSERT INTO USERPROGRAM(userId,programId,programPackageId,userEmailAddress,isLinkedProgram,linkAccountVerificationCode,    
 verificationCodeValidTill,isVerificationCodeDone,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted)    
 VALUES(@UserId,@ProgramId,NULL,NULL,1,NULL,NULL,1,1,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0)    
    
 --------------------------------------------------------------------------------  User Role  -------------------------------------------------------------------------------------------    
    
 INSERT INTO UserRole(UserId,RoleId) Values(@UserId,1)    
 ------------------------------------------------------------------------------    User Plan --------------------------------------------------------------------------------------------------    
    
   DECLARE @NumUserPlan INT=1,    
   @UserPlanMaxCount INT    
 SET @UserPlanMaxCount=(SELECT COUNT(*)    
 FROM #tempPlanIds)    
  PRINT 'Start User Plan'    
 WHILE(@NumUserPlan <= @UserPlanMaxCount)    
   BEGIN   ---- UserPlan Loop    
      
   DECLARE @PlanIdSelect INT    
      
   SELECT TOP(1) @PlanIdSelect=PlanId FROM #tempPlanIds WHERE ID=@NumUserPlan    
   PRINT CONCAT('------------>>>>>>>>>>>>>>>>>>>    ',@PlanIdSelect)    
  INSERT INTO UserPlans(userId,programPackageId) VALUES(@UserId,@PlanIdSelect)    
    
 SET @NumUserPlan=@NumUserPlan+1    
    
   END  ------ UserPlan Loop    
   PRINT 'End UserPlan'    
     
    
 -------------------------------------------------      User Transaction Info (for Crediting initial balance for Meal Passes and Flex points from ProgramAccounts table   ------------------------------------------------    
    
 CREATE TABLE #tempProgramAccountInitialBalanceContent    
(    
 ID int identity(1,1) not null,    
 ProgramId int NULL,    
 PlanId int NULL,    
 ProgramAccountId int NULL,    
 AccountName NVARCHAR(150) NULL,    
 intialBalanceCount DECIMAL NULL,    
 AccountTypeId int NULL    
)    
    
INSERT INTO #tempProgramAccountInitialBalanceContent(ProgramId,PlanId,ProgramAccountId,AccountName,intialBalanceCount,AccountTypeId)    
 Select     
                                                                        pp.ProgramId,    
                                                                        pp.Id as PlanId,    
                                                                        ppal.ProgramAccountId,    
                                                                        pa.AccountName,    
                                                                        pa.intialBalanceCount,    
                                                                        pa.AccountTypeId    
                                                                        from ProgramPackage pp     
                                                                        INNER JOIN PlanProgramAccountsLinking ppal ON pp.Id=ppal.planId    
                                                                        INNER JOIN ProgramAccounts pa ON pa.Id=ppal.programAccountId    
                                                                        WHERE pp.Id IN (Select PlanId FROM #tempPlanIds) AND pa.AccountTypeId!=3     
                      
    
    
    
       DECLARE @UserTransactionInfoProgramAccountMaxCount INT,@NumProgramAccountUserTransactionInfo INT=1;    
    DECLARE @UserFoundryAdminId INT;    
 SET @UserTransactionInfoProgramAccountMaxCount=(SELECT COUNT(*)    
 FROM #tempProgramAccountInitialBalanceContent)    
  PRINT 'Start UserTransactionInfo'    
 WHILE(@NumProgramAccountUserTransactionInfo <= @UserTransactionInfoProgramAccountMaxCount)    
   BEGIN   ---- UserTransactionInfo Loop    
      
   DECLARE @ProgramAccountIdSelect INT,@UserTransactionInfoAccountTypeId INT,@UTPlanIdSelect INT    
   DECLARE @InitialAmount DECIMAL    
    
   SELECT TOP(1) @ProgramAccountIdSelect=ProgramAccountId,@UserTransactionInfoAccountTypeId=AccountTypeId,@UTPlanIdSelect=PlanId,@InitialAmount=intialBalanceCount FROM #tempProgramAccountInitialBalanceContent WHERE ID=@NumPlanProgramAccountLinking    
       
   SELECT @UserFoundryAdminId=Id from [User] Where Email='foundry@mailinator.com'    
    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@UserTransactionInfoAccountTypeId,@UserId,1,0,@ProgramId,@InitialAmount,GETUTCDATE(),1,4,@ProgramAccountIdSelect,@UserFoundryAdminId,@UserFoundryAdminId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,@UTPlanIdSelect,@UserFoundryAdminId,0)    
    
    
  ---------------------------------------------------------------------------------- Debit Account Transactions for User  ----------------------------------------------------------------------------    
    
    
   DECLARE @DebitUserTransactionCount INT = 1,     
   @DebitUserTransactionMerchantMaxCount INT, @DebitUserTransactionId INT    
     
 SET @DebitUserTransactionMerchantMaxCount=(SELECT COUNT(*)    
 FROM #tempMerchantIds)    
 PRINT @DebitUserTransactionMerchantMaxCount    
  PRINT 'Start Account Debit User Transactions'    
 WHILE(@DebitUserTransactionCount <= @DebitUserTransactionMerchantMaxCount)    
   BEGIN  ---- Debit User Transaction Loop    
      
   DECLARE @MerchantIdDebitUserTransactionSelect INT    
         
   SELECT TOP(1) @MerchantIdDebitUserTransactionSelect=MerchantId FROM #tempMerchantIds WHERE ID=@DebitUserTransactionCount    
    
      
      
   DECLARE @NumMerchantDebitTransactionForAccountDetail INT=1;    
   PRINT 'Start Inner merchant account debit User transaction'    
   WHILE(@NumMerchantDebitTransactionForAccountDetail <= 50)    
   BEGIN  ----  Inner merchant account debit User transaction Loop    
  IF(@NumMerchantDebitTransactionForAccountDetail<=3)    
  BEGIN    
   INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@UserTransactionInfoAccountTypeId,@MerchantIdDebitUserTransactionSelect,1,0,@ProgramId,1,GETUTCDATE(),3,1,    
  @ProgramAccountIdSelect,@UserId,@UserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,@UTPlanIdSelect,@UserId,1)    
   SET @DebitUserTransactionId=IDENT_CURRENT('UserTransactionInfo')    
   END    
   ELSE IF(@NumMerchantDebitTransactionForAccountDetail=4)    
  BEGIN    
   INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@UserTransactionInfoAccountTypeId,@MerchantIdDebitUserTransactionSelect,1,0,@ProgramId,1,GETUTCDATE(),3,1,    
  @ProgramAccountIdSelect,@UserId,@UserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,@UTPlanIdSelect,@UserId,2)    
   SET @DebitUserTransactionId=IDENT_CURRENT('UserTransactionInfo')    
   END    
   ELSE    
  BEGIN    
   INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@UserTransactionInfoAccountTypeId,@MerchantIdDebitUserTransactionSelect,1,0,@ProgramId,1,GETUTCDATE(),3,1,    
  @ProgramAccountIdSelect,@UserId,@UserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,@UTPlanIdSelect,@UserId,0)    
   SET @DebitUserTransactionId=IDENT_CURRENT('UserTransactionInfo')    
   END    
  SET @NumMerchantDebitTransactionForAccountDetail=@NumMerchantDebitTransactionForAccountDetail+1      
    
   END  --   ----  Inner merchant account debit User transaction Loop    
   PRINT 'End Inner merchant account debit User transaction'    
   SET @DebitUserTransactionCount=@DebitUserTransactionCount+1    
   END  ---- Debit user transaction  Loop    
    
PRINT 'End Account Debit user transaction'    
    
    
 SET @NumProgramAccountUserTransactionInfo=@NumProgramAccountUserTransactionInfo+1    
    
   END  ---- UserTransactionInfo Loop     
   PRINT 'End UserTransactionInfo'    
    
    ----------------------------------------------------------------------       Benefactor     ----------------------------------------------------------------------------------------------------------------------------------------------------------    
    
 DECLARE @BenefactorUserId INT,@NumBenefactorUser INT=1    
  PRINT 'Start Benefactor'    
while @NumBenefactorUser <= 20--100    -------- Benefactor Loop    
BEGIN      
    
    
 INSERT INTO [User](UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,    
PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,FirstName,MiddleName,LastName,OrganisationId,    
[Address],Location,UserCode,Custom1,Custom2,Custom3,Custom4,Custom5,Custom6,Custom7,Custom8,Custom9,Custom10,Custom11,Custom12,UserDeviceId,    
UserDeviceType,SessionId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,IsAdmin,ProgramId,secondaryEmail,genderId,dateOfBirth,    
customInfo,IsMobileRegistered,InvitationStatus)    
VALUES(CONCAT('BenefactorData-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser,'@yopmail.com'),UPPER(CONCAT('Benefactor-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser,'@yopmail.com')),CONCAT('Benefactor-',@OrganisationId,'-',@ProgramId,'-'
,  
@NumBenefactorUser,'@yopmail.com'),UPPER(CONCAT('Benefactor-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser,'@yopmail.com')),1,'AQAAAAEAACcQAAAAEDM5iXo1pgIQ9iooLw4JjuYSVEB2qWSpUpxgHgxiGzePUXvpqdco2jJ6IYQjFVLnJA==',NULL,NULL,'6575547843',1,0,NULL,0
,0,CONCAT('Benefactor-FirstName-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser),NULL,CONCAT('Benefactor-LastName-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser),@OrganisationId,NULL,'30.7093802,76.6952725',  NULL,NULL,NULL,NULL,NULL,NULL
,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NEWID(),1,@UserId,GETUTCDATE(),@UserId,GETUTCDATE(),0,0,@ProgramId,NULL,  NULL,NULL,NULL,1,NULL)    
 SET @BenefactorUserId=IDENT_CURRENT('[User]')    
    
    
 --INSERT INTO #tempBenefactorIds(BenefactorId) VALUES(@BenefactorUserId)      
 IF(@NumBenefactorUser%2=0)    
 BEGIN    
 INSERT INTO BenefactorUsersLinking(userId,benefactorId,relationshipId,linkedDateTime,canViewTransaction,isActive,createdBy,createdDate,modifiedBy,    
 modifiedDate,isDeleted,IsRequestAccepted,IsInvitationSent)    
 VALUES(@UserId,@BenefactorUserId,3,GETUTCDATE(),1,1,@UserId,GETUTCDATE(),@UserId,GETUTCDATE(),0,1,1)    
    
 END    
 ELSE    
 BEGIN    
 INSERT INTO BenefactorUsersLinking(userId,benefactorId,relationshipId,linkedDateTime,canViewTransaction,isActive,createdBy,createdDate,modifiedBy,    
 modifiedDate,isDeleted,IsRequestAccepted,IsInvitationSent)    
 VALUES(@UserId,@BenefactorUserId,4,GETUTCDATE(),1,1,@UserId,GETUTCDATE(),@UserId,GETUTCDATE(),0,1,1)    
 END    
    
 --------------------------------------------------------------------  UserTransactionInfo Benefactor Credit  -------------------------------------------------------------------------------------------------------    
    
 DECLARE @NumBenefactorUserTransaction INT=1    
  PRINT 'Start Benefactor Credit User Transaction'    
while @NumBenefactorUserTransaction <= 40    -------- Benefactor Credit User Transaction Loop    
BEGIN     
 IF(@NumBenefactorUserTransaction<=2)    
 BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(3,@UserId,1,0,@ProgramId,2000,GETUTCDATE(),1,2,NULL,@BenefactorUserId,@BenefactorUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@BenefactorUserId,1)    
  END    
  ELSE IF(@NumBenefactorUserTransaction<=2)    
  BEGIN    
    INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(3,@UserId,1,0,@ProgramId,2000,GETUTCDATE(),1,2,NULL,@BenefactorUserId,@BenefactorUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@BenefactorUserId,0)    
  END    
  ELSE    
    BEGIN    
    INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(3,@UserId,1,0,@ProgramId,2000,GETUTCDATE(),1,2,NULL,@BenefactorUserId,@BenefactorUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@BenefactorUserId,2)    
  END    
    ---------------------------------------------------------------------------------- Debit Benefactor Transactions for User  ----------------------------------------------------------------------------    
    
    
   DECLARE @DebitBenefactorUserTransactionCount INT = 1,     
   @DebitBenefactorUserTransactionMerchantMaxCount INT    
     
 SET @DebitBenefactorUserTransactionMerchantMaxCount=(SELECT COUNT(*)    
 FROM #tempMerchantIds)    
 PRINT @DebitBenefactorUserTransactionMerchantMaxCount    
  PRINT 'Start Benefactor Debit User Transactions'    
 WHILE(@DebitBenefactorUserTransactionCount <= @DebitBenefactorUserTransactionMerchantMaxCount)    
   BEGIN  ---- Debit Benefactor User Transaction Loop    
      
   DECLARE @MerchantIdBenefactorDebitUserTransactionSelect INT    
         
   SELECT TOP(1) @MerchantIdBenefactorDebitUserTransactionSelect=MerchantId FROM #tempMerchantIds WHERE ID=@DebitBenefactorUserTransactionCount    
    
      
      
   DECLARE @NumMerchantBenefactorDebitTransactionForAccountDetail INT=1;    
   PRINT 'Start Inner merchant benefactor debit User transaction'    
   WHILE(@NumMerchantBenefactorDebitTransactionForAccountDetail <= 50)    
   BEGIN  ----  Inner merchant account debit User transaction Loop    
  IF(@NumMerchantBenefactorDebitTransactionForAccountDetail<=3)    
  BEGIN    
   INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(3,@MerchantIdDebitUserTransactionSelect,1,0,@ProgramId,10,GETUTCDATE(),3,1,    
  NULL,@UserId,@UserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@UserId,1)    
  END    
  ELSE IF(@NumMerchantBenefactorDebitTransactionForAccountDetail=4)    
  BEGIN    
   INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(3,@MerchantIdDebitUserTransactionSelect,1,0,@ProgramId,10,GETUTCDATE(),3,1,    
  NULL,@UserId,@UserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@UserId,0)    
  END    
  ELSE    
    BEGIN    
   INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(3,@MerchantIdDebitUserTransactionSelect,1,0,@ProgramId,10,GETUTCDATE(),3,1,    
  NULL,@UserId,@UserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@UserId,2)    
  END    
  SET @NumMerchantBenefactorDebitTransactionForAccountDetail=@NumMerchantBenefactorDebitTransactionForAccountDetail+1      
    
   END  --   ----  Inner merchant account debit User transaction Loop    
   PRINT 'End Inner merchant benefactor debit User transaction'    
   SET @DebitBenefactorUserTransactionCount=@DebitBenefactorUserTransactionCount+1    
   END  ---- Debit Benefactor user transaction  Loop    
    
PRINT 'End Benefactor Debit user transaction'    
    
    
    
  SET @NumBenefactorUserTransaction=@NumBenefactorUserTransaction+1;    
  END ------ Benefactor Credit User Transaction Loop    
  PRINT 'END Benefactor Credit User Transaction'    
 SET @NumBenefactorUser=@NumBenefactorUser+1;    
    
END ---- End Benefactor Loop    
  PRINT 'END Benefactor'    
    
  -----------------------------------------------------------------------------     Admin User     ------------------------------------------------------------------------------------------------    
  DECLARE @AdminUserMaxId INT,@AdminUserId INT,@NumAdminUser INT=1    
  PRINT 'Start Admin User'    
while @NumAdminUser <= 60    -------- Admin User Loop    
BEGIN      
IF(@NumAdminUser=1 OR @NumAdminUser=2)  --- Organisation Admin Insert    
BEGIN    
SET @AdminUserMaxId=(SELECT MAX(ID) From [User])    
  INSERT INTO [User](UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,    
PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,FirstName,MiddleName,LastName,OrganisationId,    
[Address],Location,UserCode,Custom1,Custom2,Custom3,Custom4,Custom5,Custom6,Custom7,Custom8,Custom9,Custom10,Custom11,Custom12,UserDeviceId,    
UserDeviceType,SessionId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,IsAdmin,ProgramId,secondaryEmail,genderId,dateOfBirth,    
customInfo,IsMobileRegistered,InvitationStatus)    
VALUES(CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),1,'AQAAAAEAACcQAAAAEDM5iXo1pgIQ9iooLw4JjuYSVEB2qWSpUpxgHgxiGzePUXvpqdco2jJ6IYQjFVLnJA==',   
NULL,NULL,'6575547843',1,0,NULL,0,0,CONCAT('AdminUser-FirstName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),NULL,CONCAT('AdminUser-LastName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),@OrganisationId,NULL,'30.7093802,76.6952725',    
NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NEWID(),1,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0,1,@ProgramId,NULL,    
NULL,NULL,NULL,1,3)    
 SET @AdminUserId=IDENT_CURRENT('[User]')    
    
 --------------------------------------------------------------------------------Admin User Role  -------------------------------------------------------------------------------------------    
    
 INSERT INTO UserRole(UserId,RoleId) Values(@AdminUserId,3)    
    
 --------------------------------------------------------------------  UserTransactionInfo Admin Credit  -------------------------------------------------------------------------------------------------------    
    
 DECLARE @NumOrgAdminUserTransaction INT=1    
  PRINT 'Start Organisation Admin Credit User Transaction'    
while @NumOrgAdminUserTransaction <= 40--100    -------- Admin Credit User Transaction Loop    
BEGIN     
IF(@NumOrgAdminUserTransaction<=2)    
BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@NumOrgAdminUserTransaction,@UserId,1,0,@ProgramId,30,GETUTCDATE(),1,5,NULL,@AdminUserId,@AdminUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@AdminUserId,1)    
  END    
  ELSE IF(@NumOrgAdminUserTransaction=3)    
BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@NumOrgAdminUserTransaction,@UserId,1,0,@ProgramId,30,GETUTCDATE(),1,5,NULL,@AdminUserId,@AdminUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@AdminUserId,0)    
  END    
  ELSE     
  BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(3,@UserId,1,0,@ProgramId,30,GETUTCDATE(),1,5,NULL,@AdminUserId,@AdminUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@AdminUserId,2)    
  END    
    
      
    
  SET @NumOrgAdminUserTransaction=@NumOrgAdminUserTransaction+1;    
  END ------ Admin Credit User Transaction Loop    
  PRINT 'END Organisation Admin Credit User Transaction'    
    
 END    
 ELSE IF(@NumAdminUser=3 OR @NumAdminUser=4)--- Program Admin Insert    
BEGIN    
SET @AdminUserMaxId=(SELECT MAX(ID) From [User])    
  INSERT INTO [User](UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,    
PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,FirstName,MiddleName,LastName,OrganisationId,    
[Address],Location,UserCode,Custom1,Custom2,Custom3,Custom4,Custom5,Custom6,Custom7,Custom8,Custom9,Custom10,Custom11,Custom12,UserDeviceId,    
UserDeviceType,SessionId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,IsAdmin,ProgramId,secondaryEmail,genderId,dateOfBirth,    
customInfo,IsMobileRegistered,InvitationStatus)    
VALUES(CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),1,'AQAAAAEAACcQAAAAEDM5iXo1pgIQ9iooLw4JjuYSVEB2qWSpUpxgHgxiGzePUXvpqdco2jJ6IYQjFVLnJA==',    
NULL,NULL,'6575547843',1,0,NULL,0,0,CONCAT('AdminUser-FirstName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),NULL,CONCAT('AdminUser-LastName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),@OrganisationId,NULL,'30.7093802,76.6952725',    
NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NEWID(),1,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0,1,@ProgramId,NULL,    
NULL,NULL,NULL,1,3)    
 SET @AdminUserId=IDENT_CURRENT('[User]')    
    
 --------------------------------------------------------------------------------Admin User Role  -------------------------------------------------------------------------------------------    
    
 INSERT INTO UserRole(UserId,RoleId) Values(@AdminUserId,8)    
 --------------------------------------------------------------------  UserTransactionInfo Admin Credit  -------------------------------------------------------------------------------------------------------    
    
 DECLARE @NumProgramAdminUserTransaction INT=1    
  PRINT 'Start Program Admin Credit User Transaction'    
while @NumProgramAdminUserTransaction <= 40--100    -------- Admin Credit User Transaction Loop    
BEGIN     
IF(@NumProgramAdminUserTransaction<=2)    
BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@NumProgramAdminUserTransaction,@UserId,1,0,@ProgramId,30,GETUTCDATE(),1,7,NULL,@AdminUserId,@AdminUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@AdminUserId,1)    
  END    
  ELSE IF(@NumProgramAdminUserTransaction=3)    
  BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@NumProgramAdminUserTransaction,@UserId,1,0,@ProgramId,30,GETUTCDATE(),1,7,NULL,@AdminUserId,@AdminUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@AdminUserId,2)    
  END    
  ELSE     
  BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(3,@UserId,1,0,@ProgramId,30,GETUTCDATE(),1,7,NULL,@AdminUserId,@AdminUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@AdminUserId,0)    
  END    
  SET @NumProgramAdminUserTransaction=@NumProgramAdminUserTransaction+1;    
  END ------ Admin Credit User Transaction Loop    
  PRINT 'END Program Admin Credit User Transaction'    
    
    
 END    
 ELSE --- Merchant Admin Insert    
 BEGIN    
 SET @AdminUserMaxId=(SELECT MAX(ID) From [User])    
  INSERT INTO [User](UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,    
PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,FirstName,MiddleName,LastName,OrganisationId,    
[Address],Location,UserCode,Custom1,Custom2,Custom3,Custom4,Custom5,Custom6,Custom7,Custom8,Custom9,Custom10,Custom11,Custom12,UserDeviceId,    
UserDeviceType,SessionId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,IsAdmin,ProgramId,secondaryEmail,genderId,dateOfBirth,    
customInfo,IsMobileRegistered,InvitationStatus)    
VALUES(CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('AdminUser-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),1,'AQAAAAEAACcQAAAAEDM5iXo1pgIQ9iooLw4JjuYSVEB2qWSpUpxgHgxiGzePUXvpqdco2jJ6IYQjFVLnJA==',    
NULL,NULL,'6575547843',1,0,NULL,0,0,CONCAT('AdminUser-FirstName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),NULL,CONCAT('AdminUser-LastName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),@OrganisationId,NULL,'30.7093802,76.6952725',    
NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NEWID(),1,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0,1,@ProgramId,NULL,    
NULL,NULL,NULL,1,3)    
 SET @AdminUserId=IDENT_CURRENT('[User]')    
    
 --------------------------------------------------------------------------------Admin User Role  -------------------------------------------------------------------------------------------    
    
 INSERT INTO UserRole(UserId,RoleId) Values(@AdminUserId,6)    
    
 --------------------------------------------------------------------  UserTransactionInfo Admin Credit  -------------------------------------------------------------------------------------------------------    
    
 DECLARE @NumMerchantAdminUserTransaction INT=1    
  PRINT 'Start Merchant Admin Credit User Transaction'    
while @NumMerchantAdminUserTransaction <= 4--100    -------- Admin Credit User Transaction Loop    
BEGIN     
 IF(@NumMerchantAdminUserTransaction<=2)    
 BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@NumMerchantAdminUserTransaction,@UserId,1,0,@ProgramId,30,GETUTCDATE(),1,6,NULL,@AdminUserId,@AdminUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@AdminUserId,2)    
  END    
  ELSE IF(@NumMerchantAdminUserTransaction=3)    
   BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(@NumMerchantAdminUserTransaction,@UserId,1,0,@ProgramId,30,GETUTCDATE(),1,6,NULL,@AdminUserId,@AdminUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@AdminUserId,1)    
  END    
  ELSE    
   BEGIN    
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(3,@UserId,1,0,@ProgramId,30,GETUTCDATE(),1,6,NULL,@AdminUserId,@AdminUserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@AdminUserId,0)    
  END    
  SET @NumMerchantAdminUserTransaction=@NumMerchantAdminUserTransaction+1;    
  END ------ Admin Credit User Transaction Loop    
  PRINT 'END Merchant Admin Credit User Transaction'    
    
 ---------------------------------------------------------------------------------- Debit Admin Transactions for User  ----------------------------------------------------------------------------    
    
    
   DECLARE @DebitAdminUserTransactionCount INT = 1,     
   @DebitAdminUserTransactionMerchantMaxCount INT    
     
 SET @DebitAdminUserTransactionMerchantMaxCount=(SELECT COUNT(*)    
 FROM #tempMerchantIds)    
     
  PRINT 'Start Admin Debit User Transactions'    
 WHILE(@DebitAdminUserTransactionCount <= @DebitAdminUserTransactionMerchantMaxCount)    
   BEGIN  ---- Debit Admin User Transaction Loop    
      
   DECLARE @MerchantIdAdminDebitUserTransactionSelect INT    
         
   SELECT TOP(1) @MerchantIdAdminDebitUserTransactionSelect=MerchantId FROM #tempMerchantIds WHERE ID=@DebitAdminUserTransactionCount    
    
      
      
   DECLARE @NumMerchantAdminDebitTransactionForAccountDetail INT=1;    
   PRINT 'Start Inner merchant admin debit User transaction'    
   WHILE(@NumMerchantAdminDebitTransactionForAccountDetail <= 50)    
   BEGIN  ----  Inner merchant account debit User transaction Loop    
  IF(@NumMerchantAdminDebitTransactionForAccountDetail<=3)    
  BEGIN    
   INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(1,@MerchantIdAdminDebitUserTransactionSelect,1,0,@ProgramId,1,GETUTCDATE(),3,1,    
  NULL,@UserId,@UserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@UserId,2)    
      
  END    
  ELSE  IF(@NumMerchantAdminDebitTransactionForAccountDetail=4)    
  BEGIN    
   INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(1,@MerchantIdAdminDebitUserTransactionSelect,1,0,@ProgramId,1,GETUTCDATE(),3,1,    
  NULL,@UserId,@UserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@UserId,0)    
  END    
  ELSE     
  BEGIN     
  INSERT INTO UserTransactionInfo(accountTypeId,creditUserId,isActive,isDeleted,programId,transactionAmount,transactionDate,CreditTransactionUserType,DebitTransactionUserType,    
  programAccountId,createdBy,modifiedBy,createdDate,modifiedDate,organisationId,planId,debitUserId,transactionStatus)    
  VALUES(1,@MerchantIdAdminDebitUserTransactionSelect,1,0,@ProgramId,1,GETUTCDATE(),3,1,    
  NULL,@UserId,@UserId,GETUTCDATE(),GETUTCDATE(),@OrganisationId,NULL,@UserId,3)    
  END    
  SET @NumMerchantAdminDebitTransactionForAccountDetail=@NumMerchantAdminDebitTransactionForAccountDetail+1      
    
   END  --   ----  Inner merchant admin debit User transaction Loop    
   PRINT 'End Inner merchant admin debit User transaction'    
   SET @DebitAdminUserTransactionCount=@DebitAdminUserTransactionCount+1    
   END  ---- Debit admin user transaction  Loop    
    
PRINT 'End admin Debit user transaction'    
 END    
    
    
    
 SET @NumAdminUser=@NumAdminUser+1    
 END  -------- Admin User Loop    
    
     
    
 SET @NumUser=@NumUser+1;    
 DROP TABLE #tempProgramAccountInitialBalanceContent    
END ---- -------- User (Account Holder) Loop    
   PRINT 'End User (Account Holder) '    
    
    
  -- DELETE FROM #tempMerchantIds    
      
   -- DELETE FROM #tempPlanIds    
      
 --  DELETE FROM  #tempProgramAccountIds    
        
 --  DELETE FROM #tempBenefactorIds    
        
   -- DELETE FROM #tempProgramAccountInitialBalanceContent    
      
    
SET @NumProgram=@NumProgram+1    
DROP TABLE #tempPlanIds    
DROP TABLE #tempMerchantIds    
DROP TABLE #tempProgramAccountIds    
  END ----- Program Loop  PRINT 'End Program'    
SET @NumOrganisation=@NumOrganisation+1    
     END   ---- Organisation Loop    
  PRINT 'End Organisation'    
END    
    
    
    
    
    
    
    
    
    
GO
/****** Object:  StoredProcedure [dbo].[InsertDummyDatanew]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*********************************************************************************************************************************************************************  
  
SP Name: InsertDummyData  
Test Executable Statement: Exec InsertDummyDataNew  
  
**********************************************************************************************************************************************************************/  
  
CREATE PROCEDURE [dbo].[InsertDummyDatanew] --exec [InsertDummyDatanew]  
AS  
BEGIN  
	DECLARE @NumOrganisation INT,@NumProgram INT;   
	DECLARE @ProgramAutoId NVARCHAR(30), @MerchantIdsConcat NVARCHAR(50)      
    
	--BEGIN TRY
--------------------------------------------------------------------------  Organisation -------------------------------------------------------------------------------------------  
  
	SET @NumOrganisation=1;  
	PRINT 'Start Organisation'  
	WHILE @NumOrganisation <= 50   
		BEGIN    ------ Organisation Loop  
			INSERT INTO Organisation(name,addressLine1,addressLine2,location,organisationType,emailAddress,contactNumber,isMaster,isActive,createdBy,createdDate,modifiedBy,modifiedDate,  
			isDeleted,maxCapacity,[description],websiteURL,getHelpEmail,getHelpPhone1,getHelpPhone2,facebookURL,twitterURL,skypeHandle,ContactName,ContactTitle,  
			MerchantId,country,[state],city,zip,isMapVisible,isClosed,businessTypeId,OrganisationSubTitle,dwellTime)   
			VALUES(CONCAT('OrganisationDummyNew-',@NumOrganisation),'#245, Toronto, Canada',null,null,2,CONCAT('adminnew',@NumOrganisation,'@aprio.com'),'(123) 345 6787',0,1,null,GetUTCDATE(),NULL,GETUTCDATE(),  
			0,NULL,'Aprip University Lorem ipsum dolor sit amet',NULL,NULL,NULL,NULL,NULL,NULL,NULL,CONCAT('Admin Aprio',@NumOrganisation),'Mr.',  
			NULL,'Canada','Toronto','Toronto','647833',NULL,NULL,NULL,'APU',NULL)  
  
			DECLARE @OrganisationId INT;  
			SET @OrganisationId=IDENT_CURRENT('Organisation')

			--------------------------------------------------------------------------  Program ---------------------------------------------------------------------------------------------------------------------------  
			DECLARE @ProgramId INT,@ProgramMaxId INT  
			SET @NumProgram=1  
			PRINT 'Start Program'  
			WHILE @NumProgram <= 50   
				BEGIN    ------ Program Loop 
					
					Create Table #tempMerchantIds  
					(  
						ID int identity(1,1) not null,  
						MerchantId int not null  
					)  
  
					CREATE TABLE #tempPlanIds  
					(  
						ID int identity(1,1) not null,  
						PlanId int not null  
					)  
  
					CREATE TABLE #tempProgramAccountIds  
					(  
						ID int identity(1,1) not null,  
						ProgramAccountId int not null  
					) 

					SET @ProgramMaxId=(SELECT MAX(ID) From Program)  
  
					INSERT INTO Program(name,[description],organisationId,logoPath,colorCode,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,timeZone,ProgramCodeId,ProgramTypeId,website,[address],country,[state],city,zipcode,customName,customInputMask,customErrorMessaging,customInstructions,programCustomFields,AccountHolderGroups,AccountHolderUniqueId)  
					Values(CONCAT('ProgramDummyNew-',@OrganisationId,'-',@NumProgram),'Lorem ipsum dolor sit amet',NULL,NULL,NULL,1,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0,'Eastern Standard Time',CONCAT('P1000-',@ProgramMaxId+1),3,NULL,'Edmonton International Airport (YEG), A
irport Road, Edmonton International Airport, AB, Canada','Canada','Alberta',NULL,'T9E 0V3','UserID','XXX-XXX-XXXX','Please enter  user id.',NULL,'[]','Test',CONCAT('AC1000-',@ProgramMaxId+1))    
					SET @ProgramId=IDENT_CURRENT('Program')

					----------------------------------------------------------------------- Organisation Program (Organisation) ---------------------------------------------------------------------------------------------------  
  
					INSERT INTO OrganisationProgram(organisationId, programId,CreatedDate,ModifiedDate,IsActive,IsDeleted)  
					VALUES(@OrganisationId,@ProgramId,GETUTCDATE(),GETUTCDATE(),1,0)  

					------------------------------------------------------------------------

					---------------------------------------------------------------------------------------   Merchant    -------------------------------------------------------------------------------------------  
  
					DECLARE @NumMerchant INT=1;  
					PRINT 'Start Merchant'  
					WHILE @NumMerchant <= 50   
						BEGIN   ------ Merchant Loop 
							DECLARE @MerchantMaxId INT, @MerchantId INT  
							SET @MerchantMaxId=(SELECT MAX(ID) From Program)  

							INSERT into Organisation(name,addressLine1,addressLine2,location,organisationType,emailAddress,contactNumber,isMaster,isActive,createdBy,createdDate,modifiedBy,modifiedDate,  
							isDeleted,maxCapacity,[description],websiteURL,getHelpEmail,getHelpPhone1,getHelpPhone2,facebookURL,twitterURL,skypeHandle,ContactName,ContactTitle,  
							MerchantId,country,[state],city,zip,isMapVisible,isClosed,businessTypeId,OrganisationSubTitle,dwellTime)   
							Values(CONCAT('MerchantDummyNew-',@OrganisationId,'-',@ProgramId,'-',@NumMerchant),'#245, Toronto, Canada',null,'-125.26824210000001, 50.2057635',3,CONCAT('merchant',@NumMerchant,'@aprio.com'),'(123) 345 6787',0,1,null,GetUTCDATE(),NULL,GETUTCDATE(),  

							0,NULL,'Aprip University Lorem ipsum dolor sit amet',NULL,NULL,NULL,NULL,NULL,NULL,NULL,CONCAT('merchant title',@NumMerchant),'Mr.',  
							CONCAT('M1000-',@MerchantMaxId+1),'Canada','Toronto','Toronto','647833',NULL,NULL,NULL,'APU',15)   
  
  
							SET @MerchantId=IDENT_CURRENT('Organisation')   

							INSERT INTO #tempMerchantIds(MerchantId)VALUES(@MerchantId)  

							INSERT INTO OrganisationProgram(organisationId, programId,CreatedDate,ModifiedDate,IsActive,IsDeleted)  
							VALUES(@MerchantId,@ProgramId,GETUTCDATE(),GETUTCDATE(),1,0)  

							----Merchant Terminal
							DECLARE @MerchantTerminal INT=1;
							WHILE @MerchantTerminal <= 4
								BEGIN
									Insert into merchantterminal values('T'+Cast(@MerchantTerminal AS varchar(10)),'Terminal ' + Cast(@MerchantTerminal AS varchar(10)),1,@MerchantId,'')
									SET @MerchantTerminal = @MerchantTerminal + 1;  
								END
							---------------------

							--Select * from #tempMerchantIds 

							--------------------------------------------------------------------------------------- Program Account Linking  ----------------------------------------------------------------------  
							DECLARE @NumProgramAccountLinkingType INT=1;  
							PRINT 'Start ProgramAccountLinking'  
							WHILE @NumProgramAccountLinkingType <= 3   
								BEGIN   ------ Program Account Linking Loop  
  
									DECLARE @ProgramAccountLinkId INT
									INSERT INTO ProgramAccountLinking(programId,accountTypeId) VALUES(@ProgramId,@NumProgramAccountLinkingType)  
  
									SET @ProgramAccountLinkId=IDENT_CURRENT('ProgramAccountLinking')   
  
									INSERT INTO ProgramMerchantAccountType(organisationId,programAccountLinkingId) VALUES(@MerchantId,@ProgramAccountLinkId)  
  
   
									SET @NumProgramAccountLinkingType = @NumProgramAccountLinkingType + 1;  
  
  
								END   ------ Program Account Linking Loop  
							PRINT 'End ProgramAccountLinking'

							SET @NumMerchant=@NumMerchant+1  
						END   ----- Merchant Loop  
  
					PRINT 'End Merchant'

					----------------------------------------------------------------------------------  Program Accounts  --------------------------------------------------------------------------------  
					DECLARE @ProgramAccountMaxId INT,@ProgramAccountId INT,@NumProgramAccount INT=1;  
					PRINT 'Start ProgramAccounts'  
					WHILE @NumProgramAccount <= 3   
						BEGIN   ------ Program Accounts Loop

							SET @ProgramAccountMaxId=(SELECT MAX(ID) From ProgramAccounts)  
							IF(@NumProgramAccount=1)  
								BEGIN  
									INSERT INTO ProgramAccounts(accountName,accountTypeId,programId,passType,intialBalanceCount,resetPeriodType,resetDay,resetTime,  
									maxPassUsage,isPassExchangeEnabled,exchangePassValue,exchangeResetPeriodType,exchangeResetDay,exchangeResetTime,isRollOver,flexEndDate,isActive,  
									createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,ProgramAccountId,maxPassPerWeek,maxPassPerMonth,resetDateOfMonth,flexMaxSpendPerDay,  
									flexMaxSpendPerWeek,flexMaxSpendPerMonth,exchangeResetDateOfMonth,vplMaxBalance,vplMaxAddValueAmount,vplMaxNumberOfTransaction)  
									VALUES(CONCAT('AccountDummyNew-',@OrganisationId,'-',@ProgramId,'-',@NumProgramAccount),@NumProgramAccount,@ProgramId,5,300,2,6,NULL,2,1,1,1,NULL,NULL,0,NULL,1,NULL,GetUTCDATE(),NULL,GETUTCDATE(),0, CONCAT('PA1000-',@ProgramAccountMaxId+1),  
									NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL)  
								END  
							ELSE IF(@NumProgramAccount=2)
								BEGIN  
									INSERT INTO ProgramAccounts(accountName,accountTypeId,programId,passType,intialBalanceCount,resetPeriodType,resetDay,resetTime,  
									maxPassUsage,isPassExchangeEnabled,exchangePassValue,exchangeResetPeriodType,exchangeResetDay,exchangeResetTime,isRollOver,flexEndDate,isActive,  
									createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,ProgramAccountId,maxPassPerWeek,maxPassPerMonth,resetDateOfMonth,flexMaxSpendPerDay,  
									flexMaxSpendPerWeek,flexMaxSpendPerMonth,exchangeResetDateOfMonth,vplMaxBalance,vplMaxAddValueAmount,vplMaxNumberOfTransaction)  
									VALUES(CONCAT('AccountDummyNew-',@OrganisationId,'-',@ProgramId,'-',@NumProgramAccount),@NumProgramAccount,@ProgramId,5,450,2,6,NULL,2,1,1,1,NULL,NULL,0,NULL,1,NULL,GetUTCDATE(),NULL,GETUTCDATE(),0, CONCAT('PA1000-',@ProgramAccountMaxId+1),  
									NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL)  
							END  
							ELSE  
								BEGIN  
									INSERT INTO ProgramAccounts(accountName,accountTypeId,programId,passType,intialBalanceCount,resetPeriodType,resetDay,resetTime,  
									maxPassUsage,isPassExchangeEnabled,exchangePassValue,exchangeResetPeriodType,exchangeResetDay,exchangeResetTime,isRollOver,flexEndDate,isActive,  
									createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,ProgramAccountId,maxPassPerWeek,maxPassPerMonth,resetDateOfMonth,flexMaxSpendPerDay,  
									flexMaxSpendPerWeek,flexMaxSpendPerMonth,exchangeResetDateOfMonth,vplMaxBalance,vplMaxAddValueAmount,vplMaxNumberOfTransaction)  
									VALUES(CONCAT('AccountDummyNew-',@OrganisationId,'-',@ProgramId,'-',@NumProgramAccount),@NumProgramAccount,@ProgramId,NULL,NULL,NULL,NULL,NULL,2,1,1,1,NULL,NULL,0,NULL,1,NULL,GetUTCDATE(),NULL,GETUTCDATE(),0, CONCAT('PA1000-',@ProgramAccountMaxId+1),
  
									NULL,NULL,NULL,NULL,NULL,NULL,NULL,12.00,12.00,12)  
								END  
							SET @ProgramAccountId=IDENT_CURRENT('ProgramAccounts')

							--------------------------------------------------------------------------  AccountMerchantRules -------------------------------------------------------------------------------  
							DECLARE @AccountMerchantRuleMerchantCount INT = 1,   
							@AccountMerchantRuleMerchantMaxCount INT, @AccountMerchantRuleId INT  
							-- Select * from #tempMerchantIds  
							SET @AccountMerchantRuleMerchantMaxCount=(SELECT COUNT(*)  
							FROM #tempMerchantIds)  
							PRINT @AccountMerchantRuleMerchantMaxCount  
							PRINT 'Start AccountMerchantRules'  
							WHILE(@AccountMerchantRuleMerchantCount <= @AccountMerchantRuleMerchantMaxCount)  
								BEGIN  ---- Account Merchant Rules Loop 
									DECLARE @MerchantIdSelect INT  
       
									SELECT TOP(1) @MerchantIdSelect=MerchantId FROM #tempMerchantIds WHERE ID=@AccountMerchantRuleMerchantCount  
  
									INSERT INTO AccountMerchantRules(programAccountID,merchantId,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,accountTypeId)  
									VALUES(@ProgramAccountId,@MerchantIdSelect,1,NULL,GETUTCDATE(),NULL,GetUTCDATE(),0,@NumProgramAccountLinkingType)  
  
									SET @AccountMerchantRuleId=IDENT_CURRENT('AccountMerchantRules')  
									DECLARE @NumAccountMerchantRuleDetail INT=1;  
									WHILE(@NumAccountMerchantRuleDetail <= 4)  
										BEGIN  ----  Account Merchant Rules Detail Loop 
											INSERT INTO AccountMerchantRulesDetail(accountMerchantRuleId,mealPeriod,maxPassUsage,minPassValue,maxPassValue,transactionLimit)  
											VALUES(@AccountMerchantRuleId,@NumAccountMerchantRuleDetail,1,0,1,NULL)  
  
											SET @NumAccountMerchantRuleDetail=@NumAccountMerchantRuleDetail+1
										END
									PRINT 'End AccountMerchantRulesDetail'  
									SET @AccountMerchantRuleMerchantCount=@AccountMerchantRuleMerchantCount+1 
								END
								PRINT 'End AccountMerchantRules'  
  
  
								INSERT INTO #tempProgramAccountIds(ProgramAccountId)VALUES(@ProgramAccountId)  
  
								SET @NumProgramAccount=@NumProgramAccount+1 
						END
						PRINT 'End ProgramAccounts'
						
						-----------------------------------------------------------------------------------  ProgramPackage (Plans)  ------------------------------------------------------------------------------------------------------------  
						DECLARE @PlanMaxId INT,@PlanId INT,@NumPlans INT=1  
						WHILE @NumPlans <= 5   
							BEGIN    
								SET @PlanMaxId=(SELECT MAX(ID) From ProgramPackage)  
								INSERT INTO ProgramPackage(name,programId,noOfMealPasses,noOfFlexPoints,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,startDate,  
								endDate,startTime,endTime,[description],clientId,planId,isActive)  
								VALUES(CONCAT('PlanDummyNew-',@OrganisationId,'-',@ProgramId,'-',@NumPlans),@ProgramId,23,23,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0,'2019-09-06 10:57:10.130',  
								'2019-10-06 10:57:10.130','10:45:00.0000000','23:45:00.0000000','Lorem ipsum dolor sit amet',CONCAT('P1000-',@PlanMaxId+1),NULL,1)  
  
								SET @PlanId=IDENT_CURRENT('ProgramPackage')  
  
								DECLARE @NumPlanProgramAccountLinking INT=1,  
								@PlanProgramAccountLinkingtMaxCount INT  
								SET @PlanProgramAccountLinkingtMaxCount=(SELECT COUNT(*)  
								FROM #tempProgramAccountIds)  
								PRINT 'Start PlanProgramAccountLinking'  
								WHILE(@NumPlanProgramAccountLinking <= @PlanProgramAccountLinkingtMaxCount)  
									BEGIN   ---- PlanProgramAccountLinking Loop  
    
										DECLARE @AccountIdSelect INT  
     
  
										SELECT TOP(1) @AccountIdSelect=ProgramAccountId FROM #tempProgramAccountIds WHERE ID=@NumPlanProgramAccountLinking  
  
										INSERT INTO PlanProgramAccountsLinking(planId,programAccountId) VALUES(@PlanId,@AccountIdSelect)  
  
										SET @NumPlanProgramAccountLinking=@NumPlanProgramAccountLinking+1  
  
									END  ---- PlanProgramAccountLinking Loop   
								PRINT 'End PlanProgramAccountLinking'  
								Print CONCAT('Plan ID: ',@PlanId)  
								INSERT INTO #tempPlanIds(PlanId) VALUES(@PlanId)     
  
								SET @NumPlans=@NumPlans+1;  
							END 
						--------------------------------------------------------------------------------    User  ------------------------------------------------------------------------------------------------------------------  
						DECLARE @UserMaxId INT,@UserId INT,@NumUser INT=1  
						PRINT 'Start User (Account Holder)'  
						while @NumUser <= 100   -------- User (Account Holder) Loop  
						BEGIN    
							SET @UserMaxId=(SELECT MAX(ID) From [User])  
  
							INSERT INTO [User](UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,  
							PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,FirstName,MiddleName,LastName,OrganisationId,  
							[Address],Location,UserCode,Custom1,Custom2,Custom3,Custom4,Custom5,Custom6,Custom7,Custom8,Custom9,Custom10,Custom11,Custom12,UserDeviceId,  
							UserDeviceType,SessionId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,IsAdmin,ProgramId,secondaryEmail,genderId,dateOfBirth,  
							customInfo,IsMobileRegistered,InvitationStatus)  
							VALUES(CONCAT('UserDummyNew-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('User-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),CONCAT('User-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com'),UPPER(CONCAT('Us
							er-',@OrganisationId,'-',@ProgramId,'-',@NumUser,'@yopmail.com')),1,'AQAAAAEAACcQAAAAEDM5iXo1pgIQ9iooLw4JjuYSVEB2qWSpUpxgHgxiGzePUXvpqdco2jJ6IYQjFVLnJA==',  
							NULL,NULL,'6575547843',1,0,NULL,0,0,CONCAT('User-FirstName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),NULL,CONCAT('User-LastName-',@OrganisationId,'-',@ProgramId,'-',@NumUser),@OrganisationId,NULL,'30.7093802,76.6952725',  
							CONCAT('AHD1000-',@UserMaxId+1),NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NEWID(),1,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0,0,@ProgramId,NULL,  
							NULL,NULL,NULL,1,3)  
							SET @UserId=IDENT_CURRENT('[User]')  
  
							----------------------------------------------------------------------------     User Program    -----------------------------------------------------------------------------------  
  
							INSERT INTO USERPROGRAM(userId,programId,programPackageId,userEmailAddress,isLinkedProgram,linkAccountVerificationCode,  
							verificationCodeValidTill,isVerificationCodeDone,isActive,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted)  
							VALUES(@UserId,@ProgramId,NULL,NULL,1,NULL,NULL,1,1,NULL,GETUTCDATE(),NULL,GETUTCDATE(),0)  
  
							--------------------------------------------------------------------------------  User Role  -------------------------------------------------------------------------------------------  
  
							INSERT INTO UserRole(UserId,RoleId) Values(@UserId,1)
							
							------------------------------------------------------------------------------    User Plan --------------------------------------------------------------------------------------------------  
  
							DECLARE @NumUserPlan INT=1,  
							@UserPlanMaxCount INT  
							SET @UserPlanMaxCount=(SELECT COUNT(*)  
							FROM #tempPlanIds)  
							PRINT 'Start User Plan'  
							WHILE(@NumUserPlan <= @UserPlanMaxCount)  
								BEGIN   ---- UserPlan Loop  
    
									DECLARE @PlanIdSelect INT  
    
									SELECT TOP(1) @PlanIdSelect=PlanId FROM #tempPlanIds WHERE ID=@NumUserPlan  
									PRINT CONCAT('------------>>>>>>>>>>>>>>>>>>>    ',@PlanIdSelect)  
									INSERT INTO UserPlans(userId,programPackageId) VALUES(@UserId,@PlanIdSelect)  
  
									SET @NumUserPlan=@NumUserPlan+1  
  
								END  ------ UserPlan Loop  
							PRINT 'End UserPlan'
							
							CREATE TABLE #tempProgramAccountInitialBalanceContent  
							(  
								ID int identity(1,1) not null,  
								ProgramId int NULL,  
								PlanId int NULL,  
								ProgramAccountId int NULL,  
								AccountName NVARCHAR(150) NULL,  
								intialBalanceCount DECIMAL NULL,  
								AccountTypeId int NULL  
							) 
							
							INSERT INTO #tempProgramAccountInitialBalanceContent(ProgramId,PlanId,ProgramAccountId,AccountName,intialBalanceCount,AccountTypeId)  
							Select   
                                                                        pp.ProgramId,  
                                                                        pp.Id as PlanId,  
                                                                        ppal.ProgramAccountId,  
                                                                        pa.AccountName,  
                                                                        pa.intialBalanceCount,  
                                                                        pa.AccountTypeId  
                                                                        from ProgramPackage pp   
                                                                        INNER JOIN PlanProgramAccountsLinking ppal ON pp.Id=ppal.planId  
                                                                        INNER JOIN ProgramAccounts pa ON pa.Id=ppal.programAccountId  
                                                                        WHERE pp.Id IN (Select PlanId FROM #tempPlanIds) AND pa.AccountTypeId!=3

							--Select '#tempProgramAccountInitialBalanceContent'
							--Select @NumPlanProgramAccountLinking
							--Select * from #tempProgramAccountInitialBalanceContent

							DECLARE @UserTransactionInfoProgramAccountMaxCount INT,@NumProgramAccountUserTransactionInfo INT=1;  
							DECLARE @UserFoundryAdminId INT;  
							SET @UserTransactionInfoProgramAccountMaxCount=(SELECT COUNT(*)  
							FROM #tempProgramAccountInitialBalanceContent)  
							PRINT 'Start UserTransactionInfo'  
							WHILE(@NumProgramAccountUserTransactionInfo <= @UserTransactionInfoProgramAccountMaxCount)  
								BEGIN   ---- UserTransactionInfo Loop
									DECLARE @ProgramAccountIdSelect INT,@UserTransactionInfoAccountTypeId INT,@UTPlanIdSelect INT  
									DECLARE @InitialAmount DECIMAL  
  
									SELECT TOP(1) @ProgramAccountIdSelect=ProgramAccountId,@UserTransactionInfoAccountTypeId=AccountTypeId,@UTPlanIdSelect=PlanId,@InitialAmount=intialBalanceCount FROM #tempProgramAccountInitialBalanceContent WHERE ID=@NumProgramAccountUserTransactionInfo--@NumPlanProgramAccountLinking  
     
									SELECT @UserFoundryAdminId=Id from [User] Where Email='foundry@mailinator.com'
									
									--Select 'Values'

									--SELECT @ProgramAccountIdSelect
									--SELECT @UserTransactionInfoAccountTypeId
									--SELECT @InitialAmount
									--SELECT @UTPlanIdSelect

									--Print 'START --Credit By Admin in Users Account(Initial Balance)'
									--Print 'Admin ==  @UserTransactionInfoAccountTypeId:-' + Cast(@UserTransactionInfoAccountTypeId as varchar(10))
									--Print 'Admin ==  @InitialAmount:-' + Cast(@InitialAmount as varchar(10))
									--Print 'Admin ==  @UTPlanIdSelect:-' + Cast(@UTPlanIdSelect as varchar(10))
									--Print 'Admin ==  @ProgramAccountIdSelect:-' + Cast(@ProgramAccountIdSelect as varchar(10))
									--Print 'END --Credit By Admin in Users Account(Initial Balance)'
									INSERT INTO UserTransactionInfo(debituserid,credituserid,accounttypeid,transactionAmount,periodremark,transactiondate,programid,isactive,createdby,createddate,
									modifiedby,modifieddate,isdeleted,credittransactionusertype,organisationid,planid,programAccountid,merchantid,transactionstatus,debittransactionusertype,terminalid,transactionid)
									values(@UserFoundryAdminId,@UserId,@UserTransactionInfoAccountTypeId,@InitialAmount,NULL,Getdate(),@ProgramId,2,@UserFoundryAdminId,getdate(),@UserFoundryAdminId,getdate(),0,1,@OrganisationId,@UTPlanIdSelect,@ProgramAccountIdSelect,NULL,1,4,NULL,
									(Select 'Tran' + Cast((ISNULL(MAX(id),0) + 1) as varchar(10)) from UserTransactionInfo)) 


									---------------------------------------------------------------------------------- Debit Account Transactions for User  ----------------------------------------------------------------------------  
  
  
									DECLARE @DebitUserTransactionCount INT = 1,   
									@DebitUserTransactionMerchantMaxCount INT, @DebitUserTransactionId INT  
   
									SET @DebitUserTransactionMerchantMaxCount=(SELECT COUNT(*)  
									FROM #tempMerchantIds)  
									PRINT @DebitUserTransactionMerchantMaxCount  
									--PRINT 'Start Account Debit User Transactions' 
									
									--Print 'START --Credit By User in Merchnat Account--------------------------------------'
									--Print 'Admin ==  @UserTransactionInfoAccountTypeId:-' + Cast(@UserTransactionInfoAccountTypeId as varchar(10))
									--Print 'Admin ==  @InitialAmount:-' + Cast(@InitialAmount as varchar(10))
									--Print 'Admin ==  @UTPlanIdSelect:-' + Cast(@UTPlanIdSelect as varchar(10))
									--Print 'Admin ==  @ProgramAccountIdSelect:-' + Cast(@ProgramAccountIdSelect as varchar(10))
									--Print 'END ---Credit By User in Merchnat Account--------------------------------------'
									 
									WHILE(@DebitUserTransactionCount <= @DebitUserTransactionMerchantMaxCount)
										BEGIN  ---- Debit User Transaction Loop  
    
											DECLARE @MerchantIdDebitUserTransactionSelect INT  
       
											SELECT TOP(1) @MerchantIdDebitUserTransactionSelect=MerchantId FROM #tempMerchantIds WHERE ID=@DebitUserTransactionCount  
  
    
		
											DECLARE @NumMerchantDebitTransactionForAccountDetail INT=1;  
											PRINT 'Start Inner merchant account debit User transaction'  
											WHILE(@NumMerchantDebitTransactionForAccountDetail <= 20)  
												BEGIN  ----  Inner merchant account debit User transaction Loop
													IF(@NumMerchantDebitTransactionForAccountDetail<=3)  
														BEGIN
															Print '__IF'
															INSERT INTO UserTransactionInfo(debituserid,credituserid,accounttypeid,transactionAmount,periodremark,transactiondate,programid,isactive,createdby,createddate,
															modifiedby,modifieddate,isdeleted,credittransactionusertype,organisationid,planid,programAccountid,merchantid,transactionstatus,debittransactionusertype,terminalid,transactionid)
															values(@UserId,@MerchantIdDebitUserTransactionSelect,@UserTransactionInfoAccountTypeId,(CASE WHEN @NumMerchantDebitTransactionForAccountDetail = 1 Then 3
															when @NumMerchantDebitTransactionForAccountDetail = 2 Then 4 Else 5 end)
															,(CASE WHEN @NumMerchantDebitTransactionForAccountDetail = 1 Then 'Lunch'
															when @NumMerchantDebitTransactionForAccountDetail = 2 Then 'Brunch' Else 'MealPeriod' end),Getdate(),@ProgramId,1,@UserId,getdate(),@UserId,getdate(),0,3,@OrganisationId,@UTPlanIdSelect,@ProgramAccountIdSelect,@MerchantIdDebitUserTransactionSelect,1,1,(Select Top 1 id from MerchantTerminal where organisationId = @MerchantIdDebitUserTransactionSelect),(Select 'Tran' + Cast((ISNULL(MAX(id),0) + 1) as varchar(10)) from UserTransactionInfo))    
   
															SET @DebitUserTransactionId=IDENT_CURRENT('UserTransactionInfo')  
														END 
													ELSE IF(@NumMerchantDebitTransactionForAccountDetail=4)  
														BEGIN  
															Print '__ELSEIF'
															INSERT INTO UserTransactionInfo(debituserid,credituserid,accounttypeid,transactionAmount,periodremark,transactiondate,programid,isactive,createdby,createddate,
															modifiedby,modifieddate,isdeleted,credittransactionusertype,organisationid,planid,programAccountid,merchantid,transactionstatus,debittransactionusertype,terminalid,transactionid)
															values(@UserId,@MerchantIdDebitUserTransactionSelect,@UserTransactionInfoAccountTypeId,6,'Breakfast',Getdate(),@ProgramId,1,@UserId,getdate(),@UserId,getdate(),0,3,@OrganisationId,@UTPlanIdSelect,@ProgramAccountIdSelect,@MerchantIdDebitUserTransactionSelect,2,1,(Select Top 1 id from MerchantTerminal where organisationId = @MerchantIdDebitUserTransactionSelect),(Select 'Tran' + Cast((ISNULL(MAX(id),0) + 1) as varchar(10)) from UserTransactionInfo))      
															SET @DebitUserTransactionId=IDENT_CURRENT('UserTransactionInfo')  
														END
													ELSE  
														BEGIN
															Print '__ELSE'  
															INSERT INTO UserTransactionInfo(debituserid,credituserid,accounttypeid,transactionAmount,periodremark,transactiondate,programid,isactive,createdby,createddate,
															modifiedby,modifieddate,isdeleted,credittransactionusertype,organisationid,planid,programAccountid,merchantid,transactionstatus,debittransactionusertype,terminalid,transactionid)
															values(@UserId,@MerchantIdDebitUserTransactionSelect,@UserTransactionInfoAccountTypeId,7,'Dinner',Getdate(),@ProgramId,1,@UserId,getdate(),@UserId,getdate(),0,3,@OrganisationId,@UTPlanIdSelect,@ProgramAccountIdSelect,@MerchantIdDebitUserTransactionSelect,0,1,(Select Top 1 id from MerchantTerminal where organisationId = @MerchantIdDebitUserTransactionSelect),(Select 'Tran' + Cast((ISNULL(MAX(id),0) + 1) as varchar(10)) from UserTransactionInfo))        
															SET @DebitUserTransactionId=IDENT_CURRENT('UserTransactionInfo')  
														END
													SET @NumMerchantDebitTransactionForAccountDetail=@NumMerchantDebitTransactionForAccountDetail+1    
												END
											--   ----  Inner merchant account debit User transaction Loop  
											PRINT 'End Inner merchant account debit User transaction'  
											SET @DebitUserTransactionCount=@DebitUserTransactionCount+1  
										END  ---- Debit user transaction  Loop  
  
										PRINT 'End Account Debit user transaction' 
									SET @NumProgramAccountUserTransactionInfo=@NumProgramAccountUserTransactionInfo+1 
								END 
								PRINT 'End UserTransactionInfo'
								
								----------------------------------------------------------------------       Benefactor     ---------------------------------------------------------------------------------------------------------------------------------------------------------- 
 
  
								DECLARE @BenefactorUserId INT,@NumBenefactorUser INT=1  
								PRINT 'Start Benefactor'  
								while @NumBenefactorUser <= 10--100    -------- Benefactor Loop  
									BEGIN    
										INSERT INTO [User](UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,  
										PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,FirstName,MiddleName,LastName,OrganisationId,  
										[Address],Location,UserCode,Custom1,Custom2,Custom3,Custom4,Custom5,Custom6,Custom7,Custom8,Custom9,Custom10,Custom11,Custom12,UserDeviceId,  
										UserDeviceType,SessionId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDeleted,IsAdmin,ProgramId,secondaryEmail,genderId,dateOfBirth,  
										customInfo,IsMobileRegistered,InvitationStatus)  
										VALUES(CONCAT('BenefactorDummyNew-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser,'@yopmail.com'),UPPER(CONCAT('Benefactor-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser,'@yopmail.com')),CONCAT('Benefactor-',@OrganisationId,'-',@ProgramId,'-',
										@NumBenefactorUser,'@yopmail.com'),UPPER(CONCAT('Benefactor-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser,'@yopmail.com')),1,'AQAAAAEAACcQAAAAEDM5iXo1pgIQ9iooLw4JjuYSVEB2qWSpUpxgHgxiGzePUXvpqdco2jJ6IYQjFVLnJA==',NULL,NULL,'6575547843',
										1,0,NULL,0,0,CONCAT('Benefactor-FirstName-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser),NULL,CONCAT('Benefactor-LastName-',@OrganisationId,'-',@ProgramId,'-',@NumBenefactorUser),@OrganisationId,NULL,'30.7093802,76.6952725',  NULL,NULL,NULL,NULL
										,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NEWID(),1,@UserId,GETUTCDATE(),@UserId,GETUTCDATE(),0,0,@ProgramId,NULL,  NULL,NULL,NULL,1,NULL)  
										SET @BenefactorUserId=IDENT_CURRENT('[User]')  
  
  
										--INSERT INTO #tempBenefactorIds(BenefactorId) VALUES(@BenefactorUserId)    
										IF(@NumBenefactorUser%2=0)  
											BEGIN  
												INSERT INTO BenefactorUsersLinking(userId,benefactorId,relationshipId,linkedDateTime,canViewTransaction,isActive,createdBy,createdDate,modifiedBy,  
												modifiedDate,isDeleted,IsRequestAccepted,IsInvitationSent)  
												VALUES(@UserId,@BenefactorUserId,3,GETUTCDATE(),1,1,@UserId,GETUTCDATE(),@UserId,GETUTCDATE(),0,1,1)  
  
											END  
										ELSE  
											BEGIN  
												INSERT INTO BenefactorUsersLinking(userId,benefactorId,relationshipId,linkedDateTime,canViewTransaction,isActive,createdBy,createdDate,modifiedBy,  
												modifiedDate,isDeleted,IsRequestAccepted,IsInvitationSent)  
												VALUES(@UserId,@BenefactorUserId,4,GETUTCDATE(),1,1,@UserId,GETUTCDATE(),@UserId,GETUTCDATE(),0,1,1)  
											END  
  
										--------------------------------------------------------------------  UserTransactionInfo Benefactor Credit  -------------------------------------------------------------------------------------------------------  
  
										DECLARE @NumBenefactorUserTransaction INT=1  
										PRINT 'Start Benefactor Credit User Transaction'  
										while @NumBenefactorUserTransaction <= 40    -------- Benefactor Credit User Transaction Loop  
											BEGIN   
												IF(@NumBenefactorUserTransaction<=2)  
													BEGIN  
														INSERT INTO UserTransactionInfo(debituserid,credituserid,accounttypeid,transactionAmount,periodremark,transactiondate,programid,isactive,createdby,createddate,
														modifiedby,modifieddate,isdeleted,credittransactionusertype,organisationid,planid,programAccountid,merchantid,transactionstatus,debittransactionusertype,terminalid,transactionid)
														values(@BenefactorUserId,@UserId,3,2000,NULL,Getdate(),@ProgramId,1,@BenefactorUserId,getdate(),@BenefactorUserId,getdate(),0,1,@OrganisationId,@UTPlanIdSelect,@ProgramAccountIdSelect,NULL,1,2,NULL,(Select 'Tran' + Cast((ISNULL(MAX(id),0) + 1) as varchar(10)) from UserTransactionInfo))
													END  
												ELSE IF(@NumBenefactorUserTransaction<=2)  
													BEGIN  
														INSERT INTO UserTransactionInfo(debituserid,credituserid,accounttypeid,transactionAmount,periodremark,transactiondate,programid,isactive,createdby,createddate,
														modifiedby,modifieddate,isdeleted,credittransactionusertype,organisationid,planid,programAccountid,merchantid,transactionstatus,debittransactionusertype,terminalid,transactionid)
														values(@BenefactorUserId,@UserId,3,2000,NULL,Getdate(),@ProgramId,1,@BenefactorUserId,getdate(),@BenefactorUserId,getdate(),0,1,@OrganisationId,@UTPlanIdSelect,@ProgramAccountIdSelect,NULL,2,2,NULL,(Select 'Tran' + Cast((ISNULL(MAX(id),0) + 1) as varchar(10)) from UserTransactionInfo))
    
													END  
												ELSE  
													BEGIN  
														INSERT INTO UserTransactionInfo(debituserid,credituserid,accounttypeid,transactionAmount,periodremark,transactiondate,programid,isactive,createdby,createddate,
														modifiedby,modifieddate,isdeleted,credittransactionusertype,organisationid,planid,programAccountid,merchantid,transactionstatus,debittransactionusertype,terminalid,transactionid)
														values(@BenefactorUserId,@UserId,3,2000,NULL,Getdate(),@ProgramId,1,@BenefactorUserId,getdate(),@BenefactorUserId,getdate(),0,1,@OrganisationId,@UTPlanIdSelect,@ProgramAccountIdSelect,NULL,0,2,NULL,(Select 'Tran' + Cast((ISNULL(MAX(id),0) + 1) as varchar(10)) from UserTransactionInfo))  
													END
												SET @NumBenefactorUserTransaction=@NumBenefactorUserTransaction+1;  
											END ------ Benefactor Credit User Transaction Loop  
										PRINT 'END Benefactor Credit User Transaction'  
										SET @NumBenefactorUser=@NumBenefactorUser+1;  
  
									END ---- End Benefactor Loop  
								PRINT 'END Benefactor'    
								SET @NumUser=@NumUser+1;  
								DROP TABLE #tempProgramAccountInitialBalanceContent  
							END ---- -------- User (Account Holder) Loop  
					PRINT 'End User (Account Holder) '
					SET @NumProgram=@NumProgram+1  
					DROP TABLE #tempPlanIds  
					DROP TABLE #tempMerchantIds  
					DROP TABLE #tempProgramAccountIds  
				END ----- Program Loop  PRINT 'End Program'
			SET @NumOrganisation=@NumOrganisation+1  
		END   ---- Organisation Loop  
	PRINT 'End Organisation'
	
	--END TRY
	
	--BEGIN CATCH
	--	SELECT   
 --       ERROR_NUMBER() AS ErrorNumber  
 --      ,ERROR_MESSAGE() AS ErrorMessage; 
	--END CATCH 
END  
GO
/****** Object:  StoredProcedure [dbo].[InsertUpdateBinData]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE  [dbo].[InsertUpdateBinData]
@BinFile [dbo].[BinFile] READONLY
AS
BEGIN
MERGE dbo.BinData t  USING @BinFile s
ON ( t.BinNumberStart =s.binstart AND t.BinNumberEnd =s.binend AND t.CountryCode =s.code)
WHEN MATCHED 
     THEN UPDATE
     SET   t.UpdatedDate= GETDATE()
	 ,t.[Delete]=NULL
WHEN NOT MATCHED BY TARGET
THEN INSERT ( BinNumberStart , BinNumberEnd , CountryCode , CreatedDate)
    VALUES (s.binstart, s.binend, s.code, GETDATE())
WHEN NOT MATCHED BY SOURCE
THEN   UPDATE
     SET   t.UpdatedDate= GETDATE() , t.[Delete]=1;
	 
	 END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSodexhoUserType]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetSodexhoUserType]  
(  
 @Id INT)  
 as  
 DECLARE @out varchar(50)  
IF EXISTS(SELECT        dbo.[User].Id, dbo.[User].Email, dbo.I2CAccountDetail.UserId, dbo.ReloadRules.isDeleted,  
 dbo.ReloadRules.i2cBankAccountId, dbo.[User].ProgramId  
FROM            dbo.[User] left JOIN  
                         dbo.I2CAccountDetail ON dbo.[User].Id = dbo.I2CAccountDetail.UserId left JOIN  
                         dbo.ReloadRules ON dbo.[User].Id = dbo.ReloadRules.userId   
       where ReloadRules.IsDeleted=0 and [User].id = @Id)  
          
 set @out='vip';  
ELSE  
IF EXISTS(SELECT        dbo.[User].Id, dbo.[User].Email, dbo.I2CAccountDetail.UserId, dbo.[User].ProgramId  
FROM            dbo.[User] INNER JOIN  
                         dbo.I2CAccountDetail ON dbo.[User].Id = dbo.I2CAccountDetail.UserId    
           where [User].id = @Id)  
    
    set @out='bite pay';  
  
 ELSE  
   
      set @out='regular';  
 select @out as usertype;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserCardwithBankAccount]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_GetUserCardwithBankAccount]
(@byUserId int,
@toUserId int)
as
begin
declare @bankaccount table(CardBankName varchar(100),IdValue varchar(100));
insert into @bankaccount
select
case when cba.Status=1 then cba.bankname +' (Verified)' 
      else cba.bankname +' (Non Verified)' end,cba.accountSrNo
	  from [dbo].[i2cCardBankAccount] cba
INNER JOIN i2caccountdetail icd ON icd.Id=cba.I2cAccountDetailId AND icd.UserId=@toUserId
WHERE cba.UserId=@byUserId


Insert into @bankaccount 
select nickName +'('+ maskedLastDigitCard + ')' ,ClientToken from GatewayCardWebHookToken
where   creditUserId=@toUserId and debitUserId=@byUserId and IsCardToSave=1 and maskedLastDigitCard is not null
select * from @bankaccount
end
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserCardwithBankAccount1]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_GetUserCardwithBankAccount1]  
(@byUserId int,  
@toUserId int)  
as  
begin  
declare @bankaccount table(CardBankName varchar(100),IdValue varchar(100),CardStatus bit);  
insert into @bankaccount  
select  
case when cba.Status=1 then cba.bankname +' (Verified)'   
      else cba.bankname +' (Non Verified)' end,cba.accountSrNo  ,1
   from [dbo].[i2cCardBankAccount] cba  
INNER JOIN i2caccountdetail icd ON icd.Id=cba.I2cAccountDetailId AND icd.UserId=@toUserId  
WHERE cba.UserId=@byUserId  
  
  
Insert into @bankaccount   
select nickName +'('+ maskedLastDigitCard + ')' ,ClientToken,  ISNULL(IsCardValid,1)   from GatewayCardWebHookToken  
where   creditUserId=@toUserId and debitUserId=@byUserId and IsCardToSave=1 and maskedLastDigitCard is not null  
select * from @bankaccount  
end  

GO
/****** Object:  StoredProcedure [dbo].[SplitString]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[SplitString] 
@stringValue  NVARCHAR(50),
@long NVARCHAR(20) OUTPUT,
@lat NVARCHAR(20) OUTPUT
AS 
BEGIN
		DECLARE @leftValue NVARCHAR(30)

		Select @long = SUBSTRING(@stringValue, 0, CHARINDEX (',', @stringValue)),
		 @lat = LTRIM(SUBSTRING(@stringValue, CHARINDEX (',', @stringValue) +1, LEN(@stringValue)))
		SELECT @long as Long, @lat as Lat
END
GO
/****** Object:  StoredProcedure [dbo].[UserPushNotificationList]    Script Date: 7/30/2020 7:55:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**********************************************************************************************************************

Procedure Name: UserPushNotificationList
Exec Statement to Run: exec UserPushNotificationList 1072,19,0,1,0,1,10,'',''

************************************************************************************************************************/
CREATE PROCEDURE [dbo].[UserPushNotificationList]
@UserId INT=0,
@ProgramId INT=0,
@NotificationType INT=0,
@IsActive BIT=1,
@IsDeleted BIT=0,
@PageNumber INT=1, 
@PageSize INT=10,
@UserDeviceId nvarchar(MAX)='',
@UserDeviceType nvarchar(40)=''
AS 
BEGIN
DECLARE @TopNotificationId INT, @TopUserNotificationReadId INT;

Set @TopUserNotificationReadId= (Select ISNULL(notificationId,0) from UserPushedNotificationsStatus where UserId=@UserId);


WITH ctepaging 
     AS  (
		 Select 
		 Row_number() OVER(ORDER BY upn.CreatedDate DESC) AS SrNo,
upn.ID as UserNotificationId,
NotificationMessage,
referenceId,
convert(varchar(50), upn.createdDate, 107) AS NotificationDate,
FORMAT(upn.createdDate,'hh:mm tt') as NotificationTime,
nts.Id AS NotificationTypeId,
nts.Name AS NotificationType,
nts.ColorCode as ColorCode,
upn.IsRedirect as IsRedirect,
upn.NotificationSubType as NotificationSubType,
upn.CustomReferenceId as CustomReferenceId,
CASE WHEN upn.Id>@TopNotificationId THEN 0
ELSE 1 END As IsNotificationRead
 from UserPushedNotifications upn
 INNER JOIN NotificationSettings nts ON nts.Id=upn.notificationType
 Where upn.IsActive=@IsActive AND upn.IsDeleted=@IsDeleted 
 AND (upn.UserId=0 OR upn.UserId=@UserId)
 AND (upn.ProgramId=0 OR upn.ProgramId=@ProgramId)
 AND (@NotificationType=0 OR (upn.notificationType=@NotificationType)) )	
	

SELECT * 
INTO #ctepaging
FROM ctepaging 


SET @TopNotificationId=(SELECT Top(1) UserNotificationId FROM #ctepaging)

SELECT * 
FROM #ctepaging
WHERE  SrNo BETWEEN ( @PageNumber - 1 ) * @PageSize + 1 AND
@PageNumber * @PageSize


Select RANK () OVER ( 
 ORDER BY ID 
 ) SrNo,Id as NotificationTypeId, Name as NotificationType, colorCode as ColorCode from NotificationSettings UNION 
SELECT 6 as SrNo,0 as NotificationTypeId, 'See all' as NotificationType, ''


IF EXISTS(Select * from UserPushedNotificationsStatus Where UserId=@UserId)
BEGIN 
UPDATE UserPushedNotificationsStatus Set notificationId=@TopNotificationId,
   UserDeviceId=@UserDeviceId,userDeviceType=@UserDeviceType,IsReadTillId=1,notificationReadDate=GETUTCDATE()
    Where UserId=@UserId
END ELSE
BEGIN
INSERT INTO UserPushedNotificationsStatus(UserId,UserDeviceId,userDeviceType,notificationId)
       VALUES(@UserId,@UserDeviceId,@UserDeviceType,@TopNotificationId)
END
END


GO
USE [master]
GO
ALTER DATABASE [TroveProduction] SET  READ_WRITE 
GO
