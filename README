# __ROOM 753__

This is a Unity3D game made for Ludum Dare 37.

[https://local_minimum.itch.io/first-person-cleaner](If you just wan't to play, here's the game.)


## Making Levels

Use program that produces CSV with semicolon as separators (e.g. LibreOffice, Excel, or a plain text editor if you want).

For each room indicate if it should cause more glitch "1" or less glitch "0".
Mark the start room with asterisc `*1` and the elevator tile with a hash `#1`.
Note that the elevator tile must be at the end of a hallway (have three walls).

If you want the tile to invoke an action/several actions those are listed after the tile type `2,ACTION1,ACTION2,ACTION3`.

Action settings/parameters are separated with ":".
Each action has a final optional parameter that restricts the action to entry from that direction.

### Actions

There can be several actions on a tile and they are enacted in the order they are listed. 
Upon reaching a new tile (teleport), the actions of this tile __are not__ enacted.

_Teleport_

Sends player to another tile:

`teleport:ROW:COLUMN` or `teleport:ROW:COLUMN:DIRECTION`

Examples:

    teleport:1:1
    teleport:3:4:west
 
_Look at_

Makes player look at a different direction.

`lookat:DIRECTION` or `lookat:DIRECTION:DIRECTION`

Examples:

    lookat:south:south

Makes player look towars south if and only if player entered from south.

_Rotate_

Rotates the player in relation to players current direction

`rotate:AMOUNT` or `rotate:AMOUNT:DIRECTION`

Examples:

    rotate:-1
    rotate:1:east
