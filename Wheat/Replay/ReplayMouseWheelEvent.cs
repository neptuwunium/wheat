// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using System.Numerics;

namespace Wheat.Replay;

public record struct ReplayMouseWheelEvent : IInputEvent {
	public required Vector2 Delta { get; init; }
	public required ushort ModifierMask { get; init; }
}
