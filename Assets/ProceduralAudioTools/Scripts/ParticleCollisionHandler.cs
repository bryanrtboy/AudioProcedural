using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProceduralAudioTools
{
	[RequireComponent (typeof(AudioSource))]
	public class ParticleCollisionHandler : MonoBehaviour
	{
		public ParticleSystem part;
		public List<ParticleCollisionEvent> collisionEvents;
		public AudioClip[] m_clips;
		public int m_magnitudeThreshold = 10;

		float maxVelocity = -1000f;

		void Start ()
		{
			part = GetComponent<ParticleSystem> ();
			collisionEvents = new List<ParticleCollisionEvent> ();

			if (m_clips.Length < 1) {
				Debug.LogError ("No sounds! Have to kill myself :-(");
				Destroy (this);
			}
		}

		void OnParticleCollision (GameObject other)
		{
			int numCollisionEvents = part.GetCollisionEvents (other, collisionEvents);

			int i = 0;

			AudioClip clip = m_clips [Random.Range (0, m_clips.Length)];

//			if (other.tag == "water")
//				clip.

			while (i < numCollisionEvents) {
				Vector3 pos = collisionEvents [i].intersection;
				float force = collisionEvents [i].velocity.sqrMagnitude;

				if (force > maxVelocity)
					maxVelocity = force;

				//Debug.Log (force.magnitude + ", " + force.sqrMagnitude);
				if (force > m_magnitudeThreshold) {
					
					float vol = Mathf.Lerp (0, .8f, Mathf.InverseLerp (0, maxVelocity, force));
					AudioSource.PlayClipAtPoint (clip, pos, vol);
				}
				i++;
			}
		}
	}
}