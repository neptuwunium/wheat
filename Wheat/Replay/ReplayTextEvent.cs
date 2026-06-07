// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

namespace Wheat.Replay;

public record struct ReplayTextEvent : IInputEvent {
	public required string Text { get; init; }
	public required ushort ModifierMask { get; init; }
}
