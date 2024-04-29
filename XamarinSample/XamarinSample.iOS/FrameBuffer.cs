using System;
using OpenTK.Graphics.ES30;

namespace XamarinSample.iOS
{
	public class FrameBuffer : IDisposable
    {
        private int frameBuffer;
        private GLTexture colorBuffer;
        private int renderBuffer;
        private int width;
        private int height;
        private int textureUnit;

        public FrameBuffer(int width, int height, int textureUnit)
		{
            this.width = width;
            this.height = height;
            this.textureUnit = textureUnit;

            GL.GenFramebuffers(1, out frameBuffer);
            GLCommon.GLError();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GLCommon.GLError();

            colorBuffer = new GLTexture(width, height, textureUnit);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.Texture2D, colorBuffer.TextureID, 0);
            GLCommon.GLError();

            GL.GenRenderbuffers(1, out renderBuffer);
            GLCommon.GLError();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBuffer);
            GLCommon.GLError();
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.Depth24Stencil8, width, height);
            GLCommon.GLError();
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderBuffer);
            GLCommon.GLError();

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");
            }

            GLCommon.SetDefaultFrameBuffer();
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
            }

            // 非管理（unmanaged）リソースの破棄処理をここに記述します。
            GL.DeleteFramebuffers(1, ref frameBuffer);
            GLCommon.GLError();
            GL.DeleteRenderbuffers(1, ref renderBuffer);
            GLCommon.GLError();
            colorBuffer.Dispose();
            colorBuffer = null;
        }

        ~FrameBuffer()
        {
            Dispose(false);
        }

        public void SetFrameBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GLCommon.GLError();
        }

        public void UseTexture()
        {
            colorBuffer.UseTexture();
        }
    }
}
