using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class catch_release : MonoBehaviour
{
    Spring SpringCS;
    // Start is called before the first frame update
    void Start()
    {
        SpringCS = this.GetComponent<Spring>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpringCS.isFixed = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            SpringCS.isFixed = false;
        }
    }
}
