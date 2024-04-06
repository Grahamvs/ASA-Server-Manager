using System.ComponentModel;

namespace ASA_Server_Manager.Interfaces.Configs;

public interface IMod : INotifyPropertyChanged
{
    string Description { get; set; }

    int ID { get; set; }

    string Name { get; set; }
}