// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using Pluto.SourceGen.BitStructGenerator;

namespace Wheat.Replay;

[BitStruct(1)]
public partial struct PacketButtonInfo {
	[BitField(7)] public partial MouseButtonType Type { get; set; }
	[BitField(1)] public partial bool IsPressed { get; set; }
}
