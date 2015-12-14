using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("DonutGraph")]
public class DonutGraphParser {
    public static DonutGraphParser Load(TextAsset textAsset)
    {
        return Serializer<DonutGraphParser>.Deserialize(textAsset.text);
    }

	public static DonutGraphParser Load (WWW xml)
	{
		return Serializer<DonutGraphParser>.Deserialize (xml.text);
	}

    [XmlAttribute("version")]
    public string Version;

    [XmlElement("subtitle1")]
	public Parser.Subtitle Subtitle1;

    [XmlElement("subtitle2")]
	public Parser.Subtitle Subtitle2;

    [XmlElement("subtitle3")]
	public Parser.Subtitle Subtitle3;

    [XmlElement("rotation")]
	public Parser.Rotation Rotation;

    [XmlElement("slices")]
    public SlicesParser Slices;

    public class SliceParser
    {
        [XmlAttribute("startTime")]
        public float StartTime;

        [XmlAttribute("easeType")]
        public string EaseType;

        [XmlAttribute("finalPercent")]
        public int FinalPercent;
    }

    public class SlicesParser
    {
        [XmlAttribute("count")]
        public int Count;

        [XmlAttribute("duration")]
        public float Duration;

        [XmlElement("slice")]
        public List<SliceParser> Slices = new List<SliceParser>();
    }
}
