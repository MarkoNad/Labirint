using UnityEngine;
using System.Collections.Generic;

public class SpawnPointManager : MonoBehaviour {
    public Transform[] spawnPoints;
    public GameObject[] pickups;

    private List<GameObject> instantiated;

	void Awake() {
        instantiated = new List<GameObject>();
        System.Random rand = new System.Random();
        List<int> visited = new List<int>();

        for(int i = 0; i < pickups.Length; i++) {
            while (true) {
                int pointIdx = rand.Next(0, spawnPoints.Length);
                if (visited.Contains(pointIdx) && pickups.Length<spawnPoints.Length) continue;
                visited.Add(pointIdx);

                GameObject newPickup = (GameObject)Instantiate(pickups[i], spawnPoints[pointIdx].position, Quaternion.identity);
                newPickup.SetActive(true);
                instantiated.Add(newPickup);
                   
                break;
            }
        }

	}
	
	public void deactivatePickups() {
        foreach (GameObject pickup in instantiated) {
            pickup.SetActive(false);
        }
    }
}
