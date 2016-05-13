using UnityEngine;
using System.Collections;

public class WinCollider : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if ("Player".Equals(other.tag)) {
            GameObject.Find("GameManager").GetComponent<GameManager>().setEscaped(true);
        }
    }

}
