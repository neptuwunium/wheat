// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using Pluto.IO.Binary;

namespace Wheat.Replay;

public record ReplayInputEvent : ReplayEvent {
	public ReplayInputEvent(BufferBinaryReader reader, Replay replay) : base(reader, replay) => Data = reader.ReadSharedBytes(reader.ReadVarInt());

	// this is a message bus packet, refer to InputSchema (or MessageBusSchema?) in header
	public IRentedArray<byte> Data { get; set; }

	protected override void Dispose(bool disposing) {
		if (disposing) {
			Data.Dispose();
		}
	}
}
