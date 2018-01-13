using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace AcadLib.Blocks.Dublicate
{
    /// <summary>
    /// Данные о вхождении блока
    /// </summary>
    [PublicAPI]
    public class BlockRefDublicateInfo : IEqualityComparer<BlockRefDublicateInfo>, IEquatable<BlockRefDublicateInfo>
    {
        public const double pi2 = 2 * Math.PI;
        public static double toleranceRotateNear360 = pi2 - CheckDublicateBlocks.Tolerance.EqualVector;
        public int CountDublic { get; set; }
        public List<BlockRefDublicateInfo> Dublicates { get; set; }
        public ObjectId IdBlRef { get; set; }
        public string Name { get; set; }
        public Point3d Position { get; set; }
        public double Rotation { get; set; }
        public Matrix3d Transform { get; set; }
        public Matrix3d TransformToModel { get; set; }

        //public Extents3d? Bounds { get; set; }
        public BlockRefDublicateInfo([NotNull] BlockReference blRef, Matrix3d transToModel, double rotateToModel)
        {
            IdBlRef = blRef.Id;
            Transform = blRef.BlockTransform;
            TransformToModel = transToModel;
            Position = blRef.Position.TransformBy(TransformToModel);
#pragma warning disable 618
            Name = blRef.GetEffectiveName();
#pragma warning restore 618
            Rotation = getRotateToModel(blRef.Rotation, rotateToModel);
            //Bounds = blRef.Bounds;
        }

        public bool Equals(BlockRefDublicateInfo other)
        {
            if (other == null) return false;
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
            if (obj is BlockRefDublicateInfo info)
            {
                return Equals(info);
            }
            return false;
        }

        public bool Equals(BlockRefDublicateInfo x, BlockRefDublicateInfo y)
        {
            if (ReferenceEquals(x, y)) return true;
            return x != null && x.Equals(y);
        }

        public int GetHashCode(BlockRefDublicateInfo obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Name.GetHashCode();
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
    }
}