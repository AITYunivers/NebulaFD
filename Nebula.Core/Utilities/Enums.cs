namespace Nebula.Core.Utilities
{
    public static class Enums
    {
        public enum AppHeaderFlags
        {
            /// <summary>
            /// Heading When Maximized
            /// </summary>
            HeadingWhenMaximized,

            /// <summary>
            /// Heading is disabled
            /// </summary>
            HeadingDisabled,

            /// <summary>
            /// Fit inside (black bars)
            /// </summary>
            FitInside,

            /// <summary>
            /// Machine-independent speed
            /// </summary>
            MachineIndependentSpeed,

            /// <summary>
            /// Resize display to fill window size
            /// </summary>
            ResizeDisplay,

            /// <summary>
            /// Menu displayed on boot-up is disabled
            /// </summary>
            MenuDisplayedDisabled = 7,

            /// <summary>
            /// Menu Bar
            /// </summary>
            MenuBar,

            /// <summary>
            /// Maximized on boot-up
            /// </summary>
            MaximizedOnBoot,

            /// <summary>
            /// Multi-Samples
            /// </summary>
            MultiSamples,

            /// <summary>
            /// Change Resolution Mode
            /// </summary>
            ChangeResolutionMode,

            /// <summary>
            /// Allow user to switch to/from full screen
            /// </summary>
            AllowFullscreenSwitch
        };

        public enum AppHeaderNewFlags
        {
            /// <summary>
            /// Play sounds over frames
            /// </summary>
            PlaySoundsOverFrames,

            /// <summary>
            /// Do not mute samples when application loses focus
            /// </summary>
            DontMuteOnLostFocus = 3,

            /// <summary>
            /// No Minimize box
            /// </summary>
            NoMinimizeBox,

            /// <summary>
            /// No Maximize box
            /// </summary>
            NoMaximizeBox,

            /// <summary>
            /// No Thick frame
            /// </summary>
            NoThickFrame,

            /// <summary>
            /// Do not center frame area in window
            /// </summary>
            DontCenterFrame,

            /// <summary>
            /// Do not stop screen saver when input event
            /// </summary>
            DontStopScreenSaver,

            /// <summary>
            /// Disable Close button
            /// </summary>
            DisableCloseButton,

            /// <summary>
            /// Hidden at start
            /// </summary>
            HiddenAtStart,

            /// <summary>
            /// Enable Visual Themes
            /// </summary>
            EnableVisualThemes,

            /// <summary>
            /// V-Sync
            /// </summary>
            VSync,

            /// <summary>
            /// Run when minimized
            /// </summary>
            RunWhenMinimized,

            /// <summary>
            /// Run while resizing
            /// </summary>
            RunWhileResizing = 15
        };

        public enum AppHeaderOtherFlags
        {
            /// <summary>
            /// Enable debugger keyboard shortcuts
            /// </summary>
            DebuggerShortcuts,

            /// <summary>
            /// Do not share data if run as sub-application
            /// </summary>
            DontShareSubAppData = 3,

            /// <summary>
            /// Include external files
            /// </summary>
            IncludeExternalFiles = 6,

            /// <summary>
            /// Show Debugger
            /// </summary>
            ShowDebugger,

            /// <summary>
            /// Display Mode: Direct 3D 9 / Direct 3D 11
            /// </summary>
            Direct3D9or11 = 14,

            /// <summary>
            /// Display Mode: Direct 3D 8 / Direct 3D 11
            /// </summary>
            Direct3D8or11
        };
    }
}
