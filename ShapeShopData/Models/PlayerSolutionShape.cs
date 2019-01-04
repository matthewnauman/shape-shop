using Microsoft.Xna.Framework;
using System;

namespace ShapeShopData.Models
{
    [Serializable]
    public struct PlayerSolutionShape
    {
        public Point OriginPoint;
        public int ShapeKey;
        public bool IsFlippedHorizontal;
        public bool IsFlippedVertical;
        public int RotationIdx;
    }
}