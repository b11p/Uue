using System;
using System.IO;
using UueLib;

namespace Uue
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("file path");
            string path = Console.ReadLine().Trim('"');
            byte[] data = File.ReadAllBytes(path);
            string encode = UUEncoding.ToUUEncodingString(data, 0, data.Length, true, 80);
            File.WriteAllText(path + ".uue.txt", encode);
        }


    }
}
