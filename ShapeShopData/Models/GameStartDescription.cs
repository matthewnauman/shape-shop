using Microsoft.Xna.Framework.Content;

namespace ShapeShopData.Models
{
    /// The data needed to start a new game.
    public class GameStartDescription
    {
        /// The content name of the  map for a new game.
        private string puzzleSetContentName;
        public string PuzzleSetContentName
        {
            get { return puzzleSetContentName; }
            set { puzzleSetContentName = value; }
        }

        private int saveSlotNumber;
        [ContentSerializerIgnore]
        public int SaveSlotNumber
        {
            get { return saveSlotNumber; }
            set { saveSlotNumber = value; }
        }

        /*
        /// Content Type Reader
        /// Read a GameStartDescription object from the content pipeline.
        public class GameStartDescriptionReader : ContentTypeReader<GameStartDescription>
        {
            protected override GameStartDescription Read(ContentReader input, GameStartDescription existingInstance)
            {
                GameStartDescription desc = existingInstance;
                if (desc == null)
                {
                    desc = new GameStartDescription();
                }

                desc.PuzzleSetContentName = input.ReadString();

                return desc;
            }
        }
        */

    }
}
