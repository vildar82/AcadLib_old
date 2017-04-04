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
    public class Error : IError
    {
        private Dictionary<Icon, ErrorStatus> dictErrorIcons = new Dictionary<Icon, ErrorStatus>() {
            { SystemIcons.Error, ErrorStatus.Error },
            { SystemIcons.Exclamation, ErrorStatus.Exclamation },
            { SystemIcons.Hand, ErrorStatus.Exclamation },
            { SystemIcons.Information, ErrorStatus.Info },
            { SystemIcons.Warning, ErrorStatus.Error }
        };
        protected string _msg;
        protected string _shortMsg;
        protected ObjectId _idEnt;
        protected Extents3d _extents;
        protected bool _alreadyCalcExtents;
        protected bool _isNullExtents;
        protected bool _hasEntity;

        public object Tag { get; set; }
        public Matrix3d Trans { get; set; }       
        public string Message { get { return _msg; } }
        public string ShortMsg { get { return _shortMsg; } }
        public ObjectId IdEnt { get { return _idEnt; } }
        public bool HasEntity { get { return _hasEntity; } }
        public Icon Icon { get; set; }
        public ErrorStatus Status { get; set; }
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
        public bool CanShow { get; set; }

        public virtual void Show()
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

        public Error()
        {

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
            this.CanShow = err.CanShow;
            this.Status = err.Status;            
        }               

        public Error(string message, Icon icon = null)
        {
            _msg = PrepareMessage(message);
            _shortMsg = GetShortMsg(_msg);
            _hasEntity = false;
            Icon = icon ?? SystemIcons.Error;
            Trans = Matrix3d.Identity;
            DefineStatus();
        }               

        public Error(string message, Entity ent, Icon icon = null)
        {
            _msg = PrepareMessage(message);
            _shortMsg = GetShortMsg(_msg);
            _idEnt = ent.Id;
            _hasEntity = true;
            Icon = icon ?? SystemIcons.Error;
            Trans = Matrix3d.Identity;
            CanShow = true;
            DefineStatus();
        }

        public Error(string message, Entity ent, Matrix3d trans, Icon icon = null)
        {
            _msg = PrepareMessage(message);
            _shortMsg = GetShortMsg(_msg);
            _idEnt = ent.Id;
            _hasEntity = true;
            Icon = icon ?? SystemIcons.Error;
            Trans = trans;
            CanShow = true;
            DefineStatus();
        }

        public Error(string message, Extents3d ext, Entity ent, Icon icon = null)
        {
            _msg = PrepareMessage(message);
            _shortMsg = GetShortMsg(_msg);
            _idEnt = ent.Id;
            _extents = ext;
            _alreadyCalcExtents = true;
            _hasEntity = true;
            Icon = icon ?? SystemIcons.Error;
            Trans = Matrix3d.Identity;
            CanShow = true;
            DefineStatus();
        }

        public Error(string message, Extents3d ext, ObjectId idEnt, Icon icon = null)
        {
            _msg = PrepareMessage(message);
            _shortMsg = GetShortMsg(_msg);
            _idEnt = idEnt;
            _extents = ext;
            _alreadyCalcExtents = true;
            _hasEntity = true;
            Icon = icon ?? SystemIcons.Error;
            Trans = Matrix3d.Identity;
            CanShow = true;
            DefineStatus();
        }

        public Error(string message, Extents3d ext, Matrix3d trans, Icon icon = null)
        {
            _msg = PrepareMessage(message);
            _shortMsg = GetShortMsg(_msg);            
            _extents = ext;
            _alreadyCalcExtents = true;
            _hasEntity = false;
            Icon = icon ?? SystemIcons.Error;
            Trans = trans;
            CanShow = true;
            DefineStatus();
        }

        public Error(string message, ObjectId idEnt, Icon icon = null)
        {
            _msg = PrepareMessage(message);
            _shortMsg = GetShortMsg(_msg);
            _idEnt = idEnt;
            _hasEntity = true;
            Icon = icon ?? SystemIcons.Error;
            Trans = Matrix3d.Identity;
            CanShow = true;
            DefineStatus();
        }

        public Error(string message, ObjectId idEnt, Matrix3d trans, Icon icon = null)
        {
            _msg = PrepareMessage(message);
            _shortMsg = GetShortMsg(_msg);
            _idEnt = idEnt;
            _hasEntity = true;
            Icon = icon ?? SystemIcons.Error;
            Trans = trans;
            CanShow = true;
            DefineStatus();
        }

        protected string PrepareMessage(string message)
        {
            return message;//.ClearString(); // делать очистку в момент создания ошибки при необходимости
        }

        protected string GetShortMsg(string msg)
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

        public int CompareTo(IError other)
        {
            var res = Status.CompareTo(other.Status);
            if (res != 0) return res;
            return string.Compare(Message,other.Message);
        }

        public bool Equals(IError other)
        {
            return string.Equals(Message,other.Message);
        }

        public override int GetHashCode()
        {
           return Message.GetHashCode();
        }

        public IError GetCopy()
        {
            //var errCopy = new Error(this);
            //return errCopy;
            return (IError)MemberwiseClone();
        }

        public void AdditionToMessage(string addMsg)
        {
            _msg += addMsg;
            _shortMsg = GetShortMsg(_msg);
        }

        /// <summary>
        /// Сгруппированные ошибки по одинаковым сообщениям.
        /// </summary>
        /// <returns></returns>
        public static List<IError> GetCollapsedErrors(List<IError> errors)
        {
            var errCounts = errors.GroupBy(e => e.Message).Select(g =>
            {
                var e = g.First().GetCopy();
                var err = e as Error;
                err?.SetCount(g.Count());
                return e;
            });
            return errCounts.ToList();
        }
        internal void SetCount(int v)
        {
            if (v > 1)
            {
                _shortMsg = $"{v}...{_shortMsg}";
            }
        }

        protected void DefineStatus()
        {
            if (Icon == null) Icon = SystemIcons.Error;
            dictErrorIcons.TryGetValue(Icon, out ErrorStatus status);
            Status = status;
        }
    }
}
