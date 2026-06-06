// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using System.Text;
using Pluto.IO.Binary;

namespace Wheat.Replay;

public static class ReplayExtensions {
	extension(BufferBinaryReader reader) {
		public int ReadVarInt() {
			var bytes = (stackalloc byte[4]);
			bytes[0] = reader.Read<byte>();

			var n = bytes[0] & 3;
			reader.ReadBytes(bytes.Slice(1, n));

			return (int) ((bytes[0] | ((uint) bytes[1] << 8) | ((uint) bytes[2] << 16) | ((uint) bytes[3] << 24)) >> 2);
		}

		public string ReadVarString() {
			var length = reader.ReadVarInt();
			return reader.ReadCString<byte>(Encoding.UTF8, length, true);
		}
	}
}
