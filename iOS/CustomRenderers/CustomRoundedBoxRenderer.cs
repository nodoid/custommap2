using System;
using custommap2;
using Xamarin.Forms;
using custommap2.iOS;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomRoundedBox), typeof(CustomRoundedBoxRenderer))]
namespace custommap2.iOS
{
    public class CustomRoundedBoxRenderer : BoxRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                Layer.MasksToBounds = true;
                UpdateCornerRadius(Element as CustomRoundedBox);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CustomRoundedBox.CornerRadiusProperty.PropertyName)
            {
                UpdateCornerRadius(Element as CustomRoundedBox);
            }
        }

        void UpdateCornerRadius(CustomRoundedBox box)
        {
            Layer.CornerRadius = (float)box.CornerRadius;
        }
    }
}

