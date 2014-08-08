using System.Collections.Generic;
using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    public interface ICell : IBaseCell
    {
        TerrainType Terrain { get; set; }

        Dictionary<Direction, SideType> Sides { get; set; }

        ICell Clone();
    }
}