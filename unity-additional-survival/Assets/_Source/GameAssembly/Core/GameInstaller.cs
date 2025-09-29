using Player;
using UnityEngine;
using Utils;
using Utils.Data;
using VContainer;
using VContainer.Unity;
using WeaponSystem;

namespace Core
{
    public class GameInstaller : LifetimeScope
    {
        [SerializeField] private LayersDataSO layersDataSO;
        
        private InputSystem_Actions _input;

        protected override void Configure(IContainerBuilder builder)
        {
            #region Player
            
            _input = new InputSystem_Actions();
            _input.Player.Enable();
            builder.RegisterInstance(_input);

            #endregion

            #region Utils

            builder.RegisterComponentInHierarchy<BulletsPool>();
            LayersBase.InitLayersBase(layersDataSO);

            #endregion
        }

        protected void Start()
        {
            ObjectInjector.InitInjector(Container);
        }
    }
}