using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DynamicPP : MonoBehaviour
{
	public float PlayerDamagedStrength = 0.85f;
	public float PlayerDamagedFadeIn = 0.25f;
	public float PlayerDamagedFadeOut = 3f;

	Volume ppVolume;

	public static void PPTriggerPlayerDamaged()
	{
		GameObject ppObj = GameObject.FindGameObjectWithTag("ThePP");
		DynamicPP pp = ppObj.GetComponent<DynamicPP>();
		pp.StartCoroutine(pp.TriggerPlayerDamaged());
	}

	public IEnumerator TriggerPlayerDamaged()
	{
		ChromaticAberration ca;
		ppVolume.profile.TryGet(out ca);

		Vignette v;
		ppVolume.profile.TryGet(out v);

		float t = 0f;
		while (t < PlayerDamagedFadeIn)
		{
			ca.intensity.value = Mathf.Lerp(0f, PlayerDamagedStrength, t / PlayerDamagedFadeIn);
			v.intensity.value = Mathf.Lerp(0f, PlayerDamagedStrength, t / PlayerDamagedFadeIn);
			t += Time.deltaTime;
			yield return null;
		}

		t = 0f;
		while (t < PlayerDamagedFadeOut)
		{
			ca.intensity.value = Mathf.Lerp(PlayerDamagedStrength, 0f, t / PlayerDamagedFadeOut);
			v.intensity.value = Mathf.Lerp(PlayerDamagedStrength, 0f, t / PlayerDamagedFadeOut);
			t += Time.deltaTime;
			yield return null;
		}

		ca.intensity.value = 0f;
	}

	private void Start()
	{
		ppVolume = GetComponent<Volume>();
	}
}
