using System;
using Metal;
using SkiaSharp;
using Xamarin.Forms;

namespace XamarinSample.iOS
{
	public class RenderControl
	{
		private Rectangular rectangular;
		private MTLTexture imageTexture;
        private MTLTexture imageTexture2;
        private FrameBuffer leftFrameBuffer;
        private FrameBuffer rightFrameBuffer;

        public RenderControl(IMTLDevice device)
		{
			rectangular = new Rectangular(device);
            imageTexture = new MTLTexture(500, 500, 0, device, false);
            imageTexture2 = new MTLTexture(500, 500, 0, device, false);
            leftFrameBuffer = new FrameBuffer(imageTexture.Width, imageTexture.Height, 0, device);
            rightFrameBuffer = new FrameBuffer(imageTexture.Width, imageTexture.Height, 0, device);
        }

        private void Draw(SKBitmap bitmap, bool invert)
        {
            int width = imageTexture.Width;
            int height = imageTexture.Height;

            // カラーを設定して、クリア
            MTLCommon.SetClearColor(new Color(0.5f, 0.5f, 0.5f, 1.0f));

            MTLCommon.SetViewport(width, height);
            float size = Math.Min((float)width / 2, (float)height / 2);
            MTLCommon.SetModel(size, size, invert);
            MTLCommon.SetView(0.0f, 0.0f);

            imageTexture.SetTexture(bitmap);
            MTLCommon.SetTexture(imageTexture);
            rectangular.Draw(MTLLoadAction.Clear);

            MTLCommon.SetModel(size, size);
            imageTexture2.SetTexture(Time.GetTimeBitmap(width, height));
            MTLCommon.SetTexture(imageTexture2);
            rectangular.Draw();
        }

		public void Draw(int width, int height, bool doubleDisplay, bool leftDisplay, bool rightDisplay, bool leftInvert, bool rightInvert)
		{
            if (leftDisplay)
            {
                leftFrameBuffer.SetFrameBuffer();

                Draw(ImageManager.ImageManagers[0].GetImage(), leftInvert);
            }

            if (rightDisplay)
            {
                rightFrameBuffer.SetFrameBuffer();

                Draw(ImageManager.ImageManagers[1].GetImage(), rightInvert);
            }

            MTLCommon.SetDefaultFrameBuffer();

            // カラーを設定して、クリア
            MTLCommon.SetClearColor(new Color(0.3f, 0.4f, 0.5f, 1.0f));

            MTLCommon.SetViewport(width, height);

            if (doubleDisplay)
			{
                float size = Math.Min((float)width / 4, (float)height / 4);
                MTLCommon.SetModel(size, size);

                MTLCommon.SetView(-(float)width / 4, 0.0f);
                leftFrameBuffer.SetTexture();
                rectangular.Draw(MTLLoadAction.Clear);

                MTLCommon.SetView((float)width / 4, 0.0f);
                rightFrameBuffer.SetTexture();
                MTLCommon.ReservePresent();
                rectangular.Draw();
            }
			else
			{
                float size = Math.Min((float)width / 2, (float)height / 2);
                MTLCommon.SetModel(size, size);
                MTLCommon.SetView(0.0f, 0.0f);

                leftFrameBuffer.SetTexture();
                MTLCommon.ReservePresent();
                rectangular.Draw(MTLLoadAction.Clear);
            }
        }
	}
}
