# Dodgeball

**Dec 12th, 10pm**

*Note*
- `Team Id` for all the agents need to be set to 0

*Changes*
- Allow ball to be picked up if same color as agent or is neutral
- Remove `canpickup`
- Check if agent that shot the ball exists before rewarding the shooter
- Removed group rewards
- Removed existential rewards
- Add reward for picking up and shooting ball
- Add reward for hitting an opponent

**Dec 12th, 1am**

*Side Note*
- It looks like we have the behaviour paramters' actions for movement encoded as discrete branches. Should those be continuous instead? In this (https://www.youtube.com/watch?v=zPFU30tbyKs) video, the guy makes movement actions continuous. If discrete is already working then no need to change this.


*Changes*
- Updated "Game Environment" Prefab to match our recent changes. Should hopefully help centralize changes for ML training.
- Made middle invisible wall smaller.
- Fixed bug where couldn't pickup balls after game restarted.

*To do*
- Add reward system to new features

</br></br>

**Dec 10th, 9pm**

*Changes*
- Added agent features pickup and shoot balls in inventory
- Ball is picked up when in contact with agent and if the ball is in neutral state
- Ball is shot using SPACE when inventory != 0
- Ball is now set to neutral if ball.velocity.magnitude is less than 2
- Cloned ball from instantiate are removed when reset scene

*To do*
- Add reward system to new features

</br></br>


**Dec 7th, 8pm**

*Changes*
- Added an invisible wall in the middle that blocks agents but allows balls through. This should keep agents to their respective sides.
- Fixed bug where balls would keep moving after a game ended and their position was reset.
- Added effect to change game floor color when one team wins or looses. <- Feel free to delete this is this is distracting.

</br></br>

**Dec 7 11:30am**

*Still To Do*

- Need to add an invisible wall halfway to stop agents crossing halfway point but ball should be allowed through.
- Fix random generation of player and ball spawn locations

*Changes*

- Now there are multiple balls

</br></br>



**Dec 6 12:30pm**

*Still To Do*

- Add multiple balls.
- Need to add an invisible wall halfway to stop agents crossing halfway point but ball should be allowed through.

*Changes*

- Walls are now invisible

</br></br>


**Dec 6 11:30pm**

*Still To Do*

- Add multiple balls.
- Invisible walls behind agents are not invisible anymore. Need to fix this.
- Need to add an invisible wall halfway to stop agents crossing halfway point but ball should be allowed through.

*Changes*

- Updated Game Environment Prefab to contain all scripts, previously was behind actual game environment in scene.
- Ball correctly changes color.
- Ball and agents leaving arena is seemingly fixed now.
- Game can now end.



</br></br>

**Dec 6, 2:30**

*Changes*

- Agents can now move around and kick the ball

- Ball changes colour when kicked (ONLY FOR PURPLE RIGHT NOW) <- need to fix

- Ball can be kicked out of arena, players can launch themselves out of arena somehow <- need to fix

- Game has no end <- NEED TO FIX
