using System;
using OpenTK.Graphics.ES30;

namespace XamarinSample.iOS
{
	public class Rectangular : IDisposable
	{
        #region 定数データ
        /// <summary>
        /// 四角形のための頂点座標データ（固定）
        /// </summary>
        private static readonly float[] vertexData = {
			// positionX, positionY, positionZ, textureX, textureY
            -1.0f, -1.0f, 0.0f, 0.0f, 1.0f, // 左下
             1.0f, -1.0f, 0.0f, 1.0f, 1.0f, // 右下
            -1.0f,  1.0f, 0.0f, 0.0f, 0.0f, // 左上
             1.0f,  1.0f, 0.0f, 1.0f, 0.0f  // 右上
        };
        #endregion

        /// <summary>
        /// 頂点配列オブジェクト
        /// </summary>
        private uint vertexArray;
        /// <summary>
        /// 頂点バッファオブジェクト
        /// </summary>
        private uint vertexBuffer;

        public Rectangular()
		{
            // 1つの頂点配列オブジェクトの生成
            GL.GenVertexArrays(1, out vertexArray);
            GLCommon.GLError();
            // 頂点配列オブジェクトの指定
            GL.BindVertexArray(vertexArray);
            GLCommon.GLError();

            // 1つの頂点バッファオブジェクトの生成
            GL.GenBuffers(1, out vertexBuffer);
            GLCommon.GLError();
            // 頂点バッファオブジェクトの指定
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GLCommon.GLError();
            // 頂点バッファオブジェクトに頂点データを渡す
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Length * sizeof(float)), vertexData, BufferUsage.StaticDraw);
            GLCommon.GLError();

            // 頂点データのデータ構造を指定
            GL.EnableVertexAttribArray((int)GLCommon.VertexAttribute.Position);
            GLCommon.GLError();
            GL.VertexAttribPointer((int)GLCommon.VertexAttribute.Position, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, new IntPtr(0));
            GLCommon.GLError();
            GL.EnableVertexAttribArray((int)GLCommon.VertexAttribute.TextureCoordinate);
            GLCommon.GLError();
            GL.VertexAttribPointer((int)GLCommon.VertexAttribute.TextureCoordinate, 2, VertexAttribPointerType.Float, false, sizeof(float) * 5, new IntPtr(3 * sizeof(float)));
            GLCommon.GLError();

            // 頂点配列オブジェクトの指定解除
            GL.BindVertexArray(0);
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
            // 頂点バッファオブジェクトの削除
            GL.DeleteBuffers(1, ref vertexBuffer);
            GLCommon.GLError();
            // 頂点配列オブジェクトの削除
            GL.DeleteVertexArrays(1, ref vertexArray);
            GLCommon.GLError();
        }

        ~Rectangular()
        {
            Dispose(false);
        }

        /// <summary>
        /// 描画処理を行います。
        /// </summary>
        public void Draw()
        {
            // 頂点配列オブジェクトを指定
            GL.BindVertexArray(vertexArray);
            GLCommon.GLError();

            // 描画を行う
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);
            GLCommon.GLError();

            // 頂点配列オブジェクトの指定解除
            GL.BindVertexArray(0);
            GLCommon.GLError();
        }
    }
}
