using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiverConnector : ConnectorBase
{
    /// <summary>
    /// this is the Connector that sits on the Receiver and connects the Receiver to EndConnectors or Amplifiers
    /// </summary>



    public Receiver receiver;



    [SerializeField] 
    protected List<LineRenderer> lineRendererList;

    [SerializeField] 
    protected List<List<Amplifier>> connectionsList;

    [SerializeField] 
    protected List<int> identifierList;

    public List<EndConnector> connectedEndConnectors;



    [SerializeField] 
    protected LayerMask detectLayer;



    [SerializeField] 
    protected int maxConnections, currentConnections;
    
    
    [SerializeField] 
    protected bool maxConnectionsReached;





    #region Unity Methods
    void Start()
    {
        connectionsList = new List<List<Amplifier>>(maxConnections);

        for (int i = 0; i < maxConnections; i++)
        {
            connectionsList.Add(new List<Amplifier>());
        }
    }

    void Update()
    {
        if (maxConnectionsReached) return;
        if (!hasPower) return;

        CheckForAmplifierOrEndConnector();
    }

    #endregion Unity Methods





    #region OverlapSphere Check

    public virtual void CheckForAmplifierOrEndConnector()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, detectLayer);

        if (cols.Length != 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.TryGetComponent<EndConnector>(out EndConnector _endConnector))
                {
                    OnConnectToEndConnector(_endConnector, out bool _connectingSuccessful);
                    if (_connectingSuccessful)
                    {
                        isConnected = true;

                        currentConnections++;
                        if (currentConnections >= maxConnections) maxConnectionsReached = true;

                        break;
                    }
                }
                else if (cols[i].gameObject.TryGetComponent<Amplifier>(out Amplifier _amplifier))
                {
                    OnConnectToAmplifier(_amplifier, out bool _connectingSuccessful);
                    if (_connectingSuccessful)
                    {
                        isConnected = true;
                        break;
                    }
                }
            }
        }
    }

    #endregion OverlapSphere Check





    #region Connect To Amplifier
    public virtual void OnConnectToAmplifier(Amplifier _amplifier, out bool _connectingSuccessful)
    {
        if (maxConnectionsReached)
        {
            _connectingSuccessful = false;
            return;
        }

        if (_amplifier.isConnectedToReceiver || _amplifier.isOnSenderSide)
        {
            _connectingSuccessful = false;
            return;
        }

        if (!CreateConnectionEntryInConnectionList(_amplifier))
        {
            _connectingSuccessful = false;
            return;
        }


        _amplifier.isOnSenderSide = false;
        _amplifier.hasPower = true;
        _amplifier.connectedReceiver = this;
        _amplifier.isConnectedToReceiver = true;

        _connectingSuccessful = true;

        SetLineRendererPositionsOnSpecificLineRenderer(_amplifier.identifier, transform.position, _amplifier.transform.position);
    }


    public virtual bool CreateConnectionEntryInConnectionList(Amplifier _amplifier)
    {
        if (maxConnectionsReached)
        {
            return false;
        }

        for (int i = 0; i < connectionsList.Count; i++)
        {
            if (connectionsList[i].Count == 0)
            {
                int _index = i;

                _amplifier.identifier = _index;

                AddAmplifierToConnection(_amplifier);
                currentConnections++;

                if (currentConnections >= maxConnections) maxConnectionsReached = true;

                return true;
            }
        }

        return false;
    }

    #endregion Connect To Amplifier





    public virtual void SetLineRendererPositionsOnSpecificLineRenderer(int _amplifierIdentifier, Vector3 _startPos, Vector3 _endPos)
    {
        if (lineRendererList[_amplifierIdentifier].enabled == false) lineRendererList[_amplifierIdentifier].enabled = true;

        lineRendererList[_amplifierIdentifier].SetPosition(0, _startPos);
        lineRendererList[_amplifierIdentifier].SetPosition(1, _endPos);
    }

    public virtual void OnConnectToEndConnector(EndConnector _endConnector, out bool _connectingSuccessful)
    {
        if (_endConnector.isConnected)
        {
            _connectingSuccessful = false;
            return;
        }

        if (maxConnectionsReached)
        {
            _connectingSuccessful = false;
            return;
        }

        _endConnector.OnConnectToReceiver(receiver, this);
        _endConnector.isConnected = true;
        _endConnector.connectedReceiver = receiver;

        connectedEndConnectors.Add(_endConnector);

        _connectingSuccessful = true;

    }

    public virtual void OnDisconnectFromEndConnector(EndConnector _endConnector)
    {
        connectedEndConnectors.Remove(_endConnector);

        currentConnections--;
        if (maxConnectionsReached) maxConnectionsReached = false;
    }

    public virtual void RemoveAmplifierFromConnection(Amplifier _amplifier)
    {
        connectionsList[_amplifier.identifier].Remove(_amplifier);
    }

    public virtual void AddAmplifierToConnection(Amplifier _amplifier)
    {
        connectionsList[_amplifier.identifier].Add(_amplifier);
    }

    public virtual void DisconnectSpecificConnection(int _connectionIdentifier)
    {
        lineRendererList[_connectionIdentifier].enabled = false;

        for (int i = 0; i < connectionsList[_connectionIdentifier].Count; i++)
        {
            connectionsList[_connectionIdentifier][i].Disconnect();
        }

        Debug.Log(connectionsList[_connectionIdentifier].Count);
        connectionsList[_connectionIdentifier].Clear();

        currentConnections--;

        if (maxConnectionsReached) maxConnectionsReached = false;
    }

    private IEnumerator DisconnectAmplifierWithDelay(int _connectionIdentifier)
    {

        for (int i = 0; i < connectionsList[_connectionIdentifier].Count; i++)
        {
            connectionsList[_connectionIdentifier][i].Disconnect();
            yield return new WaitForSeconds(0.3f);
        }

    }
}
