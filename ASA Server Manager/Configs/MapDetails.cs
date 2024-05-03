using ASA_Server_Manager.Common;

namespace ASA_Server_Manager.Configs
{
    public class MapDetails : BindableBase
    {
        private string _id;
        private string _name;

        public string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name ?? _id;
            set => SetProperty(ref _name, value);
        }
    }
}