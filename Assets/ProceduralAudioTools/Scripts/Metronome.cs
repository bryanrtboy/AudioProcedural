//Bryan Leister - Jan 2017
using System.Collections;
using UnityEngine;

// The code example shows how to implement a metronome that procedurally generates the click sounds via the OnAudioFilterRead callback.
// While the game is paused or the suspended, this time will not be updated and sounds playing will be paused. Therefore developers of
//  music scheduling routines do not have to do any rescheduling after the app is unpaused

namespace ProceduralAudioTools
{
	public class Metronome : MonoBehaviour
	{
		public static Metronome instance = null;
	
		public float bpm = 140.0F;

		void Awake ()
		{
			//Check if instance already exists
			if (instance == null) {
				//if not, set instance to this
//				Debug.Log ("Instance is null at " + Time.time);
				instance = this;
				//If instance already exists and it's not this:
			} else if (instance != this) {
				Debug.Log ("Instance does not equal this at " + Time.time);
				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a TouchDetector.
				Destroy (gameObject); 
			}
		}

	}
}