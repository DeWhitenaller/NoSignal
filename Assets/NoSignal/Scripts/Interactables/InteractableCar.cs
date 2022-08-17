using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCar : InteractableBase
{
    private CarController carController;

    private SoundManager soundManager;

    private PlayerMovement playerMovement;

    private InteractSphere interactSphere;


    [SerializeField] 
    private GameObject carCam, playerSeat;





    public override void Start()
    {
        base.Start();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        interactSphere = GameObject.FindGameObjectWithTag("Player").GetComponent<InteractSphere>();
        carController = GetComponent<CarController>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }


    public override void Interaction()
    {
        // disable player movement and the ability to interact with things
        interactSphere.enabled = false;
        interactUI.SetActive(false);
        playerMovement.transform.GetComponent<CharacterController>().enabled = false;
        playerMovement.movementDisabled = true;
        playerMovement.DisableMovement();

        // set the player to the seat position
        playerMovement.gameObject.transform.parent = playerSeat.transform;
        playerMovement.transform.position = playerSeat.transform.position;
        playerMovement.transform.rotation = playerSeat.transform.rotation;

        //start the car
        carController.enabled = true;
        carCam.SetActive(true);
        carController.AudioPlay();
        soundManager.MuteSound();
    }


    public virtual void GetOutOfCar()
    {
        // turn the car off
        carCam.SetActive(false);
        carController.AudioStop();
        soundManager.PlaySound();

        // set new player position
        playerMovement.gameObject.transform.parent = null;
        playerMovement.transform.position += new Vector3(-10, 5, 0);
        playerMovement.transform.GetComponent<CharacterController>().enabled = true;

        // enable player movement and interact ability
        interactSphere.enabled = true;
        carController.enabled = false;
        playerMovement.movementDisabled = false;
    }
}
