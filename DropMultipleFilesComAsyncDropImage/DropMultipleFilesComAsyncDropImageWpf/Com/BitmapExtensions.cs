using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrawingColor = System.Drawing.Color;
using DrawingRectangle = System.Drawing.Rectangle;

namespace DropMultipleFilesComAsyncIconWpf.Com
{
    internal static class BitmapExtensions
    {
        public static Bitmap ToBitmap(this UIElement ui)
        {
            var w = ui.RenderSize.Width;
            var h = ui.RenderSize.Height;
            if (w > h && w > 300.0)
            {
                h *= 300.0 / w;
                w = 300.0;
            }
            else if (h > w && h > 300.0)
            {
                w *= 300.0 / h;
                h = 300.0;
            }
            var iw = (int)Math.Floor(w);
            var ih = (int)Math.Floor(h);
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawRectangle(new BitmapCacheBrush(ui), null, new Rect(0, 0, iw, ih));
            }
            var bmpSource = new RenderTargetBitmap(iw, ih, 96, 96, PixelFormats.Pbgra32);
            bmpSource.Render(dv);

            var sourceRect = new Int32Rect(0, 0, bmpSource.PixelWidth, bmpSource.PixelHeight);
            var pxFormat = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
            var bmp = new Bitmap(sourceRect.Width, sourceRect.Height, pxFormat);

            BitmapData bmpData = bmp.LockBits(
                sourceRect.ToDrawingRectangle(),
                ImageLockMode.ReadWrite,
                pxFormat);
            bmpSource.CopyPixels(sourceRect, bmpData.Scan0, bmpData.Stride * sourceRect.Height, bmpData.Stride);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private static DrawingColor ToDrawingColor(this System.Windows.Media.Color color)
        {
            return DrawingColor.FromArgb(
                color.A, color.R, color.G, color.B);
        }

        private static DrawingRectangle ToDrawingRectangle(this Int32Rect rect)
        {
            return new DrawingRectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
