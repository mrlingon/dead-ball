using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static EnemyController;

public class FMODPlayerScript : MonoBehaviour
{
    public FMODUnity.EventReference Jump;
    FMOD.Studio.EventInstance playerJump;
    //public FMODUnity.EventReference Drag;
    //FMOD.Studio.EventInstance playerCharge;
    public PlayerController playerController;
    public PlayerHoldDrag playerHoldDrag;

    FMOD.Studio.EventDescription JumpDescription;
    FMOD.Studio.PARAMETER_DESCRIPTION dragVelocityDesc;
    FMOD.Studio.PARAMETER_ID dragVelocityID;

    void Start()
    {
        InitializeEvents();
        InitializeParameters();
    }

    void Update()
    {
        PlayerIsJumping();
        //PlayerisCharging();
    }



    /*void PlayerisCharging()
    {
        playerHoldDrag.StartDrag += (_) =>
        {
            playerCharge.start();
            //playerCharge.release();
        };


    }*/

    void PlayerIsJumping()
    {
        playerHoldDrag.Released += (_, drag) =>
        {
            //Sätt Drag Velocity till ett värde mellan 0 och ett baserat på hur hårt man sparkar bollen
            float p = math.length(drag) / GameManager.Instance.Player.DragPower.MaxPower;
            playerJump.setParameterByID(dragVelocityID, p);
            playerJump.start();
            //playerJump.release();

        };

    }

    void InitializeEvents()
    {
        playerJump = FMODUnity.RuntimeManager.CreateInstance(Jump);
        //playerCharge = FMODUnity.RuntimeManager.CreateInstance(Drag);

    }

    void InitializeParameters()
    {
        JumpDescription = FMODUnity.RuntimeManager.GetEventDescription(Jump);
        JumpDescription.getParameterDescriptionByName("DragVelocity", out dragVelocityDesc);
        dragVelocityID = dragVelocityDesc.id;
    }

}
