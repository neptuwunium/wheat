// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

namespace Wheat.Replay;

[Flags]
public enum ReplayDomain {
	None = 0,
	Editor = 1 << 0,
	ClientUI = 1 << 1,
	ClientNoUI = 1 << 2,
	ServerUI = 1 << 3,
	ServerNoUI = 1 << 4,
}
