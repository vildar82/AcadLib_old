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

      public FormError()
      {
         InitializeComponent();

         _binding = new BindingSource();
         _binding.DataSource = Inspector.Errors;
         listBoxError.DataSource = _binding;
         listBoxError.DisplayMember = "ShortMsg";         
         textBoxErr.DataBindings.Add("Text", _binding, "Message", false, DataSourceUpdateMode.OnPropertyChanged);
      }

      private void buttonShow_Click(object sender, EventArgs e)
      {         
         Error err = (Error)listBoxError.SelectedItem;
         if (err.HasEntity)
            ed.Zoom(err.Extents);
      }

      private void listBoxError_DoubleClick(object sender, EventArgs e)
      {
         buttonShow_Click(null, null);
      }

      private void listBoxError_SelectedIndexChanged(object sender, EventArgs e)
      {
         Error err = (Error)listBoxError.SelectedItem;
         buttonShow.Enabled = err.HasEntity;
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
         this.buttonShow = new System.Windows.Forms.Button();
         this.buttonExport = new System.Windows.Forms.Button();
         this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
         this.listBoxError = new System.Windows.Forms.ListBox();
         this.SuspendLayout();
         // 
         // textBoxErr
         // 
         this.textBoxErr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.textBoxErr.Location = new System.Drawing.Point(12, 375);
         this.textBoxErr.Multiline = true;
         this.textBoxErr.Name = "textBoxErr";
         this.textBoxErr.ReadOnly = true;
         this.textBoxErr.Size = new System.Drawing.Size(642, 128);
         this.textBoxErr.TabIndex = 5;
         // 
         // buttonShow
         // 
         this.buttonShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.buttonShow.Location = new System.Drawing.Point(12, 339);
         this.buttonShow.Name = "buttonShow";
         this.buttonShow.Size = new System.Drawing.Size(109, 30);
         this.buttonShow.TabIndex = 4;
         this.buttonShow.Text = "Показать";
         this.buttonShow.UseVisualStyleBackColor = true;
         this.buttonShow.Click += new System.EventHandler(this.buttonShow_Click);
         // 
         // buttonExport
         // 
         this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.buttonExport.Location = new System.Drawing.Point(579, 343);
         this.buttonExport.Name = "buttonExport";
         this.buttonExport.Size = new System.Drawing.Size(75, 23);
         this.buttonExport.TabIndex = 6;
         this.buttonExport.Text = "Сохранить";
         this.toolTip1.SetToolTip(this.buttonExport, "Сохранить список ошибок в Excel");
         this.buttonExport.UseVisualStyleBackColor = true;
         this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
         // 
         // listBoxError
         // 
         this.listBoxError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.listBoxError.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
         this.listBoxError.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.listBoxError.FormattingEnabled = true;
         this.listBoxError.ItemHeight = 18;
         this.listBoxError.Location = new System.Drawing.Point(12, 12);
         this.listBoxError.Name = "listBoxError";
         this.listBoxError.Size = new System.Drawing.Size(642, 310);
         this.listBoxError.TabIndex = 3;
         this.listBoxError.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxError_DrawItem);
         this.listBoxError.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxError_MeasureItem);
         this.listBoxError.SelectedIndexChanged += new System.EventHandler(this.listBoxError_SelectedIndexChanged);
         this.listBoxError.DoubleClick += new System.EventHandler(this.buttonShow_Click);
         // 
         // FormError
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(666, 515);
         this.Controls.Add(this.buttonExport);
         this.Controls.Add(this.textBoxErr);
         this.Controls.Add(this.buttonShow);
         this.Controls.Add(this.listBoxError);
         this.Name = "FormError";
         this.Text = "FormError";
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
            foreach (var item in Inspector.Errors)
            {
               worksheet.Cells[row, 1].Value = item.Message;
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
                  e.Graphics.DrawImage(image, e.Bounds.X, e.Bounds.Y);
               }
               int xDelta = 24 + 5;
               SizeF size = e.Graphics.MeasureString(error.Message, e.Font);
               e.Graphics.DrawString(error.Message, e.Font, Brushes.Black, e.Bounds.Left + xDelta, e.Bounds.Top + (e.Bounds.Height / 2 - size.Height / 2));
            }
         }
      }

      private void listBoxError_MeasureItem(object sender, MeasureItemEventArgs e)
      {
         e.ItemHeight = 24;
      }
   }
}
