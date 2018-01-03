using System.Windows;

namespace PfscTask5
{
    public class UiApplication : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new InfoWindow();
            window.Visibility = Visibility.Visible;
            window.Show();
        }
    }
}
