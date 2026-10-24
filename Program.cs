using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Patcher
{
    internal class Program
    {
        class Hex
        {
            public static ReadOnlySpan<string> HexTable => new string[]
{
                "0", "1", "2", "3", "4", "5", "6", "7",
                "8", "9", "A", "B", "C", "D", "E", "F"
            };

            public static int GetDecimal(char hex)
            {
                if (Char.IsDigit(hex))
                {
                    return Convert.ToInt32(hex);
                }
                else
                {
                    for(int i = 0; i < HexTable.Length; i++)
                    {
                        if(hex.ToString() == HexTable[i])
                        {
                            return i;
                        }
                    }

                    return -1;
                }
            }
        }

        static void Main(string[] args)
        {
            FileInfo file = new FileInfo(args[0]);
            FileInfo fileconfig = new FileInfo(args[1]);
            if (!file.Exists || args[0] == string.Empty)
            {
                ErrorExit($"{file.FullName} does not exist.");
                return;
            }

            if (!fileconfig.Exists || args[1] == string.Empty)
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

                    if(DecimalFormat(patch.offset) < stream.Length)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"============================================");
                        Console.WriteLine($"Offset: {patch.offset}");
                        Console.WriteLine($"Restore: {patch.restore}");
                        Console.WriteLine($"Patch: {patch.patch}");
                        Console.WriteLine($"============================================");
                        Console.WriteLine();

                        if (patch.restore.Length > 2)
                        {
                            List<byte> patchBytes = new List<byte>();
                            foreach (string @byte in patch.patch.Split(' '))
                            {
                                patchBytes.Add(Convert.ToByte(@byte, 16));
                            }
                            byte[] bytesPatch = new byte[patchBytes.Count];
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
                                stream.WriteByte(Convert.ToByte(patch.patch, 16));
                            }
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
                    resulthex.Add(Hex.HexTable[p].ToString());
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
                resulthex[i] = Convert.ToChar(Hex.HexTable[p].ToString());
                lastd = d;
            }
            Array.Reverse(resulthex);
            return new string(resulthex);
        }

        public static int DecimalFormat(string hex)
        {
            int result = 0;
            string formathex = hex.Replace("0x", "");

            for(int i = 0; i < formathex.Length; i++)
            {
                char c = formathex[formathex.Length - 1 - i];
                double pow = Math.Pow(16, i);
                if (Char.IsDigit(c))
                {
                    if(int.TryParse(c.ToString(), out int convert))
                    {
                        result += (convert * (int)pow);
                    }
                }
                else
                {
                    int getDecimal = Hex.GetDecimal(c);
                    if (getDecimal != -1)
                    {
                        result += (getDecimal * (int)pow);
                    }
                }
            }

            return result;
        }
    }
}
