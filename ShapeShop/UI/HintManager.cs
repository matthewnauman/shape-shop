using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShopData.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeShop.UI
{
    public class HintManager
    {
        private static Random random = new Random();

        private ContentSolution solution;
        private List<ContentSolutionShape> solutionShapeList;
        private int currentSolutionShapeIdx = 0;
        private float alpha = 0;
        private float showHintTimer = 0;
        private const float TIMER_MAX = 1.5f;

        private ContentSolutionShape CurrentSolutionShape
        {
            get { return solutionShapeList[currentSolutionShapeIdx]; }
        }

        public HintManager()
        {
        }

        // need to call this on every puzzle change!
        public void Initialize(ContentSolution cs)
        {
            currentSolutionShapeIdx = 0;
            alpha = 0;
            showHintTimer = 0;

            this.solution = cs;

            int totalShapeCount = solution.SolutionShapes.Count;
            int capacity = 0;

            if (totalShapeCount % 2 != 0)
            {
                capacity = (totalShapeCount - 1) / 2;
            }
            else
            {
                capacity = totalShapeCount / 2;
            }

            solutionShapeList = new List<ContentSolutionShape>(capacity);
            List<int> choiceList = new List<int>(capacity);

            while (choiceList.Count < capacity)
            {
                int choice = random.Next(0, solution.SolutionShapes.Count - 1);
                if (!choiceList.Contains<int>(choice))
                {
                    choiceList.Add(choice);
                }
            }

            foreach (int i in choiceList)
            {
                ContentSolutionShape css = solution.SolutionShapes[i];
                solutionShapeList.Add(css);                
            }

        }

        public void Update(GameTime gameTime)
        {
            if (PuzzleEngine.MainScreen.PuzzlePanel.PanelState == PuzzlePanel.PuzzlePanelState.Hint)
            {
                switch (PuzzleEngine.MainScreen.PuzzlePanel.HintState)
                {
                    case PuzzlePanel.PuzzleHintState.HintFinished:
                        foreach (Shape s in PuzzleEngine.CurrentPuzzleSet.ShapesDict.Values)
                        {
                            if (s.State != Shape.ShapeState.Waiting)
                            {
                                s.State = Shape.ShapeState.Dropped;
                            }
                        }
                        PuzzleEngine.MainScreen.PuzzlePanel.PanelState = PuzzlePanel.PuzzlePanelState.Running;
                        break;
                    case PuzzlePanel.PuzzleHintState.StartHint:
                        foreach (Shape s in PuzzleEngine.CurrentPuzzleSet.ShapesDict.Values)
                        {
                            if (s.State != Shape.ShapeState.Waiting)
                            {
                                s.State = Shape.ShapeState.TransitionOutForHint;
                            }
                        }
                        PuzzleEngine.MainScreen.PuzzlePanel.HintState = PuzzlePanel.PuzzleHintState.FadeInHint;
                        break;
                    case PuzzlePanel.PuzzleHintState.FadeInHint:
                        if (alpha < 1)
                        {
                            alpha += Shape.AutoFadeDelta_Default;
                        }
                        if (alpha >= 1)
                        {
                            alpha = 1;
                            if (PuzzleEngine.CurrentPuzzleSet.AreAllShapesWaitingOnHint)
                            {
                                PuzzleEngine.MainScreen.PuzzlePanel.HintState = PuzzlePanel.PuzzleHintState.ShowHint;
                            }
                        }
                        break;
                    case PuzzlePanel.PuzzleHintState.ShowHint:
                        showHintTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (showHintTimer >= TIMER_MAX)
                        {
                            showHintTimer = 0;
                            foreach (Shape s in PuzzleEngine.CurrentPuzzleSet.ShapesDict.Values)
                            {
                                if (s.State != Shape.ShapeState.Waiting)
                                {
                                    s.State = Shape.ShapeState.TransitionInAfterHint;
                                }
                            }
                            PuzzleEngine.MainScreen.PuzzlePanel.HintState = PuzzlePanel.PuzzleHintState.FadeOutHint;
                        }

                        break;
                    case PuzzlePanel.PuzzleHintState.FadeOutHint:
                        if (alpha > 0)
                        {
                            alpha -= Shape.AutoFadeDelta_Default;
                        }
                        if (alpha <= 0)
                        {
                            alpha = 0;
                            if (PuzzleEngine.CurrentPuzzleSet.AreAllShapesBackAfterHint)
                            {
                                PuzzleEngine.MainScreen.PuzzlePanel.HintState = PuzzlePanel.PuzzleHintState.HintFinished;
                                nextHint();
                            }
                        }
                        break;
                }
            }
        }

        private void nextHint()
        {
            currentSolutionShapeIdx++;
            if (currentSolutionShapeIdx >= solutionShapeList.Count)
            {
                currentSolutionShapeIdx = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // no idea why i have to add 6 px to the half-tile offset??
            Vector2 offset = new Vector2(-PuzzleEngine.CurrentPuzzle.TileSize.X / 2 + 6, -PuzzleEngine.CurrentPuzzle.TileSize.Y / 2 + 6);

            // account for gridshift in puzzle
            if (PuzzleEngine.CurrentPuzzle.IsGridShiftX && PuzzleEngine.CurrentPuzzle.IsGridShiftY)
            {
                offset += new Vector2(PuzzleEngine.CurrentPuzzle.TileSize.X / 2, PuzzleEngine.CurrentPuzzle.TileSize.Y / 2);
            }
            else if (PuzzleEngine.CurrentPuzzle.IsGridShiftX)
            {
                offset.X += (PuzzleEngine.CurrentPuzzle.TileSize.X / 2);
            }
            else if (PuzzleEngine.CurrentPuzzle.IsGridShiftY)
            {
                offset.Y += (PuzzleEngine.CurrentPuzzle.TileSize.Y / 2);
            }

            PuzzleEngine.CurrentPuzzleSet.ShapesDict[CurrentSolutionShape.ShapeKey].DrawForHint(spriteBatch, PuzzleEngine.CurrentPuzzle.GridOrigin, CurrentSolutionShape, alpha);            
        }

    }
}
