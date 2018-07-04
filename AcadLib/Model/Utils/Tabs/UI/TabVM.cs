namespace AcadLib.Utils.Tabs.UI
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using NetLib.WPF;
    using NetLib.WPF.Data;

    public class TabVM : BaseModel
    {
        public TabVM(string drawing, bool restore)
        {
            File = drawing;
            Name = Path.GetFileNameWithoutExtension(drawing);
            Restore = restore;
            try
            {
                CheckFileExist();
                var fi = new FileInfo(drawing);
                DateLastWrite = System.IO.File.GetLastWriteTime(drawing);
                Size = fi.Length;
                Image = NetLib.IO.Path.GetThumbnail(drawing).ConvertToBitmapImage();
            }
            catch (Exception ex)
            {
                Err = ex.Message;
            }
        }

        private async void CheckFileExist()
        {
            var isFileExists = await NetLib.IO.Path.FileExistsAsync(File);
            if (!isFileExists)
            {
                Name += " (файл не найден)";
                Err = "Файл не найден";
            }
        }

        public string Name { get; set; }

        public string File { get; set; }

        public bool Restore { get; set; }

        public ImageSource Image { get; set; }

        public DateTime? DateLastWrite { get; set; }

        public long Size { get; set; }

        public string Err { get; set; }

        public DateTime Start { get; set; }
    }
}