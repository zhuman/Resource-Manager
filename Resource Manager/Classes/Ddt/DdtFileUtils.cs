using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using Microsoft.Toolkit.HighPerformance;
using Resource_Manager.Classes.Alz4;
using Resource_Manager.Classes.L33TZip;
using Resource_Manager.Classes.TGA;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


using BCnEncoder.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Resource_Manager.Classes.Ddt
{
    public static class DdtFileUtilsDepricated
    {

        public static async Task Ddt2TgaAsync(string ddtFile)
        {

            var data = await File.ReadAllBytesAsync(ddtFile);

            if (Alz4Utils.IsAlz4File(data))
            {
                data = await Alz4Utils.ExtractAlz4BytesAsync(data);
            }
            else
            {
                if (L33TZipUtils.IsL33TZipFile(data))
                    data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
            }
            DdtFileDepricated ddt = new DdtFileDepricated(data, false);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            byte[] raw_image = new byte[4 * ddt.BaseWidth * ddt.BaseHeight];
            ddt.Bitmap.CopyPixels(raw_image, 4 * ddt.BaseWidth, 0);
            var tga = new TGAImage(ddt.BaseWidth, ddt.BaseHeight, (byte)ddt.Usage, (byte)ddt.Alpha, (byte)ddt.Format, ddt.MipmapLevels, raw_image);

            var file_name = Path.GetFileNameWithoutExtension(ddtFile);
            file_name = file_name + ".(" + tga.image_id[0].ToString() + "," + tga.image_id[1].ToString() + "," + tga.image_id[2].ToString() + "," + tga.image_id[3].ToString() + ").tga";

            await File.WriteAllBytesAsync(Path.Combine(Path.GetDirectoryName(ddtFile), file_name), tga.ToByteArray());

        }

        public static async Task DdtBytes2TgaAsync(byte[] source, string path)
        {
            var data = source;

            if (Alz4Utils.IsAlz4File(data))
            {
                data = await Alz4Utils.ExtractAlz4BytesAsync(data);
            }
            else
            {
                if (L33TZipUtils.IsL33TZipFile(data))
                    data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
            }
            DdtFileDepricated ddt = new DdtFileDepricated(data, false);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            byte[] raw_image = new byte[4 * ddt.BaseWidth * ddt.BaseHeight];
            ddt.Bitmap.CopyPixels(raw_image, 4 * ddt.BaseWidth, 0);
            var tga = new TGAImage(ddt.BaseWidth, ddt.BaseHeight, (byte)ddt.Usage, (byte)ddt.Alpha, (byte)ddt.Format, ddt.MipmapLevels, raw_image);

            var file_name = Path.GetFileNameWithoutExtension(path);
            file_name = file_name + ".(" + tga.image_id[0].ToString() + "," + tga.image_id[1].ToString() + "," + tga.image_id[2].ToString() + "," + tga.image_id[3].ToString() + ").tga";

            await File.WriteAllBytesAsync(Path.Combine(Path.GetDirectoryName(path), file_name), tga.ToByteArray());
        }

        public static async Task Tga2DdtAsync(string tgaFile)
        {
            var tga = new TGAImage(tgaFile);

            var usage = (DdtFileTypeUsage)tga.image_id[0];
            BcEncoder encoder = new BcEncoder();

           // byte[] bytes = tga.raw_data;
            byte[] bytes = new byte[tga.image_width * tga.image_height * 4];
            for (int i = 0; i < tga.image_width * tga.image_height; i++)
            {
                var b = tga.raw_data[i * 4];
                var g = tga.raw_data[i * 4 + 1];
                byte r;
                byte a;
                if (usage.HasFlag(DdtFileTypeUsage.Bump) && tga.image_id[2] == 9)
                {
                    r = tga.raw_data[i * 4 + 3];
                    a = tga.raw_data[i * 4 + 2];
                }
                else
                {
                    r = tga.raw_data[i * 4 + 2];
                    a = tga.raw_data[i * 4 + 3];
                }
                bytes[i * 4] = b;
                bytes[i * 4 + 1] = g;
                bytes[i * 4 + 2] = r;
                bytes[i * 4 + 3] = a;

            }

            encoder.OutputOptions.GenerateMipMaps = true;
            encoder.OutputOptions.Quality = CompressionQuality.Balanced;
            switch (tga.image_id[2])
            {
                case 4:
                    encoder.OutputOptions.Format = CompressionFormat.Bc1;
                    break;
                case 5:
                    encoder.OutputOptions.Format = CompressionFormat.Bc1WithAlpha;
                    break;
                case 8:
                    encoder.OutputOptions.Format = CompressionFormat.Bc2;
                    break;
                case 9:
                    encoder.OutputOptions.Format = CompressionFormat.Bc3;
                    break;
                case 1:
                case 7:
                    encoder.OutputOptions.Format = CompressionFormat.Bgra;
                    break;
                default:
                    {
                        throw new Exception("Not valid DDT format.");
                    }
            }

            
            encoder.OutputOptions.FileFormat = OutputFileFormat.Dds; //Change to Dds for a dds file.
     
            var data=await encoder.EncodeToRawBytesAsync(bytes, tga.image_width, tga.image_height, BCnEncoder.Encoder.PixelFormat.Bgra32);
            using FileStream fs = File.OpenWrite(Path.Combine(Path.GetDirectoryName(tgaFile), Path.GetFileName(tgaFile).Split('.')[0] + ".dds"));
            using MemoryStream f = new MemoryStream();

            encoder.EncodeToStream(bytes, tga.image_width, tga.image_height, BCnEncoder.Encoder.PixelFormat.Bgra32, f);

            var ddt = new DdtFileDepricated(tga.image_id[0], tga.image_id[1], tga.image_id[2], tga.image_id[3], tga.image_width, tga.image_height, data);
            await File.WriteAllBytesAsync(Path.Combine(Path.GetDirectoryName(tgaFile), Path.GetFileName(tgaFile).Split('.')[0] + ".ddt"), ddt.ToByteArray());
            f.Seek(0, SeekOrigin.Begin);
           // using FileStream f = File.OpenRead(Path.Combine(Path.GetDirectoryName(tgaFile), Path.GetFileName(tgaFile).Split('.')[0] + ".dds"));
            BcDecoder decoder = new BcDecoder();
            using Image<Rgba32> image = decoder.DecodeToImageRgba32(f);

            using FileStream outFs = File.OpenWrite(Path.Combine(Path.GetDirectoryName(tgaFile), "decoding_test_bc1.png"));
            image.SaveAsPng(outFs);
            /*
            byte[] bytes = new byte[tga.image_width * tga.image_height * 4];
            for (int i = 0; i < tga.image_width * tga.image_height; i++)
            {
                var b = tga.raw_data[i * 4];
                var g = tga.raw_data[i * 4 + 1];
                byte r;
                byte a;
                if (usage.HasFlag(DdtFileTypeUsage.Bump) && tga.image_id[2] == 9)
                {
                    r = tga.raw_data[i * 4 + 3];
                    a = tga.raw_data[i * 4 + 2];
                }
                else
                {
                    r = tga.raw_data[i * 4 + 2];
                    a = tga.raw_data[i * 4 + 3];
                }
                bytes[i * 4] = b;
                bytes[i * 4 + 1] = g;
                bytes[i * 4 + 2] = r;
                bytes[i * 4 + 3] = a;

            } 
                switch (tga.image_id[2])
            {
                case 4:
                case 5:
                    {
                        data = DxtEncoding.Dxt1Encode(bytes, tga.image_width, tga.image_height);
                        break;
                    }
                case 8:
                    {
                        data = DxtEncoding.Dxt3Encode(bytes, tga.image_width, tga.image_height);
                        break;
                    }
                case 9:
                    {


                            data = DxtEncoding.Dxt5Encode(bytes, tga.image_width, tga.image_height);

                        
                        break;
                    }
                case 1:
                case 7:
                    {
                        data = tga.raw_data;
                        break;
                    }
                default:
                    {
                        throw new Exception("Not valid DDT format.");
                    }
            }


            var ddt = new DdtFile(tga.image_id[0], tga.image_id[1], tga.image_id[2], tga.image_id[3], tga.image_width, tga.image_height, data);

            await File.WriteAllBytesAsync(Path.Combine(Path.GetDirectoryName(tgaFile), Path.GetFileName(tgaFile).Split('.')[0]+".ddt"), ddt.ToByteArray());
       */
        }
    


    public static async Task Ddt2PngAsync(string ddtFile)
        {
            var data = await File.ReadAllBytesAsync(ddtFile);

            if (Alz4Utils.IsAlz4File(data))
            {
                data = await Alz4Utils.ExtractAlz4BytesAsync(data);
            }
            else
            {
                if (L33TZipUtils.IsL33TZipFile(data))
                    data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
            }
            var ddt = new DdtFileDepricated(data, false);
            var file_name = Path.GetFileNameWithoutExtension(ddtFile);
            file_name = file_name + ".(" + ((byte)ddt.Usage).ToString() + "," + ((byte)ddt.Alpha).ToString() + "," + ((byte)ddt.Format).ToString() + "," + ((byte)ddt.MipmapLevels).ToString() + ").png";
            using (var fileStream = new FileStream(Path.Combine(Path.GetDirectoryName(ddtFile), file_name), FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(ddt.Bitmap));
                encoder.Save(fileStream);
            }
        }

        public static async Task DdtBytes2PngAsync(byte[] ddtfile, string path)
        {
            
            var data = ddtfile;
            var ddt = new DdtFileDepricated(data, false);
            var file_name = Path.GetFileNameWithoutExtension(path);
            file_name = file_name + ".(" + ((byte)ddt.Usage).ToString() + "," + ((byte)ddt.Alpha).ToString() + "," + ((byte)ddt.Format).ToString() + "," + ((byte)ddt.MipmapLevels).ToString() + ").png";

            if (Alz4Utils.IsAlz4File(data))
            {
                data = await Alz4Utils.ExtractAlz4BytesAsync(data);
            }
            else
            {
                if (L33TZipUtils.IsL33TZipFile(data))
                    data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
            }


            using (var fileStream = new FileStream(Path.Combine(Path.GetDirectoryName(path), file_name), FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(ddt.Bitmap));
                encoder.Save(fileStream);
            }
        }
    }
}
