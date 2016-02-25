using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Blocks.Dublicate
{
   /// <summary>
   /// Данные о вхождении блока 
   /// </summary>
   public class BlockRefDublicateInfo : IEqualityComparer<BlockRefDublicateInfo>, IEquatable<BlockRefDublicateInfo>
   {
      public string Name { get; set; }
      public ObjectId IdBlRef { get; set; }
      public Point3d Position { get; set; }
      public double Rotation { get; set; }
      public Matrix3d TransformToModel { get; set; }
      public Matrix3d Transform { get; set; }
      public int CountDublic { get; set; }

      public BlockRefDublicateInfo(BlockReference blRef)
      {
         IdBlRef = blRef.Id;
         Position = blRef.Position;
         Name = blRef.Name;
         Rotation = blRef.Rotation;
         Transform = blRef.BlockTransform;         
      }

      public bool Equals(BlockRefDublicateInfo other)
      {
         return Name.Equals(other.Name) &&
                Position.IsEqualTo(other.Position, CheckDublicateBlocks.Tolerance) &&
                Math.Abs(Rotation - other.Rotation) < CheckDublicateBlocks.Tolerance.EqualPoint &&
                TransformToModel.Equals (other.TransformToModel);
      }

      public override bool Equals(object obj)
      {
         if (obj is BlockRefDublicateInfo)
         {
            return Equals((BlockRefDublicateInfo)obj);
         }
         else
         {
            return false;
         }
      }      

      public bool Equals(BlockRefDublicateInfo x, BlockRefDublicateInfo y)
      {
         return x.Equals(y);
      }

      public int GetHashCode(BlockRefDublicateInfo obj)
      {
         return obj.GetHashCode();
      }

      public override int GetHashCode()
      {
         int hCode = Name.GetHashCode();// ^ Position.GetHashCode() ^ Rotation.GetHashCode();
         return hCode.GetHashCode();
      }

      public BlockRefDublicateInfo TransCopy(Matrix3d transtoModel)
      {
         BlockRefDublicateInfo resVal = (BlockRefDublicateInfo)this.MemberwiseClone();
         resVal.TransformToModel = transtoModel;
         resVal.TransformByModel();
         return resVal;
      }

      private void TransformByModel()
      {
         Position = Position.TransformBy(TransformToModel);
      }
   }
}
