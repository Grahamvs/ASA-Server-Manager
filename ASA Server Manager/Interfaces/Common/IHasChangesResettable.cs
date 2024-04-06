namespace ASA_Server_Manager.Interfaces.Common;

public interface IHasChangesResettable : IHasChanges
{
    void ResetHasChanges();
}