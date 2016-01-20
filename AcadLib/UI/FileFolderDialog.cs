using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AcadLib.UI
{
   /// <summary>
   /// Диалог выбора файлов с возможностью включания выбора папки IsFolderDialog=true.
   /// Если включить IsFolderDialog, то отключается проверка CheckFileExists и CheckPathExists и dialog.FileName = " ";
   /// </summary>
   public class FileFolderDialog : CommonDialog
   {
      public OpenFileDialog Dialog { get; private set; } = new OpenFileDialog();            

      public bool IsFolderDialog { get; set; }            
      
      public new DialogResult ShowDialog()
      {
         return this.ShowDialog(null);
      }

      public new DialogResult ShowDialog(IWin32Window owner)
      {
         if (IsFolderDialog)
         {            
            Dialog.FileName = "п";
            Dialog.Title += " Для выбора текущей папки оставьте в поле имени файла 'п' и нажмите открыть.";
            Dialog.CheckFileExists = false;
            Dialog.CheckPathExists = false;
            Dialog.ValidateNames = false;                        
         }

         if (owner == null)
            return Dialog.ShowDialog();
         else
            return Dialog.ShowDialog(owner);
      }     

      /// <summary>
      // Helper property. Parses FilePath into either folder path (if Folder Selection. is set)
      // or returns file path
      /// </summary>
      public string SelectedPath
      {
         get
         {
            if (IsFolderDialog)
            {
               return Path.GetDirectoryName(Dialog.FileName);
            }
            else
            {
               return Dialog.FileName;               
            }            
         }         
      }      

      public override void Reset()
      {
         Dialog.Reset();
      }

      protected override bool RunDialog(IntPtr hwndOwner)
      {
         return true;
      }
   }
}