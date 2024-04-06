using System.Windows;
using System.Windows.Controls;

namespace ASA_Server_Manager.Controls;

/// <summary> Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
///
/// Step 1a) Using this custom control in a XAML file that exists in the current project. Add this
/// XmlNamespace attribute to the root element of the markup file where it is to be used:
///
/// xmlns:MyNamespace="clr-namespace:ASA_Server_Runner.Controls"
///
///
/// Step 1b) Using this custom control in a XAML file that exists in a different project. Add this
/// XmlNamespace attribute to the root element of the markup file where it is to be used:
///
/// xmlns:MyNamespace="clr-namespace:ASA_Server_Runner.Controls;assembly=ASA_Server_Runner.Controls"
///
/// You will also need to add a project reference from the project where the XAML file lives to this
/// project and Rebuild to avoid compilation errors:
///
/// Right-click on the target project in the Solution Explorer and "Add
/// Reference"->"Projects"->[Browse to and select this project]
///
///
/// Step 2) Go ahead and use your control in the XAML file.
///
/// <MyNamespace:HintedTextBox/>
///
/// </summary>
public class HintedTextBox : TextBox
{
    static HintedTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HintedTextBox), new FrameworkPropertyMetadata(typeof(HintedTextBox)));
    }

    public HintedTextBox()
    {
        // Assuming you have a Loaded event handler to set things up after the control is loaded.
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (GetTemplateChild("MainTextBox") is TextBox internalTextBox)
        {
            // Listen to text changes on the internal TextBox.
            internalTextBox.TextChanged -= InternalTextBox_TextChanged;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (GetTemplateChild("MainTextBox") is TextBox internalTextBox)
        {
            // Listen to text changes on the internal TextBox.
            internalTextBox.TextChanged += InternalTextBox_TextChanged;
        }
    }

    private void InternalTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Forward the Text value from the internal TextBox to the HintedTextBox.
        if (sender is TextBox internalTextBox)
        {
            SetCurrentValue(TextProperty, internalTextBox.Text);
        }
    }

    public static readonly DependencyProperty HintProperty = DependencyProperty.Register(
        nameof(Hint),
        typeof(string),
        typeof(HintedTextBox),
        new PropertyMetadata(string.Empty)
    );

    public string Hint
    {
        get => (string) GetValue(HintProperty);
        set => SetValue(HintProperty, value);
    }
}