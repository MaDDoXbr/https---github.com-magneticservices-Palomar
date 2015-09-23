using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace EzEditor {

public enum GuiFieldType { String, Vector3, Int }


public class EzGUI {

    // Instance of HorizontalBlock:
    private static gui.HorizontalBlock horBlock = new gui.HorizontalBlock();
 
    // Now a 'Horizontal' method:
    public gui.HorizontalBlock Horizontal() {
         return horBlock.Begin();
    }        
}

public class gui: Editor {
    public static Texture2D DeleteButton = Resources.Load("deletebutton", typeof(Texture2D)) as Texture2D;
    public static Texture2D AddButton = Resources.Load("addbutton", typeof(Texture2D)) as Texture2D;

    private const float CharSize = 6f;

    public static void HBlockBegin() {EditorGUILayout.BeginHorizontal();}
    public static void HBlockEnd() {EditorGUILayout.EndHorizontal();}
    public static void VBlockBegin() {EditorGUILayout.BeginVertical();}
    public static void VBlockEnd() {EditorGUILayout.EndVertical();}

    #region Horizontal Block: using (gui.Horizontal()) {...}
    private static readonly HorizontalBlock Hblock = new HorizontalBlock();

    public class HorizontalBlock : IDisposable {
         public HorizontalBlock Begin() { GUILayout.BeginHorizontal(); return this; }
         public void Dispose() { GUILayout.EndHorizontal(); }
    }
    
    public static HorizontalBlock Horizontal() {
         return Hblock.Begin();
    } 
    #endregion

    #region Vertical Block: using (gui.Vertical()) {...}
    private static readonly VerticalBlock Vblock = new VerticalBlock();

    public class VerticalBlock : IDisposable {
         public VerticalBlock Begin() { GUILayout.BeginVertical(); return this; }
         public void Dispose() { GUILayout.EndVertical(); }
    }
    
    public static VerticalBlock Vertical() {
         return Vblock.Begin();
    } 
    #endregion

	public static void Separator() {
		EditorGUILayout.Separator();
	}
		
	public static void Separator(int pixels) {
		GUILayout.Label("", EditorStyles.label, GUILayout.Height(pixels));
	}
	
	public static GUILayoutOption MinWidth(float minWidth){
		return GUILayout.MinWidth(minWidth);
    }
		
	public static GUILayoutOption MaxWidth(float maxWidth) {
		return GUILayout.MaxWidth(maxWidth);
	}
	
	public static void EzCol (Action body) {
			EditorGUILayout.BeginVertical();
			body();
			EditorGUILayout.EndVertical();
	}
	
	public static void EzRow (Action body) {
			HBlockBegin();
			body();
			HBlockEnd();
	}	
		
	public static void EzRow (float indent, Action body) {
				HBlockBegin();
				EzSpacer (indent);
				body();
				HBlockEnd();
	}
	
	public static void EzSpacer (float pixels) {
			GUILayout.Label("", EditorStyles.label, MaxWidth(pixels), MinWidth(pixels));
	}
	
	public static void EzLabel (string label, float offset = 0f, params GUILayoutOption[] options) {
        AutosetFieldSize(label, offset);
        GUILayout.Label (label, options);
	}

    public static Color EzColorField(string label, Color color, float offset = 0f, params GUILayoutOption[] options)
    {
        AutosetFieldSize(label, offset);
        return EditorGUILayout.ColorField(label, color, options);        
    }

    //TODO: Count characters width in pixels
    private static void AutosetFieldSize(string label, float offset)
    {
        LookLikeControls(CharSize * label.Length + offset);
    }

	public static int EzIntField (string label, int val, float offset = 0f, params GUILayoutOption[] options)
	{
		AutosetFieldSize (label, offset);
		return EditorGUILayout.IntField (label, val, options);
	}

	public static float EzFloatField (string label, float val, float offset = 0f, params GUILayoutOption[] options)
    {
        AutosetFieldSize(label, offset);        
	    return EditorGUILayout.FloatField(label, val, options);
    }
	
	public static float EzFloatSlider(string label, float value, float minValue, float maxValue, float offset = 0f, params GUILayoutOption[] options) 
    {
		using (Horizontal()) {
			EzLabel(label, offset);
			return EditorGUILayout.Slider(value, minValue, maxValue, options);
		}
	}

	public static bool EzToggleButton (Texture2D controlTexture, Texture2D textureOn, Texture2D textureOff, bool toggleVariable, string tooltip = "", GUILayoutOption opt1 = null, GUILayoutOption opt2 = null) {
/*	    controlTexture = toggleVariable ? textureOn : textureOff;*/
	    bool buttonClicked;
        if (tooltip != "") {
	        var guiContent = new GUIContent(controlTexture, tooltip);
            buttonClicked = GUILayout.Button(guiContent, GUILayout.ExpandWidth(false), GUILayout.MinHeight(16), opt1, opt2);	    
	    } else 
            buttonClicked = GUILayout.Button(controlTexture, GUILayout.ExpandWidth(false), GUILayout.MinHeight(16), opt1, opt2);

	    if (!buttonClicked) return toggleVariable;

		controlTexture = (controlTexture == textureOff) ? textureOn : textureOff;
		return !toggleVariable;
	}

	public static void LookLikeControls(float size1, float size2){	
		EditorGUIUtility.labelWidth = size1;
		EditorGUIUtility.fieldWidth = size2;
	}

	public static void LookLikeControls(float size) {		
		EditorGUIUtility.labelWidth = size;
	}
			
	public static void LookLikeControls() {		
		EditorGUIUtility.labelWidth = 0f;
		EditorGUIUtility.fieldWidth = 0f;
	}

//    public static Object EzObjectField<T> (string label, Object obj, float offset = 0f)
//    {
//        LookLikeControls(CharSize * label.Length + offset);
//	    return EditorGUILayout.ObjectField(label, obj, typeof(T), true);
//    }

	public static Object EzObjectField<T> (string label, T obj, float offset, params GUILayoutOption[] options) where T: Object
	{
	    //if (obj == null) return null;
        AutosetFieldSize(label, offset);        
	    return EditorGUILayout.ObjectField(label, obj, typeof(T), true, options);
    }

    public static Vector3 EzV3Field(string label, Vector3 v3, float offset=0f, params GUILayoutOption[] options)
    {
        AutosetFieldSize(label, offset);        
        return EditorGUILayout.Vector3Field(label, v3, options);
    }

    // Eg.: _lineMesh.FillMode = (LineType)gui.EzEnumPopup("Fill Mode", _lineMesh.FillMode);
    public static Enum EzEnumPopup (string label, Enum enumToShow, float offset = 0f, params GUILayoutOption[] options) {
        AutosetFieldSize(label, offset);
        return EditorGUILayout.EnumPopup(label, enumToShow, options);
    }

    public static GameObject EzGameObjectField(string label, GameObject gO, float offset = 0f) {
		if (gO == null) return null;
        AutosetFieldSize(label, offset);        
		return EditorGUILayout.ObjectField(label, gO, typeof(GameObject), true) as GameObject;
	}

	public static GameObject EzGameObjectField(string label, GameObject gO, float offset, params GUILayoutOption[] options) {
        AutosetFieldSize(label, offset);        
		return EditorGUILayout.ObjectField(label, gO, typeof(GameObject), true, options) as GameObject;
	}

	public static bool EzFoldout(string label, bool variable, float offset=0f) {
        AutosetFieldSize(label, offset);        
		return EditorGUILayout.Foldout(variable, label);
	}

    // For 8x8 textured buttons, 14 is the optimal height
    public static bool EzButton(Texture2D texture, int height=14)
    {
        return GUILayout.Button(texture, GUILayout.ExpandWidth(false), GUILayout.Height(height));
    }

    public static bool EzButton(string text, int height = 16)
    {
        return GUILayout.Button(text, GUILayout.ExpandWidth(false), GUILayout.Height(height));
    }

	public static bool EzToggle(string label, bool variable) {
		return GUILayout.Toggle(variable, label);
	}

	public static bool EzToggle(string label, bool variable, GUILayoutOption opt1) {
		return GUILayout.Toggle(variable, label, opt1);
	}

	public static bool EzToggle(string label, bool variable, GUILayoutOption opt1, GUILayoutOption opt2) {
		return GUILayout.Toggle(variable, label, opt1, opt2);
	}

	public static List<T> EzList<T>(string title, List<T> list, ref bool foldoutFlag) where T : Object 
    {
		using (Vertical()) {
			LookLikeControls(110f, 50f);
			var fullTitle = title + " (" + list.Count + "):";
			foldoutFlag = EzFoldout(fullTitle, foldoutFlag);
			if (!foldoutFlag) return list;

			//Debug.Log("count: "+list.Count);
			T newVar = null;
			using (Horizontal()) {
				EzSpacer(5f);
				using (Vertical()) {
					LookLikeControls(110f, 100f);
					for (int i = 0; i < list.Count; i++) {
						using (Horizontal()) {
							list[i] = EzObjectField<T>("", list[i], 5f) as T;
							if (EzButton(DeleteButton)) {
								list.RemoveAt(i);
								break;
							}
						}
					}
					using (Horizontal()) {
						LookLikeControls(15f, 10f);
						newVar = EzObjectField<T>("+", newVar, 10f) as T;
					}
				}
			}

			if (newVar != null) {
				if (list.Contains(newVar)) {
					Separator();
					return list;
				}
				list.Add(newVar);
				newVar = default(T);
				Separator();
			}
			return list;
		}
	}

    public static Vector3[] EzV3Array(string title, Vector3[] array, ref Vector3 newVar, ref bool foldoutFlag) 
    {
        using (Vertical()) {
            LookLikeControls(110f, 50f);
            var fullTitle = title + " (" + array.Length + "):";
            foldoutFlag = EzFoldout(fullTitle, foldoutFlag);
            if (!foldoutFlag) return array;

            using (Horizontal()) {
                EzSpacer(5f);
                using (Vertical()) {
                    //LookLikeControls(110f, 100f);
                    for (int i = 0; i < array.Length; i++) {
                        using (Horizontal()) {
                            array[i] = EzV3Field("", array[i], 0f, GUILayout.Height(22f));
							if (EzButton(DeleteButton)) {
                                array = array.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    Separator();
                    using (Horizontal()) {
                        LookLikeControls(15f, 10f);
						if (EzButton(AddButton))
                            array = array.Add(newVar);
                        //break
                        newVar = EzV3Field("", newVar, 10f);
                    }
                }
            }
            return array;
        }
    }

	public static T[] EzObjectArray<T> (string title, T[] array, ref T newVar, ref bool foldoutFlag) where T: Object
	{
		using (Vertical ()) {
			LookLikeControls (110f, 50f);
			var fullTitle = title + " (" + array.Length + "):";
			foldoutFlag = EzFoldout (fullTitle, foldoutFlag);
			if (!foldoutFlag)
				return array;

			using (Horizontal ()) {
				EzSpacer (5f);
				using (Vertical ()) {
					//LookLikeControls(110f, 100f);
					for (int i = 0; i < array.Length; i++) {
						using (Horizontal ()) {
							array[i] = EzObjectField ("", array[i], 0f) as T;
							if (EzButton (DeleteButton)) {
								array = array.RemoveAt (i);
								//break;
							}
						}
					}
					Separator ();
					using (Horizontal ()) {
						LookLikeControls (15f, 10f);
						EzLabel("+", 0f, GUILayout.MaxWidth(12f));
						newVar = EzObjectField ("", newVar, 10f) as T;
					}
					if (newVar != null) {
						array = array.Add (newVar);
						newVar = null;
					}
				}
			}
			return array;
		}		
	}
}

} //namespace EzEditor
