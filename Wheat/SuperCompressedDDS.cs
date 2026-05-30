// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using System.Runtime.InteropServices;
using Charon.Compression;
using Pluto;
using Pluto.IO.Binary;
using Triton.Surface;
using Triton.Surface.DirectDraw;

namespace Wheat;

public class SuperCompressedDDS : DDS {
	public SuperCompressedDDS(IRentedArray<byte> buffer, DDSHeader header, DDSHeader10 header10, bool leaveOpen = false) : base(buffer, header, header10, leaveOpen) => SurfaceBuffers = ObjectPool<List<IRentedArray<byte>>>.Rent();

	public SuperCompressedDDS(IRentedArray<byte> buffer, bool leaveOpen = false) : base(buffer, leaveOpen) {
		SurfaceBuffers = ObjectPool<List<IRentedArray<byte>>>.Rent();

		if (!IsSuperCompressed) {
			return;
		}

		var surfaceSizes = MemoryMarshal.Cast<byte, int>(buffer.Span[DataStart..])[..Header.MipMapCount];
		DataStart += surfaceSizes.Length * 4;
		var offset = DataStart;

		var memory = buffer.Memory;
		foreach (var surfaceSize in surfaceSizes) {
			var compressed = memory.Slice(offset, surfaceSize);
			offset += surfaceSize;

			var decompressed = new RentedArray<byte>(CompressionHelper.FindDecompressedLength(CompressionType.Zstd, compressed));
			CompressionHelper.Decompress(CompressionType.Zstd, compressed, decompressed.Memory);
			SurfaceBuffers.Add(decompressed);
		}
	}

	public List<IRentedArray<byte>> SurfaceBuffers { get; set; }

	public bool IsSuperCompressed => Header.Reserved1[0] == 0x4477;

	public override void Write(BufferBinaryWriter writer) {
		if (!IsSuperCompressed) {
			base.Write(writer);
			return;
		}

		writer.Write(Header with { Reserved1 = new DDSHeader.Reserved1Array() });
		if (Header.PixelFormat.FourCC == D3DFORMAT.DX10) {
			writer.Write(Header10);
		}

		WriteSurfaces(writer);
	}

	public override void WriteSurfaces(BufferBinaryWriter writer) {
		if (!IsSuperCompressed) {
			base.WriteSurfaces(writer);
			return;
		}

		foreach (var buffer in SurfaceBuffers) {
			writer.Write(buffer.Span);
		}
	}

	public override IRentedArray<byte> GetSurfaceBuffer(int surfaceIndex) {
		if (!IsSuperCompressed || surfaceIndex >= SurfaceBuffers.Count) {
			return base.GetSurfaceBuffer(surfaceIndex);
		}

		return SurfaceBuffers[surfaceIndex];
	}

	protected override void Dispose(bool disposing) {
		if (!disposing) {
			return;
		}

		foreach (var buffer in SurfaceBuffers) {
			buffer.Dispose();
		}

		SurfaceBuffers.Clear();
		ObjectPool<List<IRentedArray<byte>>>.Return(SurfaceBuffers);
		SurfaceBuffers = null!;
	}
}
