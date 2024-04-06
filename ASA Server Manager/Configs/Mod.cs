using ASA_Server_Manager.Common;
using ASA_Server_Manager.Interfaces.Configs;

namespace ASA_Server_Manager.Configs;

public class Mod : BindableBase, IMod
{
    private string _description;
    private int _id;
    private string _name;

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public int ID
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}