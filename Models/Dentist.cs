using DentalApp.Models.Enum;
using DentalApp.Models;

namespace DentalApp.Models
{
    public partial class Dentist : BaseModel
    {
        public int UserId { get; set; }
        public DentistStatus Status { get; set; } = DentistStatus.On_Duty;
        public virtual User? User { get; set; }

        //public virtual ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();
    }
}
