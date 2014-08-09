using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Contracts;
using Karcero.Engine.Helpers;

namespace Karcero.Engine.Models
{
    internal class BinaryCell : IBinaryCell
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsOpen { get; set; }
        public Dictionary<Direction,bool> Sides { get; set; }

        public BinaryCell()
        {
            Sides = new Dictionary<Direction, bool>();
            foreach (Direction direction in GetAll.ValuesOf<Direction>())
            {
                Sides[direction] = false;
            }
        }
    }
}
