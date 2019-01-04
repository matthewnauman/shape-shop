using Microsoft.Xna.Framework;
using System;

namespace ShapeShopData.Util
{
    public class Circle
    {
        private float radius;
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        private Vector2 center;
        public Vector2 Center
        {
            get { return center; }
            set { center = value; }
        }

        public Circle(float radius, Vector2 center)
        {
            this.radius = radius;
            this.center = center;
        }

        public bool IsPositionWithin(Vector2 position)
        {
            int check = Convert.ToInt32(Math.Sqrt(Math.Pow(position.X - center.X, 2) + Math.Pow(position.Y - center.Y, 2)));
            if (check < radius) return true;
            return false;
        }

    }
}
