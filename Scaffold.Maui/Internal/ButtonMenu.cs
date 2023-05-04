using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Internal
{
    public class ButtonMenu : ButtonSam.Maui.Button
    {
        // image source
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
            nameof(ImageSource), 
            typeof(ImageSource),
            typeof(ButtonMenu),
            null,
            propertyChanged: Update
        );
        public ImageSource? ImageSource
        {
            get => GetValue(ImageSourceProperty) as ImageSource;
            set => SetValue(ImageSourceProperty, value);
        }

        // image color
        public static readonly BindableProperty ImageColorProperty = BindableProperty.Create(
            nameof(ImageColor),
            typeof(Color),
            typeof(ButtonMenu),
            null,
            propertyChanged:(b,o,n) =>
            {
                if (b is ButtonMenu self && self.Content is ImageTint img)
                    img.TintColor = n as Color;
            }
        );
        public Color? ImageColor
        {
            get => GetValue(ImageColorProperty) as Color;
            set => SetValue(ImageColorProperty, value);
        }

        // text
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(ButtonMenu),
            null, 
            propertyChanged: Update
        );
        public string? Text
        {
            get => GetValue(TextProperty) as string;
            set => SetValue(TextProperty, value);
        }

        private static void Update(BindableObject b, object old, object newest)
        {
            if (b is ButtonMenu self)
            {
                self.Update();
            }
        }

        private void Update()
        {
            if (ImageSource != null)
            {
                Padding = new Thickness(5);
                BackgroundColor = Colors.Transparent;
                Content = new ImageTint
                {
                    WidthRequest = 26,
                    HeightRequest = 26,
                    Source = ImageSource,
                    TintColor = ImageColor,
                };
                CornerRadius = 18;
            }
            else
            {
                Padding = new Thickness(10);
                Content = new Label
                {
                    Text = Text,
                };
                CornerRadius = 8;
            }
        }
    }
}
