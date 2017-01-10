using MicroMvvm;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AcadLib.Errors
{
    public class ErrorsViewModel : ViewModelBase
    {
        public ErrorsViewModel()
        {

        }

        public ErrorsViewModel(List<IError> errors)
        {
            // Группировка ошибок
            Errors = new ObservableCollection<ErrorModel>(errors.Where(w=>!string.IsNullOrEmpty(w.Message)).
                GroupBy(g=>g).Select(s=> new ErrorModel(s.ToList())).ToList());
            CollapseAll = new RelayCommand(OnCollapseExecute, CanCollapseExecute);
            ExpandeAll = new RelayCommand(OnExpandedExecute, CanExpandExecute);
            ExportToExcel = new RelayCommand(OnExportToExcelExecute);
            ExportToTxt = new RelayCommand(OnExportToTxtExecute);
            ErrorsCountInfo = errors.Count.ToString();
        }        

        public ObservableCollection<ErrorModel> Errors { get; set; }
        public bool IsDialog { get { return isDialog; } set { isDialog = value; RaisePropertyChanged(); } }
        bool isDialog;

        public RelayCommand CollapseAll { get; set; }
        public RelayCommand ExpandeAll { get; set; }
        public RelayCommand ExportToExcel { get; set; }
        public RelayCommand ExportToTxt { get; set; }
        public string ErrorsCountInfo { get; set; }

        private bool CanCollapseExecute()
        {
            return Errors.Any(a => a.IsExpanded);
        }
        private void OnCollapseExecute()
        {
            foreach (var item in Errors)
            {
                item.IsExpanded = false;
            }
        }
        private bool CanExpandExecute()
        {
            return Errors.Any(a=>a.SameErrors!= null) && Errors.Any(a=>!a.IsExpanded);
        }
        private void OnExpandedExecute()
        {
            foreach (var item in Errors)
            {
                item.IsExpanded = true;
            }
        }

        private void OnExportToExcelExecute()
        {
            try
            {                
                var excelApp = new Microsoft.Office.Interop.Excel.Application { DisplayAlerts = false };
                if (excelApp == null)
                    return;

                // Открываем книгу
                Workbook workBook = excelApp.Workbooks.Add();

                // Получаем активную таблицу
                Worksheet worksheet = workBook.ActiveSheet as Worksheet;
                worksheet.Name = "Ошибки";

                int row = 1;
                // Название
                worksheet.Cells[row, 1].Value = "Список ошибок";
                row++;
                foreach (var err in Errors)
                {
                    if (err.SameErrors == null)
                    {
                        worksheet.Cells[row, 1].Value = err.Message;
                        row++;
                    }
                    else
                    {
                        foreach (var item in err.SameErrors)
                        {
                            worksheet.Cells[row, 1].Value = item.Message;
                            row++;
                        }
                    }                    
                }
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Log.Error(ex, "Сохранение ошибок в Excel");
            }
        }

        private void OnExportToTxtExecute ()
        {
            var sbText = new StringBuilder("Список ошибок:").AppendLine();            
            foreach (var err in Errors)
            {
                if (err.SameErrors == null)
                {
                    sbText.AppendLine(err.Message);
                }
                else
                {
                    foreach (var item in err.SameErrors)
                    {
                        sbText.AppendLine(err.Message);
                    }
                }
            }            
            var fileTxt = Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
            File.WriteAllText(fileTxt, sbText.ToString());
            Process.Start(fileTxt);
        }
    }
}
