
using Nop.Core.Configuration;

namespace Nop.Plugin.BrainStation.QuickView
{
    public class QuickViewSettings : ISettings
    {

        public bool ShowAlsoPurchased { get; set; }
        public bool ShowRelatedProducts { get; set; }
        public bool EnableWidget { get; set; }
        public bool EnableEnlargePicture { get; set; }
        public string ButtonContainerName { get; set; }
        

    }
}