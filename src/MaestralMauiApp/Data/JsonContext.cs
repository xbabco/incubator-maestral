// Copyright © 2025 xbabco. All rights reserved.

using System.Text.Json.Serialization;
using MaestralMauiApp.Models;

[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(ProjectTask))]
[JsonSerializable(typeof(ProjectsJson))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Tag))]
public partial class JsonContext : JsonSerializerContext { }
