using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using ImGuiNET;
using Num = System.Numerics;

namespace TheDotPlugin;

public sealed class TheDotPlugin : IDalamudPlugin
{
    public string Name => "TheDot Plugin";

    private const string commandName = "/thedot";

    private DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    private Configuration Configuration { get; init; }
    private TheDotPluginUI PluginUi { get; init; }

    private ClientState ClientState { get; init; }
    private GameGui GameGui { get; init; }
    private Condition Condition { get; init; }
    
    public TheDotPlugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager,
        [RequiredVersion("1.0")] ClientState clientState,
        [RequiredVersion("1.0")] GameGui gameGui,
        [RequiredVersion("1.0")] Condition condition)
    {
        PluginInterface = pluginInterface;
        CommandManager = commandManager;
        ClientState = clientState;
        GameGui = gameGui;
        Condition = condition;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        // you might normally want to embed resources and load them from the manifest stream
        // var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
        // var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);
        PluginUi = new TheDotPluginUI(Configuration);

        CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "uh, it's a dot..."
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void Dispose()
    {
        this.PluginUi.Dispose();
        this.CommandManager.RemoveHandler(commandName);
    }

    private void OnCommand(string command, string args)
    {
        DrawConfigUI();
    }

    private void DrawUI()
    {
        this.PluginUi.Draw();
        var actor = ClientState.LocalPlayer;
        
        if (actor == null)
            return;

        if (!Condition[ConditionFlag.InCombat] && !Configuration.ShowDotAlways)
            return;

        if (!GameGui.WorldToScreen(
                new Num.Vector3(actor.Position.X, actor.Position.Y, actor.Position.Z),
                out var pos)) return;
        
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0, 0));
        ImGuiHelpers.ForceNextWindowMainViewport();
        ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Num.Vector2(0, 0));
        ImGui.Begin("Dot",
            ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
        ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
        
        ImGui.GetWindowDrawList().AddCircleFilled(
            new Num.Vector2(pos.X, pos.Y),
            4f,
            ImGui.GetColorU32(new Num.Vector4(0f, 1.0f, 0f, 1.0f)),
            100);
    }

    private void DrawConfigUI()
    {
        PluginUi.SettingsVisible = true;
    }
}