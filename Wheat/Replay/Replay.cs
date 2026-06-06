// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using Pluto;
using Pluto.IO.Binary;

namespace Wheat.Replay;

public sealed class Replay : IDisposable {
	public Replay(BufferBinaryReader reader) {
		CustomTypes = ObjectPool<Dictionary<int, string>>.Rent();
		Events = ObjectPool<List<ReplayEvent>>.Rent();
		CustomTypes.Clear();
		Events.Clear();

		Header = new ReplayHeader(reader);
		while (reader.Unconsumed > 0) {
			Events.Add(ReplayEvent.Read(reader, this));
		}
	}

	public ReplayHeader Header { get; set; }
	public Dictionary<int, string> CustomTypes { get; set; }
	public List<ReplayEvent> Events { get; set; }

	public void Dispose() {
		foreach (var ev in Events) {
			ev.Dispose();
		}

		Events.Clear();
		CustomTypes.Clear();

		ObjectPool<List<ReplayEvent>>.Return(Events);
		ObjectPool<Dictionary<int, string>>.Return(CustomTypes);

		Events = null!;
		CustomTypes = null!;
	}
}
