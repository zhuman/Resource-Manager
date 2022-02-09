using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
<<<<<<< HEAD
using System.IO;
=======

>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f

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
<<<<<<< HEAD
        private bool autoDDTToPNGConversion = false;
        private bool autoDDTToTGAConversion = true;
=======
        private bool autoDDTConversion = false;

>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
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

<<<<<<< HEAD
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
=======
        public bool AutoDDTConversion
        {
            get
            {
                return autoDDTConversion;
            }
            set
            {
                autoDDTConversion = value;
>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
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
<<<<<<< HEAD
            ExportPath.Text = Directory.Exists(Settings.Default.lastExportedPath) ? Settings.Default.lastExportedPath : DefaultRootPath;
=======
            ExportPath.Text = System.IO.Path.GetDirectoryName(DefaultRootPath);
>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
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
<<<<<<< HEAD
                    Settings.Default.lastExportedPath = dialog.SelectedPath;
                    Settings.Default.Save();
=======
>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
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