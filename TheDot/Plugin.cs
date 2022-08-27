﻿using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;

namespace TheDotPlugin;

public sealed class TheDotPlugin : IDalamudPlugin
{
    public string Name => "TheDot Plugin";

    private const string commandName = "/thedot";

    private DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    private Configuration Configuration { get; init; }
    private TheDotPluginUI PluginUi { get; init; }

    public TheDotPlugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager)
    {
        this.PluginInterface = pluginInterface;
        this.CommandManager = commandManager;

        this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.Configuration.Initialize(this.PluginInterface);

        // you might normally want to embed resources and load them from the manifest stream
        var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
        var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);
        this.PluginUi = new TheDotPluginUI(this.Configuration, goatImage);

        this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        this.PluginInterface.UiBuilder.Draw += DrawUI;
        this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void Dispose()
    {
        this.PluginUi.Dispose();
        this.CommandManager.RemoveHandler(commandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just display our main ui
        this.PluginUi.Visible = true;
    }

    private void DrawUI()
    {
        this.PluginUi.Draw();
    }

    private void DrawConfigUI()
    {
        this.PluginUi.SettingsVisible = true;
    }
}