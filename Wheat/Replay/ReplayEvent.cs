// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics;
using Pluto.IO.Binary;

namespace Wheat.Replay;

public abstract record ReplayEvent : IDisposable {
	// ReSharper disable once UnusedParameter.Local
	protected ReplayEvent(BufferBinaryReader reader, Replay replay) {
		EventType = reader.Read<ReplayEventType>();
		FrameId = reader.Read<uint>();
	}

	public ReplayEventType EventType { get; set; }
	public uint FrameId { get; set; }

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~ReplayEvent() => Dispose(false);

	public static ReplayEvent Read(BufferBinaryReader reader, Replay replay) {
		var type = reader.Peek<ReplayEventType>();

		Debug.Assert(type is not (ReplayEventType.EditorPacket or ReplayEventType.ClientNoUIPacket or ReplayEventType.ServerUIPacket));

		// seems to only send Packet2 (ClientPacket) and Packet5
		return type switch {
			ReplayEventType.PlayerConnect or ReplayEventType.PlayerDisconnect => new ReplayPlayerEvent(reader, replay),
			ReplayEventType.EditorPacket
				or ReplayEventType.ClientUIPacket or ReplayEventType.ClientNoUIPacket
				or ReplayEventType.ServerUIPacket or ReplayEventType.ServerNoUIPacket => new ReplayPacketEvent(reader, replay),
			ReplayEventType.Prefab => new ReplayLoadPrefabEvent(reader, replay),
			ReplayEventType.Input => new ReplayInputEvent(reader, replay),
			ReplayEventType.Custom => new ReplayCustomEvent(reader, replay),
			_ => throw new NotSupportedException(),
		};
	}

	protected virtual void Dispose(bool disposing) { }
}
