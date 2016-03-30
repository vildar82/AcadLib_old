using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoCAD_PIK_Manager;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Microsoft.Office.Interop.Excel;

namespace AcadLib.Errors
{
    public class FormError : Form
    {
        private Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
        private BindingSource _binding;
        private System.Windows.Forms.TextBox textBoxErr;
        private System.Windows.Forms.Button buttonShow;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.ListBox listBoxError;
        private ToolTip toolTip1;
        private System.Windows.Forms.Button buttonAllErrors;
        private List<Error> collapsedErrors;
        private List<Error> _errors;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonDel;
        private System.Windows.Forms.Button buttonDelAll;
        private ErrorProvider errorProvider1;
        private ContextMenuStrip contextMenuError;
        private ToolStripMenuItem toolStripMenuItemRemove;
        private bool isAllErrors;

        public FormError(bool modal) : this(Inspector.Errors, modal)
        {            
        }

        public FormError(List<Error> errors, bool modal)
        {
            this._errors = errors;
            InitializeComponent();
            EnableDialog(modal);

            UpdateCollapsedErrors();

            _binding = new BindingSource();
            bindingErrors(collapsedErrors);
            listBoxError.DataSource = _binding;
            listBoxError.DisplayMember = "ShortMsg";
            textBoxErr.DataBindings.Add("Text", _binding, "Message", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public void EnableDialog(bool modal)
        {
            buttonCancel.Visible = modal;
            buttonOk.Visible = modal;
        }

        private void bindingErrors(List<Error> errors)
        {
            _binding.DataSource = errors;
            _binding.ResetBindings(false);
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            Error err = (Error)listBoxError.SelectedItem;
            if (err != null && err.HasEntity && ed.Document != null)
            {
                Document curDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                if (ed.Document != curDoc)
                {
                    errorProvider1.SetError(buttonShow, $"Должен быть активен документ {ed.Document.Name}");
                }
                else
                {
                    if (err.Extents.Diagonal()>1)
                    {
                        ed.Zoom(err.Extents);
                        if (err.HasEntity)
                        {
                            err.IdEnt.FlickObjectHighlight(2, 100,100);
                        }
                    }                    
                }
            }
        }

        private void listBoxError_DoubleClick(object sender, EventArgs e)
        {
            buttonShow_Click(null, null);
        }

        private void listBoxError_SelectedIndexChanged(object sender, EventArgs e)
        {
            Error err = (Error)listBoxError.SelectedItem;
            buttonShow.Visible = err.HasEntity;
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBoxErr = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonDel = new System.Windows.Forms.Button();
            this.buttonAllErrors = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonShow = new System.Windows.Forms.Button();
            this.buttonDelAll = new System.Windows.Forms.Button();
            this.listBoxError = new System.Windows.Forms.ListBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.contextMenuError = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemRemove = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.contextMenuError.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxErr
            // 
            this.textBoxErr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxErr.Location = new System.Drawing.Point(12, 391);
            this.textBoxErr.Multiline = true;
            this.textBoxErr.Name = "textBoxErr";
            this.textBoxErr.ReadOnly = true;
            this.textBoxErr.Size = new System.Drawing.Size(688, 128);
            this.textBoxErr.TabIndex = 5;
            // 
            // buttonDel
            // 
            this.buttonDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDel.BackgroundImage = global::AcadLib.Properties.Resources.Delete;
            this.buttonDel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonDel.Location = new System.Drawing.Point(153, 357);
            this.buttonDel.Name = "buttonDel";
            this.buttonDel.Size = new System.Drawing.Size(33, 33);
            this.buttonDel.TabIndex = 9;
            this.toolTip1.SetToolTip(this.buttonDel, "Удалить выбранные дубликаты");
            this.buttonDel.UseVisualStyleBackColor = true;
            this.buttonDel.Visible = false;
            this.buttonDel.Click += new System.EventHandler(this.buttonDel_Click);
            // 
            // buttonAllErrors
            // 
            this.buttonAllErrors.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonAllErrors.BackgroundImage = global::AcadLib.Properties.Resources.Expand;
            this.buttonAllErrors.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonAllErrors.Location = new System.Drawing.Point(337, 361);
            this.buttonAllErrors.Name = "buttonAllErrors";
            this.buttonAllErrors.Size = new System.Drawing.Size(25, 25);
            this.buttonAllErrors.TabIndex = 7;
            this.toolTip1.SetToolTip(this.buttonAllErrors, "Показаны только неповторяющиеся сообщения. Показать все?");
            this.buttonAllErrors.UseVisualStyleBackColor = true;
            this.buttonAllErrors.Click += new System.EventHandler(this.buttonAllErrors_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExport.BackgroundImage = global::AcadLib.Properties.Resources.excel;
            this.buttonExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonExport.Location = new System.Drawing.Point(672, 361);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(28, 26);
            this.buttonExport.TabIndex = 6;
            this.toolTip1.SetToolTip(this.buttonExport, "Открыть список ошибок в Excel");
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonShow
            // 
            this.buttonShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonShow.BackgroundImage = global::AcadLib.Properties.Resources.Show;
            this.buttonShow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonShow.Location = new System.Drawing.Point(12, 359);
            this.buttonShow.Name = "buttonShow";
            this.buttonShow.Size = new System.Drawing.Size(54, 30);
            this.buttonShow.TabIndex = 4;
            this.toolTip1.SetToolTip(this.buttonShow, "Показать объект на чертеже. Так же работает двойной клик на записи в таблице.");
            this.buttonShow.UseVisualStyleBackColor = true;
            this.buttonShow.Click += new System.EventHandler(this.buttonShow_Click);
            // 
            // buttonDelAll
            // 
            this.buttonDelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelAll.BackgroundImage = global::AcadLib.Properties.Resources.DeleteAll;
            this.buttonDelAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonDelAll.Location = new System.Drawing.Point(192, 357);
            this.buttonDelAll.Name = "buttonDelAll";
            this.buttonDelAll.Size = new System.Drawing.Size(33, 33);
            this.buttonDelAll.TabIndex = 9;
            this.toolTip1.SetToolTip(this.buttonDelAll, "Удалить все дубликаты.");
            this.buttonDelAll.UseVisualStyleBackColor = true;
            this.buttonDelAll.Visible = false;
            this.buttonDelAll.Click += new System.EventHandler(this.buttonDelAll_Click);
            // 
            // listBoxError
            // 
            this.listBoxError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxError.ContextMenuStrip = this.contextMenuError;
            this.listBoxError.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxError.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxError.FormattingEnabled = true;
            this.listBoxError.ItemHeight = 18;
            this.listBoxError.Location = new System.Drawing.Point(12, 12);
            this.listBoxError.Name = "listBoxError";
            this.listBoxError.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxError.Size = new System.Drawing.Size(688, 344);
            this.listBoxError.TabIndex = 3;
            this.listBoxError.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxError_DrawItem);
            this.listBoxError.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxError_MeasureItem);
            this.listBoxError.SelectedIndexChanged += new System.EventHandler(this.listBoxError_SelectedIndexChanged);
            this.listBoxError.DoubleClick += new System.EventHandler(this.buttonShow_Click);
            this.listBoxError.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBoxError_KeyUp);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(572, 363);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Прервать";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(485, 363);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(81, 23);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "Продолжить";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // contextMenuError
            // 
            this.contextMenuError.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRemove});
            this.contextMenuError.Name = "contextMenuError";
            this.contextMenuError.Size = new System.Drawing.Size(138, 26);
            // 
            // toolStripMenuItemRemove
            // 
            this.toolStripMenuItemRemove.Name = "toolStripMenuItemRemove";
            this.toolStripMenuItemRemove.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItemRemove.Text = "Исключить";
            this.toolStripMenuItemRemove.Click += new System.EventHandler(this.toolStripMenuItemRemove_Click);
            // 
            // FormError
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(712, 531);
            this.Controls.Add(this.buttonDelAll);
            this.Controls.Add(this.buttonDel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonAllErrors);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.textBoxErr);
            this.Controls.Add(this.buttonShow);
            this.Controls.Add(this.listBoxError);
            this.Name = "FormError";
            this.Text = "Инфо";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.contextMenuError.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void buttonExport_Click(object sender, EventArgs e)
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
                foreach (var item in listBoxError.Items)
                {
                    var error = item as Error;
                    if (error == null)
                    {
                        continue;
                    }
                    worksheet.Cells[row, 1].Value = error.Message;
                    row++;
                }
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Сохранение ошибок в Excel");
            }
        }

        private void listBoxError_DrawItem(object sender, DrawItemEventArgs e)
        {
            System.Windows.Forms.ListBox list = (System.Windows.Forms.ListBox)sender;
            if (e.Index > -1)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                Error error = list.Items[e.Index] as Error;
                if (error != null)
                {
                    if (error.Icon != null)
                    {
                        System.Drawing.Image image = new Bitmap(error.Icon.ToBitmap(), 24, 24);
                        e.Graphics.DrawImage(image, e.Bounds.X, e.Bounds.Y + 1);
                    }
                    int xDelta = 24 + 5;
                    SizeF size = e.Graphics.MeasureString(error.ShortMsg, e.Font);
                    e.Graphics.DrawString(error.ShortMsg, e.Font, Brushes.Black, e.Bounds.Left + xDelta, e.Bounds.Top + (e.Bounds.Height / 2 - size.Height / 2));
                }
            }
        }

        private void listBoxError_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 26;
        }

        private void buttonAllErrors_Click(object sender, EventArgs e)
        {
            UpdateBinding();
            isAllErrors = !isAllErrors;
        }

        private void UpdateBinding()
        {
            if (isAllErrors)
            {
                // Показать уникальные сообщения;      
                UpdateCollapsedErrors();
                bindingErrors(collapsedErrors);
                //buttonAllErrors.Text = "Все ошибки";
                buttonAllErrors.BackgroundImage = Properties.Resources.Expand;
                toolTip1.SetToolTip(buttonAllErrors, "Показать все ошибки.");
            }
            else
            {
                // Показать все ошибки;         
                bindingErrors(_errors);                
                //buttonAllErrors.Text = "Без повторов";
                buttonAllErrors.BackgroundImage = Properties.Resources.Collapse;
                toolTip1.SetToolTip(buttonAllErrors, "Показать только неповторяющиеся сообщения.");
            }
        }

        private void UpdateCollapsedErrors()
        {
            collapsedErrors = Error.GetCollapsedErrors(_errors);
            if (_errors.Count == collapsedErrors.Count)
            {
                buttonAllErrors.Visible = false;
            }
            else
            {
                buttonAllErrors.Visible = true;
                isAllErrors = false;
            }
        }

        public void EnableDublicateButtons()
        {
            buttonDel.Visible = true;
            buttonDelAll.Visible = true;
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            var errors = listBoxError.SelectedItems.Cast<Error>().ToList();
            DeleteDublicates(errors);
        }

        private void buttonDelAll_Click(object sender, EventArgs e)
        {
            var errors = listBoxError.Items.Cast<Error>().ToList();
            DeleteDublicates(errors);
        }

        private void DeleteDublicates(List<Error> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                return;
            }

            try
            {
                Blocks.Dublicate.CheckDublicateBlocks.DeleteDublicates(errors);                
                errors.ForEach(e => _errors.Remove(e));
                UpdateBinding();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления дубликатов - {ex.Message}");
                Log.Error(ex, "FormError DeleteDublicates");
            }
        }

        private void listBoxError_KeyUp(object sender, KeyEventArgs e)
        {
            // Delete - удаление выбранных ошибок из списка
            if (e.KeyCode == Keys.Delete)
            {
                RemoveErrorSelected();
            }
        }

        private void RemoveErrorSelected()
        {
            if (listBoxError.SelectedItems != null)
            {
                foreach (var item in listBoxError.SelectedItems)
                {
                    _errors.Remove((Error)item);
                }
                UpdateBinding();
            }
        }

        private void toolStripMenuItemRemove_Click(object sender, EventArgs e)
        {
            RemoveErrorSelected();
        }
    }
}
