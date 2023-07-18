using p5s.uiToggler.Configuration;
using p5s.uiToggler.Template;
using p5s.uiToggler.Utilities;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using SharpDX.DirectInput;
using System.Diagnostics;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;

namespace p5s.uiToggler;
/// <summary>
/// Your mod logic goes here.
/// </summary>
public unsafe class Mod : ModBase // <= Do not Remove.
{
    /// <summary>
    /// Provides access to the mod loader API.
    /// </summary>
    private readonly IModLoader _modLoader;

    /// <summary>
    /// Provides access to the Reloaded.Hooks API.
    /// </summary>
    /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
    private readonly IReloadedHooks? _hooks;

    /// <summary>
    /// Provides access to the Reloaded logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Entry point into the mod, instance that created this class.
    /// </summary>
    private readonly IMod _owner;

    /// <summary>
    /// Provides access to this mod's configuration.
    /// </summary>
    private Config _configuration;

    /// <summary>
    /// The configuration of the currently executing mod.
    /// </summary>
    private readonly IModConfig _modConfig;

    private IHook<UIMainRenderDelegate> _uiRenderHook;
    private bool _uiEnabled = true;

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;

        Utils.Initialise(_logger, _configuration, _modLoader);

        Utils.SigScan("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 41 54 41 55 41 56 41 57 48 83 EC 40 49 8B 00",
            "UI Main Render", address =>
        {
            _uiRenderHook = _hooks.CreateHook<UIMainRenderDelegate>(UIMainRender, address).Activate();
        });

        Task.Run(() => InputHook());
    }

    private nuint UIMainRender(nuint param_1, nuint* param_2, nuint* param_3, char param_4, uint* param_5)
    {
        if (_uiEnabled)
            return _uiRenderHook.OriginalFunction(param_1, param_2, param_3, param_4, param_5);
        return 0;
    }

    private void InputHook()
    {
        var directInput = new DirectInput();
        var keyboard = new Keyboard(directInput);

        // Acquire the joystick
        keyboard.Properties.BufferSize = 128;
        keyboard.Acquire();

        // Poll events from keyboard
        while (true)
        {
            keyboard.Poll();
            var datas = keyboard.GetBufferedData();
            foreach (var state in datas)
            {
                if (state.Key == _configuration.ToggleKey && state.IsPressed)
                {
                    Utils.LogDebug("Toggled UI");
                    _uiEnabled = !_uiEnabled;
                }
            }
        }
    }

    [Function(CallingConventions.Microsoft)]
    private delegate nuint UIMainRenderDelegate(nuint param_1, nuint* param_2, nuint* param_3, char param_4, uint* param_5);

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}