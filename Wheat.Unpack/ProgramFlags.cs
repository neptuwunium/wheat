// SPDX-FileCopyrightText: 2026 Neptuwunium
//
// SPDX-License-Identifier: EUPL-1.2

using Pluto.CommandLine;

namespace Wheat.Unpack;

public record ProgramFlags : CommandLineFlags {
	[Flag("input", IsRequired = true, Help = "Game Install Directory", Positional = 0)]
	public string Input { get; set; } = null!;

	[Flag("output", IsRequired = true, Help = "Output Directory", Positional = 1)]
	public string Output { get; set; } = null!;
}
