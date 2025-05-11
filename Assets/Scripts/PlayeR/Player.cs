using UnityEngine;

public class Player : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxisRaw("Horizontal")!= 0)
        {
            this.transform.position += new Vector3((float)Input.GetAxisRaw("Horizontal") * 0.16f, 0f, 0f);
        }
    }
}
