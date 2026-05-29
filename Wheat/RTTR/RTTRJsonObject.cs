// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wheat.RTTR;

public abstract record RTTRJsonObject {
	public static JsonSerializerOptions Options { get; } = new() {
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
	};

	[JsonPropertyName("__type__")]
	public string RTTRType { get; set; } = null!;

	public abstract bool Validate();
}
