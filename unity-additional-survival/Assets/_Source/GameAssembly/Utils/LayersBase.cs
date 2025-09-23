using Utils.Data;

namespace Utils
{
    public static class LayersBase
    {
        public static LayersDataSO LayersData;

        private static bool _initialized;
        
        public static void InitLayersBase(LayersDataSO layersData)
        {
            if (_initialized)
                return;
            
            LayersData = layersData;
            
            _initialized = true;
        }
    }
}