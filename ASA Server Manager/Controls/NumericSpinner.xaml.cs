using System.ComponentModel;
using System.Windows;
using Range = ASA_Server_Manager.Common.Range;

namespace ASA_Server_Manager.Controls;

public partial class NumericSpinner
{
    #region Public Fields

    public static readonly DependencyProperty DecimalsProperty = DependencyProperty.Register(
        nameof(Decimals),
        typeof(int),
        typeof(NumericSpinner),
        new PropertyMetadata(2)
    );

    public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(
        nameof(DefaultValue),
        typeof(double?),
        typeof(NumericSpinner),
        new PropertyMetadata(default(double?))
    );

    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
        nameof(MaxValue),
        typeof(double),
        typeof(NumericSpinner),
        new PropertyMetadata(double.MaxValue)
    );

    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
        nameof(MinValue),
        typeof(double),
        typeof(NumericSpinner),
        new PropertyMetadata(double.MinValue)
    );

    public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
        nameof(Step),
        typeof(double?),
        typeof(NumericSpinner),
        new PropertyMetadata(0.1d)
    );

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double?),
        typeof(NumericSpinner),
        new PropertyMetadata(null)
    );

    #endregion

    #region Private Fields

    private bool _loaded;

    #endregion

    #region Public Events

    public event EventHandler ValueChanged;

    #endregion

    #region Public Constructors

    public NumericSpinner()
    {
        InitializeComponent();

        DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(NumericSpinner)).AddValueChanged(this, ValidateValue);
        DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(NumericSpinner)).AddValueChanged(this, ValueChanged);
        DependencyPropertyDescriptor.FromProperty(DecimalsProperty, typeof(NumericSpinner)).AddValueChanged(this, ValidateValue);
        DependencyPropertyDescriptor.FromProperty(MinValueProperty, typeof(NumericSpinner)).AddValueChanged(this, ValidateValue);
        DependencyPropertyDescriptor.FromProperty(MaxValueProperty, typeof(NumericSpinner)).AddValueChanged(this, ValidateValue);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    #endregion

    #region Public Properties

    public int Decimals
    {
        get => (int) GetValue(DecimalsProperty);
        set => SetValue(DecimalsProperty, value);
    }

    public double? DefaultValue
    {
        get => (double?) GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }

    public double MaxValue
    {
        get => (double) GetValue(MaxValueProperty);
        set
        {
            if (value < MinValue)
            {
                value = MinValue;
            }

            SetValue(MaxValueProperty, value);
        }
    }

    public double MinValue
    {
        get => (double) GetValue(MinValueProperty);
        set
        {
            if (value > MaxValue)
            {
                MaxValue = value;
            }

            SetValue(MinValueProperty, value);
        }
    }

    public double Step
    {
        get => (double) GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public double? Value
    {
        get => (double?) GetValue(ValueProperty);
        set => SetValue(ValueProperty, Range.SetInRange(value, MinValue, MaxValue));
    }

    #endregion

    #region Private Methods

    private void cmdDown_Click(object sender, RoutedEventArgs e)
    {
        var value = Value ?? DefaultValue ?? MinValue;

        var newValue = Range.SetInRange(value - Step, MinValue, MaxValue);

        if (newValue != Value)
        {
            SetCurrentValue(ValueProperty, newValue);
        }
    }

    private void cmdUp_Click(object sender, RoutedEventArgs e)
    {
        var value = Value ?? DefaultValue ?? MinValue;

        var newValue = Range.SetInRange(value + Step, MinValue, MaxValue);

        if (newValue != Value)
        {
            SetCurrentValue(ValueProperty, newValue);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
    }

    /// <summary>
    /// Revalidate the object, whenever a value is changed...
    /// </summary>
    private void Validate()
    {
        if (Value == null || !_loaded)
            return;

        var currentValue = Value.Value;
        var newValue = Range.SetInRange(double.Round(currentValue, Decimals), MinValue, MaxValue);

        if (newValue != currentValue)
        {
            SetCurrentValue(ValueProperty, newValue);
        }
    }

    private void ValidateValue(object sender, EventArgs e) => Validate();

    #endregion
}