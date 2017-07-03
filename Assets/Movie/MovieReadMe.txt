


//loading image
// effect
// effect decided at random, but a question table may contain included and exluded effect if any
// effect table will be a constant market data seperatly
// effect table will have min max limit of effect's parameter
// and depending on the current level toughness, randomally choosen effect's parameter value will be picked
// sound viz will have many prefab with diff viz effect

question can be of image/gif/sound/TEXT type...
question toughness :
    depends on asset toughness, (how tough screenshot is)
    question title, what exactly was asked .
they should have min and max level , or some funtion with distribution among level

there should be option to force no effect condition for some image .. as some movie screenshot can be manully special effected , like cartoonize, face swap, cartoon logo, 

//asset downloader :
// question have two things, asset and json
// json will be on db/table , eg put it in server side only
// but assets for now ,some, can be put with apk assets...
// for package or so, we can download many images at once, as a zip/asset-bundle .. and put to sdcard..and then read from sdcard
//image/assetbundles will be on firebase storage. Don’t know how to load asset-bundle using firebase sdk.

four option, with mulitp correct may be
two answer  type
option based, or fill in the blanks/
each fill in the blanks can have own lifeline 3-4. you can buy further or get it converted from other resourc/hints..???
but then will have to think how session ends..

??
1. is how session ends
2. how carrier/levelprogress work
3. what carrency conversion wll be , from package/puzzle to carriere



Effect:
replace face with different actor.... extra reward question.. as such asset making  is tough
or put effects only on face.. or remove face with white background.... we can upload image with a binary mask, then shader can take care of rest
unhide blocks.. as effect

Logo:
======

name:
=====
Bollywood: Game of guess
Bollywood: The Game
(bollywood, game , guess, movie, cinema, film)
Don't duplicate
unique name (but english word), so that google search can also give your app name as result 
under 11 character
spell well, prononciation, clear, grey/gray not good, 
subtitle
google: in description use the main keywords at least 5-5 times
if possible try to include the keyword in name
google : video is very much important

keyword:
comma seperate, no phrase, no special character, 
No need to put trivial namse always: Better to be in the top 5 results for an average-searched keyword than in the top 100 for a highly-searched keyword
 It doesn’t matter if you rank #542 for a keyword that gets a billion searches a month. Nobody will scroll that far in search results to find you. Look for keywords that you can rank in the top 10 for.

block the name on facebook, tutuor, domain

rateapp popup, contact us/mail us for complainl,... make it easy for unhappy user to reach u... create forum for discussion.
add support address in itune
on Fixing theror complain , reply with If you like the app, don’t hesitate to support us and leave a review on the App Store

deep link : 
in setting ‘share app’ button which will share a deep link to friends.
Even a question a share button (saying ‘ask friends’). It will create deep link with that question with its image and share it.
The receiver of this link will directly see this question… if he has not played to that level or not eligible (mostly it would be the case), user
will just see this question and get bonus marks for it and then go back to home page of app.
(or complete package itself, don’t know where to put share button for package?)

Can insentive/reward  on the invites done.
Trick like keep some feature (not important) locked and on share/invite it will get unlocked… keep it remote configurable as user may not like this.. 


App indexing : to let ur app get searched in google search. To bring back old user also.


lock a email address with app name, and support/feedback email with app name
Don’t use push notifications, these should be for valuable data and not for you to beg for a good rating 😉
reward in exchange of review :) not sure if is right , moral ethical

question asset name???? randomly genreted uid, but should be small , or should make sense not random, but should not coflict with others

Resource:
===========
Level with XP (
    for carrier only, but this will be considered as main game , so level is the unit to test success in the game
)
Gold (
     premium currency, 
     source: can be bought with money,
     source: as reward , on question answered, game success, package/carrier complete/levelup.
     sink: can be used to unlock game... (otherwise game will be unlocked after its min level set (normally this level should be big))
     sink: can be used to unlock Package if minimum level is fullfiled, and not unlocked yet... 
           (otherwise normally packages will be unlock by its own after some time(fixed time after minlevel reach or after minlevel+extralevel,))
 )
 Hint (
     source: as reward , on question answered, game success, package/carrier complete/levelup.
     source : can be bought from money, or can be converted from gold??
     sink: hint for question asked in carrier/package.
     Hint types : {
        
     }
 )
 Ticket (
 ??? is required
 to start level-jump , or start package, or game /// 

 Can buy with money
 But User should not be blocked .... user should have aleast one always.. then what is the point in this....
 can ask user to come tommorow to claim and get ticket, and play further. Or can keep package out of ticket (either free or gold priced)

 )

Carrirer:
===========
  level 1-----2-------3 , each with some(10) questions
while each levelup you will get 3/5 freedom to do wrong letter selection. But if answer given is wrong
you will jump back to old level, ie no levelup. 
For each qustion answered you will get some reward (it will be gold or hints or tickets)
For level up , some level up reward will also be given

Hint is overall, but for carrier level jump there is there can be limit to the HINT used.

For starting each level-jurney (from level 2 to 3) , some currency (only ticket or some gold may be) can be charged


Packages:
===========
Will show notification on package icon , if new packages shows up or gets unlocked
each package-item will have it sticker image movie poster , containing mixture of all the questio image in that package
each packageitem will have some theme song/music (or else default). horror, comedy, romantic, 
normally packages will be unlock by its own after some time(fixed time after minlevel reach or after minlevel+extralevel,))
or can be bought with gold but it should be crossed the minlevel.
each package can be played multiple times and best of all time played till date will be considered and updated with

Packages should be allowed to play multiple times , But why would they play it multiple time?
May be we can keep package play time bound, count/down.. () .. and keep best score of all + playing reward(minimal)

Game:
======
minigame will have own logo 
notification if any new game comes/uunlock
games have some minimum level (it should be little higher), 
but can be bought with gold.
will be rewarded with gold/hint/ticket
to start each game, need to pay some fee (gold/ticket)

MiniGame : Mystry box : Puzzle
1. find diff in two same image
2. grid4
3. slot
4. dog block, circular block
5. maze games > 3d album

HotEvent:
===========
Can keep hot event quize one time play, and then vanishes... 
notification
1. Daily bonus (
 just need to claim
 randomally/cmsable to give differet resouce in daily bonus
 )
2. Hot news ?? will be shown on the outermost front screen/icon of hotevent.. not imp
3. One hot latest trending upcoming movie boxoffice buget gk question , with good reward(rare currency or may be hint)
4. Daily quest ( to play this game or packages , 
and extra quest complete reward will be given, 
and to start this game mey be start price /ticket/gold not charged
and reward given can be 2Xed



Resource Market:
=================
can buy gold, hint, ticket.
Sale:
=====
Can unlock package or game with sale/lesser price.
Can buy hint , gold , ticket with sale price.


Jam Popup:
==============
When out of Hint or ticket, show user how to get that, 
market popup redirect, tell him to come after one day to claim reward, redirect him to play package or game to earn resource
Put analytics/bi/AI to detect ,user is running out of some resource , and reward him that resource(like as luck give some tickets/hints), Sale accordingly

Feature/liveOps/cmsable time to unlock :
=========================================


LeaderBoard (Boxoffice report):
=================================



Push Notification/ Signalone:
==============================


Unity Ads:
============
Video ad as rewarded ads in Resource Market... to buy currency(hint/gold) free. (tapjoy in future for free buy)
Full screen ad (or video ad..NO) on Level-jump, package , game complete.
Video and in front 3d UI
Banner ads in Front UI, and in games
Lates movie ad/video ad , as full screen or front UI..... this is best suited and most revenue generator

Unity Analytics : 
==================
(Firebase not for unity right now)


Service: http://unity3d.com/services
http://www.apptamin.com/blog/app-developer-tools/

In app: unity iapp , https://unity3d.com/learn/tutorials/topics/analytics/integrating-unity-iap-your-game, 
//https://unity3d.com/learn/tutorials/topics/analytics/integrating-unity-iap-your-game
//http://docs.unity3d.com/Manual/UnityIAPSettingUp.html?_ga=1.249426894.89713102.1464271253
//http://unityads.unity3d.com/help/help/resources
Monetize Ads : unity ad, everyplay , rewarded ad , if somehow can get movie trailer ads > Done , reward ad with unity

Analytics : unity analytics , firebase > Done with Unity

Advertice :
Crash report : https://fabric.io/onboard, criticism > Done
Push Notification : https://onesignal.com/ > Done
Multiplayer : player.io (does it work in unity5), unity network


Sound:
==========
eagle , desert, cowboy sound


Setting:
==========


Multiplayer:
==============
tounament
non-live multiplayer, as tournament
Live multiplayer


Crashalytics:
====================



Terms:
=========
Oscor
filmfare
footage
star
trailers… teasers… 
flashback
release , coming soon
takkies
screenplay
box office
blockBuster
take
reel


Copycat: question
poster replace with diffrent actor face

User_Package_state:
CURRENCY_LEVEL_LOCK : will not be saved in userpackage : cost will be factor of diff with user level
FORCED_LEVEL_LOCK : will not be saved in userpackage : " ,
CURRENCY_TIME_LOCK : initiated for user
UNLOCK
UNPLAYED
PLAYED     : userpackage contain score detail
WELLPLAYED : "
COMPLETED
EXPIRED

User_Question_state:
UnSeen : not saved in userquestion
wrong:
correct:



Bug:
server side:
if error comes in userpackagemodel or usercurrency,, then for next/further users also they comes as blank;