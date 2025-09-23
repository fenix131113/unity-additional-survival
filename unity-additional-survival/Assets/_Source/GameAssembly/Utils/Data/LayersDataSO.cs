using UnityEngine;

namespace Utils.Data
{
    [CreateAssetMenu(fileName = "Layers Data", menuName = "SO/New Layers Data")]
    public class LayersDataSO : ScriptableObject
    {
        [field: SerializeField] public LayerMask PlayerLayer { get; private set; }
    }
}