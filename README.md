# Improved Cougar

Improved Cougar is a gameplay mod for **The Long Dark** by Hinterland Studios

Improved Cougar 2.0 is a total overhaul and rework of the cougar, it's mechanics and behavior. 

## Arrival

By default, it will take some time before the cougar arrives in the world. The minimum and maximum time is configurable in the settings. 
Once the cougar is in the world, it will not leave completely. However, the cougar is region specific; meaning each region that has valid cougar territory will have it's own cougar inhabiting it. 
Once a cougar is in a region, it has a chance to leave that region entirely for a period of time before moving back in.
Most of the time hwoever, the cougar will inhabit a region and it's territory within a region until it is killed.

## Territory

Once the cougar has arrived and is in a given region, it will occupy territory around that region. This territory will NOT be shown on your map.
It will be up to the player to scope out and identify the currently occupied territory. 
The cougar will make audible cries, leave behind carcasses, etc... All of these signs will indicate where it is currently inhabiting. 
This is important, as the cougar will often wander and patrol around and out of it's territory seeking prey. 
It will also move around the region following set paths, changing it's current territory every few days (also configurable in settings).
In addition, crows will go silent in and around cougar territory, especially when the cougar is near. Keep an ear out!

## Behaviour

The cougar has had a full behaviour overhaul/rework using Expanded AI Framework. 

### Patrolling

The cougar's standard behaviour will be patrolling it's territory. It will follow set paths that may cross or approach common areas of interest.
If the cougar spots the player, it will begin to slowly and quietly stalk the player.

### Stalking

The cougar's stalking behaviour has received the biggest changes. It will no longer make excessive noise. The cougar will stalk the player
silently, stealthily and quickly. You will not hear it coming until it is close enough. If you keep an ear out you may hear the occasional branch snapping. 

The cougar will prioritize steathily moving from cover to cover using a complex pathfinding algorithm to approach the player without being seen (this is not perfect).
If the cougar is seen it will adjust it's behaviour accordingly.

### Freezing and Hiding

If the cougar is spotting stalking, it will freeze quickly reposition to a spot to hide, and reposition to continue stalking the player. 
This behaviour will differ depending on proximity. 

### Attacking

Once the cougar has caught the player in the open, it will attack, charging at the player from a close distance and from behind. This is tuned to allow the player enough time to turn and fight back.
If the cougar does successfully attack, it won't necessarilly full struggle. Depending on the player's health (cold, fatigue, hunger, condition, encumberance, afflictions, etc...) 
the attack will differ. A healthy survivor has a higher chance of sustaining a simple swipe blow, compared to a full attack.

If the cougar does successfully pounce the player, the severe lacerations affliction will still be applied (this may change in the future depending on feedback).
Using a firearm will force the cougar to do a passing swipe attack like in vanilla.

### Gunshots

Firing at the cougar in almost any state will force it to flee to a distant position where it will continue to stalk the player once it is safe. Unless it is attacking as previously mentioned.

## Cougar Health & Speed

In the Mod Settings menu you have the option to tune the Cougar's max health and attack speed. 
Default values are lower for the max health to make it easier to kill but higher for the speed to make it harder to catch when it attacks.


## Installation

* Install MelonLoader by downloading and running [MelonLoader.Installer.exe](https://github.com/HerpDerpinstine/MelonLoader/releases/latest/download/MelonLoader.Installer.exe)
* Install the following dependencies in your mods folder: 

- [ModSettings](https://github.com/DigitalzombieTLD/ModSettings/releases/latest)
- [ComplexLogger](https://github.com/Arkhorse/Complex-Logger/releases/latest)
- [Expanded AI Framework](https://github.com/monsieurmeh/ExpandedAiFramework/releases/latest)
- [AudioManager](https://github.com/DigitalzombieTLD/AudioManager/releases/latest)
- [ModData](https://github.com/dommrogers/ModData/releases/latest)
- 
* Install the latest release and drop it in your mods folder

