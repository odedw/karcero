using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
