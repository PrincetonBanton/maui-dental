namespace DentalApp
{
    public partial class App : Application
    {
        public static App Instance { get; private set; }

        // Global navigation state variables
        public string UserNavigated { get; set; } = "start";
        public string PatientNavigated { get; set; } = "start";
        public string DentistNavigated { get; set; } = "start";
        public bool IsConnected { get; set; } = true;

        public App()
        {
            InitializeComponent();
            Instance = this;
            MainPage = new AppShell();
        }
    }
}
