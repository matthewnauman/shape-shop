using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShopData.Models;

namespace ShapeShop.UI
{
    public class PickerEntry
    {
        public static readonly float RENDER_SCALE = .333f;
        public static readonly float ANCHOR_TOP_ENTRY_Y = 120f;
        public static readonly Vector2 TOP_ENTRY_POSITION = new Vector2(0, ANCHOR_TOP_ENTRY_Y);

        private Vector2 entryTexturePosition;
        private Vector2 entryTextureOrigin;

        private float rotation = 0f;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        private Puzzle puzzle;
        public Puzzle Puzzle
        {
            get { return puzzle; }
        }

        private Texture2D entryTexture;
        public Texture2D EntryTexture
        {
            get { return entryTexture; }
            set 
            { 
                entryTexture = value;
                entryTexturePosition = new Vector2(PuzzleEngine.ViewportCenter.X - entryTexture.Width / 2, 0f);
                entryTextureOrigin = new Vector2(entryTexture.Width / 2, entryTexture.Height * 1.11f);
            }
        }

        private bool isRenderEntry = false;
        public bool IsRenderEntry
        {
            get { return isRenderEntry; }
            set { isRenderEntry = value; }
        }

        public PickerEntry(Puzzle puzzle)
        {
            this.puzzle = puzzle;      
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 basePosition)
        {
            spriteBatch.Draw(entryTexture, 
                             basePosition + entryTexturePosition + entryTextureOrigin, 
                             null, 
                             Color.White, 
                             rotation, 
                             entryTextureOrigin, 
                             1f, 
                             SpriteEffects.None, 
                             0);
        }

        public override string ToString()
        {
            return puzzle.Name + " {" + puzzle.Key + "}";
        }

    }
}
