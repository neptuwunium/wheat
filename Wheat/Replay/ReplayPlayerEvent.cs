// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using Pluto.IO.Binary;

namespace Wheat.Replay;

public record ReplayPlayerEvent : ReplayEvent {
	public ReplayPlayerEvent(BufferBinaryReader reader, Replay replay) : base(reader, replay) => PlayerId = reader.Read<byte>();

	public int PlayerId { get; set; }
}
