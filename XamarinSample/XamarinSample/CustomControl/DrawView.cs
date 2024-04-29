using System;
using Xamarin.Forms;

namespace XamarinSample.CustomControl
{
	public class DrawView : View
	{
        public static readonly BindableProperty DoubleProperty = BindableProperty.Create(
            propertyName: "Double",
            returnType: typeof(bool),
            declaringType: typeof(DrawView),
            defaultValue: false);

        public static readonly BindableProperty LeftProperty = BindableProperty.Create(
            propertyName: "Left",
            returnType: typeof(bool),
            declaringType: typeof(DrawView),
            defaultValue: false);

        public static readonly BindableProperty RightProperry = BindableProperty.Create(
            propertyName: "Right",
            returnType: typeof(bool),
            declaringType: typeof(DrawView),
            defaultValue: false);

        public static readonly BindableProperty LeftInvertProperty = BindableProperty.Create(
            propertyName: "LeftInvert",
            returnType: typeof(bool),
            declaringType: typeof(DrawView),
            defaultValue: false);

        public static readonly BindableProperty RightInvertProperry = BindableProperty.Create(
            propertyName: "RightInvert",
            returnType: typeof(bool),
            declaringType: typeof(DrawView),
            defaultValue: false);

        public bool Double
        {
            get { return (bool)GetValue(DoubleProperty); }
            set { SetValue(DoubleProperty, value); }
        }

        public bool Left
        {
            get { return (bool)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public bool Right
        {
            get { return (bool)GetValue(RightProperry); }
            set { SetValue(RightProperry, value); }
        }

        public bool LeftInvert
        {
            get { return (bool)GetValue(LeftInvertProperty); }
            set { SetValue(LeftInvertProperty, value); }
        }

        public bool RightInvert
        {
            get { return (bool)GetValue(RightInvertProperry); }
            set { SetValue(RightInvertProperry, value); }
        }
    }
}
