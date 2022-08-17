using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerConnector : StartConnector
{
    [SerializeField] 
    protected MinerBase minerScript;




    public virtual void Start()
    {
        minerScript = GetComponentInParent<MinerBase>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public override void OnConnectToSender(Sender _sender)
    {
        minerScript.connectedSender = _sender;
        minerScript.isConnectedToSender = true;
    }

    public override void Disconnect()
    {
        minerScript.isConnectedToSender = false;
        minerScript.connectedSender = null;

        base.Disconnect();
    }
}
