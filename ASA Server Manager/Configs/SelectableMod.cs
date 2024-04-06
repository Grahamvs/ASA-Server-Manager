using System.ComponentModel;
using ASA_Server_Manager.Common;
using ASA_Server_Manager.Interfaces.Configs;

namespace ASA_Server_Manager.Configs;

public class SelectableMod : BindableBase, IMod
{
    #region Private Fields

    private static readonly HashSet<string> CommonProperties = [.. typeof(IMod).GetProperties().Select(p => p.Name)];

    private readonly IMod _mod;
    private bool _isPassive;
    private bool _isSelected;

    #endregion

    #region Public Constructors

    public SelectableMod(IMod mod)
    {
        _mod = mod;

        _mod.PropertyChanged += OnMod_PropertyChanged;
    }

    #endregion

    #region Public Properties

    public string Description
    {
        get => _mod.Description;
        set => _mod.Description = value;
    }

    public int ID
    {
        get => _mod.ID;
        set => _mod.ID = value;
    }

    public bool IsPassive
    {
        get => _isPassive;
        set => SetProperty(ref _isPassive, value, OnIsPassiveChanged);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value, OnIsSelectedChanged);
    }

    public string Name
    {
        get => _mod.Name;
        set => _mod.Name = value;
    }

    #endregion

    #region Private Methods

    private void OnIsPassiveChanged()
    {
        if (IsPassive)
        {
            IsSelected = true;
        }
    }

    private void OnIsSelectedChanged()
    {
        if (!IsSelected)
        {
            IsPassive = false;
        }
    }

    private void OnMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (CommonProperties.Contains(e.PropertyName))
        {
            RaisePropertyChanged(e.PropertyName);
        }
    }

    #endregion
}