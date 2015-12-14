using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class is responsible for managing the key input for changing scenes and loading XMLs
/// </summary>
public class SceneManager : MonoBehaviour {

	public MonoBehaviour Target;
	private SceneData _sceneData;
	public enum GraphType { Line, Donut, World, Filled }

	internal bool ShowDebug;

	/// <summary> Persists between Scene Loads & Loads a ScriptableAsset with Scene Data names
	/// </summary>
	public void Awake()
	{
		DontDestroyOnLoad(gameObject);
		_sceneData = Resources.Load<SceneData>("SceneData");
	}

	public void LateUpdate()
	{
		if (Input.GetKey(KeyCode.U)) {
			ShowDebug = !ShowDebug;
		}
		else if (Input.GetKey(KeyCode.Alpha1)){
			ImportAndAnimate(1);
		}
		else if (Input.GetKey(KeyCode.Alpha2)){
			ImportAndAnimate(2);
		}
		else if (Input.GetKey(KeyCode.Alpha3)){
			ImportAndAnimate(3);
		}
		else if (Input.GetKey(KeyCode.Alpha4)){
			ImportAndAnimate(4);
		}
		else if (Input.GetKey(KeyCode.L)) {
 			Application.LoadLevel(_sceneData.LineIdx);
		}
		else if (Input.GetKey(KeyCode.D)) {
 			Application.LoadLevel(_sceneData.DonutIdx);
		}
		else if (Input.GetKey(KeyCode.W)) {
 			Application.LoadLevel(_sceneData.WorldIdx);
		}
		else if (Input.GetKey(KeyCode.F)) {
 			Application.LoadLevel(_sceneData.FilledIdx);
		}
	}

	private void ImportAndAnimate(int i)
	{
		Target.SendMessage("ImportXML", i, SendMessageOptions.DontRequireReceiver);
		Target.SendMessage("PlayMotion", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary> Every time a new scene is loaded, update 'Target' with the proper graph script
	/// </summary>
	public void OnLevelWasLoaded(int lvl)
	{
		if (lvl == _sceneData.LineIdx)
			Target = FindObjectOfType<LineGraph>();
		if (lvl == _sceneData.DonutIdx)
			Target = FindObjectOfType<DonutGraph>();
		if (lvl == _sceneData.WorldIdx)
			Target = FindObjectOfType<WorldGraph>();
		if (lvl == _sceneData.FilledIdx)
			Target = FindObjectOfType<FilledLineGraph>();
	}

}
