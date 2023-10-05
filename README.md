# Item Stand All Items

Client side mod that allows putting any item on item stands and customizing the item transformations.

Install on the client (modding [guide](https://youtu.be/L9ljm2eKLrk)).

Install on the server to sync the config.

## Configuration

If transformations are enabled, the hovered item stand be configured with commands:

- `itemstand_hide [-1/0/1]`: Sets automatic hiding. 1 to hide, 0 to use the default value and -1 to always show.
- `itemstand_info`: Prints item, offset, rotation, scale and hide information.
- `itemstand_offset [forward,right,up=0,0,0]`: Sets the item offset. Limited by maximum offset setting.
- `itemstand_rotation [roll,pitch,yaw=0,0,0]`: Sets the item rotation.
- `itemstand_scale [x,y,z=1,1,1]`: Sets the item scale. A single value sets all sides. Limited by maximum scale setting.

Following settings are available:

- Custom transformations (key: `custom_transformations`): Allows setting default offset, rotation and scale for each item. See below for more info.
- Enable transformations (default: `true`, key: `enable_transformations`): Required to customize item offset, rotation or scale. May cause lower performance with lots of item stands.
- Hide automatically (default: `false`, key: `hide_automatically`): Hides item stands which have items.
- Item stand ids (default: `itemstand,itemstandh`, key: `item_stands_ids`): Which item stands are affected by this mod.
- Maximum scale (key: `maximum_scale`): Limits how big items can be made with the command `itemstand_scale`.
- Maximum offset (key: `maximum_offset`): Limits how far items can be moved with the command `itemstand_offset`.
- Migration command (key: `migration_command`): Whether the migration command is available for clients.
- Mode (default: `All`, key: `mode`): Sets which items are available. All, Compatible or Vanilla.
- Move items closer (default: `false`, key: `move_items_closer`): Removes the base offset to make items attach closer to the wall.
- Use legacy attaching (default: `false`, key: `use_legacy_attaching`): Reverts to the version 1.1.0 attaching method. If the mod causes any issues this can be tried. It works very reliably but some items will be disabled or may not contain all parts of the model.

### Custom transformations

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

## How it works

The mod uses a few different ways to attach the items:

1. The default way is to use "attach" child object of items. This is how the base game works. However not all items have this child object.
2. The next way is to check if the item has a single child object and use that. This requires some extra logic to filter out unnecessary child objects but otherwise works the same as "attach" child object.
3. If there are multiple children, then the whole item must be used. Unfortunately the parent object contains scripts like ItemDrop which would make the item fall on ground (basically duplicating it). In this case, a dummy object is created to replace the parent object.

The legacy attaching uses the first children. This means the whole is never returned but the attached item may miss some parts of the model.
