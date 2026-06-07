// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using System.Numerics;

namespace Wheat.Replay;

public record struct ReplayMouseButtonEvent : IInputEvent {
	public required Vector2 Position { get; init; }
	public required PacketButtonInfo ButtonInfo { get; init; }
	public required int Clicks { get; init; }
	public required ushort ModifierMask { get; init; }
}
