using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

sealed class SpriteAssetImporter : AssetPostprocessor {

	// This event is raised when a texture asset is imported
	private void OnPreprocessTexture() {
		// I prefix my texture asset's file names with screen, this line says "if 'screen' is not in the asset file name, do nothing"
		if(!assetPath.Contains("screen")) 
			return;
		
		// Get the reference to the assetImporter (From the AssetPostProcessor class) and unbox it to a TextureImporter (Which is inherited and extends the AssetImporter with texture specific utilities)
		var importer = assetImporter as TextureImporter;

		importer.textureType = TextureImporterType.Sprite;
		Debug.Log ("Converted: "+assetPath); // Assets/screen3.png
	}

	public void OnPostProcessTexture (Texture2D texture) 
	{

		Debug.Log ("path: "+assetPath);

		// No worky.. c'mon Unity :(
//		Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath)
//			.OfType<Sprite>().ToArray();
//		foreach(Sprite item in sprites) {
//			Debug.Log (item.name);
//		}

//		var name = "screen";
//		var newGO = new GameObject(name);
//		
//		var img = newGO.AddComponent<Image>();
		//img.sprite = newSprite;
		
		//AssetDatabase.ImportAsset( filePath );
	}
}
	