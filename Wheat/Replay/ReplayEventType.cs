// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

namespace Wheat.Replay;

public enum ReplayEventType : byte {
	PlayerConnect = 1,
	PlayerDisconnect,
	EditorPacket,
	ClientUIPacket,
	ClientNoUIPacket,
	Prefab,
	ServerUIPacket,
	ServerNoUIPacket,
	Input,
	Custom,
}
