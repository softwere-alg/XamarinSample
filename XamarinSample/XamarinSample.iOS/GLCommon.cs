using System;
using GLKit;
using OpenTK;
using OpenTK.Graphics.ES30;

namespace XamarinSample.iOS
{
    public static class GLCommon
    {
        /// <summary>
        /// 頂点データ番号を定義します。
        /// </summary>
        public enum VertexAttribute
        {
            Position = 0,       // 頂点位置
            TextureCoordinate   // テクスチャ座標
        }

        /// <summary>
        /// シェーダに送るデータ種類を定義します。
        /// </summary>
        public enum Uniform
        {
            ViewportSize = 0,   // ビューポートサイズ
            ModelMatrix,        // モデル行列
            ViewMatrix,         // ビュー行列
            Texture,            // テクスチャ
            Count
        }

        #region メンバ変数
        /// <summary>
        /// シェーダプログラム
        /// </summary>
        private static int program;

        /// <summary>
        /// ユニフォーム番号を保持します。
        /// </summary>
        private static int[] uniforms = new int[(int)Uniform.Count];

        private static GLKView view;
        #endregion

        public static void GLError()
        {
            ErrorCode errorCode = GL.GetErrorCode();
            if (errorCode != ErrorCode.NoError)
            {
                Console.WriteLine(errorCode);
            }
        }

        public static void Initialize(GLKView view)
        {
            GLCommon.view = view;

            // シェーダのロード
            LoadShaders();
        }

        public static void Release()
        {
            // シェーダの削除
            if (program > 0)
            {
                GL.DeleteProgram(program);
                GLCommon.GLError();
                program = 0;
            }
        }

        /// <summary>
        /// シェーダをロードします。
        /// </summary>
        /// <returns></returns>
        private static bool LoadShaders()
        {
            int vertShader, fragShader;

            // シェーダプログラムの作成
            program = GL.CreateProgram();
            GLCommon.GLError();

            // 頂点シェーダを作成
            if (!CompileShader(ShaderType.VertexShader, Application.LoadResource("Shader/Shader", "vsh"), out vertShader))
            {
                Console.WriteLine("Failed to compile vertex shader");
                return false;
            }
            // フラグメントシェーダを作成
            if (!CompileShader(ShaderType.FragmentShader, Application.LoadResource("Shader/Shader", "fsh"), out fragShader))
            {
                Console.WriteLine("Failed to compile fragment shader");
                return false;
            }

            // シェーダプログラムに頂点シェーダを登録する
            GL.AttachShader(program, vertShader);
            GLCommon.GLError();

            // シェーダプログラムにフラグメントシェーダを登録する
            GL.AttachShader(program, fragShader);
            GLCommon.GLError();

            // 頂点データの番号を指定
            // プログラムのリンク前に行う必要がある
            GL.BindAttribLocation(program, (int)VertexAttribute.Position, "position");
            GLCommon.GLError();
            GL.BindAttribLocation(program, (int)VertexAttribute.TextureCoordinate, "texCoordinate");
            GLCommon.GLError();

            // プログラムをリンクする
            if (!LinkProgram(program))
            {
                Console.WriteLine("Failed to link program: {0:x}", program);

                if (vertShader != 0)
                {
                    GL.DeleteShader(vertShader);
                    GLCommon.GLError();
                }

                if (fragShader != 0)
                {
                    GL.DeleteShader(fragShader);
                    GLCommon.GLError();
                }

                if (program != 0)
                {
                    GL.DeleteProgram(program);
                    GLCommon.GLError();
                    program = 0;
                }
                return false;
            }

            // ユニフォーム番号の取得
            uniforms[(int)Uniform.ViewportSize] = GL.GetUniformLocation(program, "viewportSize");
            GLCommon.GLError();
            uniforms[(int)Uniform.ModelMatrix] = GL.GetUniformLocation(program, "modelMatrix");
            GLCommon.GLError();
            uniforms[(int)Uniform.ViewMatrix] = GL.GetUniformLocation(program, "viewMatrix");
            GLCommon.GLError();
            uniforms[(int)Uniform.Texture] = GL.GetUniformLocation(program, "s_texture");
            GLCommon.GLError();

            // 一時オブジェクトの解放
            if (vertShader != 0)
            {
                GL.DetachShader(program, vertShader);
                GLCommon.GLError();
                GL.DeleteShader(vertShader);
                GLCommon.GLError();
            }
            if (fragShader != 0)
            {
                GL.DetachShader(program, fragShader);
                GLCommon.GLError();
                GL.DeleteShader(fragShader);
                GLCommon.GLError();
            }

            return true;
        }

        /// <summary>
        /// シェーダをコンパイルします。
        /// </summary>
        /// <param name="type">シェーダタイプ</param>
        /// <param name="src">コード</param>
        /// <param name="shader">シェーダ番号</param>
        /// <returns></returns>
        private static bool CompileShader(ShaderType type, string src, out int shader)
        {
            // シェーダ作成後、コンパイル
            shader = GL.CreateShader(type);
            GLCommon.GLError();
            GL.ShaderSource(shader, src);
            GLCommon.GLError();
            GL.CompileShader(shader);
            GLCommon.GLError();

#if DEBUG
            int logLength = 0;
            GL.GetShader(shader, ShaderParameter.InfoLogLength, out logLength);
            GLCommon.GLError();
            if (logLength > 0)
            {
                Console.WriteLine("Shader compile log:\n{0}", GL.GetShaderInfoLog(shader));
            }
#endif

            // シェーダ番号取得
            int status = 0;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            GLCommon.GLError();
            if (status == 0)
            {
                GL.DeleteShader(shader);
                GLCommon.GLError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// プログラムをリンクします。
        /// </summary>
        /// <param name="prog">プログラム番号</param>
        /// <returns></returns>
        private static bool LinkProgram(int prog)
        {
            GL.LinkProgram(prog);
            GLCommon.GLError();

#if DEBUG
            int logLength = 0;
            GL.GetProgram(prog, ProgramParameter.InfoLogLength, out logLength);
            GLCommon.GLError();
            if (logLength > 0)
            {
                Console.WriteLine("Program link log:\n{0}", GL.GetProgramInfoLog(prog));
                GLCommon.GLError();
            }
#endif
            int status = 0;
            GL.GetProgram(prog, ProgramParameter.LinkStatus, out status);
            GLCommon.GLError();
            return status != 0;
        }

        public static void UseProgram()
        {
            // シェーダプログラムを指定
            GL.UseProgram(program);
            GLCommon.GLError();
        }

        public static void SetDefaultFrameBuffer()
        {
            view.BindDrawable();
        }

        public static void SetViewport(int width, int height)
        {
            Vector2i viewportSize = new Vector2i(width, height);
            GL.Uniform2(uniforms[(int)Uniform.ViewportSize], viewportSize.X, viewportSize.Y);
            GLCommon.GLError();
        }

        public static void SetModel(float scaleX, float scaleY, bool invert = false)
        {
            Matrix4 translationMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            Matrix4 rotationMatrix = invert ? Matrix4.CreateRotationY((float)Math.PI) : Matrix4.CreateRotationY(0.0f);
            Matrix4 scaleMatrix = Matrix4.Scale((float)scaleX, (float)scaleY, 1.0f);

            Matrix4 modelMatrix = translationMatrix * rotationMatrix * scaleMatrix;
            GL.UniformMatrix4(uniforms[(int)Uniform.ModelMatrix], false, ref modelMatrix);
            GLCommon.GLError();
        }

        public static void SetView(float x, float y)
        {
            Matrix4 viewMatrix = Matrix4.CreateTranslation(x, y, 0.0f);
            GL.UniformMatrix4(uniforms[(int)Uniform.ViewMatrix], false, ref viewMatrix);
            GLCommon.GLError();
        }

        public static void SetTextureUniform(int textureUnit)
        {
            GL.Uniform1(uniforms[(int)Uniform.Texture], textureUnit);
            GLCommon.GLError();
        }
    }
}
