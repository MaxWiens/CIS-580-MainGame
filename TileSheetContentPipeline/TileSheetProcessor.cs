using Microsoft.Xna.Framework.Content.Pipeline;

using TInput = System.String;
using TileSheet = System.String;

namespace TileSheetContentPipeline {
	[ContentProcessor(DisplayName = "TileSheet Processor")]
	class TileSheetProcessor : ContentProcessor<TInput, TileSheet> {
		public override TileSheet Process(TInput input, ContentProcessorContext context) {
			return default(TileSheet);
		}
	}
}
