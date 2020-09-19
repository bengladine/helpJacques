using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test
{
    private static Test _instance = null;

    public static Test Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Test();
            return _instance;
        }
    }

    public void TestOutput()
    {
        Debug.Log("This is a test output");
    }
}
