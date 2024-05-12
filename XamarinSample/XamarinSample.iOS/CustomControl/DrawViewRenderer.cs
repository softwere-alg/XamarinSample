using System;
using XamarinSample.CustomControl;
using XamarinSample.iOS.CustomControl;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using System.ComponentModel;
using MetalKit;
using Metal;

[assembly: ExportRenderer (typeof(DrawView), typeof(DrawViewRenderer))]
namespace XamarinSample.iOS.CustomControl
{
	public class DrawViewRenderer : ViewRenderer<DrawView, MTKView>, IMTKViewDelegate
    {
        /// <summary>
        /// Metalビュー
        /// </summary>
        private MTKView mtkView;

        private IMTLDevice device;

        private CGSize size;

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
                    device = MTLDevice.SystemDefault;

                    mtkView = new MTKView();
                    SetNativeControl(mtkView);


                    // Viewの設定
                    mtkView.ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
                    mtkView.DepthStencilPixelFormat = MTLPixelFormat.Depth32Float;
                    mtkView.ClearDepth = 1.0;
                    mtkView.Delegate = this;

                    this.Element.SizeChanged += Element_SizeChanged;

                    MTLCommon.Initialize(mtkView, device);

                    Rectangle rectangle = this.Element.Bounds;

                    // System.Timers.Timer timer = new System.Timers.Timer();
                    // timer.AutoReset = true;
                    // timer.Interval = 1000f / 30;
                    // timer.Elapsed += Timer_Elapsed;
                    // timer.Start();
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
            //MainThread.BeginInvokeOnMainThread(glkView.Display);
        }

        public void DrawableSizeWillChange(MTKView view, CGSize size)
        {
            this.size = size;
        }

        public void Draw(MTKView view)
        {
            if (renderControl == null)
            {
                renderControl = new RenderControl(device);
            }

            renderControl.Draw((int)size.Width, (int)size.Height, doubleDisplay, leftDisplay, rightDisplay, leftInvert, rightInvert);
        }
    }
}
