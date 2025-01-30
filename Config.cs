using System.ComponentModel;
using mrfpc.patch.outfitseverywhere.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;

namespace mrfpc.patch.outfitseverywhere.Configuration
{
    public class Config : Configurable<Config>
    {
        [DisplayName("Free Roam")]
        [Description("Applies combat outfits in Free Roam mode")]
        [DefaultValue(true)]
        public bool FieldOutfits { get; set; } = true;

        [DisplayName("Cutscenes / Events")]
        [Description("Applies combat outfits during ingame cutscenes/events")]
        [DefaultValue(true)]
        public bool EventOutfits { get; set; } = true;

        [DisplayName("Randomized Outfits")]
        [Description("Will give the player a random outfit")]
        [DefaultValue(true)]
        public bool RandomOutfits { get; set; } = true;
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        // 
    }
}
