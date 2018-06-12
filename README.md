# The ONLY Discord Invite Finder #

This program will find you all valid discord invites (Discodes).

And will either save them localy with the <a href="https://github.com/Jaminima/DiscordInviteFinder/tree/master/FinderBuilds/Solo">Solo Method.</a></br>
Or</br>
You may search in a <a href="https://github.com/Jaminima/DiscordInviteFinder/tree/master/FinderBuilds/PoolClient">Pool.</a></br>
Which will mean you might be able to view codes, at the pools hosts discression.</br>
</br>
For help getting started go <a href="https://github.com/Jaminima/DiscordInviteFinder/tree/master/FinderBuilds">Here</a>.

## How It Works ##

To describe crudely</br>
We iterate thorugh every possible Discode,</br>
Starting at "aaaaaa" and finishing at "999999".</br>
Each time we iterate through we use the discord API to check its validity</br>
If it is valid we store it.

## Effectivness ##

We have tried our best to make the process as fast as possible.</br>
To Do This We Have Added:
### Ram Limiting ###
The program will cull all inactive threads while there are more than 1000 threads.</br>
We reuse as much data as possible (ie Global Variables) to ensure as little ram is used as possible.
### Threading ###
On Pool Finder there is a thread with above normal priority,</br>
which listens for any server messages.</br>
On both Solo and Pool each code check gets its own thread which will be auto culled,</br>
as soon as it is finished.
### Pool Load Splitting ###
Each client connected to the pool will have a equal propotrion of all possible Discodes.</br>
The splitting structure follows this sequence:</br>

1 : 1/1</br>
2 : 1/2</br>
3+4 : 1/4</br>
5+6+7+8 : 1/8</br>
n : `1/(next power of 2 >= n)`

## Processing Complexity ##

The Character set is 62</br>
And the discord invite is 6 charcters.</br>
Hence 62^6 = 56,800,235,584.</br>
From my testing on an I5-4570s, i average 10,000 Codes/sec and Robo gets around 5,000.</br>
Therfore it would take me ~65 Days and Robo ~132 Days.</br>
So To approximate the number of days is (57 * 10^9) / (Codes/Sec)*86000.</br>
</br>
This is why solo isnt such a good idea!

## What Does This Open Up? ##

As we will have a list of loads of valid invites we will have access to some servers</br>
that people wanted to keep private,</br>
Or as we found out Group DM's, which is strange as we cant get invites for adding friends.</br>
</br>
Being able to join these may seem intrusive to some of the server/group members.</br>
Hence discretion is adivised but will fall onto the person in possession of the link.</br>
