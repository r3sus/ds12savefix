using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DeadSpace2SaveEditor.Code;

namespace ds2fix
{
    class Program
    {
        //private MemoryStream DataStream { get; set; } = new MemoryStream();
        //private MC02Header MC02Header { get; set; }

        static void Main(string[] args)
        {
            string SaveFilePath = "..\\..\\x2pi\\ds_slot_05";

            if (args.Length == 0) { }//return;
            else SaveFilePath = args[0];
            
            var DataStream = new MemoryStream();

            using (var fs = File.OpenRead(SaveFilePath)) fs.CopyTo(DataStream);
            /*
            DataStream.Seek(0x0, SeekOrigin.Begin);
            var magic = DataStream.ReadInt32();
            if (magic != 1213024082) // RGMH
            {
                return;
            }

            DataStream.Seek(0x28, SeekOrigin.Begin);
            var sig = DataStream.ReadUnicodeString();
            if (sig != "Dead Space 2")
            {
                return;
            }
            */
            DataStream.Seek(0xD000, SeekOrigin.Begin);
            var MC02Header = new MC02Header
            {
                Magic = DataStream.ReadUInt32(),
                TotalLength = DataStream.ReadUInt32(),
                Chunk0Length = DataStream.ReadUInt32(),
                Chunk1Length = DataStream.ReadUInt32(),
                Checksum0 = DataStream.ReadUInt32(),
                Checksum1 = DataStream.ReadUInt32(),
                Checksum2 = DataStream.ReadUInt32(),
            };
            /*
            Console.WriteLine(MC02Header.Checksum0);
            Console.WriteLine(MC02Header.Checksum1);
            Console.WriteLine(MC02Header.Checksum2);
            */
            if (MC02Header.Magic != 1296248882) // 20CM
            {
                Console.WriteLine("MC02");
                return;
            }

            if (MC02Header.TotalLength != MC02Header.Chunk0Length + MC02Header.Chunk1Length + 0x1C)
            {
                Console.WriteLine("Length");
                return;
            }
            

            ChecksumsStuff.FixChecksums(DataStream, MC02Header);
            //using (var fs = File.Create(SaveFilePath+".fix"))
            {
                //DataStream.WriteTo(fs);
            }

            //Console.WriteLine("end");
            //Console.ReadLine();
        }
    }
}
