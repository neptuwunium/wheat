// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

namespace Wheat.Replay;

public record struct ReplayKeyboardEvent : IInputEvent {
	public required KeyboardKeyId KeyId { get; init; }
	public required KeyboardModId ModId { get; init; }
	public required PacketKeyboardFlags Flags { get; init; }
	public required ushort ModifierMask { get; init; }
}
