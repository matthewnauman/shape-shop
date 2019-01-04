using System.Collections.Generic;

namespace ShapeShopData.Models
{
    public class ShapeAgeComparer : IComparer<Shape>
    {
        // sort by timeIdle, (oldest move to head)
        public int Compare(Shape s1, Shape s2)
        {
            if (s1 != null && s2 != null)
            {
                return s2.TimeIdle.CompareTo(s1.TimeIdle);
            }
            return 0;
        }

    }
}