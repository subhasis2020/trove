using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class WeekDayDto
    {
        public string DayName { get; set; }
        public int Id { get; set; }
    }
    public class PassTypeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
    public class ResetPeriodDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
    public class ExchangeResetPeriodDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
    public class InitialBalanceDto
    {
        public int Id { get; set; }
        public int Balance { get; set; }
    }
}
