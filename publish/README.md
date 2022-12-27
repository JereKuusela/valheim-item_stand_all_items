# Item Stand All Items

Client side mod that allows putting any item on item stands and customizing the item transformations.

Install on the client (modding [guide](https://youtu.be/L9ljm2eKLrk)).

Install on the server to sync the config.

# Usage

See [documentation](https://github.com/JereKuusela/valheim-item_stand_all_items/blob/main/README.md).

Sources: [GitHub](https://github.com/JereKuusela/valheim-item_stand_all_items)

Donations: [Buy me a computer](https://www.buymeacoffee.com/jerekuusela)

# Changelog

- v1.15
	- Fixes error with some items.

- v1.14
	- Fixes lantern not working.

- v1.13
	- Update for Mistlands PTB.

- v1.12
	- Fixes errors on close.

- v1.11
	- Breaking change: Old item stand transformations no longer work. Use `itemstand_migrate` command to convert item stands. See the documentation from more information.
	- Adds a new setting `Mode` to set which items are allowed.
	- Improves performance.
	- Fixes `Custom transformations` rotations not being degrees.
	- Fixes compatibility issue with Better Wards mod.
	- Fixes the black screen.

- v1.10
	- Fixes version 1.9 resetting the config.

- v1.9
	- Adds a new command `itemstand_info` to print item stand properties.
	- Changes the mod GUID.

- v1.8
	- Adds a new setting `item_stand_ids` to change which item stands are affected by the mod (other mods may add new item stands).
	- Adds support for removing items from boss stones (if added to the id setting).

- v1.7
	- Adds the missing config lock setting to the config sync.
	- Fixes item stand hiding not working when reloading the area.
	- Fixes the command `itemstand_scale` not setting all axis with a single value.
	- Fixes acorns not appearing on item stands.
