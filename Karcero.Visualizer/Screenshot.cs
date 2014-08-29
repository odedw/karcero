using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Karcero.Visualizer
{
    public static class Screenshot
    {
        /// <summary>
        /// Gets a PNG "screenshot" of the current UIElement
        /// </summary>
        /// <param name="source">UIElement to screenshot</param>
        /// <param name="scale">Scale to render the screenshot</param>
        /// <returns>Byte array of PNG data</returns>
        public static byte[] GetPngImage(this UIElement source, double scale = 1)
        {
            var actualHeight = source.RenderSize.Height;
            var actualWidth = source.RenderSize.Width;

            var renderHeight = actualHeight * scale;
            var renderWidth = actualWidth * scale;

            var renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            var sourceBrush = new VisualBrush(source);

            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Size(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);

            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
            Byte[] imageArray;

            using (var outputStream = new MemoryStream())
            {
                pngEncoder.Save(outputStream);
                imageArray = outputStream.ToArray();
            }

            return imageArray;
        }
    }
}
