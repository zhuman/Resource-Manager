using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Resource_Manager.Classes.Ddt
{
    public enum DdtFileTypeAlpha : byte
    {
        None = 0,
        Player = 1,
        Trans = 4,
        Blend = 8
    }

    public enum DdtFileTypeFormat : byte
    {
        Bgra = 1,
        Dxt1 = 4,
        Dxt1DE = 5,
        Grey = 7,
        Dxt3 = 8,
        Dxt5 = 9
    }

    public enum DdtFileTypeUsage : byte
    {
        AlphaTest = 1,
        LowDetail = 2,
        Bump = 4,
        Cube = 8
    }


    public class Ddt
    {
        public uint Head { get; set; }
        public DdtFileTypeUsage Usage { get; set; }
        public DdtFileTypeAlpha Alpha { get; set; }
        public DdtFileTypeFormat Format { get; set; }
        public byte MipmapLevels { get; set; }
        public ushort BaseWidth { get; set; }
        public ushort BaseHeight { get; set; }
        public BitmapImage Bitmap { get; set; }

        public Image<Rgba32> mipmapMain { get; set; }

        public string DdtInfo
        {
            get
            {
                return BaseWidth.ToString() + "x" + BaseHeight.ToString() + ", " + Format.ToString().ToUpper() + " (" + ((byte)Usage).ToString() + " " + ((byte)Alpha).ToString() + " " + ((byte)Format).ToString() + " " + (MipmapLevels).ToString() + ")";
            }
        }

        public async Task Create(byte[] source)
        {
            using (var stream = new MemoryStream(source))
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    Head = binaryReader.ReadUInt32();
                    Usage = (DdtFileTypeUsage)binaryReader.ReadByte();
                    Alpha = (DdtFileTypeAlpha)binaryReader.ReadByte();
                    Format = (DdtFileTypeFormat)binaryReader.ReadByte();
                    MipmapLevels = binaryReader.ReadByte();
                    BaseWidth = (ushort)binaryReader.ReadInt32();
                    BaseHeight = (ushort)binaryReader.ReadInt32();
                    List<byte> mipmaps = new List<byte>();
                    var numImagesPerLevel = Usage.HasFlag(DdtFileTypeUsage.Cube) ? 6 : 1;
                    for (var index = 0; index < MipmapLevels * numImagesPerLevel; index++)
                    {
                        binaryReader.BaseStream.Position = 16 + 8 * index;
                        var width = BaseWidth >> (index / numImagesPerLevel);
                        if (width < 1)
                            width = 1;
                        var height = BaseHeight >> (index / numImagesPerLevel);
                        if (height < 1)
                            height = 1;
                        var offset = binaryReader.ReadInt32();
                        var length = binaryReader.ReadInt32();
                        binaryReader.BaseStream.Position = offset;
                        mipmaps.AddRange(binaryReader.ReadBytes(length));
                        break;
                    }

                    BcDecoder decoder = new BcDecoder();
                    
                    Image<Rgba32> image;
                    switch (Format)
                    {
                        case DdtFileTypeFormat.Dxt1:
                            image = await decoder.DecodeRawToImageRgba32Async(mipmaps.ToArray(), BaseWidth, BaseHeight, BCnEncoder.Shared.CompressionFormat.Bc1);
                            break;
                        case DdtFileTypeFormat.Dxt1DE:
                            image = await decoder.DecodeRawToImageRgba32Async(mipmaps.ToArray(), BaseWidth, BaseHeight, BCnEncoder.Shared.CompressionFormat.Bc1WithAlpha);
                            break;
                        case DdtFileTypeFormat.Dxt3:
                            image = await decoder.DecodeRawToImageRgba32Async(mipmaps.ToArray(), BaseWidth, BaseHeight, BCnEncoder.Shared.CompressionFormat.Bc2);
                            break;
                        case DdtFileTypeFormat.Dxt5:
                            image = await decoder.DecodeRawToImageRgba32Async(mipmaps.ToArray(), BaseWidth, BaseHeight, BCnEncoder.Shared.CompressionFormat.Bc3);
                            break;
                        default:
                            image = await decoder.DecodeRawToImageRgba32Async(mipmaps.ToArray(), BaseWidth, BaseHeight, BCnEncoder.Shared.CompressionFormat.Bgra);
                            break;
                        case DdtFileTypeFormat.Grey:
                            image = await decoder.DecodeRawToImageRgba32Async(mipmaps.ToArray(), BaseWidth, BaseHeight, BCnEncoder.Shared.CompressionFormat.R);
                            break;
                    }


                    if (Usage.HasFlag(DdtFileTypeUsage.Bump) && Format == DdtFileTypeFormat.Dxt5)
                    {
                        binaryReader.BaseStream.Position = 16;
                        var offset = binaryReader.ReadInt32();
                        var length = binaryReader.ReadInt32();
                        binaryReader.BaseStream.Position = offset;
                        byte[] data = DxtFileUtils.DecompressDxt5Bump(binaryReader.ReadBytes(length), BaseWidth, BaseHeight);
                        image = Image.LoadPixelData<Bgra32>(data, BaseWidth, BaseHeight).CloneAs<Rgba32>();
                    }
                    mipmapMain = image;
                    using MemoryStream memory = new MemoryStream();
                    await image.SaveAsBmpAsync(memory);
                    Bitmap = new BitmapImage();
                    Bitmap.BeginInit();
                    Bitmap.StreamSource = memory;
                    Bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    Bitmap.EndInit();
                }
            }

        }

        public async Task Create(string source)
        {
            byte[] data = await File.ReadAllBytesAsync(source);
            await Create(data);
        }

        public async Task FromPicture(string source)
        {
            string file_name = Path.GetFileName(source);
            var splitted_name = file_name.Split('.');
            if (splitted_name.Length != 3)
            {
                throw new Exception("Missing DDT details in filename");
            }
            var splitted_params = splitted_name[1].Split(new char[] { ',', '(', ')' });
            if (splitted_params.Length != 6)
            {
                throw new Exception("Missing params in DDT details");
            }
            Head = 0x33535452;
            Usage = (DdtFileTypeUsage)Convert.ToByte(splitted_params[1]);
            Alpha = (DdtFileTypeAlpha)Convert.ToByte(splitted_params[2]);
            Format = (DdtFileTypeFormat)Convert.ToByte(splitted_params[3]);
            MipmapLevels = Convert.ToByte(splitted_params[4]);


            using Image<Rgba32> image = await Image.LoadAsync<Rgba32>(source);

            BaseWidth = (ushort)image.Width;
            BaseHeight = (ushort)image.Height;

            BcEncoder encoder = new BcEncoder();

            if (Usage.HasFlag(DdtFileTypeUsage.Bump) && Format == DdtFileTypeFormat.Dxt5)
            {
                throw new Exception("App doesn't support DXT5 bump conversion");
            }

            encoder.OutputOptions.GenerateMipMaps = true;
            encoder.OutputOptions.Quality = CompressionQuality.Balanced;
            switch (Format)
            {
                case DdtFileTypeFormat.Dxt1:
                    encoder.OutputOptions.Format = CompressionFormat.Bc1;
                    break;
                case DdtFileTypeFormat.Dxt1DE:
                    encoder.OutputOptions.Format = CompressionFormat.Bc1WithAlpha;
                    break;
                case DdtFileTypeFormat.Dxt3:
                    encoder.OutputOptions.Format = CompressionFormat.Bc2;
                    break;
                case DdtFileTypeFormat.Dxt5:
                    encoder.OutputOptions.Format = CompressionFormat.Bc3;
                    break;
                case DdtFileTypeFormat.Grey:
                case DdtFileTypeFormat.Bgra:
                    encoder.OutputOptions.Format = CompressionFormat.Bgra;
                    break;
                default:
                    {
                        throw new Exception("Not valid DDT format.");
                    }
            }


            encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;

            var mipmaps = await encoder.EncodeToRawBytesAsync(image);

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(Head);
                    bw.Write((byte)Usage);
                    bw.Write((byte)Alpha);
                    bw.Write((byte)Format);
                    bw.Write(MipmapLevels);
                    bw.Write((int)BaseWidth);
                    bw.Write((int)BaseHeight);
                    var numImagesPerLevel = Usage.HasFlag(DdtFileTypeUsage.Cube) ? 6 : 1;
                    for (var index = 0; index < MipmapLevels * numImagesPerLevel; ++index)
                    {
                        if (index == 0)
                            bw.Write(16 + 8 * MipmapLevels * numImagesPerLevel);
                        else
                        {
                            bw.Write(16 + 8 * MipmapLevels * numImagesPerLevel + mipmaps.ToList().GetRange(0, index).Sum(x => x.Length));
                        }
                        bw.Write(mipmaps[index].Length);

                    }
                    for (var index = 0; index < MipmapLevels * numImagesPerLevel; ++index)
                    {
                        bw.Write(mipmaps[index]);
                    }
                    await File.WriteAllBytesAsync(Path.Combine(Path.GetDirectoryName(source), Path.GetFileName(source).Split('.')[0] + ".ddt"), ms.ToArray());
                }
            }
        }

        public async Task SaveAsTGA(string dest)
        {
            TgaEncoder tga = new TgaEncoder();
            tga.BitsPerPixel = TgaBitsPerPixel.Pixel32;
            var file_name = Path.GetFileNameWithoutExtension(dest);
            file_name = file_name + ".(" + ((byte)Usage).ToString() + "," + ((byte)Alpha).ToString() + "," + ((byte)Format).ToString() + "," + ((byte)MipmapLevels).ToString() + ").tga";
            await mipmapMain.SaveAsTgaAsync(Path.Combine(Path.GetDirectoryName(dest), file_name), tga);
        }

        public async Task SaveAsPNG(string dest)
        {
            PngEncoder png = new PngEncoder();
            png.ColorType = PngColorType.RgbWithAlpha;
            var file_name = Path.GetFileNameWithoutExtension(dest);
            file_name = file_name + ".(" + ((byte)Usage).ToString() + "," + ((byte)Alpha).ToString() + "," + ((byte)Format).ToString() + "," + ((byte)MipmapLevels).ToString() + ").png";
            await mipmapMain.SaveAsPngAsync(Path.Combine(Path.GetDirectoryName(dest), file_name), png);
        }
    }
}
