using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class PhotosDto
    {
        public int Id { get; set; }
        public string photoPath { get; set; }
        public int? entityId { get; set; }
        public int? photoType { get; set; }
        public int? createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? updatedDate { get; set; }
    }

    public class PhotosExpirationDto
    {
        public string photoPath { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime expirationDate { get; set; }
    }
}
