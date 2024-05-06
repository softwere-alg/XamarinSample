using System;
using Metal;

namespace XamarinSample.iOS
{
	public class FrameBuffer : IDisposable
    {
        private MTLTexture colorBuffer;
        private int width;
        private int height;
        private int textureUnit;

        public FrameBuffer(int width, int height, int textureUnit, IMTLDevice device)
		{
            this.width = width;
            this.height = height;
            this.textureUnit = textureUnit;

            colorBuffer = new MTLTexture(width, height, textureUnit, device);
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
                colorBuffer = null;
            }

            // 非管理（unmanaged）リソースの破棄処理をここに記述します。
        }

        ~FrameBuffer()
        {
            Dispose(false);
        }

        public void SetFrameBuffer()
        {
            MTLCommon.SetFrameBuffer(colorBuffer);
        }

        public void SetTexture()
        {
            MTLCommon.SetTexture(colorBuffer);
        }

        public void UseTexture(IMTLRenderCommandEncoder renderEncoder)
        {
            colorBuffer.UseTexture(renderEncoder);
        }
    }
}
