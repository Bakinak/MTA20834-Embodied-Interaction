using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class evilGuy : MonoBehaviour
{
    public float moveSpeed = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(moveSpeed * Time.deltaTime, 0);
    }
}
