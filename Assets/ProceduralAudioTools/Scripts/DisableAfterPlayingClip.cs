using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class DisableAfterPlayingClip : MonoBehaviour
{

	private AudioSource m_audioSource;
	private AudioClip m_clip;

	void Awake ()
	{
		m_audioSource = GetComponent<AudioSource> () as AudioSource;
	}

	void OnEnable ()
	{
		m_clip = m_audioSource.clip;
		Invoke ("DisableAfterPlay", m_clip.length);
	}

	void OnDisable ()
	{
		CancelInvoke ();
	}

	void DisableAfterPlay ()
	{
		this.gameObject.SetActive (false);
	}
}
