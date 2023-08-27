using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
	public float accel_m_s2 = 4;
	public float maxSpeed_m_s = 10;
	public float speedTargetThresh = 0.9f;
	public float lifetime_s = 5f;
	public AudioClip fireClip;
	public GameObject explosionObj;
	public bool enemyTorp = false;

	Rigidbody2D rigidbody2d;
	float lifeTimer = 0;
	GameObject sonarTorpedo = null;
	RectTransform sonarRect;
	RectTransform sonarCanvas;

	private GameObject PPobj;
	private DynamicPP _dynamicPp => PPobj.GetComponent<DynamicPP>();

	public float damage = 34f;

	public void AddSonarTorpedo(GameObject sonarObj)
	{
		sonarTorpedo = sonarObj;
		sonarRect = sonarTorpedo.GetComponent<RectTransform>();
		sonarCanvas = sonarTorpedo.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
		{
			BigBoom();
			return;
		}

		if (enemyTorp)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				other.GetComponent<PlayerControls>().health -= damage;
				DynamicPP.PPTriggerPlayerDamaged();
				BigBoom();
			}

			return;
		}

		if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			Debug.Log("down");
			BigBoom();
			other.GetComponent<Enemy>().DamageEnemy(damage);
		}
	}

	private void BigBoom()
	{
		Instantiate(explosionObj, transform.position, Quaternion.identity);
		Destroy(sonarTorpedo);
		Destroy(gameObject);
	}

	private void Start()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();

		Vector3 pos = transform.position;
		pos.z = Camera.main.transform.position.z;
		AudioSource.PlayClipAtPoint(fireClip, pos);

		PPobj = GameObject.FindGameObjectWithTag("ThePP");
	}

	private void Update()
	{
		if (rigidbody2d.velocity.sqrMagnitude <= Mathf.Pow(maxSpeed_m_s, 2) * speedTargetThresh)
		{
			rigidbody2d.AddForce(transform.up * accel_m_s2, ForceMode2D.Impulse);
		}

		lifeTimer += Time.deltaTime;
		if (lifeTimer >= lifetime_s) BigBoom();

		if (sonarTorpedo != null)
		{
			sonarRect.anchoredPosition = CanvasUtils.WorldToCanvasPointCentered(sonarCanvas, transform.position, Camera.main);
		}
	}
}
