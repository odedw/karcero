using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    internal interface IMapPreProcessor<T> where T : class, IBinaryCell, new()
    {
        void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer);

    }
}
