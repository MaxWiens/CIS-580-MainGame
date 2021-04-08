using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TileSheetContentPipeline {
	[ContentImporter(".png", DisplayName = "TileSheet Importer", DefaultProcessor = "TileSheetProcessor")]
	public class TileSheetImporter : ContentImporter<TileSheet> {
		public override TileSheet Import(string filename, ContentImporterContext context) {
			context.Logger.LogMessage("Importing TileSheet file: " + filename);
			using(StreamReader sr = new StreamReader(filename)) {
				TextureImporter imp = new TextureImporter();
				var v = imp.Import(filename, context);
				
			}
			return default(TileSheet);
		}
	}
}
