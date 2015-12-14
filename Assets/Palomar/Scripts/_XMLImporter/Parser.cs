using System.Xml.Serialization;
using UnityEngine;
using System.Collections;

/// <summary>
/// Parser classes common to multiple graph parsers
/// </summary>
public class Parser : MonoBehaviour {

	public class Subtitle
	{
		[XmlAttribute ("text")]
		public string Text;

		[XmlAttribute ("size")]
		public int Size;

		[XmlAttribute ("posX")]
		public float? PosX;

		[XmlAttribute ("posY")]
		public float? PosY;
	}

	public class Point
	{
	    [XmlAttribute("x")]
	    public float X;
	
	    [XmlAttribute("y")]
	    public float Y;
	
		public Point() {
			X = 0f;
			Y = 0f;
		}
	}

	public class Rotation
	{
	    [XmlAttribute("in")]
	    public string In;
	
	    [XmlAttribute("out")]
	    public string Out;
	
	    [XmlAttribute("duration")]
	    public int Duration;
	
	    [XmlAttribute("easeType")]
	    public string EaseType;
	}

	public class Ruler
	{
		[XmlAttribute ("along")]
		public string Along;

		[XmlAttribute ("lineCount")]
		public int LineCount;

		[XmlAttribute ("stepCount")]
		public float StepCount;

		[XmlAttribute ("minValue")]
		public float MinValue;

		[XmlAttribute ("padIn")]
		public float PadIn;

		[XmlAttribute ("padOut")]
		public float PadOut;

		[XmlAttribute ("startX")]
		public float StartX;

		[XmlAttribute ("startY")]
		public float StartY;

		[XmlAttribute ("endY")]
		public float EndY;

		[XmlAttribute ("stepX")]
		public float StepX;

		[XmlAttribute ("stepY")]
		public float StepY;

		[XmlAttribute ("markerTextMin")]
		public float MarkerTextMin;

		[XmlAttribute ("markerTextMax")]
		public float MarkerTextMax;

		[XmlAttribute ("startTime")]
		public float StartTime;

		[XmlAttribute ("duration")]
		public float Duration;

		[XmlAttribute ("easeType")]
		public string EaseType;
	}

	public class BGLines
	{
	    [XmlAttribute("startTime")]
	    public float StartTime;
	
	    [XmlAttribute("duration")]
	    public float Duration;
	
	    [XmlAttribute("stepDelay")]
	    public float StepDelay;
	
	    [XmlAttribute("easeType")]
	    public string EaseType;
	
	    [XmlAttribute("orientation")]
	    public string Orientation;
	
	    [XmlAttribute("startX")]
	    public float StartX;
	
	    [XmlAttribute("endX")]
	    public float EndX;
	
	    [XmlAttribute("startY")]
	    public float StartY;
	
	    [XmlAttribute("endY")]
	    public float EndY;
	
	    [XmlAttribute("stepX")]
	    public int StepX;
	
	    [XmlAttribute("stepY")]
	    public float StepY;
	
	    [XmlAttribute("lineCount")]
	    public int LineCount;
	}
}
