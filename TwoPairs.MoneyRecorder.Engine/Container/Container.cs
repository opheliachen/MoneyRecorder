using Microsoft.Practices.Unity;

namespace TwoPairs.MoneyRecorder.Engine
{
    public class Container : UnityContainer
    {
        public static Container Instance { get; private set; }

        public Container()
        {
            Instance = this;
        }
    }
}