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

foreach (var asset in assetDeps.Assets) {
	using var data = vfs.OpenFile(asset.Asset);
	if (data.Length == 0) {
		continue;
	}

	var assetPath = asset.Asset.SanitizeTraversal();
	var normalizedPath = Path.Combine(flags.Output, assetPath);
	if (Path.GetRelativePath(flags.Output, normalizedPath)[0] is '.' or '/') {
		throw new InvalidOperationException("tried to path traverse");
	}

	Log.Information("Exporting {Path}", asset.Asset);
	var dir = Path.GetDirectoryName(normalizedPath);
	Directory.CreateDirectory(dir ?? flags.Output);
	using var stream = new FileStream(normalizedPath, FileMode.Create,  FileAccess.ReadWrite, FileShare.ReadWrite);
	stream.Write(data.Span);
}
