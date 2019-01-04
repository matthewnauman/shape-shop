using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace ShapeShopData.Models
{
    [Serializable]
    public class ContentSolution : ICloneable
    {
        private int puzzleKey;
        [ContentSerializerIgnore]
        public int PuzzleKey
        {
            get { return puzzleKey; }
            set { puzzleKey = value; }
        }

        private List<ContentSolutionShape> solutionShapes = new List<ContentSolutionShape>();
        public List<ContentSolutionShape> SolutionShapes
        {
            get { return solutionShapes; }
            set { solutionShapes = value; }
        }

        private ContentSolution() { }

        public ContentSolution(int puzzleKey) 
        {
            this.puzzleKey = puzzleKey;
        }

        public override string ToString()
        {
            string retString = " AssetName: " + "" +
                               " PuzzleKey: " + PuzzleKey;

            foreach (ContentSolutionShape pss in SolutionShapes)
            {
                retString += '\n'+pss.ToString();
            }

            return retString;
        }

        public object Clone()
        {
            ContentSolution cs = new ContentSolution();
//            cs.AssetName = AssetName;
            cs.PuzzleKey = PuzzleKey;
            cs.SolutionShapes.AddRange(SolutionShapes);

            return cs;
        }

        /*
        /// Read a Map object from the content pipeline.
        public class ContentSolutionReader : ContentTypeReader<ContentSolution>
        {
            protected override ContentSolution Read(ContentReader input, ContentSolution existingInstance)
            {
                ContentSolution puzzleSolution = existingInstance;
                if (puzzleSolution == null)
                {
                    puzzleSolution = new ContentSolution();
                }

                puzzleSolution.AssetName = input.AssetName;
                puzzleSolution.SolutionShapes.AddRange(input.ReadObject<List<ContentSolutionShape>>());

                return puzzleSolution;
            }
        }
        */
    }
}