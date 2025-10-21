using UnityEngine;

namespace BuildingSystem.Buildings
{
    public class BaseHeart : ABuilding
    {
        protected override void OnDeathLogic()
        {
            base.OnDeathLogic();
            Debug.Log("Game Over!!! Heart is destroyed!");
            //TODO: Do death logic
        }
    }
}