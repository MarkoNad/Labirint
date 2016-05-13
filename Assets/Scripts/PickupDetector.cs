using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PickupDetector : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if("Pickup".Equals(other.tag) && !other.GetComponent<PickupManager>().getPickedUp()) {
            other.GetComponent<PickupManager>().setPickedUp();
			other.gameObject.SetActive (false);
            GameObject.Find("GameManager").GetComponent<GameManager>().increaseScore();
        }
    }

}
