using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShopData.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeShopData.Models
{
    /// One section of the world, and all of the data in it.
    public class Puzzle : ICloneable
    {
        private int place;
        [ContentSerializerIgnore]
        public int Place
        {
            get { return place; }
            set { place = value; }
        }

        /// The name of this section of the world.
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int key;
        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        /// The dimensions of the map, in tiles.
        private Point dimensions;
        public Point Dimensions 
        {
            get { return dimensions; }
            set { dimensions = value; }
        }

        /// The size of the tiles in this map, in pixels.
        private Point tileSize;
        public Point TileSize
        {
            get { return tileSize; }
            set { tileSize = value; }
        }

        private Point size;
        [ContentSerializerIgnore]
        public Point Size
        {
            get { return size; }
            set { size = value; }
        }

        private Vector2 gridOrigin;
        public Vector2 GridOrigin
        {
            get { return gridOrigin; }
            set { gridOrigin = value; }
        }

        private bool isGridShiftX;
        public bool IsGridShiftX
        {
            get { return isGridShiftX; }
            set { isGridShiftX = value; }
        }

        private bool isGridShiftY;
        public bool IsGridShiftY
        {
            get { return isGridShiftY; }
            set { isGridShiftY = value; }
        }

        private bool isLocked = true;
        [ContentSerializerIgnore]
        public bool IsLocked
        {
            get { return isLocked; }
            set { isLocked = value; }
        }

        private bool isCleared = false;
        [ContentSerializerIgnore]
        public bool IsCleared
        {
            get { return isCleared; }
            set { isCleared = value; }
        }

        private bool isRenderPortrait = false;
        [ContentSerializerIgnore]
        public bool IsRenderPortrait
        {
            get { return isRenderPortrait; }
            set { isRenderPortrait = value; }
        }

        private Vector2 bgFgOrigin;
        public Vector2 BgFgOrigin
        {
            get { return bgFgOrigin; }
            set { bgFgOrigin = value; }
        }

        /// The content name of the texture that contains the tiles for this map.
        private string backgroundTextureName;
        public string BackgroundTextureName
        {
            get { return backgroundTextureName; }
            set { backgroundTextureName = value; }
        }

        /// The content name of the texture that contains the tiles for this map.
        private string foregroundTextureName;
        public string ForegroundTextureName
        {
            get { return foregroundTextureName; }
            set { foregroundTextureName = value; }
        }

        private Texture2D portraitTexture;
        [ContentSerializerIgnore]
        public Texture2D PortraitTexture
        {
            get { return portraitTexture; }
            set { portraitTexture = value; }
        }

        /// The number of tiles in a row of the map texture.
        /// Used to determine the source rectangle from the map layer value.
        private int tilesPerRow = 10;

        // Music
        /// The name of the music cue for this tile.
        private string musicCueName;
        public string MusicCueName
        {
            get { return musicCueName; }
            set { musicCueName = value; }
        }

        // map layers
        /// Spatial array for the ground tiles for this map.
        private int[] puzzleLayer;
        public int[] PuzzleLayer
        {
            get { return puzzleLayer; }
            set { puzzleLayer = value; }
        }

        /// Spatial array for the ground tiles for this map.
        private int[] tileLayer;
        public int[] TileLayer
        {
            get { return tileLayer; }
            set { tileLayer = value; }
        }

        private ContentSolution ourSolution;
        public ContentSolution OurSolution
        {
            get { return ourSolution; }
            set { ourSolution = value; }
        }

        private PlayerSolution playerSolution;
        [ContentSerializerIgnore]
        public PlayerSolution PlayerSolution
        {
            get { return playerSolution; }
            set { playerSolution = value; }
        }

        private PuzzleStatistics statistics;
        [ContentSerializer(Optional = true)]
        public PuzzleStatistics Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }

        public void LoadContent(ContentManager content)
        {
            size = new Point(dimensions.X * tileSize.X,
                             dimensions.Y * tileSize.Y);

            if (isGridShiftX) // if isEven
            {
                gridOrigin += new Vector2(tileSize.X / 2, 0);
            }

            if (isGridShiftY) // if isEven
            {
                gridOrigin += new Vector2(0, tileSize.Y / 2);
            }

            ourSolution.PuzzleKey = key;
            shapeLayer = new int[165];
            collisionLayer = new int[165];
            statistics = new PuzzleStatistics();
        }

        public string PlaceText
        {
            get
            {
                string prefixStr = "";
                if (place < 10)
                {
                    prefixStr = "00";
                }
                else if (place < 100)
                {
                    prefixStr = "0";
                }

                return prefixStr + place.ToString();
            }
        }

        // puzzle stats helper methods
        public void CountHintUsed() { statistics.HintsUsed++; }
        public void CountCWRotation() { statistics.CWRotations++; }
        public void CountCCWRotation() { statistics.CCWRotations++; }
        public void CountHorizontalFlip() { statistics.HorizontalFlips++; }
        public void CountVerticalFlip() { statistics.VerticalFlips++; }
        public void SetCompletedStats(int shapesUsed, double secs)
        {
            statistics.ShapesUsed = shapesUsed;
            statistics.SecondsTaken = secs;
        }

        /// Retrieves the base layer value for the given map position.
        public int GetPuzzleLayerValue(Point puzzlePoint)
        {
            // check the parameter
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= Dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= Dimensions.Y))
            {
                throw new ArgumentOutOfRangeException("puzzlePoint");
            }

            return puzzleLayer[puzzlePoint.Y * dimensions.X + puzzlePoint.X];
        }

        /// Retrieves the base layer value for the given map position.
        public int GetTileLayerValue(Point puzzlePoint)
        {
            // check the parameter
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= Dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= Dimensions.Y))
            {
                throw new ArgumentOutOfRangeException("puzzlePoint");
            }

            return tileLayer[puzzlePoint.Y * dimensions.X + puzzlePoint.X];
        }

        /// <summary>
        /// Retrieves the source rectangle for the tile in the given position
        /// in the base layer.
        /// </summary>
        /// <remarks>This method allows out-of-bound (blocked) positions.</remarks>
        public Rectangle GetTileLayerSourceRectangle(Point puzzlePoint)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= Dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= Dimensions.Y))
            {
                return Rectangle.Empty;
            }

            int baseLayerValue = GetTileLayerValue(puzzlePoint);
            if (baseLayerValue < 0)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(
                (baseLayerValue % tilesPerRow) * tileSize.X,
                (baseLayerValue / tilesPerRow) * tileSize.Y,
                tileSize.X, tileSize.Y);
        }

        /// <summary>
        /// Retrieves the source rectangle for the given position.
        /// </summary>
        public Rectangle GetBackgroundSourceRectangle(Point puzzlePoint)
        {
            // check the parameter, but out-of-bounds is nonfatal
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= Dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= Dimensions.Y))
            {
                return Rectangle.Empty;
            }

            return new Rectangle(puzzlePoint.X * tileSize.X, 
                                 puzzlePoint.Y * tileSize.Y, 
                                 tileSize.X, 
                                 tileSize.Y);
        }

        public bool IsPointInPuzzle(Point puzzlePoint)
        {
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= Dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= Dimensions.Y))
            {
                return false;
            }
            return true;
        }

        // if solved returns number of shapes used in solution, otherwise returns 0
        public int CheckSolved(Dictionary<int, Shape> shapesDict)
        {
            List<int> shapeKeys = new List<int>();

            // add all keys for shapes that exist somewhere within the puzzle outline
            for (int y = 0; y < Dimensions.Y; y++)
            {
                for (int x = 0; x < Dimensions.X; x++)
                {
                    if (GetPuzzleLayerValue(new Point(x, y)) == 0)
                    {
                        Point point = new Point(x, y);

                        // if tile empty can return not solved
                        if (GetCollisionLayerValue(point) == 0)
                        {
                            return 0;
                        }

                        int key = GetShapeLayerValue(point);
                        if (!shapeKeys.Contains<int>(key))
                        {
                            shapeKeys.Add(key);
                        }
                    }
                   
                }
            }

            // now make sure every shape in puzzle outline is placed validly
            foreach (int key in shapeKeys)
            {
                if (!shapesDict[key].IsValid)
                {
                    return 0;
                }
            }

            return shapeKeys.Count;
        }

        public bool CanSnapShape(Point puzzlePoint, Shape shape)
        {
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= Dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= Dimensions.Y))
            {
                throw new ArgumentOutOfRangeException("puzzlePoint");
            }

            for (int y = 0; y < shape.CurrentMatrixDimensions.Y; y++)
            {
                for (int x = 0; x < shape.CurrentMatrixDimensions.X; x++)
                {
                    if (shape.CurrentMatrix[y][x] == 1)
                    {
                        if (puzzleLayer[(puzzlePoint.Y + y) * dimensions.X + puzzlePoint.X + x] != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool CanAddShape(Point puzzlePoint, Shape shape)
        {
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= Dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= Dimensions.Y))
            {
                throw new ArgumentOutOfRangeException("puzzlePoint");
            }

            for (int y = 0; y < shape.CurrentMatrixDimensions.Y; y++)
            {
                for (int x = 0; x < shape.CurrentMatrixDimensions.X; x++)
                {
                    if (shape.CurrentMatrix[y][x] == 1)
                    {
                        if (collisionLayer[(puzzlePoint.Y + y) * dimensions.X + puzzlePoint.X + x] != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        // add shape to matrices
        public void AddShape(Point puzzlePoint, Shape shape)
        {
            // check the parameter
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= Dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= Dimensions.Y))
            {
                throw new ArgumentOutOfRangeException("puzzlePoint");
            }

            shape.IsValid = true;

            for (int y = 0; y < shape.CurrentMatrixDimensions.Y; y++)
            {
                for (int x = 0; x < shape.CurrentMatrixDimensions.X; x++)
                {

                    if (shape.CurrentMatrix[y][x] == 1)
                    {
                        // check if all parts of shape are within the valid outline
                        if (puzzleLayer[(puzzlePoint.Y + y) * dimensions.X + puzzlePoint.X + x] == 1)
                        {
                            shape.IsValid = false;
                        }
                        shapeLayer[(puzzlePoint.Y + y) * dimensions.X + puzzlePoint.X + x] = shape.Key;
                        collisionLayer[(puzzlePoint.Y + y) * dimensions.X + puzzlePoint.X + x] = shape.Key;
                    }
                }
            }
            
        }

        // remove shape from matrices
        public void RemoveShape(Shape shape)
        {
            for (int y = 0; y < Dimensions.Y; y++)
            {
                for (int x = 0; x < Dimensions.X; x++)
                {
                    int idx = y * dimensions.X + x;
                    if (shapeLayer[idx] == shape.Key)
                    {
                        shapeLayer[idx] = 0;
                        collisionLayer[idx] = 0;
                    }
                }
            }

            shape.IsValid = false;
        }

        /// Spatial array for the fringe tiles for this map.
        private int[] shapeLayer;
        [ContentSerializerIgnore]
        public int[] ShapeLayer
        {
            get { return shapeLayer; }
            set { shapeLayer = value; }
        }

        /// Retrieves the fringe layer value for the given map position.
        public int GetShapeLayerValue(Point puzzlePoint)
        {
            // check the parameter
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= dimensions.Y))
            {
                throw new ArgumentOutOfRangeException("puzzlePoint");
            }

            return shapeLayer[puzzlePoint.Y * dimensions.X + puzzlePoint.X];
        }

        /// Spatial array for the object images on this map.
        private int[] collisionLayer;
        [ContentSerializerIgnore]
        public int[] CollisionLayer
        {
            get { return collisionLayer; }
            set { collisionLayer = value; }
        }

        /// Retrieves the object layer value for the given map position.
        public int GetCollisionLayerValue(Point puzzlePoint)
        {
            // check the parameter
            if ((puzzlePoint.X < 0) || (puzzlePoint.X >= dimensions.X) ||
                (puzzlePoint.Y < 0) || (puzzlePoint.Y >= dimensions.Y))
            {
                throw new ArgumentOutOfRangeException("puzzlePoint");
            }

            return collisionLayer[puzzlePoint.Y * dimensions.X + puzzlePoint.X];
        }

        public object Clone()
        {
            Puzzle puzzle = new Puzzle();

//            puzzle.AssetName = AssetName;
            puzzle.BgFgOrigin = BgFgOrigin;
            puzzle.BackgroundTextureName = BackgroundTextureName;
            puzzle.CollisionLayer = (collisionLayer != null ? CollisionLayer.Clone() as int[] : null);
            puzzle.Dimensions = Dimensions;
            puzzle.ForegroundTextureName = ForegroundTextureName;
            puzzle.GridOrigin = GridOrigin;
            puzzle.IsCleared = IsCleared;
            puzzle.IsGridShiftX = IsGridShiftX;
            puzzle.isGridShiftY = IsGridShiftY;
            puzzle.IsLocked = IsLocked;
            puzzle.Key = Key;
            puzzle.MusicCueName = MusicCueName;
            puzzle.Name = Name;
            puzzle.Place = Place;
            puzzle.OurSolution = OurSolution;
            puzzle.PlayerSolution = PlayerSolution;
            puzzle.portraitTexture = portraitTexture;
            puzzle.PuzzleLayer = (puzzleLayer != null ? PuzzleLayer.Clone() as int[] : null);
            puzzle.ShapeLayer = (shapeLayer != null ? ShapeLayer.Clone() as int[] : null);
            puzzle.Size = Size;
            puzzle.Statistics = Statistics;
            puzzle.TileLayer = (tileLayer != null ? TileLayer.Clone() as int[] : null);
            puzzle.TileSize = TileSize;
            puzzle.tilesPerRow = tilesPerRow;

            return puzzle;
        }

        public string GetDebugInformation()
        {
            String returnString = "Pl: " + place + " Name : " + name + " Key: " + key +
                                  "\n" + "" +
                                  "\nMusic : " + musicCueName +
                                  "\nDimensions : " + dimensions.ToString() +
                                  "\nTile Size : " + tileSize.ToString() +
                                  "\nOrigin : " + GridOrigin.ToString() +
                                  "\nIs Cleared: " + isCleared.ToString();

            returnString += "\n\nPuzzle Layer: ";
            returnString += "\n--------------------\n";
            for (int y = 0; y < Dimensions.Y; y++)
            {
                for (int x = 0; x < Dimensions.X; x++)
                {
                    returnString += PuzzleLayer[y * dimensions.X + x] + " ";
                }
                returnString += "\n";
            }

            returnString += "\nShape Layer: ";
            returnString += "\n--------------------\n";
            for (int y = 0; y < Dimensions.Y; y++)
            {
                for (int x = 0; x < Dimensions.X; x++)
                {
                    returnString += ShapeLayer[y * dimensions.X + x] + " ";
                }
                returnString += "\n";
            }

            returnString += "\nTile Layer: ";
            returnString += "\n--------------------\n";
            for (int y = 0; y < Dimensions.Y; y++)
            {
                for (int x = 0; x < Dimensions.X; x++)
                {
                    returnString += TileLayer[y * dimensions.X + x] + " ";
                }
                returnString += "\n";
            }            
            return returnString;
        }

        public override string ToString()
        {
 	        return base.ToString();
        }

        /*
        /// Read a Map object from the content pipeline.
        public class PuzzleReader : ContentTypeReader<Puzzle>
        {
            protected override Puzzle Read(ContentReader input, Puzzle existingInstance)
            {                
                Puzzle puzzle = existingInstance;
                if (puzzle == null)
                {
                    puzzle = new Puzzle();
                }

                puzzle.AssetName = input.AssetName;

                puzzle.Name = input.ReadString();
                puzzle.Key = input.ReadInt32();
                puzzle.Dimensions = input.ReadObject<Point>();
                puzzle.TileSize = input.ReadObject<Point>();
                puzzle.Size = new Point(puzzle.Dimensions.X * puzzle.TileSize.X,
                                        puzzle.Dimensions.Y * puzzle.TileSize.Y);
                puzzle.GridOrigin = input.ReadObject<Vector2>();
                puzzle.IsGridShiftX = input.ReadBoolean();
                puzzle.IsGridShiftY = input.ReadBoolean();

                if (puzzle.IsGridShiftX) // if isEven
                {
                    puzzle.GridOrigin += new Vector2(puzzle.TileSize.X / 2, 0);
                }

                if (puzzle.IsGridShiftY) // if isEven
                {
                    puzzle.GridOrigin += new Vector2(0, puzzle.TileSize.Y / 2);
                }

                puzzle.BgFgOrigin = input.ReadObject<Vector2>();
                puzzle.BackgroundTextureName = input.ReadString();
                puzzle.ForegroundTextureName = input.ReadString();                
                puzzle.MusicCueName = input.ReadString();
                puzzle.PuzzleLayer = input.ReadObject<int[]>();
                puzzle.TileLayer = input.ReadObject<int[]>();
                puzzle.OurSolution = input.ReadObject<ContentSolution>();
                puzzle.OurSolution.PuzzleKey = puzzle.Key;
                puzzle.ShapeLayer = new int[165];
                puzzle.CollisionLayer = new int[165];

                puzzle.Statistics = new PuzzleStatistics();

                return puzzle;
            }
        }
        */
    }
}
