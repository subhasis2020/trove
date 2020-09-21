using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class UserColumnsDto
    {
        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }
        public string CHARACTER_MAXIMUM_LENGTH { get; set; }
        public string NUMERIC_PRECISION { get; set; }
        public string DATETIME_PRECISION { get; set; }
        public string IS_NULLABLE { get; set; }
    }
}
