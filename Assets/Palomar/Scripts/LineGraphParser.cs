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
	public ParserRuler VerticalRuler;

	[XmlElement("horizontalRuler")]
	public ParserRuler HorizontalRuler;

	[XmlElement("subtitle1")]
	public ParserSubtitle Subtitle1;

	[XmlElement("subtitle2")]
	public ParserSubtitle Subtitle2;
	
	[XmlElement("subtitle3")]
	public ParserSubtitle Subtitle3;

	public ParserGraphData Data;

    public class ParserRuler {
        [XmlAttribute("stepCount")]
        public int StepCount;

        [XmlAttribute("minValue")]
        public float MinValue;

        [XmlAttribute("padIn")]
        public float PadIn;

        [XmlAttribute("padOut")]
        public float PadOut;
    }

    public class ParserSubtitle
    {
        [XmlAttribute("text")]
        public string Text;
    }

    public class ParserMultiLines
    {
        [XmlAttribute("lineCount")]
        public int lineCount;
    }

    public class ParserGraphData
    {
        [XmlArray("data")]
        [XmlArrayItem("point")]
        public List<ParserDataPoint> points = new List<ParserDataPoint>();
    }

    public class ParserDataPoint
    {
        [XmlAttribute("x")]
        public float x;

        [XmlAttribute("y")]
        public float y;
    }
}