using Player;
using VContainer;
using VContainer.Unity;

namespace Core
{
    public class GameInstaller : LifetimeScope
    {
        private InputSystem_Actions _input;

        protected override void Configure(IContainerBuilder builder)
        {
            #region Player
            
            _input = new InputSystem_Actions();
            _input.Player.Enable();
            builder.RegisterInstance(_input);

            #endregion
        }

        protected void Start()
        {
            ObjectInjector.InitInjector(Container);
        }
    }
}