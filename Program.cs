using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DlcPatcher
{
    internal class Program
    {
        private static ReadOnlySpan<string> HexTable => new string[]
        {
                "0", "1", "2", "3", "4", "5", "6", "7",
                "8", "9", "A", "B", "C", "D", "E", "F"
        };

        static void Main(string[] args)
        {
            FileInfo file = new FileInfo(args[0]);
            if (!file.Exists)
            {
                ErrorExit($"{file.FullName} does not exist.");
                return;
            }

            if (args[1] == string.Empty)
            {
                ErrorExit($"config does not exist.");
                return;
            }

            Sample? sample = JsonSerializer.Deserialize(File.ReadAllText(args[1]), SourceGenerationContext.Default.Sample);

            if (sample == null || sample.patchs == null)
            {
                ErrorExit($"problem with sample!");
                return;
            }

            Console.WriteLine($"Load Patchs: {sample.patchs.Count}");

            using (FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite))
            {
                for(int i = 0; i < sample.patchs.Count; i++)
                {
                    Patchs patch = sample.patchs[i];
                    Console.WriteLine();
                    Console.WriteLine($"============================================");
                    Console.WriteLine($"Offset: {patch.offset}");
                    Console.WriteLine($"Restore: {patch.restore}");
                    Console.WriteLine($"Patch: {patch.patch}");
                    Console.WriteLine($"============================================");
                    Console.WriteLine();
                    if (patch.restore.Length > 1)
                    {
                        List<byte> patchBytes = new List<byte>();
                        string[] gp = patch.patch.Split(' ');
                        foreach (string @byte in gp)
                        {
                            patchBytes.Add(Convert.ToByte(@byte, 16));
                        }
                        byte[] bytesPatch = new byte[gp.Length];
                        patchBytes.CopyTo(bytesPatch);
                        stream.Position = Convert.ToInt64(patch.offset, 16);
                        stream.Write(bytesPatch, 0, bytesPatch.Length);
                    }
                    else
                    {
                        stream.Position = Convert.ToInt64(patch.offset, 16);
                        int bytedecimal = stream.ReadByte();
                        if (HexFormat(bytedecimal) == patch.restore)
                        {
                            stream.WriteByte(Convert.ToByte(patch.restore, 16));
                        }
                    }
                }
            }

            Console.WriteLine("Success Patch!");
        }

        public static void ErrorExit(string content)
        {
            Console.WriteLine(content);
            Console.ReadLine();
        }

        public static string HexFormat(byte[] bytes)
        {
            List<string> resulthex = new List<string>();
            foreach (byte @byte in bytes)
            {
                int lastd = @byte;
                for(int i = 0; i < 2; i++)
                {
                    int d = lastd / 16;
                    int p = lastd % 16;
                    resulthex.Add(HexTable[p].ToString());
                    lastd = d;
                }
                resulthex.Reverse();
            }
            return string.Join(' ', resulthex);
        }

        public static string HexFormat(int @decimal)
        {
            char[] resulthex = new char[2];
            int lastd = @decimal;
            for (int i = 0; i < 2; i++)
            {
                int d = lastd / 16;
                int p = lastd % 16;
                resulthex[i] = Convert.ToChar(HexTable[p].ToString());
                lastd = d;
            }
            Array.Reverse(resulthex);
            return new string(resulthex);
        }
    }
}
