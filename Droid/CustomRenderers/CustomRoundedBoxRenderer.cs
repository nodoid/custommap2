using System;
using Xamarin.Forms;
using custommap2;
using custommap2.Droid;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Graphics;

[assembly: ExportRenderer(typeof(CustomRoundedBox), typeof(CustomRoundedBoxRenderer))]
namespace custommap2.Droid
{
    public class CustomRoundedBoxRenderer : BoxRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);

            SetWillNotDraw(false);

            Invalidate();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CustomRoundedBox.CornerRadiusProperty.PropertyName)
            {
                Invalidate();
            }
        }

        public override void Draw(Canvas canvas)
        {
            var box = Element as CustomRoundedBox;
            var rect = new Rect();
            var paint = new Paint
            {
                AntiAlias = true,
            };
            paint.SetARGB((int)box.BackgroundColor.A, (int)box.BackgroundColor.R, (int)box.BackgroundColor.G, (int)box.BackgroundColor.B);

            GetDrawingRect(rect);

            var radius = (float)(rect.Width() / box.Width * box.CornerRadius);

            canvas.DrawRoundRect(new RectF(rect), radius, radius, paint);
        }
    }
}

