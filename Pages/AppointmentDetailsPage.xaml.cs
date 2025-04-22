namespace DentalApp.Pages;

public partial class AppointmentDetailsPage : ContentPage
{
	public AppointmentDetailsPage()
	{
		InitializeComponent();
        PopulateTimePickers();
    }

    private void PopulateTimePickers()
    {
        var startTime = new TimeSpan(8, 0, 0); // 8:00 AM
        var endTime = new TimeSpan(17, 0, 0);  // 5:00 PM

        var times = new List<string>();
        while (startTime <= endTime)
        {
            times.Add(DateTime.Today.Add(startTime).ToString("hh:mm tt"));
            startTime = startTime.Add(TimeSpan.FromMinutes(30));
        }

        StartTimePicker.ItemsSource = times;
        EndTimePicker.ItemsSource = times;
    }
    private void StartTimePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (StartTimePicker.SelectedItem is string selectedTimeStr &&
            DateTime.TryParse(selectedTimeStr, out DateTime selectedStartTime))
        {
            var newEndTime = selectedStartTime.AddHours(1);
            string formattedNewEndTime = newEndTime.ToString("hh:mm tt");
            var index = EndTimePicker.ItemsSource.IndexOf(formattedNewEndTime);

            // If found, select it
            EndTimePicker.SelectedIndex = index >= 0 ? index : -1;
        }
    }
    private void EndTimePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var startText = StartTimePicker.SelectedItem as string;
        var endText = EndTimePicker.SelectedItem as string;

        if (DateTime.TryParse(startText, out var startTime) &&
            DateTime.TryParse(endText, out var endTime))
        {
            if (endTime <= startTime)
            {
                DisplayAlert("Invalid Time", "End time must be later than start time.", "OK");
                EndTimePicker.SelectedIndex = -1; // Clear invalid selection
            }
        }
    }


}