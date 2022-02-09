using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace Resource_Manager
{
    /// <summary>
    /// Interaktionslogik für ExportDDT.xaml
    /// </summary>
    public partial class ExtractDialog : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Path { get; set; } = "";

        private bool autoDecompress = true;
        private bool autoXMBConversion = false;
        private bool autoDDTToPNGConversion = false;
        private bool autoDDTToTGAConversion = true;
        public bool AutoXMBConversion
        {
            get
            {
                return autoXMBConversion;
            }
            set
            {
                autoXMBConversion = value;
                NotifyPropertyChanged();
            }
        }

        public bool AutoDDTToPNGConversion
        {
            get
            {
                return autoDDTToPNGConversion;
            }
            set
            {
                autoDDTToPNGConversion = value;
                NotifyPropertyChanged();
            }
        }

        public bool AutoDDTToTGAConversion
        {
            get
            {
                return autoDDTToTGAConversion;
            }
            set
            {
                autoDDTToTGAConversion = value;
                NotifyPropertyChanged();
            }
        }

        public bool AutoDecompress
        {
            get
            {
                return autoDecompress;
            }
            set
            {
                autoDecompress = value;
                NotifyPropertyChanged();
            }
        }

        public ExtractDialog(string DefaultRootPath)
        {         
            InitializeComponent();
            ExportPath.Text = Directory.Exists(Settings.Default.lastExportedPath) ? Settings.Default.lastExportedPath : DefaultRootPath;
            DataContext = this;
        }

        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ExportPath.Text = dialog.SelectedPath;
                    Settings.Default.lastExportedPath = dialog.SelectedPath;
                    Settings.Default.Save();
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(ExportPath.Text))
            {
                Path = ExportPath.Text;
                DialogResult = true;
            }
            else
                System.Windows.Forms.MessageBox.Show("The path " + ExportPath.Text + "doesn't exist. Select valid path.");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}