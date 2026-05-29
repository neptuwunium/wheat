// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

namespace Wheat.RTTR;

public record AssetDepsCompact {
	public string Asset { get; set; } = null!;
	public List<int> Dependencies { get; set; } = [];

	public override string ToString() => $"{nameof(AssetDepsCompact)} {{ asset = \"{Asset}\", {Dependencies.Count} dependencies }}";
}