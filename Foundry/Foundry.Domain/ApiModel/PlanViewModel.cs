using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class PlanViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int programId { get; set; }
        public int? noOfMealPasses { get; set; }
        public int? noOfFlexPoints { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public TimeSpan? startTime { get; set; }
        public TimeSpan? endTime { get; set; }
        public string description { get; set; }
        public string clientId { get; set; }
        public string planId { get; set; }
        public bool? isActive { get; set; }
        public string Jpos_PlanId { get; set; }
        public List<PlanProgramAccountModel> PlanProgramAccount { get; set; }
    }
}
