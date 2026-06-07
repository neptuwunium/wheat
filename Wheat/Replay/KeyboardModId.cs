namespace Wheat.Replay;

[Flags]
public enum KeyboardModId : byte {
	None,
	Shift = 1 << 0,
	Control = 1 << 1,
	Alt = 1 << 2,
	Meta = 1 << 3,
	Num = 1 << 4,
	Caps = 1 << 5,
	Mode = 1 << 6,
	Scroll = 1 << 7,
}
