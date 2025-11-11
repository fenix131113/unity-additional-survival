using Utils;

namespace BuildingSystem.Buildings
{
    public class BaseHeart : ABuilding
    {
        protected override void OnDeathLogic() // Called on the server
        {
            base.OnDeathLogic();
            ShutdownUtil.Shutdown();
        }
    }
}