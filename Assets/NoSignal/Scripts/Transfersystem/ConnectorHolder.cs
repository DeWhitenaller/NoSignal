using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorHolder : MonoBehaviour
{
    /// <summary>
    /// this adds a Connector Component to an object
    /// </summary>
     


    private enum State
    {
        StartConnector,
        EndConnector,
        MinerConnector
    }

    [SerializeField] 
    private State currentState;
    


    [SerializeField] 
    private Inventory inventoryFromDevice;





    public void AddConnectorScript(GameObject _connector)
    {
        switch (currentState)
        {
            case State.StartConnector:
                _connector.AddComponent<StartConnector>();
                _connector.layer = 13;
                break;

            case State.EndConnector:
                EndConnector _endConnector = _connector.AddComponent<EndConnector>();
                _endConnector.inventoryFromDevice = inventoryFromDevice;
                _connector.layer = 12;
                break;

            case State.MinerConnector:
                _connector.AddComponent<MinerConnector>();
                _connector.layer = 13;
                break;

            default:
                break;
        }
    }
}
