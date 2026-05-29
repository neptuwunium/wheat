// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using System.IO.Hashing;
using System.Runtime.InteropServices;
using System.Text;
using Charon.Compression;
using Charon.Compression.Zip;
using Pluto;
using Pluto.Extensions;
using Pluto.IO.Binary;
using Pluto.IO.FileSystem;
using Serilog;
using ZipFile = Charon.Compression.Zip.ZipFile;

namespace Wheat;

public sealed class PackageFileSystem : IDisposable {
	public PackageFileSystem() {
		Files = ObjectPool<Dictionary<string, (int ZipIndex, ZipEntry ZipEntry)>>.Rent();
		Files.Clear();

		ZipFiles = ObjectPool<List<ZipFile>>.Rent();
		ZipFiles.Clear();
	}

	public List<ZipFile> ZipFiles { get; set; }
	public Dictionary<string, (int ZipIndex, ZipEntry ZipEntry)> Files { get; set; }

	public void Mount(string path) {
		foreach (var zipPath in new FileEnumerator(path, "*.zip")) {
			var zip = new ZipFile(zipPath);
			var index = ZipFiles.Count;
			ZipFiles.Add(zip);

			foreach (var entry in zip.Entries) {
				Files[entry.Path] = (index, entry);
			}
		}
	}

	public void Dispose() {
		Files.Clear();
		ObjectPool<Dictionary<string, (int ZipIndex, ZipEntry ZipEntry)>>.Return(Files);
		Files = null!;

		ZipFiles.Clear();
		ObjectPool<List<ZipFile>>.Return(ZipFiles);
		ZipFiles = null!;
	}

	public static IRentedArray<byte> Decompress(IRentedArray<byte> data) {
		if (data.Length <= 8 || MemoryMarshal.Read<uint>(data.Span) != 0xFD2FB528) {
			return data;
		}

		var compressed = data.Memory;
		var size = CompressionHelper.FindDecompressedLength(CompressionType.Zstd, compressed);
		if (size <= 0) {
			return data;
		}

		RentedArray<byte>? decompressed = null;
		try {
			decompressed = new RentedArray<byte>(size);
			CompressionHelper.Decompress(CompressionType.Zstd, compressed, decompressed.Memory);
			data.Dispose();
			return decompressed;
		} catch {
			decompressed?.Dispose();
			throw;
		}
	}

	public IRentedArray<byte> OpenFile(string path, string platform = "") => OpenFile(path, platform, out _);

	public IRentedArray<byte> OpenFile(string path, string platform, out string ext) {
		path = path.ToLower();
		if (path.Contains('\\', StringComparison.Ordinal)) {
			path = path.Replace('\\', '/');
		}

		ext = Path.GetExtension(path).ToLower();

		if (Files.TryGetValue(path, out var file)) {
			return Decompress(ZipFiles[file.ZipIndex].Open(file.ZipEntry));
		}

		if (path.StartsWith(".assets/output")) {
			if (Files.TryGetValue(path, out file)) {
				return Decompress(ZipFiles[file.ZipIndex].Open(file.ZipEntry));
			}

			Log.Warning("File {Path} not found", path);
			return Decompress(IRentedArray<byte>.Empty);
		}

		var nameBuffer = (stackalloc byte[Encoding.UTF8.GetMaxByteCount(path.Length)]);
		var n = Encoding.UTF8.GetBytes(path, nameBuffer);
		var hash = XxHash128.HashToUInt128(nameBuffer[..n]);
		var name = Path.GetFileNameWithoutExtension(path);

		switch (ext) {
			case ".png":
			case ".jpg":
			case ".tga":
			case ".tif":
			case ".tiff":
			case ".exr":
			case ".layertex":
				ext = ".dds";
				break;
			case ".svg":
				ext = ".svgf";
				break;
			case ".po":
				ext = ".mo";
				break;
			case ".pck":
				ext = ".bnk";
				break;
			case ".prefab":
			case ".world":
				ext = $".{platform}.scene";
				break;
			case ".fbx":
				ext = ".mesh";
				break;
			case ".fbxphys":
				ext = ".phys";
				break;
			case ".fbxskel":
				ext = ".skel";
				break;
			case ".fbxtriphys":
				ext = ".triphys";
				break;
			case ".txt":
				ext = ".html";
				break;
			case ".fx":
			case ".cfx":
				ext = $".{platform}.fxo";
				break;
		}

		var old = path;
		path = $".assets/output/{name}{hash.High:x16}{hash.Low:x16}{ext}";
		Log.Debug("Transforming {OldPath} to {Path}", old, path);

		if (Files.TryGetValue(path, out file)) {
			return Decompress(ZipFiles[file.ZipIndex].Open(file.ZipEntry));
		}

		Log.Warning("File {Path} ({OldPath}) not found", path, old);
		return Decompress(IRentedArray<byte>.Empty);
	}
}
