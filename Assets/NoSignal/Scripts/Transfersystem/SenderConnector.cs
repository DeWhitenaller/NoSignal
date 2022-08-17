using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenderConnector : ConnectorBase
{
    /// <summary>
    /// this is the Connector that sits on the Sender and connects the Sender to StartConnectors or Amplifiers
    /// </summary>



    public Sender sender;

    [SerializeField] 
    protected List<List<Amplifier>> connectionsList;

    public List<StartConnector> connectedStartConnectors;



    [SerializeField] 
    protected List<LineRenderer> lineRendererList;



    [SerializeField] 
    protected LayerMask detectLayer;




    [SerializeField] 
    protected List<int> identifierList;

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

        CheckForAmplifierOrStartConnector();
    }

    #endregion Unity Methods

    #region OverlapSphere Check

    public virtual void CheckForAmplifierOrStartConnector()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, detectLayer);

        if (cols.Length != 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.TryGetComponent<StartConnector>(out StartConnector _startConnector))
                {
                    OnConnectToStartConnector(_startConnector, out bool _connectingSuccessful);

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

        if (_amplifier.isConnectedToSender || _amplifier.isConnectedToReceiver)
        {
            _connectingSuccessful = false;
            return;
        }

        if (!CreateConnectionEntryInConnectionList(_amplifier))
        {
            _connectingSuccessful = false;
            return;
        }

        //if (!AddAmplifierToList(_amplifier))
        //{
        //    _connectingSuccessful = false;
        //    return;
        //}


        _amplifier.isOnSenderSide = true;
        _amplifier.hasPower = true;
        _amplifier.connectedSender = this;
        _amplifier.isConnectedToSender = true;

        _connectingSuccessful = true;

        SetLineRendererPositionsOnSpecificLineRenderer(_amplifier.identifier, transform.position, _amplifier.transform.position);
    }



    public virtual bool CreateConnectionEntryInConnectionList(Amplifier _amplifier)
    {
        if(maxConnectionsReached)
        {
            return false;
        }


        for (int i = 0; i < connectionsList.Count; i++)
        {
            if(connectionsList[i].Count == 0)
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

    public virtual void OnConnectToStartConnector(StartConnector _startConnector, out bool _connectingSuccessful)
    {
        if (_startConnector.isConnected)
        {
            _connectingSuccessful = false;
            return;
        }

        if (maxConnectionsReached)
        {
            _connectingSuccessful = false;
            return;
        }

        Debug.Log(_startConnector.isConnected);

        _startConnector.isConnected = true;
        _startConnector.OnConnectToSender(sender);
        _startConnector.SetLineRendererPositions(_startConnector.transform.position, transform.position);
        _startConnector.connectedSenderConnector = this;
        _startConnector.connectedSender = sender;

        connectedStartConnectors.Add(_startConnector);

        _connectingSuccessful = true;

    }

    public virtual void OnDisconnectFromStartConnector(StartConnector _startConnector)
    {
        connectedStartConnectors.Remove(_startConnector);

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
