using System.ComponentModel.DataAnnotations;

namespace DentalApp.Models
{
    public abstract class BaseModel
    {
        [Key]
        public virtual int Id { get; set; }
    }
}
