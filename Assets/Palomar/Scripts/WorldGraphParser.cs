using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("WorldGraph")]
public class WorldGraphParser
{
    public static WorldGraphParser Load(TextAsset textAsset)
    {
        return Serializer<WorldGraphParser>.Deserialize(textAsset.text);
    }

	public static WorldGraphParser Load (WWW xml)
	{
		return Serializer<WorldGraphParser>.Deserialize (xml.text);
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

    [XmlElement("regions")]
    public RegionsParser Regions;

    public class RegionsParser
    {
	    [XmlAttribute("tweenDuration")]
	    public float Duration;

	    [XmlAttribute("units")]
	    public string Units;

        [XmlElement("region")]
        public List<RegionParser> Region = new List<RegionParser>();
    }

    public class RegionParser
    {
        [XmlAttribute("value")]
        public float Value;

        [XmlAttribute("startTime")]
        public float StartTime;

    }

}
