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
            this.FileName = Path.GetFileNameWithoutExtension(FilePath);
            this.Extension = Path.GetExtension(FilePath);
            this.CreationDate = GetTakenDateTime(path);
        }

        public void SetFileNameAndCreationDate(string newFileName, DateTime newCreationDate)
        {
            this.CreationDate = newCreationDate;
            this.FilePath = this.FilePath.Replace(this.FileName, newFileName);
            this.FileName = newFileName;
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
        public bool HasExtendedProperties { get; private set; }

        DateTime GetTakenDateTime(string filePath)
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath);
            var directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            DateTime dateTime = File.GetCreationTime(filePath);

            if (directory != null && directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out dateTime))
            {
                this.HasExtendedProperties = true;
            }

            return dateTime;
        }

    }
}
