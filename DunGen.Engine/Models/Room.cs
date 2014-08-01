using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunGen.Engine.Models
{
    public class Room
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Size Size { get; set; }
        public int Top { get { return Row; }}
        public int Bottom { get { return Row + Size.Height; }}
        public int Left { get { return Column; }}
        public int Right { get { return Column + Size.Width; }}

        public bool IsCellInRoom(Cell cell)
        {
            return cell != null && 
                   Row <= cell.Row && Bottom > cell.Row &&
                   Column <= cell.Column && Right > cell.Column;
        }
    }
}
