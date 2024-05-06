using System;
using System.Runtime.InteropServices;
using Metal;
using OpenTK;

namespace XamarinSample.iOS
{
	public class Rectangular : IDisposable
	{
        #region 定数データ
        /// <summary>
        /// 四角形のための頂点座標データ（固定）
        /// </summary>
        private static readonly MTLCommon.VertexAttribute[] vertexData = {
            new MTLCommon.VertexAttribute() { // 左下
                Position = new Vector3(-1.0f, -1.0f, 0.0f), TextureCoordinate = new Vector2(0.0f, 1.0f)
            },
            new MTLCommon.VertexAttribute() { // 右下
                Position = new Vector3( 1.0f, -1.0f, 0.0f), TextureCoordinate = new Vector2(1.0f, 1.0f)
            },
            new MTLCommon.VertexAttribute() { // 左上
                Position = new Vector3(-1.0f,  1.0f, 0.0f), TextureCoordinate = new Vector2(0.0f, 0.0f)
            },
            new MTLCommon.VertexAttribute() { // 右上
                Position = new Vector3( 1.0f,  1.0f, 0.0f), TextureCoordinate = new Vector2(1.0f, 0.0f)
            }
        };
        #endregion

        /// <summary>
        /// 頂点データバッファ
        /// </summary>
        private IMTLBuffer vertexBuffer;

        public Rectangular(IMTLDevice device)
		{
            // 頂点バッファの作成
            vertexBuffer = device.CreateBuffer((nuint)(Marshal.SizeOf(typeof(MTLCommon.VertexAttribute)) * vertexData.Length), MTLResourceOptions.CpuCacheModeDefault);
            vertexBuffer.Label = "Vertices";

            // 頂点バッファにデータコピー
            MTLCommon.CopyToBuffer(vertexData, vertexBuffer);
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
                // 頂点バッファオブジェクトの削除
                vertexBuffer = null;
            }

            // 非管理（unmanaged）リソースの破棄処理をここに記述します。
        }

        ~Rectangular()
        {
            Dispose(false);
        }

        /// <summary>
        /// 描画処理を行います。
        /// </summary>
        /// <param name="loadAction"></param>
        /// <param name="storeAction"></param>
        public void Draw(MTLLoadAction loadAction = MTLLoadAction.Load, MTLStoreAction storeAction = MTLStoreAction.Store)
        {
            MTLCommon.SetVertexBuffer(vertexBuffer);
            MTLCommon.DrawPrimitives(MTLPrimitiveType.TriangleStrip, 0, 4, loadAction, storeAction);
        }
    }
}
