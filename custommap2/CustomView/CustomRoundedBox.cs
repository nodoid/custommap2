using Xamarin.Forms;

namespace custommap2
{
    public class CustomRoundedBox : BoxView
    {
        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create("CornerRadius", typeof(double), typeof(CustomRoundedBox), 0.0);

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public CustomRoundedBox()
        {
        }
    }
}

