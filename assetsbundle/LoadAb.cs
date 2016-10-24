using UnityEngine;
using System.Collections;

public class LoadAb : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (load ());
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	IEnumerator load ()
	{
		WWW w = new WWW ("file://" + Application.streamingAssetsPath + "/AssetsBundle/AssetsBundle");
		yield return w;
		if (w.error != null) {
			Debug.Log (w.error);
		}

		AssetBundleManifest info = w.assetBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		var alls = info.GetAllAssetBundles ();
		foreach (var name in alls) {
			var deps = info.GetAllDependencies (name);
			if (deps.Length <= 0) {
				continue;
			}

			string ds = "";
			foreach (var d in deps) {
				ds += d + " ";
			}

			Debug.Log (string.Format ("{0} deps: {1}", name, ds));
		}
	}
}