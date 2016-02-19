using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

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

      public Matrix3d Trans { get; set; }
      public string Message { get { return _msg; } }
      public string ShortMsg { get { return _shortMsg; } }
      public ObjectId IdEnt { get { return _idEnt; } }      
      public bool HasEntity { get { return _hasEntity; } }      
      public Icon Icon { get; set; }
      public Extents3d Extents {
         get {
            if (!_alreadyCalcExtents)
            {
               _alreadyCalcExtents = true;
               using (var ent = _idEnt.Open( OpenMode.ForRead, false, true) as Entity)
               {
                  if (ent != null)
                  {
                     try
                     {
                        _extents = ent.GeometricExtents;
                        if (Trans != Matrix3d.Identity)
                        {
                           _extents.TransformBy(Trans);
                        }
                     }
                     catch
                     {
                        _isNullExtents = true;
                     }
                  }
               }               
            }
            if (_isNullExtents)
            {
               Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Границы объекта не определены.");
            }
            return _extents;
         }
      }

      private Error (Error err)
      {
         this._msg = err._msg;
         this._shortMsg = err._shortMsg;
         this._idEnt = err._idEnt;
         this._alreadyCalcExtents = err._alreadyCalcExtents;
         this._isNullExtents = err._isNullExtents;
         this._extents = err._extents;
         this._hasEntity = err._hasEntity;
         this.Icon = err.Icon;
      }

      public Error(string message, Icon icon = null)
      {
         _msg = message;
         _shortMsg = getShortMsg(_msg);
         _hasEntity = false;
         Icon = icon;
      }

      internal void SetCount(int v)
      {
         _shortMsg = $"{v}...{_shortMsg}";
      }

      public Error(string message, Entity ent, Icon icon = null)
      {
         _msg = message;
         _shortMsg = getShortMsg(_msg);
         _idEnt = ent.Id;         
         _hasEntity = true;
         Icon = icon;
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
      }

      public Error(string message, ObjectId idEnt, Icon icon = null)
      {
         _msg = message;
         _shortMsg = getShortMsg(_msg);
         _idEnt = idEnt;
         //_extents = ent.GeometricExtents;
         _hasEntity = true;
         Icon = icon;
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
         return Message.CompareTo(other.Message);
      }

      public bool Equals(Error other)
      {
         return Message.Equals(other.Message);
      }

      public override int GetHashCode()
      {
         return _msg.GetHashCode();
      }

      internal Error GetCopy()
      {
         Error errCopy = new Error(this);
         return errCopy;
      }

      public void AdditionToMessage(string addMsg)
      {
         _msg += addMsg;
      }
   }
}
