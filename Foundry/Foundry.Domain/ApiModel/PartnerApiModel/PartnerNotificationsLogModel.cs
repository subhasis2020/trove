using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel.PartnerApiModel
{
   public class PartnerNotificationsLogModel
    {
		public  int Id { get; set; }
		public  int? UserId { get; set; }
		public  string ApiName { get; set; }
		public  string ApiUrl { get; set; }
		public  string Request { get; set; }
		public  string Response { get; set; }
		public  string Status { get; set; }
		public  DateTime CreatedDate { get; set; }
		public  DateTime? UpdatedDate { get; set; }
        public ProgramModel programModel { get; set; }
	}
}
