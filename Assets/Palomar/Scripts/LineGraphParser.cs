using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("LineGraph")]
public class LineGraphParser
{
    public static LineGraphParser Load(TextAsset textAsset) {
		return Serializer<LineGraphParser>.Deserialize (textAsset.text);
    }

	[XmlElement("verticalRuler")]
    public RulerParser VerticalRuler;

	[XmlElement("horizontalRuler")]
    public RulerParser HorizontalRuler;

	[XmlElement("subtitle1")]
    public SubtitleParser Subtitle1;

	[XmlElement("subtitle2")]
    public SubtitleParser Subtitle2;
	
	[XmlElement("subtitle3")]
    public SubtitleParser Subtitle3;

    [XmlElement("lines")]
    public LinesParser Lines;

    public class RulerParser {
        [XmlAttribute("stepCount")]
        public int StepCount;

        [XmlAttribute("minValue")]
        public float MinValue;

        [XmlAttribute("padIn")]
        public float PadIn;

        [XmlAttribute("padOut")]
        public float PadOut;
    }

    public class SubtitleParser
    {
        [XmlAttribute("text")]
        public string Text;
    }

    public class LinesParser
    {
        [XmlAttribute("lineCount")]
        public int lineCount;

        [XmlElement("line")]
        public List<LineParser> lines = new List<LineParser>(); 
    }

    public class LineParser
    {
        [XmlElement("point")]
        public List<PointParser> points = new List<PointParser>();
    }

    public class PointParser
    {
        [XmlAttribute("x")]
        public float x;

        [XmlAttribute("y")]
        public float y;
    }
}