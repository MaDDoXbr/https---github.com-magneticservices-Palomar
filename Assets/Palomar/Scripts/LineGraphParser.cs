using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("LineGraph")]
public class LineGraphParser
{
    public static LineGraphParser Load(TextAsset textAsset) {
		return Serializer<LineGraphParser>.Deserialize (textAsset.text);
    }

	public static LineGraphParser Load(WWW xml) {
		return Serializer<LineGraphParser>.Deserialize (xml.text);
	}

    [XmlAttribute("version")]
    public string Version;

    [XmlAttribute("lineCount")]
    public int LineCount;

	[XmlElement("subtitle1")]
    public Parser.Subtitle Subtitle1;

	[XmlElement("subtitle2")]
	public Parser.Subtitle Subtitle2;
	
	[XmlElement("subtitle3")]
	public Parser.Subtitle Subtitle3;

    [XmlElement("rotation")]
    public Parser.Rotation Rotation;

    [XmlElement("ruler")]
    public List<Parser.Ruler> Rulers = new List<Parser.Ruler>();

    [XmlElement("bglines")]
	public Parser.BGLines BGLines;

    [XmlElement("lines")]
    public LinesParser Lines;

    public class LinesParser
    {
        [XmlElement("line")]
        public List<LineParser> Line = new List<LineParser>();
    }

    public class LineParser
    {
        [XmlAttribute("color")]
        public string Color;

        [XmlAttribute("startTime")]
        public float StartTime;

        [XmlAttribute("duration")]
        public float Duration;

        [XmlAttribute("easeType")]
        public string EaseType;

	    [XmlAttribute("z")]
	    public string z;

        [XmlElement("point")]
        public List<Parser.Point> Points = new List<Parser.Point>();
    }

//	/// <summary> This changes for each graph script and idx ; eg.: LineGraph1.xml </summary>
//	public string GetImportPath (int idx, SceneData data, bool localFile)
//	{
//		var urlPath = data.LinePath + idx + ".xml";
//		string path;
//
//		if (localFile) {
//			if (Application.isEditor)
//				path = "file://" + Application.dataPath + "/Palomar/" + urlPath;
//			else
//				path = "file://" + Application.dataPath + '/' + urlPath;
//		} else
//			path = urlPath;
//
//		return path;
//	}

}
