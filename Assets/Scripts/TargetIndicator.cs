using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    public Transform Target;
    public float HideDistance;

    void Start()
    {

    }

    private bool dying = false;
    private bool targetSet = false;
    public void SetTarget(Transform target)
    {
        Target = target;
        targetSet = true;
    }

    void Update()
    {
        if (!targetSet) return;
        if (dying) return;

        if (Target == null || Target.gameObject.TryGetComponent(out EnemyController ec) && ec.isDying)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            Destroy(this.gameObject);
            dying = true;
            return;
        }

        var dir = Target.position - transform.position;
        var dist = dir.magnitude;

        if (dist <= HideDistance)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            return;
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
