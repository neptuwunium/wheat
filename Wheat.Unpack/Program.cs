// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using System.Text;
using System.Text.Json;
using Pluto.CommandLine;
using Pluto.Extensions;
using Serilog;
using Wheat;
using Wheat.RTTR;
using Wheat.Unpack;

if (File.Exists("wheat.log")) {
	File.Delete("wheat.log");
}

Log.Logger = new LoggerConfiguration()
			 .MinimumLevel.Information()
			 .WriteTo.Console()
			 .WriteTo.File("wheat.log")
			 .CreateLogger();

var flags = CommandLineFlags.Singleton<ProgramFlags>.Instance;
using var vfs = new PackageFileSystem();
vfs.Mount(Path.Combine(flags.Input, ".assets/output"));

var output = flags.Output;

AssetDepsDBCompact assetDeps;
using (var data = vfs.OpenFile(".assets/output/client_dependencies_db.json")) {
	Log.Information("Exporting {Path}", "client_dependencies_db.json");
	Directory.CreateDirectory(output);
	using var stream = new FileStream(Path.Combine(output, "client_dependencies_db.json"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
	stream.Write(data.Span);
	var str = Encoding.UTF8.GetString(data.Span);
	assetDeps = JsonSerializer.Deserialize<AssetDepsDBCompact>(str, RTTRJsonObject.Options)!;
}

string[] platformsPrefab = ["client", "server"]; // assuming "server", it doesn't actually exist on the client, obviously.
string[] platformsShader = ["dx12.pc", "dx12.xseries", "vk.pc", "vk.mac", "vk.ios", "ps5"];
string[] platformsDefault = [string.Empty];

var paths = new HashSet<string> { ".assets/output/client_dependencies_db.json" };
foreach (var asset in assetDeps.Assets) {
	var selectors = Path.GetExtension(asset.Asset) switch {
		".fx" or ".cfx" => platformsShader,
		".world" or ".prefab" => platformsPrefab,
		_ => platformsDefault,
	};

	foreach (var platform in selectors) {
		var resultPath = asset.Asset;
		using var data = vfs.OpenFile(ref resultPath, platform, out var ext);
		if (data.Length == 0) {
			continue;
		}

		paths.Add(resultPath);

		var normalizedPath = Path.GetFullPath(Path.Combine(output, asset.Asset.SanitizeTraversal()));
		if (Path.GetRelativePath(output, normalizedPath)[0] is '.' or '/') {
			throw new InvalidOperationException("tried to path traverse");
		}

		normalizedPath = Path.ChangeExtension(normalizedPath, ext);
		var dir = Path.GetDirectoryName(normalizedPath);

		Log.Information("Exporting {Path}", Path.ChangeExtension(asset.Asset, ext));
		Directory.CreateDirectory(dir ?? output);
		using var stream = new FileStream(normalizedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
		stream.Write(data.Span);
	}
}

output = Path.Combine(flags.Output, "__vfs__");
foreach (var assetPath in vfs.Files.Keys.Where(assetPath => !paths.Contains(assetPath))) {
	using var data = vfs.OpenFile(assetPath);
	if (data.Length == 0) {
		continue;
	}

	Log.Warning("Manually exporting unexported {Path}", assetPath);
	var normalizedPath = Path.GetFullPath(Path.Combine(output, assetPath.SanitizeTraversal()));
	if (Path.GetRelativePath(output, normalizedPath)[0] is '.' or '/') {
		throw new InvalidOperationException("tried to path traverse");
	}

	var dir = Path.GetDirectoryName(normalizedPath);
	Directory.CreateDirectory(dir ?? output);
	using var stream = new FileStream(normalizedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
	stream.Write(data.Span);
}
