using System.Xml.Serialization;
using System.Text;
using System.IO;

public class Serializer<T> {

	public string Serialize(){return Serialize (this);}

	protected static XmlSerializer serializer = new XmlSerializer(typeof(T));

	public static string Serialize(object toSerialize){
		StringBuilder builder = new StringBuilder();	
		serializer.Serialize(
			System.Xml.XmlWriter.Create(builder), toSerialize);
		
		return builder.ToString();
	}
	
	public static T Deserialize(string serializedData){
		return (T)serializer.Deserialize(new StringReader(serializedData));
	}
}
