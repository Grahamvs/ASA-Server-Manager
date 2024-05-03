using System.ComponentModel;

namespace ASA_Server_Manager.Interfaces.Configs;

public interface IMod : INotifyPropertyChanged
{
    string Comments { get; set; }

    int ID { get; set; }

    string Name { get; set; }
}