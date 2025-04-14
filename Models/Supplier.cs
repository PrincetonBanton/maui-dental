using DentalApp.Models;
using DentalApp.Models.Enum;
using System.Collections.Generic;

namespace DentalApp.Models
{
    public partial class Supplier : BaseModel
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Mobile { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

    }
}
