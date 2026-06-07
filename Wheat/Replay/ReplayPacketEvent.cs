// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using Pluto;
using Pluto.IO.Binary;

namespace Wheat.Replay;

public record ReplayPacketEvent : ReplayEvent {
	public ReplayPacketEvent(BufferBinaryReader reader, Replay replay) : base(reader, replay) {
		Payloads = ObjectPool<List<IRentedArray<byte>>>.Rent();
		Payloads.Clear();

		PacketId = reader.Read<ulong>();
		var payloadCount = reader.ReadVarInt();

		for (var i = 0; i < payloadCount; i++) {
			var size = reader.ReadVarInt();
			if (size > 0x4000) {
				throw new InvalidDataException("packet too large");
			}

			Payloads.Add(reader.ReadSharedBytes(size));
		}
	}

	public ulong PacketId { get; set; }

	// this is a packet, refer to ReplicationSchema (or MessageBusSchema?) in header
	public List<IRentedArray<byte>> Payloads { get; set; }

	protected override void Dispose(bool disposing) {
		if (disposing) {
			foreach (var payload in Payloads) {
				payload.Dispose();
			}

			Payloads.Clear();
			ObjectPool<List<IRentedArray<byte>>>.Return(Payloads);
			Payloads = null!;
		}
	}
}
