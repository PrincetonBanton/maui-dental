using SQLite;

namespace DentalApp.Models
{
    public class Patient 
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Occupation { get; set; }
        public int? UserId { get; set; }
        public string? PatientNo { get; set; }
        public int Status { get; set; }
        public double? AmountOwing { get; set; }

        [Ignore]
        public User User { get; set; }


    }
}
