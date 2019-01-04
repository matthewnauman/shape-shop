using System.Collections.Generic;

namespace ShapeShopData.Models
{
    public class ShapeSnapAgeComparer : IComparer<Shape>
    {
        // sort by isSnapped and then timeIdle
        public int Compare(Shape s1, Shape s2)
        {
            int retValue = 1;

            if (s1 != null && s2 != null)
            {
                retValue = s2.IsSnapped.CompareTo(s1.IsSnapped);
                if (retValue == 0)
                {
                    return s2.TimeIdle.CompareTo(s1.TimeIdle);
                }
                return retValue;
            }

            return 0;
        }

    }
}