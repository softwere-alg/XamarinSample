using System;
using GLKit;
using System.Diagnostics;
using OpenGLES;
using XamarinSample.CustomControl;
using XamarinSample.iOS.CustomControl;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using Xamarin.Essentials;
using System.ComponentModel;
using UIKit;

[assembly: ExportRenderer (typeof(DrawView), typeof(DrawViewRenderer))]
namespace XamarinSample.iOS.CustomControl
{
	public class DrawViewRenderer : ViewRenderer<DrawView, GLKView>, IGLKViewDelegate
    {
        /// <summary>
        /// OpenGL ESのコンテキスト
        /// </summary>
        private EAGLContext Context { get; set; }

        /// <summary>
        /// OpenGLビュー
        /// </summary>
        private GLKView glkView;

        private RenderControl renderControl;

        private bool doubleDisplay;
        private bool leftDisplay;
        private bool rightDisplay;
        private bool leftInvert;
        private bool rightInvert;

        protected override void OnElementChanged(ElementChangedEventArgs<DrawView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    glkView = new GLKView();
                    SetNativeControl(glkView);

                    // コンテキストの作成
                    Context = new EAGLContext(EAGLRenderingAPI.OpenGLES2);

                    if (Context == null)
                    {
                        Debug.WriteLine("Failed to create ES context");
                    }

                    // Viewの設定
                    glkView.Context = Context;
                    glkView.Delegate = this;

                    this.Element.SizeChanged += Element_SizeChanged;

                    // 使用するコンテキストの指定
                    EAGLContext.SetCurrentContext(Context);

                    GLCommon.Initialize(glkView);

                    Rectangle rectangle = this.Element.Bounds;

                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.AutoReset = true;
                    timer.Interval = 1000f / 30;
                    timer.Elapsed += Timer_Elapsed;
                    timer.Start();
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            doubleDisplay = ((DrawView)sender).Double;
            leftDisplay = ((DrawView)sender).Left;
            rightDisplay = ((DrawView)sender).Right;
            leftInvert = ((DrawView)sender).LeftInvert;
            rightInvert = ((DrawView)sender).RightInvert;
        }

        private void Element_SizeChanged(object sender, EventArgs e)
        {
            Rectangle rectangle = this.Element.Bounds;
            //GL.Viewport(0, 0, (int)rectangle.Width, (int)rectangle.Height);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(glkView.Display);
        }

        public void DrawInRect(GLKView view, CGRect rect)
        {
            if (renderControl == null)
            {
                renderControl = new RenderControl();
            }

            renderControl.Draw((int)(rect.Width * UIScreen.MainScreen.Scale), (int)(rect.Height * UIScreen.MainScreen.Scale), doubleDisplay, leftDisplay, rightDisplay, leftInvert, rightInvert);
        }
    }
}
