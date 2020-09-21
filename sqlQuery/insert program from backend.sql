--SELECT ID FROM  dbo.Organisation WHERE name='SODEXHO'
--SELECT * FROM dbo.Program WHERE name='sodexhoprogram'
--SELECT * FROM dbo.ProgramAccounts WHERE programId='36'
--SELECT * FROM dbo.ProgramPackage WHERE name='SODEXHOplan name'

--SELECT * FROM dbo.ProgramAccountLinking

SELECT * FROM dbo.PlanProgramAccountsLinking --25
SELECT * FROM dbo.i2cCardBankAccount
SELECT * FROM dbo.I2CAccountDetail


--SELECT* FROM dbo.[OrganisationProgram] --74    --1
SELECT ProgramCodeId FROM dbo.[Program] --43 ---1
--SELECT* FROM dbo.[ProgramAccounts] --40 --2
--SELECT* FROM dbo.[ProgramPackage] --18 --1
SELECT * FROM dbo.[User]
SELECT * FROM ProgramPackage
--SELECT * FROM dbo.PlanProgramAccountsLinking WHERE planId='1035'

SELECT * FROM dbo.UserRelations
select 'SAINT MICHAEL''S COLLEGE'

--="insert into #d  values('"&A2&"','"&B2&"','"&C2&"','"&D2&"','"&E2&"','"&F2&"','"&G2&"','"&H2&"','"&I2&"','"&J2&"','"&K2&"','"&L2&"','"&M2&"','"&N2&"','"&O2&"')"
--="insert into #d  values('"&A2&"','"&B2&"','"&C2&"','"&D2&"','"&E2&"','"&TEXT(F2,"yyyy-mm-dd")&"','"&TEXT(G2,"yyyy-mm-dd")&"','"&H2&"','"&I2&"','"&J2&"','"&K2&"','"&L2&"','"&M2&"','"&N2&"','"&O2&"')"
drop table #d
CREATE TABLE #d (
id INT,	institutionid INT,	active VARCHAR(50),	name VARCHAR(300),	tz VARCHAR(500),	startdate DATETIME	,enddate DATETIME,
	journal  VARCHAR(500),	assetsaccount  VARCHAR(500)	,earningsaccount VARCHAR(500),	lossesaccount  VARCHAR(500),	cpissuer	 VARCHAR(500),  authtype  VARCHAR(500),	authurl VARCHAR(500),	ds VARCHAR(500)

)

insert into #d  values('2','2','Y','STONEHILL COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('8','8','Y','CENTRAL STATE UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','156','up','','saas')
insert into #d  values('9','9','Y','COLORADO CHRISTIAN UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','732','up','','saas')
insert into #d  values('10','10','Y','Johnson State College N Vermont Univ','UTC','2020-08-01','2030-08-01','1','','','','','up','','saas')
insert into #d  values('13','13','Y','OTERO JUNIOR COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','747','up','','saas')
insert into #d  values('15','15','Y','ROCKY MOUNTAIN COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','229','up','','saas')
insert into #d  values('16','16','Y','DEAN COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','659','up','','saas')
insert into #d  values('18','18','Y','EASTERN NEW MEXICO UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','743','up','','saas')
insert into #d  values('19','19','Y','EASTERN OREGON UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','675','up','','saas')
insert into #d  values('21','21','Y','ACADEMY OF ART DINING SERVICES','UTC','2020-08-01','2030-08-01','1','','','','765','up','','saas')
insert into #d  values('26','26','Y','CABRINI UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','724','up','','saas')
insert into #d  values('28','28','Y','CASPER COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','717','up','','saas')
insert into #d  values('30','30','Y','CENTENARY COLLEGE OF LOUISIANA','UTC','2020-08-01','2030-08-01','1','','','','49','up','','saas')
insert into #d  values('31','31','Y','CENTRAL OREGON COMM COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','710','up','','saas')
insert into #d  values('32','32','Y','COLLEGE OF SOUTHERN IDAHO','UTC','2020-08-01','2030-08-01','1','','','','671','up','','saas')
insert into #d  values('35','35','Y','CROWN COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','687','up','','saas')
insert into #d  values('37','37','Y','EASTERN UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','602','up','','saas')
insert into #d  values('40','40','Y','Friends University','UTC','2020-08-01','2030-08-01','1','','','','613','up','','saas')
insert into #d  values('42','42','Y','HARCUM COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','634','up','','saas')
insert into #d  values('44','44','Y','KEYSTONE COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','54','up','','saas')
insert into #d  values('45','45','Y','LAMAR COMMUNITY COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','763','up','','saas')
insert into #d  values('46','46','Y','LARAMIE COUNTY COMMUNITY COLL','UTC','2020-08-01','2030-08-01','1','','','','713','up','','saas')
insert into #d  values('54','54','Y','NORTHWEST COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','697','up','','saas')
insert into #d  values('55','55','Y','OREGON INSTITUTE OF TECHNOLOGY','UTC','2020-08-01','2030-08-01','1','','','','615','up','','saas')
insert into #d  values('56','56','Y','ROCKFORD UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','654','up','','saas')
insert into #d  values('60','60','Y','BEACON COLLEGE INC','UTC','2020-08-01','2030-08-01','1','','','','691','up','','saas')
insert into #d  values('63','63','Y','HENDERSON STATE UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','695','up','','saas')
insert into #d  values('65','65','Y','SIMPSON COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','683','up','','saas')
insert into #d  values('66','66','Y','WILLIAM PENN COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','690','up','','saas')
insert into #d  values('68','68','Y','SOUTHEASTERN OKLAHOMA STATE UNIV','UTC','2020-08-01','2030-08-01','1','','','','672','up','','saas')
insert into #d  values('69','69','Y','SOUTHERN MAINE COMMUNITY COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','728','up','','saas')
insert into #d  values('70','70','Y','SOUTHERN NAZARENE UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','28','up','','saas')
insert into #d  values('71','71','Y','SPOKANE COMMUNITY COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','704','up','','saas')
insert into #d  values('72','72','Y','TRINIDAD STATE','UTC','2020-08-01','2030-08-01','1','','','','774','up','','saas')
insert into #d  values('73','73','Y','UNIV OF CALIFORNIA @ RIVERSIDE','UTC','2020-08-01','2030-08-01','1','','','','741','up','','saas')
insert into #d  values('75','75','Y','UNIVERSITY OF MARYLAND','UTC','2020-08-01','2030-08-01','1','','','','729','up','','saas')
insert into #d  values('76','76','Y','UTICA COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','725','up','','saas')
insert into #d  values('77','77','Y','VALLEY FORGE CHRISTIAN COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','761','up','','saas')
insert into #d  values('79','79','Y','WALLA WALLA COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','709','up','','saas')
insert into #d  values('81','81','Y','WILLIAM PEACE UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','267','up','','saas')
insert into #d  values('86','86','Y','LEE UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('87','87','Y','CONCORDIA UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('88','88','Y','NORWICH UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('89','89','Y','SAINT MICHAEL''S COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('90','90','Y','WESTERN CONNECTICUT ST UNIV MI','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('91','91','Y','SAINT AMBROSE UNIV.','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('92','92','Y','CHAMPLAIN COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('95','95','Y','EMORY & HENRY COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('96','96','Y','FRANKLIN PIERCE COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('97','97','Y','LEWIS-CLARK STATE COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('98','98','Y','MINNESOTA STATE UNIV MOORHEAD','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('100','100','Y','VIRGINIA WESLEYAN UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('102','102','Y','WEBSTER UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('103','103','Y','MONTANA STATE UNIV-BILLINGS','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('104','104','Y','KEISER UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('105','105','Y','CLAFLIN UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('106','106','Y','ADRIAN COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('107','107','Y','KEISER UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('109','109','Y','ALMA COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('110','110','Y','TEXAS LUTHERAN UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('111','111','Y','BETHEL COLLEGE-DINING COMMONS','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('112','112','Y','VINCENNES UNIV TDC 18','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('113','113','Y','ALDERSON-BROADDUS COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('114','114','Y','NEUMANN UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('115','115','Y','GRACE COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('116','116','Y','MANCHESTER UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('118','118','Y','COLUMBIA COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('119','119','Y','NORTH CAROLINA WESLEYAN','UTC','2020-08-01','2030-08-01','1','','','','766','up','','saas')
insert into #d  values('121','121','Y','COKER UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','642','up','','saas')
insert into #d  values('122','122','Y','MADONNA UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','255','up','','saas')
insert into #d  values('123','123','Y','WARNER PACIFIC COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','700','up','','saas')
insert into #d  values('124','124','Y','Fort Valley State University','UTC','2020-08-01','2030-08-01','1','','','','237','up','','saas')
insert into #d  values('126','126','Y','BUENA VISTA UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','689','up','','saas')
insert into #d  values('129','129','Y','EAST TENNESSEE STATE UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('131','131','Y','UNIVERSITY OF NEBRASKA - KEARNEY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('132','132','Y','OHIO NORTHERN UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('133','133','Y','NICHOLS COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('134','134','Y','SAGE COLLEGES','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('135','135','Y','UNIV OF MISSOURI KANSAS CITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('136','136','Y','WESTERN ILLINOIS UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('138','138','Y','USC-UPSTATE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('139','139','Y','UNIVERSITY OF MISSOURI ST. LOUIS','UTC','2020-08-01','2030-08-01','1','','','','119','up','','saas')
insert into #d  values('140','140','Y','BINGHAMTON UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','saas')
insert into #d  values('141','141','Y','CARROLL COLLEGE','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('144','144','Y','WAYLAND BAPTIST UNIVERSITY','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')
insert into #d  values('145','145','Y','Clark university','UTC','2020-08-01','2030-08-01','1','','','','','up','','cbord  ')


--SELECT * FROM  dbo.Organisation WHERE name='SODEXHO'
--UPDATE dbo.Program SET organisationId ='71' WHERE id IN (30,31,32,33,34,35)

--SELECT * FROM dbo.Program WHERE organisationId=71
--SELECT * FROM dbo.PlanProgramAccountsLinking

--SELECT * FROM dbo.ProgramPackage

--SELECT * FROM dbo.AccountType
--SELECT * FROM dbo.ProgramAccounts
--SELECT * FROM 

--INSERT	into dbo.[OrganisationProgram]
--        ( [organisationId] ,
--          [programId] ,
--          [CreatedDate] ,
--          [ModifiedDate] ,
--          [IsActive] ,
--          [IsDeleted] 
--        )

--SELECT organisationId,id, GETDATE() , -- CreatedDate - datetime
--          GETDATE() , -- ModifiedDate - datetime
--          1 , -- IsActive - bit
--          0  FROM dbo.Program WHERE id IN (30,31,32,33,34,35)
--SELECT * FROM [dbo].[OrganisationProgram] WHERE organisationId=71
--SELECT * FROM dbo.ProgramAccounts WHERE  programId=28



 DELETE FROM #d WHERE id IN (
  SELECT ProgramCodeId FROM dbo.Program
  )

SELECT ROW_NUMBER()OVER(ORDER BY name)ROW_NUMBER,* FROM #d

ALTER TABLE #d ADD  isdone BIT

drop table #d1
SELECT ROW_NUMBER()OVER(ORDER BY name)ROW_NUMBER,* INTO #d1 FROM #d

SELECT * FROM #d
DECLARE @ROW_NUMBER INT =0,
@Programid INT =0  ,
@accountid INT=0,
@organisationId INT =(SELECT ID FROM  dbo.Organisation WHERE name='Sodexo' )
,@planID INT =0
WHILE((SELECT COUNT(*) FROM #d1 WHERE ISNULL(isdone ,0)=0)>0)
BEGIN
set @ROW_NUMBER = (SELECT MIN(ROW_NUMBER ) FROM #d1 WHERE ISNULL(isdone ,0)=0)



INSERT INTO dbo.Program
        ( name ,
          organisationId ,
          isActive ,
         -- cratedBy ,
          createdDate ,
          timeZone ,
          ProgramCodeId ,
          ProgramTypeId ,
          AccountHolderUniqueId ,
          IsAllNotificationShow ,
          IsRewardsShowInApp
        )
		SELECT TOP 1 name ,@organisationId,1,GETDATE(),'Eastern Standard Time',CONVERT(VARCHAR,id ),1,'SOD000-'+ CONVERT(VARCHAR,id ), 1,1 FROM #d1 WHERE ROW_NUMBER=@ROW_NUMBER

	SET @Programid=	(SELECT  SCOPE_IDENTITY())

INSERT	into dbo.[OrganisationProgram]
        ( [organisationId] ,
          [programId] ,
          [CreatedDate] ,
          [ModifiedDate] ,
          [IsActive] ,
          [IsDeleted] 
        )
VALUES  ( @organisationId , -- organisationId - int
          @Programid , -- programId - int
          GETDATE() , -- CreatedDate - datetime
          GETDATE() , -- ModifiedDate - datetime
          1 , -- IsActive - bit
          0  -- IsDeleted - bit
        ) 



	INSERT INTO dbo.[ProgramAccounts]
		        ( [accountName] ,
		          [accountTypeId] ,
		          [programId] ,
		          [isPassExchangeEnabled] ,
		          [isRollOver] ,
		          [isActive] ,
		          [createdDate] ,
		          [isDeleted] ,
		          [vplMaxBalance] ,
		          [vplMaxAddValueAmount] ,
		          [vplMaxNumberOfTransaction] 
		        )
				
SELECT name+'-Accounts', 3,@Programid,1,0,1,GETDATE(),0,999999,999999,999999 FROM #d1 WHERE ROW_NUMBER= @ROW_NUMBER
SET @accountid=	(SELECT  SCOPE_IDENTITY())


INSERT INTO [dbo].[ProgramPackage]
        ( [name] ,
          [programId] ,
          [noOfMealPasses] ,
          [noOfFlexPoints] ,
          [createdDate] ,
         
          [isDeleted] ,
          [startDate] ,
          [endDate] ,
          [startTime] ,
          [endTime] ,
          [clientId] ,
          [planId] ,
          [isActive]
        )
	
		SELECT name+'-Plan',@Programid,0,0,GETDATE(),0,startdate,enddate,'00:00:00.0000000','00:00:00.0000000','SOD','SOD',1 FROM #d1 WHERE ROW_NUMBER=@ROW_NUMBER
SET @planID=	(SELECT  SCOPE_IDENTITY())

		
		INSERT INTO dbo.PlanProgramAccountsLinking
		        ( planId, programAccountId )
		VALUES  ( @planID, -- planId - int
		          @accountid -- programAccountId - int
		          )
		 

		
UPDATE #d1 SET  isdone=1 WHERE ROW_NUMBER=@ROW_NUMBER
END


SELECT * FROM Program


dbo.PlanProgramAccountsLinking






--SELECT * FROM #d1
  


--INSERT INTO dbo.Program
--        ( name ,
--          organisationId ,
--          isActive ,
--         -- cratedBy ,
--          createdDate ,
--          timeZone ,
--          ProgramCodeId ,
--          ProgramTypeId ,
--          AccountHolderUniqueId ,
--          IsAllNotificationShow ,
--          IsRewardsShowInApp
--        )
--		SELECT TOP 1 name ,3091,1,GETDATE(),'Eastern Standard Time','P1000-1',1,'AC1000-36', 1,1 FROM #d

--		SELECT SCOPE_IDENTITY()

--		SELECT* FROM dbo.[ProgramAccounts] --40 --2

--		INSERT INTO dbo.[ProgramAccounts]
--		        ( [accountName] ,
--		          [accountTypeId] ,
--		          [programId] ,
--		          [isPassExchangeEnabled] ,
--		          [isRollOver] ,
--		          [isActive] ,
--		          [createdDate] ,
--		          [isDeleted] ,
--		          [vplMaxBalance] ,
--		          [vplMaxAddValueAmount] ,
--		          [vplMaxNumberOfTransaction] 
--		        )
