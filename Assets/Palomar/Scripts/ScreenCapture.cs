using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEditor;

public enum CaptureMethod
{
	AppCapture_Asynch,
	AppCapture_Synch,
	ReadPixels_Asynch,
	ReadPixels_Synch,
	RenderToTex_Asynch,
	RenderToTex_Synch
}

public class ScreenCapture : MonoBehaviour
{
	void OnGUI() //For testing
	{
		if(GUI.Button(new Rect(100 * 2, 0, 100, 30), "ReadPixels"))
	        Save(CaptureMethod.ReadPixels_Asynch, Application.dataPath + "/RenderedImages/screen3.png");
	    else if(GUI.Button(new Rect(100 * 4, 0, 100, 30), "RenderToTex"))
	        Save(CaptureMethod.RenderToTex_Asynch, Application.dataPath + "/RenderedImages/screen5.png");
	}
	
	public void Save(CaptureMethod method, string filePath)
	{
		if(method == CaptureMethod.ReadPixels_Asynch)
			StartCoroutine(ReadPixels(filePath));
		else if(method == CaptureMethod.RenderToTex_Asynch)
			StartCoroutine(RenderToTex(filePath));
	}
	
	public IEnumerator ReadPixels(string filePath)
	{
		//Wait for graphics to render
		yield return new WaitForEndOfFrame();
		
		//Create a texture to pass to encoding
		var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);

		//Put buffer into texture
		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		
		//Split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
		yield return 0;
		
		byte[] bytes = texture.EncodeToPNG();
		
		//Save our test image (could also upload to WWW)
		File.WriteAllBytes(filePath, bytes);
		
		//Tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
		DestroyObject(texture);
	}
	
	public IEnumerator RenderToTex(string filePath)
	{
		//Wait for graphics to render
		yield return new WaitForEndOfFrame();
		
		RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 32);        
		Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
		
		//Camera.main.targetTexture = rt;
		//Camera.main.Render();
		
		//Render from all!
		foreach(Camera cam in Camera.allCameras) {
			cam.targetTexture = rt;
			cam.Render();
			cam.targetTexture = null;
		}
		
		RenderTexture.active = rt;        
		screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		Camera.main.targetTexture = null;
		RenderTexture.active = null; //Added to avoid errors
		Destroy(rt);
		
		//Split the process up
		yield return 0;
		
		byte[] bytes = screenShot.EncodeToPNG();
		File.WriteAllBytes(filePath, bytes);

		yield return null;
		//AssetDatabase.ImportAsset( filePath );
		AssetDatabase.Refresh();

		// Sprite.Create generates *something*.. but nothing useful :/
		Rect rect = new Rect(0, 0, Screen.width, Screen.height);
		var newSprite = Sprite.Create(screenShot,rect,new Vector2(0.5f,0.5f),.01f);

		var gOname = "screen";
		var newGO = new GameObject(gOname);
		
		var img = newGO.AddComponent<Image>();
		img.sprite = newSprite;
	
	// This won't really work.. setting is changed but it doesnt really create a sprite
//		var textureImporter = AssetImporter.GetAtPath("Assets/screen5.png") as TextureImporter;
//		if (textureImporter == null) { Debug.Log ("Not Found: "+filePath); yield break; }
//		textureImporter.textureType = TextureImporterType.Sprite; 
//		//AssetDatabase.ImportAsset( filePath );
//		AssetDatabase.Refresh();
//		}

		//Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
	}
}