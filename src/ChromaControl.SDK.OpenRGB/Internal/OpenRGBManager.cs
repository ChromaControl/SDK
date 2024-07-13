// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace ChromaControl.SDK.OpenRGB.Internal;

internal sealed partial class OpenRGBManager : IDisposable
{
    private static bool Is32Bit => nint.Size == 4;

    private readonly Process _process = CreateProcess();
    private readonly Job _job;

    public OpenRGBManager()
    {
        _job = new();
    }

    public void Start()
    {
        _job.AssignProcess(Process.GetCurrentProcess());
        _process.Start();
    }

    public void Stop()
    {
        _process.Kill();
    }

    public void Dispose()
    {
        _process.Kill();
        _process.Dispose();

        _job.Dispose();

        GC.SuppressFinalize(this);
    }

    private static Process CreateProcess()
    {
        var process = new Process();

        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.Arguments = $"/C \"{FindExecutablePath()}\" --noautoconnect --server";

        return process;
    }

    private static string FindExecutablePath()
    {
        var assembly = typeof(OpenRGBManager).Assembly;
        var assemblyFilePath = assembly.Location;
        var assemblyPath = Path.GetDirectoryName(assemblyFilePath) ?? throw new FileNotFoundException("Unable to find the OpenRGB executable.");
        var runtimeIdentifier = Is32Bit ? "win-x86" : "win-x64";

        var searchPaths = new List<string>()
        {
            Path.Combine(assemblyPath, "OpenRGB.exe"),
            Path.Combine(assemblyPath, "runtimes", runtimeIdentifier, "native", "OpenRGB.exe")
        };

        foreach (var searchPath in searchPaths)
        {
            if (File.Exists(searchPath))
            {
                return searchPath;
            }
        }

        throw new FileNotFoundException("Unable to find the OpenRGB executable.");
    }
}
