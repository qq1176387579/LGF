﻿/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/6/29 21:34:56
/// 功能描述:  
****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace UnityEngine
{
    //
    // 摘要:
    //     A 2D Rectangle defined by X and Y position, width and height.
    public struct Rect : IEquatable<Rect>, IFormattable
    {
        private float m_XMin;

        private float m_YMin;

        private float m_Width;

        private float m_Height;

        //
        // 摘要:
        //     Shorthand for writing new Rect(0,0,0,0).
        public static Rect zero => new Rect(0f, 0f, 0f, 0f);

        //
        // 摘要:
        //     The X coordinate of the rectangle.
        public float x
        {
            get
            {
                return m_XMin;
            }
            set
            {
                m_XMin = value;
            }
        }

        //
        // 摘要:
        //     The Y coordinate of the rectangle.
        public float y
        {
            get
            {
                return m_YMin;
            }
            set
            {
                m_YMin = value;
            }
        }

        //
        // 摘要:
        //     The X and Y position of the rectangle.
        public Vector2 position
        {
            get
            {
                return new Vector2(m_XMin, m_YMin);
            }
            set
            {
                m_XMin = value.x;
                m_YMin = value.y;
            }
        }

        //
        // 摘要:
        //     The position of the center of the rectangle.
        public Vector2 center
        {
            get
            {
                return new Vector2(x + m_Width / 2f, y + m_Height / 2f);
            }
            set
            {
                m_XMin = value.x - m_Width / 2f;
                m_YMin = value.y - m_Height / 2f;
            }
        }

        //
        // 摘要:
        //     The position of the minimum corner of the rectangle.
        public Vector2 min
        {
            get
            {
                return new Vector2(xMin, yMin);
            }
            set
            {
                xMin = value.x;
                yMin = value.y;
            }
        }

        //
        // 摘要:
        //     The position of the maximum corner of the rectangle.
        public Vector2 max
        {
            get
            {
                return new Vector2(xMax, yMax);
            }
            set
            {
                xMax = value.x;
                yMax = value.y;
            }
        }

        //
        // 摘要:
        //     The width of the rectangle, measured from the X position.
        public float width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
            }
        }

        //
        // 摘要:
        //     The height of the rectangle, measured from the Y position.
        public float height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
            }
        }

        //
        // 摘要:
        //     The width and height of the rectangle.
        public Vector2 size
        {
            get
            {
                return new Vector2(m_Width, m_Height);
            }
            set
            {
                m_Width = value.x;
                m_Height = value.y;
            }
        }

        //
        // 摘要:
        //     The minimum X coordinate of the rectangle.
        public float xMin
        {
            get
            {
                return m_XMin;
            }
            set
            {
                float xMax = this.xMax;
                m_XMin = value;
                m_Width = xMax - m_XMin;
            }
        }

        //
        // 摘要:
        //     The minimum Y coordinate of the rectangle.
        public float yMin
        {
            get
            {
                return m_YMin;
            }
            set
            {
                float yMax = this.yMax;
                m_YMin = value;
                m_Height = yMax - m_YMin;
            }
        }

        //
        // 摘要:
        //     The maximum X coordinate of the rectangle.
        public float xMax
        {
            get
            {
                return m_Width + m_XMin;
            }
            set
            {
                m_Width = value - m_XMin;
            }
        }

        //
        // 摘要:
        //     The maximum Y coordinate of the rectangle.
        public float yMax
        {
            get
            {
                return m_Height + m_YMin;
            }
            set
            {
                m_Height = value - m_YMin;
            }
        }

        [Obsolete("use xMin")]
        public float left => m_XMin;

        [Obsolete("use xMax")]
        public float right => m_XMin + m_Width;

        [Obsolete("use yMin")]
        public float top => m_YMin;

        [Obsolete("use yMax")]
        public float bottom => m_YMin + m_Height;

        //
        // 摘要:
        //     Creates a new rectangle.
        //
        // 参数:
        //   x:
        //     The X value the rect is measured from.
        //
        //   y:
        //     The Y value the rect is measured from.
        //
        //   width:
        //     The width of the rectangle.
        //
        //   height:
        //     The height of the rectangle.
        public Rect(float x, float y, float width, float height)
        {
            m_XMin = x;
            m_YMin = y;
            m_Width = width;
            m_Height = height;
        }

        //
        // 摘要:
        //     Creates a rectangle given a size and position.
        //
        // 参数:
        //   position:
        //     The position of the minimum corner of the rect.
        //
        //   size:
        //     The width and height of the rect.
        public Rect(Vector2 position, Vector2 size)
        {
            m_XMin = position.x;
            m_YMin = position.y;
            m_Width = size.x;
            m_Height = size.y;
        }

        //
        // 参数:
        //   source:
        public Rect(Rect source)
        {
            m_XMin = source.m_XMin;
            m_YMin = source.m_YMin;
            m_Width = source.m_Width;
            m_Height = source.m_Height;
        }

        //
        // 摘要:
        //     Creates a rectangle from min/max coordinate values.
        //
        // 参数:
        //   xmin:
        //     The minimum X coordinate.
        //
        //   ymin:
        //     The minimum Y coordinate.
        //
        //   xmax:
        //     The maximum X coordinate.
        //
        //   ymax:
        //     The maximum Y coordinate.
        //
        // 返回结果:
        //     A rectangle matching the specified coordinates.
        public static Rect MinMaxRect(float xmin, float ymin, float xmax, float ymax)
        {
            return new Rect(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        //
        // 摘要:
        //     Set components of an existing Rect.
        //
        // 参数:
        //   x:
        //
        //   y:
        //
        //   width:
        //
        //   height:
        public void Set(float x, float y, float width, float height)
        {
            m_XMin = x;
            m_YMin = y;
            m_Width = width;
            m_Height = height;
        }

        //
        // 摘要:
        //     Returns true if the x and y components of point is a point inside this rectangle.
        //     If allowInverse is present and true, the width and height of the Rect are allowed
        //     to take negative values (ie, the min value is greater than the max), and the
        //     test will still work.
        //
        // 参数:
        //   point:
        //     Point to test.
        //
        //   allowInverse:
        //     Does the test allow the Rect's width and height to be negative?
        //
        // 返回结果:
        //     True if the point lies within the specified rectangle.
        public bool Contains(Vector2 point)
        {
            return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
        }

        //
        // 摘要:
        //     Returns true if the x and y components of point is a point inside this rectangle.
        //     If allowInverse is present and true, the width and height of the Rect are allowed
        //     to take negative values (ie, the min value is greater than the max), and the
        //     test will still work.
        //
        // 参数:
        //   point:
        //     Point to test.
        //
        //   allowInverse:
        //     Does the test allow the Rect's width and height to be negative?
        //
        // 返回结果:
        //     True if the point lies within the specified rectangle.
        public bool Contains(Vector3 point)
        {
            return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
        }

        //
        // 摘要:
        //     Returns true if the x and y components of point is a point inside this rectangle.
        //     If allowInverse is present and true, the width and height of the Rect are allowed
        //     to take negative values (ie, the min value is greater than the max), and the
        //     test will still work.
        //
        // 参数:
        //   point:
        //     Point to test.
        //
        //   allowInverse:
        //     Does the test allow the Rect's width and height to be negative?
        //
        // 返回结果:
        //     True if the point lies within the specified rectangle.
        public bool Contains(Vector3 point, bool allowInverse)
        {
            if (!allowInverse)
            {
                return Contains(point);
            }

            bool flag = (width < 0f && point.x <= xMin && point.x > xMax) || (width >= 0f && point.x >= xMin && point.x < xMax);
            bool flag2 = (height < 0f && point.y <= yMin && point.y > yMax) || (height >= 0f && point.y >= yMin && point.y < yMax);
            return flag && flag2;
        }

        private static Rect OrderMinMax(Rect rect)
        {
            if (rect.xMin > rect.xMax)
            {
                float xMin = rect.xMin;
                rect.xMin = rect.xMax;
                rect.xMax = xMin;
            }

            if (rect.yMin > rect.yMax)
            {
                float yMin = rect.yMin;
                rect.yMin = rect.yMax;
                rect.yMax = yMin;
            }

            return rect;
        }

        //
        // 摘要:
        //     Returns true if the other rectangle overlaps this one. If allowInverse is present
        //     and true, the widths and heights of the Rects are allowed to take negative values
        //     (ie, the min value is greater than the max), and the test will still work.
        //
        // 参数:
        //   other:
        //     Other rectangle to test overlapping with.
        //
        //   allowInverse:
        //     Does the test allow the widths and heights of the Rects to be negative?
        public bool Overlaps(Rect other)
        {
            return other.xMax > xMin && other.xMin < xMax && other.yMax > yMin && other.yMin < yMax;
        }

        //
        // 摘要:
        //     Returns true if the other rectangle overlaps this one. If allowInverse is present
        //     and true, the widths and heights of the Rects are allowed to take negative values
        //     (ie, the min value is greater than the max), and the test will still work.
        //
        // 参数:
        //   other:
        //     Other rectangle to test overlapping with.
        //
        //   allowInverse:
        //     Does the test allow the widths and heights of the Rects to be negative?
        public bool Overlaps(Rect other, bool allowInverse)
        {
            Rect rect = this;
            if (allowInverse)
            {
                rect = OrderMinMax(rect);
                other = OrderMinMax(other);
            }

            return rect.Overlaps(other);
        }

        //
        // 摘要:
        //     Returns a point inside a rectangle, given normalized coordinates.
        //
        // 参数:
        //   rectangle:
        //     Rectangle to get a point inside.
        //
        //   normalizedRectCoordinates:
        //     Normalized coordinates to get a point for.
        public static Vector2 NormalizedToPoint(Rect rectangle, Vector2 normalizedRectCoordinates)
        {
            return new Vector2(Mathf.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), Mathf.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
        }

        //
        // 摘要:
        //     Returns the normalized coordinates cooresponding the the point.
        //
        // 参数:
        //   rectangle:
        //     Rectangle to get normalized coordinates inside.
        //
        //   point:
        //     A point inside the rectangle to get normalized coordinates for.
        public static Vector2 PointToNormalized(Rect rectangle, Vector2 point)
        {
            return new Vector2(Mathf.InverseLerp(rectangle.x, rectangle.xMax, point.x), Mathf.InverseLerp(rectangle.y, rectangle.yMax, point.y));
        }

        public static bool operator !=(Rect lhs, Rect rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(Rect lhs, Rect rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (width.GetHashCode() << 2) ^ (y.GetHashCode() >> 2) ^ (height.GetHashCode() >> 1);
        }

        public override bool Equals(object other)
        {
            if (!(other is Rect))
            {
                return false;
            }

            return Equals((Rect)other);
        }

        public bool Equals(Rect other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && width.Equals(other.width) && height.Equals(other.height);
        }

        //
        // 摘要:
        //     Returns a formatted string for this Rect.
        //
        // 参数:
        //   format:
        //     A numeric format string.
        //
        //   formatProvider:
        //     An object that specifies culture-specific formatting.
        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        }

        //
        // 摘要:
        //     Returns a formatted string for this Rect.
        //
        // 参数:
        //   format:
        //     A numeric format string.
        //
        //   formatProvider:
        //     An object that specifies culture-specific formatting.
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.InvariantCulture.NumberFormat);
        }

        //
        // 摘要:
        //     Returns a formatted string for this Rect.
        //
        // 参数:
        //   format:
        //     A numeric format string.
        //
        //   formatProvider:
        //     An object that specifies culture-specific formatting.
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "F2";
            }

            return String.Format("(x:{0}, y:{1}, width:{2}, height:{3})", x.ToString(format, formatProvider), y.ToString(format, formatProvider), width.ToString(format, formatProvider), height.ToString(format, formatProvider));
        }
    }
}