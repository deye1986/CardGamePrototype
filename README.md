A Blackjack card game.

Important:<br>
Working from _Recovery/0(1) (because ive changed machines and somthing happened to the project, i dont know what..)<br>

Dave's Dev journal.

next job is to fix the count the scores logic.
next step to fix the deck Remove method, also clear the ui and restart the game after GameOver method called.
The remove appears to work but with visual bugs going into the next round.
A stand button has been placed on the UI, it needs hooking up properly in the script. 
A condition for the dealer to stand on 18 19 & 20 has been implemented, although im not sure if it is working, need to debug.<br>
gitingnore updated to ignore *.blob (i think they are crash files)<br>
stand button works as should. <br>
the deck has been hidden from scene and players view. they nolonger interfere with the starting of new hands.<br>


Job list.

Faulty ace logic - HasDealerBust & HasPlayerBust<br>
Improve the README<br>
add untracked files to gitignore sheet.<br>
the cards are being copied from the deck and drawn to the players hand. they need to be properly removed from the deck when when this happens.<br>
Destroying the Cards in GameOver() means there is a problem dealing cards at runtime. perhaps look for a alternative to recycle the cards.<br> 