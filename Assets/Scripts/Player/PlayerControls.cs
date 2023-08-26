using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
	public float accel_m_s2 = 2;
	public float angular_accel_deg_s2 = 45;
	public float audioFadeout_s = 0.5f;
	public GameObject torpedoPrefab;
	public float attackCooldown = 1f;
	public GameObject cooldownIndicatorObj;
	public float health = 100f;
	public GameObject healthIndicatorObj;
	public StatusBar healthBar => healthIndicatorObj.GetComponent<StatusBar>();

	public delegate void OnPlayerMoved(Vector2 delta);
	public OnPlayerMoved onPlayerMoved;

	public AudioClip moveClip;
	public bool inputControl = true;
	public bool lockBackwards = true;

	SonarManager sonar;
	PlayerInput plyrInput;
	Rigidbody2D rigidbody2d;
	AudioSource audioSource;
	float sourceVolume;
	ParticleSystem partSys;
	bool canAttack = true;
	StatusBar cooldownIndicator;

	private void Start()
	{
		plyrInput = GetComponent<PlayerInput>();
		rigidbody2d = GetComponent<Rigidbody2D>();
		sonar = GameObject.FindGameObjectWithTag("SonarManager").GetComponent<SonarManager>();
		audioSource = GetComponent<AudioSource>();
		partSys = GetComponentInChildren<ParticleSystem>();
		cooldownIndicator = cooldownIndicatorObj.GetComponent<StatusBar>();

		audioSource.volume = 0;
	}

	public IEnumerator AttackCooldown()
	{
		float timer = attackCooldown;
		cooldownIndicator.SetBarValue(0);

		while (timer > 0)
		{
			timer -= Time.deltaTime;

			float timerFract = timer / attackCooldown;
			cooldownIndicator.SetBarValue(1 - timerFract);

			yield return null;
		}

		canAttack = true;
	}

	public void OnAttack()
	{
		if (!canAttack) return;

		GameObject torpedo = Instantiate(torpedoPrefab, transform.position, transform.rotation);
		sonar.CreateSonarTorpedo(torpedo);

		canAttack = false;
		StartCoroutine(AttackCooldown());
	}

	public void OnSonar()
	{
		sonar.PingSonar();
	}

	public void SetPos(Vector3 pos)
	{
		transform.position = pos;
		sonar.SetSonarPlayer(transform.rotation.eulerAngles.z, pos);
	}

	private void Update()
	{
		float velMagnitude = rigidbody2d.velocity.sqrMagnitude;

		if (inputControl)
		{
			Vector2 input = plyrInput.actions["Move"].ReadValue<Vector2>();

			if (input.y != 0)
			{
				Vector2 fwd = transform.up * input.y;

				if (lockBackwards && input.y <= 0)
				{
					if (velMagnitude == 0 || Vector2.Dot(fwd, rigidbody2d.velocity) < 0)
					{
						fwd = Vector2.zero;
					}
				}

				Vector2 delta = accel_m_s2 * Time.deltaTime * fwd;
				rigidbody2d.AddForce(delta, ForceMode2D.Impulse);
				sourceVolume = 1;
			}

			if (input.x != 0)
			{
				float rot = -input.x * angular_accel_deg_s2 * Time.deltaTime;
				rigidbody2d.AddTorque(rot, ForceMode2D.Impulse);
				sourceVolume = 1;
			}
		}

		if (sourceVolume > 0)
		{
			if (partSys.isStopped) partSys.Play();
			sourceVolume -= Time.deltaTime / audioFadeout_s;
			audioSource.volume = Mathf.Clamp01(sourceVolume);
		}
		else
		{
			if (partSys.isPlaying) partSys.Stop();
		}

		if (velMagnitude > 0)
		{
			Vector2 delta = rigidbody2d.velocity * Time.deltaTime;
			sonar.SetSonarPlayer(transform.rotation.eulerAngles.z);
			onPlayerMoved?.Invoke(delta);

			var psMain = partSys.main;
			psMain.startSpeed = velMagnitude * 0.75f;
		}

		if (rigidbody2d.angularVelocity != 0)
		{
			sonar.SetSonarPlayer(transform.rotation.eulerAngles.z);
		}
		
		healthBar.SetBarValue(health);
	}
}
