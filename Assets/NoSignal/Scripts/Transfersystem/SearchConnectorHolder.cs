using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchConnectorHolder : MonoBehaviour
{
    void Awake()
    {
        GetComponentInParent<ConnectorHolder>().AddConnectorScript(gameObject);
    }
}
