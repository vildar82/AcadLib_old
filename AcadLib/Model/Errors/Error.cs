using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using System.Windows;

namespace AcadLib.Errors
{
    public class Error : IComparable<Error>, IEquatable<Error>
    {
        private string _msg;
        private string _shortMsg;
        private ObjectId _idEnt;
        private Extents3d _extents;
        private bool _alreadyCalcExtents;
        private bool _isNullExtents;
        private bool _hasEntity;

        public object Tag { get; set; }
        public Matrix3d Trans { get; set; }       

        public string Message { get { return _msg; } }
        public string ShortMsg { get { return _shortMsg; } }
        public ObjectId IdEnt { get { return _idEnt; } }
        public bool HasEntity { get { return _hasEntity; } }
        public Icon Icon { get; set; }
        public Extents3d Extents
        {
            get
            {
                if (!_alreadyCalcExtents)
                {
                    _alreadyCalcExtents = true;
                    if (_hasEntity)
                    {
                        using (var ent = _idEnt.Open(OpenMode.ForRead, false, true) as Entity)
                        {
                            if (ent != null)
                            {
                                try
                                {
                                    _extents = ent.GeometricExtents;
                                    _extents.TransformBy(Trans);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log.Error(ex, "AcadLib.Error.Extents ent.GeometricExtents;");
                                    _isNullExtents = true;
                                }
                            }
                        }
                    }
                }
                if (_isNullExtents)
                {
                    Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Границы объекта не определены.");
                }
                return _extents;
            }
        }

        public void Show()
        {
            try
            {
                var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;

                if (Extents.Diagonal() > 1) ed.Zoom(Extents);

                if (HasEntity)
                {
                    // Проверка соответствия документа
                    if (IdEnt.Database == doc.Database)
                        IdEnt.FlickObjectHighlight(2, 60, 60);
                    else
                        MessageBox.Show($"Должен быть активен чертеж {IdEnt.Database.Filename}");
                }
            }
            catch { }
        }

        private Error(Error err)
        {            
            this._msg = err._msg;
            this._shortMsg = err._shortMsg;
            this._idEnt = err._idEnt;
            this._alreadyCalcExtents = err._alreadyCalcExtents;
            this._isNullExtents = err._isNullExtents;
            this._extents = err._extents;
            this._hasEntity = err._hasEntity;
            this.Icon = err.Icon;
            this.Trans = err.Trans;
            this.Tag = err.Tag;
        }

        public Error(string message, Icon icon = null)
        {
            _msg = message;
            _shortMsg = getShortMsg(_msg);
            _hasEntity = false;
            Icon = icon;
            Trans = Matrix3d.Identity;
        }

        internal void SetCount(int v)
        {
            if (v > 1)
            {
                _shortMsg = $"{v}...{_shortMsg}";
            }
        }

        public Error(string message, Entity ent, Icon icon = null)
        {
            _msg = message;
            _shortMsg = getShortMsg(_msg);
            _idEnt = ent.Id;
            _hasEntity = true;
            Icon = icon;
            Trans = Matrix3d.Identity;
        }

        public Error(string message, Entity ent, Matrix3d trans, Icon icon = null)
        {
            _msg = message;
            _shortMsg = getShortMsg(_msg);
            _idEnt = ent.Id;
            _hasEntity = true;
            Icon = icon;
            Trans = trans;
        }

        public Error(string message, Extents3d ext, Entity ent, Icon icon = null)
        {
            _msg = message;
            _shortMsg = getShortMsg(_msg);
            _idEnt = ent.Id;
            _extents = ext;
            _alreadyCalcExtents = true;
            _hasEntity = true;
            Icon = icon;
            Trans = Matrix3d.Identity;
        }

        public Error(string message, Extents3d ext, ObjectId idEnt, Icon icon = null)
        {
            _msg = message;
            _shortMsg = getShortMsg(_msg);
            _idEnt = idEnt;
            _extents = ext;
            _alreadyCalcExtents = true;
            _hasEntity = true;
            Icon = icon;
            Trans = Matrix3d.Identity;
        }

        public Error(string message, Extents3d ext, Matrix3d trans, Icon icon = null)
        {
            _msg = message;
            _shortMsg = getShortMsg(_msg);            
            _extents = ext;
            _alreadyCalcExtents = true;
            _hasEntity = false;
            Icon = icon;
            Trans = trans;
        }

        public Error(string message, ObjectId idEnt, Icon icon = null)
        {
            _msg = message;
            _shortMsg = getShortMsg(_msg);
            _idEnt = idEnt;
            _hasEntity = true;
            Icon = icon;
            Trans = Matrix3d.Identity;
        }

        public Error(string message, ObjectId idEnt, Matrix3d trans, Icon icon = null)
        {
            _msg = message;
            _shortMsg = getShortMsg(_msg);
            _idEnt = idEnt;
            _hasEntity = true;
            Icon = icon;
            Trans = trans;
        }

        private string getShortMsg(string msg)
        {
            string resVal = string.Empty;
            if (msg.Length > 200)
            {
                resVal = msg.Substring(0, 200);
            }
            else
            {
                resVal = msg;
            }
            return resVal.Replace("\n", " ");
        }

        public int CompareTo(Error other)
        {
            return string.Compare(Message,other.Message);
        }

        public bool Equals(Error other)
        {
            return string.Equals(Message,other.Message);
        }

        public override int GetHashCode()
        {
           return Message.GetHashCode();
        }

        public Error GetCopy()
        {
            Error errCopy = new Error(this);
            return errCopy;
        }

        public void AdditionToMessage(string addMsg)
        {
            _msg += addMsg;
            _shortMsg = getShortMsg(_msg);
        }

        /// <summary>
        /// Сгруппированные ошибки по одинаковым сообщениям.
        /// </summary>
        /// <returns></returns>
        public static List<Error> GetCollapsedErrors(List<Error> errors)
        {
            var errCounts = errors.GroupBy(e => e.Message).Select(g =>
            {
                var e = g.First().GetCopy();
                e.SetCount(g.Count());
                return e;
            });
            return errCounts.ToList();
        }
    }
}
