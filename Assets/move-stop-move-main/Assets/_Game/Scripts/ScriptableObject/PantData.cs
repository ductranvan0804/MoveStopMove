using UnityEngine;

[CreateAssetMenu(fileName = "PantData", menuName = "Scriptable Objects/PantData")]
public class PantData : ScriptableObject
{
    [SerializeField] Material[] materials;

    public Material GetPant(PantType pantType)
    {
        return materials[(int)pantType];
    }
}
