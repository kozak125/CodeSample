using UnityEngine;

namespace VRShooter
{
    [CreateAssetMenu(menuName = "Float Value")]
    public class FloatValue : ScriptableObject
    {
        [field: SerializeField]
        public float Value { get; set; }
    }
}
