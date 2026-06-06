// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using Pluto.IO.Binary;

namespace Wheat.Replay;

public record ReplayHeader {
	public ReplayHeader(BufferBinaryReader reader) {
		Magic = reader.ReadVarString();
		World = reader.ReadVarString();
		Version = reader.ReadVarString();
		Domain = reader.Read<ReplayDomain>();
		ReplicationHash = reader.Read<ulong>();
		InputHash = reader.Read<ulong>();
		MessageBusHash = reader.Read<ulong>();
		PredefinedReplicablesHash = reader.Read<ulong>();
		Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(reader.Read<long>());
		ReplicationSchema = reader.ReadVarString();
		InputSchema = reader.ReadVarString();
		MessageBusSchema = reader.ReadVarString();
		PredefinedReplicables = reader.ReadVarString();
	}

	public string Magic { get; set; }
	public string World { get; set; }
	public string Version { get; set; }
	public ReplayDomain Domain { get; set; }
	public ulong ReplicationHash { get; set; }
	public ulong InputHash { get; set; }
	public ulong MessageBusHash { get; set; }
	public ulong PredefinedReplicablesHash { get; set; }
	public DateTimeOffset Timestamp { get; set; }
	// the format these schemas are in is a little bit cursed...
	public string ReplicationSchema { get; set; }
	public string InputSchema { get; set; }
	public string MessageBusSchema { get; set; }
	// [replicable_id=index,...]
	public string PredefinedReplicables { get; set; }
}
