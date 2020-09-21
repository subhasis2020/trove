USE [TroveProduction]
GO
/****** Object:  StoredProcedure [dbo].[GetNotificationsLogs]    Script Date: 25-08-2020 1.37.37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/***********************************************************************************************************************************************      
Test Result Run : Exec GetNotificationsLogs 1
************************************************************************************************************************************************/      
CREATE PROCEDURE [dbo].[GetNotificationsLogs]      
(      
@PageNumber int=1,      
@PageSize int=10,      
@SortColumnName nvarchar(20)='CreatedDate',      
@SortOrderDirection nvarchar(10)='DESC'    
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
CASE WHEN (@SortColumnName='CreatedDate' AND @SortOrderDirection='asc') THEN PartnerNotificationsLog.CreatedDate END ASC,      
CASE WHEN (@SortColumnName='CreatedDate' AND @SortOrderDirection='desc') THEN PartnerNotificationsLog.CreatedDate END DESC   ,
CASE WHEN (@SortColumnName='ApiName' AND @SortOrderDirection='asc') THEN ApiName END ASC,      
CASE WHEN (@SortColumnName='ApiName' AND @SortOrderDirection='desc') THEN ApiName END DESC  
   
) AS RowNumber,      
COUNT(*) OVER() AS TotalCount,  u.PartnerUserId, PartnerNotificationsLog.*
FROM   PartnerNotificationsLog  join [User] u on PartnerNotificationsLog.UserId=u.Id
where PartnerNotificationsLog.UserId is not null
--order by  l.CreatedDate desc
       
  )      
    
  SELECT RowNumber,TotalCount,Id,UserId,ApiName,ApiUrl,Request,Response,[Status],CreatedDate ,PartnerUserId     
  FROM CTEResult      
  WHERE (RowNumber>@StartRow AND RowNumber<@EndRow)  
  END 

  go
--=======================

GO
/****** Object:  StoredProcedure [dbo].[GetNotificationsLogsWithFilter]    Script Date: 25-08-2020 1.37.09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/***********************************************************************************************************************************************        
Test Result Run : Exec GetNotificationsLogsWithFilter 1 ,10,'CreatedDate','DESC',null,null,'2020-08-12',null
************************************************************************************************************************************************/        
CREATE	 PROCEDURE [dbo].[GetNotificationsLogsWithFilter]        
(        
@PageNumber int=1,        
@PageSize int=10,        
@SortColumnName nvarchar(20)='CreatedDate',        
@SortOrderDirection nvarchar(10)='DESC' ,
@ApiName nvarchar(100)=null,
@Status nvarchar(50)=null,
@Date datetime=null,
@ProgramId nvarchar(10)=null
)    
AS        
BEGIN        
         
 SET NOCOUNT ON;        
     Declare @SQLQuery AS NVarchar(4000)   
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
CASE WHEN (@SortColumnName='CreatedDate' AND @SortOrderDirection='asc') THEN PartnerNotificationsLog.CreatedDate END ASC,        
CASE WHEN (@SortColumnName='CreatedDate' AND @SortOrderDirection='desc') THEN PartnerNotificationsLog.CreatedDate END DESC   ,  
CASE WHEN (@SortColumnName='ApiName' AND @SortOrderDirection='asc') THEN ApiName END ASC,        
CASE WHEN (@SortColumnName='ApiName' AND @SortOrderDirection='desc') THEN ApiName END DESC    
     
) AS RowNumber,        
COUNT(*) OVER() AS TotalCount,  u.PartnerUserId, PartnerNotificationsLog.*  
FROM   PartnerNotificationsLog  join [User] u on PartnerNotificationsLog.UserId=u.Id  
where PartnerNotificationsLog.UserId is not null  
and ((@ApiName is null) or (ApiName = @ApiName))
and ((@Status is null) or (Status = @Status))
and ((@Date is null) or (CAST(PartnerNotificationsLog.CreatedDate AS DATE) = @Date))
and ((@ProgramId is null) or (u.ProgramId = @ProgramId))
         
  )        
      
  SELECT RowNumber,TotalCount,Id,UserId,ApiName,ApiUrl,Request,Response,[Status],CreatedDate ,PartnerUserId       
  FROM CTEResult        
  WHERE (RowNumber>@StartRow AND RowNumber<@EndRow)    
  END 

  go
  ---================================================

  GO
/****** Object:  StoredProcedure [dbo].[GetLoyaltyTrackingTransactions]    Script Date: 25-08-2020 1.35.58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/***********************************************************************************************************************************************      
SP Name: GetAccountHolders      
Description: This stored procedure will fetch the list of Account holders (users) based on the search value and server side paging.      
Test Result Run : Exec GetLoyaltyTrackingTransactions 1311,1  
************************************************************************************************************************************************/      
CREATE PROCEDURE [dbo].[GetLoyaltyTrackingTransactions]      
(      
@userId int,      
@PageNumber int=1,      
@PageSize int=10,      
@SortColumnName nvarchar(20)='transactionDate',      
@SortOrderDirection nvarchar(10)='DESC'    
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
CASE WHEN (@SortColumnName='transactionDate' AND @SortOrderDirection='asc') THEN transactionDate END ASC,      
CASE WHEN (@SortColumnName='transactionDate' AND @SortOrderDirection='desc') THEN transactionDate END DESC    
   
) AS RowNumber,      
COUNT(*) OVER() AS TotalCount,      
     CASE  WHEN isThresholdReached=1 THEN  totalPoints-leftOverPoints ELSE 0.00 END [pointDebited] ,*  
       FROM  dbo.UserLoyaltyPointsHistory where userId =@userId   
       
  )      
    
      
  SELECT RowNumber,TotalCount,Id,transactionDate,pointDebited,TranlogID,transactionAmount,pointsEarned,totalPoints,rewardAmount,leftOverPoints      
  FROM CTEResult      
  WHERE (RowNumber>@StartRow AND RowNumber<@EndRow) and userId =@userId 
  END 