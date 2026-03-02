# Red Engine — Game Developer Task

![Concept](Assets/Docs/concept.png)

## Task Requirements

This is for a game where the player is aiming to have the largest distance between two of their pucks. The concept artist has come up with the above effect for it. Attempt to recreate this in Unity using whatever means you see fit.

- Use the **RedEngine** scene.
- Use `PuckTestDataLoader.LoadPuckData` to load the puck tracking data.
  - We've provided 3 sets of data.
  - We should be able to press 1, 2 or 3 on our keyboard to play each set of data.
- Create the line effect:
  - Only pucks of the same colour should be connected.
  - The longest line should be the most intense.
  - The line effect should be animated (like electricity).
- Move the pucks according to the loaded data, and ensure the line effect updates accordingly.
- The camera should attempt to contain all pucks on screen

## Notes

- This application will be run on a windows desktop.
- The puck positions are provided in mm, however you'll probably find it easier to convert these to metres for use in Unity.
- You may install third party packages.

## Considerations

Your response to this test doesn't need to be perfect. We're looking to see how you approach the problem, and how you justify your decisions. Some things to consider that we may discuss in a potential follow-up interview:

- Why have you written your code in the way you have? How might you evolve this for a production game?
- Why have you used the Unity features / tools you have?
- What optimisations could you make?
- How could you make the scene even more visually interesting?
- How might you make the game itself more interesting?

> To submit your response please either zip up the project or push to a repo we can clone.