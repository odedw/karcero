
namespace Karcero.Engine.Models
{
    public class Room
    {
        #region Properties
        public int Row { get; set; }
        public int Column { get; set; }
        public Size Size { get; set; }
        public int Top { get { return Row; }}
        public int Bottom { get { return Row + Size.Height; }}
        public int Left { get { return Column; }}
        public int Right { get { return Column + Size.Width; }}
        #endregion

        #region Methods
        public bool IsLocationInRoom(int row, int column)
        {
            return Row <= row && Bottom > row &&
                   Column <= column && Right > column;
        }
        #endregion
    }
}
