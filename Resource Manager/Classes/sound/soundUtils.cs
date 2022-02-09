using System;
<<<<<<< HEAD
using System.IO;
using System.Numerics;
=======
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
using System.Threading.Tasks;

namespace Resource_Manager.Classes.sound
{
    public static class soundUtils
    {
<<<<<<< HEAD

        public async static Task<byte[]> DecryptSound(byte[] source)
        {


            ulong qword8 = 0x23966BA95E28C33F;
            ulong qword10 = 0x39BAE3441DB35873;
            ulong qword18 = 0x2AF92545ADDE0B65;



            int non_padded_size = source.Length;
            int padding_length = (8 - non_padded_size % 8) % 8;

            byte[] data = new byte[non_padded_size + padding_length];
            long currentBlock = data.Length / 8;

            Array.Copy(source, data, source.Length);


=======
        public static ulong RotateLeft(this ulong value, int count)
        {
            return (value << count) | (value >> (64 - count));
        }

        public async static Task<byte[]> DecryptSound(byte[] data)
        {

            ulong qword8 = 2564355413007450943;
            ulong qword10 = 4159887087525648499;
            ulong qword18 = 3096547199993908069;
            long currentBlock = data.Length / 8;
>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
            using (var streamReader = new MemoryStream(data, false))
            using (var reader = new BinaryReader(streamReader))
            using (var streamWriter = new MemoryStream())
            using (var writer = new BinaryWriter(streamWriter))
            {
                await Task.Run(() =>
                {
                    while (currentBlock > 0)
                    {
                        var block = reader.ReadUInt64();

<<<<<<< HEAD
                        qword18 = BitOperations.RotateLeft(qword10 * (qword18 + qword8), 32);
                        writer.Write(block ^ qword18);
                        currentBlock--;
                    }


=======
                        qword18 = RotateLeft(qword10 * (qword18 + qword8), 32);
                        writer.Write(block ^ qword18);
                        currentBlock--;
                    }
>>>>>>> 3f92ca114e5b86ed99edfd63366968ccb5d4834f
                });
                return streamWriter.ToArray();
            }

        }
    }
}
