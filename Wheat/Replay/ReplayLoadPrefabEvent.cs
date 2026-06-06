// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using Pluto.IO.Binary;

namespace Wheat.Replay;

public record ReplayLoadPrefabEvent : ReplayEvent {
	public ReplayLoadPrefabEvent(BufferBinaryReader reader, Replay replay) : base(reader, replay) {
		PrefabPath = reader.ReadVarString();
		ChildIndex = reader.Read<uint>();
		var size = reader.ReadVarInt();
		PrefabInitData = reader.ReadSharedBytes(size);
	}

	// unhashed asset path
	public string PrefabPath { get; set; }
	public uint ChildIndex { get; set; }
	// raw prefab init data
	public IRentedArray<byte> PrefabInitData { get; set; }

	protected override void Dispose(bool disposing) {
		if (disposing) {
			PrefabInitData.Dispose();
		}
	}
}
