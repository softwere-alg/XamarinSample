using System;
using System.Runtime.InteropServices;
using SkiaSharp;
using Metal;

namespace XamarinSample.iOS
{
	public class MTLTexture : IDisposable
    {
        /// <summary>
        /// テクスチャ
        /// </summary>
        public IMTLTexture Texture { get; private set; }

        /// <summary>
        /// サンプラー
        /// </summary>
        public IMTLSamplerState Sampler { get; private set; }

        /// <summary>
        /// テクスチャ幅
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// テクスチャ高さ
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// テスクチャインデックス
        /// </summary>
        private uint index;

        public MTLTexture(int width, int height, int index, IMTLDevice device, bool frameBuffer)
		{
            Width = width;
            Height = height;
            this.index = (uint)index;

            CreateTexture(device, frameBuffer);

            MTLSamplerDescriptor samplerDescriptor = new MTLSamplerDescriptor();

            samplerDescriptor.SAddressMode = MTLSamplerAddressMode.ClampToEdge;
            samplerDescriptor.TAddressMode = MTLSamplerAddressMode.ClampToEdge;
            samplerDescriptor.MinFilter = MTLSamplerMinMagFilter.Linear;
            samplerDescriptor.MagFilter = MTLSamplerMinMagFilter.Linear;

            Sampler = device.CreateSamplerState(samplerDescriptor);
        }

        private void CreateTexture(IMTLDevice device, bool frameBuffer)
        {
            MTLTextureDescriptor textureDescriptor = new MTLTextureDescriptor();

            textureDescriptor.PixelFormat = MTLPixelFormat.BGRA8Unorm;
            textureDescriptor.Width = (nuint)Width;
            textureDescriptor.Height = (nuint)Height;
            textureDescriptor.Usage = MTLTextureUsage.ShaderRead;
            if (frameBuffer)
            {
                textureDescriptor.Usage |= MTLTextureUsage.RenderTarget;
            }

            Texture = device.CreateTexture(textureDescriptor);
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
                // テクスチャの削除
                Texture = null;
                Sampler = null;
            }

            // 非管理（unmanaged）リソースの破棄処理をここに記述します。
        }

        ~MTLTexture()
        {
            Dispose(false);
        }

        /// <summary>
        /// テスクチャの使用を通知します。
        /// </summary>
        /// <param name="renderEncoder">エンコーダ</param>
        public void UseTexture(IMTLRenderCommandEncoder renderEncoder)
        {
            renderEncoder.SetFragmentTexture(Texture, index);
            renderEncoder.SetFragmentSamplerState(Sampler, index);
        }

        /// <summary>
        /// テスクチャにビットマップを設定します。
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        public void SetTexture(SKBitmap bitmap)
        {
            // サイズが違う場合は、テスクチャの初期化
            if (Width != bitmap.Width || Height != bitmap.Height)
            {
                Width = bitmap.Width;
                Height = bitmap.Height;
                CreateTexture(Texture.Device, (Texture.GetUsage() & MTLTextureUsage.RenderTarget) == MTLTextureUsage.RenderTarget);
            }

            Texture.ReplaceRegion(MTLRegion.Create2D(0, 0, Width, Height), 0, bitmap.GetPixels(), (nuint)(4 * Width));
        }

        /// <summary>
        /// テスクチャを取得します。
        /// </summary>
        public void GetTexture()
        {
            IntPtr ptr = Marshal.AllocHGlobal(Width * Height * 4);

            Texture.GetBytes(ptr, (nuint)(4 * Width), MTLRegion.Create2D(0, 0, Width, Height), 0);

            Marshal.FreeHGlobal(ptr);
        }
    }
}
