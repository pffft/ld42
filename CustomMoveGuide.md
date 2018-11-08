# Move API
## Basics

Make a new file in the directory of choice. Use the namespace "Moves.User". Create a new class (any name works), and extend "Move".


[TODO: when the API is fully released, there will be more strict rules on the location for custom moves. Most likely it'll be a directory, "MODNAME", in some assets file Unity can access. The structure will then look like the following:

- MODNAME/
  - Moves
  - Phases
  - Routines

]

In the constructor, make sure you define the following properties:
* Difficulty: A float from 0 to 8. Consult the difficulty ranking if you're unsure what to place here.
* Description: A present-tense summary of what this move does.
* Sequence: The actual AISequence that the game will use to execute your custom Move.

You can optionally define the "Name" property, but usually the default value is adequate. 

## Sequence API

This is where the fun stuff happens. The definition for "Sequence" can be a simple, predefined sequence, such as:

`Sequence = new Shoot1();`

However, there are many ways to chain and build up `AISequence`s. Below is a summary of the key ones, and the best practice for using them.

### Basic `AISequence`s

There are many simple `AISequence`s defined in the `Moves.Basic` namespace. These are used so commonly that the template includes this namespace for you, so that you don't have to explicitly call them every time.

The following is a list of the most common basic `AISequence`s. Any parameters for the move are included in parenthesis after the name. 

#### `AISequence`s that fire `Projectile`s

Note that, by default, the standard projectile chosen has size `Size.SMALL`, speed `Speed.MEDIUM`, originates from the Boss's position, and moves in the direction of the Player. Unless it hits the Player, a physical obstacle, or goes too far off the arena, the default projectile will despawn after 10 seconds.

* `Shoot1(Projectile skeleton)`: Shoots a single projectile. The parameter specifies the type of projectile that gets fired. When called without parameters, fires a default projectile (which originates at the Boss's position, and moves in the direction of the Player).
* `Shoot3(Projectile skeleton)`: Shoots a three-projectile spread; one aimed at the player, and a projectile each at +/- 30 degrees from the player position.
* `ShootArc(int density, float from, float to, Projectile skeleton)`: Shoots an arc of projectiles. `density` is a parameter for how many projectiles would be in this arc if it went in a full 360. `from` and `to` describe the start and end point of the arc, where `0` is the direction the arc is aimed at. 

  For example, if `density` is `50`, `from` is `-90`, and `to` is `90`, `ShootArc` will fire a total of 25 projectiles in the direction of the player. They will be in an arc 180 degrees wide, from the Boss's direct left to his direct right.
* `ShootLine(int amount, float width, Vector3? target, Speed speed, Size size)`: Shoots a line of projectiles aimed at `target`. If `target` is null, it defaults to the Player's position. `amount` is how many projectiles spawn, `width` is how many physical units wide the line is. `speed` and `size` affect the fired projectiles.
* `ShootWall(float angleOffset)`: Shoots a wall of projectiles at the Player, with a small gap in the middle. The projectiles are default, except their speed is `Speed.SLOW`. `angleOffset` affects the offet from the player's position, in degrees.

#### `AISequence`s that fire `AOE`s

The default `AOE` *has no regions on*. You have to set them using the `On` and `Off` methods, or else it will appear as if your custom Move doesn't do anything!

* `ShootAOE(AOE skeleton)`: Shoots an `AOE`. Much like the projectile shooting methods above, the parameter describes the actual `AOE` being fired.

#### Other useful `AISequences`

* `Teleport(Vector3? target, int speed)`: Moves the Boss to the specified position. If `target` is null (which it is by default), the Boss will teleport to a random position within the arena, subject to some additional rules. You will usually want to call `Teleport()` to randomly position the Boss before your move.
* `Pause(float duration)`: Waits for the given period of time before executing the next `AISequence`. This is equivalent to calling `[AISequence].Wait(duration)`, where "[AISequence]" is any other AISequence. However, this form may look cleaner when creating `AISequence`s via a list of other `AISequence`s.
* `PlayerLock(float enableLock)`: Toggles locking onto the player. If `true`, then the Boss will save the Player's current position and fire all future projectiles at that position (until this method is called again with `false` passed in). Many attacks, such as `Sweep`, use this to create a bullet pattern that isn't affected by where the Player moves while it's being fired by the Boss.
* `MoveCamera(bool isFollow, Vector3? targetPosition)`: Moves the camera position to `targetPosition`. If `targetPosition` is null, then this will reset the camera to its default position. If `isFollow` is set to `true`, then the camera will follow the Player (which it does by default); otherwise, it will be stationary at the specified position. This is useful for moves where the Boss moves frequently, or where the Player needs to have a wider viewing angle.
* `Invincible(bool to)`: Sets the Boss to be either invincible (if `to` is `true`), or vulnerable to damage (if `to` is `false`).
* `Strafe(bool clockwise, float degrees, int speed, Vector3 center)`: Moves the boss. In contrast to `Teleport`, this will move sideways `degrees` number of degrees about the point `center`. 
* `ShootHomingStrafe(bool clockwise, float strafeAmount, int speed)`: Fires a homing projectile, and then calls `Strafe` with the specified arguments. 


### Constructor

The most common way of chaining multiple `AISequence`s together is by using the `AISequence` constructor, which takes a variable amount of other `AISequence`s. Here's a basic example that teleports, waits for half a second, and then shoots a single large projectile:

`Sequence = new AISequence(
  new Teleport(),
  new Pause(0.5f),
  new Shoot1(new Projectile { Size = Size.LARGE })
);`

This syntax is designed to express a flow of actions from top to bottom, with method names that (hopefully) describe the action without requiring looking up the function.

### Builder methods

To make more complex `AISequence`s, there are quite a few methods that give additional functionality. These are described below.

#### Wait/Pause
#### Then
#### Times
#### For
#### If
#### Either
#### Merge
