// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

namespace Wheat.Replay;

public enum ReplayEventType : byte {
	PlayerConnect = 1,
	PlayerDisconnect,
	Packet1,
	ClientPacket,
	Packet3,
	LoadPrefab,
	Packet4,
	Packet5,
	InputEvent,
	CustomEvent,
}
