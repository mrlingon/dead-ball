using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : MonoBehaviour
{
    public List<HumanController> humans = new List<HumanController>();
    public float ballSpeedCutoff = 1f;
    public BallPhysicsBody ball;

    // Update is called once per frame
    void Update()
    {
        if (ball.Rigidbody.velocity.magnitude <= ballSpeedCutoff)
        {
            foreach (HumanController h in humans)
            {
                h.currentState = HumanController.HumanState.RUN_TOWARDS;
            }
        }
        else
        {
            foreach (HumanController h in humans)
            {
                h.currentState = HumanController.HumanState.RUN_AWAY;
            }
        }

    }
}
