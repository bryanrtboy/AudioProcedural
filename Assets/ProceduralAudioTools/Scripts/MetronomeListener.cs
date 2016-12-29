//Bryan Leister - Jan 2017

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace ProceduralAudioTools
{
	public class MetronomeListener : MonoBehaviour
	{

		public AudioClip[] m_clips;
		public bool m_randomize = true;
		public float m_clipsSpatialBlend = 0f;
		public float m_audioLevel = 1;
		public AudioMixerGroup m_audioMixer;
		public int numBeatsPerSegment = 16;
		public float m_randomChanceOfPlaying = 1f;
		public bool m_pingPan = false;
		//This only works with slower sounds, i.e. the setScheduled is going to play the sound at a specific time, which may not correspond with the pan settings...

		private double nextEventTime;
		private int randClip = 0;
		private int lastRandClip = 0;
		private AudioSource[] audioSources;
		private bool running = false;
		private int count;
		private Metronome m;
		private bool isLeft = false;

		void Awake ()
		{
			m = FindObjectOfType<Metronome> () as Metronome;
			if (m_clips.Length < 2) {
				Debug.LogError ("Not enough clips to play, must have more than 2 clips!");
				Destroy (this.gameObject);
			}
		}

		void Start ()
		{
			int length = m_clips.Length;
			audioSources = new AudioSource[length];

			int i = 0;
			while (i < length) {
				GameObject child = new GameObject (this.name + " Audio Player " + i.ToString ());

				child.transform.SetParent (this.transform);
				audioSources [i] = child.AddComponent<AudioSource> ();
				audioSources [i].spatialBlend = m_clipsSpatialBlend;
				audioSources [i].volume = m_audioLevel;
				if (m_audioMixer != null)
					audioSources [i].outputAudioMixerGroup = m_audioMixer;

				i++;
			}
			nextEventTime = AudioSettings.dspTime + 2.0F;
			running = true;
		}

		void Update ()
		{
			if (!running)
				return;

			if (m == null)
				m = FindObjectOfType<Metronome> () as Metronome;
        
			double time = AudioSettings.dspTime;

			if (time + 1.0F > nextEventTime) {
				if (m_randomize) {
					while (randClip == lastRandClip) {											
						randClip = Random.Range (0, m_clips.Length);
						continue;
					}
					lastRandClip = randClip;
				} else {
					randClip = count;
				}

				audioSources [count].clip = m_clips [randClip];
				audioSources [count].volume = Random.Range (.1f, m_audioLevel);

				if (m_pingPan) {

					if (isLeft)
						audioSources [count].panStereo = -1f;
					else
						audioSources [count].panStereo = 1f;

					isLeft = !isLeft;
				}

				if (Random.Range (0f, 1f) < m_randomChanceOfPlaying)
					audioSources [count].PlayScheduled (nextEventTime);
				//Debug.Log ("Scheduled source " + randClip + " to start at time " + nextEventTime);
				nextEventTime += 60.0F / m.bpm * numBeatsPerSegment; //calculate the next nextEventTime....
				count++;
				if (count >= m_clips.Length)
					count = 0;
			}
		}
	}
}
