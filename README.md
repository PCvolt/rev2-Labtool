# GGxrd rev2 Labtool
External tool adding more features to the training mode of Guilty Gear Xrd Rev2.

## How to use
0. Download rev2-Labtool here: https://github.com/PCvolt/rev2-Labtool/releases/download/0.2.0/Release.Framework.0.2.0.7z
2. Boot up Labtool. It is recommended to close and start it up again everytime you switch characters.

## Features
- Value displays: HP, defense modifier, meter, RISC, dizzy with thresholds
- Frame advantage (even on Roman Cancels!)
- Gap display for both characters(up to 30F gaps)

## Upcoming features
- Guts display
- Positions display
- Gap display on hit

## Known bugs
- Crashes when changing characters, hence the recommendation.
- If you have the wrong characters displayed, you may want to hit left then right on the training menu's selected characters so that Labtool updates.
- Playing as P2 swaps some info on the display.
- Blocking animation and Preparing to block animation are the same, meaning that if you hold back in advance for blocking, the blockstring may be displayed as tight. Test your blockstrings on a dummy.
- Attacks such as blitz or Ramlethal 5PPP have a part of their recovery animation which is cancellable but has no (found) indication of being so. To display the proper frame advantage from those attacks, cancel the recovery with walk.

The project migrated from .NET Core to .NET Framework for compatibility purposes. 
