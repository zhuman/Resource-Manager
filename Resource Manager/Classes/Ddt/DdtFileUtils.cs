using Resource_Manager.Classes.Alz4;
using Resource_Manager.Classes.L33TZip;
<<<<<<< HEAD
using Resource_Manager.Classes.TGA;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
=======
using System.IO;
>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Resource_Manager.Classes.Ddt
{
    public static class DdtFileUtils
    {
<<<<<<< HEAD

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
            DdtFile ddt = new DdtFile(data, false);
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
            DdtFile ddt = new DdtFile(data, false);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            byte[] raw_image = new byte[4 * ddt.BaseWidth * ddt.BaseHeight];
            ddt.Bitmap.CopyPixels(raw_image, 4 * ddt.BaseWidth, 0);
            var tga = new TGAImage(ddt.BaseWidth, ddt.BaseHeight, (byte)ddt.Usage, (byte)ddt.Alpha, (byte)ddt.Format, ddt.MipmapLevels, raw_image);

            var file_name = Path.GetFileNameWithoutExtension(path);
            file_name = file_name + ".(" + tga.image_id[0].ToString() + "," + tga.image_id[1].ToString() + "," + tga.image_id[2].ToString() + "," + tga.image_id[3].ToString() + ").tga";

            await File.WriteAllBytesAsync(Path.Combine(Path.GetDirectoryName(path), file_name), tga.ToByteArray());
        }



=======
>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
        public static async Task Ddt2PngAsync(string ddtFile)
        {
            var outname = ddtFile.ToLower().Replace(".ddt", ".png");

            using (var fileStream = new FileStream(outname, FileMode.Create))
            {
<<<<<<< HEAD
                // DdtFile ddt = new DdtFile(File.ReadAllBytes(ddtFile));
=======
               // DdtFile ddt = new DdtFile(File.ReadAllBytes(ddtFile));
>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
                BitmapEncoder encoder = new PngBitmapEncoder();
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


                //await File.WriteAllBytesAsync("test", data);

                encoder.Frames.Add(BitmapFrame.Create(new DdtFile(data, false).Bitmap));
                encoder.Save(fileStream);
            }
        }

        public static async Task DdtBytes2PngAsync(byte[] ddt, string path)
        {
            var outname = path.ToLower().Replace(".ddt", ".png");

            using (var fileStream = new FileStream(outname, FileMode.Create))
            {
                // DdtFile ddt = new DdtFile(File.ReadAllBytes(ddtFile));
                BitmapEncoder encoder = new PngBitmapEncoder();
                var data = ddt;

                if (Alz4Utils.IsAlz4File(data))
                {
                    data = await Alz4Utils.ExtractAlz4BytesAsync(data);
                }
                else
                {
                    if (L33TZipUtils.IsL33TZipFile(data))
                        data = await L33TZipUtils.ExtractL33TZippedBytesAsync(data);
                }


                //await File.WriteAllBytesAsync("test", data);

                encoder.Frames.Add(BitmapFrame.Create(new DdtFile(data, false).Bitmap));
                encoder.Save(fileStream);
            }
        }
    }
}
