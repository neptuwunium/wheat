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

AssetDepsDBCompact assetDeps;
using (var file = vfs.OpenFile(".assets/output/client_dependencies_db.json")) {
	var str = Encoding.UTF8.GetString(file.Span);
	assetDeps = JsonSerializer.Deserialize<AssetDepsDBCompact>(str, RTTRJsonObject.Options)!;
}

string[] platformsPrefab = ["client", "server"]; // assuming "server", it doesn't actually exist on the client, obviously.
string[] platformsShader = ["dx12.pc", "vk.pc"];
string[] platformsDefault = [string.Empty];

foreach (var asset in assetDeps.Assets) {
	var selectors = Path.GetExtension(asset.Asset) switch {
		".fx" or ".cfx" => platformsShader,
		".world" or ".prefab" => platformsPrefab,
		_ => platformsDefault
	};

	foreach (var platform in selectors) {
		using var data = vfs.OpenFile(asset.Asset, platform, out var ext);
		if (data.Length == 0) {
			continue;
		}

		var assetPath = asset.Asset.SanitizeTraversal();
		var normalizedPath = Path.Combine(flags.Output, assetPath);
		if (Path.GetRelativePath(flags.Output, normalizedPath)[0] is '.' or '/') {
			throw new InvalidOperationException("tried to path traverse");
		}

		normalizedPath = Path.ChangeExtension(normalizedPath, ext);

		Log.Information("Exporting {Path}", Path.ChangeExtension(asset.Asset, ext));
		var dir = Path.GetDirectoryName(normalizedPath);
		Directory.CreateDirectory(dir ?? flags.Output);
		using var stream = new FileStream(normalizedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
		stream.Write(data.Span);
	}
}
