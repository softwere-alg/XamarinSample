using System;
using SkiaSharp;

namespace XamarinSample
{
	public static class Time
	{
		public static SKBitmap GetTimeBitmap(int width, int height)
		{
            SKBitmap bitmap = new SKBitmap(width, height);
            SKBitmap flippedBitmap = new SKBitmap(width, height);

            using (SKCanvas bitmapCanvas = new SKCanvas(bitmap))
            {
                bitmapCanvas.Clear(new SKColor(0, 0, 0, 0));
                bitmapCanvas.DrawText($"{DateTime.Now}", 0, 32.0f, new SKPaint {
                    TextSize = 32.0f,
                    IsAntialias = true,
                    IsStroke = false,
                    Color = new SKColor(255, 255, 255, 255)
                });
            }

            using (SKCanvas bitmapCanvas = new SKCanvas(flippedBitmap))
            {
                bitmapCanvas.Clear(new SKColor(0, 0, 0, 0));
                bitmapCanvas.Scale(1, -1, 0, bitmap.Height / 2);
                bitmapCanvas.DrawBitmap(bitmap, new SKPoint());
            }

            return flippedBitmap;
        }
	}
}
