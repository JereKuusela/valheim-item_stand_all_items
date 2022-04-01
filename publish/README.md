# Item Stand All Items

Client side mod that allows putting any item on item stands.

Additionally allows customizing the item positions.

# Manual Installation:

1. Install the [BepInExPack Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
2. Download the latest zip.
3. Extract it in the \<GameDirectory\>\BepInEx\plugins\ folder.

# Settings

Config syncing is available if also installed on the server.

- Use legacy attaching: Reverts to the version 1.1.0 attaching method. If the mod causes any issues this can be tried. It works very reliably but some items will be disabled or may not contain all parts of the model.
- Hide item stands which have items: If true, item stands with items will be invisible.
- Enable transformations: Must be true for "Move items closer" and "Custom transformations" to work. These require some extra computing which may lower performance with massive amounts of item stands.
- Move items closer: Removes the base offset from item stands to make items attach closer to the wall.
- Custom transformations: Allows modifying position, rotation and scale of each item to have them exactly like you want.

# Custom transformations

The format is id,distance,offset_x,offset_y,angle_1,angle_2,angle_3,scale_1,scale_2,scale_3|id,distance,...

- id: Id of item (check wiki for item ids if needed).
- distance: Distance from the item stand (use negative value to move closer).
- offset: Position offset to x and y directions.
- rotation: Rotation to different directions.
- scale: Scaling to different directions (usually you want the same number for each value).

For example:

- Wood,0,0,0,0,0,0,10,10,10 would cause attached wood items to have 10x size.
- Wood,0,0,0,0,0,20 would rotate attached wood items slightly.
- Wood,-0.1|CarrotSoup,-0.1 would move carrot soup and wood items slightly towards the item stand.

# How it works

The mod uses a few different ways to attach the items:

1. The default way is to use "attach" child object of items. This is how the base game works. However not all items have this child object.
2. The next way is to check if the item has a single child object and use that. This requires some extra logic to filter out unnecessary child objects but otherwise works the same as "attach" child object.
3. If there are multiple children, then the whole item must be used. Unfortunately the parent object contains scripts like ItemDrop which would make the item fall on ground (basically duplicating it). In this case, a dummy object is created to replace the parent object.

The legacy attaching uses the first children. This means the whole is never returned but the attached item may miss some parts of the model.


# Changelog

- v1.6:
	- Adds config syncing (if installed server side).

- v1.5:
	- Adds setting to hide item stands with items.
	- Adds setting to have items closer towards the item stand.
	- Improves custom transformations (applied gradually instead of overwriting natural transformation).
	- Changes parameters in custom transformations so that the distance is first (probably the most needed).

- v1.4:
	- Improves attaching.
	- Adds setting to allow custom position, rotation and scaling for attached items.
	- Removes some settings as obsolete.
	- Fixes conflict with Epic Loot.

- v1.3:
	- Adds a new less instrusive attach method (automatically used for items that support it).
	- Adds a setting to revert back to v1.2 behavior.
	- Adds removal of the root rotation (to fix weird angles of some items).
	- Adds a setting to disable the automatic rotation removal.
	- Adds a setting to disable the automatic offset removal (implemented in v1.2).

- v1.2:
	- Adds support for seeds.
	- Adds glow back to attached Greydwarf Eyes.
	- Adds setting which allows using the attach method from the previous version.
	- Adds setting for less supported items but also less intrusive attach method.

- v1.1:
	- Fixes some items not appearing.

- v1.0: 
	- Initial release.
