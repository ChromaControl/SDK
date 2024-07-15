// Licensed to the Chroma Control Contributors under one or more agreements.
// The Chroma Control Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using System.Text.Json.Nodes;

namespace ChromaControl.SDK.OpenRGB.Internal.Extensions;

internal static class JsonNodeExtensions
{
    public static void Merge(this JsonNode current, JsonNode merge)
    {
        var mergeObject = merge.AsObject();

        foreach (var mergeNode in mergeObject)
        {
            if (mergeNode.Value is null)
            {
                current[mergeNode.Key] = null;
            }
            else
            {
                if (current[mergeNode.Key] is null)
                {
                    current[mergeNode.Key] = mergeNode.Value.DeepClone();
                }
                else
                {
                    if (mergeNode.Value.GetValueKind() is JsonValueKind.Object)
                    {
                        current[mergeNode.Key]!.Merge(mergeNode.Value);
                    }
                    else
                    {
                        current[mergeNode.Key] = mergeNode.Value.DeepClone();
                    }
                }
            }
        }
    }
}
