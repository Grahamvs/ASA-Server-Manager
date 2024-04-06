using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ASA_Server_Manager.Attached;

public static class TextBlockHelper
{
    public static readonly DependencyProperty TrimPathProperty =
        DependencyProperty.RegisterAttached(
            "TrimPath",
            typeof(string),
            typeof(TextBlockHelper),
            new PropertyMetadata(null, OnTrimPathChanged)
        );

    public static string GetTrimPath(TextBlock textBlock) => (string) textBlock.GetValue(TrimPathProperty);

    public static void SetTrimPath(TextBlock textBlock, string value) => textBlock.SetValue(TrimPathProperty, value);

    private static void OnTrimPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBlock textBlock)
        {
            textBlock.Text = SmartTrim(textBlock, (string) e.NewValue);
        }
    }

    private static string SmartTrim(TextBlock textBlock, string path)
    {
        var typeface = new Typeface(
            textBlock.FontFamily,
            textBlock.FontStyle,
            textBlock.FontWeight,
            textBlock.FontStretch
        );

        var formattedText = GetFormattedText(path);

        if (formattedText.Width <= textBlock.ActualWidth)
            return path;

        const string TrimText = "...";
        var separator = Path.DirectorySeparatorChar;
        var fileParts = path.Split(separator);

        var parts = new List<string> {fileParts.Last()};

        for (var x = 0; x < fileParts.Length - 2; x++)
        {
            // Right now the order doesn't matter as we are only checking the width. We also want to
            // ensure that we have enough space if we need to trim the next folder, so we'll include
            // the trim text in the calculation.
            var nextPart = fileParts[x];
            var val = $"{string.Join(separator, parts)}{separator}{nextPart}{separator}{TrimText}";

            var formatted = GetFormattedText(val);

            if (formatted.Width > textBlock.ActualWidth)
            {
                parts.Insert(x, TrimText);
                break;
            }

            parts.Insert(x, nextPart);
        }

        return string.Join(separator, parts);

        FormattedText GetFormattedText(string text)
        {
            return new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                textBlock.FontSize,
                Brushes.Black, // brush doesn't matter since we're measuring
                VisualTreeHelper.GetDpi(textBlock).PixelsPerDip
            );
        }
    }
}