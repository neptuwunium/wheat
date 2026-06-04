<!--
SPDX-FileCopyrightText: 2026 Neptuwunium

SPDX-License-Identifier: EUPL-1.2
-->

# wheat

3rd Party BroEngine Tooling, optimized for HEAT

At the moment only exports files, almost all resource stuff is done by the sister project [io_scene_bro](https://github.com/neptuwunium/io_scene_bro).

## Notice

This project is not authorized, affiliated or endorsed by Wargaming.net, Wargaming Group Limited or Wargaming International Limited.

"World of Tanks: HEAT", "World of Tanks" are registered trademarks or trademarks of Wargaming Group Limited.

## Notes

If the first reserved field of DDS Files is `0x4477`, each MIP surface is compressed.

At the start of the surface buffer (i.e. after the DDS/DX10 header) there is a list of integers for compressed sizes.

Each mip is compressed with ZStandard, see Wheat/SuperCompressedDDS.cs

Wheat decompresses these textures.
