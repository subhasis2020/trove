USE [FoundryStaging]
GO
SET IDENTITY_INSERT [dbo].[OrgLoyalityGlobalSettings] ON 

INSERT [dbo].[OrgLoyalityGlobalSettings] ([id], [organisationId], [loyalityThreshhold], [globalReward], [globalRatePoints], [bitePayRatio], [dcbFlexRatio], [userStatusVipRatio], [userStatusRegularRatio], [createdDate], [modifiedDate], [FirstTransactionBonus]) VALUES (1, 71, CAST(300.00 AS Decimal(18, 2)), CAST(2.50 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(2.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(4.00 AS Decimal(18, 2)), CAST(3.00 AS Decimal(18, 2)), CAST(N'2020-06-09T03:34:55.127' AS DateTime), CAST(N'2020-07-30T13:46:46.167' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[OrgLoyalityGlobalSettings] OFF
