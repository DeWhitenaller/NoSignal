//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AmplifierBackUp : MonoBehaviour
//{
//    public Amplifier connectedAmplifier;
//    public List<Amplifier> amplifiersThatAreConnectedToThis;
//    public Sender connectedSender;
//    public Receiver connectedReceiver;
//    public List<ConnectorBase> startConnectorList;
//    public List<ReceiverConnector> receiverConnectorList;
//    public EndConnector connectionEnd;

//    public bool isConnected, hasPower, isOnSenderSide;

//    [SerializeField] private LayerMask senderSideLayer, receiverSideLayer;
//    [SerializeField] protected LineRenderer lineRenderer;

//    private void Update()
//    {
//        if (isConnected) return;
//        if (!hasPower) return;

//        if (isOnSenderSide)
//        {
//            CheckForAmplifierOrSender();
//        }
//        else
//        {
//            CheckForAmplifierOrEndConnector();
//        }
//    }

//    public void OnConnectToSender(Sender _sender)
//    {
//        connectedSender = _sender;

//        foreach (StartConnector startConnector in startConnectorList)
//        {
//            startConnector.OnConnectToSender(_sender);
//        }

//        connectedSender.connectedAmplifiers.Add(this);
//        SetLineRendererPositions(transform.position, connectedSender.transform.position);
//    }

//    public void OnConnectToAmplifier(Amplifier _amplifier, out bool _connectingSuccessful)
//    {
//        if (_amplifier.isConnected)
//        {
//            _amplifier.amplifiersThatAreConnectedToThis.Add(this);
//            MergeStartConnectorsFromAmplifier(_amplifier);
//            _connectingSuccessful = true;
//        }
//        else
//        {
//            connectedAmplifier = _amplifier;
//            connectedAmplifier.hasPower = true;
//            connectedAmplifier.isOnSenderSide = isOnSenderSide;
//            connectedAmplifier.gameObject.layer = 0;
//            connectedAmplifier.amplifiersThatAreConnectedToThis.Add(this);
//        }


//        if (isOnSenderSide)
//        {
//            foreach (StartConnector startConnector in startConnectorList)
//            {
//                connectedAmplifier.AddStartConnectorToList(startConnector);
//            }


//            if (startConnectorList.Count != 0)
//            {
//                foreach (StartConnector startConnector in startConnectorList)
//                {
//                    startConnector.amplifierConnectionList.Add(connectedAmplifier);
//                }
//            }
//        }
//        else
//        {
//            connectedAmplifier.connectedReceiver = connectedReceiver;

//            foreach (ReceiverConnector receiverConnector in receiverConnectorList)
//            {
//                connectedAmplifier.AddReceiverConnectorToList(receiverConnector);
//            }

//            if (connectedReceiver != null)
//            {
//                foreach (ReceiverConnector receiverConnector in receiverConnectorList)
//                {
//                    receiverConnector.amplifierConnectionList.Add(connectedAmplifier);
//                }
//            }
//        }

//        _connectingSuccessful = true;

//        SetLineRendererPositions(transform.position, connectedAmplifier.transform.position);
//    }

//    public virtual void OnConnectToEndConnector(EndConnector _endConnector)
//    {
//        _endConnector.OnConnectToReceiver(connectedReceiver);
//        SetLineRendererPositions(transform.position, _endConnector.transform.position);
//    }

//    public virtual void SetLineRendererPositions(Vector3 _startPos, Vector3 _endPos)
//    {
//        if (lineRenderer.enabled == false) lineRenderer.enabled = true;

//        lineRenderer.SetPosition(0, _startPos);
//        lineRenderer.SetPosition(1, _endPos);
//    }

//    public void CheckForAmplifierOrSender()
//    {
//        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, senderSideLayer);

//        if (cols.Length != 0)
//        {
//            for (int i = 0; i < cols.Length; i++)
//            {
//                if (cols[0].gameObject.TryGetComponent<Sender>(out Sender _sender))
//                {
//                    Debug.Log("amplifier connected");
//                    OnConnectToSender(_sender);
//                    isConnected = true;
//                    break;
//                }
//                else if (cols[0].gameObject.TryGetComponent<Amplifier>(out Amplifier _amplifier))
//                {
//                    if (_amplifier == this)
//                    {
//                        Debug.Log("lol", gameObject);
//                        return;
//                    }

//                    OnConnectToAmplifier(_amplifier, out bool _connectingSucccessful);

//                    if (_connectingSucccessful)
//                    {
//                        isConnected = true;
//                        break;
//                    }
//                }
//            }
//        }
//    }

//    public void CheckForAmplifierOrEndConnector()
//    {
//        Collider[] cols = Physics.OverlapSphere(transform.position, 20f, receiverSideLayer);

//        if (cols.Length != 0)
//        {
//            for (int i = 0; i < cols.Length; i++)
//            {
//                if (cols[i].gameObject.TryGetComponent<EndConnector>(out EndConnector _endConnector))
//                {
//                    OnConnectToEndConnector(_endConnector);
//                    isConnected = true;
//                    break;
//                }
//                else if (cols[i].gameObject.TryGetComponent<Amplifier>(out Amplifier _amplifier))
//                {
//                    if (_amplifier == this)
//                    {
//                        Debug.Log("lol", gameObject);
//                        return;
//                    }

//                    OnConnectToAmplifier(_amplifier, out bool _connectingSuccessful);

//                    if (_connectingSuccessful)
//                    {
//                        isConnected = true;
//                        break;
//                    }
//                }
//            }
//        }
//    }


//    public virtual void MergeStartConnectorsFromAmplifier(Amplifier _amplifier)
//    {
//        List<ConnectorBase> newStartConnectors = new List<ConnectorBase>();

//        foreach (var otherStartConnector in _amplifier.startConnectorList)
//        {
//            bool isAlreadyInList = false;

//            foreach (var thisStartConnector in startConnectorList)
//            {
//                if (otherStartConnector == thisStartConnector)
//                {
//                    isAlreadyInList = true;
//                    break;
//                }
//            }

//            if (!isAlreadyInList)
//            {
//                newStartConnectors.Add(otherStartConnector);
//            }
//        }

//        if (newStartConnectors.Count != 0)
//        {
//            startConnectorList.AddRange(newStartConnectors);
//        }
//    }

//    public virtual void AddStartConnectorToList(StartConnector _startConnector)
//    {
//        bool isAlreadyInList = false;

//        for (int i = 0; i < startConnectorList.Count; i++)
//        {
//            if (startConnectorList[i] == _startConnector)
//            {
//                isAlreadyInList = true;
//            }
//        }

//        if (!isAlreadyInList)
//        {
//            startConnectorList.Add(_startConnector);
//        }
//    }

//    public virtual void AddReceiverConnectorToList(ReceiverConnector _receiverConnector)
//    {
//        bool isAlreadyInList = false;

//        for (int i = 0; i < receiverConnectorList.Count; i++)
//        {
//            if (receiverConnectorList[i] == _receiverConnector)
//            {
//                isAlreadyInList = true;
//            }
//        }

//        if (!isAlreadyInList)
//        {
//            receiverConnectorList.Add(_receiverConnector);
//        }
//    }

//    public void Disconnect()
//    {
//        lineRenderer.enabled = false;
//        isConnected = false;
//        hasPower = false;
//        gameObject.layer = 11;

//        if (startConnectorList.Count != 0)
//        {
//            foreach (StartConnector startConnector in startConnectorList)
//            {
//                startConnector.amplifierConnectionList.Remove(this);
//                startConnector.Disconnect();
//            }
//        }
//        else if (receiverConnectorList.Count != 0)
//        {
//            foreach (ReceiverConnector receiverConnector in receiverConnectorList)
//            {
//                receiverConnector.amplifierConnectionList.Remove(this);
//                receiverConnector.Disconnect();
//            }
//        }
//    }

//    private void OnDestroy()
//    {
//        Disconnect();
//    }
//}
