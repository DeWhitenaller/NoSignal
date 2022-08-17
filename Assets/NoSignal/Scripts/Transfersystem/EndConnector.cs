using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndConnector : ConnectorBase
{
    /// <summary>
    /// this is the part of the Transfersystem that gets connected by a ReceiverConnector or an Amplifier
    /// </summary>
    
    
    public Inventory inventoryFromDevice;

    public Receiver connectedReceiver;

    public ReceiverConnector connectedReceiverConnector;



    [SerializeField] 
    protected LayerMask detectLayer;




    public virtual void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (isConnected) return;

        CheckForAmplifier();
    }




    public virtual void OnConnectToReceiver(Receiver _receiver, ReceiverConnector _receiverConnector)
    {
        if (isConnected) return;
        connectedReceiverConnector = _receiverConnector;
        _receiver.ConnectInventory(inventoryFromDevice);
        SetLineRendererPositions(transform.position, _receiver.transform.position);
    }


    public virtual void CheckForAmplifier()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, detectLayer);

        if (cols.Length != 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.TryGetComponent<Amplifier>(out Amplifier _amplifier))
                {
                    if (!_amplifier.hasPower) return;
                    _amplifier.OnConnectToEndConnector(this, out bool _connectingSuccessful);
                }
            }
        }
    }


    public override void Disconnect()
    {
        connectedReceiver.DisconnectInventory(inventoryFromDevice);

        isConnected = false;
        identifier = 0;
        connectedReceiver = null;

        lineRenderer.enabled = false;
    }


    private void OnDestroy()
    {
        Disconnect();

        if(connectedReceiverConnector != null)
        {
            connectedReceiverConnector.OnDisconnectFromEndConnector(this);
        }
    }
}
