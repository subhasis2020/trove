using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
  public  class PartnerNotificationsLogDto
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
        public  long TotalCount { get; set; }
        public string PartnerUserId { get; set; }

    }
}
