namespace ASA_Server_Manager_Tests.Common;

[TestClass]
public abstract class BaseTest
{
    [TestCleanup]
    public void Cleanup()
    {
        OnCleanup();
    }

    [TestInitialize]
    [Obsolete("Use OnSetup instead")]
    public virtual void Initialize()
    {
        OnSetup();
    }

    protected virtual void OnCleanup()
    {
    }

    protected virtual void OnSetup()
    {
    }
}