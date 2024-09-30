﻿using NAudio.Wave;
using Newtonsoft.Json;
using Resource_Manager.Classes.Alz4;
using Resource_Manager.Classes.Bar;
using Resource_Manager.Classes.Ddt;
using Resource_Manager.Classes.L33TZip;
using Resource_Manager.Classes.sound;
using Resource_Manager.Classes.Xmb;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using WebPWrapper;
using Color = System.Drawing.Color;
using Image = SixLabors.ImageSharp.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace Archive_Unpacker.Classes.BarViewModel
{
    public class BarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private CollectionViewSource entriesCollection;
        private string filterText;

        public ICollectionView SourceCollection
        {
            get
            {
                return this.entriesCollection.View;
            }
        }

        private bool isLatestChangesVisible { get; set; } = false;

        public bool IsLatestChangesVisible
        {
            get { return isLatestChangesVisible; }
            set
            {
                isLatestChangesVisible = value;
                NotifyPropertyChanged();
                entriesCollection.View.Refresh();
            }
        }
        public string FilterText
        {
            get
            {
                return filterText;
            }
            set
            {
                filterText = value;
                NotifyPropertyChanged();
                entriesCollection.View.Refresh();
            }
        }

        private double currentProgress;
        public double CurrentProgress
        {
            get
            {
                return currentProgress;
            }
            set
            {
                currentProgress = value;
                NotifyPropertyChanged();
            }
        }


        public int extractingState { get; set; }

        private void ResetProgress()
        {
            CurrentProgress = 0;
        }

        public class AudioSource
        {
            public WaveStream Audio { get; init; }
            public MemoryStream RawAudioStream { get; init; }

            public string FormatText => Audio?.WaveFormat.ToString() ?? "Unknown";
        }

        private MemoryStream rawAudioStream;
        private Func<Stream, WaveStream> rawAudioToWaveStream;
        private AudioSource previewAudio;

        public AudioSource PreviewAudio => previewAudio ?? (rawAudioStream == null ? null : (previewAudio = new AudioSource { RawAudioStream = rawAudioStream, Audio = rawAudioToWaveStream(rawAudioStream) }));

        public string barFilePath { get; set; }
        public BarFile barFile { get; set; }

        private BitmapImage previewImage;
        public BitmapImage PreviewImage
        {
            get { return previewImage; }
            set
            {
                previewImage = value;
                NotifyPropertyChanged();
            }
        }

        private Ddt previewDdt;
        public Ddt PreviewDdt
        {
            get { return previewDdt; }
            set
            {
                previewDdt = value;
                NotifyPropertyChanged();
            }
        }

        public class Document
        {
            public string Text { get; set; }
            public string SyntaxHighlighting { get; set; }
        }

        private Document previewText;
        public Document PreviewText
        {
            get { return previewText; }
            set
            {
                previewText = value;
                NotifyPropertyChanged();
            }
        }

        public string barFileName
        {
            get
            {
                if (barFile.barFileHeader.Version == 2)
                    return Path.GetFileName(barFilePath) + " - AoE3: Legacy";
                else
                if (barFile.barFileHeader.Version == 4)
                    return Path.GetFileName(barFilePath) + " - AoE3: Definitive Edition - 1st wave of beta testing";
                else
                    return Path.GetFileName(barFilePath) + " - AoE3: Definitive Edition";
            }
        }

        void Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText) && IsLatestChangesVisible == false)
            {
                e.Accepted = true;
                return;
            }
            if (!string.IsNullOrEmpty(FilterText))
            {
                var entry = e.Item as BarEntry;
                if (IsLatestChangesVisible)
                    e.Accepted = entry.FileNameWithRoot.ToLower().Contains(FilterText.ToLower()) && entry.IsLatestChange;
                else
                    e.Accepted = entry.FileNameWithRoot.ToLower().Contains(FilterText.ToLower());
            }
            else
            {
                var entry = e.Item as BarEntry;
                if (IsLatestChangesVisible)
                    e.Accepted = entry.IsLatestChange;
                else
                    e.Accepted = true;
            }
        }




        public async Task saveFiles(List<BarEntry> files, string savePath, bool Decompress, CancellationToken token, bool convertDDTToPNG, bool convertDDTToTGA, bool convertXMB, bool OneFolder, bool SavePNGasBMP, bool AutoJSONConversion, Color OverlayColor, bool CompressPNG, bool ConvertWEBP)
        {
            ResetProgress();
            if (files.Count == 0) return;

            using var input = File.OpenRead(barFilePath);

            long filesSize = files.Sum(x => (long)x.FileSize2);

            foreach (var file in files)
            {
                if (token.IsCancellationRequested)
                {
                    while (extractingState == 1)
                        await Task.Delay(1000);
                }
                if (token.IsCancellationRequested && extractingState == 2)
                {
                    ResetProgress();
                    return;
                }
                // Locate the file within the BAR file.
                input.Seek(file.Offset, SeekOrigin.Begin);


                string ExtractPath = Path.Combine(savePath, file.FileNameWithRoot);
                if (OneFolder)
                {
                    ExtractPath = Path.Combine(savePath, file.fileNameWithoutPath);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(ExtractPath));


                var data = new byte[file.FileSize2];
                await input.ReadAsync(data, 0, data.Length);

                // XMB and decompress 
                if (file.Extension != ".XMB" && (L33TZipUtils.IsL33TZipFile(data) || Alz4Utils.IsAlz4File(data)) && Decompress)
                {
                    if (Alz4Utils.IsAlz4File(data))
                    {
                        data = await Alz4Utils.ExtractAlz4BytesAsync(data);
                    }
                    else
                    {
                        if (L33TZipUtils.IsL33TZipFile(data))
                            data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
                    }
                }

                // WAV or MP3
                if (file.Extension == ".WAV" || file.Extension == ".MP3")
                {
                    if (file.isCompressed == 2)
                    {
                        data = await soundUtils.DecryptSound(data);
                    }
                }

                // Save PNG
                if (file.Extension == ".PNG")
                {

                    using var memory = new MemoryStream(data);

                    var reader = new BinaryReader(memory);
                    var png_header = reader.ReadInt32();
                    memory.Position = 0;
                    if (png_header == 0x474E5089)
                    {
                        using Image image = await Image.LoadAsync(memory);
                        PngEncoder encoder = new()
                        {
                            ColorType = PngColorType.RgbWithAlpha
                        };

                        using var stream = new MemoryStream();
                        await image.SaveAsync(stream, encoder);
                        Bitmap img = new Bitmap(stream);





                        // Color Overlay
                        if (SavePNGasBMP && img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                        {
                            PixelFormat fmt1 = img.PixelFormat;

                            byte bpp1 = 4;

                            Rectangle rect = new Rectangle(Point.Empty, new Size(img.Width, img.Height));
                            BitmapData bmpData = img.LockBits(rect, ImageLockMode.ReadWrite, fmt1);

                            int size1 = bmpData.Stride * bmpData.Height;
                            byte[] pixels = new byte[size1];
                            Marshal.Copy(bmpData.Scan0, pixels, 0, size1);

                            for (int y = 0; y < img.Height; y++)
                            {
                                for (int x = 0; x < img.Width; x++)
                                {
                                    int index = y * bmpData.Stride + x * bpp1;
                                    var alpha = pixels[index + 3];
                                    if (alpha < 255)
                                    {

                                        pixels[index] = (byte)(pixels[index] * OverlayColor.B / 255);  //b
                                        pixels[index + 1] = (byte)(pixels[index + 1] * OverlayColor.G / 255); //g
                                        pixels[index + 2] = (byte)(pixels[index + 2] * OverlayColor.R / 255); //r
                                        pixels[index + 3] = 255;
                                    }

                                }
                            }

                            Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
                            img.UnlockBits(bmpData);
                            using (Graphics g = Graphics.FromImage(img))
                            {
                                g.DrawImage(new Bitmap(stream), Point.Empty);

                            }
                        }
                        memory.Position = 0;
                        // Convert to WEBP
                        if (ConvertWEBP)
                        {

                            using (WebP webp = new WebP())
                            {
                                byte[] webpData = webp.EncodeLossy(img, 75);
                                await File.WriteAllBytesAsync(Path.ChangeExtension(ExtractPath, "webp"), webpData);
                            }

                        }

                        // Compressing
                        if (CompressPNG)
                        {

                            ImageConverter converter = new ImageConverter();
                            byte[] uncompressed = (byte[])converter.ConvertTo(img, typeof(byte[]));
                            ProcessStartInfo info = new ProcessStartInfo()
                            {
                                CreateNoWindow = true,
                                FileName = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "pngquant.exe"),
                                Arguments = "--quality=45-85 -",
                                RedirectStandardInput = true,
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                            };
                            Process pro = Process.Start(info);
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                await pro.StandardInput.BaseStream.WriteAsync(uncompressed, 0, uncompressed.Length);
                                await pro.StandardInput.BaseStream.FlushAsync();
                                await pro.StandardOutput.BaseStream.CopyToAsync(outputStream);
                                byte[] output = outputStream.ToArray();
                                await File.WriteAllBytesAsync(ExtractPath, output);

                            }
                            // var quantizer = new PnnQuant.PnnQuantizer();
                            // using (var dest = quantizer.QuantizeImage(img, System.Drawing.Imaging.PixelFormat.Undefined, 256, true))
                            //{
                            //      dest.Save(ExtractPath, System.Drawing.Imaging.ImageFormat.Png);
                            //  }
                        }
                        else
                        {
                            img.Save(ExtractPath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                    else if (png_header == 0x33535452)
                    {
                        var ddt = new Ddt();
                        await ddt.Create(data);
                        await ddt.SaveAsPNG(ExtractPath);
                    }
                    else
                    {
                        await File.WriteAllBytesAsync(ExtractPath, data);
                    }

                    CurrentProgress += (double)file.FileSize2 / filesSize;
                    continue;

                }


                // Save data
                await File.WriteAllBytesAsync(ExtractPath, data);


                // Additionaly convert xmb
                if (file.Extension == ".XMB" && convertXMB)
                {
                    if (Alz4Utils.IsAlz4File(data))
                    {
                        data = await Alz4Utils.ExtractAlz4BytesAsync(data);
                    }
                    else
                    {
                        if (L33TZipUtils.IsL33TZipFile(data))
                            data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
                    }

                    using MemoryStream stream = new MemoryStream(data);
                    XMBFile xmb = await XMBFile.LoadXMBFile(stream);

                    xmb.file.Save(Path.ChangeExtension(ExtractPath, ""));
                }

                // Additionaly convert xmb -> json
                if (file.Extension == ".XMB" && AutoJSONConversion)
                {
                    if (Alz4Utils.IsAlz4File(data))
                    {
                        data = await Alz4Utils.ExtractAlz4BytesAsync(data);
                    }
                    else
                    {
                        if (L33TZipUtils.IsL33TZipFile(data))
                            data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
                    }

                    using MemoryStream stream = new (data);
                    XMBFile xmb = await XMBFile.LoadXMBFile(stream);
                    string json = JsonConvert.SerializeXmlNode(xmb.file, Newtonsoft.Json.Formatting.None, true);
                    await File.WriteAllTextAsync(
                        Path.ChangeExtension(
                            Path.ChangeExtension(ExtractPath, null),
                            "json"
                        ),
                        json
                    );
                }

                // Additionaly convert xml -> json
                if (file.Extension == ".XML" && AutoJSONConversion)
                {
                    if (Alz4Utils.IsAlz4File(data))
                    {
                        data = await Alz4Utils.ExtractAlz4BytesAsync(data);
                    }
                    else
                    {
                        if (L33TZipUtils.IsL33TZipFile(data))
                            data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
                    }

                    using MemoryStream stream = new (data);
                    XmlDocument xml = new ();
                    xml.Load(stream);
                    string json = JsonConvert.SerializeXmlNode(xml, Newtonsoft.Json.Formatting.None, true);
                    await File.WriteAllTextAsync(
                        Path.ChangeExtension(
                            Path.ChangeExtension(ExtractPath, null), 
                            "json"
                        ), 
                        json
                    );
                }




                // Additionaly convert ddt to png
                if (file.Extension == ".DDT" && convertDDTToPNG)
                {
                    var ddt = new Ddt();
                    await ddt.Create(data);
                    await ddt.SaveAsPNG(ExtractPath);
                }
                // Additionaly convert ddt to tga
                if (file.Extension == ".DDT" && convertDDTToTGA)
                {
                    var ddt = new Ddt();
                    await ddt.Create(data);
                    await ddt.SaveAsTGA(ExtractPath);
                }



                CurrentProgress += (double)file.FileSize2 / filesSize;
            }
            ResetProgress();



        }

        public async Task readFile(BarEntry file)
        {

            // Firstly, is the file parameter null?

            PreviewDdt = null;
            PreviewText = null;
            PreviewImage = null;
            if (file == null)
                return;

            if (file.Extension == ".WAV" || file.Extension == ".MP3")
            {
                using FileStream input = File.OpenRead(barFilePath);
                // Locate the file within the BAR file.
                input.Seek(file.Offset, SeekOrigin.Begin);
                var data = new byte[file.FileSize2];
                await input.ReadAsync(data, 0, data.Length);

                previewAudio = null;

                if (file.isCompressed == 2)
                {
                    rawAudioStream = new MemoryStream(await soundUtils.DecryptSound(data));
                }
                else
                    rawAudioStream = new MemoryStream(data);

                if (file.Extension == ".WAV")
                    rawAudioToWaveStream = (stream) => new WaveFileReader(stream);
                else if (file.Extension == ".MP3")
                    rawAudioToWaveStream = (stream) => new Mp3FileReader(stream);

                return;
            }
            else if (file.Extension == ".DDT")
            {
                using FileStream input = File.OpenRead(barFilePath);
                // Locate the file within the BAR file.
                input.Seek(file.Offset, SeekOrigin.Begin);
                var data = new byte[file.FileSize2];
                await input.ReadAsync(data, 0, data.Length);


                if (Alz4Utils.IsAlz4File(data))
                {
                    data = await Alz4Utils.ExtractAlz4BytesAsync(data);
                }
                else
                {
                    if (L33TZipUtils.IsL33TZipFile(data))
                        data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
                }



                var ddt = new Ddt();
                await ddt.Create(data);
                PreviewDdt = ddt;
                return;
            }
            else if (file.Extension == ".BMP" || file.Extension == ".TGA" || file.Extension == ".PNG" || file.Extension == ".CUR" || file.Extension == ".JPG")
            {
                using FileStream input = File.OpenRead(barFilePath);
                // Locate the file within the BAR file.
                input.Seek(file.Offset, SeekOrigin.Begin);
                var data = new byte[file.FileSize2];
                await input.ReadAsync(data, 0, data.Length);
                if (Alz4Utils.IsAlz4File(data))
                {
                    data = await Alz4Utils.ExtractAlz4BytesAsync(data);
                }
                else
                {
                    if (L33TZipUtils.IsL33TZipFile(data))
                        data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
                }

                var bitmap = new BitmapImage();

                using (var stream = new MemoryStream(data))
                {
                    var reader = new BinaryReader(stream);
                    var png_header = reader.ReadInt32();
                    stream.Position = 0;

                    if (png_header == 0x33535452)
                    {
                        var ddt = new Ddt();
                        await ddt.Create(data);
                        bitmap = ddt.Bitmap;
                    }
                    else
                    {


                        if (file.Extension == ".TGA")
                        {

                            var image = await SixLabors.ImageSharp.Image.LoadAsync(stream);

                            using MemoryStream memory = new MemoryStream();
                            await image.SaveAsBmpAsync(memory);
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = memory;
                            bitmap.EndInit();
                        }
                        else
                        {
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            bitmap.Freeze();
                        }
                    }
                }


                PreviewImage = bitmap;
                return;
            }

            if (file.Extension == ".XMB")
            {
                using FileStream input = File.OpenRead(barFilePath);
                // Locate the file within the BAR file.
                input.Seek(file.Offset, SeekOrigin.Begin);
                var data = new byte[file.FileSize2];
                await input.ReadAsync(data, 0, data.Length);

                if (Alz4Utils.IsAlz4File(data))
                {
                    data = await Alz4Utils.ExtractAlz4BytesAsync(data);
                }
                else
                {
                    if (L33TZipUtils.IsL33TZipFile(data))
                        data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
                }

                //File.WriteAllBytes(file.fileNameWithoutPath, data);

                PreviewText = new Document()
                {
                    SyntaxHighlighting = "XML",
                    Text = await XMBFile.XmbToXmlAsync(data)
                };

                NotifyPropertyChanged("Preview");
                return;
            }
            else if (file.Extension == ".XAML" || file.Extension == ".XML" || file.Extension == ".SHP" || file.Extension == ".LGT" || file.Extension == ".XS" || file.Extension == ".TXT" || file.Extension == ".CFG" || file.Extension == ".PY" || file.Extension == ".TACTICS")
            {
                using FileStream input = File.OpenRead(barFilePath);
                // Locate the file within the BAR file.
                input.Seek(file.Offset, SeekOrigin.Begin);
                var data = new byte[file.FileSize2];
                await input.ReadAsync(data, 0, data.Length);
                if (Alz4Utils.IsAlz4File(data))
                {
                    data = await Alz4Utils.ExtractAlz4BytesAsync(data);
                }
                else
                {
                    if (L33TZipUtils.IsL33TZipFile(data))
                        data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
                }
                PreviewText = new Document()
                {
                    Text = System.Text.Encoding.UTF8.GetString(data),
                    SyntaxHighlighting = file.Extension switch
                    {
                        ".XS" => "C++",
                        _ => "XML"
                    }
                };
                NotifyPropertyChanged("Preview");
                return;
            }

            return;
        }


        public async static Task<BarViewModel> Load(string filename)
        {
            BarViewModel barViewModel = new BarViewModel();
            barViewModel.extractingState = 0;
            barViewModel.barFilePath = filename;
            barViewModel.barFile = await BarFile.Load(filename);

            barViewModel.ResetProgress();

            barViewModel.entriesCollection = new CollectionViewSource();
            barViewModel.entriesCollection.Source = barViewModel.barFile.BarFileEntrys;
            barViewModel.entriesCollection.Filter += barViewModel.Filter;
            return barViewModel;
        }

        public static async Task<BarViewModel> Create(string rootFolder, uint version, string filename)
        {
            BarViewModel barViewModel = new BarViewModel();
            barViewModel.extractingState = 0;
            barViewModel.barFilePath = Path.Combine(Directory.GetParent(rootFolder).FullName, filename + ".bar");
            barViewModel.barFile = await BarFile.Create(rootFolder, version, filename);
            barViewModel.entriesCollection = new CollectionViewSource();
            barViewModel.ResetProgress();

            barViewModel.entriesCollection.Source = barViewModel.barFile.BarFileEntrys;
            barViewModel.entriesCollection.Filter += barViewModel.Filter;

            return barViewModel;
        }




    }
}
