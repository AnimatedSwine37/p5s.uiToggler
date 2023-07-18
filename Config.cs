using p5s.uiToggler.Template.Configuration;
using SharpDX.DirectInput;
using System.ComponentModel;

namespace p5s.uiToggler.Configuration;
public class Config : Configurable<Config>
{
    [DisplayName("Toggle Key")]
    [Description("The key to press to toggle the UI")]
    [DefaultValue(Key.F2)]
    public Key ToggleKey { get; set; } = Key.F2;

    [DisplayName("Debug Mode")]
    [Description("Logs additional information to the console that is useful for debugging.")]
    [DefaultValue(false)]
    public bool DebugEnabled { get; set; } = false;
}

/// <summary>
/// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
/// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
/// </summary>
public class ConfiguratorMixin : ConfiguratorMixinBase
{
    // 
}
