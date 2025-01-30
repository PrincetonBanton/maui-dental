using SQLite;

namespace DentalApp.Models
{
    public class Role
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
