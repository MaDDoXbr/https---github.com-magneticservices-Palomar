using UnityEngine;
using System;
using System.Xml;
using System.IO;

[ExecuteInEditMode]
[Serializable]
public static class RyXmlTools
{
	public static XmlDocument loadXml (TextAsset xmlFile)
	{
		MemoryStream assetStream = new MemoryStream (xmlFile.bytes);
		XmlReader reader = XmlReader.Create (assetStream);
		XmlDocument xmlDoc = new XmlDocument ();
		try {
			xmlDoc.Load (reader);
		} catch (Exception ex) {
			Debug.Log ("Error loading " + xmlFile.name + ":\n" + ex);
		} finally {
			Debug.Log (xmlFile.name + " loaded");
		}

		return xmlDoc;
	}

	public static void writeXml (string filepath, XmlDocument xmlDoc)
	{
		if (File.Exists (filepath)) {
			using (TextWriter sw = new StreamWriter (filepath, false, System.Text.Encoding.UTF8)) //Set encoding
             {
				xmlDoc.Save (sw);
			}
		}
	}
}
