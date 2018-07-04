namespace AcadLib.Utils.Tabs.UI
{
    using System;
    using System.IO;
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
                if (!NetLib.IO.Path.FileExists(drawing))
                {
                    Name += " (файл не найден)";
                    Err = "Файл не найден";
                    return;
                }

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