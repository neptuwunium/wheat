namespace Wheat.Replay;

[Flags]
public enum PacketKeyboardFlags : byte {
	None = 0,
	Pressed = 1 << 0,
	Repeated = 1 << 1,
}
