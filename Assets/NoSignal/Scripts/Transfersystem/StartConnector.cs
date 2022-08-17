using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartConnector : ConnectorBase
{
    /// <summary>
    /// this is the part of the Transfersystem that gets connected by a SenderConnector or an Amplifier
    /// </summary>




    [SerializeField] 
    protected Amplifier connectedAmplifier;

    public Sender connectedSender;

    public SenderConnector connectedSenderConnector;



    [SerializeField] 
    protected LayerMask detectLayer;





    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }


    private void Update()
    {
        if (isConnected) return;

        CheckForAmplifier();
    }


    public virtual void OnConnectToSender(Sender _sender)
    {

    }


    public override void Disconnect()
    {
        isConnected = false;
        identifier = 0;
        connectedSender = null;

        lineRenderer.enabled = false;
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
                    _amplifier.OnConnectToStartConnector(this, out bool _connectingSuccessful);
                }
            }
        }
    }


    public virtual void OnDestroy()
    {
        Disconnect();

        if (connectedSenderConnector != null)
        {
            connectedSenderConnector.OnDisconnectFromStartConnector(this);
        }
    }

}
