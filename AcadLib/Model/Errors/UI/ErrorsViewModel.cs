using MicroMvvm;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            //"Дублирование блоков"                        
            Errors = new ObservableCollection<ErrorModel>(errors.Where(w => !string.IsNullOrEmpty(w.Message)).
                GroupBy(g => g).Select(s =>
                {
                    var errModel = new ErrorModel(s.ToList(), this);
                    errModel.SelectionChanged += ErrModel_SelectionChanged;
                    errModel.SameErrors?.Iterate(e => e.SelectionChanged += ErrModel_SelectionChanged);
                    return errModel;
                }).ToList());
            IsDublicateBlocksEnabled = errors.Any(e => e.Message?.StartsWith("Дублирование блоков") ?? false);
            CollapseAll = new RelayCommand(OnCollapseExecute, CanCollapseExecute);
            ExpandeAll = new RelayCommand(OnExpandedExecute, CanExpandExecute);
            ExportToExcel = new RelayCommand(OnExportToExcelExecute);
            ExportToTxt = new RelayCommand(OnExportToTxtExecute);
            DeleteSelectedDublicateBlocks = new RelayCommand(OnDeleteSelectedDublicateBlocksExecute);
            ErrorsCountInfo = errors.Count;
        }

        private void ErrModel_SelectionChanged(object sender, bool e)
        {
            CountSelectedErrors += e ? 1 : -1;
        }

        public ObservableCollection<ErrorModel> Errors { get; set; }
        public bool IsDialog { get { return isDialog; } set { isDialog = value; RaisePropertyChanged(); } }  
        bool isDialog;

        public RelayCommand CollapseAll { get; set; }
        public RelayCommand ExpandeAll { get; set; }
        public RelayCommand ExportToExcel { get; set; }
        public RelayCommand ExportToTxt { get; set; }
        public RelayCommand DeleteSelectedDublicateBlocks { get; set; }
        
        public int ErrorsCountInfo { get { return errorsCountInfo; } set { errorsCountInfo = value; RaisePropertyChanged(); } }
        int errorsCountInfo;

        public bool IsDublicateBlocksEnabled { get { return isDublicateBlocksEnabled; } set { isDublicateBlocksEnabled = value; RaisePropertyChanged(); } }
        bool isDublicateBlocksEnabled;

        public int CountSelectedErrors { get { return countSelectedErrors; } set { countSelectedErrors = value; RaisePropertyChanged(); } }
        int countSelectedErrors;

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
                var workBook = excelApp.Workbooks.Add();

                // Получаем активную таблицу
                var worksheet = workBook.ActiveSheet as Worksheet;
                worksheet.Name = "Ошибки";

                var row = 1;
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

        private void OnDeleteSelectedDublicateBlocksExecute()
        {
            List<IError> errors;
            var selectedErrors = GetSelectedErrors(out errors);
            try
            {
                Blocks.Dublicate.CheckDublicateBlocks.DeleteDublicates(errors);
                RemoveErrors(selectedErrors);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка удаления дубликатов блоков - {ex.Message}");   
            }            
        }    

        /// <summary>
        /// Удаление выделенных ошибок
        /// </summary>
        public void DeleteSelectedErrors()
        {
            List<IError> errors;
            var selectedErrors = GetSelectedErrors(out errors);
            RemoveErrors(selectedErrors);
        }

        private void RemoveErrors(List<ErrorModel> selectedErrors)
        {
            var countIsSelectedErr = 0;
            foreach (var item in selectedErrors)
            {
                if (item.parentErr == null)
                {
                    Errors.Remove(item);                    
                }
                else
                {
                    item.parentErr.SameErrors.Remove(item);                    
                }
                if (item.IsSelected) countIsSelectedErr++;
            }
            ErrorsCountInfo -= selectedErrors.Count;
            CountSelectedErrors -= countIsSelectedErr;
        }

        private List<ErrorModel> GetSelectedErrors(out List<IError> errors)
        {
            errors = new List<IError>();
            var selectedErrors = new List<ErrorModel>();            
            foreach (var err in Errors)
            {
                if (err.IsSelected)
                {
                    selectedErrors.Add(err);
                    errors.Add(err.Error);
                }
                else if (err.SameErrors != null)
                {
                    foreach (var innerErr in err.SameErrors)
                    {
                        if (innerErr.IsSelected)
                        {
                            selectedErrors.Add(innerErr);
                            errors.Add(innerErr.Error);
                        }
                    }
                }
            }
            return selectedErrors;
        }
    }
}
