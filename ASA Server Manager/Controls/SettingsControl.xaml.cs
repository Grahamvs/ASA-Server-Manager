using System.Windows;
using System.Windows.Markup;

namespace ASA_Server_Manager.Controls;

/// <summary>
/// Interaction logic for SettingsControl.xaml
/// </summary>
[ContentProperty("MainControl")]
public partial class SettingsControl
{
    #region Public Fields

    public static readonly DependencyProperty ContentHorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(ContentHorizontalAlignment), typeof(HorizontalAlignment), typeof(SettingsControl));

    public static readonly DependencyProperty ContentVerticalAlignmentProperty =
        DependencyProperty.Register(nameof(ContentVerticalAlignment), typeof(VerticalAlignment), typeof(SettingsControl), new PropertyMetadata(VerticalAlignment.Center));

    public static readonly DependencyProperty InfoTextProperty =
                DependencyProperty.Register(nameof(InfoText),
            typeof(string), typeof(SettingsControl),
            new PropertyMetadata(default(string)));

    // Define Dependency Properties for alignment
    public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(LabelHorizontalAlignment), typeof(HorizontalAlignment), typeof(SettingsControl));

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(SettingsControl), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty LabelVerticalAlignmentProperty =
        DependencyProperty.Register(nameof(LabelVerticalAlignment), typeof(VerticalAlignment), typeof(SettingsControl), new PropertyMetadata(VerticalAlignment.Center));

    public static readonly DependencyProperty MainControlProperty = DependencyProperty.Register(nameof(MainControl), typeof(object), typeof(SettingsControl), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty SecondaryControlProperty = DependencyProperty.Register(nameof(SecondaryControl), typeof(object), typeof(SettingsControl), new PropertyMetadata(default(object)));

    #endregion

    #region Public Constructors

    public SettingsControl()
    {
        InitializeComponent();
    }

    #endregion

    #region Public Properties

    public HorizontalAlignment ContentHorizontalAlignment
    {
        get => (HorizontalAlignment)GetValue(ContentHorizontalAlignmentProperty);
        set => SetValue(ContentHorizontalAlignmentProperty, value);
    }

    public VerticalAlignment ContentVerticalAlignment
    {
        get => (VerticalAlignment)GetValue(ContentVerticalAlignmentProperty);
        set => SetValue(ContentVerticalAlignmentProperty, value);
    }

    public string InfoText
    {
        get => (string)GetValue(InfoTextProperty);
        set => SetValue(InfoTextProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public HorizontalAlignment LabelHorizontalAlignment
    {
        get => (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty);
        set => SetValue(LabelHorizontalAlignmentProperty, value);
    }

    public VerticalAlignment LabelVerticalAlignment
    {
        get => (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty);
        set => SetValue(LabelVerticalAlignmentProperty, value);
    }

    public object MainControl
    {
        get => GetValue(MainControlProperty);
        set => SetValue(MainControlProperty, value);
    }

    public object SecondaryControl
    {
        get => GetValue(SecondaryControlProperty);
        set => SetValue(SecondaryControlProperty, value);
    }

    #endregion
}