using Microsoft.Xna.Framework;
using System;

namespace ShapeShopData.Models
{
    [Serializable]
    public class ContentSolutionShape : ICloneable
    {
        private Point originPoint;
        public Point OriginPoint
        {
            get { return originPoint; }
            set { originPoint = value; }
        }

        private int shapeKey;
        public int ShapeKey
        {
            get { return shapeKey; }
            set { shapeKey = value; }
        }

        private bool isFlippedHorizontal = false;
        public bool IsFlippedHorizontal
        {
            get { return isFlippedHorizontal; }
            set { isFlippedHorizontal = value; }
        }

        private bool isFlippedVertical = false;
        public bool IsFlippedVertical
        {
            get { return isFlippedVertical; }
            set { isFlippedVertical = value; }
        }

        private int rotationIdx;
        public int RotationIdx
        {
            get { return rotationIdx; }
            set { rotationIdx = value; }
        }

        private ContentSolutionShape() { }

        public ContentSolutionShape(int shapeKey, Point originPoint, bool isFlippedHorizontal, bool isFlippedVertical, int rotationIdx)
        {
            this.shapeKey = shapeKey;
            this.originPoint = originPoint;
            this.isFlippedHorizontal = isFlippedHorizontal;
            this.isFlippedVertical = isFlippedVertical;
            this.rotationIdx = rotationIdx;
        }
    
        public void DrawTheShape()
        {

        }

        public override string ToString()
        {
            return "  AssetName: " + "" +
                   "  ShapeKey: " + shapeKey +
                   "  IsFlippedHorizontal: " + isFlippedHorizontal +
                   "  IsFlippedVertical: " + IsFlippedVertical +
                   "  PuzzlePoint: " + originPoint +
                   "  RotationIdx: " + rotationIdx;
        }

        public object Clone()
        {
            ContentSolutionShape ss = new ContentSolutionShape();
//            ss.AssetName = AssetName;
            ss.IsFlippedHorizontal = IsFlippedHorizontal;
            ss.IsFlippedVertical = IsFlippedVertical;
            ss.OriginPoint = OriginPoint;
            ss.RotationIdx = RotationIdx;
            ss.ShapeKey = ShapeKey;
            return ss;
        }

        /*
        /// Read a Map object from the content pipeline.
        public class ContentSolutionShapeReader : ContentTypeReader<ContentSolutionShape>
        {
            protected override ContentSolutionShape Read(ContentReader input, ContentSolutionShape existingInstance)
            {
                ContentSolutionShape solutionShape = existingInstance;
                if (solutionShape == null)
                {
                    solutionShape = new ContentSolutionShape();
                }

                solutionShape.AssetName = input.AssetName;
                solutionShape.OriginPoint = input.ReadObject<Point>();
                solutionShape.ShapeKey = input.ReadInt32();
                solutionShape.IsFlippedHorizontal = input.ReadBoolean();
                solutionShape.IsFlippedVertical = input.ReadBoolean();
                solutionShape.RotationIdx = input.ReadInt32();

                return solutionShape;
            }
        }
        */
    }
}