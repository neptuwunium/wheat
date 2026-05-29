// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

namespace Wheat.RTTR;

// todo: dump and codegen this probably.

// ReSharper disable once InconsistentNaming
public record AssetDepsDBCompact : RTTRJsonObject {
	public List<AssetDepsCompact> Assets { get; set; } = [];
	public override bool Validate() => RTTRType == nameof(AssetDepsDBCompact);

	public override string ToString() => $"{nameof(AssetDepsDBCompact)} {{ {Assets.Count} assets }}";
}
