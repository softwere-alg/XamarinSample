using System;
using SkiaSharp;

namespace XamarinSample
{
	public class ImageManager : IDisposable
    {
        public static ImageManager[] ImageManagers = new ImageManager[] { new ImageManager(), new ImageManager() };

		private SKBitmap bitmap;
		private object _lock = new object();
        private System.Timers.Timer timer;
        private int angle = 0;

        public ImageManager()
		{
			int width = 500;
			int height = 500;

			bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul);

            using (SKCanvas bitmapCanvas = new SKCanvas(bitmap))
            {
                bitmapCanvas.Clear();
				bitmapCanvas.DrawRect(width / 4, width / 4, width / 2, height / 2, new SKPaint { Color = new SKColor(255, 100, 5, 255) });
            }

            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Interval = 1000f / 60;
            timer.Elapsed += Timer_Elapsed;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 管理（managed）リソースの破棄処理をここに記述します。
                timer.Stop();
                timer = null;
            }

            // 非管理（unmanaged）リソースの破棄処理をここに記述します。
        }

        ~ImageManager()
        {
            Dispose(false);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_lock)
            {
                angle = (angle + 1) % 360;
            }
        }

        public void StartUpdate()
        {
            timer.Start();
        }

        public void StopUpdate()
        {
            timer.Stop();
        }

		public SKBitmap GetImage()
		{
            SKBitmap rotatedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

            lock (_lock)
            {
                using (SKCanvas bitmapCanvas = new SKCanvas(rotatedBitmap))
                {
                    bitmapCanvas.Clear();
                    bitmapCanvas.RotateDegrees(angle, bitmap.Width / 2, bitmap.Height / 2);
                    bitmapCanvas.DrawBitmap(bitmap, new SKPoint());
                }
            }

            return rotatedBitmap;
        }
    }
}
