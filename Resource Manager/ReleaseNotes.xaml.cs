using System.Windows;

namespace Resource_Manager
{
    /// <summary>
    /// Логика взаимодействия для ReleaseNotes.xaml
    /// </summary>
    public partial class ReleaseNotes : Window
    {
        public ReleaseNotes()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
