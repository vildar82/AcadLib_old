//   Rectangle.java
//   Java Spatial Index Library
//   Copyright (C) 2002 Infomatiq Limited
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307 USA

// Ported to C# By Dror Gluska, April 9th, 2009

using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using System;
using System.Text;

namespace AcadLib.RTree.SpatialIndex
{
    /**
     * Currently hardcoded to 2 dimensions, but could be extended.
     *
     * @author  aled@sourceforge.net
     * @version 1.0b2p1
     */

    [PublicAPI]
    public class Rectangle
    {
        /**
         * Number of dimensions in a rectangle. In theory this
         * could be exended to three or more dimensions.
         */
        public double[] _max;
        public double[] _min;
        internal const int DIMENSIONS = 3;

        /**
         * array containing the minimum value for each dimension; ie { min(x), min(y) }
         */
        /**
         * array containing the maximum value for each dimension; ie { max(x), max(y) }
         */
        /**
         * Constructor.
         *
         * @param x1 coordinate of any corner of the rectangle
         * @param y1 (see x1)
         * @param x2 coordinate of the opposite corner
         * @param y2 (see x2)
         */

        public Rectangle(double x1, double y1, double x2, double y2, double z1, double z2)
        {
            _min = new double[DIMENSIONS];
            _max = new double[DIMENSIONS];
            set(x1, y1, x2, y2, z1, z2);
        }

        /**
         * Constructor.
         *
         * @param min array containing the minimum value for each dimension; ie { min(x), min(y) }
         * @param max array containing the maximum value for each dimension; ie { max(x), max(y) }
         */

        public Rectangle([NotNull] double[] min, [NotNull] double[] max)
        {
            if (min.Length != DIMENSIONS || max.Length != DIMENSIONS)
            {
                throw new Exception("Error in Rectangle constructor: " +
                          "min and max arrays must be of length " + DIMENSIONS);
            }
            _min = new double[DIMENSIONS];
            _max = new double[DIMENSIONS];
            set(min, max);
        }

        public Rectangle(Extents3d extents) : this(extents.MinPoint.X, extents.MinPoint.Y,
            extents.MaxPoint.X, extents.MaxPoint.Y, 0, 0)
        {
        }

        public Rectangle(Extents2d extents) : this(extents.MinPoint.X, extents.MinPoint.Y,
            extents.MaxPoint.X, extents.MaxPoint.Y, 0, 0)
        {
        }

        /**
          * Sets the size of the rectangle.
          *
          * @param x1 coordinate of any corner of the rectangle
          * @param y1 (see x1)
          * @param x2 coordinate of the opposite corner
          * @param y2 (see x2)
          */

#pragma warning disable 659

        public override bool Equals(object obj)
#pragma warning restore 659
        {
            var equals = false;
            if (obj?.GetType() == typeof(Rectangle))
            {
                var r = (Rectangle)obj;
                //#warning possible didn't convert properly
                if (CompareArrays(r._min, _min) && CompareArrays(r._max, _max))
                {
                    equals = true;
                }
            }
            return equals;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            // min coordinates
            sb.Append('(');
            for (var i = 0; i < DIMENSIONS; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(_min[i]);
            }
            sb.Append("), (");

            // max coordinates
            for (var i = 0; i < DIMENSIONS; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(_max[i]);
            }
            sb.Append(')');
            return sb.ToString();
        }

        internal void add(Rectangle r)
        {
            for (var i = 0; i < DIMENSIONS; i++)
            {
                if (r._min[i] < _min[i])
                {
                    _min[i] = r._min[i];
                }
                if (r._max[i] > _max[i])
                {
                    _max[i] = r._max[i];
                }
            }
        }

        internal double area()
        {
            return (_max[0] - _min[0]) * (_max[1] - _min[1]);
        }

        private static bool CompareArrays([CanBeNull] double[] a1, [CanBeNull] double[] a2)
        {
            if (a1 == null || a2 == null)
                return false;
            if (a1.Length != a2.Length)
                return false;

            for (var i = 0; i < a1.Length; i++)
                if (Math.Abs(a1[i] - a2[i]) > 0.0001)
                    return false;
            return true;
        }

        internal bool containedBy(Rectangle r)
        {
            for (var i = 0; i < DIMENSIONS; i++)
            {
                if (_max[i] > r._max[i] || _min[i] < r._min[i])
                {
                    return false;
                }
            }
            return true;
        }

        internal bool contains(Rectangle r)
        {
            for (var i = 0; i < DIMENSIONS; i++)
            {
                if (_max[i] < r._max[i] || _min[i] > r._min[i])
                {
                    return false;
                }
            }
            return true;
        }

        [NotNull]
        internal Rectangle copy()
        {
            return new Rectangle(_min, _max);
        }

        internal double distance(Point p)
        {
            double distanceSquared = 0;
            for (var i = 0; i < DIMENSIONS; i++)
            {
                var greatestMin = Math.Max(_min[i], p.coordinates[i]);
                var leastMax = Math.Min(_max[i], p.coordinates[i]);
                if (greatestMin > leastMax)
                {
                    distanceSquared += (greatestMin - leastMax) * (greatestMin - leastMax);
                }
            }
            return Math.Sqrt(distanceSquared);
        }

        internal double distance(Rectangle r)
        {
            double distanceSquared = 0;
            for (var i = 0; i < DIMENSIONS; i++)
            {
                var greatestMin = Math.Max(_min[i], r._min[i]);
                var leastMax = Math.Min(_max[i], r._max[i]);
                if (greatestMin > leastMax)
                {
                    distanceSquared += (greatestMin - leastMax) * (greatestMin - leastMax);
                }
            }
            return Math.Sqrt(distanceSquared);
        }

        internal double distanceSquared(int dimension, double point)
        {
            double distanceSquared = 0;
            var tempDistance = point - _max[dimension];
            for (var i = 0; i < 2; i++)
            {
                if (tempDistance > 0)
                {
                    distanceSquared = tempDistance * tempDistance;
                    break;
                }
                tempDistance = _min[dimension] - point;
            }
            return distanceSquared;
        }

        internal bool edgeOverlaps(Rectangle r)
        {
            for (var i = 0; i < DIMENSIONS; i++)
            {
                if (Math.Abs(_min[i] - r._min[i]) < 0.0001 || Math.Abs(_max[i] - r._max[i]) < 0.0001)
                {
                    return true;
                }
            }
            return false;
        }

        internal double enlargement([NotNull] Rectangle r)
        {
            var enlargedArea = (Math.Max(_max[0], r._max[0]) - Math.Min(_min[0], r._min[0])) *
                                 (Math.Max(_max[1], r._max[1]) - Math.Min(_min[1], r._min[1]));

            return enlargedArea - area();
        }

        internal double furthestDistance(Rectangle r)
        {
            double distanceSquared = 0;

            for (var i = 0; i < DIMENSIONS; i++)
            {
                distanceSquared += Math.Max(r._min[i], r._max[i]);
                //#warning possible didn't convert properly
                //distanceSquared += Math.Max(distanceSquared(i, r.min[i]), distanceSquared(i, r.max[i]));
            }

            return Math.Sqrt(distanceSquared);
        }

        internal bool intersects(Rectangle r)
        {
            // Every dimension must intersect. If any dimension
            // does not intersect, return false immediately.
            for (var i = 0; i < DIMENSIONS; i++)
            {
                if (_max[i] < r._min[i] || _min[i] > r._max[i])
                {
                    return false;
                }
            }
            return true;
        }

        internal bool sameObject(object o)
        {
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            return base.Equals(o);
        }

        internal void set(double x1, double y1, double x2, double y2, double z1, double z2)
        {
            _min[0] = Math.Min(x1, x2);
            _min[1] = Math.Min(y1, y2);
            _min[2] = Math.Min(z1, z2);
            _max[0] = Math.Max(x1, x2);
            _max[1] = Math.Max(y1, y2);
            _max[2] = Math.Max(z1, z2);
        }

        /**
         * Sets the size of the rectangle.
         *
         * @param min array containing the minimum value for each dimension; ie { min(x), min(y) }
         * @param max array containing the maximum value for each dimension; ie { max(x), max(y) }
         */

        internal void set([NotNull] double[] min, [NotNull] double[] max)
        {
            Array.Copy(min, 0, _min, 0, DIMENSIONS);
            Array.Copy(max, 0, _max, 0, DIMENSIONS);
        }

        /**
         * Make a copy of this rectangle
         *
         * @return copy of this rectangle
         */
        /**
         * Determine whether an edge of this rectangle overlies the equivalent
         * edge of the passed rectangle
         */
        /**
         * Determine whether this rectangle intersects the passed rectangle
         *
         * @param r The rectangle that might intersect this rectangle
         *
         * @return true if the rectangles intersect, false if they do not intersect
         */
        /**
         * Determine whether this rectangle contains the passed rectangle
         *
         * @param r The rectangle that might be contained by this rectangle
         *
         * @return true if this rectangle contains the passed rectangle, false if
         *         it does not
         */
        /**
         * Determine whether this rectangle is contained by the passed rectangle
         *
         * @param r The rectangle that might contain this rectangle
         *
         * @return true if the passed rectangle contains this rectangle, false if
         *         it does not
         */
        /**
         * Return the distance between this rectangle and the passed point.
         * If the rectangle contains the point, the distance is zero.
         *
         * @param p Point to find the distance to
         *
         * @return distance beween this rectangle and the passed point.
         */
        /**
         * Return the distance between this rectangle and the passed rectangle.
         * If the rectangles overlap, the distance is zero.
         *
         * @param r Rectangle to find the distance to
         *
         * @return distance between this rectangle and the passed rectangle
         */
        /**
         * Return the squared distance from this rectangle to the passed point
         */
        /**
         * Return the furthst possible distance between this rectangle and
         * the passed rectangle.
         *
         * Find the distance between this rectangle and each corner of the
         * passed rectangle, and use the maximum.
         *
         */
        /**
         * Calculate the area by which this rectangle would be enlarged if
         * added to the passed rectangle. Neither rectangle is altered.
         *
         * @param r Rectangle to union with this rectangle, in order to
         *          compute the difference in area of the union and the
         *          original rectangle
         */
        /**
         * Compute the area of this rectangle.
         *
         * @return The area of this rectangle
         */
        /**
         * Computes the union of this rectangle and the passed rectangle, storing
         * the result in this rectangle.
         *
         * @param r Rectangle to add to this rectangle
         */
        /**
         * Find the the union of this rectangle and the passed rectangle.
         * Neither rectangle is altered
         *
         * @param r The rectangle to union with this rectangle
         */

        [NotNull]
        internal Rectangle union(Rectangle r)
        {
            var union = copy();
            union.add(r);
            return union;
        }

        /**
         * Determine whether this rectangle is equal to a given object.
         * Equality is determined by the bounds of the rectangle.
         *
         * @param o The object to compare with this rectangle
         */
        /**
         * Determine whether this rectangle is the same as another object
         *
         * Note that two rectangles can be equal but not the same object,
         * if they both have the same bounds.
         *
         * @param o The object to compare with this rectangle.
         */
        /**
         * Return a string representation of this rectangle, in the form:
         * (1.2, 3.4), (5.6, 7.8)
         *
         * @return String String representation of this rectangle.
         */
    }
}