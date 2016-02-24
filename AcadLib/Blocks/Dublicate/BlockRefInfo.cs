using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Blocks
{
   /// <summary>
   /// Данные о вхождении блока 
   /// </summary>
   public class BlockRefInfo : IEqualityComparer<BlockRefInfo>, IEquatable<BlockRefInfo>
   {
      public static Tolerance Tolerance { get; set; } = new Tolerance(0.02, 10);

      public string Name { get; set; }
      public ObjectId IdBlRef { get; set; }
      public Point3d Position { get; set; }
      public double Rotation { get; set; }
      public Matrix3d TransformToModel { get; set; }

      public BlockRefInfo(BlockReference blRef, Matrix3d transToModel, string blName = null)
      {
         IdBlRef = blRef.Id;
         Position = blRef.Position.TransformBy(transToModel);
         if (string.IsNullOrEmpty(blName))
         {
            blName = blRef.GetEffectiveName();
         }
         Name = blName;
         Rotation = blRef.Rotation;
         TransformToModel = transToModel;
      }

      public bool Equals(BlockRefInfo other)
      {
         return Name.Equals(other.Name) &&
                Position.IsEqualTo(other.Position, Tolerance) &&
                Math.Abs(Rotation - other.Rotation) < Tolerance.EqualPoint &&
                TransformToModel.Equals (other.TransformToModel);
      }

      public override bool Equals(object obj)
      {
         if (obj is BlockRefInfo)
         {
            return Equals((BlockRefInfo)obj);
         }
         else
         {
            return false;
         }
      }      

      public bool Equals(BlockRefInfo x, BlockRefInfo y)
      {
         return x.Equals(y);
      }

      public int GetHashCode(BlockRefInfo obj)
      {
         return obj.GetHashCode();
      }

      public override int GetHashCode()
      {
         int hCode = Name.GetHashCode();// ^ Position.GetHashCode() ^ Rotation.GetHashCode();
         return hCode.GetHashCode();
      }
   }
}
