using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODPlayerScript : MonoBehaviour
{
    public FMODUnity.EventReference Jump;
    FMOD.Studio.EventInstance playerJump;
    public FMODUnity.EventReference Drag;
    FMOD.Studio.EventInstance playerCharge;
    public PlayerController playerController;
    public PlayerHoldDrag playerHoldDrag;
     

    // Start is called before the first frame update
    void Start()
    {
        playerJump = FMODUnity.RuntimeManager.CreateInstance(Jump);
        playerCharge = FMODUnity.RuntimeManager.CreateInstance(Drag);
        
    }

    void PlayerisCharging()
    {
        playerHoldDrag.StartDrag += () =>
        {
            playerCharge.start();
        };
       

    }


    void PlayerIsJumping()
    {
        playerHoldDrag.Released += (drag) =>
        {
            playerJump.start();
        };
        
    }
    // Update is called once per frame
    void Update()
    {
        PlayerIsJumping();
        PlayerisCharging();
    }
}
