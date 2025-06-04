using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace DentalApp.Services
{
    public static class FrameAnimationService
    {
        public static async Task ToggleVisibility(Frame frame)
        {
            if (frame == null) return;

            if (!frame.IsVisible)
            {
                frame.TranslationY = -500; // Start above the screen
                frame.IsVisible = true;
                await frame.TranslateTo(0, 0, 250, Easing.SinInOut); // Animate down
            }
            else
            {
                await frame.TranslateTo(0, -500, 250, Easing.SinInOut); // Animate up
                frame.IsVisible = false;
            }
        }
    }
}
