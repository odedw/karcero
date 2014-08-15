namespace Karcero.Engine.Models
{
    public struct Location
    {
        #region Properties
        public int Row { get; set; }
        public int Column { get; set; }
        #endregion

        #region Constructor
        public Location(int row, int column) : this()
        {
            Row = row;
            Column = column;
        }
        #endregion
    }
}
