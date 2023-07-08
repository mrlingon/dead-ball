using UnityEngine;

public class FishInstantiator : MonoBehaviour
{
    public GameObject fishPrefab;
    public Transform goal;
    public HumanManager humanManager;

    [Range(0, 300)]
    public int number;

    private void Start()
    {
        for (int i = 0; i < number; i++)
        {
            GameObject go = Instantiate(fishPrefab, Vector3.zero, Quaternion.identity);
            humanManager.humans.Add(go.GetComponent<HumanController>());
            if (go.TryGetComponent<HumanController>(out var h))
            {
                h.ballTransform = goal;
            }
        }
    }
}