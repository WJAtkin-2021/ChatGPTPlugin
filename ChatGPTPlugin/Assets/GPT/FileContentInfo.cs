using System;
using System.IO;

namespace GPTPlugin
{
    public class FileContentInfo
    {
        public FileContentInfo(byte[] fileContent, string fileName)
        {
            if (fileContent == null || fileContent.Length == 0)
            {
                throw new ArgumentException("FileContent is required");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("FileName is required");
            }

            FileContent = fileContent;
            FileName = fileName;
        }

        public byte[] FileContent { get; set; }

        public string FileName { get; set; }

        public FileContentInfo() { }

        public static FileContentInfo Load(string file)
        {
            var bytes = File.ReadAllBytes(file);
            var name = new FileInfo(file).Name;
            return new FileContentInfo(bytes, name);
        }

        public void Save(string path)
        {
            File.WriteAllBytes(path, FileContent);
        }
    }
}
