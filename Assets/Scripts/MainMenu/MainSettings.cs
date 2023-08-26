using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainSettings : MonoBehaviour
{
	public GameObject sfxSlider;
	public GameObject musicSlider;
	public AudioMixer sfxMixer;
	public AudioMixer musicMixer;
	public static float sfxVolume = 0.7f;
	public static float musicVolume = 0.7f;

	public static void OnSFXVolumeChanged()
	{
		MainSettings settings = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSettings>();
		settings.sfxMixer.SetFloat("SFXVolume", Mathf.Log10(settings.sfxSlider.GetComponent<Slider>().value + 0.01f) * 20);
		sfxVolume = settings.sfxSlider.GetComponent<Slider>().value;
	}

	public static void OnMusicVolumeChanged()
	{
		MainSettings settings = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSettings>();
		settings.musicMixer.SetFloat("MusicVolume", Mathf.Log10(settings.musicSlider.GetComponent<Slider>().value + 0.01f) * 20);
		musicVolume = settings.musicSlider.GetComponent<Slider>().value;
	}

	public static void OnSettingsBack()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<MainMenuManager>().settingsMenu.SetActive(false);
		GameObject.FindGameObjectWithTag("GameController").GetComponent<MainMenuManager>().mainMenu.SetActive(true);
		MainMenuManager.ManagerPlayClick();
	}

	private void Start()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSettings>().sfxSlider.GetComponent<Slider>().value = sfxVolume;
		GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSettings>().musicSlider.GetComponent<Slider>().value = musicVolume;
	}
}
