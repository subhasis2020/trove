using AutoMapper;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using Dapper;
using Foundry.Domain;
using System.Collections.Generic;
using Foundry.Domain.Dto;

namespace Foundry.Services
{
    public class InvitationService : FoundryRepositoryBase<Invitation>, IInvitationService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IBenefactorsProgram _benefactorprogram;

        public InvitationService(IDatabaseConnectionFactory databaseConnectionFactory
            , IBenefactorsProgram benefactorsProgram)
        : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _benefactorprogram = benefactorsProgram;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<List<Invitation>> GetAllInvitation()
        {
            object obj = new { };
            return (await GetDataAsync(obj)).ToList();
        }


        public async Task<InvitationDto> GetUserInfoById(int inviteid)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<InvitationDto>(SQLQueryConstants.GetInvitationById, new { id = inviteid });

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }

        }





        public async Task<Invitation> GetExistingInvitation(int userId, string emailAddress)
        {
            object obj = new { Email = emailAddress.Trim(), CreatedBy = userId, IsActive = true, IsDeleted = false };
            return await GetDataByIdAsync(obj);
        }
        public async Task<int> AddUpdateInvitationByUser(BenefactorRegisterModel model)
        {
            try
            {
                var invitationId = 0;
                object obj = new { Email = model.EmailAddress.Trim(), CreatedBy = model.UserId, IsActive = true, IsDeleted = false };
                var linkConnection = await GetDataByIdAsync(obj);
                if (linkConnection != null)
                {
                    linkConnection.IsActive = true;
                    linkConnection.IsDeleted = false;
                    linkConnection.FirstName = model.FirstName;
                    linkConnection.LastName = model.LastName;
                    linkConnection.relationshipId = model.RelationshipId;
                    linkConnection.ModifiedDate = DateTime.UtcNow;
                    linkConnection.ModifiedBy = model.UserId;
                    linkConnection.Email = model.EmailAddress;
                    linkConnection.ImagePath = model.BenefactorImagePath;
                    linkConnection.IsRequestAccepted = false;
                    linkConnection.PhoneNumber = model.MobileNumber;
                    linkConnection.InvitationType = InvitationType.Benefactor;
                    linkConnection.programId = model.ProgramId;
                    await UpdateAsync(linkConnection, new { Id = linkConnection.id });
                    invitationId = linkConnection.id;
                }
                else
                {
                    var userConnection = new Invitation
                    {
                        IsActive = true,
                        IsDeleted = false,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        relationshipId = model.RelationshipId,
                        ModifiedDate = DateTime.UtcNow,
                        ModifiedBy = model.UserId,
                        Email = model.EmailAddress,
                        ImagePath = model.BenefactorImagePath,
                        IsRequestAccepted = false,
                        InvitationType = InvitationType.Benefactor,
                        CreatedBy = model.UserId,
                        CreatedDate = DateTime.UtcNow,
                        PhoneNumber = model.MobileNumber,
                        programId = model.ProgramId
                    };
                    await AddAsync(userConnection);
                    invitationId = (await GetDataByIdAsync(new { Email = model.EmailAddress.Trim(), CreatedBy = model.UserId, IsActive = true, IsDeleted = false })).id;
                }
                return invitationId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Invitation> GetExistingInvitationWithEmail(string emailAddress, int userId)
        {
            object obj = new { Email = emailAddress.Trim(), CreatedBy = userId, IsActive = true, IsDeleted = false };
            return await GetDataByIdAsync(obj);
        }
        public async Task<Invitation> GetExistingInvitationWithEmailUserProgram(string emailAddress, int userId, int programId)
        {
            object obj = new { Email = emailAddress.Trim(), IsActive = true, IsDeleted = false };
            if (programId > 0)
            {
                obj = new { Email = emailAddress.Trim(), CreatedBy = userId, ProgramId = programId, IsActive = true, IsDeleted = false };
            }
            return await GetDataByIdAsync(obj);
        }
        public async Task<int> DeleteUserInvitation(int userId, int benefactorId)
        {
            var invitationid = 0;
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    object obj = new { Id = benefactorId, IsActive = true, IsDeleted = false };
                    var user = await sqlConnection.QuerySingleOrDefaultAsync<User>(SQLQueryConstants.GetUserByIdQuery, obj);
                    obj = new { Email = user.Email.Trim(), CreatedBy = userId, IsActive = true, IsDeleted = false };
                    var invitation = await GetDataByIdAsync(obj);
                    if (invitation != null)
                    {
                        invitation.IsDeleted = true;
                        invitation.IsActive = false;
                        invitation.IsRequestAccepted = false;
                        invitation.ModifiedDate = DateTime.UtcNow;
                        invitation.ModifiedBy = benefactorId;
                        await UpdateAsync(invitation, new { Id = invitation.id });
                        invitationid = invitation.id;
                    }
                    return invitationid;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        public async Task<int> AcceptUserInvitation(int userId, int benefactorId, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    object obj = new { Id = benefactorId, IsActive = true, IsDeleted = false };
                    var user = await sqlConnection.QuerySingleOrDefaultAsync<User>(SQLQueryConstants.GetUserByIdQuery, obj);
                    var userEmail = user.Email ?? user.Email;

                    obj = new { Email = user.Email.Trim(), CreatedBy = userId, IsActive = true, IsDeleted = false };
                    var invitation = await GetDataByIdAsync(obj);
                    int invitationId = 0;
                    if (invitation != null)
                    {
                        invitation.IsRequestAccepted = true;
                        await UpdateAsync(invitation, new { Id = invitation.id });
                        var buExist = await sqlConnection.QuerySingleOrDefaultAsync<BenefactorUsersLinking>(SQLQueryConstants.GetConnectionUserBenefactorQuery, new { UserId = userId, BenefactorId = benefactorId, IsActive = true, IsDeleted = false });
                        if (buExist != null)
                        {
                            buExist.isActive = true;
                            buExist.isDeleted = false;
                            buExist.IsRequestAccepted = true;
                            buExist.linkedDateTime = DateTime.UtcNow;
                            buExist.modifiedBy = benefactorId;
                            buExist.modifiedDate = DateTime.UtcNow;
                            buExist.relationshipId = invitation.relationshipId.Value;
                            await sqlConnection.ExecuteAsync(SQLQueryConstants.UpdateBenefactorUserLinkingQuery, buExist);
                            invitationId = buExist.id;
                        }
                        else
                        {
                            var buLinking = new BenefactorUsersLinking
                            {
                                benefactorId = benefactorId,
                                createdBy = benefactorId,
                                linkedDateTime = DateTime.UtcNow,
                                IsRequestAccepted = true,
                                modifiedBy = benefactorId,
                                relationshipId = invitation.relationshipId.Value,
                                userId = userId

                            };
                            invitationId = await sqlConnection.ExecuteAsync(SQLQueryConstants.AddBenefactorUserLinkingQuery, buLinking);
                        }
                        try
                        {
                            var bpExist = (await _benefactorprogram.GetDataByIdAsync(new { BenefactorId = benefactorId, ProgramId = programId }));
                            if (bpExist == null)
                            {
                                var userPg = new BenefactorProgram
                                {
                                    programId = programId,
                                    benefactorId = benefactorId,
                                };
                                invitationId = await _benefactorprogram.AddAsync(userPg);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    return invitationId;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }
    }
}
