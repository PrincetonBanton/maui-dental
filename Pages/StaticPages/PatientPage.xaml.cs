using DentalApp.Models;

namespace DentalApp.Pages.StaticPages
{
    public partial class PatientPage : ContentPage
    {
        public PatientPage()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            var sampleUsers = new List<User>
            {
                new User { FirstName = "Juan", LastName = "dela Cruz", Email = "trial@email.com", Mobile = "09514652627", BirthDate = new DateTime(2025, 1, 1) },
                new User { FirstName = "Maria", LastName = "Santos", Email = "maria@email.com", Mobile = "0987654321", BirthDate = new DateTime(1995, 2, 15) }
            };

            UserListView.ItemsSource = sampleUsers;
        }
    }
}
