using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

namespace DentalApp.Services
{
    public static class NumericValidationService
    {
        public static bool IsValidNumericInput(string text)
        {
            if (string.IsNullOrEmpty(text)) return true;
            return Regex.IsMatch(text, @"^\d*\.?\d*$");
        }

        public static void OnNumericEntryChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                string newText = entry.Text;
                if (!IsValidNumericInput(newText)) entry.Text = e.OldTextValue;
            }
        }
    }
}
