using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Karcero.Engine.Models;

[assembly: InternalsVisibleTo("Karcero.Tests")]

namespace Karcero.Engine.Contracts
{
    internal interface IBinaryCell : IBaseCell
    {
        bool IsOpen { get; set; }
        Dictionary<Direction, bool> Sides { get; set; }
    }
}
