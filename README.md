# RogueSphere

## Introduction 

This project is a personal project that i decided to make during summer holidays 2024. This allowed me to learn C# and Unity.
Everything has been made in 3 months. Sadly, i don't have enough time to continue this project for now, due to my studies.
This project is not meant to be downloaded and to be ran on another computer as mine, as git is not configurated correctly yet.. :(
However, the scripts that are on the project can show my skills and what i have done. 
ChatGPT has been used to help me during this project, as i had no experiences at all with Unity.

## Description

The project consists on a rogue-like game, where you control a sphere with gun. Your goal is to survive as long as possible in a map that is generated procedurally.
Your gun can evolve while leveling-up by killing enemies, and use a potential huge amount of differents guns.

### Weapons

I made the gun creation quite easy, by allowing everything about the statistics of a weapon to be modified in a .ods file.
The evolutions are working like a techtree : you can reach some evolutions only by reaching his lower evolutions before.
There are many types of guns, such as paint guns, shotguns, laser guns, drone guns, sniper guns, etc... Possibilities are endless
I can combine theses kinds of guns to make hybrid guns.

### Maps

The map is generated procedurally.
There are 2 kind of rooms that can be generated : boss rooms (not implemented yet) and normal rooms. The algorithm that generates the map is working.
The differents rooms are not yet implemented.

### Enemies

Enemies uses A* algorithm to move throught the obstacles to reach the player. Different kind of ennemies can be created very easily, with each enemy their own weapons, aggro range, firing range, and many more...
Enemies can basically use every weapon that the player can use, with some algorithms that allows them to pre-shot the player depending on his bullet velocity, the player speed and position, etc..
