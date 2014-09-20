using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karcero.Engine.Models
{
    public class Size
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Size(int width, int height)
        {
            Height = height;
            Width = width;
        }
    }
}
