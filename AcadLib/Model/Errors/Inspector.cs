using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AutoCAD_PIK_Manager;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Errors
{
    public static class Inspector
    {
        private static Database _db;
        private static Document _doc;
        private static Editor _ed;
        public static List<Error> Errors { get; private set; }

        public static bool HasErrors { get { return Errors.Count > 0; } }

        static Inspector()
        {
            Clear();
        }

        public static void Clear()
        {
            _doc = Application.DocumentManager.MdiActiveDocument;
            _db = _doc.Database;
            _ed = _doc.Editor;
            Errors = new List<Error>();
        }

        ///// <summary>
        ///// Сгруппированные ошибки по одинаковым сообщениям.
        ///// </summary>
        ///// <returns></returns>
        //public static List<Error> GetCollapsedErrors()
        //{
        //   var errCounts = Errors.GroupBy(e => e.Message).Select(g=>
        //   {
        //      var e = g.First().GetCopy();
        //      e.SetCount(g.Count());
        //      return e;
        //   });
        //   return errCounts.ToList();
        //}

        public static void AddError(Error err)
        {            
            Errors.Add(err);
        }

        public static void AddError(string msg, Icon icon = null)
        {
            var err = new Error(msg, icon);
            Errors.Add(err);
        }

        public static void AddError(string msg)
        {
            var err = new Error(msg);
            Errors.Add(err);
        }

        public static void AddError(string msg, params object[] args)
        {
            var err = new Error(string.Format(msg, args));
            Errors.Add(err);
        }


        public static void AddError(string msg, Entity ent, Icon icon = null)
        {
            var err = new Error(msg, ent, icon);
            Errors.Add(err);
        }
        public static void AddError(string msg, Entity ent)
        {
            var err = new Error(msg, ent);
            Errors.Add(err);
        }
        public static void AddError(string msg, Entity ent, Matrix3d trans, Icon icon = null)
        {
            var err = new Error(msg, ent, trans, icon);
            Errors.Add(err);
        }

        public static void AddError(string msg, Entity ent, Extents3d ext, Icon icon = null)
        {
            var err = new Error(msg, ext, ent, icon);
            Errors.Add(err);
        }
        public static void AddError(string msg, Entity ent, Extents3d ext)
        {
            var err = new Error(msg, ext, ent);
            Errors.Add(err);
        }
        public static void AddError(string msg, Extents3d ext, ObjectId idEnt, Icon icon = null)
        {
            var err = new Error(msg, ext, idEnt, icon);
            Errors.Add(err);
        }

        public static void AddError(string msg, Extents3d ext, Matrix3d trans, Icon icon = null)
        {
            var err = new Error(msg, ext, trans, icon);
            Errors.Add(err);
        }
        public static void AddError(string msg, Extents3d ext, ObjectId idEnt)
        {
            var err = new Error(msg, ext, idEnt);
            Errors.Add(err);
        }
        public static void AddError(string msg, ObjectId idEnt, Icon icon = null)
        {
            var err = new Error(msg, idEnt, icon);
            Errors.Add(err);
        }
        public static void AddError(string msg, ObjectId idEnt)
        {
            var err = new Error(msg, idEnt);
            Errors.Add(err);
        }
        public static void AddError(string msg, ObjectId idEnt, Matrix3d trans, Icon icon = null)
        {
            var err = new Error(msg, idEnt, trans, icon);
            Errors.Add(err);
        }

        public static void Show()
        {
            if (HasErrors)
            {
                Logger.Log.Error(string.Join("\n", Errors.Select(e => e.Message)));
                Errors = SortErrors(Errors);

                // WinForms
                Application.ShowModelessDialog(new FormError(false));

                //// WPF
                //var errVM = new ErrorsViewModel(Errors);
                //errVM.IsDialog = false;
                //var errView = new ErrorsView(errVM);
                //Application.ShowModelessWindow(errView);
            }
        }

        private static List<Error> SortErrors(List<Error> errors)
        {
            var comparer = Comparers.AlphanumComparator.New;
            return errors.OrderBy(o=>o.Message, comparer).ToList();
        }

        /// <summary>
        /// При прерывании вызывает исключение "Отменено пользователем.".
        /// Т.е. можно не обрабатывает DialogResult.
        /// </summary>      
        public static System.Windows.Forms.DialogResult ShowDialog()
        {
            if (HasErrors)
            {
                Logger.Log.Error(string.Join("\n", Errors.Select(e => e.Message)));
                Errors = SortErrors(Errors);

                // WinForms
                var formErr = new FormError(true);
                var res = Application.ShowModalDialog(formErr);
                if (res != System.Windows.Forms.DialogResult.OK)
                {
                    formErr.EnableDialog(false);
                    //Application.ShowModelessDialog(formErr);
                    throw new Exception("Отменено пользователем.");
                }
                return res;

                //// WPF
                //var errVM = new ErrorsViewModel(Errors);
                //errVM.IsDialog = true;
                //var errView = new ErrorsView(errVM);
                //if (Application.ShowModalWindow(errView) == true)
                //{
                //    return System.Windows.Forms.DialogResult.OK;
                //}
                //else
                //{                
                //    throw new CancelByUserException();
                //}                
            }
            else
            {
                return System.Windows.Forms.DialogResult.OK;
            }
        }

        public static void LogErrors()
        {
            Logger.Log.Error(string.Join("\n", Errors.Select(e => e.Message)));
            Errors.Sort();
        }
    }
}