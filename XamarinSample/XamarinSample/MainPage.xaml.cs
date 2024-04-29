using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            drawView.Double = doubleCheck.IsChecked;
            drawView.Left = leftCheck.IsChecked;
            drawView.Right = rightCheck.IsChecked;
            drawView.LeftInvert = leftInvertCheck.IsChecked;
            drawView.RightInvert = rightInvertCheck.IsChecked;
        }

        void doubleCheck_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            drawView.Double = doubleCheck.IsChecked;
        }

        void leftCheck_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            drawView.Left = leftCheck.IsChecked;

            if (leftCheck.IsChecked)
            {
                ImageManager.ImageManagers[0].StartUpdate();
            }
            else
            {
                ImageManager.ImageManagers[0].StopUpdate();
            }
        }

        void rightCheck_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            drawView.Right = rightCheck.IsChecked;

            if (rightCheck.IsChecked)
            {
                ImageManager.ImageManagers[1].StartUpdate();
            }
            else
            {
                ImageManager.ImageManagers[1].StopUpdate();
            }
        }

        void leftInvertCheck_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            drawView.LeftInvert = leftInvertCheck.IsChecked;
        }

        void rightInvertCheck_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            drawView.RightInvert = rightInvertCheck.IsChecked;
        }
    }
}
