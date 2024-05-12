using System;
using System.Runtime.InteropServices;
using Foundation;
using Metal;
using MetalKit;
using OpenTK;
using Xamarin.Forms;

namespace XamarinSample.iOS
{
    public static class MTLCommon
    {
        /// <summary>
        /// 頂点データの構造体を定義します。
        /// </summary>
        public struct VertexAttribute
        {
            public Vector3 Position;            // 頂点位置
            public Vector2 TextureCoordinate;   // テクスチャ座標
        }

        /// <summary>
        /// 頂点データ以外の構造体を定義します。
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct Uniform
        {
            [FieldOffset(0)]
            public Vector2i ViewportSize;   // ビューポートサイズ
            [FieldOffset(16)]
            public Matrix4 ModelMatrix;     // モデル行列 16byte目から配置されるように指定
            [FieldOffset(80)]
            public Matrix4 ViewMatrix;      // ビュー行列 80byte目から配置されるように指定
        }

        /// <summary>
        /// バッファ番号を定義します。
        /// </summary>
        public enum Buffers
        {
            VertexAttributeIndex = 0,   // 頂点データ
            UniformIndex                // ユニフォームデータ
        }

        /// <summary>
        /// テスクチャ番号を定義します。
        /// </summary>
        public enum Textures
        {
            DisplayTextureIndex = 0,    // 表示するテクスチャ
        }

        #region メンバ変数
        /// <summary>
        /// ビュー
        /// </summary>
        private static MTKView view;

        /// <summary>
        /// 使用するGPU
        /// </summary>
        private static IMTLDevice device;

        /// <summary>
        /// コマンドキュー
        /// </summary>
        private static IMTLCommandQueue commandQueue;

        /// <summary>
        /// パイプラインステート
        /// </summary>
        private static IMTLRenderPipelineState pipelineState;

        /// <summary>
        /// ユニフォームデータ
        /// </summary>
        private static Uniform uniform = new Uniform();

        /// <summary>
        /// 頂点データバッファ
        /// </summary>
        private static IMTLBuffer vertexBuffer;

        /// <summary>
        /// 入力テクスチャ
        /// </summary>
        private static MTLTexture inputTexture;

        /// <summary>
        /// フレームバッファテクスチャ
        /// </summary>
        private static MTLTexture frameBufferTexture;

        /// <summary>
        /// クリアカラー
        /// </summary>
        private static Color clearColor = Color.Black;

        private static bool reservedPresent = false;
        #endregion

        public static void Initialize(MTKView view, IMTLDevice device)
        {
            // 使用するGPUの選択
            MTLCommon.device = device;

            MTLCommon.view = view;
            MTLCommon.view.Device = device;

            // コマンドキューの作成
            commandQueue = device.CreateCommandQueue();

            // シェーダのロード
            LoadShaders();
        }

        public static void Release()
        {
        }

        /// <summary>
        /// シェーダをロードします。
        /// </summary>
        /// <returns></returns>
        private static bool LoadShaders()
        {
            // プロジェクト内のmetal拡張子のシェーダファイルを全て読み込む
            NSError error;
            IMTLLibrary defaultLibrary = device.CreateLibrary("default.metallib", out error);
            if (error != null)
            {
                Console.WriteLine("Failed to created library, error " + error);
                return false;
            }

            // 頂点シェーダの読み込み
            IMTLFunction vertexProgram = defaultLibrary.CreateFunction("sample_vertex");
            // フラグメントシェーダの読み込み
            IMTLFunction fragmentProgram = defaultLibrary.CreateFunction("sample_fragment");

            // パイプラインステートの作成
            MTLRenderPipelineDescriptor pipelineStateDescriptor = new MTLRenderPipelineDescriptor
            {
                Label = "MyPipeline",
                SampleCount = 1,
                VertexFunction = vertexProgram,
                FragmentFunction = fragmentProgram,
            };
            pipelineStateDescriptor.DepthAttachmentPixelFormat = MTLPixelFormat.Depth32Float;
            pipelineStateDescriptor.ColorAttachments[0].PixelFormat = MTLPixelFormat.BGRA8Unorm;
            pipelineStateDescriptor.ColorAttachments[0].BlendingEnabled = true;
            pipelineStateDescriptor.ColorAttachments[0].AlphaBlendOperation = MTLBlendOperation.Add;
            pipelineStateDescriptor.ColorAttachments[0].RgbBlendOperation = MTLBlendOperation.Add;
            pipelineStateDescriptor.ColorAttachments[0].SourceAlphaBlendFactor = MTLBlendFactor.SourceAlpha;
            pipelineStateDescriptor.ColorAttachments[0].SourceRgbBlendFactor = MTLBlendFactor.SourceAlpha;
            pipelineStateDescriptor.ColorAttachments[0].DestinationAlphaBlendFactor = MTLBlendFactor.OneMinusSourceAlpha;
            pipelineStateDescriptor.ColorAttachments[0].DestinationRgbBlendFactor = MTLBlendFactor.OneMinusSourceAlpha;
            pipelineState = device.CreateRenderPipelineState(pipelineStateDescriptor, out error);

            if (pipelineState == null)
            {
                Console.WriteLine("Failed to created pipeline state, error " + error);
                return false;
            }

            return true;
        }

        public static void DrawPrimitives(MTLPrimitiveType primitiveType, nuint vertexStart, nuint vertexCount, MTLLoadAction loadAction = MTLLoadAction.Load, MTLStoreAction storeAction = MTLStoreAction.Store)
        {
            // コマンドバッファを作成
            IMTLCommandBuffer commandBuffer = commandQueue.CommandBuffer();
            commandBuffer.Label = "MyCommand";

            if (commandBuffer == null)
            {
                Console.WriteLine("commandBuffer is null");
                return;
            }

            MTLRenderPassDescriptor renderPassDescriptor = view.CurrentRenderPassDescriptor;
            if (frameBufferTexture == null)
            {
                //renderPassDescriptor = view.CurrentRenderPassDescriptor;
            }
            else
            {
                //renderPassDescriptor = new MTLRenderPassDescriptor();
                renderPassDescriptor.ColorAttachments[0].Texture = frameBufferTexture.Texture;
            }
            renderPassDescriptor.ColorAttachments[0].ClearColor = new MTLClearColor(clearColor.R, clearColor.G, clearColor.B, clearColor.A);
            renderPassDescriptor.ColorAttachments[0].LoadAction = loadAction;
            renderPassDescriptor.ColorAttachments[0].StoreAction = storeAction;
            //renderPassDescriptor.RenderTargetWidth = (nuint)uniform.ViewportSize.X;
            //renderPassDescriptor.RenderTargetHeight = (nuint)uniform.ViewportSize.Y;

            // レンダーコマンドエンコーダを作成
            IMTLRenderCommandEncoder renderEncoder = commandBuffer.CreateRenderCommandEncoder(renderPassDescriptor);
            renderEncoder.Label = "MyRenderEncoder";

            renderEncoder.SetCullMode(MTLCullMode.None);

            // パイプラインステート指定
            renderEncoder.SetRenderPipelineState(pipelineState);

            // 頂点データバッファ指定
            renderEncoder.SetVertexBuffer(vertexBuffer, 0, (uint)Buffers.VertexAttributeIndex);
            // ユニフォームデータ設定
            GetUnmanagedMemory(uniform, (IntPtr ptr, int length) => renderEncoder.SetVertexBytes(ptr, (nuint)length, (uint)Buffers.UniformIndex));
            // テクスチャ指定
            inputTexture.UseTexture(renderEncoder);

            // プリミティブを指定
            renderEncoder.DrawPrimitives(primitiveType, vertexStart, vertexCount);

            // エンコード終了を通知
            renderEncoder.EndEncoding();

            if (reservedPresent == true)
            {
                // コマンド完了後の表示をスケジュールする
                commandBuffer.PresentDrawable(view.CurrentDrawable);
                reservedPresent = false;
            }

            // コマンドバッファをGPUに送る
            commandBuffer.Commit();
        }

        public static void ReservePresent()
        {
            reservedPresent = true;
        }

        public static void SetDefaultFrameBuffer()
        {
            frameBufferTexture = null;
        }

        public static void SetFrameBuffer(MTLTexture texture)
        {
            frameBufferTexture = texture;
        }

        public static void SetViewport(int width, int height)
        {
            uniform.ViewportSize = new Vector2i(width, height);
        }

        public static void SetModel(float scaleX, float scaleY, bool invert = false)
        {
            Matrix4 translationMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            Matrix4 rotationMatrix = invert ? Matrix4.CreateRotationY((float)Math.PI) : Matrix4.CreateRotationY(0.0f);
            Matrix4 scaleMatrix = Matrix4.Scale((float)scaleX, (float)scaleY, 1.0f);

            Matrix4 modelMatrix = translationMatrix * rotationMatrix * scaleMatrix;

            uniform.ModelMatrix = modelMatrix;
        }

        public static void SetView(float x, float y)
        {
            Matrix4 viewMatrix = Matrix4.CreateTranslation(x, y, 0.0f);

            uniform.ViewMatrix = viewMatrix;
        }

        public static void SetVertexBuffer(IMTLBuffer vertexBuffer)
        {
            MTLCommon.vertexBuffer = vertexBuffer;
        }

        public static void SetTexture(MTLTexture texture)
        {
            inputTexture = texture;
        }

        public static void SetClearColor(Color color)
        {
            clearColor = color;
        }

        /// <summary>
        /// 構造体をバッファにコピーします。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="st">構造体</param>
        /// <param name="buffer">バッファ</param>
        public static void CopyToBuffer<T>(T st, IMTLBuffer buffer) where T : struct
        {
            // 構造体のサイズ取得
            int rawsize = Marshal.SizeOf(typeof(T));

            // マネージドバイト配列確保
            byte[] rawdata = new byte[rawsize];
            // アンマネージドメモリ確保
            IntPtr ptr = Marshal.AllocHGlobal(rawsize);

            // 構造体データをアンマネージドメモリにコピー
            Marshal.StructureToPtr(st, ptr, false);
            // アンマネージドメモリからバイト配列にコピー
            Marshal.Copy(ptr, rawdata, 0, rawsize);

            // アンマネージドメモリ解放
            Marshal.FreeHGlobal(ptr);

            // バイト配列からバッファにデータをコピー
            Marshal.Copy(rawdata, 0, buffer.Contents, rawsize);
        }

        /// <summary>
        /// 構造体配列をバッファにコピーします。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="st">構造体配列</param>
        /// <param name="buffer">バッファ</param>
        public static void CopyToBuffer<T>(T[] st, IMTLBuffer buffer) where T : struct
        {
            // 構造体のサイズ取得
            int typesize = Marshal.SizeOf(typeof(T));
            // 全体のサイズ取得
            int rawsize = typesize * st.Length;

            // マネージドバイト配列確保
            byte[] rawdata = new byte[rawsize];
            // アンマネージドメモリ確保
            IntPtr ptr = Marshal.AllocHGlobal(typesize);

            for (int i = 0; i < st.Length; i++)
            {
                // 構造体データをアンマネージドメモリにコピー
                Marshal.StructureToPtr(st[i], ptr, false);
                // アンマネージドメモリからバイト配列にコピー
                Marshal.Copy(ptr, rawdata, typesize * i, typesize);
            }

            // アンマネージドメモリ解放
            Marshal.FreeHGlobal(ptr);

            // バイト配列からバッファにデータをコピー
            Marshal.Copy(rawdata, 0, buffer.Contents, rawsize);
        }

        /// <summary>
        /// 構造体のアンマネージドメモリを取得します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="st">構造体</param>
        /// <param name="buffer">バッファ</param>
        public static void GetUnmanagedMemory<T>(T st, Action<IntPtr, int> action) where T : struct
        {
            // 構造体のサイズ取得
            int rawsize = Marshal.SizeOf(typeof(T));

            // アンマネージドメモリ確保
            IntPtr ptr = Marshal.AllocHGlobal(rawsize);

            // 構造体データをアンマネージドメモリにコピー
            Marshal.StructureToPtr(st, ptr, false);

            // コールバック呼び出し
            action(ptr, rawsize);

            // アンマネージドメモリ解放
            Marshal.FreeHGlobal(ptr);
        }
    }
}
