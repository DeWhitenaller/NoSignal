using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorBase : MonoBehaviour
{
    [SerializeField] 
    protected LineRenderer lineRenderer;
    
    public List<Amplifier> amplifierConnectionList;
    
    
    
    public int identifier;


    public bool isConnected, hasPower;




    public virtual void SetLineRendererPositions(Vector3 _startPos, Vector3 _endPos)
    {
        if (lineRenderer.enabled == false) lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, _startPos);
        lineRenderer.SetPosition(1, _endPos);
    }

    public virtual void Disconnect()
    {

    }
}
