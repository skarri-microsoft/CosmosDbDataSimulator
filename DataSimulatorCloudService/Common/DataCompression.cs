﻿namespace Common
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using ICSharpCode.SharpZipLib.Zip;

    public class DataCompression
    {
        // Another way to compress the data and send it to the event hub
        // Not used any where.
        public static byte[] GetZipOutputInBytes(string data, string entryName)
        {
            using (Stream memOutput = new MemoryStream())
            using (ZipOutputStream zipOutput = new ZipOutputStream(memOutput))
            {
                zipOutput.SetLevel(9);

                ZipEntry entry = new ZipEntry(entryName);
                entry.DateTime = DateTime.Now;
                zipOutput.PutNextEntry(entry);
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                zipOutput.Write(dataBytes, 0, dataBytes.Length);
                zipOutput.Finish();

                byte[] newBytes = new byte[memOutput.Length];
                memOutput.Seek(0, SeekOrigin.Begin);
                memOutput.Read(newBytes, 0, newBytes.Length);

                zipOutput.Close();

                return newBytes;
            }
        }

        public static byte[] GetGZipContentInBytes(string data)
        {
            byte[] compressed;
            using (var outStream = new MemoryStream())
            {
                using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
                    mStream.CopyTo(tinyStream);

                compressed = outStream.ToArray();
            }

            return compressed;
        }


    }
}
