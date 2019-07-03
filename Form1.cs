using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RenameApp
{
    public partial class Form1 : Form
    {
        const string dragContents = "Перетащите каталоги и файлы с изображениями на данную форму для их обработки";

        private List<ImageFile> imagesToModify = new List<ImageFile>();
        private List<ImageFile> imagesSuccessful = new List<ImageFile>();
        private List<ImageFile> imagesUnsuccessful = new List<ImageFile>();
        private List<string> imagePaths = new List<string>();


        public Form1()
        {
            this.InitializeComponent();
            this.Init();
        }

        private void Init()
        {
            imagesToModify.Clear();
            imagesSuccessful.Clear();
            imagesUnsuccessful.Clear();

            imagePaths.Clear();

            progressBar.Value = 0;
            progressBar.Minimum = 0;
            progressBar.Maximum = 1;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                ModifyDates(files);
            }
        }

        private void BtnSelectFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    ModifyDates(new[] { fbd.SelectedPath });
                }
            }
        }

        private void BtnClearLogs_Click(object sender, EventArgs e)
        {
            logBox.Items.Clear();
        }

        private void ModifyDates(string[] paths)
        {

            Init();
            lblStatus.Text = "Обработка...";

            foreach (string loc in paths)
            {
                GetImageFilesRecurse(loc);
            }

            logBox.Items.Add($"Добавлено {imagePaths.Count} изображений для обработки");

            foreach (var fileLoc in imagePaths)
            {
                try
                {
                    imagesToModify.Add(new ImageFile(fileLoc));
                }
                catch (Exception ex)
                {
                    logBox.Items.Add($"Невозможно прочесть атрибуты для файла {fileLoc}. Исключение: {ex.Message}");
                }
            }

            logBox.Items.Add($"Атрибуты получены для {imagesToModify.Count} изображений.");

            progressBar.Maximum = imagesToModify.Count;

            foreach (var image in imagesToModify)
            {
                try
                {
                    if (!image.HasExtendedProperties)
                    {
                        RenameFileDialog dlg = new RenameFileDialog(image.FileName, image.CreationDate);

                        if (dlg.ShowDialog(this) == DialogResult.OK)
                        {
                            var fileName = dlg.tbFileName.Text.Trim();

                            if (fileName == "")
                                fileName = image.FileName;

                            var creationDate = dlg.dtpCreationDate.Value;
                            var filePath = image.FilePath.Replace(image.FileName, fileName);

                            File.Move(image.FilePath, filePath);

                            image.SetFileNameAndCreationDate(fileName, creationDate);
                        }

                        dlg.Dispose();
                    }

                    File.SetCreationTime(image.FilePath, image.CreationDate);
                    File.SetLastWriteTime(image.FilePath, image.CreationDate);

                    if (File.GetCreationTime(image.FilePath) == image.CreationDate &&
                    File.GetLastWriteTime(image.FilePath) == image.CreationDate)
                    {
                        imagesSuccessful.Add(image);
                        File.SetLastAccessTime(image.FilePath, image.CreationDate);
                    }
                    else
                    {
                        imagesUnsuccessful.Add(image);
                        logBox.Items.Add($"Невозможно изменить атрибуты изображения {image.FilePath}. Недостаточно прав.");
                    }
                }
                catch (Exception ex)
                {
                    imagesUnsuccessful.Add(image);
                    logBox.Items.Add($"Невозможно изменить атрибуты изображения {image.FilePath}: {ex.Message}");
                }

                progressBar.Value += 1;
            }

            logBox.Items.Add($"Установка атрибутов для {imagesToModify.Count} завершена (успех: {imagesSuccessful.Count}, отказ: {imagesUnsuccessful.Count})");
            lblStatus.Text = "Обработка завершена";
        }

        private void GetImageFilesRecurse(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    List<string> innerPaths = new List<string>();
                    innerPaths.AddRange(Directory.GetDirectories(path));
                    innerPaths.AddRange(Directory.GetFiles(path));
                    foreach (var p in innerPaths)
                    {
                        GetImageFilesRecurse(p);
                    }
                }
                catch (Exception ex)
                {
                    logBox.Items.Add($"Обработка пути {path} завершилась с ошибкой: " + ex.Message);
                }
            }
            else if (File.Exists(path) && ImageFile.FileExtensions.Any(f => path.ToLower().EndsWith(f)))
            {
                imagePaths.Add(path);
            }
        }
    }
}

