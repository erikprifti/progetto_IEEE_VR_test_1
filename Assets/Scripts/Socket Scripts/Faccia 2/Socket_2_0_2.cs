using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket_2_0_2 : MonoBehaviour
{
    public Interface _interface;

    int x = 2;
    int y = 0;
    int z = 2;

    public void setOccupied()
    {
        _interface.setSocketOccupied(x, y, z);
    }

}
