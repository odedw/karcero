using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karcero.Engine.Models
{
    public struct Location
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Location(int row, int column) : this()
        {
            Row = row;
            Column = column;
        }
    }
}
