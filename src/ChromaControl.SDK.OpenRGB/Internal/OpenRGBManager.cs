// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ChromaControl.SDK.OpenRGB.Internal.Extensions;
using ChromaControl.SDK.OpenRGB.Internal.Windows;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

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
        UpdateConfigFile();

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

    private static void UpdateConfigFile()
    {
        var configFilePath = Path.Combine(OpenRGBConstants.ConfigPath, "OpenRGB.json");

        JsonNode? document;

        if (File.Exists(configFilePath))
        {
            var json = File.ReadAllText(configFilePath);
            document = JsonNode.Parse(json)!;
        }
        else
        {
            document = JsonNode.Parse("{\"Detectors\":{\"detectors\":{}}}")!;
        }

        foreach (var detector in OpenRGBConstants.DisabledDetectors)
        {
            document["Detectors"]!["detectors"]![detector] = false;
        }

        try
        {
            var mergeConfigFilePath = Path.Combine(OpenRGBConstants.ConfigPath, "OpenRGB.Merge.json");

            if (File.Exists(mergeConfigFilePath))
            {
                var json = File.ReadAllText(mergeConfigFilePath);
                var mergeDocument = JsonNode.Parse(json);

                if (mergeDocument is not null)
                {
                    document.Merge(mergeDocument);
                }
            }
        }
        catch { }

        var newJson = document.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(configFilePath, newJson);
    }

    private static Process CreateProcess()
    {
        var process = new Process();

        var configPath = OpenRGBConstants.ConfigPath;

        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath);
        }

        configPath = $"{configPath}";

        var arguments = new string[]
        {
            "--server",
            "--server-host 127.0.0.1",
            $"--server-port {OpenRGBConstants.PortNumber}",
            "--noautoconnect",
            $"--config \"{configPath}\""
        };

        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.Arguments = $"/C \"\"{FindExecutablePath()}\" {string.Join(' ', arguments)}\"";

        return process;
    }

    private static string FindExecutablePath()
    {
        var assemblyPath = AppContext.BaseDirectory;
        var runtimeIdentifier = Is32Bit ? "win-x86" : "win-x64";

        var searchPaths = new List<string>()
        {
            Path.Combine(assemblyPath, "ChromaControl.OpenRGB.exe"),
            Path.Combine(assemblyPath, "runtimes", runtimeIdentifier, "native", "ChromaControl.OpenRGB.exe")
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
