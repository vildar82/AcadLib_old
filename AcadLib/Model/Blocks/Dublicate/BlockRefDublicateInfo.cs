using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.Blocks.Dublicate
{
    /// <summary>
    /// Данные о вхождении блока 
    /// </summary>
    public class BlockRefDublicateInfo : IEqualityComparer<BlockRefDublicateInfo>, IEquatable<BlockRefDublicateInfo>
    {
        public const double pi2 = 2 * Math.PI;
        public static double toleranceRotateNear360 = pi2 - CheckDublicateBlocks.Tolerance.EqualVector;
        public string Name { get; set; }
        public ObjectId IdBlRef { get; set; }
        public Point3d Position { get; set; }
        public double Rotation { get; set; }
        public Matrix3d TransformToModel { get; set; }
        public Matrix3d Transform { get; set; }
        public int CountDublic { get; set; }
        //public Extents3d? Bounds { get; set; }

        public List<BlockRefDublicateInfo> Dublicates { get; set; }

        public BlockRefDublicateInfo(BlockReference blRef, Matrix3d transToModel, double rotateToModel)
        {
            IdBlRef = blRef.Id;
            Transform = blRef.BlockTransform;
            TransformToModel = transToModel;
            Position = blRef.Position.TransformBy(TransformToModel);
            Name = blRef.GetEffectiveName();
            Rotation = getRotateToModel(blRef.Rotation, rotateToModel);            
            //Bounds = blRef.Bounds;            
        }

        private double getRotateToModel(double rotation, double rotateToModel)
        {
            var res = rotation + rotateToModel;
            if (res > pi2)
            {
                res -= pi2;
            }
            return res;
        }

        public bool Equals(BlockRefDublicateInfo other)
        {
            var rotDiff = Math.Abs(Rotation - other.Rotation);
            return Name.Equals(other.Name) &&
                   Position.IsEqualTo(other.Position, CheckDublicateBlocks.Tolerance) &&
                   (
                       rotDiff < CheckDublicateBlocks.Tolerance.EqualVector ||
                       rotDiff > toleranceRotateNear360
                   );
                   //(
                   //    (Bounds.HasValue && other.Bounds.HasValue)
                   //    && Bounds.Value.IsEqualTo(other.Bounds.Value, CheckDublicateBlocks.Tolerance)
                   //);
            //TransformToModel.Equals (other.TransformToModel);
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
            return Name.GetHashCode();
        }

        //public BlockRefDublicateInfo TransCopy()
        //{
        //   BlockRefDublicateInfo resVal = (BlockRefDublicateInfo)this.MemberwiseClone();
        //   Position = Position.TransformBy(TransformToModel);
        //   return resVal;
        //}      
    }
}
