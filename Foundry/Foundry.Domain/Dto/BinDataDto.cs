using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
   public class BinDataDto
    {
        public  int Id { get; set; }
        
        public  long? BinNumberStart { get; set; }
       
        public  long? BinNumberEnd { get; set; }
       
        public  string CountryCode { get; set; }
        
        public  bool? Delete { get; set; }
       
        public  DateTime CreatedDate { get; set; }
     
        public  DateTime? UpdatedDate { get; set; }
    }
}
