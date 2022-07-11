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
        private MemoryStream DataStream { get; set; } = new MemoryStream();
        private MC02Header MC02Header { get; set; }

        public void check(string path)
        {

        }

        public void outro(string path)
        {

        }

        static void Main(string[] args)
        {
            string SaveFilePath="";//, dir = "D:/d/2/saves/", fname;

            //fname = "ds_slot_02.deadspace2saved";
            //fname = "ds_slot_01.deadspacesaved";
            //fname = "ds_slot_05";

            //string SaveFilePath = dir + fname;

            //Console.WriteLine(SaveFilePath);

            if (args.Length == 0) { }//return;
            else SaveFilePath = args[0];

            //if (!File.Exists(SaveFilePath))

            var DataStream = new MemoryStream();

            using (var fs = File.OpenRead(SaveFilePath)) fs.CopyTo(DataStream);

            DataStream.Seek(0x0, 0);

            string s1 = DataStream.ReadASCIIString(), s2;

            bool BE = false;

            if (s1 == "RGMH") ;//s2 = "PC";
            else if (s1 == "CON ") BE = true;//s2 = "XB";
            else s2 = "wrong file";

            int fst = BE ? 0xD000 : 0x2834;

            Console.WriteLine("0x{0:x}", fst);

            s2 = BE ? "BE" : "LE";

            Console.WriteLine(s2);

            DataStream.Seek(fst, 0);
            var MC02Header = new MC02Header
            {
                Magic = DataStream.ReadUInt32(BE),
                TotalLength = DataStream.ReadUInt32(BE),
                Chunk0Length = DataStream.ReadUInt32(BE),
                Chunk1Length = DataStream.ReadUInt32(BE),
                Checksum0 = DataStream.ReadUInt32(BE),
                Checksum1 = DataStream.ReadUInt32(BE),
                Checksum2 = DataStream.ReadUInt32(BE),
            };

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

            DataStream.Seek(0x8, SeekOrigin.Current);

            s1 = DataStream.ReadASCIIString();

            bool ds2 = (s1 == "ds_2");

            int cpo = 0x60 + (ds2 ? 4 : 0);

            var in2 = new uint[] { 0x4bc9c50b, 0xc08b592a, 0x544e4543, 0x53574f4b };

            var in1 = new uint[] { 0x4bc8a99c, 0xd622dbbb, 0x544e4543, 0x53574f4b };

            var o1 = new uint[] { 0x4bc78c36, 0x9f71988a, 0x544e4543, 0x53574f4b};

            var o2 = new uint[] { 0x36cb1994, 0x6663e415, 0x45544150, 0x3230314c };

            var ix = ds2 ? in2 : in1;
            var ox = ds2 ? o2 : o1;

            DataStream.Seek(fst + cpo, 0);

            for (int i = 0; i < 4; i++)
            {
                uint t = DataStream.ReadUInt32(BE);
                //Console.Write("0x{0:x}, ", t);
                if (t == ix[i]) // & t != ox[i]
                {
                    DataStream.Seek(-4, SeekOrigin.Current);
                    DataStream.WriteUInt32(ox[i], BE);
                    Console.WriteLine("{0:x} -> {1:x} ", t, ox[i]);
                    //Console.Write("0x{0:x}, ", t);
                }
                else break;
            }

            if (ds2) ChecksumsStuff.Hash(DataStream, BE, fst);
            else Console.WriteLine("not DS2");


            ChecksumsStuff.MC02(DataStream, MC02Header, BE, fst);

            Console.WriteLine("continue to save");
            Console.ReadLine();

            using (var fs = File.Create(SaveFilePath))DataStream.WriteTo(fs);

            return;

            //Console.WriteLine("end");
            //Console.ReadLine();
        }
    }
}
