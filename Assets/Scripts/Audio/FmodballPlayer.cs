using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodballPlayer : MonoBehaviour
{
    public FMODUnity.EventReference Land;
    FMOD.Studio.EventInstance BallLand;
    private bool check = true;
    BallPhysicsBody ballPhysicsBody;
    [SerializeField] GameObject ball;
    // Start is called before the first frame update
    void Awake()
    {
        BallLand = FMODUnity.RuntimeManager.CreateInstance(Land);
        ballPhysicsBody = ball.GetComponent<BallPhysicsBody>();

        GameManager.Instance.Ball.HitWall += () =>
        {
            BallLand.start();
        };
    }
    void ballLand()
    {
        if(ballPhysicsBody.IsGrounded() && check)
        {
            BallLand.start();
            check = !check;
        }
        if(ballPhysicsBody.IsAirborne)
        {
            check = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        ballLand();
    }
}
