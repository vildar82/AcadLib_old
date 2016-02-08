using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Errors
{
   public class Error : IComparable<Error>, IEquatable<Error>
   {
      private string _msg;
      private string _shortMsg;
      private ObjectId _idEnt;
      private Extents3d _extents;
      private bool _hasEntity;      

      public string Message { get { return _msg; } }
      public string ShortMsg { get { return _shortMsg; } }
      public ObjectId IdEnt { get { return _idEnt; } }
      public Extents3d Extents { get { return _extents; } }
      public bool HasEntity { get { return _hasEntity; } }      
      public Icon Icon { get; set; }

      public Error(string message, Icon icon = null)
      {
         _msg = message;
         _shortMsg = getShortMsg(_msg);
         _hasEntity = false;
         Icon = icon;
      }     

      public Error(string message, Entity ent, Icon icon = null) : this(message, ent.GeometricExtents, ent, icon)
      {         
      }

      public Error(string message, Extents3d ext, Entity ent, Icon icon = null)
      {         
         _msg = message;
         _shortMsg = getShortMsg(_msg);
         _idEnt = ent.Id;
         _extents = ext;         
         _hasEntity = true;
         Icon = icon;
      }

      public Error(string message, Extents3d ext, ObjectId idEnt, Icon icon = null)
      {
         _msg = message;
         _shortMsg = getShortMsg(_msg);
         _idEnt = idEnt;
         _extents = ext;
         _hasEntity = true;
         Icon = icon;
      }

      public Error(string message,ObjectId idEnt, Icon icon = null)
      {
         using (var ent = idEnt.Open( OpenMode.ForRead) as Entity)
         {
            _msg = message;
            _shortMsg = getShortMsg(_msg);
            _idEnt = idEnt;
            _extents = ent.GeometricExtents;
            _hasEntity = true;
            Icon = icon;
         }             
      }

      private string getShortMsg(string msg)
      {
         if (msg.Length > 200)
         {
            return msg.Substring(0, 200);
         }
         else
         {
            return msg;
         }
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
   }
}
