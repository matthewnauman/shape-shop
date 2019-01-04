using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShopData.Statistics;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShapeShopData.Models
{
    /// One section of the world, and all of the data in it.
    public class PuzzleSet : ContentObject, ICloneable
    {
        public static readonly Point BackgroundSize = new Point(684, 492);

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// The content name of the texture that contains the tiles for this map.
        private string tileTextureName;
        public string TileTextureName
        {
            get { return tileTextureName; }
            set { tileTextureName = value; }
        }

        /// The texture that contains the tiles for this map.
        private Texture2D tileTexture;
        [ContentSerializerIgnore]
        public Texture2D TileTexture
        {
            get { return tileTexture; }
        }

        private int unlockedAtOnce;
        public int UnlockedAtOnce
        {
            get { return unlockedAtOnce; }
            set { unlockedAtOnce = value; }
        }

        private int key;
        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        private readonly List<string> puzzleContentNames = new List<string>();
        public List<string> PuzzleContentNames
        {
            get { return puzzleContentNames; }
        }

        private List<Puzzle> puzzles = new List<Puzzle>();
        [ContentSerializerIgnore]
        public List<Puzzle> Puzzles
        {
            get { return puzzles; }
            set { puzzles = value; }
        }

        private List<string> shapeContentNames = new List<string>();
        public List<string> ShapeContentNames
        {
            get { return shapeContentNames; }
            set { shapeContentNames = value; }
        }

        private Dictionary<int, Shape> shapesDict = new Dictionary<int, Shape>();
        [ContentSerializerIgnore]
        public Dictionary<int, Shape> ShapesDict
        {
            get { return shapesDict; }
            set { shapesDict = value; }
        }

        private PuzzleSetStatistics statistics;
        [ContentSerializer(Optional = true)]
        public PuzzleSetStatistics Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }

        private int shapePickerSelectedIdx = 0;
        [ContentSerializerIgnore]
        public int ShapePickerSelectedIdx
        {
            get { return shapePickerSelectedIdx; }
            set { shapePickerSelectedIdx = value; }
        }

        private bool isCleared = false;
        [ContentSerializerIgnore]
        public bool IsCleared
        {
            get { return isCleared; }
            set { isCleared = value; }
        }

        private int finalPuzzleKey = -1;
        [ContentSerializerIgnore]
        public int FinalPuzzleKey
        {
            get { return finalPuzzleKey; }
        }

        private Texture2D bg1Tex, fg1Tex, bg2Tex, fg2Tex, bg3Tex, fg3Tex, bg4Tex, fg4Tex, bg5Tex, fg5Tex, bg6Tex, fg6Tex,
                          bg7Tex, fg7Tex, bg8Tex, fg8Tex, bg9Tex, fg9Tex, bg10Tex, fg10Tex, bg11Tex, fg11Tex, bg12Tex, fg12Tex;
        
        public bool AreAllInvalidShapesWaiting
        {
            get
            {
                foreach (Shape shape in ShapesDict.Values)
                {
                    if (!shape.IsValid)
                    {
                        if (shape.State != Shape.ShapeState.Waiting)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public bool AreAllShapesWaiting
        {
            get
            {
                foreach (Shape shape in ShapesDict.Values)
                {
                    if (shape.State != Shape.ShapeState.Waiting)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool AreAnyShapesDropped
        {
            get
            {
                foreach (Shape shape in ShapesDict.Values)
                {
                    if (shape.State == Shape.ShapeState.Dropped)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool AreAllShapesWaitingOnHint
        {
            get
            {
                foreach (Shape shape in ShapesDict.Values)
                {
                    if (shape.State != Shape.ShapeState.Waiting)
                    {
                        if (shape.State != Shape.ShapeState.WaitOnHint)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public bool AreAllShapesBackAfterHint
        {
            get
            {
                foreach (Shape shape in ShapesDict.Values)
                {
                    if (shape.State == Shape.ShapeState.TransitionInAfterHint)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void LoadContent(ContentManager content)
        {
            ///////////////////////////////////////////////////
            /// READER Content
            tileTexture = content.Load<Texture2D>(Path.Combine(@"Textures\GameScreens\Puzzle", tileTextureName));

            // add puzzles to puzzleSet
            int counter = 1;
            foreach (string puzzleName in puzzleContentNames)
            {
                Puzzle puzzle = content.Load<Puzzle>(Path.Combine("Puzzles", puzzleName)).Clone() as Puzzle;
                puzzle.LoadContent(content);
                puzzle.Place = counter;
                puzzles.Add(puzzle);
                counter++;
            }

            // set the key for the final puzzle of the set
            finalPuzzleKey = puzzles[puzzles.Count - 1].Key;

            if (unlockedAtOnce > puzzles.Count)
            {
                unlockedAtOnce = puzzles.Count;
            }

            // load shapes and set base positions
            foreach (string shapeContent in shapeContentNames)
            {
                Shape tmpShape = content.Load<Shape>(System.IO.Path.Combine(@"Shapes", shapeContent)).Clone() as Shape;
                tmpShape.LoadContent(content);
                tmpShape.Reset();
                shapesDict.Add(tmpShape.Key, tmpShape);
            }
            ///////////////////////////////////////////////////

            bg1Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg1");
            fg1Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg1");
            bg2Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg2");
            fg2Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg2");
            bg3Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg3");
            fg3Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg3");
            bg4Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg4");
            fg4Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg4");
            bg5Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg5");
            fg5Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg5");
            bg6Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg6");
            fg6Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg6");
            bg7Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg7");
            fg7Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg7");
            bg8Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg8");
            fg8Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg8");
            bg9Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg9");
            fg9Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg9");
            bg10Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg10");
            fg10Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg10");
            bg11Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg11");
            fg11Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg11");
            bg12Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\bg12");
            fg12Tex = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\bgfg\fg12");
        }

        public Texture2D GetBackgroundTex(String key)
        {
            if (key.Equals("bg1"))
            {
                return bg1Tex;
            }
            else if (key.Equals("bg2"))
            {
                return bg2Tex;
            }
            else if (key.Equals("bg3"))
            {
                return bg3Tex;
            }
            else if (key.Equals("bg4"))
            {
                return bg4Tex;
            }
            else if (key.Equals("bg5"))
            {
                return bg5Tex;
            }
            else if (key.Equals("bg6"))
            {
                return bg6Tex;
            }
            else if (key.Equals("bg7"))
            {
                return bg7Tex;
            }
            else if (key.Equals("bg8"))
            {
                return bg8Tex;
            }
            else if (key.Equals("bg9"))
            {
                return bg9Tex;
            }
            else if (key.Equals("bg10"))
            {
                return bg10Tex;
            }
            else if (key.Equals("bg11"))
            {
                return bg11Tex;
            }
            else if (key.Equals("bg12"))
            {
                return bg12Tex;
            }

            return null;
        }

        public Texture2D GetForegroundTex(String key)
        {
            if (key.Equals("fg1"))
            {
                return fg1Tex;
            }
            else if (key.Equals("fg2"))
            {
                return fg2Tex;
            }
            else if (key.Equals("fg3"))
            {
                return fg3Tex;
            }
            else if (key.Equals("fg4"))
            {
                return fg4Tex;
            }
            else if (key.Equals("fg5"))
            {
                return fg5Tex;
            }
            else if (key.Equals("fg6"))
            {
                return fg6Tex;
            }
            else if (key.Equals("fg7"))
            {
                return fg7Tex;
            }
            else if (key.Equals("fg8"))
            {
                return fg8Tex;
            }
            else if (key.Equals("fg9"))
            {
                return fg9Tex;
            }
            else if (key.Equals("fg10"))
            {
                return fg10Tex;
            }
            else if (key.Equals("fg11"))
            {
                return fg11Tex;
            }
            else if (key.Equals("fg12"))
            {
                return fg12Tex;
            }

            return null;
        }

        public void Update(GameTime gameTime)
        {
            foreach (Shape shape in ShapesDict.Values)
            {
                shape.Update(gameTime);
            }
        }

        public void ResetPuzzleLocks()
        {
            if (UnlockedAtOnce <= 0)
            {
                foreach (Puzzle puzzle in puzzles)
                {
                    puzzle.IsLocked = false;
                }
            }
            else
            {
                foreach (Puzzle puzzle in puzzles)
                {
                    puzzle.IsLocked = true;
                }

                for (int i = 0; i < UnlockedAtOnce; i++)
                {
                    puzzles[i].IsLocked = false;
                }
            }
        }

        public int UnlockNextPuzzle()
        {
            // returns 1 for next puzzle
            // returns 0 if no more puzzles to unlock
            for (int i = 0; i < puzzles.Count; i++)
            {
                if (puzzles[i].IsLocked == true)
                {
                    puzzles[i].IsLocked = false;
                    return 1;
                }
            }
            return 0;
        }

        public override string ToString()
        {
            return "\n--- Asset Name: " + AssetName +
                   "\n--- isCleared: " + IsCleared +
                   "\n--- Key: " + Key +
                   "\n--- Name: " + Name +
                   "\n--- PuzzleContentNames: " + string.Join(",", PuzzleContentNames.ToArray()) +
                   "\n--- ShapeContentNames: " + string.Join(",", ShapeContentNames.ToArray()) +
                   "\n--- ShapePickerSelectedIdx: " + ShapePickerSelectedIdx +
                   "\n--- finalPuzzleKey: " + finalPuzzleKey +
                   "\n--- Statistics: " + Statistics;
        }

        public object Clone()
        {
            PuzzleSet ps = new PuzzleSet();
            ps.AssetName = AssetName;
            ps.IsCleared = IsCleared;
            ps.Key = Key;
            ps.Name = Name;
            ps.PuzzleContentNames.AddRange(puzzleContentNames);
            ps.Puzzles.AddRange(puzzles);
            ps.ShapeContentNames.AddRange(ShapeContentNames);
            ps.ShapePickerSelectedIdx = ShapePickerSelectedIdx;

            // deep copy shapes dict
            ps.ShapesDict = new Dictionary<int, Shape>(ShapesDict.Count);
            foreach (KeyValuePair<int, Shape> kvp in ShapesDict)
            {
                ps.ShapesDict.Add(kvp.Key, kvp.Value.Clone() as Shape);
            }

            ps.Statistics = Statistics;
            ps.tileTexture = TileTexture;
            ps.TileTextureName = TileTextureName;
            ps.UnlockedAtOnce = UnlockedAtOnce;

            return ps;
        }

        /*
        /// Read a PuzzleSet object from the content pipeline.
        public class PuzzleSetReader : ContentTypeReader<PuzzleSet>
        {
            protected override PuzzleSet Read(ContentReader input, PuzzleSet existingInstance)
            {
                PuzzleSet puzzleSet = existingInstance;
                if (puzzleSet == null)
                {
                    puzzleSet = new PuzzleSet();
                }

                puzzleSet.AssetName = input.AssetName;
                puzzleSet.Name = input.ReadString();

                puzzleSet.TileTextureName = input.ReadString();
                puzzleSet.tileTexture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine(@"Textures\GameScreens\Puzzle", puzzleSet.TileTextureName));
                puzzleSet.UnlockedAtOnce = input.ReadInt32();
                puzzleSet.Key = input.ReadInt32();
                puzzleSet.PuzzleContentNames.AddRange(input.ReadObject<List<string>>());

                // moved below shape foreach to add shapesdict to each puzzle
                int counter = 1;
                foreach (string puzzleName in puzzleSet.puzzleContentNames)
                {
                    Puzzle puzzle = input.ContentManager.Load<Puzzle>(Path.Combine("Puzzles", puzzleName)).Clone() as Puzzle;
                    puzzle.Place = counter;
                    puzzleSet.puzzles.Add(puzzle);
                    counter++;
                }

                // set the key for the final puzzle of the set
                puzzleSet.finalPuzzleKey = puzzleSet.puzzles[puzzleSet.puzzles.Count - 1].Key;

                if (puzzleSet.UnlockedAtOnce > puzzleSet.Puzzles.Count)
                {
                    puzzleSet.UnlockedAtOnce = puzzleSet.Puzzles.Count;
                }

                // load shapes and set base positions
                puzzleSet.ShapeContentNames = input.ReadObject<List<string>>();
                foreach (string shapeContent in puzzleSet.ShapeContentNames)
                {
                    Shape tmpShape = input.ContentManager.Load<Shape>(System.IO.Path.Combine(@"Shapes", shapeContent)).Clone() as Shape;
                    tmpShape.Reset();
                    puzzleSet.ShapesDict.Add(tmpShape.Key, tmpShape);
                }

                puzzleSet.Statistics = new PuzzleSetStatistics(puzzleSet.ShapesDict.Keys.ToList<int>());

                return puzzleSet;
            }
        }
        */
    }
}