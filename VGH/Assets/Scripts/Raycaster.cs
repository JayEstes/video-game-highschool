using UnityEngine;

public class Raycaster : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask layers;

    public bool IsHitting
    {
        get
        {
            return isHitting;
        }
    }

    bool isHitting = false;


    public GameObject IntersectedGameObject
    {
        get
        {
            return intersectedGameObject;
        }
    }

    private GameObject intersectedGameObject;

    public void Update()
    {
        intersectedGameObject = null;
        isHitting = Physics.Raycast(new (transform.position, transform.forward), out RaycastHit hit, maxDistance, layers, QueryTriggerInteraction.Ignore);

        if (isHitting)
        {
            intersectedGameObject = hit.transform.gameObject;
        }
    }
}
