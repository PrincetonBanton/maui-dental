using DentalApp.Models.Enum;
using DentalApp.Models;
using System.Collections.Generic;
using SQLite;

namespace DentalApp.Models
{
    public partial class Patient : BaseModel
    {
        public string? Occupation { get; set; }
        public int UserId { get; set; }
        public string? PatientNo { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public decimal? AmountOwing { get; set; }

        public virtual User? User { get; set; }

        //public virtual ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();

        //public virtual ICollection<Note> Notes { get; set; } = new HashSet<Note>();
    }
}
