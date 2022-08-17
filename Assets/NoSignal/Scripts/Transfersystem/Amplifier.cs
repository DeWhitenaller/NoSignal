using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amplifier : MonoBehaviour
{
    /// <summary>
    /// this can be used to amplify the connection of a Sender or Receiver to connect Start/EndConnectors that are far away
    /// </summary>



    public Amplifier connectedAmplifier;

    public List<Amplifier> amplifiersThatAreConnectedToThis;

    public SenderConnector connectedSender;

    public ReceiverConnector connectedReceiver;

    public StartConnector connectedStartConnector;

    public List<ConnectorBase> connectedStartConnectorList;

    public List<ReceiverConnector> receiverConnectorList;

    public EndConnector connectedEndConnector;

    [SerializeField] 
    protected List<LineRenderer> lineRenderers;

    [SerializeField] 
    private LayerMask senderSideLayer, receiverSideLayer;



    public int identifier;
    
    
    
    public bool isConnected, isConnectedToSender, isConnectedToReceiver, hasPower, isOnSenderSide;





    #region Unity Methods

    private void Update()
    {
        if (isConnected) return;
        if (!hasPower) return;

        if (isOnSenderSide)
        {
            CheckForAmplifierOrStartConnector();
        }
        else
        {
            CheckForAmplifierOrEndConnector();
        }
    }

    #endregion Unity Methods




    #region OnConnect

    public void OnConnectToAmplifier(Amplifier _amplifier, out bool _connectingSuccessful)
    {
        if (isOnSenderSide)
        {
            if (_amplifier.isConnectedToSender)
            {
                _connectingSuccessful = false;
                return;
            }

            _amplifier.isOnSenderSide = true;
            _amplifier.isConnectedToSender = true;
            _amplifier.connectedSender = connectedSender;
            _amplifier.identifier = identifier;

            connectedSender.AddAmplifierToConnection(_amplifier);
        }
        else
        {
            if (_amplifier.isConnectedToReceiver)
            {
                _connectingSuccessful = false;
                return;
            }

            _amplifier.isOnSenderSide = false;
            _amplifier.isConnectedToReceiver = true;
            _amplifier.connectedReceiver = connectedReceiver;
            _amplifier.identifier = identifier;

            connectedReceiver.AddAmplifierToConnection(_amplifier);
        }

        _amplifier.hasPower = true;

        SetLineRendererPositions(0, transform.position, _amplifier.transform.position);

        _connectingSuccessful = true;
        isConnected = true;
        connectedAmplifier = _amplifier;

    }

    public virtual void OnConnectToStartConnector(StartConnector _startConnector, out bool _connectingSuccessful)
    {
        if(connectedStartConnector != null || _startConnector.isConnected)
        {
            _connectingSuccessful = false;
            return;
        }

        Debug.Log(_startConnector.isConnected);

        _startConnector.isConnected = true;
        _startConnector.connectedSender = connectedSender.sender;
        _startConnector.identifier = identifier;

        _startConnector.OnConnectToSender(connectedSender.sender);

        _connectingSuccessful = true;

        connectedStartConnector = _startConnector;

        connectedSender.connectedStartConnectors.Add(_startConnector);

        SetLineRendererPositions(1, transform.position, _startConnector.transform.position);
    }

    public virtual void OnConnectToEndConnector(EndConnector _endConnector, out bool _connectingSuccessful)
    {
        if (connectedEndConnector != null || _endConnector.isConnected)
        {
            _connectingSuccessful = false;
            return;
        }

        _endConnector.isConnected = true;
        _endConnector.connectedReceiver = connectedReceiver.receiver;
        _endConnector.identifier = identifier;

        _endConnector.OnConnectToReceiver(connectedReceiver.receiver, connectedReceiver);

        _connectingSuccessful = true;

        connectedEndConnector = _endConnector;

        connectedReceiver.connectedEndConnectors.Add(_endConnector);
        connectedEndConnector.connectedReceiver.connectedInventories.Add(_endConnector.inventoryFromDevice);

        SetLineRendererPositions(1, transform.position, _endConnector.transform.position);
    }


    #endregion OnConnect




    #region OverlapSphere Check

    public virtual void CheckForAmplifierOrStartConnector()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, senderSideLayer);

        if (cols.Length != 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.TryGetComponent<StartConnector>(out StartConnector _startConnector))
                {
                    OnConnectToStartConnector(_startConnector, out bool _connectingSuccessful);

                    if (_connectingSuccessful)
                    {
                        break;
                    }
                }
                if (cols[i].gameObject.TryGetComponent<Amplifier>(out Amplifier _amplifier))
                {
                    OnConnectToAmplifier(_amplifier, out bool _connectingSuccessful);

                    if (_connectingSuccessful)
                    {
                        //isConnected = true;
                        break;
                    }
                }
            }
        }
    }

    public void CheckForAmplifierOrEndConnector()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, receiverSideLayer);

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
                        break;
                    }
                }
                else if (cols[i].gameObject.TryGetComponent<Amplifier>(out Amplifier _amplifier))
                {
                    if(_amplifier == this)
                    {
                        continue;
                    }

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




    public virtual void SetLineRendererPositions(int _lineRendererIndex, Vector3 _startPos, Vector3 _endPos)
    {
        if (lineRenderers[_lineRendererIndex].enabled == false) lineRenderers[_lineRendererIndex].enabled = true;

        lineRenderers[_lineRendererIndex].SetPosition(0, _startPos);
        lineRenderers[_lineRendererIndex].SetPosition(1, _endPos);
    }


    public void Disconnect()
    {
        isConnected = false;
        hasPower = false;
        identifier = 0;

        if (isOnSenderSide)
        {
            if(connectedStartConnector != null)
            {
                connectedStartConnector.Disconnect();
                connectedSender.connectedStartConnectors.Remove(connectedStartConnector);

                connectedStartConnector = null;
            }

            connectedSender = null;
            isConnectedToSender = false;
        }
        else
        {
            if (connectedEndConnector != null)
            {
                connectedEndConnector.Disconnect();
                connectedReceiver.connectedEndConnectors.Remove(connectedEndConnector);

                connectedEndConnector = null;
            }

            connectedReceiver = null;
            isConnectedToReceiver = false;
        }


        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].enabled = false;
        }        
    }


    private void OnDestroy()
    {
        if (isOnSenderSide)
        {
            connectedSender.DisconnectSpecificConnection(identifier);
        }
        else
        {
            connectedReceiver.DisconnectSpecificConnection(identifier);
        }
    }
}
