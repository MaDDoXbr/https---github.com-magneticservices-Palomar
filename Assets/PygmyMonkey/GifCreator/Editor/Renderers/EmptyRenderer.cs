using UnityEditor;
using UnityEngine;
using PygmyMonkey.Utils;

namespace PygmyMonkey.GifCreator
{
	public class EmptyRenderer
	{
		private GifCreatorWindow gifCreatorWindow;

		public EmptyRenderer(GifCreatorWindow gifCreatorWindow)
		{
			this.gifCreatorWindow = gifCreatorWindow;
		}

		private void assignCallbacks()
		{
			if (Recorder.Singleton == null)
			{
				return;
			}

			Recorder.Singleton.OnRecordingProgress += delegate(float percent)
			{
				gifCreatorWindow.RecordRenderer.OnRecordingProgress(percent);
			};

			Recorder.Singleton.OnRecordingDone += delegate()
			{
				gifCreatorWindow.RecordRenderer.OnRecordingDone();
				gifCreatorWindow.PreviewRenderer.OnRecordingDone();
				gifCreatorWindow.ParametersRenderer.OnRecordingDone();
			};

			Recorder.Singleton.OnPreProcessingDone += delegate()
			{
				gifCreatorWindow.RecordRenderer.OnPreProcessingDone();
			};

			Recorder.Singleton.OnFileSaveProgress += delegate(int threadId, float percent)
			{
				gifCreatorWindow.RecordRenderer.OnFileSaveProgress(threadId, percent);
			};

			Recorder.Singleton.OnFileSaved += delegate(int threadId, string filePath)
			{
				gifCreatorWindow.RecordRenderer.OnFileSaved(threadId, filePath);
				gifCreatorWindow.ExportRenderer.OnFileSaved(threadId, filePath);
			};
		}

		public void Update()
		{
			if (gifCreatorWindow.Parameters.camera == null)
			{
				OnDestroy();
			}

			if (Recorder.Singleton == null && gifCreatorWindow.Parameters.camera != null)
			{
				createRecorder();
			}

			if (CustomRectPreview.Singleton == null)
			{
				createCustomRectPreview();
			}
		}

		public void OnPlayModeChange(bool isEditorPlaying)
		{
			if (isEditorPlaying)
			{
				assignCallbacks();
			}
			else
			{
				gifCreatorWindow.Reset();
			}
		}

		private Camera lastCamera;
		public void drawInspector()
		{
			if (GUIUtils.DrawHeader("Camera"))
			{
				using (new GUIUtils.GUIContents())
				{
					bool isUIEnabled = Recorder.Singleton == null || Recorder.Singleton.State == RecorderState.None;
					using (new GUIUtils.GUIEnabled(isUIEnabled))
					{
						if (gifCreatorWindow.Parameters.camera == null)
						{
							EditorGUILayout.HelpBox("You first need to specify a camera that will be use to record things on screen", MessageType.Warning);
						}
						
						EditorGUI.BeginChangeCheck();
						gifCreatorWindow.Parameters.camera = (Camera)EditorGUILayout.ObjectField("Camera", gifCreatorWindow.Parameters.camera, typeof(Camera), true);
						if (EditorGUI.EndChangeCheck())
						{
							if (gifCreatorWindow.Parameters.camera == null)
							{
								OnDestroy();
							}
							else
							{
								lastCamera = gifCreatorWindow.Parameters.camera;
							}
						}
						
						gifCreatorWindow.Parameters.recordEntireGameView = EditorGUILayout.Toggle(new GUIContent("Record entire Game view", "This is usually useful if you have a Canvas using 'Screen Space - Overlay' or if you're using multiple cameras"), gifCreatorWindow.Parameters.recordEntireGameView);
						
						if (gifCreatorWindow.Parameters.recordEntireGameView)
						{
							EditorGUILayout.HelpBox("You decided to record the entire Game view. Note that this is a really slow method, that will impact FPS if used on a large complex scene. But it will record everything in the Game view (multiple cameras, UI etc...) and not just one camera.", MessageType.Warning);
						}
					}
				}
			}
		}

		public void OnDestroy()
		{
			if (gifCreatorWindow.Parameters.camera != null)
			{
				Component.DestroyImmediate(gifCreatorWindow.Parameters.camera.GetComponent<Recorder>());
				Component.DestroyImmediate(gifCreatorWindow.Parameters.camera.GetComponent<CustomRectPreview>());
			}

			if (lastCamera != null)
			{
				Component.DestroyImmediate(lastCamera.GetComponent<Recorder>());
				Component.DestroyImmediate(lastCamera.GetComponent<CustomRectPreview>());
			}

			if (Recorder.Singleton != null)
			{
				Component.DestroyImmediate(Recorder.Singleton);
			}

			if (CustomRectPreview.Singleton != null)
			{
				Component.DestroyImmediate(CustomRectPreview.Singleton);
			}
		}

		private void createRecorder()
		{
			if (gifCreatorWindow.Parameters.camera == null)
			{
				return;
			}

			Recorder recorder = gifCreatorWindow.Parameters.camera.gameObject.GetComponent<Recorder>();
			if (recorder == null)
			{
				recorder = gifCreatorWindow.Parameters.camera.gameObject.AddComponent<Recorder>();
				recorder.hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInInspector;
				//recorder.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
			}

			recorder.Init(gifCreatorWindow.Parameters);
			Recorder.Singleton = recorder;
		}

		private void createCustomRectPreview()
		{
			if (gifCreatorWindow.Parameters.camera == null)
			{
				return;
			}

			CustomRectPreview customRectPreview = gifCreatorWindow.Parameters.camera.gameObject.GetComponent<CustomRectPreview>();
			if (customRectPreview == null)
			{
				customRectPreview = gifCreatorWindow.Parameters.camera.gameObject.AddComponent<CustomRectPreview>();
				customRectPreview.hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInInspector;
				//customRectPreview.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
			}

			CustomRectPreview.Singleton = customRectPreview;
		}
	}
}
