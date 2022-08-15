using GLTF.Schema;
using Newtonsoft.Json.Linq;
public class KHR_materials_unlitExtension: Extension
{
	public JProperty Serialize()
	{
		return new JProperty(ExtensionManager.GetExtensionName(typeof(KHR_materials_unlitExtensionFactory)), new JObject());
	}
}