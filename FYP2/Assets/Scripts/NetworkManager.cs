using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public Camera standbyCamera;
	SpawnSpot[] spawnSpots;

	// Use this for initialization
	void Start () {
		spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
		Connect();
	}

	void Connect() {
		//PhotonNetwork.offlineMode = true;
		PhotonNetwork.ConnectUsingSettings("MultiFPS v1.0.0"); // game ver
	}

	void OnGUI() {
		GUILayout.Label( PhotonNetwork.connectionStateDetailed.ToString() );
	}

	void OnJoinedLobby() {
		Debug.Log ("OnJoinedLobby");
		PhotonNetwork.JoinRandomRoom();
	}

	void OnPhotonRandomJoinFailed() {			// used if random join failed (do sth)
		Debug.Log ("OnPhotonRandomJoinFailed");
		PhotonNetwork.CreateRoom(null);
	}

	void OnJoinedRoom() {
		Debug.Log ("OnJoinedRoom");

		SpawnMyPlayer();
	}

	void SpawnMyPlayer() {
		if(spawnSpots == null) {
			Debug.LogError("SpawnSpots NULL!");
			return;
		}

		SpawnSpot mySpawnSpot = spawnSpots[ Random.Range(0, spawnSpots.Length) ];
		//instantiates a prefab over the network
		PhotonNetwork.Instantiate("PlayerController", 
		                          mySpawnSpot.transform.position, 
		                          mySpawnSpot.transform.rotation, 0);
		standbyCamera.enabled = false;
	}
}