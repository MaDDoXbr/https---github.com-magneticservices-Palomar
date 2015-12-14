using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class TextWipeData
{
	public Text UIText;
	public int FontSize;
	public string FinalText;
	public Color FinalColor;
	public float StartTime;
	public float Duration = 0f;
	public Ease EaseType;
	public Vector3 Offset;

//	public void AssignTextProperties(string txt, int size) 
//	{
//		AssignTextProperties(txt, size, null, null);
//	}

	public void AssignTextProperties(Parser.Subtitle subt)
	{
		//string txt, int size, float? xPos, float? yPos
		FinalText = subt.Text;
		UIText.text = subt.Text;
		if (subt.PosX != null || subt.PosY != null) {
			var rectTransform = UIText.GetComponent<RectTransform> ();
			var curPos = rectTransform.localPosition;
			if (subt.PosX != null)
				curPos.x = (float)subt.PosX;
			if (subt.PosY != null)
				curPos.y = (float)subt.PosY;
			rectTransform.localPosition = curPos;
		}
		if (subt.Size == 0)
			return;
		UIText.fontSize = subt.Size;
		FontSize = subt.Size;
	}
}
