using UnityEngine;
using System.Collections;

public class TimeMonitor : MonoBehaviour {
	private Pokemon[] pokeModels;

	void Start () {
		pokeModels = new Pokemon[2];
		pokeModels [0] = new Pokemon(GameObject.Find ("Charizard"), 0.025f);
		pokeModels [1] = new Pokemon(GameObject.Find ("Pikachu"), 0.075f);

		#if UNITY_ANDROID
		AndroidJavaObject javaObj = new AndroidJavaObject("edu.temple.gamemanager.TimerTracker");
		ModelSwapper listener = new ModelSwapper(pokeModels);
		javaObj.Call("SetTimerUpdateListener", listener);
		#else
		Debug.Log("Timer placeholder for Play Mode (not running on Android device)");
		#endif
	}

	void Update () { }

	private class ModelSwapper : AndroidJavaProxy {
		private AndroidJavaObject toastDebugger;
		private Pokemon[] viewModels;
		private bool displayFirst;

		public ModelSwapper(Pokemon[] viewModels) 
			: base("edu.temple.gamemanager.TimerUpdateListener") {
			this.viewModels = viewModels;
			displayFirst = true;

			toastDebugger = new AndroidJavaObject("edu.temple.gamemanager.ToastDebugger");
			SetActivityInNativePlugin();
			ShowMessage("Timer Listener ready! " + viewModels.Length + " models available.");
		}

		public void onTimerUpdate() {
			if (displayFirst) {
				viewModels [0].UpdateVisibility (false);
				viewModels [1].UpdateVisibility (true);
			} else {
				viewModels [0].UpdateVisibility (true);
				viewModels [1].UpdateVisibility (false);
			}
			displayFirst = !displayFirst;
		}

		private void SetActivityInNativePlugin() {
			AndroidJavaClass jclass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = jclass.GetStatic<AndroidJavaObject>("currentActivity");
			toastDebugger.Call("setActivity", activity);
		}

		private void ShowMessage(string message) {
			toastDebugger.Call("showMessage", message);
		}
	}

	private class Pokemon {
		private GameObject model;
		private float scale;

		public Pokemon(GameObject model, float scale) {
			this.model = model;
			this.scale = scale;
		}

		public void UpdateVisibility(bool visible) {
			if (visible) {
				model.transform.localScale = new Vector3 (this.scale, this.scale, this.scale);
			} else {
				model.transform.localScale = new Vector3 (0, 0, 0);
			}
		}
	}
}