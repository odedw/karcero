namespace Karcero.Engine.Contracts
{
    /// <summary>
    /// Base interface for cell type
    /// </summary>
    public interface IBaseCell
    {
        /// <summary>
        /// The cell's row in the map (will be set by the generator).
        /// </summary>
        int Row { get; set; }
        /// <summary>
        /// The cell's column in the map (will be set by the generator).
        /// </summary>
        int Column { get; set; }
    }
}
