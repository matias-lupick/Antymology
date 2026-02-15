## Matias Lupick Feb 13 2026
## Gitlab link: https://csgit.ucalgary.ca/matias.lupick/cpsc565as3
## Github link: https://github.com/matias-lupick/Antymology


## Antymology

The antymology project shows the evolution of a colony of ants. The ants lose health at a constant rate and must eat mulch to restore their health. The primary goal of the ants is to create as many nest blocks as possible. Only the queen ant can create nest blocks. The queen can be differentiated from the workers by its size.


## Ant actions

The ants can perform the following actions:
Do nothing, move forwards, turn right, turn left, share food, eat food, dig terrain, create a nest block (queen only).


## Modeling Choices

In early versions of the simulator, the optimal strategy was for the queen to run around the map eating and making nests whenever possible. Since we want ants that work together, the queen is not allowed to eat. Additionally to make nests more expensive, the queen has 10 times the health of the worker ants.


## Ant Brain

The ant brain is a simple neural network with 1 layer. The ant performs the action that receives the highest value. The ant takes the following inputs from the environment:

Percent health
A random value from 0 to 1
3 memory values routed from output last tick
If the ant is standing on acid
If the ant is standing on mulch
If the ant is standing on a nest
If the ant can move forwards
If the ant is facing a wall
If the ant is the Queen
If the ant is the only ant on the tile


## Evolution

The fitness for ants is determined primarily by the nest count. They receive 1000 fitness for every nest block. To encourage developing eating, ants also gain 1 fitness for every mulch block they consume. The evolution strategy used is (3 + 9) with 5% chance of each weight/bias mutating by a step size of 0.1.


## Running a simulation

A simulation can be run from the Unity editor 6000.3. The EvolutionManager script (on the Evolution Manager game object) contains the parameters for mutation per generation as well as the starting traits. If the loadTrait field is empty, a blank run will be started. If a file is specified, the ant traits described in this file will be loaded. This must be set before run to take effect. The saveTrait field determines where the best ant’s traits will be saved at the end of the generation when the saveAtEnd field is ticked. For both loading and saving the path should be relative to the “Assets/Brain” folder. The simulation can be started by ticking the go field in the TimeManager script (on the Time Manager game object).


## Results

After 285 generations the ants achieved a fitness of roughly 130000. The strategy used was simply having the queen run around the map eating and creating nests.


## Future Improvements

There are several things I would like to improve about the simulation. To start, I would like the world generation to vary each generation. While the current code contains the ability to do this, the world generation can result in the ants starting on an immovable, inescapable boulder which guarantees all ants receive 0 fitness. Another issue is the lack of colony formation. Ideally the optimal strategy for ants is to form a colony, instead of having a single queen run around the planet eating everything. The best solution would be to disallow the queen from eating anything. Although we would then need the workers to feed the queen. The workers at this time have no tools for finding the queen, and thus would be unable to feed it. The simplest tool to let ants find the queen would be an action that rotates the ant to face the queen (regardless of how far away it is).

