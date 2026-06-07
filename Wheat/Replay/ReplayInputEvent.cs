// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Pluto.IO.Binary;

namespace Wheat.Replay;

public record ReplayInputEvent : ReplayEvent {
	private const float NORM = 1.0f / 0x8000;

	public ReplayInputEvent(BufferBinaryReader reader, Replay replay) : base(reader, replay) {
		var length = reader.ReadVarInt();
		if (length > 0x403) {
			throw new InvalidOperationException();
		}

		var buffer = (stackalloc byte[length]);
		reader.ReadBytes(buffer);

		var type = (PacketInputType) buffer[0];
		var mask = MemoryMarshal.Read<ushort>(buffer[1..]);

		switch (type) {
			case PacketInputType.None: {
				Event = new ReplayBlankInputEvent {
					ModifierMask = mask,
				};
				break;
			}
			case PacketInputType.Keyboard: {
				var keyId = MemoryMarshal.Read<KeyboardKeyId>(buffer[3..]);
				var modId = (KeyboardModId) buffer[5];
				var flags = (PacketKeyboardFlags) buffer[6];
				Event = new ReplayKeyboardEvent {
					ModifierMask = mask,
					KeyId = keyId,
					ModId = modId,
					Flags = flags,
				};
				break;
			}
			case PacketInputType.MouseButton: {
				var pos = new Vector2(
					MemoryMarshal.Read<short>(buffer[3..]) * NORM,
					MemoryMarshal.Read<short>(buffer[5..]) * NORM
				);

				var info = new PacketButtonInfo(buffer[7]);
				var clicks = buffer[8];
				Event = new ReplayMouseButtonEvent {
					ModifierMask = mask,
					Position = pos,
					ButtonInfo = info,
					Clicks = clicks,
				};
				break;
			}
			case PacketInputType.MouseMove: {
				var pos = new Vector2(
					MemoryMarshal.Read<short>(buffer[3..]) * NORM,
					MemoryMarshal.Read<short>(buffer[5..]) * NORM
				);
				var delta = new Vector2(
					MemoryMarshal.Read<short>(buffer[7..]) * NORM,
					MemoryMarshal.Read<short>(buffer[9..]) * NORM
				);

				Event = new ReplayMouseMoveEvent {
					ModifierMask = mask,
					Position = pos,
					Delta = delta,
				};
				break;
			}
			case PacketInputType.MouseWheel: {
				var delta = new Vector2(
					MemoryMarshal.Read<short>(buffer[3..]) * NORM,
					MemoryMarshal.Read<short>(buffer[5..]) * NORM
				);

				Event = new ReplayMouseWheelEvent {
					ModifierMask = mask,
					Delta = delta,
				};
				break;
			}
			case PacketInputType.TextInput: {
				var textLength = MemoryMarshal.Read<ushort>(buffer[3..]);
				if (textLength >= 0x400 || textLength > buffer.Length - 5) {
					throw new InvalidDataException();
				}

				Event = new ReplayTextEvent {
					ModifierMask = mask,
					Text = Encoding.UTF8.GetString(buffer[5..]),
				};
				break;
			}
			default: throw new NotSupportedException();
		}
	}

	public IInputEvent Event { get; }
}
