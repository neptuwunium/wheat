// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using Pluto.IO.Binary;

namespace Wheat.Replay;

public record ReplayCustomEvent : ReplayEvent {
	public ReplayCustomEvent(BufferBinaryReader reader, Replay replay) : base(reader, replay) {
		TypeId = reader.Read<byte>();

		if (!replay.CustomTypes.TryGetValue(TypeId, out var value)) {
			value = replay.CustomTypes[TypeId] = reader.ReadVarString();
		}

		TypeName = value;

		var size = reader.ReadVarInt();
		EventData = reader.ReadSharedBytes(size);
	}

	public int TypeId { get; set; }
	// look me up in rttr
	public string TypeName { get; set; }
	// raw event type data
	public IRentedArray<byte> EventData { get; set; }

	protected override void Dispose(bool disposing) {
		if (disposing) {
			EventData.Dispose();
		}
	}
}
