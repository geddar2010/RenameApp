using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace RenameApp
{
    public class ImageFile
    {
        public static string[] FileExtensions = { ".tif", ".tiff", ".gif", ".jpeg", ".jpg", ".jif", ".jfif", ".psd", ".png", ".webp", ".bmp" };

        public ImageFile(string path)
        {
            this.FilePath = path;
            this.FileName = Path.GetFileName(FilePath);
            this.Extension = Path.GetExtension(FilePath);
            this.CreationDate = GetTakenDateTime(path) ?? File.GetCreationTime(path);
            //using (Image img = Image.FromFile(path))
            //{
            //    this.Width = img.Width;
            //    this.Height = img.Height;
            //    this.Resolution = Width* Height;
            //    ImageFormat format = img.RawFormat;                
            //    this.ImageType = format.ToString();                
            //    this.PixelDepth = Image.GetPixelFormatSize(img.PixelFormat) + "bpp";                
            //}                
        }

        public string ImageType { get; private set; }
        public string FilePath { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string PixelDepth { get; private set; }
        public DateTime CreationDate { get; private set; }
        public int Resolution { get; private set; }
        public string FileName { get; private set; }
        public string Extension { get; private set; }

        DateTime? GetTakenDateTime(string filePath)
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath);
            var directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            if (directory == null)
                return null;

            if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime))
                return dateTime;

            return null;
        }

    }
}
