using SQLite;

namespace DentalApp.Models
{
    public class Role : BaseModel
    {
        public string Description { get; set; }
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
