using static Foundry.Domain.Constants;

namespace Foundry.Domain
{
    public static class SQLQueryConstants
    {
        #region Program
        public static readonly string CheckProgramExpiryQuery = @"SELECT prg.Id FROM Program prg JOIN UserProgram uprg ON" +
            " prg.Id = uprg.ProgramId AND uprg.UserId = @UserId WHERE prg.id = @ProgramId AND prg.IsActive = 1  AND prg.IsDeleted=0 AND uprg.IsActive = 1  AND uprg.IsDeleted=0 ";

        public static readonly string GetProgramByIDQuery = @"SELECT Prg.Id,Prg.Name,Prg.ColorCode,ISNULL(Img.PhotoPath,NULL) AS LogoPath,
        Prg.OrganisationId FROM Program prg LEFT JOIN Photo img ON prg.Id=img.EntityId
		 and img.PhotoType= " + (int)PhotoEntityType.Program + " where prg.Id=@ProgramId AND prg.IsActive=1 AND prg.IsDeleted=0";

        public static readonly string GetUserProgramByUserIDQuery = @"SELECT Prg.Id,
                                                                        Prg.Name,
                                                                        Prg.ColorCode,
                                                                       ISNULL(Img.PhotoPath,NULL) AS LogoPath,
                                                                        Prg.OrganisationId,
                                                                        up.IsLinkedProgram as IsProgramLinkedWithUser
                                                                        FROM Program prg
                                                                        JOIN UserProgram up ON prg.Id= up.ProgramID AND up.UserId= @UserId
                                                                         LEFT JOIN Photo img ON prg.Id= img.EntityId
                                                                         and img.PhotoType= " + (int)PhotoEntityType.Program + " WHERE prg.IsActive= 1 AND prg.IsDeleted= 0";

        public static readonly string GetUserProgramLinkingByEmailNIDQuery = @"SELECT up.* FROM UserProgram up JOIN [User] u ON" +
            " up.UserId = u.Id WHERE up.ProgramId = @ProgramId AND u.UserCode = @UserId AND u.Email=@Email AND (up.IsActive = @IsActive or up.IsActive is null) " +
            "AND (up.IsDeleted = @IsDeleted or up.IsDeleted is null)";

        public static readonly string GetUserProgramLinkingByEmailQuery = @"SELECT up.* FROM UserProgram up JOIN [User] u ON" +
            " up.UserId = u.Id WHERE up.ProgramId = @ProgramId AND LOWER(u.Email) = Lower(@EmailAddress) AND (up.IsActive = @IsActive or up.IsActive is null) " +
            "AND (up.IsDeleted = @IsDeleted or up.IsDeleted is null)";

        public static readonly string GetAllProgramsQuery = @"SELECT Prg.Id,Prg.Name,Prg.ColorCode,ISNULL(Img.PhotoPath,NULL) AS LogoPath,
        Prg.OrganisationId FROM Program prg LEFT JOIN Photo img ON prg.Id=img.EntityId
         and img.PhotoType= " + (int)PhotoEntityType.Program + "  WHERE prg.IsActive = 1  AND prg.IsDeleted= 0 ";

        public static readonly string GetProgramLevelAdminListQuery = @"
                                                                        Select DISTINCT
                                                                        usr.Id AS UserId,
                                                                        usr.FirstName+' ' +LastName AS Name,
                                                                        usr.Email AS EmailAddress,
                                                                        usr.CreatedDate AS DateAdded,
                                                                        usr.PhoneNumber AS PhoneNumber,
                                                                        rl.Name AS RoleName,
                                                                        usr.IsActive AS [Status],
                                                                         ISNULL(userPic.PhotoPath,NULL) AS UserImagePath,
                                                                       ProgramsAccessibility = STUFF((
																			SELECT N', ' + pt.ProgramTypeName FROM ProgramType pt
																			INNer JOIN AdminProgramAccess apa ON apa.ProgramTypeId=pt.Id
																			WHERE apa.UserId = usr.Id
																			FOR XML PATH(''), TYPE).value(N'.[1]', N'nvarchar(max)'), 1, 2, N''),
                                                                         Custom1 AS Title,
                                                                         IsAdmin,
                                                                         ISNULL(InvitationStatus, 0) AS InvitationStatus,
                                                                         EmailConfirmed
                                                                         FROM[User] usr
                                                                         LEFT JOIN UserRole urole ON usr.Id=urole.UserId
                                                                        LEFT JOIN[Role] rl ON urole.RoleId=rl.Id
                                                                        LEFT JOIN Photo userPic ON userPic.EntityId=usr.Id AND userPic.PhotoType=" + (int)PhotoEntityType.UserProfile +
                                                                " Where usr.ProgramId= @ProgramId AND usr.IsDeleted = 0 AND usr.IsAdmin=1";

        public static readonly string GetTransactions = @"select utf.Id,Amount= CASE WHEN utf.AccountTypeId=3 THEN 
 '$'+convert(varchar,convert(decimal(8,2), ISNULL(utf.TransactionAmount,0)))
 WHEN utf.AccountTypeId=2 THEN 
 convert(varchar,convert(decimal(8,2), ISNULL(utf.TransactionAmount,0)))
 ELSE  convert(varchar,convert(decimal(8,0), ISNULL(utf.TransactionAmount,0)))+' passes'
 END ,at.AccountType,o.Name as MerchantName,
utf.TransactionDate
from[dbo].[UserTransactionInfo]
        utf
inner join[dbo].[AccountType] at on utf.accounttypeid = at.id
inner join[dbo].[Organisation] o on utf.credituserid = o.id and o.organisationType=utf.CreditTransactionUserType
inner join[dbo].[OrganisationProgram] op on o.id  = op.organisationId
where utf.CreditTransactionUserType = @OrgType and op.programId = @ProgramId
 AND (@DateTime IS NULL OR  (Month(utf.TransactionDate) = MONTH(@DateTime)) AND YEAR(utf.TransactionDate) = YEAR(@DateTime))";

        public static readonly string GetOrgnisationListBasedOnUserRolePrgram = @"Select org.Id,
                                            org.Name from Organisation org 
                                            INNER JOIN OrganisationProgram orgPrg ON org.Id=OrgPrg.OrganisationId
                                            INNER JOIN Program prg ON prg.Id=orgPrg.ProgramId AND prg.IsActive=@IsActive AND prg.IsDeleted=@IsDeleted
                                            INNER JOIN [User] usr ON usr.ProgramId=orgPrg.ProgramId AND usr.Id=@UserId
                                            Where usr.IsActive=@IsActive AND usr.IsDeleted=@IsDeleted 
                                            AND org.IsActive=@IsActive AND org.IsDeleted=@IsDeleted 
                                            AND org.OrganisationType=@OrganisationType";

        public static readonly string GetOrganisationListBasedOnUserRoleMerchant = @"Select org.Id,
                                            org.Name from Organisation org
                                            INNER JOIN [User] usr ON usr.Id=@UserId
                                            INNER JOIN OrganisationProgram orgPrg1 ON orgPrg1.OrganisationId=usr.OrganisationId
                                            Where usr.IsActive=1 AND usr.IsDeleted=0 AND org.IsActive=1 AND org.IsDeleted=0 AND
                                            org.Id IN (Select OrganisationId from OrganisationProgram  oPrg Where oPrg.ProgramId=orgPrg1.ProgramId) 
                                            AND org.OrganisationType=@OrganisationType";
        #endregion

        #region Relation
        public static readonly string GetRelationsQuery = "SELECT ID,RelationName FROM UserRelations";

        public static readonly string GetRelationByIdQuery = @"SELECT ID,RelationName FROM UserRelations WHERE Id=@Id AND (IsActive = @IsActive or IsActive is null)" +
            "AND (IsDeleted = @IsDeleted or IsDeleted is null)";
        #endregion

        #region User And Benefactor
        public static readonly string AddUserRoleQuery = @"INSERT into UserRole VALUES (@UserId, @RoleId)";
        public static readonly string DeleteUserRoleQuery = @"Delete FROM UserRole WHERE UserId=@UserId";

        public static readonly string GetGroupByNameQuery = @"SELECT * FROM [Group]  WHERE Name = @Name";

        public static readonly string GetUserByEmailQuery = @"SELECT * FROM [User] WHERE LOWER(Email) = LOWER(@EmailAddress) AND (IsActive = @IsActive or IsActive is null)" +
            "AND (IsDeleted = @IsDeleted or IsDeleted is null)";

        public static readonly string GetBenefactorPhotoQuery = @"SELECT * FROM Photo WHERE EntityId = @EntityId AND PhotoType = @PhotoType";

        public static readonly string DeleteBenefactorPhotoQuery = @"DELETE FROM Photo WHERE Id = @Id";

        public static readonly string GetUserByIdQuery = @"SELECT * FROM [User] WHERE Id = @Id AND (IsActive = @IsActive or IsActive is null) " +
            "AND (IsDeleted = @IsDeleted or IsDeleted is null)";

        public static readonly string GetInvitationByUserBenefactorEmailQuery = @"SELECT * FROM Invitation WHERE LOWER(Email) = Lower(@EmailAddress) AND CreatedBy = @UserId AND (IsActive = @IsActive or IsActive is null)" +
            "AND (IsDeleted = @IsDeleted or IsDeleted is null)";

        public static readonly string UpdateInvitationByEmailQuery = @"UPDATE Invitation SET IsActive = @IsActive, IsDeleted = @IsDeleted, ModifiedBy = @ModifiedBy, ModifiedDate = @ModifiedDate WHERE Email = @Email";

        public static readonly string GetReloadRequestQuery = @"SELECT * FROM ReloadBalanceRequest WHERE UserId = @UserId AND BenefactorUserId = @BenefactorId AND (IsActive = @IsActive or IsActive is null)" +
            "AND (IsDeleted = @IsDeleted or IsDeleted is null)";

        public static readonly string UpdateReloadBalanceRequest = @"UPDATE ReloadBalanceRequest SET IsActive = @IsActive, IsDeleted = @IsDeleted WHERE Id = @Id";

        public static readonly string UpdateBenefactorUserLinkingQuery = @"UPDATE BenefactorUsersLinking SET IsActive = @IsActive, IsDeleted = @IsDeleted,IsRequestAccepted = @IsRequestAccepted, LinkedDateTime =@LinkedDateTime,  ModifiedBy = @ModifiedBy, ModifiedDate = @ModifiedDate, RelationshipId = @RelationshipId WHERE Id = @Id";

        public static readonly string AddBenefactorUserLinkingQuery = @"Insert into BenefactorUsersLinking (BenefactorId, CreatedBy, IsRequestAccepted, LinkedDateTime, ModifiedBy, RelationshipId, UserId)" +
            " values (@BenefactorId,@CreatedBy,@IsRequestAccepted,@LinkedDateTime,@ModifiedBy,@RelationshipId,@UserId);  SELECT CAST(SCOPE_IDENTITY() as int)";

        public static readonly string GetBenefactorNotificationQuery = @"SELECT u.FirstName AS FirstName,
        u.LastName AS LastName,
        IsInvitation = 1,
        UserId = u.Id,
        (u.FirstName + ' ' + u.LastName + ' sent a connection request.') AS [Message],
        ISNULL((SELECT PhotoPath from Photo WHERE EntityId = u.Id AND PhotoType = 1),'') AS ImagePath,
        inv.ModifiedDate AS ModifiedDate,
        inv.ProgramId AS ProgramId,
        0 AS ReloadRequestId 
        FROM Invitation AS inv
        INNER JOIN [User] AS u ON inv.CreatedBy = u.Id 
        WHERE inv.Email = @Email AND inv.IsRequestAccepted = 0 AND 
        inv.IsDeleted = 0 AND inv.IsActive = 1 AND u.IsActive = 1 AND u.IsDeleted = 0
        UNION
        SELECT u.FirstName AS FirstName,
        LastName = u.LastName,
        0 AS IsInvitation,
        u.Id AS UserId,
        (u.FirstName + ' ' + u.LastName + ' sent a reload request.') AS [Message],
        ISNULL((SELECT PhotoPath from Photo WHERE EntityId = u.Id AND PhotoType = 1),'') AS ImagePath,
        rl.ModifiedDate AS ModifiedDate,
        ProgramId = up.ProgramId,
        ReloadRequestId = rl.Id
        FROM ReloadBalanceRequest AS rl
        INNER JOIN [User] AS U ON rl.UserId = u.Id
        INNER JOIN UserProgram AS up ON rl.UserId = up.UserId AND rl.ProgramId = up.ProgramId
        WHERE rl.BenefactorUserId = @BenefactorId AND rl.IsRequestAccepted = 0 AND rl.IsDeleted = 0 AND rl.IsActive = 1 AND u.IsActive = 1 AND u.IsDeleted = 0";



        public static readonly string GetUserConnectionsQuery = @"SELECT 
        MobileNumber = i.PhoneNumber,
        EmailAddress =i.Email,
        FirstName =i.FirstName,
        LastName =i.LastName,
        BenefactorUserId =i.Id,
        BenefactorImage =ISNULL(i.ImagePath,''),
        IsInvitee = 1,
        RelationshipName = ur.relationName,
        IsReloadRequest = 0,
        CreationDate =i.ModifiedDate
        FROM Invitation AS i
        --LEFT JOIN Photo AS p ON p.EntityId = i.Id  AND phototype=5
        INNER JOIN UserRelations AS ur ON i.RelationshipId = ur.id AND ur.IsActive = 1
        where i.CreatedBy = @UserId
        AND i.IsActive = 1 AND i.IsDeleted = 0 ";



        //       public static readonly string GetUserConnectionsQuery = @"SELECT 
        //       MobileNumber = i.PhoneNumber,
        //       EmailAddress =i.Email,
        //       FirstName =i.FirstName,
        //       LastName =i.LastName,
        //       BenefactorUserId =i.Id,
        //       BenefactorImage =ISNULL(i.ImagePath,''),
        //       IsInvitee = 1,
        //       RelationshipName = ur.relationName,
        //       IsReloadRequest = 0,
        //       CreationDate =i.ModifiedDate
        //       FROM Invitation AS i
        //       --LEFT JOIN Photo AS p ON p.EntityId = i.Id  AND phototype=5
        //       INNER JOIN UserRelations AS ur ON i.RelationshipId = ur.id AND ur.IsActive = 1
        //       where i.CreatedBy = @UserId
        //       AND i.IsActive = 1 AND i.IsDeleted = 0 AND i.IsRequestAccepted = 0
        //       UNION

        //       SELECT MobileNumber = u.PhoneNumber,
        //       EmailAddress = u.Email,
        //       FirstName = u.FirstName,
        //       LastName = u.LastName,
        //       BenefactorUserId = u.Id,
        //       BenefactorImage = ISNULL(p.photoPath,''),
        //       IsInvitee = 0,
        //       RelationshipName = ur.RelationName,
        //       IsReloadRequest = CASE 
        //WHEN (
        //(SELECT Top(1) IsRequestAccepted FROM ReloadBalanceRequest AS rl WHERE rl.BenefactorUserId = bul.BenefactorId 
        //     AND rl.UserId=@UserId AND IsActive = 1 AND IsDeleted = 0 ORDER BY id DESC) 
        //	IS NULL) THEN 0
        //	WHEN (GETUTCDATE() >= (SELECT Top(1) DATEADD(MINUTE, Cast(11 as int), rl.ModifiedDate) 
        //			  FROM ReloadBalanceRequest AS rl WHERE rl.BenefactorUserId = bul.BenefactorId AND rl.UserId=@UserId  AND IsActive = 1 AND IsDeleted = 0 ORDER BY id DESC))
        //			  THEN 0
        //			  WHEN ((SELECT Top(1) IsRequestAccepted FROM ReloadBalanceRequest AS rl  WHERE rl.BenefactorUserId = bul.BenefactorId AND rl.UserId=@UserId AND IsActive = 1 AND IsDeleted = 0 ORDER BY id DESC) = 1)
        //			  THEN 0
        //			  ELSE
        //			    1 END,  
        //       CreationDate = bul.ModifiedDate--bu.LinkedDateTime
        //       FROM [User] AS u
        //       INNER JOIN BenefactorUsersLinking AS bul ON u.Id = bul.BenefactorId
        //       LEFT JOIN Photo AS p ON p.EntityId = u.Id AND phototype=5
        //       INNER JOIN UserRelations AS ur ON bul.RelationshipId = ur.Id AND ur.IsActive = 1
        //       LEFT JOIN BenefactorProgram AS bp ON u.Id = bp.BenefactorId AND bp.ProgramId = @ProgramId
        //       WHERE bul.UserId = @UserId AND bul.IsActive = 1 AND bul.IsDeleted = 0";

        public static readonly string GetLinkedUsersTransactionQuery = @"SELECT Amount =  CASE WHEN ut.AccountTypeId=3 THEN 
 '$'+convert(varchar,convert(decimal(8,2), ISNULL(ut.TransactionAmount,0)))
 WHEN ut.AccountTypeId=2 THEN 
 convert(varchar,convert(decimal(8,2), ISNULL(ut.TransactionAmount,0)))
 ELSE  convert(varchar,convert(decimal(8,0), ISNULL(ut.TransactionAmount,0)))+' passes'
 END,
        [Date] = Convert(varchar(10),ut.TransactionDate,103),
        [Time] = CONVERT(varchar(15),CAST(ut.TransactionDate AS TIME),100),
        Period = ut.PeriodRemark,
        PlanName = acct.AccountType,
        MerchantFullName = organisation.Name 
        FROM [User] AS u 
        INNER JOIN UserTransactionInfo AS ut ON u.Id = ut.DebitUserId AND ut.DebitTransactionUserType=1
        INNER JOIN UserProgram AS up ON ut.DebitUserId = up.UserId AND ut.ProgramId = up.ProgramId
        INNER JOIN AccountType AS acct ON ut.AccountTypeId = acct.Id
        
        LEFT JOIN Organisation AS organisation ON organisation.Id =ut.CreditUserId AND ut.CreditTransactionUserType=3
        WHERE u.Id = @linkedUserId AND ut.DebitUserId = @LinkedUserId AND ut.CreditUserId != @LinkedUserId AND
        (@DateMonth IS NULL OR (Month(ut.TransactionDate) = MONTH(@DateMonth)) AND YEAR(ut.TransactionDate) = YEAR(@DateMonth))
        AND (@Plan = '' OR @Plan IS NULL OR acct.AccountType = @Plan)";

        public static readonly string GetLinkedUsersOfBenefactorQuery = @"SELECT 
        linkedUserId = u.Id,
        UserFirstName = u.FirstName,
        UserLastName = u.LastName,
        ImageUserPath = ISNULL((SELECT PhotoPath FROM Photo WHERE EntityId = u.Id AND PhotoType = @ImageType), ''),
        CanViewTransaction=bu.CanViewTransaction
        FROM BenefactorUsersLinking AS bu 
        INNER JOIN [User] AS u ON bu.UserId = u.Id 
        WHERE bu.BenefactorId = @BenefactorId AND bu.IsActive = 1 AND bu.IsDeleted = 0";

        public static readonly string BenefectorDetails = @"SELECT 
        linkedUserId = u.Id,
        UserFirstName = u.FirstName,
        UserLastName = u.LastName,
        EmailAddress= u.email  
        from [User] u
		inner join BenefactorUsersLinking AS bu 
		on bu.benefactorId=u.Id
		where bu.userId=@BenefactorId and bu.IsActive = 1 AND bu.IsDeleted = 0";


        public static readonly string GetLinkedUsersInformationOfBenefactorQuery = @"SELECT linkedUserId = u.Id,
        UserFirstName = u.FirstName,
        UserLastName = u.LastName,
        ImageUserPath = ISNULL((SELECT PhotoPath FROM Photo WHERE EntityId = u.Id AND PhotoType = @ImageType), ''),
        PhoneNumber = u.PhoneNumber,
        EmailAddress = u.Email,
        UserCode = u.UserCode,
        DateAdded = Convert(varchar(10),bu.CreatedDate,103),
        CanViewTransaction=bu.CanViewTransaction,
        School = o.Name
		FROM BenefactorUsersLinking AS bu
		INNER JOIN [User] AS u ON bu.UserId = u.Id
		LEFT JOIN Organisation AS o ON u.OrganisationId = o.Id
		WHERE bu.BenefactorId = @BenefactorId AND bu.IsActive = 1 AND bu.IsDeleted = 0";

        public static readonly string GetLinkedUsersInformationOfBenefactorWithPrivacySettingQuery = @"SELECT linkedUserId = u.Id,
        UserFirstName = u.FirstName,
        UserLastName = u.LastName,
        ImageUserPath = ISNULL((SELECT PhotoPath FROM Photo WHERE EntityId = u.Id AND PhotoType = @ImageType), NULL),
        PhoneNumber = u.PhoneNumber,
        EmailAddress = u.Email,
        UserCode = u.UserCode,
        DateAdded = Convert(varchar(10),bu.CreatedDate,103),
        School = o.Name
		FROM BenefactorUsersLinking AS bu
		INNER JOIN [User] AS u ON bu.UserId = u.Id
		LEFT JOIN Organisation AS o ON u.OrganisationId = o.Id
		WHERE bu.BenefactorId = @BenefactorId AND bu.IsActive = 1 AND bu.IsDeleted = 0 AND bu.CanViewTransaction=1";

        public static readonly string GetRemainingBalanceOfUserQuery = @"Select RemainingBalance = ISNULL(ISNULL((SELECT SUM(TransactionAmount) FROM UserTransactionInfo WHERE CreditUserId = @UserId AND AccountTypeId = 3),0)
        -ISNULL((SELECT SUM(TransactionAmount) FROM UserTransactionInfo WHERE DebitUserId = @UserId AND CreditUserId != @UserId AND AccountTypeId = 3),0),0)";

        public static readonly string GetReloadRuleOfUserQuery = @"Select * from ReloadRules WHERE UserId = @UserId AND BenefactorUserId = @BenefactorId AND IsActive = 1 AND IsDeleted = 0";
        public static readonly string GetReloadRuleUserQuery = @"Select * from ReloadRules WHERE UserId = @UserId  AND IsActive = 1 AND IsDeleted = 0";
        public static readonly string GetReloadRuleForTriggerQuery = @"Select top 1 * from ReloadRules WHERE UserId = @UserId  AND userDroppedAmount>= @Balance AND IsActive = 1 AND IsDeleted = 0";
        public static readonly string GetAllReloadRuleOfUserQuery = @"SELECT r.id, u.UserName +' '+ ISNULL(u.LastName ,'') CreatedbyName , r.userDroppedAmount, r.reloadAmount
                                    FROM dbo.ReloadRules r JOIN dbo.[User] u ON u.Id = r.benefactorUserId 
                                        WHERE  r.isDeleted=0 AND r.isActive=1 AND r.userId=  @UserId";



        public static readonly string GetPrivacySettingQuery = @"Select Id = bul.id,MobileNumber = u.PhoneNumber,
        EmailAddress = u.Email,
        FirstName = u.FirstName,
        LastName = u.LastName,
        BenefactorUserId = u.Id,
        BenefactorImage = ISNULL(p.photoPath,NULL),
        RelationshipName = ur.RelationName,
        CanViewTransaction = bul.canViewTransaction,
        CreationDate = bul.ModifiedDate from BenefactorUsersLinking AS bul
		INNER JOIN [user] AS u ON bul.BenefactorId = u.Id 
		INNER JOIN UserRelations AS ur ON bul.RelationshipId = ur.Id AND ur.IsActive = 1
		LEFT JOIN photo AS p ON bul.BenefactorId = p.EntityId AND p.photoType = 5
		LEFT JOIN BenefactorProgram AS bp ON u.Id = bp.BenefactorId AND bp.ProgramId = @ProgramId
		Where bul.UserId = @UserId AND bul.IsActive = 1 AND bul.IsDeleted = 0 AND bul.IsRequestAccepted = @IsRequestAccepted";
        #endregion

        #region Organisation
        public static readonly string GetOrganisationQuery = @"SELECT o.id,o.name,'' as ClosingStatus, dbo.fn_getDistance(@Latlon,o.location) as Distance
        ,ISNULL(Img.PhotoPath,NULL) AS LogoPath
        ,ISNULL(uf.isFavorite,0) as IsFavorite,o.OrganisationSubTitle,o.IsClosed,o.isTrafficChartVisible FROM Organisation AS o
        INNER JOIN OrganisationProgram AS op ON o.Id = op.OrganisationId AND op.ProgramId = @ProgramId
        INNER JOIN UserProgram AS up on op.ProgramId = up.ProgramId AND up.UserId = @UserId
        INNER JOIN ProgramMerchantAccountType AS pmat ON o.Id = pmat.OrganisationId
		INNER JOIN ProgramAccountLinking AS pal ON pal.Id = pmat.ProgramAccountLinkingId AND pal.ProgramId = up.ProgramId AND pal.AccountTypeId = @AccountTypeId
		--INNER JOIN ProgramAccounts pa ON pa.ProgramId=pal.ProgramId And pa.IsDeleted = @IsDeleted AND  pa.IsActive = @IsActive AND pa.AccountTypeId = @AccountTypeId
       -- INNER JOIN AccountMerchantRules amr ON amr.accountTypeId=@AccountTypeId AND amr.programAccountID=pa.id AND amr.merchantId=op.organisationId
        LEFT JOIN Photo AS img ON o.Id=img.EntityId AND img.PhotoType= " + (int)PhotoEntityType.Organisation +
        "LEFT JOIN UserFavorites AS uf ON uf.userId = up.UserId AND uf.OrgnisationId = o.Id "
        + "WHERE o.IsActive = @IsActive AND o.OrganisationType = @OrganisationType  AND (o.IsDeleted = @IsDeleted OR o.IsDeleted is null)";

        public static readonly string GetOrganisationDetailByIdQuery = @"SELECT o.id,o.name,'' as ClosingStatus,o.AddressLine1,o.AddressLine2,o.Description,O.MaxCapacity,o.Location,
        o.OrganisationSubTitle,
o.City,
o.[State],
o.Zip,
o.Country,
o.IsMapVisible,
o.IsClosed,
o.dwellTime,
o.isTrafficChartVisible,
dbo.GetOpenCloseTimeForOrganisation(o.id) AS OpenCloseTime
        ,ISNULL(Img.PhotoPath,NULL) AS LogoPath
        ,ISNULL(uf.isFavorite,0) as IsFavorite  FROM Organisation AS o
        INNER JOIN OrganisationProgram AS op ON o.Id = op.OrganisationId AND op.ProgramId = @ProgramId
        INNER JOIN UserProgram AS up on op.ProgramId = up.ProgramId AND up.UserId = @UserId
        INNER JOIN ProgramMerchantAccountType AS pmat ON o.Id = pmat.OrganisationId
		INNER JOIN ProgramAccountLinking AS pal ON pal.Id = pmat.ProgramAccountLinkingId AND pal.ProgramId = up.ProgramId AND pal.AccountTypeId = @AccountTypeId
		--INNER JOIN ProgramAccounts pa ON pa.ProgramId=pal.ProgramId And pa.IsDeleted = @IsDeleted AND  pa.IsActive = @IsActive AND pa.AccountTypeId = @AccountTypeId
       -- INNER JOIN AccountMerchantRules amr ON amr.accountTypeId=@AccountTypeId AND amr.programAccountID=pa.id AND amr.merchantId=op.organisationId

        LEFT JOIN Photo AS img ON o.Id=img.EntityId AND img.PhotoType= " + (int)PhotoEntityType.Organisation +
       "LEFT JOIN UserFavorites AS uf on uf.userId = up.UserId AND uf.OrgnisationId = o.Id "
       + "WHERE o.IsActive = @IsActive AND o.OrganisationType = @OrganisationType  AND (o.IsDeleted = @IsDeleted OR o.IsDeleted is null) AND o.Id = @Id";

        public static readonly string GetOrganisationScheduleByIdQuery = @"SELECT id, organisationId,workingDay, openTime, closedTime, isActive, createdBy
,createdDate, modifiedBy,modifiedDate, isDeleted FROM OrganisationSchedule WHERE (IsActive = @IsActive OR IsActive is null) AND OrganisationId = @Id " +
            "AND (IsDeleted = @IsDeleted OR IsDeleted is null)";

        public static readonly string GetOrganisationScheduleByIdSP = @"GetOrgScheduleHours";

        public static readonly string GetOrganisationListByTypeQuery = @"SELECT 
                                                                            Id,
                                                                            Name,
                                                                            AddressLine1,
                                                                            AddressLine2,
                                                                            Location,
                                                                            EmailAddress,
                                                                            ContactNumber,
                                                                            [Description],
                                                                            WebsiteURL,
                                                                            OrganisationSubTitle,
                                                                            JPOS_MerchantId
                                                                             FROM Organisation WHERE ISACTIVE=@IsActive AND ISDELETED=@IsDeleted 
                                                                             AND ORGANISATIONTYPE=@OrganisationType AND (@SearchName IS NOT NULL AND @SearchName!='' AND (OrganisationSubTitle LIKE  @SearchName OR NAME LIKE @SearchName))
                                                                             AND (@RoleName='super admin' OR 
                                                                             (Id in (Select organisationId from [User] usr
                                                                               INNER JOIN UserRole ur ON usr.Id=ur.UserId
                                                                                INNER JOIN [Role] r on r.Id=ur.RoleId and r.Name=@RoleName
                                                                                   WHERE usr.Id=@UserId AND usr.IsActive=1 AND usr.IsDeleted=0)))";
        #endregion

        #region Merchant Offer
        public static readonly string GetOffersOfMerchantsQuery = @"SELECT 
o.Name AS OrganisationName,
o.Id AS OrganisationId,
ofr.Id AS OfferId,
ofr.OfferTypeId AS OfferTypeId,
ofr.OfferSubTypeId AS OfferSubTypeId,
oc.Id AS BannnerTypeId,
ofr.Name AS OfferText,
ISNULL(Img.PhotoPath,NULL) AS OfferImagePath,
ofr.BannerDescription as OfferValue,
dbo.fn_getDistance(@Latlon,o.location) as Distance,
ofr.StartDate,
ofr.EndDate,
ofr.StartTime,
ofr.EndTime,
o.IsClosed,
o.isTrafficChartVisible
 FROM Promotion AS ofr
             INNER JOIN OfferType AS ot ON ofr.OfferTypeId = ot.Id
        INNER JOIN OfferSubType AS ost ON ofr.OfferSubTypeId = ost.Id
        INNER JOIN OfferCode AS oc ON ofr.BannerTypeId = oc.Id
        INNER JOIN Organisation AS o ON ofr.MerchantId = o.Id  and o.organisationType=3
        INNER JOIN OrganisationProgram AS op ON o.Id = op.OrganisationId AND op.ProgramId = @ProgramId
        INNER JOIN UserProgram AS up on op.ProgramId = up.ProgramId AND up.UserId = @UserId
        INNER JOIN ProgramMerchantAccountType AS pmat ON o.Id = pmat.OrganisationId
		INNER JOIN ProgramAccountLinking AS pal ON pal.Id = pmat.ProgramAccountLinkingId AND pal.ProgramId = up.ProgramId AND pal.AccountTypeId = @AccountTypeId
		--INNER JOIN ProgramAccounts pa ON pa.ProgramId=pal.ProgramId And pa.IsDeleted = @IsDeleted AND  pa.IsActive = @IsActive AND pa.AccountTypeId = @AccountTypeId
       -- INNER JOIN AccountMerchantRules amr ON amr.accountTypeId=@AccountTypeId AND amr.programAccountID=pa.id AND amr.merchantId=op.organisationId

        LEFT JOIN Photo AS img ON ofr.Id=img.EntityId AND img.PhotoType= " + (int)PhotoEntityType.OffersPromotions +
        "WHERE o.IsActive = @IsActive AND (o.IsDeleted =@IsDeleted OR o.IsDeleted = null) AND pal.AccountTypeId =@AccountTypeId AND ofr.IsActive=1 AND ofr.IsDeleted=0" +

      "AND GETUTCDATE() BETWEEN DATEADD(Hour, DATEDIFF(Hour, GETUTCDATE(), GETDATE()), Convert(DateTime, ofr.StartDate) ) AND DATEADD(Hour, DATEDIFF(Hour, GETUTCDATE(), GETDATE()),  Convert(DateTime, ofr.EndDate)) ";

        public static readonly string GetOffersOfMerchantsDetailByIdQuery = @"SELECT
o.Name AS OrganisationName,
o.Id AS OrganisationId,
ofr.Id AS OfferId,
ofr.OfferTypeId AS OfferTypeId,
ofr.OfferSubTypeId AS OfferSubTypeId,
oc.Id AS BannnerTypeId,
ofr.Name AS OfferText,
ISNULL(Img.PhotoPath,NULL) AS OfferImagePath,
ofr.BannerDescription as OfferValue,
o.AddressLine1,
o.AddressLine2,
ofr.[Description],
o.Location,
o.City,
o.[State],
o.Zip,
o.Country,
o.IsMapVisible,
ofr.StartDate,
ofr.EndDate,
ofr.StartTime,
ofr.EndTime,
o.IsClosed,
o.isTrafficChartVisible
FROM Promotion AS ofr
        INNER JOIN OfferType AS ot ON ofr.OfferTypeId = ot.Id
        INNER JOIN OfferSubType AS ost ON ofr.OfferSubTypeId = ost.Id
        INNER JOIN OfferCode AS oc ON ost.OfferCodeId = oc.Id
        INNER JOIN Organisation AS o ON ofr.MerchantId = o.Id
        INNER JOIN OrganisationProgram AS op ON o.Id = op.OrganisationId AND op.ProgramId = @ProgramId
        INNER JOIN UserProgram AS up on op.ProgramId = up.ProgramId AND up.UserId = @UserId
       INNER JOIN ProgramMerchantAccountType AS pmat ON o.Id = pmat.OrganisationId
		INNER JOIN ProgramAccountLinking AS pal ON pal.Id = pmat.ProgramAccountLinkingId AND pal.ProgramId = up.ProgramId AND pal.AccountTypeId = @AccountTypeId
	--	INNER JOIN ProgramAccounts pa ON pa.ProgramId=pal.ProgramId And pa.IsDeleted = @IsDeleted AND  pa.IsActive = @IsActive AND pa.AccountTypeId = @AccountTypeId
      --  INNER JOIN AccountMerchantRules amr ON amr.accountTypeId=@AccountTypeId AND amr.programAccountID=pa.id AND amr.merchantId=op.organisationId

        LEFT JOIN Photo AS img ON ofr.Id=img.EntityId AND img.PhotoType= " + (int)PhotoEntityType.OffersPromotions +
       " WHERE o.IsActive = @IsActive AND (o.IsDeleted = @IsDeleted OR o.IsDeleted = null) AND pal.AccountTypeId = @AccountTypeId" +
            "  AND ofr.Id = @OfferId AND ofr.IsActive= 1 AND ofr.IsDeleted= 0" +

       " AND GETUTCDATE() BETWEEN DATEADD(Hour, DATEDIFF(Hour, GETUTCDATE(), GETDATE()), Convert(DateTime, ofr.StartDate)) AND DATEADD(Hour, DATEDIFF(Hour, GETUTCDATE(), GETDATE()),  Convert(DateTime, ofr.EndDate)) ";

        public static readonly string GetNoOfRemainingMealPassesQuery = @"SELECT pp.NoOfMealPasses - SUM(ISNULL(ut.TransactionAmount, 0)) FROM ProgramPackage AS pp
        INNER JOIN Program AS p ON pp.ProgramId = p.Id
        INNER JOIN UserProgram AS up ON up.ProgramId = @ProgramId AND up.UserId = @UserId
        LEFT JOIN UserTransactionInfo AS ut ON ut.ProgramId = @ProgramId AND ut.DebitUserId = @UserId AND ut.AccountTypeId = 1
        WHERE p.Id = @ProgramId AND p.IsActive = @IsActive AND p.IsDeleted = @IsDeleted
        GROUP BY pp.NoOfMealPasses, ut.TransactionAmount";

        public static readonly string GetAllMerchantsByProgram = @"SELECT 
	    o.id AS Id,	   
	    o.name AS MerchantName
	     FROM Organisation AS o
       INNER JOIN OrganisationProgram op ON op.organisationId=o.id
        WHERE o.isDeleted = @IsDeleted AND o.isActive = @IsActive AND o.organisationType = @OrganisationType 
        AND op.programId = @programId";

        public static readonly string GetAllMerchantsWithTransactionsQuery = @"SELECT 
	    o.id AS Id,
	    o.MerchantId AS MerchantId, 
	    o.name AS MerchantName, 
	    o.addressLine1 AS Location, 
	    uti1.transactionDate AS LastTransaction,
	    o.createdDate AS DateAdded,
	    AccountType = STUFF  
	    (  
            (  
                SELECT '<br />' + at.accountType  
                FROM AccountType AS at   
                WHERE at.id in (SELECT pal.accountTypeId FROM ProgramAccountLinking AS pal INNER JOIN ProgramMerchantAccountType AS pmat ON pmat.organisationId = o.id AND pal.id = pmat.programAccountLinkingId AND pal.ProgramId=@ProgramId)
                FOR XML PATH('')  
            ),1,1,''  
	    ), 
	    dbo.fn_getMerchantActivity(o.id) AS Activity,
        o.Jpos_MerchantId
        FROM Organisation AS o
        INNER JOIN OrganisationProgram AS op ON op.organisationId = o.id
        LEFT OUTER JOIN UserTransactionInfo uti1 ON o.id = uti1.creditUserId
        LEFT OUTER JOIN UserTransactionInfo uti2 ON (uti1.creditUserId = uti2.creditUserId AND uti1.transactionDate < uti2.transactionDate)
        WHERE o.isDeleted = @IsDeleted AND o.isActive = @IsActive AND o.organisationType = @OrganisationType AND uti2.debitUserId IS NULL
        AND op.programId = @ProgramId
        GROUP BY o.id, o.name, o.addressLine1, uti1.transactionDate, o.MerchantId, o.createdDate,o.Jpos_MerchantId";

        public static readonly string GetAllMerchantsWithMerchantAdminQuery = @"SELECT 
	    o.id AS Id,
	    o.MerchantId AS MerchantId, 
	    o.name AS MerchantName, 
	    o.addressLine1 AS Location, 
	    uti1.transactionDate AS LastTransaction,
	    o.createdDate AS DateAdded,
	    AccountType = STUFF  
	    (  
            (  
                SELECT '<br />' + at.accountType  
                FROM AccountType AS at   
                WHERE at.id in (SELECT pal.accountTypeId FROM ProgramAccountLinking AS pal INNER JOIN ProgramMerchantAccountType AS pmat ON pmat.organisationId = o.id AND pal.id = pmat.programAccountLinkingId)
                FOR XML PATH('')  
            ),1,1,''  
	    ), 
	    dbo.fn_getMerchantActivity(o.id) AS Activity FROM Organisation AS o
        INNER JOIN OrganisationProgram AS op ON op.organisationId = o.id
        INNER JOIN MerchantAdmins ma ON ma.MerchantId=o.Id
		INNER JOIN [User] u ON u.Id=ma.AdminUserId
        LEFT OUTER JOIN UserTransactionInfo uti1 ON o.id = uti1.creditUserId
        LEFT OUTER JOIN UserTransactionInfo uti2 ON (uti1.creditUserId = uti2.creditUserId AND uti1.transactionDate < uti2.transactionDate)
        WHERE o.isDeleted = @IsDeleted AND o.isActive = @IsActive AND o.organisationType = @OrganisationType AND uti2.debitUserId IS NULL
        AND op.programId = @ProgramId AND u.Id=@UserId
        GROUP BY o.id, o.name, o.addressLine1, uti1.transactionDate, o.MerchantId, o.createdDate";

        public static readonly string GetAllMerchantsForMerchantAdminQuery = @"SELECT 
	    o.id AS Id,
	    o.MerchantId AS MerchantId, 
	    o.name AS MerchantName, 
	    o.addressLine1 AS Location, 
	    uti1.transactionDate AS LastTransaction,
	    o.createdDate AS DateAdded,
	    AccountType = STUFF  
	    (  
            (  
                SELECT '<br />' + at.accountType  
                FROM AccountType AS at   
                WHERE at.id in (SELECT pal.accountTypeId FROM ProgramAccountLinking AS pal INNER JOIN ProgramMerchantAccountType AS pmat ON pmat.organisationId = o.id AND pal.id = pmat.programAccountLinkingId)
                FOR XML PATH('')  
            ),1,1,''  
	    ), 
	    dbo.fn_getMerchantActivity(o.id) AS Activity FROM Organisation AS o
        INNER JOIN OrganisationProgram AS op ON op.organisationId = o.id
		INNER JOIN MerchantAdmins ma ON ma.MerchantId=o.Id
		INNER JOIN [User] u ON u.Id=ma.AdminUserId
        LEFT OUTER JOIN UserTransactionInfo uti1 ON o.id = uti1.creditUserId
        LEFT OUTER JOIN UserTransactionInfo uti2 ON (uti1.creditUserId = uti2.creditUserId AND uti1.transactionDate < uti2.transactionDate)
        WHERE o.isDeleted = @IsDeleted AND o.isActive = @IsActive AND o.organisationType = @OrganisationType AND uti2.debitUserId IS NULL
        AND op.programId = @ProgramId AND u.Id=@UserId
        GROUP BY o.id, o.name, o.addressLine1, uti1.transactionDate, o.MerchantId, o.createdDate";

        public static readonly string GetAllMerchantsByMerchantAdminIdQuery = @"SELECT 
	    o.id AS Id,
	    o.MerchantId AS MerchantId, 
	    o.name AS MerchantName, 
	    o.addressLine1 AS Location, 
	    uti1.transactionDate AS LastTransaction,
	    o.createdDate AS DateAdded,
	    AccountType = STUFF  
	    (  
            (  
                SELECT '<br />' + at.accountType  
                FROM AccountType AS at   
                WHERE at.id in (SELECT pal.accountTypeId FROM ProgramAccountLinking AS pal INNER JOIN ProgramMerchantAccountType AS pmat ON pmat.organisationId = o.id AND pal.id = pmat.programAccountLinkingId)
                FOR XML PATH('')  
            ),1,1,''  
	    ), 
	    dbo.fn_getMerchantActivity(o.id) AS Activity FROM Organisation AS o
        INNER JOIN OrganisationProgram AS op ON op.organisationId = o.id
		INNER JOIN MerchantAdmins ma ON ma.MerchantId=o.Id
		INNER JOIN [User] u ON u.Id=ma.AdminUserId
        LEFT OUTER JOIN UserTransactionInfo uti1 ON o.id = uti1.creditUserId
        LEFT OUTER JOIN UserTransactionInfo uti2 ON (uti1.creditUserId = uti2.creditUserId AND uti1.transactionDate < uti2.transactionDate)
        WHERE o.isDeleted = @IsDeleted AND o.isActive = @IsActive AND o.organisationType = @OrganisationType AND uti2.debitUserId IS NULL
        AND u.Id=@UserId
        GROUP BY o.id, o.name, o.addressLine1, uti1.transactionDate, o.MerchantId, o.createdDate";

        public static readonly string GetAllMerchantsTransactionsQuery = @"SELECT uti.id AS Id, o.MerchantId AS MerchantId, uti.transactionDate AS TransactionDate, at.accountType AS Account,
        o.name AS Name, Amount= CASE WHEN uti.AccountTypeId=3 THEN 
 '$'+convert(varchar,convert(decimal(8,2), ISNULL(uti.TransactionAmount,0)))
 WHEN uti.AccountTypeId=2 THEN 
 convert(varchar,convert(decimal(8,2), ISNULL(uti.TransactionAmount,0)))
 ELSE  convert(varchar,convert(decimal(8,0), ISNULL(uti.TransactionAmount,0)))+' passes'
 END 
        FROM UserTransactionInfo AS uti 
        INNER JOIN AccountType AS at ON uti.AccountTypeId = at.Id
        
        INNER JOIN Organisation AS o ON o.Id = uti.creditUserId AND o.organisationType = @OrganisationType AND uti.CreditTransactionUserType=3
        WHERE o.Id = @OrganisationId AND (@DateMonth IS NULL OR (Month(uti.TransactionDate) = MONTH(@DateMonth)) AND YEAR(uti.TransactionDate) = YEAR(@DateMonth))";

        public static readonly string GetMerchantDetailInformationWithMasterQuery = @"SELECT prg1.Id, prg1.Name FROM Program prg1 
                                                                                      INNER JOIN OrganisationProgram oprg1 ON oprg1.ProgramId=prg1.Id AND oprg1.IsActive=1 AND oprg1.IsDeleted=0
                                                                                      INNER JOIN Organisation org1 ON org1.Id=oprg1.OrganisationId AND org1.IsActive=1 AND org1.IsDeleted=0
                                                                                      WHERE prg1.IsActive = 1 AND prg1.IsDeleted = 0 AND org1.Id= @UniversityId    /* All programs that are created under the organisation (i.e. university)*/

                                                                                        SELECT Id, AccountType FROM AccountType WHERE IsDeleted = 0   /*All Account Types (Master)*/

                                                                                        SELECT Id, Name, IconPath FROM BusinessType WHERE IsActive = 1 AND IsDeleted = 0 /*All Business Types (Master)*/
                                                                                        
                                                                                        SELECT
                                                                                        o.Id AS OrganisationId,
                                                                                        Name AS OrganisationName,
                                                                                        [AddressLine1] AS[Address],
                                                                                        City,
                                                                                        Zip,
                                                                                        [State],
                                                                                        Country,
                                                                                        ContactNumber,
                                                                                        WebsiteURL AS Website,
                                                                                        [Description],
                                                                                        FacebookURL,
                                                                                        TwitterURL,
                                                                                        SkypeHandle,
                                                                                        InstagramHandle,
                                                                                        BusinessTypeId,
                                                                                        IsMapVisible AS ShowMap,
																						IsNULL(p.photoPath,NULL) AS ImagePath,
                                                                                        location as Location,
                                                                                        Jpos_MerchantId
                                                                                        FROM Organisation AS o
																						LEFT JOIN Photo AS p ON o.id = p.entityId AND p.photoType = 2 
                                                                                        WHERE o.OrganisationType = @OrganisationType AND o.Id = @OrganisationId AND IsActive=1 AND IsDeleted=0
                                                                                                
                                                                                        SELECT OrganisationId, ProgramId,IsPrimaryAssociation FROM OrganisationProgram 
                                                                                        WHERE OrganisationId = @OrganisationId AND IsActive=1 AND IsDeleted=0  /* All organisation programs that are selected in merchant*/

                                                                                        SELECT pal.ProgramId, pal.AccountTypeId AS Id FROM ProgramAccountLinking AS pal
                                                                                        INNER JOIN ProgramMerchantAccountType AS pmat ON pal.Id = pmat.ProgramAccountLinkingId WHERE pmat.OrganisationId = @OrganisationId ANd pal.programId=@ProgramId
                                                                                        /* The above query is getting the account type which have been selected  under merhant*/
                                                                                        ";

        public static readonly string GetPrimaryOrgNPrgDetailOfMerchantAdminQuery = @"Select Top(1)
org1.Id as MerchantId,
                                                    org.Id as PrimaryOrganisationId,
                                                    org.name as PrimaryOrgName,
                                                    org.OrganisationSubTitle as PrimaryOrgSubtitle,
                                                    prg.id as PrimaryProgramId,
                                                    prg.name as PrimaryProgramName,
(Select Count(ma1.id) from merchantadmins ma1 INNER JOIN Organisation mer ON mer.id=ma1.merchantId Where ma1.adminUserId=@UserId AND mer.IsActive=@IsActive AND mer.IsDeleted=@IsDeleted) as TotalMerchantInAdmin
                                                    from MerchantAdmins ma 
                                                    INNER JOIN OrganisationProgram orgPrg ON orgPrg.organisationId=ma.merchantId
INNER JOIN Organisation org1 ON org1.Id=ma.merchantId AND org1.IsActive=1 AND org1.IsDeleted=0
                                                    INNER JOIN Organisation org ON org.organisationType=2 
                                                    INNER JOIN OrganisationProgram orgPrg1 ON orgPrg1.organisationId=org.Id AND orgPrg.programId=orgPrg1.programId
                                                    INNER JOIN Program prg ON prg.Id=orgPrg1.programId
                                                    WHERE ma.adminUserId=@UserId AND org.IsActive=@IsActive AND org.IsDeleted=@IsDeleted
                                                    AND prg.IsActive=@IsActive AND prg.IsDeleted=@IsDeleted";

        public static readonly string GetPrimaryOrgNPrgDetailOfProgramAdminQuery = @"Select Top(1)
                                                    org.Id as PrimaryOrganisationId,
                                                    org.name as PrimaryOrgName,
                                                    org.OrganisationSubTitle as PrimaryOrgSubtitle,
                                                    prg.id as PrimaryProgramId,
                                                    prg.name as PrimaryProgramName,
(Select Count(pa1.id) from programadmins pa1 INNER JOIN Program prg1 ON prg1.id=pa1.programId Where pa1.adminUserId=@UserId AND prg1.IsActive=@IsActive AND prg1.IsDeleted=@IsDeleted) as TotalProgramInAdmin
                                                    from ProgramAdmins pa
                                                    INNER JOIN Program prg ON prg.Id=pa.programId
                                                    INNER JOIN OrganisationProgram orgPrg ON orgPrg.programId=prg.Id
                                                    INNER JOIN Organisation org ON org.organisationType=2 AND org.Id=orgPrg.organisationId
                                                                                                       
                                                    WHERE pa.adminUserId=@UserId AND org.IsActive=@IsActive AND org.IsDeleted=@IsDeleted
                                                    AND prg.IsActive=@IsActive AND prg.IsDeleted=@IsDeleted";
        public static readonly string GetMerchantRewardInformationWithMasterQuery = @"SELECT Id, Name, IconPath FROM BusinessType WHERE IsActive = 1 AND IsDeleted = 0

SELECT id AS Id, title AS Title FROM OfferSubType WHERE OfferTypeId = @OfferTypeId AND IsDeleted = 0

SELECT p.id AS ID,
p.offerTypeId AS OfferTypeId,
p.offerSubTypeId AS OfferSubTypeId,
p.name AS RewardTitle,
p.bannerDescription AS RewardSubTitle,
p.description AS [Description],
p.startDate AS StartDate,
p.startTime AS StartTime,
p.endDate AS EndDate,
p.endTime AS EndTime,
p.MerchantId,
p.noOfVisits AS Visits,
p.amounts AS Amount,
p.businessTypeId AS BusinessTypeId,
p.backgroundColor AS BackGroundColor,
p.isActive AS IsActive,
bt.iconPath AS BusinessIconPath,
p.IsPublished FROM Promotion AS p LEFT JOIN BusinessType AS bt ON p.businessTypeId = bt.id Where p.MerchantId = @MerchantId AND p.isDeleted = @IsDeleted";

        public static readonly string GetMerchantRewardListQuery = @"SELECT p.id AS ID,
p.offerTypeId AS OfferTypeId,
p.offerSubTypeId AS OfferSubTypeId,
p.name AS RewardTitle,
p.bannerDescription AS RewardSubTitle,
p.description AS [Description],
p.startDate AS StartDate,
p.startTime AS StartTime,
p.endDate AS EndDate,
p.endTime AS EndTime,
p.MerchantId,
p.noOfVisits AS Visits,
p.amounts AS Amoount,
p.businessTypeId AS BusinessTypeId,
p.backgroundColor AS BackGroundColor,
p.isActive AS IsActive,
p.IsPublished AS IsPublished,
bt.iconPath AS BusinessIconPath FROM Promotion AS p LEFT JOIN BusinessType AS bt ON p.businessTypeId = bt.id Where p.MerchantId = @MerchantId AND p.OfferTypeId = @OfferTypeId AND p.isDeleted = @IsDeleted";

        public static readonly string DeleteOrganisationAccountTypeQuery = @"DELETE FROM ProgramMerchantAccountType WHERE ProgramAccountLinkingId IN(SELECT pal.id FROM ProgramAccountLinking AS pal
                                                                             INNER JOIN ProgramMerchantAccountType AS pmat ON pal.Id = pmat.ProgramAccountLinkingId
                                                                             WHERE pal.programId = @ProgramId AND pmat.organisationId = @OrganisationId) AND organisationId = @OrganisationId";

        public static readonly string AddUpdateOrganisationAccountTypeQuery = @"INSERT INTO ProgramMerchantAccountType values (@OrganisationId,(SELECT pal.id FROM ProgramAccountLinking AS pal WHERE pal.programId = @ProgramId AND pal.accountTypeId = @AccountTypeId))";

        public static readonly string GetMerchantBusinessInfoQuery = @"SELECT id AS Id,
organisationId AS OrganisationId,
workingDay AS WorkingDay,
openTime AS OpenTime,
closedTime AS ClosedTime,
isActive AS IsActive,
createdBy AS CreatedBy,
createdDate AS CreatedDate,
modifiedBy AS ModifiedBy,
modifiedDate AS ModifiedDate,
isDeleted AS IsDeleted,
isHoliday AS IsHoliday,
holidayDate AS HolidayDate,
HolidayName as HolidayName,
IsForHolidayNameToShow as IsForHolidayNameToShow
FROM OrganisationSchedule WHERE isActive = 1 AND isDeleted = 0 AND organisationId = @OrganisationId AND (isHoliday = 0 OR isHoliday is NULL) 

SELECT id AS Id,
organisationId AS OrganisationId,
workingDay AS WorkingDay,
openTime AS OpenTime,
closedTime AS ClosedTime,
isActive AS IsActive,
createdBy AS CreatedBy,
createdDate AS CreatedDate,
modifiedBy AS ModifiedBy,
modifiedDate AS ModifiedDate,
isDeleted AS IsDeleted,
isHoliday AS IsHoliday,
holidayDate AS HolidayDate,
HolidayName as HolidayName,
IsForHolidayNameToShow as IsForHolidayNameToShow
FROM OrganisationSchedule WHERE isActive = 1 AND isDeleted = 0 AND organisationId = @OrganisationId AND isHoliday = 1

SELECT maxCapacity AS MaxCapacity, dwellTime AS DwellTime, MerchantId, isClosed,isTrafficChartVisible FROM Organisation WHERE id = @OrganisationId

SELECT id, terminalId, terminalName, terminalType, organisationId,Jpos_TerminalId FROM MerchantTerminal WHERE organisationId = @OrganisationId

SELECT id, title, organisationId, opentime, days, isSelected, closeTime FROM OrganisationMealPeriod WHERE organisationId = @OrganisationId";

        #endregion

        #region User Detail
        public static readonly string GetUsersDetailByIdsQuery = @"Select Id,FirstName,LastName,Email,PhoneNumber,UserCode,PartnerUserId FROM [User] WHERE Id IN @UserIds";
        public static readonly string GetUserDetailById = @"Select Id,FirstName,LastName,Email,PhoneNumber,UserCode,PartnerUserId FROM [User] WHERE Id=@UserId";

        public static readonly string GetUsersDetailByUserCodeQuery = @"Select Id,FirstName,LastName,Email,PhoneNumber FROM [User] WHERE UserCode IN @UserIds";

        public static readonly string GetUsersDetailByEmailIdsQuery = @"Select Id,FirstName,LastName,Email,PhoneNumber,IsActive,IsDeleted FROM [User] WHERE Email IN (@UserEmailIds)";

        public static readonly string GetUserDetailByIdQuery = @"SELECT  
                                                                    u.Id,
                                                                    u.[Address],
                                                                    u.PhoneNumber,
                                                                    u.Email as EmailAddress,
                                                                    u.FirstName,
                                                                    u.LastName,
                                                                    u.SessionId,
                                                                    u.UserName,
                                                                    u.PartnerUserId,
																	u.PartnerId,
                                                                    ISNULL(p.Id,0) AS ProgramCode,
                                                                    ISNULL(p.Name,0) AS ProgramName,
                                                                    ISNULL(phProgram.PhotoPath,@DefaultProgramPhotoPath) AS ProgramImagePath,
                                                                    ISNULL(userPic.PhotoPath,NULL) AS ImagePath,
                                                                    up.IsVerificationCodeDone AS IsProgramVerificationDone
                                                                     FROM [User] u
                                                                    JOIN UserProgram up ON u.Id=up.UserId AND up.IsLinkedProgram=1
                                                                    JOIN Program p ON up.ProgramId=p.Id AND p.IsActive=1 AND p.IsDeleted=0
                                                                    LEFT JOIN Photo userPic ON userPic.EntityId=@UserId AND userPic.PhotoType=" + (int)PhotoEntityType.UserProfile +
            "    LEFT JOIN Photo phProgram ON phProgram.EntityID=up.ProgramId AND  phProgram.PhotoType=" + (int)PhotoEntityType.Program + " WHERE u.Id=@UserId AND u.IsActive=1 AND u.IsDeleted=0";


        public static readonly string GetUserDetailByBiteUserIdQuery = @"SELECT  
                                                                    u.Id,
                                                                    u.[Address],
                                                                    u.PhoneNumber,
                                                                    u.Email as EmailAddress,
                                                                    u.FirstName,
                                                                    u.LastName,
                                                                    u.SessionId,
                                                                    u.UserName,
                                                                    u.PartnerUserId
                                                                    FROM [User] u
                                                                    WHERE u.PartnerUserId = @PartnerUserId AND u.PartnerId = @PartnerId AND u.IsActive=1 AND u.IsDeleted=0";




        public static readonly string GetUsersDetailByIdsWithProgramQuery = @"SELECT  
                                                                    u.Id,
                                                                    u.[Address],
                                                                    u.PhoneNumber,
                                                                    u.Email as EmailAddress,
                                                                    u.FirstName,
                                                                    u.LastName,
                                                                    u.SessionId,
                                                                    u.UserName,
                                                                    ISNULL(p.Id,0) AS ProgramCode,
                                                                    ISNULL(p.Name,0) AS ProgramName,
                                                                    ISNULL(phProgram.PhotoPath,@DefaultProgramPhotoPath) AS ProgramImagePath,
                                                                    ISNULL(userPic.PhotoPath,NULL) AS ImagePath,
                                                                    up.IsVerificationCodeDone AS IsProgramVerificationDone,
                                                                    u.UserCode
                                                                     FROM [User] u
                                                                    JOIN UserProgram up ON u.Id=up.UserId 
                                                                    JOIN Program p ON up.ProgramId=p.Id AND p.IsActive=1 AND p.IsDeleted=0
                                                                    LEFT JOIN Photo userPic ON userPic.EntityId=u.Id AND userPic.PhotoType=" + (int)PhotoEntityType.UserProfile +
            "    LEFT JOIN Photo phProgram ON phProgram.EntityID=up.ProgramId AND  phProgram.PhotoType=" + (int)PhotoEntityType.Program + " WHERE u.Id IN @UserIds AND u.IsActive=1 AND u.IsDeleted=0 ";

        public static readonly string GetUserBenefactorDetailByIdQuery = @"SELECT  Top (1)
                                                                    u.Id,
                                                                    u.[Address],
                                                                    u.PhoneNumber,
                                                                    u.Email as EmailAddress,
                                                                    u.FirstName,
                                                                    u.LastName,
                                                                    u.SessionId,
                                                                    u.UserName,
                                                                    ISNULL(p.Id,0) AS ProgramCode,
                                                                    ISNULL(p.Name, 0) AS ProgramName,
                                                                    ISNULL(phProgram.PhotoPath, @DefaultProgramPhotoPath) AS ProgramImagePath,
                                                                     ISNULL(userPic.PhotoPath, NULL) AS ImagePath
                                                                   FROM [User] u
                                                                     LEFT JOIN BenefactorProgram up ON u.Id=up.benefactorId
                                                                     LEFT JOIN Program p ON up.ProgramId= p.Id AND p.IsActive= 1 AND p.IsDeleted= 0
 
                                                                     LEFT JOIN Photo userPic ON userPic.EntityId= @UserId AND userPic.PhotoType= " + (int)PhotoEntityType.Benefactor +
            "           LEFT JOIN Photo phProgram ON phProgram.EntityID= up.ProgramId AND phProgram.PhotoType= " + (int)PhotoEntityType.Program + "  WHERE u.Id= @UserId AND u.IsActive= 1 AND u.IsDeleted= 0";

        public static readonly string GetUserWithProgramCodeQuery = @"SELECT  
                                                                    u.Id,
                                                                    u.[Address],
                                                                    u.PhoneNumber,
                                                                    u.Email as EmailAddress,
                                                                    u.FirstName,
                                                                    u.LastName,
                                                                    u.SessionId,
                                                                    u.UserName,
                                                                    ISNULL(p.Id,0) AS ProgramCode,
                                                                    ISNULL(p.Name,0) AS ProgramName,
                                                                    ISNULL(phProgram.PhotoPath,@DefaultProgramPhotoPath) AS ProgramImagePath,
                                                                    ISNULL(userPic.PhotoPath,NULL) AS ImagePath,
                                                                    up.IsVerificationCodeDone AS IsProgramVerificationDone,
                                                                    up.IsLinkedProgram
                                                                     FROM [User] u
                                                                    JOIN UserProgram up ON u.Id=up.UserId AND up.IsLinkedProgram=1 AND up.ProgramId = @ProgramId
                                                                    JOIN Program p ON up.ProgramId=p.Id AND p.IsActive=1 AND p.IsDeleted=0
                                                                    LEFT JOIN Photo userPic ON userPic.EntityId=@UserId AND userPic.PhotoType=" + (int)PhotoEntityType.UserProfile +
            "    LEFT JOIN Photo phProgram ON phProgram.EntityID=up.ProgramId AND  phProgram.PhotoType=" + (int)PhotoEntityType.Program + " WHERE u.Id=@UserId AND u.IsActive=1 AND u.IsDeleted=0";

        public static readonly string GetUserWithProgramCodeBeforeRegisterQuery = @"SELECT  
                                                                    u.Id,
                                                                    u.[Address],
                                                                    u.PhoneNumber,
                                                                    u.Email as EmailAddress,
                                                                    u.FirstName,
                                                                    u.LastName,
                                                                    u.SessionId,
                                                                    u.UserName,
                                                                    ISNULL(p.Id,0) AS ProgramCode,
                                                                    ISNULL(p.Name,0) AS ProgramName,
                                                                    ISNULL(phProgram.PhotoPath,@DefaultProgramPhotoPath) AS ProgramImagePath,
                                                                    ISNULL(userPic.PhotoPath,NULL) AS ImagePath,
                                                                    up.IsVerificationCodeDone AS IsProgramVerificationDone,
                                                                    up.IsLinkedProgram
                                                                     FROM [User] u
                                                                    JOIN UserProgram up ON u.Id=up.UserId  AND up.ProgramId = @ProgramId
                                                                    JOIN Program p ON up.ProgramId=p.Id AND p.IsActive=1 AND p.IsDeleted=0
                                                                    LEFT JOIN Photo userPic ON userPic.EntityId=@UserId AND userPic.PhotoType=" + (int)PhotoEntityType.UserProfile +
            "    LEFT JOIN Photo phProgram ON phProgram.EntityID=up.ProgramId AND  phProgram.PhotoType=" + (int)PhotoEntityType.Program + " WHERE u.Id=@UserId AND u.IsActive=1 AND u.IsDeleted=0";


        public static readonly string UpdateUserDeviceNLocationQuery = @"Update[User] set UserDeviceId=@UserDeviceId,UserDeviceType=@UserDeviceType,Location=@Location,SessionId=@SessionId WHERE Email=@Email";

        public static readonly string GetUserProgramPlanNOrgByProgramAccountId = @"Select Top(1)
pa.id as ProgramAccountId,
ppal.planId,
pp.programId,
org.Id as OrganizationId,
pa.AccountTypeId 
from ProgramAccounts pa
INNER JOIN PlanProgramAccountsLinking ppal ON ppal.programAccountId=pa.id
INNER JOIN ProgramPackage pp ON pp.Id=ppal.planId AND pp.isActive=@IsActive AND pp.isDeleted=@IsDeleted
INNER JOIN UserPlans up ON up.programPackageId=pp.id and up.UserId=@UserId
INNER JOIN Program p ON p.Id=pp.programId AND p.isActive=@IsActive AND p.isDeleted=@IsDeleted
INNER JOIN OrganisationProgram op ON op.programId=pp.programId
INNER JOIN Organisation org ON org.Id=op.organisationId AND org.isActive=@IsActive AND org.isDeleted=@IsDeleted AND org.organisationType=@OrganizationType
Where pa.Id=@ProgramAccountId";
        public static readonly string GetAccountHoldersBySP = @"GetAccountHolders";
        public static readonly string GetAccountHoldersByOrganization = @"SP_GetAccountHoldersByOrganization";
        public static readonly string GetUserTypeBySP = @"sp_GetSodexhoUserType";
        public static readonly string GetUserCardwithBankAccountBySP = @"sp_GetUserCardwithBankAccount1";
        public static readonly string GetBiteUserLoyaltyTrackingBalanceQuery = @"SELECT  TOP 1 CASE  WHEN leftOverPoints=0.00 THEN  totalPoints ELSE leftOverPoints END  [loyalty tracking balance]
		     FROM  dbo.UserLoyaltyPointsHistory WHERE userId =@Id  ORDER BY id DESC";

        public static readonly string GetMultipleQueryUserDetailNUserPlansQuery = @"SELECT 
                                                                    usr.Id,
                                                                    usr.UserCode as AccountHolderID,
                                                                    usr.FirstName,
                                                                    usr.LastName,
                                                                    usr.PhoneNumber,
                                                                    usr.Email,
                                                                    usr.SecondaryEmail,
                                                                    usr.GenderId,
                                                                    usr.DateOfBirth,usr.PartnerUserId,
                                                                    usr.CustomInfo as UserCustomJsonValue,
                                                                    ISNULL(userPic.PhotoPath,NULL) AS UserImagePath,
                                                                    usr.Jpos_AccountHolderId,
                                                                     ( SELECT  TOP 1 i2c.ReferenceId FROM dbo.I2CAccountDetail i2c WHERE i2c.UserId=usr.Id  ORDER BY id DESC)i2cReferenceId 
                                                                    FROM [User] usr
                                                                     LEFT JOIN Photo userPic ON userPic.EntityId=usr.Id AND userPic.PhotoType=@PhotoTypeDetail
                                                                     WHERE usr.Id=@UserId AND usr.IsActive=1 AND usr.IsDeleted=0
                                                                    
                                                                     Select programPackageId FROM UserPlans up
                                                                     INNER JOIN ProgramPackage pp on pp.Id=up.programPackageId AND pp.ProgramId=@ProgramId
                                                                      where up.UserId = @UserId";
        #endregion

        #region Reset Password

        public static readonly string GetResetPasswordContentQuery = @"SELECT rup.Id,
                                                                            rup.UserId,
                                                                            rup.ResetToken,
                                                                            rup.ValidTill,
                                                                            rup.IsPasswordReset,
                                                                            rup.CreatedDate,
                                                                            rup.UpdatedDate from ResetUserPassword rup
                                                                            JOIN [User] u on u.Id=rup.UserId
                                                                            WHERE u.Email=@Email and u.IsActive=1 AND u.IsDeleted=0";


        public static readonly string GetMultipleResetPasswordNUserContentQuery = @"Select * from [User] WHERE Email=@Email AND IsActive=1 AND IsDeleted = 0


                                                                            SELECT rup.*
                                                                             from ResetUserPassword rup
                                                                            JOIN[User] u on u.Id= rup.UserId
                                                                            WHERE u.Email= @Email and u.IsActive= 1 AND u.IsDeleted= 0";
        #endregion

        #region Benefactor
        public static readonly string GetConnectionUserBenefactorQuery = @"SELECT Id, UserId,
                                                                            BenefactorId,
                                                                            RelationshipId,
                                                                            IsRequestAccepted
                                                                            FROM [BenefactorUsersLinking] 
                                                                            WHERE UserId = @userId AND BenefactorId = @BenefactorId AND IsActive = @IsActive AND IsDeleted = @IsDeleted";

        public static readonly string CancelReloadRuleQuery = @"update reloadrules set IsDeleted=1 Where benefactorUserId=@buid AND userId=@uid";
        #endregion

        #region NotificationSettings

        public static readonly string GetUsersNotificationSettingsQuery = @"SELECT ns.id as NotificationId,
                                                                             ns.name as NotificationName,
                                                                             ns.description as NotificationDescription,
                                                                             ns.ColorCode as ColorCode,
                                                                             ISNULL(uns.IsNotificationEnabled,0) as UserNotificationSet
                                                                             FROM NotificationSettings ns
                                                                             LEFT JOIN UserNotificationSettings uns on ns.Id=uns.NotificationId AND uns.UserId=@UserId";

        public static readonly string UpdateUsersNotificationSettingsQuery = @"UPDATE UserNotificationSettings SET IsNotificationEnabled=@IsNotificationEnabled WHERE UserId=@UserId AND NotificationId=@NotificationId";

        public static readonly string DeleteUsersNotificationSettingsQuery = @"DELETE  FROM  UserNotificationSettings WHERE UserId=@UserId;";

        #endregion

        #region OrganisationProgram
        public static readonly string GetMultipleOrganisationDetailNOrganisationProgramQuery = @"SELECT
                                                                                                Id AS OrganisationId,
                                                                                                Name AS OrganisationName,
                                                                                                [AddressLine1] AS[Address],
                                                                                                WebsiteURL AS Website,
                                                                                                ContactName,
                                                                                                ContactTitle,
                                                                                                ContactNumber,
                                                                                                EmailAddress as EmailAddress,
                                                                                                [Description],
                                                                                                OrganisationSubTitle as OrganisationSubTitle,
                                                                                                FacebookURL,
                                                                                                TwitterURL,
                                                                                                SkypeHandle,
                                                                                                JPOS_MerchantId
                                                                                                FROM Organisation
                                                                                                WHERE OrganisationType = @OrganisationType AND Id = @OrganisationId AND IsActive=1 AND IsDeleted=0
                                                                                                
                                                                                                Select ProgramTypeId FROM OrganisationProgramType where OrganisationId = @OrganisationId";

        public static readonly string GetOrganisationProgramsListQuery = @"SELECT 
                                                                            prg.Id AS ProgramId,
                                                                            prg.Name AS ProgramName,
                                                                            prg.ProgramCodeId AS ProgramCodeId,
                                                                            ISNULL(oprg.ModifiedDate,oprg.CreatedDate) AS DateAdded,
                                                                            pt.ProgramTypeName as ProgramType,
                                                                            (select count(1) 
                                                                            FROM [User] usr 
                                                                            INNER JOIN UserProgram uprg ON usr.Id=uprg.UserId and uprg.ProgramId=prg.Id  
                                                                            WHERE (usr.IsAdmin IS NULL OR usr.IsAdmin=0 ) AND usr.IsActive=1 and usr.IsDeleted=0 ) as AccountListCount
                                                                            FROM Program prg
                                                                            INNER JOIN OrganisationProgram oprg on prg.Id=oprg.ProgramId AND oprg.OrganisationId=@OrganisationId AND oprg.IsActive=1 AND oprg.IsDeleted=0
	                                                                        INNER JOIN Organisation org On oprg.OrganisationId=org.Id AND org.IsActive=1 AND org.IsDeleted=0
                                                                            LEFT JOIN ProgramType pt on prg.ProgramTypeId=pt.Id 
                                                                            WHERE prg.IsActive=1 AND prg.IsDeleted=0
                                                                             AND (@RoleName='super admin' OR 
                                                                             (prg.Id in (Select ProgramTypeId from AdminProgramAccess apa
                                                                               INNER JOIN [User] usr ON apa.UserId=usr.Id
                                                                               INNER JOIN UserRole ur ON usr.Id=ur.UserId
                                                                                INNER JOIN [Role] r on r.Id=ur.RoleId and r.Name=@RoleName
                                                                                   WHERE usr.Id=@UserId AND usr.IsActive=1 AND usr.IsDeleted=0)))";

        public static readonly string GetAllProgramsListQuery = @"SELECT 
                                                                            prg.Id AS ProgramId,
 prg.Name AS ProgramName,
 prg.ProgramCodeId AS ProgramCodeId,
 ISNULL(prg.ModifiedDate,prg.CreatedDate) AS DateAdded,
 pt.ProgramTypeName as ProgramType,
 STUFF((
 SELECT N' | ' + ISNULL(org.OrganisationSubtitle,'') FROM Organisation org
 INNER JOIN OrganisationProgram oprg on prg.Id=oprg.ProgramId AND org.Id=oprg.organisationId  AND oprg.IsActive=1 
 AND oprg.IsDeleted=0 AND org.IsActive=1 AND org.IsDeleted=0
Where org.organisationType=@OrganisationType
 FOR XML PATH(''), TYPE).value(N'.[1]', N'nvarchar(max)'), 1, 2, N'') as OrganisationSubtitle,
 OrganisationId=(Select top(1)organisationId FROM OrganisationProgram oprg1 INNER JOIN Organisation org1 ON 
org1.Id=oprg1.organisationId AND org1.IsActive=1 AND org1.IsDeleted=0
where oprg1.programId=prg.Id AND  oprg1.IsActive=1 AND oprg1.IsDeleted=0), --prg.organisationId as OrganisationId,
org.OrganisationSubTitle as OrganisationName,
(Select Count(oprg2.id) FROM OrganisationProgram oprg2 
INNER JOIN Organisation org2 ON 
org2.Id=oprg2.organisationId AND org2.IsActive=1 AND org2.IsDeleted=0
where oprg2.programId=prg.Id AND  oprg2.IsActive=1 AND oprg2.IsDeleted=0) as OrgPrgLinkCount,
prg.JPOS_IssuerId
 FROM Program prg 
 INNER JOIN ProgramType pt on prg.ProgramTypeId=pt.Id
INNER JOIN Organisation org on prg.organisationId=org.Id AND org.organisationType=@OrganisationType AND org.IsActive=1 AND org.IsDeleted=0
                                                                            WHERE prg.IsActive=@IsActive AND prg.IsDeleted=@IsDeleted
 AND (@RoleName='super admin' OR 
                                                                             (@RoleName='organization full' AND prg.Id in (Select ProgramTypeId from AdminProgramAccess apa
                                                                               INNER JOIN [User] usr ON apa.UserId=usr.Id
                                                                               INNER JOIN UserRole ur ON usr.Id=ur.UserId
                                                                                INNER JOIN [Role] r on r.Id=ur.RoleId and r.Name=@RoleName
                                                                                   WHERE usr.Id=@UserId AND usr.IsActive=1 AND usr.IsDeleted=0))
OR (@RoleName='program full' AND prg.Id in (Select usr.ProgramId from [User] usr 
                                                                               INNER JOIN UserRole ur ON usr.Id=ur.UserId
                                                                                INNER JOIN [Role] r on r.Id=ur.RoleId and r.Name=@RoleName
                                                                                   WHERE usr.Id=@UserId AND usr.IsActive=1 AND usr.IsDeleted=0))
OR (@RoleName='merchant full' AND prg.Id in (Select op.ProgramId from [User] usr 
                                                                               INNER JOIN OrganisationProgram op ON usr.OrganisationId=op.OrganisationId
                                                                                INNER JOIN UserRole ur ON usr.Id=ur.UserId
                                                                                INNER JOIN [Role] r on r.Id=ur.RoleId and r.Name=@RoleName
                                                                                   WHERE usr.Id=@UserId AND usr.IsActive=1 AND usr.IsDeleted=0)))";

        public static readonly string GetProgramsListOfPrgAdminQuery = @"SELECT 
                                                                            prg.Id AS ProgramId,
 prg.Name AS ProgramName,
 prg.ProgramCodeId AS ProgramCodeId,
 ISNULL(prg.ModifiedDate,prg.CreatedDate) AS DateAdded,
 pt.ProgramTypeName as ProgramType,
 ISNULL(org.Name,org.OrganisationSubTitle) as OrganisationName
 FROM Program prg 
 INNER JOIN ProgramType pt on prg.ProgramTypeId=pt.Id
 INNER JOIN ProgramAdmins pa ON pa.ProgramId=prg.Id
INNER JOIN OrganisationProgram orgprg ON orgprg.ProgramId=pa.ProgramId
INNER JOIN Organisation org On org.Id=orgprg.OrganisationId AND org.OrganisationType=2 AND  org.IsActive=@IsActive AND org.IsDeleted=@IsDeleted
		INNER JOIN [User] u ON u.Id=pa.AdminUserId
                                                                            WHERE prg.IsActive=@IsActive AND prg.IsDeleted=@IsDeleted
 AND u.Id=@UserId AND u.IsActive=@IsActive AND u.IsDeleted=@IsDeleted";
        public static readonly string GetOrganisationAdminsListQuery = @"
                                                                        Select DISTINCT
                                                                        usr.Id AS UserId,
                                                                        usr.FirstName+' ' +LastName AS Name,
                                                                        usr.Email AS EmailAddress,
                                                                        usr.CreatedDate AS DateAdded,
                                                                        usr.PhoneNumber AS PhoneNumber,
                                                                        rl.Name AS RoleName,
                                                                        usr.IsActive AS [Status],
                                                                         ISNULL(userPic.PhotoPath,NULL) AS UserImagePath,
                                                                       ProgramsAccessibility = STUFF((
																			SELECT N', ' + pt.name FROM Program pt
																			INNer JOIN AdminProgramAccess apa ON apa.ProgramTypeId=pt.Id
																			WHERE apa.UserId = usr.Id
																			FOR XML PATH(''), TYPE).value(N'.[1]', N'nvarchar(max)'), 1, 2, N''),
                                                                         Custom1 AS Title,
                                                                         IsAdmin,
                                                                         ISNULL(InvitationStatus, 0) AS InvitationStatus,
                                                                         EmailConfirmed
                                                                         FROM[User] usr
                                                                         LEFT JOIN UserRole urole ON usr.Id=urole.UserId
                                                                        LEFT JOIN[Role] rl ON urole.RoleId=rl.Id
                                                                        LEFT JOIN Photo userPic ON userPic.EntityId=usr.Id AND userPic.PhotoType=" + (int)PhotoEntityType.UserProfile +
                                                                        " Where usr.OrganisationId= @OrganisationID AND usr.IsDeleted = 0 AND usr.IsAdmin=1";

        public static readonly string GetMerchantAdminsListQuery = @"
                                                                        Select DISTINCT
                                                                        usr.Id AS UserId,
                                                                        usr.FirstName+' ' +LastName AS Name,
                                                                        usr.Email AS EmailAddress,
                                                                        usr.CreatedDate AS DateAdded,
                                                                        usr.PhoneNumber AS PhoneNumber,
                                                                        rl.Name AS RoleName,
                                                                        usr.IsActive AS [Status],
                                                                         ISNULL(userPic.PhotoPath,NULL) AS UserImagePath,
                                                                       ProgramsAccessibility = STUFF((
																			SELECT N', ' + pt.name FROM Program pt
																			INNer JOIN AdminProgramAccess apa ON apa.ProgramTypeId=pt.Id
																			WHERE apa.UserId = usr.Id
																			FOR XML PATH(''), TYPE).value(N'.[1]', N'nvarchar(max)'), 1, 2, N''),
                                                                         Custom1 AS Title,
                                                                         IsAdmin,
                                                                         ISNULL(InvitationStatus, 0) AS InvitationStatus,
                                                                         EmailConfirmed
                                                                         FROM[User] usr
                                                                         LEFT JOIN UserRole urole ON usr.Id=urole.UserId
                                                                        INNER JOIN MerchantAdmins ma ON ma.adminUserId=usr.Id
                                                                        LEFT JOIN[Role] rl ON urole.RoleId=rl.Id
                                                                        LEFT JOIN Photo userPic ON userPic.EntityId=usr.Id AND userPic.PhotoType=" + (int)PhotoEntityType.UserProfile +
                                                                        " Where ma.MerchantId= @OrganisationID AND usr.IsDeleted = 0 AND usr.IsAdmin=1";

        public static readonly string GetProgramAdminsListForPrgRoleQuery = @"
                                                                        Select DISTINCT
                                                                        usr.Id AS UserId,
                                                                        usr.FirstName+' ' +LastName AS Name,
                                                                        usr.Email AS EmailAddress,
                                                                        usr.CreatedDate AS DateAdded,
                                                                        usr.PhoneNumber AS PhoneNumber,
                                                                        rl.Name AS RoleName,
                                                                        usr.IsActive AS [Status],
                                                                         ISNULL(userPic.PhotoPath,NULL) AS UserImagePath,
                                                                       ProgramsAccessibility = STUFF((
																			SELECT N', ' + pt.name FROM Program pt
																			INNer JOIN AdminProgramAccess apa ON apa.ProgramTypeId=pt.Id
																			WHERE apa.UserId = usr.Id
																			FOR XML PATH(''), TYPE).value(N'.[1]', N'nvarchar(max)'), 1, 2, N''),
                                                                         Custom1 AS Title,
                                                                         IsAdmin,
                                                                         ISNULL(InvitationStatus, 0) AS InvitationStatus,
                                                                         EmailConfirmed
                                                                         FROM[User] usr
                                                                         LEFT JOIN UserRole urole ON usr.Id=urole.UserId
                                                                        INNER JOIN ProgramAdmins pa ON pa.adminUserId=usr.Id
                                                                        LEFT JOIN[Role] rl ON urole.RoleId=rl.Id
                                                                        LEFT JOIN Photo userPic ON userPic.EntityId=usr.Id AND userPic.PhotoType=" + (int)PhotoEntityType.UserProfile +
                                                                        " Where pa.programId= @ProgramId AND usr.IsDeleted = 0 AND usr.IsAdmin=1";

        public static readonly string DeleteOrganisationProgramTypeQuery = @"Delete from OrganisationProgramType  WHERE OrganisationId=@OrganisationId";

        public static readonly string DeleteOrganisationProgramQuery = @"Delete from OrganisationProgram  WHERE OrganisationId=@OrganisationId";

        public static readonly string GetProgramListByOrgIdQuery = @"Select 
                                                    prg.Id,
                                                    Prg.Name
                                                    from Program prg
                                                    INNER JOIN OrganisationProgram orgPrg ON orgPrg.ProgramId=prg.Id 
                                                    INNER JOIN [User] usr On usr.Id=@UserId and usr.IsActive=@IsActive 
                                                    AND usr.IsDeleted=@IsDeleted
                                                    WHERE prg.IsActive=1 AND prg.IsDeleted=0 AND orgPrg.OrganisationId=@OrganisationId";

        public static readonly string GetProgramLstForRoleOrganisationAdmin = @"Select 
                                                    prg.Id,
                                                    Prg.Name
                                                    from Program prg
                                                    INNER JOIN AdminProgramAccess apa ON apa.ProgramTypeId=prg.Id  AND apa.UserId=@UserId
                                                    INNER JOIN [User] usr On usr.Id=@UserId AND usr.IsActive=@IsActive
                                                    AND usr.IsDeleted=@IsDeleted
                                                    WHERE prg.IsActive=1 AND prg.IsDeleted=0 AND usr.OrganisationId=@OrganisationId";

        public static readonly string GetProgramLstForRolePrgAdmin = @"Select 
                                                    prg.Id,
                                                    Prg.Name
                                                    from Program prg
                                                    INNER JOIN [User] usr On usr.ProgramId=prg.Id 
                                                    INNER JOIN OrganisationProgram orgPrg ON orgPrg.ProgramId=usr.ProgramId
                                                    Inner JOIN Organisation org ON org.Id=orgPrg.OrganisationId and org.Id=@OrganisationId
                                                    WHERE prg.IsActive=@IsActive AND prg.IsDeleted=@IsDeleted 
                                                    AND usr.Id=@UserId and usr.IsActive=@IsActive AND usr.IsDeleted=@IsDeleted
                                                    AND org.IsActive=@IsActive AND org.IsDeleted=@IsDeleted";

        public static readonly string GetProgramLstForRoleMerchant = @"Select 
                                                    prg.Id,
                                                    Prg.Name
                                                    from Organisation org
                                                    INNER JOIN [User] usr On usr.OrganisationId=org.Id 
                                                    INNER JOIN OrganisationProgram orgPrg ON orgPrg.OrganisationId=usr.OrganisationId
                                                    INNER JOIN Program prg ON prg.Id=orgPrg.ProgramId
                                                    WHERE prg.IsActive=@IsActive AND prg.IsDeleted=@IsDeleted 
                                                    AND usr.Id=@UserId and usr.IsActive=@IsActive AND usr.IsDeleted=@IsDeleted
                                                    AND org.IsActive=@IsActive AND org.IsDeleted=@IsDeleted";

        public static readonly string GetProgramListBasedOnIdsQuery = @"Select *
                                                    from  Program prg 
                                                    WHERE prg.IsActive=@IsActive AND prg.IsDeleted=@IsDeleted 
                                                   AND prg.Id IN @ProgramIds";
        #endregion

        #region Admin Info
        public static readonly string DeleteAdminProgramTypeQuery = @"Delete from AdminProgramAccess  WHERE UserId=@UserId";

        public static readonly string DeleteMerchantAdminQuery = @"Delete from MerchantAdmins  WHERE adminUserId=@UserId";

        public static readonly string DeleteProgramLevelAdminQuery = @"Delete from ProgramAdmins  WHERE adminUserId=@UserId";

        public static readonly string UpdateOrganisationAdminStatusQuery = @"Update[User] set IsActive=@IsActive WHERE Id=@UserId";

        public static readonly string GetUserAdminDetailByIdQuery = @"SELECT 
                                                                            usr.Id AS UserId,
                                                                            usr.FirstName AS FirstName,
                                                                            usr.LastName AS LastName,
                                                                            usr.Email AS Email,
                                                                            usr.PhoneNumber AS PhoneNumber,
                                                                            usr.IsAdmin AS IsAdmin,
                                                                            ISNULL(userPic.PhotoPath,NULL) AS UserImagePath,
                                                                            urole.RoleId AS RoleId,
                                                                            Custom1 AS Title
                                                                             FROM [User] usr
                                                                             LEFT JOIN UserRole urole ON usr.Id=urole.UserId
                                                                             LEFT JOIN[Role] rl ON urole.RoleId=rl.Id
                                                                             LEFT JOIN Photo userPic ON userPic.EntityId=usr.Id AND 
                                                                            userPic.PhotoType= " + (int)PhotoEntityType.UserProfile + "   WHERE UserId = @UserId AND IsAdmin=1;" +
            " Select ProgramTypeId FROM AdminProgramAccess where UserId = @UserId;" +
            "Select MerchantId FROM MerchantAdmins where AdminUserId = @UserId;" +
            "Select ProgramId FROM ProgramAdmins where AdminUserId = @UserId;";

        public static readonly string GetOrgSelectedPrgTypeQuery = @"Select pt.* from ProgramType pt
                                                                    INNER JOIN OrganisationProgramType opt On pt.Id=opt.ProgramTypeId
                                                                    AND opt.OrganisationId=@OrganisationId";

        #endregion

        #region Roles
        public static readonly string GetRolesQuery = @"Select Id, Name from Role Where RoleType=@RoleType";
        #endregion

        #region Promotions
        public static readonly string GetAllPromotionsOfMerchantQuery = @"SELECT
                                                                        Id,
                                                                        StartDate,
                                                                        EndDate,
                                                                        PromotionDay,
                                                                        --CAST(StartTime AS DateTime) AS StartTime,
                                                                        --CAST(EndTime AS DateTime) AS EndTime,
StartTime,
EndTime,
                                                                        BannerDescription,
                                                                        IsActive,
                                                                        IsDailyPromotion=(CASE
                                                                           When promotionDay IS NULL Then 0
                                                                           ELSE 1 END),
                                                                        RepeatDailyDay=(CASE
                                                                        WHEN promotionDay IS NULL THEN NULL
                                                                        WHEN promotionDay='Monday' THEN 1
                                                                        WHEN promotionDay='Tuesday' THEN 2
                                                                        WHEN promotionDay='Wednesday' THEN 3
                                                                        WHEN promotionDay='Thursday' THEN 4
                                                                        WHEN promotionDay='Friday' THEN 5
                                                                        WHEN promotionDay='Saturday' THEN 6
                                                                        ELSE 0 END
                                                                        )
                                                                         FROM Promotion
                                                                         WHERE MerchantId = @MerchantId  AND
                                                IsDeleted=0 AND offerTypeId IN (Select ID from OfferType WHERE OfferType='promotions')";

        public static readonly string GetPromotionsOfMerchantByIdQuery = @"SELECT
                                                                        p.Id,
																		p.Name,
																		p.[Description],
																		p.bannerTypeId,
                                                                        p.StartDate,
                                                                        p.EndDate,
                                                                        p.PromotionDay,
                                                                     p.StartTime,
p.EndTime,
                                                                        p.BannerDescription,
                                                                        p.IsActive,
                                                                        IsDailyPromotion=(CASE
                                                                           When promotionDay IS NULL Then 0
                                                                           ELSE 1 END),
																		   p.MerchantId,
                                                                        RepeatDailyDay=(CASE
                                                                        WHEN promotionDay IS NULL THEN NULL
                                                                        WHEN promotionDay='Monday' THEN 1
                                                                        WHEN promotionDay='Tuesday' THEN 2
                                                                        WHEN promotionDay='Wednesday' THEN 3
                                                                        WHEN promotionDay='Thursday' THEN 4
                                                                        WHEN promotionDay='Friday' THEN 5
                                                                        WHEN promotionDay='Saturday' THEN 6
                                                                        ELSE 0 END
                                                                        ),
																		ISNULL(Img.PhotoPath,NULL) AS ImagePath
                                                                         FROM Promotion p
																		 LEFT JOIN Photo AS img ON p.Id=img.EntityId AND img.PhotoType= " + (int)PhotoEntityType.OffersPromotions +
                                                                        " WHERE p.Id = @PromotionId AND IsDeleted=0";


        public static readonly string GetPromotionDataByIdQuery = @"SELECT
                                                                        id,
offerTypeId,
offerSubTypeId,
name,
description,
bannerTypeId,
bannerDescription,
startDate,
                                                                         StartTime,
endDate,
                                                                        EndTime,
                                                                     promotionDay,
isActive,
createdBy,
createdDate,
modifiedBy,
modifiedDate,
isDeleted,
MerchantId,
noOfVisits,
amounts,
businessTypeId,
backgroundColor,
firstGradiantColor,
secondGradiantColor
                                                                         FROM Promotion
                                                                         WHERE id = @id  AND IsDeleted=0";


        #endregion

        #region Plan
        public static readonly string GetPlanLisitngQuery = @"SELECT pp.Id AS Id, pp.ClientId, pp.planId AS InternalId, pp.Name, pp.Description,
                                                              (convert(varchar,pp.StartDate, 101) + ' at ' +  CONVERT(varchar(10),pp.StartTime,100) +'<br />'+
                                                              convert(varchar,pp.EndDate, 101) + ' at ' + CONVERT(varchar(10),pp.EndTime,100)) AS StartEnd,
                                                              STUFF  
	                                                                (  
                                                                        (  
                                                                            SELECT '<br />' + pa.accountName  
                                                                            FROM ProgramAccounts AS pa   
                                                                            WHERE pa.id in (SELECT ppal.programAccountId FROM PlanProgramAccountsLinking AS ppal WHERE pp.Id = ppal.planId)
                                                                            FOR XML PATH('')  
                                                                        ),1,1,''  
	                                                                ) AS Accounts,
                                                                pp.isActive AS [Status],
                                                                pp.Jpos_PlanId
                                                                from programpackage AS pp 
                                                                WHERE pp.ProgramId = @ProgramId AND pp.IsDeleted = @IsDeleted";

        public static readonly string GetPlanDetailInformationWithMasterQuery = @"SELECT id, accountName FROM ProgramAccounts WHERE IsActive = 1 AND IsDeleted = 0 AND programId = @ProgramId
                                                                                        
                                                                                    SELECT id, name, programId, cast(clientId AS varchar(10)) AS clientId, cast(planId AS varchar(10)) AS planId, [description], startDate, startTime, endDate, endTime,Jpos_PlanId FROM ProgramPackage
                                                                                    WHERE id = @PlanId AND isDeleted = 0
                                                                                                
                                                                                    SELECT planId, programAccountId FROM PlanProgramAccountsLinking WHERE planId = @PlanId ";


        #endregion

        #region ProgramAccount
        public static readonly string GetAccountLisitngQuery = @"SELECT pa.id, pa.programAccountId, pa.accountName, at.accountType, STUFF  
	                                                                (  
                                                                        (  
                                                                            SELECT '<br />' + pp.name  
                                                                            FROM ProgramPackage AS pp 
																			LEFT JOIN PlanProgramAccountsLinking AS ppal ON pa.id = ppal.programAccountId  
                                                                            WHERE pp.id = ppal.planId
                                                                            FOR XML PATH('')  
                                                                        ),1,1,''  
	                                                                ) AS planName, pa.isActive AS Status,Jpos_ProgramAccountId FROM ProgramAccounts AS pa 
                                                                    INNER JOIN AccountType AS at ON pa.accountTypeId = at.id WHERE pa.isDeleted = @IsDeleted and pa.programId = @ProgramId";

        public static readonly string GetMerchantForAccountMerchantRuleQuery = @"SELECT Id, Name FROM BusinessType WHERE isActive = 1 AND IsDeleted = 0
                                                                                    SELECT o.Id, o.Name AS MerchantName FROM Organisation AS o 
                                                                                    INNER JOIN OrganisationProgram AS op ON o.Id = op.OrganisationId AND op.ProgramId = @ProgramId
                                                                                    INNER JOIN ProgramMerchantAccountType AS pmat ON o.Id = pmat.OrganisationId
                                                                                    INNER JOIN ProgramAccountLinking AS pal ON pal.Id = pmat.ProgramAccountLinkingId AND pal.ProgramId = op.ProgramId AND (pal.AccountTypeId in (select Value from dbo.ConvertStringToTable(@AccountTypeId)))
                                                                                    
                                                                                    WHERE o.isActive = 1 AND o.isDeleted = 0 AND o.organisationType = @OrganisationType AND (o.businessTypeId in (select Value from dbo.ConvertStringToTable(@BusinessType)) OR @BusinessType = '' OR @BusinessType Is NULL)


SELECT amrd.Id AS id,o.Name AS merchantName,
amrd.mealPeriod AS mealPeriodId,
amr.accountTypeId,
(CASE 
	WHEN amrd.mealPeriod = 1 THEN 'Breakfast' 
	WHEN amrd.mealPeriod = 2 THEN 'Lunch' 
	WHEN amrd.mealPeriod = 3 THEN 'Dinner' 
	WHEN amrd.mealPeriod = 4 THEN 'Brunch' END) AS mealPeriod,
	amrd.maxPassUsage,
	amrd.minPassValue,
	amrd.maxPassValue,
	amrd.transactionLimit
	FROM AccountMerchantRules AS amr
INNER JOIN AccountMerchantRulesDetail AS amrd ON amr.id = amrd.accountMerchantRuleId
INNER JOIN Organisation AS o ON amr.MerchantId = o.Id AND o.OrganisationType = @OrganisationType AND amr.programAccountId = @ProgramAccountId AND amr.accountTypeId IN (select Value from dbo.ConvertStringToTable(@accountTypeId))
";
        public static readonly string CheckMerchantAccountTypeQuery = @"SELECT COUNT(1) FROM ProgramAccountLinking AS pal 
                                                                        INNER JOIN ProgramMerchantAccountType AS pmat ON pal.Id = pmat.programAccountLinkingId
                                                                        WHERE ProgramId = @ProgramId AND AccountTypeId = @AccountTypeId AND OrganisationId = @MerchantId";

        public static readonly string GetProgramAccountsDetailsByPlanIds = @"Select
                                                                        pp.ProgramId,
                                                                        pp.Id as PlanId,
                                                                        ppal.ProgramAccountId,
                                                                        pa.AccountName,
                                                                        (CASE
                                                                        When pa.AccountTypeId=3 Then pa.vplMaxBalance
                                                                        ELSE pa.intialBalanceCount END) as InitialBalance,
                                                                        pa.AccountTypeId
                                                                        from ProgramPackage pp 
                                                                        INNER JOIN PlanProgramAccountsLinking ppal ON pp.Id=ppal.planId
                                                                        INNER JOIN ProgramAccounts pa ON pa.Id=ppal.programAccountId
                                                                        WHERE pp.Id IN @PlanId ";

        public static readonly string GetUserTransactionUserDetailsByAccountIds = @" Select
                                                                        uti.CreditUserId,
                                                                        uti.PlanId as PlanId,
                                                                        uti.ProgramAccountId,
                                                                        uti.CreditTransactionUserType,
                                                                        uti.AccountTypeId
                                                                        from UserTransactionInfo uti
                                                                        WHERE uti.ProgramAccountId IN @AccountId
                                                                        AND uti.CreditTransactionUserType= 1";

        public static readonly string GetAccountsDetailsByIdNCheckBranding = @"SELECT at.id AS Id,at.accountType AS AccountType,
                                                                                Count(pb.id) AS BrandingCount FROM ProgramAccounts pa
                                                                                INNER JOIN AccountType at ON pa.AccountTypeId=at.id 
                                                                                LEFT JOIN ProgramBranding pb ON pb.ProgramAccountId=pa.id 
                                                                                AND pb.IsActive=@IsActive AND pb.IsDeleted=@IsDeleted
                                                                                WHERE pa.IsActive=@IsActive AND pa.IsDeleted=@IsDeleted 
                                                                                AND pa.Id=@AccountId
                                                                                GROUP BY at.id,at.accountType,pb.id";

        public static readonly string GetAccountsDetailsByIds = @"Select pa.Id,pa.accountName,pa.Jpos_ProgramAccountId from ProgramAccounts pa where Id IN @AccountId AND IsActive=@IsActive AND IsDeleted=@IsDeleted";

        public static readonly string GetAccountBasedOnPlanNProgramSelection = @"SELECT
 pa.id,
  pa.programAccountId,
   pa.accountName,
    at.accountType
	 FROM ProgramAccounts AS pa 
INNER JOIN AccountType AS at ON pa.accountTypeId = at.id
INNER JOIN ProgramPackage pp ON pp.Id=@PlanId
INNER JOIN PlanProgramAccountsLinking ppal ON ppal.PlanId=pp.Id
 WHERE pa.isDeleted = @IsDeleted and pa.IsActive=@IsActive and pa.programId = @ProgramId and pp.IsActive=@IsActive and pp.IsDeleted=@IsDeleted";

        public static readonly string GetProgramAccountsDropdownForUser = @"Select DISTINCT pa.accountName,pa.accountTypeId,pa.id,atp.accountType as AccountType from ProgramAccounts pa
INNER JOIN PlanProgramAccountsLinking ppal ON ppal.programAccountId=pa.Id
INNER JOIN AccountType atp ON atp.Id=pa.accountTypeId
INNER JOIN UserPlans up ON up.programPackageId=ppal.planId
INNER JOIN ProgramPackage pp ON pp.id=up.programPackageId AND pp.isActive=@IsActive AND pp.isDeleted=@IsDeleted
INNER JOIN [User] u ON u.Id=up.userId AND u.IsActive=@IsActive AND u.IsDeleted=@IsDeleted
WHERE u.Id=@UserId AND pa.isActive=@IsActive AND pa.isDeleted=@IsDeleted";

        #endregion

        #region ProgramBranding

        public static readonly string GetBrandingLisitngQuery = @"SELECT pb.id, pb.programAccountId, pb.accountName, at.accountType, pb.brandingColor, ISNULL(p.photoPath, NULL) AS ImagePath,
                                                                    pb.cardNumber,
                                                                    pb.accountTypeId
                                                                    FROM ProgramBranding AS pb 
                                                                    INNER JOIN AccountType AS at ON pb.accountTypeId = at.id
                                                                    LEFT JOIN Photo AS p ON pb.Id = p.entityId AND p.photoType = @PhotoType
                                                                    WHERE pb.programId = @ProgramId AND pb.IsDeleted = @IsDeleted ";



        #endregion

        #region UserPlans
        public static readonly string DeleteUserPlansQuery = @"Delete from UserPlans  WHERE userId=@UserId";
        public static readonly string GetUserPlanDetail = @"Select pp.Id,pp.[Name],pp.[description] from ProgramPackage pp
                                        INNER JOIN UserPlans up ON up.userId=pp.id 
                                        INNER JOIN [User] u ON u.Id=up.userId AND u.IsActive=@IsActive AND u.IsDeleted=@IsDeleted
                                        WHERE pp.isActive=@IsActive AND pp.isDeleted =@IsDeleted 
                                        AND CONVERT(VARCHAR(10), GETUTCDATE(), 111) BETWEEN DATEADD(millisecond ,DATEDIFF(millisecond, 0, pp.StartTime),CAST(pp.StartDate AS DATETIME)) 
                                        AND DATEADD(millisecond ,DATEDIFF(millisecond, 0, pp.EndTime),CAST(pp.EndDate AS DATETIME))
                                        AND u.id=@UserId and pp.programId=@ProgramId ";
        #endregion

        #region Import User

        public static readonly string GetMaximumSheetRows = @"Select Id, KeyName, Value From GeneralSetting
                                                    Where KeyName ='ExcelRowsValidation'";
        public static readonly string GetUserByUserCodeQuery = @"Select Id,FirstName,LastName,Email,PhoneNumber, PartnerUserId FROM [User] WHERE IsActive =@IsActive And IsDeleted =@IsDeleted And UserCode IN @UserIds";

        #endregion

        #region I2C
        public static readonly string GetI2CBankAccountsByUserQuery = @"Select cba.* from [dbo].[i2cCardBankAccount] cba
INNER JOIN i2caccountdetail icd ON icd.Id=cba.I2cAccountDetailId AND icd.UserId=@ToUserId
WHERE cba.UserId=@ByUserId";

        #endregion

        #region PlanProgramAccountsLinking
        public static readonly string GetProgramAccountIdsByPlanIds = @"SELECT programAccountId FROM PlanProgramAccountsLinking WHERE planId IN (@PlanId) ";
        #endregion

        #region UserTransactionsInfo
        public static readonly string GetUserAvailableBalance = @"Select 1 as DataKeyType,'Meal Passes' as DataKey,convert(varchar,convert(decimal(8,0), 
                                                            ISNULL([dbo].[fnGetUserBalanceDetailWithProgram](@userId,1,0,0,1,@programId),0))) as DataValue
                                                            UNION
                                                            Select 2 as DataKeyType,'Flex Points' as DataKey, 
                                                            convert(varchar,convert(decimal(8,0), ISNULL([dbo].[fnGetUserBalanceDetailWithProgram](@userId,1,0,0,2,@programId),0))) as DataValue
                                                            UNION 
                                                            Select 3 as DataKeyType,'Discretionary' as DataKey,
                                                        convert(varchar,convert(decimal(8,2), ISNULL((Select Balance FROM I2cAccountDetail WHERE userId=@userId),0))) as DataValue 
                                                            UNION
                                                            Select 4 as DataKeyType,'Last Reload Date' as DataKey,
                                                            [dbo].[fnGetFormattedNthDate](ISNULL((Select MAX(transactionDate) FROM UserTransactionInfo WHERE CredituserId=@userId AND CreditTransactionUserType=1 AND AccountTypeId=3 AND (DebitTransactionUserType=2 OR DebitTransactionUserType=1) AND IsActive=1 AND IsDeleted=0),NULL)) as DataValue";

        public static readonly string GetUserAvailableBalanceForVPL = @"
                                                            Select 3 as DataKeyType,'Discretionary' as DataKey,
                                                        convert(varchar,convert(decimal(8,2), ISNULL((Select Balance FROM I2cAccountDetail WHERE userId=@userId),0))) as DataValue";

        public static readonly string GetRespectiveUserTransaction = @"
SELECT Amount = CASE WHEN ut.AccountTypeId=3 THEN 
 convert(varchar,convert(decimal(8,2), ISNULL(ut.TransactionAmount,0)))
 ELSE  convert(varchar,convert(decimal(8,0), ISNULL(ut.TransactionAmount,0)))
 END,
        [Date] = Convert(varchar(10),ut.TransactionDate,103),
        [Time] = CONVERT(varchar(15),CAST(ut.TransactionDate AS TIME),100),
        AccountType = acct.AccountType,
        MerchantFullName = organisation.Name ,
		ISNULL(p.photoPath,NULL) as ImagePath
        FROM [User] AS u 
        INNER JOIN UserTransactionInfo AS ut ON u.Id = ut.DebitUserId AND ut.DebitTransactionUserType=1
        INNER JOIN UserProgram AS up ON ut.DebitUserId = up.UserId AND ut.ProgramId = up.ProgramId
        INNER JOIN AccountType AS acct ON ut.AccountTypeId = acct.Id
        LEFT JOIN Organisation AS organisation ON organisation.Id =ut.CreditUserId AND ut.CreditTransactionUserType=3
		LEFT JOIN photo AS p ON ut.CreditUserId = p.EntityId AND p.photoType = @photoType
        WHERE u.Id = @linkedUserId AND ut.DebitUserId = @LinkedUserId AND ut.CreditUserId != @LinkedUserId AND
        (@DateMonth IS NULL OR (Month(ut.TransactionDate) = MONTH(@DateMonth)) AND YEAR(ut.TransactionDate) = YEAR(@DateMonth))
		AND (@ProgramId is NULL OR ut.ProgramId=@ProgramId)";
        #endregion

        #region Branding
        public static readonly string GetBrandingsMobileQueryByProgram = @"
                                                            SELECT DISTINCT
                                                            pp.name as PlanName,
                                                            pp.Id as PlanId,
                                                            pa.accountName as AccountName,
                                                            pa.Id as AccountId,
                                                            ISNULL(Img.PhotoPath,NULL) AS BrandingImagePath,
                                                            pb.AccountTypeId,
                                                            CardNumber,
                                                            BrandingColor,
                                                            at.AccountType as AccountTypeName
                                                            FROM ProgramBranding pb
                                                            INNER JOIN ProgramAccounts pa on pa.id=pb.programAccountID 
                                                            AND pa.isActive=@IsActive AND pa.isDeleted=@IsDeleted
                                                            INNER JOIN PlanProgramAccountsLinking pal 
                                                            on pal.programAccountId=pa.id
                                                            INNER JOIN AccountType at 
                                                            ON at.Id=pb.AccountTypeId
                                                            INNER JOIN ProgramPackage pp on pp.id=pal.planId 
                                                            AND pp.isActive=@IsActive AND pp.isDeleted=@IsDeleted
                                                            INNER JOIN UserPlans up ON up.programPackageId=pp.id
                                                            INNER JOIN [User] usr ON usr.Id=up.UserId AND 
                                                            usr.IsActive=1 AND usr.IsDeleted=0
                                                            LEFT JOIN Photo AS img ON pb.Id=img.EntityId 
                                                            AND img.PhotoType=" + (int)PhotoEntityType.BrandingLogo + "" +
                                                            "WHERE pp.programId=@ProgramId " +
            "                                                AND pb.isActive=@IsActive AND pb.isDeleted=@IsDeleted AND usr.Id=@UserID";

        public static readonly string GetUserDeviceTokenByProgram = @"Select usr.Id,usr.UserDeviceId,usr.UserDeviceType from [User] usr
                                                            INNER JOIN UserProgram usrPrg On usr.Id=usrPrg.userId
                                                            INNER JOIN Program prg On prg.id= usrPrg.programId AND prg.isActive= 1 
                                                            AND prg.isDeleted= 0
                                                            WHERE usr.IsActive= 1 AND
                                                            usr.IsDeleted= 0 AND
                                                            usr.EmailConfirmed= 1 AND
                                                            usr.PhoneNumberConfirmed= 1 And
                                                            usr.InvitationStatus= 3 AND
                                                            usr.IsMobileRegistered= 1 AND usrPrg.programId= @ProgramId";

        public static readonly string GetUserDeviceTokenByProgramLst = @"Select usr.Id,usr.UserDeviceId,usr.UserDeviceType from [User] usr
                                                            INNER JOIN UserProgram usrPrg On usr.Id=usrPrg.userId
                                                            INNER JOIN Program prg On prg.id= usrPrg.programId AND prg.isActive= 1 
                                                            AND prg.isDeleted= 0
                                                            WHERE usr.IsActive= 1 AND
                                                            usr.IsDeleted= 0 AND
                                                            usr.EmailConfirmed= 1 AND
                                                            usr.PhoneNumberConfirmed= 1 And
                                                            usr.InvitationStatus= 3 AND
                                                            usr.IsMobileRegistered= 1 AND prg.Id IN @ProgramId";

        public static readonly string GetUserDeviceTokenByUserId = @"Select usr.Id,usr.UserDeviceId,usr.UserDeviceType from [User] usr
                                                            INNER JOIN UserProgram usrPrg On usr.Id=usrPrg.userId
                                                            INNER JOIN Program prg On prg.id= usrPrg.programId AND prg.isActive= 1 
                                                            AND prg.isDeleted= 0
                                                            WHERE usr.IsActive= 1 AND
                                                            usr.IsDeleted= 0 AND
                                                            usr.EmailConfirmed= 1 AND
                                                            usr.PhoneNumberConfirmed= 1 And
                                                            usr.InvitationStatus= 3 AND
                                                            usr.IsMobileRegistered= 1 AND usrPrg.programId= @ProgramId AND usr.Id=@UserId";

        public static readonly string GetUserFavoriteByOrganisations = @"Select uf.userId from UserFavorites uf
                                                                INNER JOIN [User] usr ON usr.Id=uf.UserId AND usr.IsActive=1 AND usr.IsDeleted=0
                                                                WHERE uf.userId IN @UserIds AND 
                                                                uf.orgnisationId=@MerchantId AND uf.isFavorite=1";

        public static readonly string GetUserNotificationSettingByNotificaction = @"Select uns.userId from UserNotificationSettings uns
                                                            INNER JOIN [User] usr ON usr.Id=uns.UserId AND usr.IsActive=1 AND usr.IsDeleted=0
                                                            WHERE uns.notificationId = @NotificationId
                                                            AND uns.userid IN @UserIds AND uns.isNotificationEnabled=1";

        #endregion

        #region Rewards
        public static readonly string GetAchievedRewardDetailInformation = @"Select 
urpl.Id as RewardProgressId,
p.Id as RewardId,
p.name as RewardTitle,
p.bannerDescription as RewardSubTitle,
p.[description] as [Description],
p.startDate as StartDate,
p.EndDate as EndDate,
p.BusinessTypeId,
CONCAT(@BaseURL,bt.IconPath) as IconPath,
p.backgroundColor As RewardBackColor,
Org.name as MerchantName
 from Promotion p
INNER JOIN UserRewardsProgressLinking urpl ON urpl.rewardId=p.Id AND urpl.userId=@UserId
INNER JOIN BusinessType bt ON p.businessTypeId=bt.Id
INNER JOIN Organisation Org ON org.Id=p.MerchantID
INNER JOIN OrganisationProgram orgPrg ON orgPrg.OrganisationId=org.Id AND orgPrg.ProgramId=@ProgramId
WHERE p.IsActive=1 AND p.IsDeleted=0 AND offerTypeId = @OfferTypeId  AND p.IsPublished=1
AND (
(p.OfferSubTypeId= 3 AND (p.noOfVisits= urpl.rewardProgressAchieved))
OR
(p.OfferSubTypeId= 4 AND (p.amounts= urpl.rewardProgressAchieved))
 )
 AND org.IsActive=1 AND org.IsDeleted= 0 AND urpl.IsRedeemed=0
 AND CONVERT(VARCHAR(10), GETUTCDATE(), 111) BETWEEN DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.StartTime),CAST(p.StartDate AS DATETIME)) AND DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.EndTime),CAST(p.EndDate AS DATETIME))";

        public static readonly string GetAllRewardsInformation = @"Select 
p.Id as RewardId,
p.name as RewardTitle,
p.bannerDescription as RewardSubTitle,
p.[description] as [Description],
p.startDate as StartDate,
p.EndDate as EndDate,
p.BusinessTypeId,
CONCAT('',bt.IconPath) as IconPath,
urpl.rewardProgressAchieved,
 CASE p.OfferSubTypeId
           WHEN 3
           THEN p.noOfVisits
           WHEN 4
           THEN p.amounts
      END AS TotalRewardPointsToAchieve,
	  p.OfferSubTypeId,
Org.name as MerchantName,
LeftAchievementProgressLine= CASE
  WHEN p.OfferSubTypeId= 3 THEN CONCAT(p.noOfVisits-ISNULL(urpl.rewardProgressAchieved,0), ' more visits to win ',bt.name) 
WHEN p.OfferSubTypeId= 4 THEN CONCAT(p.amounts-ISNULL(urpl.rewardProgressAchieved,0), ' amount to pay to win ',bt.name) 
 ELSE ''
  END
 from Promotion p
LEFT JOIN UserRewardsProgressLinking urpl ON urpl.rewardId=p.Id AND urpl.userId=@UserId
INNER JOIN BusinessType bt ON p.businessTypeId=bt.Id
INNER JOIN Organisation Org ON org.Id=p.MerchantID AND org.OrganisationType=3
INNER JOIN OrganisationProgram orgPrg ON orgPrg.OrganisationId=org.Id AND orgPrg.ProgramId=@ProgramId
WHERE p.IsActive=1 AND p.IsDeleted=0 AND p.offerTypeId = @OfferTypeId AND p.IsPublished=1
AND (
(p.OfferSubTypeId= 3 AND (urpl.rewardProgressAchieved IS null OR p.noOfVisits!=urpl.rewardProgressAchieved))
OR
(p.OfferSubTypeId= 4 AND (urpl.rewardProgressAchieved IS null OR p.amounts!=urpl.rewardProgressAchieved))
 )
 AND org.IsActive=1 AND org.IsDeleted= 0  
 AND CONVERT(VARCHAR(10), GETUTCDATE(), 111) BETWEEN DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.StartTime),CAST(p.StartDate AS DATETIME)) AND DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.EndTime),CAST(p.EndDate AS DATETIME))";

        #endregion

        #region User Pushed Notification List
        public static readonly string GetUserPushedNotificationListSP = @"UserPushNotificationList";
        public static readonly string GetNotificationLogsBySP = @"GetNotificationsLogs";
        public static readonly string GetNotificationLogsWithFilterBySP = @"GetNotificationsLogsWithFilter";
        public static readonly string GetUserPushedUnreadNotificationCount = @"IF EXISTS (Select * from UserPushedNotificationsStatus  where UserId=@UserId )
BEGIN
         Select Count(upn.ID)
   from UserPushedNotifications upn
   INNER JOIN NotificationSettings nts ON nts.Id=upn.notificationType
   Where upn.IsActive=@IsActive AND upn.IsDeleted=@IsDeleted 
   AND (upn.UserId=0 OR upn.UserId=@UserId)
   AND (upn.ProgramId=0 OR upn.ProgramId=@ProgramId)
   AND upn.ID>( Select ISNULL(notificationId,0) from 
   UserPushedNotificationsStatus where UserId=@UserId)
   GROUP BY upn.ID,upn.CreatedDate
   ORDER BY upn.CreatedDate DESC
   END
   ELSE 
   BEGIN
   Select Count(upn.ID)
   from UserPushedNotifications upn
   INNER JOIN NotificationSettings nts ON nts.Id=upn.notificationType
   Where upn.IsActive=@IsActive AND upn.IsDeleted=@IsDeleted 
   AND (upn.UserId=0 OR upn.UserId=@UserId)
   AND (upn.ProgramId=0 OR upn.ProgramId=@ProgramId)
      GROUP BY upn.ID,upn.CreatedDate
   ORDER BY upn.CreatedDate DESC
   END
";
        #endregion

        #region SchedulerNotifications
        public static readonly string GetUserRewardsBasedOnCurrentDate = @"Select 
                                                                    p.Id as RewardId,
                                                                    p.startDate as StartDate,
                                                                    p.EndDate as EndDate,
                                                                    p.BusinessTypeId,
                                                                    p.OfferSubTypeId,
                                                                    org.Id as MerchantId,
                                                                     orgPrg.ProgramId as ProgramId,
                                                                        p.CreatedBy,
                                                                Org.name as MerchantName
                                                                     from Promotion p
                                                                   LEFT OUTER JOIN UserRewardsProgressLinking urpl ON urpl.rewardId=p.Id 
                                                                   --LEFT OUTER JOIN UserPushedNotifications upn ON upn.referenceId=p.Id AND upn.IsActive=1 AND upn.IsDeleted=0
                                                                    INNER JOIN BusinessType bt ON p.businessTypeId=bt.Id
                                                                    INNER JOIN Organisation Org ON org.Id=p.MerchantID
                                                                    INNER JOIN OrganisationProgram orgPrg ON orgPrg.OrganisationId=org.Id 
                                                                    WHERE p.IsActive=1 AND p.IsDeleted=0 AND offerTypeId = @OfferTypeId AND p.IsPublished=1
                                                                    AND (
                                                                    (p.OfferSubTypeId= 3 AND (urpl.rewardProgressAchieved IS null OR p.noOfVisits!=urpl.rewardProgressAchieved))
                                                                    OR
                                                                    (p.OfferSubTypeId= 4 AND (urpl.rewardProgressAchieved IS null OR p.amounts!=urpl.rewardProgressAchieved))
                                                                     )
                                                                     AND org.IsActive=1 AND org.IsDeleted= 0 AND
                                                                    p.Id NOT IN (Select upn.referenceId from UserPushedNotifications upn)
                                                                      AND CONVERT(VARCHAR(10), GETUTCDATE(), 111) BETWEEN DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.StartTime),CAST(p.StartDate AS DATETIME)) AND DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.EndTime),CAST(p.EndDate AS DATETIME)) Order By p.StartDate desc";


        public static readonly string GetUserOffersBasedOnCurrentDate = @"Select 
                                                                    p.Id as RewardId,
                                                                    p.startDate as StartDate,
                                                                    p.EndDate as EndDate,
                                                                    p.OfferSubTypeId,
                                                                    org.Id as MerchantId,
                                                                     orgPrg.ProgramId as ProgramId,
                                                                        p.CreatedBy,
                                                                Org.name as MerchantName
                                                                     from Promotion p
                                                                   -- LEFT OUTER JOIN UserPushedNotifications upn ON upn.referenceId=p.Id AND upn.IsActive=1 AND upn.IsDeleted=0
                                                                    INNER JOIN Organisation Org ON org.Id=p.MerchantID
                                                                    INNER JOIN OrganisationProgram orgPrg ON orgPrg.OrganisationId=org.Id 
                                                                    WHERE p.IsActive=1 AND p.IsDeleted=0 AND offerTypeId = @OfferTypeId                                                                  
                                                                     AND org.IsActive=1 AND org.IsDeleted= 0 AND
                                                                     p.Id NOT IN (Select upn.referenceId from UserPushedNotifications upn)
                                                                     AND CONVERT(VARCHAR(10), GETUTCDATE(), 111) BETWEEN DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.StartTime),CAST(p.StartDate AS DATETIME)) AND DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.EndTime),CAST(p.EndDate AS DATETIME)) Order By p.StartDate desc";

        public static readonly string GetUsersToAheadCompleteRewards = @"Select 
                                                                    p.Id as RewardId,
                                                                    p.startDate as StartDate,
                                                                    p.EndDate as EndDate,
                                                                    p.BusinessTypeId,
                                                                    p.OfferSubTypeId,
                                                                    org.Id as MerchantId,
                                                                     orgPrg.ProgramId as ProgramId,
                                                                        p.CreatedBy,
                                                                Org.name as MerchantName
                                                                 from Promotion p
                                                               -- LEFT OUTER JOIN UserPushedNotifications upn ON upn.referenceId=p.Id AND upn.IsActive=1 AND upn.IsDeleted=0 AND upn.PushedNotificationType=1
                                                                INNER JOIN UserRewardsProgressLinking urpl ON urpl.rewardId=p.Id
                                                                INNER JOIN BusinessType bt ON p.businessTypeId=bt.Id
                                                                INNER JOIN Organisation Org ON org.Id=p.MerchantID
                                                                INNER JOIN OrganisationProgram orgPrg ON orgPrg.OrganisationId=org.Id
                                                                WHERE p.IsActive=1 AND p.IsDeleted=0 AND offerTypeId = @OfferTypeId AND p.IsPublished=1
                                                                AND (
                                                                (p.OfferSubTypeId= 3 AND (p.noOfVisits-urpl.rewardProgressAchieved=1))
                                                                OR
                                                                (p.OfferSubTypeId= 4 AND (p.amounts- urpl.rewardProgressAchieved<=100))
                                                                 )
                                                                 AND org.IsActive=1 AND org.IsDeleted= 0 AND urpl.IsRedeemed=0 AND
                                                                    p.Id NOT IN (Select upn.referenceId from UserPushedNotifications upn)
                                                                 AND CONVERT(VARCHAR(10), GETUTCDATE(), 111) BETWEEN DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.StartTime),CAST(p.StartDate AS DATETIME)) AND DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.EndTime),CAST(p.EndDate AS DATETIME)) Order By p.StartDate desc";

        public static readonly string GetUsersCompletedRewardsForNotifications = @"Select 
                                                                    p.Id as RewardId,
                                                                    p.startDate as StartDate,
                                                                    p.EndDate as EndDate,
                                                                    p.BusinessTypeId,
                                                                    p.OfferSubTypeId,
                                                                    org.Id as MerchantId,
                                                                     orgPrg.ProgramId as ProgramId,
                                                                        p.CreatedBy,
                                                                Org.name as MerchantName
                                                                 from Promotion p
                                                                --LEFT OUTER JOIN UserPushedNotifications upn ON upn.referenceId=p.Id AND upn.IsActive=1 AND upn.IsDeleted=0 AND upn.PushedNotificationType=2
                                                                INNER JOIN UserRewardsProgressLinking urpl ON urpl.rewardId=p.Id
                                                                INNER JOIN BusinessType bt ON p.businessTypeId=bt.Id
                                                                INNER JOIN Organisation Org ON org.Id=p.MerchantID
                                                                INNER JOIN OrganisationProgram orgPrg ON orgPrg.OrganisationId=org.Id
                                                                WHERE p.IsActive=1 AND p.IsDeleted=0 AND offerTypeId = @OfferTypeId AND p.IsPublished=1
                                                                AND (
                                                                (p.OfferSubTypeId= 3 AND (p.noOfVisits=urpl.rewardProgressAchieved))
                                                                OR
                                                                (p.OfferSubTypeId= 4 AND (p.amounts=urpl.rewardProgressAchieved))
                                                                 )
                                                                 AND org.IsActive=1 AND org.IsDeleted= 0 AND urpl.IsRedeemed=0 AND
                                                                 p.Id NOT IN (Select upn.referenceId from UserPushedNotifications upn)
                                                                 AND CONVERT(VARCHAR(10), GETUTCDATE(), 111) BETWEEN DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.StartTime),CAST(p.StartDate AS DATETIME)) 
                                                                 AND DATEADD(millisecond ,DATEDIFF(millisecond, 0, p.EndTime),CAST(p.EndDate AS DATETIME)) Order By p.StartDate desc-";
        #endregion

        #region Cardholder Agreement
        public static readonly string GetUserAgreementHistory = @"
; WITH AgreementHistory AS (
  Select Distinct
usr.Id as UserId,
FirstName+' '+LastName as CardHolderName
From UserAgreementHistory uah
INNER JOIN [User] usr ON usr.Id=uah.UserId AND usr.IsActive=1 AND usr.IsDeleted=0
Where uah.ProgramId=@ProgramId 
)
SELECT ROW_NUMBER() OVER (ORDER BY UserId desc) AS RowNum,* FROM AgreementHistory";

        public static readonly string GetUserAgreementHistoryVersions = @"SELECT ROW_NUMBER() OVER (ORDER BY uah.dateAccepted desc) AS RowNum,
  usr.Id as UserId,
  uah.dateAccepted,
  uah.cardHolderAgreementVersionNo as VersionNo
From UserAgreementHistory uah
INNER JOIN [User] usr ON usr.Id=uah.UserId AND usr.IsActive=1 AND usr.IsDeleted=0
Where uah.ProgramId=@ProgramId and uah.UserId=@UserId ";

        public static readonly string GetCardHolderAgreementsBasedOnProgram = @"Select 
                                            row_number() over(order by CreatedDate desc) as SrNo,
                                            id as CardHolderAgreementId,
                                            ProgramId,
                                            CardHoldrAgreementContent as CardHolderAgreementContent,
                                            versionNo,
                                            IsActive,
                                            IsDeleted,
                                            CreatedDate
                                            From CardHolderAgreement
                                            Where ProgramId=@ProgramId ANd isActive=1 AND isDeleted=0
                                            Order by CreatedDate desc";

        public static readonly string GetCardHolderAgreementsBasedOnProgramNId = @"Select Top(1)
                                            id as CardHolderAgreementId,
                                            ProgramId,
                                            CardHoldrAgreementContent as CardHolderAgreementContent,
                                            versionNo,
                                            IsActive,
                                            IsDeleted,
                                            CreatedDate
                                            From CardHolderAgreement
                                            Where ProgramId=@ProgramId AND (@Id=0 OR id=@Id) ANd isActive=1 AND isDeleted=0
                                            Order by CreatedDate desc";

        public static readonly string GetCardHolderAgreementsBasedOnProgramIdAndUser = @"
                                            Select Top(1)
                                            cha.Id as CardHolderAgreementId,
                                            cha.cardHoldrAgreementContent,
                                            cha.CreatedDate,
                                            cha.VersionNo,
                                            IsAgreementRead=(CASE 
                                            WHEN usr.IsAgreementRead=0 OR usr.IsAgreementRead IS NULL THEN 0
                                            WHEN usr.AgreementReadDateTime IS NULL THEN 0
                                            When cha.CreatedDate>usr.AgreementReadDateTime THEN 0
                                             WHEN cha.CreatedDate<=usr.AgreementReadDateTime THEN 1
                                            END ),
                                            IsDiscretionaryAccountType=(CASE 
                                            WHEN pa.AccountTypeId IS NULL THEN 0
                                            When pa.AccountTypeId<=0 THEN 0
                                             ELSE 1
                                            END )
                                            from CardHolderAgreement cha
                                            INNER JOIN ProgramAccounts pa ON pa.programId=cha.programID AND pa.AccountTypeId=3
                                            INNER JOIN [User] usr ON usr.ProgramId=pa.ProgramId AND usr.Id=@UserId
                                            INNER JOIN Program p ON p.Id=usr.ProgramID                                            
                                            Where 
                                            pa.IsActive=1 AND pa.IsDeleted=0 AND cha.IsActive=1 ANd cha.IsDeleted=0 AND p.Id=@ProgramId
                                            Order By cha.createdDate desc
                                            ";


        public static readonly string GetCardHolderAgreementsExistence = @"
                                            Select Top(1)
                                            cha.Id,
                                            cha.cardHoldrAgreementContent,
                                            cha.CreatedDate,
                                           cha.VersionNo,
											pa.AccountTypeId
                                            from ProgramAccounts pa
                                            LEFT JOIN  CardHolderAgreement cha ON pa.programId=cha.programID AND cha.IsActive=1 ANd cha.IsDeleted=0
                                            INNER JOIN Program p ON p.Id=pa.ProgramID                                            
                                            Where 
                                            pa.IsActive=1 AND pa.IsDeleted=0 
											AND p.Id=@ProgramId  AND pa.AccountTypeId=3
                                            Order By cha.createdDate desc";
        #endregion

        #region Program Types
        public static readonly string GetProgramTypeDetailByIdsQuery = @"Select Id,ProgramTypeName From ProgramType WHERE Id IN @programTypeId";
        #endregion
        #region Payment
        public static readonly string GetLatestWebToken = @"Select Top(1) Token,ipgFirstTransactionId From GatewayCardWebHookToken WHERE CreditUserId=@CreditUserId and DebitUserId=@DebitUserId order by TokenReceivedDate desc";
        public static readonly string GetLatestWebTokenByClientToken = @"Select Top(1) Token,ipgFirstTransactionId,expiryMonthYear,schemetransactionID From GatewayCardWebHookToken WHERE ClientToken=@clientToken";
        public static readonly string GetBinNumberIsValid = @"SELECT Top(1) * FROM dbo.BinData WHERE @bin BETWEEN BinNumberStart AND BinNumberEnd  AND   ISNULL([Delete],0)=0";
        #endregion

        #region LoyalitySetting
        public static readonly string GetLoyalityGlobalSettingQuery = @"IF EXISTS(SELECT* FROM OrgLoyalityGlobalSettings WHERE organisationId = @id)
                                                                        BEGIN SELECT* FROM OrgLoyalityGlobalSettings WHERE organisationId =@id END ELSE 
                                                                        SELECT 0 as id, 0 as organisationId, 0 as loyalityThreshhold,0 as globalReward,
                                                                        0 as globalRatePoints,0 as bitePayRatio,0 as dcbFlexRatio,0 as userStatusVipRatio,0 as userStatusRegularRatio,  null as createdDate, null as modifiedDate,0 as FirstTransactionBonus";
        public static readonly string GetSiteLevelOverrideSettingQuery = @"Select * from SiteLevelOverrideSettings Where programId=@id";
        public static readonly string GetSiteLevelOverrideSettingQuery1 = @"IF EXISTS(SELECT* FROM SiteLevelOverrideSettings WHERE programId = @id) BEGIN SELECT* FROM SiteLevelOverrideSettings WHERE programId =@id END ELSE SELECT 0 as id, 0 as programId, 0 as siteLevelBitePayRatio,0 as siteLevelDcbFlexRatio,0 as siteLevelUserStatusVipRatio,0 as siteLevelUserStatusRegularRatio,null as createdDate, null as modifiedDate,0 as FirstTransactionBonus";
        #endregion

        #region UserLoyalityPoints
        public static readonly string GetSitelevelOverrideSettingsbyUserProgramId = @"SELECT [User].Id, [User].ProgramId, SiteLevelOverrideSettings.siteLevelBitePayRatio, SiteLevelOverrideSettings.siteLevelDcbFlexRatio, 
                                                                         SiteLevelOverrideSettings.siteLevelUserStatusVipRatio, SiteLevelOverrideSettings.siteLevelUserStatusRegularRatio,SiteLevelOverrideSettings.FirstTransactionBonus
                                                                     FROM            dbo.[User] INNER JOIN
                                                     SiteLevelOverrideSettings ON [User].ProgramId = SiteLevelOverrideSettings.programId
						                                         where [User].Id=@id";
        public static readonly string GetUserLoyaltyPointsHistoryQuery = @"Select top 1 * from UserLoyaltyPointsHistory Where userId=@id order by id desc";
        public static readonly string GetUserLeftLoyaltyPointsQuery = @"Select * from UserLeftLoyalityPoints Where userId=@id";
        public static readonly string GetUserLoyaltyTrackingHistoryQuery = @"SELECT   CASE  WHEN isThresholdReached=1 THEN  totalPoints-leftOverPoints ELSE 0.00 END [pointDebited] ,*
		     FROM  dbo.UserLoyaltyPointsHistory WHERE userId =@id  ORDER BY id DESC";
        public static readonly string GetUserLoyaltyTrackingTransactionsBySP = @"GetLoyaltyTrackingTransactions";

        #endregion

        #region PartnerNotificationLog
        public static readonly string GetApiNameQuery = @"select distinct ApiName from PartnerNotificationsLog";
        public static readonly string GetStatusQuery = @"select distinct Status from PartnerNotificationsLog where Status is not null";
        public static readonly string GetAllProgramQuery = @"select * from Program where organisationId=@organisationId";
        #endregion

        public static readonly string InsertUpdateBinData = @"InsertUpdateBinData";
        public static readonly string Sp_UpdateIssuer = @"SP_UPDATEISSUER";
        public static readonly string GetInvitationById = @"SELECT id, FirstName,LastName, Email, PhoneNumber,IsRequestAccepted FROM dbo.Invitation WHERE Id = @id";
    }
}
