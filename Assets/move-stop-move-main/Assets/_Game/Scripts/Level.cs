using UnityEngine;
using UnityEngine.AI;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform minPoint, maxPoint;
    public int botReal = 10;
    public int botTotal = 20;

    public Vector3 RandomPoint()
    {
        Vector3 randomPoint = GenerateRandomPoint();
        return FindValidNavMeshPosition(randomPoint);
    }

    private Vector3 GenerateRandomPoint()
    {
        float x = Random.Range(minPoint.position.x, maxPoint.position.x);
        float z = Random.Range(minPoint.position.z, maxPoint.position.z);
        return new Vector3(x, 0f, z);
    }

    private Vector3 FindValidNavMeshPosition(Vector3 point)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, float.PositiveInfinity, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return point; 
    }
}