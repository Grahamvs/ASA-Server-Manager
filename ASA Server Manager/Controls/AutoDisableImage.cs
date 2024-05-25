using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ASA_Server_Manager.Controls;

public sealed class AutoDisableImage : Image
{
    #region Public Fields

    public static readonly DependencyProperty DisabledOpacityProperty = DependencyProperty.Register(
        nameof(DisabledOpacity),
        typeof(double),
        typeof(AutoDisableImage),
        new PropertyMetadata(0.5)
    );

    #endregion

    #region Private Fields

    private double _cachedOpacity;
    private Brush _cachedOpacityMask;

    #endregion

    #region Public Constructors

    static AutoDisableImage()
    {
        // Override the metadata of the IsEnabled and Source properties to be notified of changes
        IsEnabledProperty.OverrideMetadata(typeof(AutoDisableImage), new FrameworkPropertyMetadata(true, OnAutoDisableImagePropertyChanged));
        SourceProperty.OverrideMetadata(typeof(AutoDisableImage), new FrameworkPropertyMetadata(null, OnAutoDisableImagePropertyChanged));
    }

    #endregion

    #region Public Properties

    public double DisabledOpacity
    {
        get => (double)GetValue(DisabledOpacityProperty);
        set => SetValue(DisabledOpacityProperty, value);
    }

    #endregion

    #region Private Properties

    private bool IsGrayScaled => Source is FormatConvertedBitmap;

    #endregion

    #region Private Methods

    /// <summary>
    /// Called when AutoDisableImage's IsEnabled or Source property values changed
    /// </summary>
    private static void OnAutoDisableImagePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
    {
        if (source is AutoDisableImage image
            && image.IsEnabled == image.IsGrayScaled)
        {
            image.UpdateImage();
        }
    }

    private void UpdateImage()
    {
        if (Source == null)
            return;

        if (IsEnabled)
        {
            if (IsGrayScaled)
            {
                // restore the original image
                Source = ((FormatConvertedBitmap)Source).Source;
                // reset the Opacity Mask
                OpacityMask = _cachedOpacityMask;
                _cachedOpacityMask = null;

                Opacity = _cachedOpacity;
            }

            return;
        }

        if (!IsGrayScaled)
        {
            // Get the source bitmap
            if (Source is BitmapSource bitmapImage)
            {
                Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);

                _cachedOpacity = Opacity;
                _cachedOpacityMask = OpacityMask;

                OpacityMask = new ImageBrush(bitmapImage);
                Opacity = DisabledOpacity;
            }
        }
    }

    #endregion
}