using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public static class DateSuffixModel
    {
        public static string GetSuffix(string day)
        {
            string suffix = "th";

            if (int.Parse(day) < 11 || int.Parse(day) > 20)
            {
                var DayToCharArray = day.ToCharArray();
                day = DayToCharArray[DayToCharArray.Length - 1].ToString();
                switch (day)
                {
                    case "1":
                        suffix = "st";
                        break;
                    case "2":
                        suffix = "nd";
                        break;
                    case "3":
                        suffix = "rd";
                        break;
                }
            }

            return suffix;
        }
    }
}
