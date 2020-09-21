using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain
{
  public  class ProgramModel
    {
        public  int id { get; set; }
      
        public  string name { get; set; }
       
        public  string description { get; set; }
       
        public  int? organisationId { get; set; }
       
    }
}
