using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.Errors
{
    [PublicAPI]
    public static class Inspector
    {
        private static Database _db;
        private static Document _doc;
        private static Editor _ed;
        private static bool needClear;
        public static List<IError> Errors { get; private set; }

        public static bool HasErrors => Errors.Count > 0;

        static Inspector()
        {
#pragma warning disable 618
            Clear();
#pragma warning restore 618
        }

        [Obsolete("После Show очищаются ошибки.")]
        public static void Clear()
        {
            Errors = new List<IError>();
            _doc = Application.DocumentManager.MdiActiveDocument;
            if (_doc != null)
            {
                _db = _doc.Database;
                _ed = _doc.Editor;
            }
        }

        private static void AddErrorInternal(IError err)
        {
            if (needClear)
            {
#pragma warning disable 618
                Clear();
#pragma warning restore 618
                needClear = false;
            }
            Errors.Add(err);
        }

        public static void AddError(IError err)
        {
            AddErrorInternal(err);
        }

        public static void AddError(Error err)
        {
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string msg, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, icon);
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string group, string msg, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, icon) { Group = group };
            AddErrorInternal(err);
        }

        public static void AddError(string msg)
        {
            var err = new Error(msg, SystemIcons.Error);
            AddErrorInternal(err);
        }

        public static void AddError(string group, string msg)
        {
            var err = new Error(msg, SystemIcons.Error);
            AddErrorInternal(err);
        }

        [Obsolete("Используй интерполяцию строк $\"\"")]
        public static void AddError([NotNull] string msg, [NotNull] params object[] args)
        {
            var err = new Error(string.Format(msg, args));
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string msg, [NotNull] Entity ent, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ent, icon);
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string group, string msg, [NotNull] Entity ent, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ent, icon) { Group = group };
            AddErrorInternal(err);
        }

        public static void AddError(string msg, [NotNull] Entity ent)
        {
            var err = new Error(msg, ent);
            AddErrorInternal(err);
        }

        public static void AddError(string group, string msg, [NotNull] Entity ent)
        {
            var err = new Error(msg, ent) { Group = group };
            AddErrorInternal(err);
        }

        public static void AddError(string msg, [NotNull] Entity ent, Matrix3d trans, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ent, trans, icon);
            AddErrorInternal(err);
        }

        public static void AddError(string group, string msg, [NotNull] Entity ent, Matrix3d trans, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ent, trans, icon) { Group = group };
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string msg, [NotNull] Entity ent, Extents3d ext, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ext, ent, icon);
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string group, string msg, [NotNull] Entity ent, Extents3d ext, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ext, ent, icon) { Group = group };
            AddErrorInternal(err);
        }

        public static void AddError(string msg, [NotNull] Entity ent, Extents3d ext)
        {
            var err = new Error(msg, ext, ent);
            AddErrorInternal(err);
        }

        public static void AddError(string group, string msg, [NotNull] Entity ent, Extents3d ext)
        {
            var err = new Error(msg, ext, ent) { Group = group };
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string msg, Extents3d ext, ObjectId idEnt, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ext, idEnt, icon);
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string group, string msg, Extents3d ext, ObjectId idEnt, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ext, idEnt, icon) { Group = group };
            AddErrorInternal(err);
        }

        public static void AddError(string msg, Extents3d ext, Matrix3d trans, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ext, trans, icon);
            AddErrorInternal(err);
        }

        public static void AddError(string group, string msg, Extents3d ext, Matrix3d trans, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, ext, trans, icon) { Group = group };
            AddErrorInternal(err);
        }

        public static void AddError(string msg, Extents3d ext, ObjectId idEnt)
        {
            var err = new Error(msg, ext, idEnt);
            AddErrorInternal(err);
        }

        public static void AddError(string group, string msg, Extents3d ext, ObjectId idEnt)
        {
            var err = new Error(msg, ext, idEnt) { Group = group };
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string msg, ObjectId idEnt, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, idEnt, icon);
            AddErrorInternal(err);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void AddError(string group, string msg, ObjectId idEnt, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, idEnt, icon) { Group = group };
            AddErrorInternal(err);
        }

        public static void AddError(string msg, ObjectId idEnt)
        {
            var err = new Error(msg, idEnt);
            AddErrorInternal(err);
        }

        public static void AddError(string group, string msg, ObjectId idEnt)
        {
            var err = new Error(msg, idEnt) { Group = group };
            AddErrorInternal(err);
        }

        public static void AddError(string msg, ObjectId idEnt, Matrix3d trans, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, idEnt, trans, icon);
            AddErrorInternal(err);
        }

        public static void AddError(string group, string msg, ObjectId idEnt, Matrix3d trans, [CanBeNull] Icon icon = null)
        {
            var err = new Error(msg, idEnt, trans, icon) { Group = group };
            AddErrorInternal(err);
        }

        public static void Show()
        {
            if (HasErrors)
            {
                Logger.Log.Error($"Окно ошибок: {string.Join("\n", Errors.Select(e => e.Group + e.Message))}");
                Errors = SortErrors(Errors);
                // WPF
                Show(Errors);
            }
        }

        public static void Show([NotNull] List<IError> errors)
        {
            var errVM = new ErrorsViewModel(errors) { IsDialog = false };
            var errView = new ErrorsView(errVM);
            errView.Show();
            needClear = true;
        }

        [NotNull]
        private static List<IError> SortErrors([NotNull] List<IError> errors)
        {
            var comparer = NetLib.Comparers.AlphanumComparator.New;
            return errors.OrderBy(o => o.Message, comparer).ToList();
        }

        /// <summary>
        /// При прерывании вызывает исключение "Отменено пользователем.".
        /// Т.е. можно не обрабатывает DialogResult.
        /// </summary>
        public static System.Windows.Forms.DialogResult ShowDialog()
        {
            if (HasErrors)
            {
                needClear = true;
                Logger.Log.Error(string.Join("\n", Errors.Select(e => e.Message)));
                Errors = SortErrors(Errors);
                // WPF
                if (ShowDialog(Errors) == true)
                {
                    return System.Windows.Forms.DialogResult.OK;
                }
                throw new OperationCanceledException();
            }
            return System.Windows.Forms.DialogResult.OK;
        }

        public static bool? ShowDialog([NotNull] List<IError> errors)
        {
            var errVM = new ErrorsViewModel(errors) { IsDialog = true };
            var errView = new ErrorsView(errVM);
            var res = errView.ShowDialog();
            needClear = true;
            return res;
        }
    }
}