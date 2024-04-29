using System;
using OpenTK.Graphics.ES30;
using SkiaSharp;

namespace XamarinSample.iOS
{
	public class GLTexture : IDisposable
    {
        /// <summary>
        /// テクスチャID
        /// </summary>
        public int TextureID { get; private set; }

        /// <summary>
        /// テクスチャユニット
        /// </summary>
        private int textureUnit;

        /// <summary>
        /// テクスチャ幅
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// テクスチャ高さ
        /// </summary>
        public int Height { get; private set; }

        public GLTexture(int width, int height, int textureUnit)
		{
            this.textureUnit = textureUnit;
            Width = width;
            Height = height;

            GL.ActiveTexture(TextureUnit.Texture0 + TextureID);
            GLCommon.GLError();

            // テスクチャ番号取得
            TextureID = GL.GenTexture();
            GLCommon.GLError();

            // テスクチャの指定
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GLCommon.GLError();

            // テスクチャパラメータ指定
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GLCommon.GLError();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GLCommon.GLError();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GLCommon.GLError();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GLCommon.GLError();

            // テスクチャの初期化
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)0);
            GLCommon.GLError();

            // テスクチャの指定解除
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GLCommon.GLError();
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
            // テクスチャの削除
            GL.DeleteTexture(TextureID);
            GLCommon.GLError();
        }

        ~GLTexture()
        {
            Dispose(false);
        }

        /// <summary>
        /// テスクチャの使用を通知します。
        /// </summary>
        public void UseTexture()
        {
            // テクスチャを指定
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GLCommon.GLError();
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GLCommon.GLError();
            GLCommon.SetTextureUniform(textureUnit);
        }

        /// <summary>
        /// テスクチャにビットマップを設定します。
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        public void SetTexture(SKBitmap bitmap)
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GLCommon.GLError();

            // サイズが違う場合は、テスクチャの初期化してからビットマップを設定
            if (Width != bitmap.Width || Height != bitmap.Height)
            {
                Width = bitmap.Width;
                Height = bitmap.Height;
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bitmap.GetPixels());
                GLCommon.GLError();
            }
            // サイズが同じ場合は、テスクチャにビットマップを設定
            else
            {
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height, PixelFormat.Rgba, PixelType.UnsignedByte, bitmap.GetPixels());
                GLCommon.GLError();
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GLCommon.GLError();
        }

        /// <summary>
        /// テスクチャを取得します。
        /// </summary>
        public void GetTexture()
        {
            byte[] bytes = new byte[Width * Height * 4];

            int fbo;
            GL.GenFramebuffers(1, out fbo);
            GLCommon.GLError();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GLCommon.GLError();
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.Texture2D, TextureID, 0);
            GLCommon.GLError();

            GL.ReadPixels(0, 0, Width, Height, PixelFormat.Rgba, PixelType.UnsignedByte, bytes);
            GLCommon.GLError();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GLCommon.GLError();
            GL.DeleteFramebuffers(1, new int[] { fbo });
            GLCommon.GLError();
        }
    }
}
