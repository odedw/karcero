# Karcero

A dungeon/cave map generation library for C# with a fluent API and generic models open for extention.

## Dungeon Map Generation

The dungeon generation algorithm is based on Jamis Buck's [algorithm](http://www.brainycode.com/downloads/RandomDungeonGenerator.pdf) with some tweaks here and there.

Here is an example of a generation call:
```csharp
var generator = new DungeonGenerator<Cell>();
generator.GenerateA()
         .MediumDungeon()
         .ABitRandom()
         .SomewhatSparse()
         .WithMediumChanceToRemoveDeadEnds()
         .WithMediumSizeRooms()
         .WithLargeNumberOfRooms()
         .AndTellMeWhenItsDone(map =>
         {
            //Do stuff with map
         });
```

and the result (visualized):
![Map Example](http://i.imgur.com/mUkajVU.jpg)

## Cave Map Generation
TBD

