using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karcero.Engine.Models
{
    /// <summary>
    /// A measurement of size.
    /// </summary>
    public class Size
    {
        /// <summary>
        /// The height.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Returns an instance with the specified measurements.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Size(int width, int height)
        {
            Height = height;
            Width = width;
        }
    }
}
