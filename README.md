# Code Sample
A VR shooter (uses an obsolete google VR plugin)

A couple things to note:
1. I didn't make proper weapons. Enemies still take damage and can be killed, however there is no visual
representation. I made only 1 type of enemy with 1 type of movement. That enemy has 50 health and takes 10 damage from shots,
so it takes 5 shots to kill each enemy.
2. You will see a couple of missing behaviours, I planned on adding them (like different movement pattens and enemies), but
run out of time. The game works without errors (except for one rare edge case, I left a comment about it.)
3. The game ends, when your HP runs out, however you cannot restart the game and the 'x' button to close it doesn't seem
to be working for some reason. Enter settings and from there you can just shut the game down.

To run the game:
1. In editor: Open the project in Unity 2019.4.39 and press play. Hold shift to tilt your view and alt to rotate around.
Click when cursor is on enemy to shoot.
2. On the phone: Copy the provided VRShooter.apk to your phone and install it. Once installed open the app and don't
forget the Google Cardboard VR. Click the button on Cardboard to shoot when cursor is over an enemy.
