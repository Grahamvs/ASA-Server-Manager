namespace ASA_Server_Manager.Interfaces.Common;

public interface ICloneable<out T> : ICloneable
{
    public new T Clone();
}