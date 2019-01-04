using System;
using System.Collections.Generic;

namespace ShapeShopData.Models
{
    [Serializable]
    public class PlayerSolution
    {
        public int PuzzleKey;
        public List<PlayerSolutionShape> SolutionShapes;
    }
}