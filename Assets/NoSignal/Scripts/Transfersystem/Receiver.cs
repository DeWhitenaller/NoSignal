using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    /// <summary>
    /// this class contains the logic of the Receiver that gets accessed by the ReceiverConnector
    /// </summary>
     



    public delegate void Action();

    public static event Action OnChannelSet;



    public List<Inventory> connectedInventories;

    public Amplifier connectedAmplifier;
    
    [SerializeField]
    protected Sender connectedSender;
    
    

    private SenderAndReceiverList senderAndReceiver;
    


    [SerializeField] 
    protected LayerMask detectLayer;



    public int channel;
    
    
    public bool isConnectedToInventory;




    private void Start()
    {
        senderAndReceiver = GameObject.FindGameObjectWithTag("SenderAndReceiverList").GetComponent<SenderAndReceiverList>();
        senderAndReceiver.receiverList.Add(this);
    }

    private void OnDisable()
    {
        senderAndReceiver.receiverList.Remove(this);
    }

    public void ConnectInventory(Inventory _inventory)
    {
        connectedInventories.Add(_inventory);
    }

    public void DisconnectInventory(Inventory _inventory)
    {
        connectedInventories.Remove(_inventory);
    }

    public void SetChannel()
    {
        OnChannelSet?.Invoke();
    }
}
