using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karcero.Engine.Models
{
    internal class Tuple<T1, T2>
    {
        internal T1 Item1 { get; private set; }
        internal T2 Item2 { get; private set; }
        internal Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}
