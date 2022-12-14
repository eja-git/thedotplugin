using ImGuiNET;
using System;
using System.Numerics;

namespace TheDotPlugin;

// It is good to have this be disposable in general, in case you ever need it
// to do any cleanup
class TheDotPluginUI : IDisposable
{
    private Configuration configuration;

    // this extra bool exists for ImGui, since you can't ref a property
    private bool visible = false;
    public bool Visible
    {
        get { return this.visible; }
        set { this.visible = value; }
    }

    private bool settingsVisible = false;
    public bool SettingsVisible
    {
        get { return this.settingsVisible; }
        set { this.settingsVisible = value; }
    }

    // passing in the image here just for simplicity
    public TheDotPluginUI(Configuration configuration)
    {
        this.configuration = configuration;
    }

    public void Dispose()
    {
    }

    public void Draw()
    {
        // This is our only draw handler attached to UIBuilder, so it needs to be
        // able to draw any windows we might have open.
        // Each method checks its own visibility/state to ensure it only draws when
        // it actually makes sense.
        // There are other ways to do this, but it is generally best to keep the number of
        // draw delegates as low as possible.

        DrawSettingsWindow();
    }

    public void DrawSettingsWindow()
    {
        if (!SettingsVisible)
        {
            return;
        }

        ImGui.SetNextWindowSize(new Vector2(232, 100), ImGuiCond.Always);
        if (ImGui.Begin("The Dot Configuration Window", ref this.settingsVisible,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            // can't ref a property, so use a local copy
            var showDotValue = this.configuration.ShowDotAlways;
            if (ImGui.Checkbox("Show Dot Always", ref showDotValue))
            {
                this.configuration.ShowDotAlways = showDotValue;
                // can save immediately on change, if you don't want to provide a "Save and Close" button
                this.configuration.Save();
            }

            var dotSize = this.configuration.DotSize;
            if (ImGui.InputFloat("Dot Size", ref dotSize))
            {
                this.configuration.DotSize = dotSize;
                this.configuration.Save();
            }
        }
        ImGui.End();
    }
}