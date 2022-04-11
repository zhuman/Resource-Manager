using Resource_Manager.Classes.Ddt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Resource_Manager.Classes.TGA
{

    public class TGAImage
    {
        byte id_length { get; set; }
        byte map_type { get; set; }
        byte image_type { get; set; }
        ushort map_origin { get; set; }
        ushort map_length { get; set; }
        byte map_entry_size { get; set; }
        ushort x_origin { get; set; }
        ushort y_origin { get; set; }
        public ushort image_width { get; set; }
        public ushort image_height { get; set; }
        byte pixel_depth { get; set; }
        byte image_desc { get; set; }
        public byte[] raw_data { get; set; }
        public byte[] image_id { get; set; } = new byte[4];

        public TGAImage(ushort width, ushort height, byte usage, byte alpha, byte format, byte mipmap_levels, byte[] raw_data)
        {

            byte num_alpha_bits;
            byte other_channel_bits;
                num_alpha_bits = 8;
                other_channel_bits = 24;

            byte pixel_depth = (byte)(num_alpha_bits + other_channel_bits);
            byte image_desc = (byte)(num_alpha_bits & 0xF);
            image_desc |= 0b10_0000;

            this.id_length = 0;
            this.map_type = 0;
            this.image_type = 2;
            this.map_origin = 0;
            this.map_length = 0;
            this.map_entry_size = 0;
            this.x_origin = 0;
            this.y_origin = 0;
            this.image_width = width;
            this.image_height = height;
            this.pixel_depth = pixel_depth;
            this.image_desc = image_desc;
            this.raw_data = raw_data;
            this.image_id = new byte[] { usage, alpha, format, mipmap_levels};
        }

        public TGAImage(string filepath)
        {
            if (!File.Exists(filepath))
                throw new Exception("TGA file does not exist!");
            using var file = File.OpenRead(filepath);
            var reader = new BinaryReader(file);

            this.id_length = reader.ReadByte();
            this.map_type = reader.ReadByte();
            this.image_type = reader.ReadByte();
            this.map_origin = reader.ReadUInt16();
            this.map_length = reader.ReadUInt16();
            this.map_entry_size = reader.ReadByte();
            this.x_origin = reader.ReadUInt16();
            this.y_origin = reader.ReadUInt16();
            this.image_width = reader.ReadUInt16();
            this.image_height = reader.ReadUInt16();
            this.pixel_depth = reader.ReadByte();
            this.image_desc = reader.ReadByte();
            this.raw_data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

            string file_name = Path.GetFileName(filepath);
            var splitted_name = file_name.Split('.');
            if (splitted_name.Length != 3)
            {
                throw new Exception("Missing DDT details in filename");
            }
            var splitted_params = splitted_name[1].Split(new char[] {',', '(', ')'});
            if (splitted_params.Length != 6)
            {
                throw new Exception("Missing params in DDT details");
            }

            this.image_id = new byte[] { Convert.ToByte(splitted_params[1]), Convert.ToByte(splitted_params[2]), Convert.ToByte(splitted_params[3]), Convert.ToByte(splitted_params[4]) };
        }


        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(id_length);
                    bw.Write(map_type);
                    bw.Write(image_type);
                    bw.Write(map_origin);
                    bw.Write(map_length);
                    bw.Write(map_entry_size);
                    bw.Write(x_origin);
                    bw.Write(y_origin);
                    bw.Write(image_width);
                    bw.Write(image_height);
                    bw.Write(pixel_depth);
                    bw.Write(image_desc);
                    bw.Write(raw_data);
                    bw.Write(image_id);
                    return ms.ToArray();
                }
            }
        }

    }

}
