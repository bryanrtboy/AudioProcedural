using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProceduralAudioTools
{
	[RequireComponent (typeof(ParticleSystem))]
	public class ParticleCollisionHandler : MonoBehaviour
	{

		public AudioClip[] m_clips;
		public int m_magnitudeThreshold = 10;
		public GameObject m_audioSourcePrefab;
		public int m_audioPoolSize = 50;

		private float m_maxVelocity = -1000f;
		private ParticleSystem m_part;
		private List<ParticleCollisionEvent> m_collisionEvents;
		private GameObject[] m_pooledGameObjects;
		private AudioSource[] m_pooledAudioSources;
		private int m_poolCount = 0;

		void Awake ()
		{
			m_pooledGameObjects = new GameObject[m_audioPoolSize];
			m_pooledAudioSources = new AudioSource[m_audioPoolSize];

			for (int i = 0; i < m_audioPoolSize; i++) {
				m_pooledGameObjects [i] = Instantiate (m_audioSourcePrefab, this.transform) as GameObject;
				if (m_pooledGameObjects [i].activeSelf) //Prefab should be inactive already
					m_pooledGameObjects [i].SetActive (false);

				m_pooledAudioSources [i] = m_pooledGameObjects [i].GetComponent<AudioSource> () as AudioSource;
			}
		}

		void Start ()
		{
			m_part = GetComponent<ParticleSystem> ();
			m_collisionEvents = new List<ParticleCollisionEvent> ();

			if (m_clips.Length < 1) {
				Debug.LogError ("No sounds! Have to kill myself :-(");
				Destroy (this);
			}
		}

		void OnParticleCollision (GameObject other)
		{
			int numCollisionEvents = m_part.GetCollisionEvents (other, m_collisionEvents);

			int i = 0;

			AudioClip clip = m_clips [Random.Range (0, m_clips.Length)];

//			if (other.tag == "water")
//				clip.

			while (i < numCollisionEvents) {
				Vector3 pos = m_collisionEvents [i].intersection;
				float force = m_collisionEvents [i].velocity.sqrMagnitude;

				if (force > m_maxVelocity)
					m_maxVelocity = force;

				//Debug.Log (force.magnitude + ", " + force.sqrMagnitude);
				if (force > m_magnitudeThreshold) {
					float vol = Mathf.Lerp (0, .8f, Mathf.InverseLerp (0, m_maxVelocity, force));
					//AudioSource.PlayClipAtPoint (clip, pos, vol); //ToDo - use object pooling for more control over the AudioSource...

					if (m_pooledGameObjects [m_poolCount].activeSelf) {
						m_pooledGameObjects [m_poolCount].SetActive (false); // This could check for an inactive gameobject in the pool and  use that one instead of cutting off the audio
					}

					m_pooledAudioSources [m_poolCount].clip = clip;
					m_pooledAudioSources [m_poolCount].volume = vol;
					m_pooledGameObjects [m_poolCount].transform.position = pos;
					m_pooledGameObjects [m_poolCount].SetActive (true);
					m_pooledAudioSources [m_poolCount].Play ();

					m_poolCount++;
					if (m_poolCount >= m_audioPoolSize)
						m_poolCount = 0;
				}
				i++;
			}
		}
	}
}