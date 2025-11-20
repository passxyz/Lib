using PassXYZLib;
using PassXYZLib.Resources;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace UITest.Views;

public partial class IconsPage : ContentPage
{
    Dictionary<string, string> glyphs;

    public IconsPage()
	{
		InitializeComponent();
        LoadIcons();
        Debug.WriteLine("Loading icons ...");
    }

    protected virtual void OnAppearing() 
    {
        base.OnAppearing();
    }

    void OnImageButtonClicked(object? sender, EventArgs e)
    {
        if (sender is ImageButton imageButton && imageButton.Source is FontImageSource fontImageSource)
        {
            var glyph = glyphs.FirstOrDefault(x => x.Value == fontImageSource.Glyph);
            Debug.WriteLine($"ImageButton clicked with Glyph: {glyph.Key}");
            selectedIcon.Text = $"The selected icon is {glyph.Key}.";
        }
        else
        {
            Debug.WriteLine("ImageButton clicked");
        }
    }

    async Task LoadIcons()
    {
        glyphs = FontData.GetGlyphs(typeof(FontAwesomeBrands));

        await Task.Run(async () =>
        {
            foreach (var glyph in glyphs)
            {
                StackLayout stackLayout = new StackLayout { Margin = new Thickness(10) };
                ImageButton image = new ImageButton
                {
                    Source = new FontImageSource
                    {
                        FontFamily = "FontAwesomeBrands",
                        Glyph = glyph.Value,
                        Size = 32,
                        Color = Microsoft.Maui.Graphics.Colors.Black
                    },

                };
                image.Clicked += OnImageButtonClicked;
                stackLayout.Children.Add(image);
                Label label = new Label
                {
                    Text = glyph.Key,
                    HorizontalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.TailTruncation,
                    WidthRequest = 64
                };
                stackLayout.Children.Add(label);
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    // Code to run on the main thread
                    flexLayout.Children.Add(stackLayout);
                });
            }
        });
    }
}