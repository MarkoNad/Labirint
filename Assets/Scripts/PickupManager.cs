using UnityEngine;
using System.Collections;

public class PickupManager : MonoBehaviour {
    private bool pickedUp = false;
    private float rotationMultiplier = 1;

    void Update() {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime * rotationMultiplier);
    }

    public void setPickedUp() {
        pickedUp = true;
        rotationMultiplier = 0.5f;
    }

    public bool getPickedUp() {
        return pickedUp;
    }
}
