How to set up da Splitter :)

Before reading, there are notes in the scripts connected to the splitter. If you still have problems talk to Colton.

Look at the prefab to see how everything is set up. Some quick notes...
Make sure that the Attack Range is smaller then the Melee Attack Range.
Have centre point NOT as a child under the splitter.
If you want a controlled point for the patrol, change the Enemy Wander script to the controlled one (Not set up yet 1/15/25) 



Some questions that might happen...

Q : Why isn't my enemy moving
A : Make sure all nav mesh are properly set up and enemy wander script has the correct values

Q: Why isn't the player taking Damage
A: The Attack Range needs to be smaller then the Melee Attack Range
Why? The Melee Attack Range is the range that damages the player and the Attack Range is the range needed for the enemy to know when to stop to attack.


Scripts still not finished
Last Edit 1/15/25