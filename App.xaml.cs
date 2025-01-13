namespace DotnetCourseowork
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set the MainPage to use BlazorWebView for Blazor components
            MainPage = new MainPage(); // This should be a page that hosts the BlazorWebView
        }
    }
}