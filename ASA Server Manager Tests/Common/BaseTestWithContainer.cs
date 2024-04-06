using Moq;
using Moq.AutoMock;

namespace ASA_Server_Manager_Tests.Common
{
    [TestClass]
    public abstract class BaseTestWithContainer : BaseTest
    {
        protected AutoMocker Container { get; private set; }

        public T CreateInstance<T>() where T : class
            => Container.CreateInstance<T>();

        [TestInitialize]
        public override void Initialize()
        {
            Container = new AutoMocker();
            base.Initialize();
        }

        protected Mock<T> GetMock<T>()
            where T : class
            => Container.GetMock<T>();
    }
}