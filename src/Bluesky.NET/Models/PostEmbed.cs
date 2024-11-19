﻿using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class PostEmbed
{
    [JsonPropertyName("$type")]
    public string Type { get; init; } = string.Empty;

    public ImageEmbed[]? Images { get; init; }

    public ExternalEmbed? External { get; init; }
}