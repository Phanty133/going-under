using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGoal : MonoBehaviour
{
	public GameObject playerPrefab;

	public void InitPlayer(GameObject cooldownObj, GameObject healthObj, GameObject levelFailedObj)
	{
		GameObject plyrObj = Instantiate(playerPrefab, transform.position, new Quaternion());
		PlayerControls playerControls = plyrObj.GetComponent<PlayerControls>();

		playerControls.cooldownIndicatorObj = cooldownObj;
		playerControls.healthIndicatorObj = healthObj;
		playerControls.levelFailed = levelFailedObj;
		playerControls.inputControl = true;
	}
}
