using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;

#pragma warning disable 0618

namespace GhysX.Framework.Utilities
{

    public class ZipHelper
    {
        private const int BUFFER_LENGTH = 2048;
        #region Zip  
        public static void ZipCompress(List<string> sourceFileLists, string descFile, int compression)
        {
            if (compression < 0 || compression > 9)
            {
                throw new ArgumentException("错误的压缩级别");
            }
            if (!Directory.Exists(new FileInfo(descFile).Directory.ToString()))
            {
                throw new ArgumentException("保存目录不存在");
            }
            foreach (string c in sourceFileLists)
            {
                if (!File.Exists(c))
                {
                    throw new ArgumentException(string.Format("文件{0} 不存在！", c));
                }
            }
            Crc32 crc32 = new Crc32();
            using (ZipOutputStream stream = new ZipOutputStream(File.Create(descFile)))
            {
                stream.SetLevel(compression);
                ZipEntry entry;
                for (int i = 0; i < sourceFileLists.Count; i++)
                {
                    entry = new ZipEntry(Path.GetFileName(sourceFileLists[i]));
                    entry.DateTime = DateTime.Now;
                    using (FileStream fs = File.OpenRead(sourceFileLists[i]))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        entry.Size = fs.Length;
                        crc32.Reset();
                        crc32.Update(buffer);
                        entry.Crc = crc32.Value;
                        stream.PutNextEntry(entry);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    stream.CloseEntry();
                }

            }
        }
        public static void ZipDeCompress(string sourceFile, string path)
        {
            if (!File.Exists(sourceFile))
            {
                throw new ArgumentException("要解压的文件不存在。");
            }
            if (!Directory.Exists(path))
            {
                throw new ArgumentException("要解压到的目录不存在！");
            }
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(sourceFile)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string fileName = System.IO.Path.GetFileName(theEntry.Name);
                    if (fileName != string.Empty)
                    {
                        using (FileStream streamWriter = File.Create(path + @"\" + theEntry.Name))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public static string ZipCompress(string text)
        {
            string result = string.Empty;
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] dData = ZipCompress(data);
            result = Convert.ToBase64String(dData);
            Array.Clear(dData, 0, dData.Length);
            return result;
        }
        public static string ZipDeCompress(string text)
        {
            string result = string.Empty;
            byte[] data = Convert.FromBase64String(text);
            byte[] dData = ZipDeCompress(data);
            result = Encoding.UTF8.GetString(dData);
            Array.Clear(dData, 0, dData.Length);
            return result;
        }
        public static byte[] ZipCompress(byte[] data, bool isClearData = true)
        {
            byte[] bytes = null;
            Deflater f = new Deflater(Deflater.BEST_COMPRESSION);
            f.SetInput(data);
            f.Finish();
            int count = 0;
            using (MemoryStream o = new MemoryStream(data.Length))
            {
                byte[] buffer = new byte[BUFFER_LENGTH];
                while (!f.IsFinished)
                {
                    count = f.Deflate(buffer);
                    o.Write(buffer, 0, count);
                }
                bytes = o.ToArray();
            }
            if (isClearData)
            {
                Array.Clear(data, 0, data.Length);
            }
            return bytes;
        }
        public static byte[] ZipDeCompress(byte[] data, bool isClearData = true)
        {
            byte[] bytes = null;
            Inflater f = new Inflater();
            f.SetInput(data);
            int count = 0;
            using (MemoryStream o = new MemoryStream(data.Length))
            {
                byte[] buffer = new byte[BUFFER_LENGTH];
                while (!f.IsFinished)
                {
                    count = f.Inflate(buffer);
                    o.Write(buffer, 0, count);
                }
                bytes = o.ToArray();
            }
            if (isClearData)
            {
                Array.Clear(data, 0, count);
            }
            return bytes;
        }
        #endregion

        #region GZip
        public static string GZipCompress(string text)
        {
            string result = string.Empty;
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] cData = GZipCompress(data);
            result = Convert.ToBase64String(cData);
            Array.Clear(cData, 0, cData.Length);
            return result;
        }
        public static string GZipDeCompress(string text)
        {
            string result = string.Empty;
            byte[] data = Convert.FromBase64String(text);
            byte[] cData = GZipDeCompress(data);
            result = Encoding.UTF8.GetString(cData);
            Array.Clear(cData, 0, cData.Length);
            return result;
        }
        public static byte[] GZipCompress(byte[] data, bool isClearData = true)
        {
            byte[] bytes = null;
            try
            {
                using (MemoryStream o = new MemoryStream())
                {
                    using (Stream s = new GZipOutputStream(o))
                    {
                        s.Write(data, 0, data.Length);
                        s.Flush();
                    }
                    bytes = o.ToArray();
                }
            }
            catch (SharpZipBaseException)
            {
            }
            catch (IndexOutOfRangeException)
            {
            }
            if (isClearData)
                Array.Clear(data, 0, data.Length);
            return bytes;
        }

        public static byte[] GZipDeCompress(byte[] data, bool isClearData = true)
        {
            byte[] bytes = null;
            try
            {
                using (MemoryStream o = new MemoryStream())
                {
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        using (Stream s = new GZipInputStream(ms))
                        {
                            s.Flush();
                            int size = 0;
                            byte[] buffer = new byte[BUFFER_LENGTH];
                            while ((size = s.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                o.Write(buffer, 0, size);
                            }
                        }
                    }
                    bytes = o.ToArray();
                }
            }
            catch (SharpZipBaseException)
            {
            }
            catch (IndexOutOfRangeException)
            {
            }
            if (isClearData)
                Array.Clear(data, 0, data.Length);
            return bytes;
        }
        #endregion

        #region Tar
        public static string TarCompress(string text)
        {
            string result = null;
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] dData = TarCompress(data);
            result = Convert.ToBase64String(dData);
            Array.Clear(dData, 0, dData.Length);
            return result;
        }
        public static string TarDeCompress(string text)
        {
            string result = null;
            byte[] data = Convert.FromBase64String(text);
            byte[] dData = TarDeCompress(data);
            result = Encoding.UTF8.GetString(dData);
            Array.Clear(dData, 0, dData.Length);
            return result;
        }
        public static byte[] TarCompress(byte[] data, bool isClearData = true)
        {
            byte[] bytes = null;
            using (MemoryStream o = new MemoryStream())
            {
                using (Stream s = new TarOutputStream(o))
                {
                    s.Write(data, 0, data.Length);
                    s.Flush();
                }
                bytes = o.ToArray();
            }
            if (isClearData)
                Array.Clear(data, 0, data.Length);
            return bytes;
        }
        public static byte[] TarDeCompress(byte[] data, bool isClearData = true)
        {
            byte[] bytes = null;
            using (MemoryStream o = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (Stream s = new TarInputStream(ms))
                    {
                        s.Flush();
                        int size = 0;
                        byte[] buffer = new byte[BUFFER_LENGTH];
                        while ((size = s.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            o.Write(buffer, 0, size);
                        }
                    }
                }
                bytes = o.ToArray();
            }
            if (isClearData)
                Array.Clear(data, 0, data.Length);
            return bytes;
        }
        #endregion

        #region BZip
        public static string BZipCompress(string text)
        {
            string result = null;
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] dData = BZipCompress(data);
            result = Convert.ToBase64String(dData);
            Array.Clear(dData, 0, dData.Length);
            return result;
        }
        public static string BZipDeCompress(string text)
        {
            string result = null;
            byte[] data = Convert.FromBase64String(text);
            byte[] dData = BZipDeCompress(data);
            result = Encoding.UTF8.GetString(dData);
            Array.Clear(dData, 0, dData.Length);
            return result;
        }
        public static byte[] BZipCompress(byte[] data, bool isClearData = true)
        {
            byte[] bytes = null;
            using (MemoryStream o = new MemoryStream())
            {
                using (Stream s = new BZip2OutputStream(o))
                {
                    s.Write(data, 0, data.Length);
                    s.Flush();
                }
                bytes = o.ToArray();
            }
            if (isClearData)
                Array.Clear(data, 0, data.Length);
            return bytes;
        }
        public static byte[] BZipDeCompress(byte[] data, bool isClearData = true)
        {
            byte[] bytes = null;
            using (MemoryStream o = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (Stream s = new BZip2InputStream(ms))
                    {
                        s.Flush();
                        int size = 0;
                        byte[] buffer = new byte[BUFFER_LENGTH];
                        while ((size = s.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            o.Write(buffer, 0, size);
                        }
                    }
                }
                bytes = o.ToArray();
            }
            if (isClearData)
                Array.Clear(data, 0, data.Length);
            return bytes;
        }
        #endregion
    }
}