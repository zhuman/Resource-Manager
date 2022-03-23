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
        private bool oneFolder = false;
        private bool savePNGasBMP = false;
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

        public bool SavePNGasBMP
        {
            get
            {
                return savePNGasBMP;
            }
            set
            {
                savePNGasBMP = value;
                NotifyPropertyChanged();
            }
        }

        public bool OneFolder
        {
            get
            {
                return oneFolder;
            }
            set
            {
                oneFolder = value;
                NotifyPropertyChanged();
            }
        }

        public ExtractDialog(string DefaultRootPath)
        {         
            InitializeComponent();
            ExportPath.Text = Directory.Exists(Settings.Default.lastExportedPath) ? Settings.Default.lastExportedPath : DefaultRootPath;

            AutoDecompress = Settings.Default.ExtractionAutoDecompress ? Settings.Default.ExtractionAutoDecompress : true;
            OneFolder = Settings.Default.ExtractionOneFolder ? Settings.Default.ExtractionOneFolder : false;
            SavePNGasBMP = Settings.Default.ExtractionSavePNGasBMP ? Settings.Default.ExtractionSavePNGasBMP : false;
            AutoXMBConversion = Settings.Default.ExtractionAutoXMBConversion ? Settings.Default.ExtractionAutoXMBConversion : false;
            AutoDDTToPNGConversion = Settings.Default.ExtractionAutoDDTToPNGConversion ? Settings.Default.ExtractionAutoDDTToPNGConversion : false;
            AutoDDTToTGAConversion = Settings.Default.ExtractionAutoDDTToTGAConversion ? Settings.Default.ExtractionAutoDDTToTGAConversion : true;

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

                Settings.Default.ExtractionAutoDecompress = AutoDecompress;
                Settings.Default.ExtractionOneFolder = OneFolder;
                Settings.Default.ExtractionSavePNGasBMP = SavePNGasBMP;
                Settings.Default.ExtractionAutoXMBConversion = AutoXMBConversion;
                Settings.Default.ExtractionAutoDDTToPNGConversion = AutoDDTToPNGConversion;
                Settings.Default.ExtractionAutoDDTToTGAConversion = AutoDDTToTGAConversion;
                Settings.Default.Save();
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