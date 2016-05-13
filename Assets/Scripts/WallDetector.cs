using UnityEngine;
using System.Collections;

public class WallDetector : MonoBehaviour {

    private bool wallInFront = false;

    void OnTriggerEnter(Collider other) {
        if("Wall".Equals(other.gameObject.tag)) {
            wallInFront = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if ("Wall".Equals(other.gameObject.tag)) {
            wallInFront = false;
        }
    }

    public bool getWallInFront() {
        return wallInFront;
    }
}
