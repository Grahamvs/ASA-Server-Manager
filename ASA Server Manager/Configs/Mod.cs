using ASA_Server_Manager.Common;
using ASA_Server_Manager.Interfaces.Configs;

namespace ASA_Server_Manager.Configs;

public class Mod : BindableBase, IMod
{
    private string _comments;
    private int _id;
    private string _name;

    public string Comments
    {
        get => _comments;
        set => SetProperty(ref _comments, value);
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