﻿using System;

namespace Foundry.Domain.Dto
{
    public class BenefactorNotificationsDto
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserId { get; set; }
        public string ImagePath { get; set; }
        public string Message { get; set; }
        public bool IsInvitation { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ReloadRequestId { get; set; }
        public int ProgramId { get; set; }
    }
}
