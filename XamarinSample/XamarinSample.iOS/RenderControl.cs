using System;
using OpenTK.Graphics.ES30;
using SkiaSharp;

namespace XamarinSample.iOS
{
	public class RenderControl
	{
		private Rectangular rectangular;
		private GLTexture imageTexture;
        private FrameBuffer leftFrameBuffer;
        private FrameBuffer rightFrameBuffer;

        public RenderControl()
		{
			rectangular = new Rectangular();
            imageTexture = new GLTexture(500, 500, 0);
            leftFrameBuffer = new FrameBuffer(imageTexture.Width, imageTexture.Height, 0);
            rightFrameBuffer = new FrameBuffer(imageTexture.Width, imageTexture.Height, 0);
        }

        private void Draw(SKBitmap bitmap, bool invert)
        {
            int width = imageTexture.Width;
            int height = imageTexture.Height;

            GL.Viewport(0, 0, width, height);
            GLCommon.GLError();

            // カラーを設定して、クリア
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            GLCommon.GLError();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GLCommon.GLError();

            GLCommon.UseProgram();

            GLCommon.SetViewport(width, height);
            float size = Math.Min((float)width / 2, (float)height / 2);
            GLCommon.SetModel(size, size, invert);
            GLCommon.SetView(0.0f, 0.0f);

            imageTexture.SetTexture(bitmap);
            imageTexture.UseTexture();
            rectangular.Draw();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GLCommon.SetModel(size, size);
            imageTexture.SetTexture(Time.GetTimeBitmap(width, height));
            imageTexture.UseTexture();
            rectangular.Draw();
            GL.Disable(EnableCap.Blend);
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

            GLCommon.SetDefaultFrameBuffer();

            GL.Viewport(0, 0, width, height);
            GLCommon.GLError();

            // カラーを設定して、クリア
            GL.ClearColor(0.3f, 0.4f, 0.5f, 1.0f);
            GLCommon.GLError();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GLCommon.GLError();

            GLCommon.UseProgram();

            GLCommon.SetViewport(width, height);

            if (doubleDisplay)
			{
                float size = Math.Min((float)width / 4, (float)height / 4);
                GLCommon.SetModel(size, size);

                GLCommon.SetView(-(float)width / 4, 0.0f);
                leftFrameBuffer.UseTexture();
                rectangular.Draw();

                GLCommon.SetView((float)width / 4, 0.0f);
                rightFrameBuffer.UseTexture();
                rectangular.Draw();
            }
			else
			{
                float size = Math.Min((float)width / 2, (float)height / 2);
                GLCommon.SetModel(size, size);
                GLCommon.SetView(0.0f, 0.0f);

                leftFrameBuffer.UseTexture();
                rectangular.Draw();
            }
        }
	}
}
