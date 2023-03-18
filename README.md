# Unity-Pooling-Test
A unity project to explore the IObjectPool capabilities

A cube prefab is spawned into the scene and moves at a constant speed, starting at (0, 0, 0), towards 3 points (P1, P2, P3) that can be changed by editing the csv file found in the project. On arrival at each point, the game object should begin movement toward the next point, but on arrival at the final point or if a collision is detected, the main game object should be removed from the scene, play a sound effect and show a particle effect.

There are 100 objects that are spawned at random positions that can collide with a main game object.
