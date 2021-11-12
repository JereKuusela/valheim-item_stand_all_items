# Item Stand All Items

This mod removes item restrictions from item stands which allows putting there any item.

# Manual Installation:

1. Install the [BepInExPack Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
2. Download the latest zip
3. Extract it in the \<GameDirectory\>\BepInEx\plugins\ folder.

# Settings

"Use legacy attaching" uses the version 1.1.0 attaching method. If the mod causes any issues this can be tried. It works very reliably but some items will be disabled or may not contain all parts of the model.

"Custom transformations" allow customizing how the items get attached. The format is id1,x1,y1,z1,a1,b1,c1,i1,k1,j1|id2,x2,y2,z2,a2,b2,c2,i2,k2,j2|...

- id: Id of item (check wiki for item ids if needed).
- x, y, z: Position offset. If not given, then 0,0,0 is used.
- a, b, c: Rotation. If not given, then 0,0,0 is used.
- i, j, k: Scaling. If not given, then 1,1,1 is used.

For example:

- Wood,0,0,0,0,0,0,10,10,10 would cause attached wood items to have 10x size.
- Wood,0,0,0,0,0,20 would rotate attached wood items slightly.
- Wood,0,0,0.1 would move attached wood items slightly.

# Changelog

- v1.4.0:
	- Improved attaching-
	- Added setting to allow custom position, rotation and scaling for attached items.
	- Removed some settings as obsolete.
	- Fixed conflict with Epic Loot.

- v1.3.0:
	- Added a new less instrusive attach method (automatically used for items that support it).
	- Added a setting to revert back to v1.2.0 behavior.
	- Added removal of the root rotation (to fix weird angles of some items).
	- Added a setting to disable the automatic rotation removal.
	- Added a setting to disable the automatic offset removal (implemented in v1.2.0).

- v1.2.0:
	- Added support for seeds.
	- Added glow back to attached Greydwarf Eyes.
	- Added setting which allows using the attach method from the previous version.
	- Added setting for less supported items but also less intrusive attach method.

- v1.1.0:
	- Fixed some items not appearing

- v1.0.0: 
	- Initial release