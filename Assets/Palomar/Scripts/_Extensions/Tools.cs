using UnityEngine;

public static class Tools {

	public static void EnableRenderers (bool state, Renderer[] renderers, Canvas canvas)
	{
		if (!Application.isPlaying)
			return;
		if (renderers != null)
			foreach (var rend in renderers)
				rend.enabled = state;
		if (canvas)
			canvas.enabled = state;
	}

}
