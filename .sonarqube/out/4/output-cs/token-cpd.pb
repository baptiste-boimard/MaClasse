ê
NC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Services\UserService.cs
	namespace 	
Service
 
. 
Database 
. 
Services #
;# $
public 
class 
UserService 
{ 
private 
readonly 

HttpClient 
_httpClient  +
;+ ,
private		 
readonly		 
IConfiguration		 #
_configuration		$ 2
;		2 3
public 

UserService 
( 

HttpClient 

httpClient 
, 
IConfiguration 
configuration $
)$ %
{ 
_httpClient 
= 

httpClient  
;  !
_configuration 
= 
configuration &
;& '
} 
public 

async 
Task 
< 
SessionData !
>! "
GetUserByIdSession# 5
(5 6
string6 <
	idSession= F
)F G
{ 
var 
response 
= 
await 
_httpClient (
.( )
PostAsJsonAsync) 8
(8 9
$" 
{ 
_configuration 
[ 
$str .
]. /
}/ 0
$str0 B
"B C
,C D
newE H 
UserBySessionRequestI ]
{ 
	IdSession 
= 
	idSession %
} 
) 
; 
if 

( 
response 
. 
IsSuccessStatusCode (
)( )
{ 	
var 
userSession 
= 
await #
response$ ,
., -
Content- 4
.4 5
ReadFromJsonAsync5 F
<F G
SessionDataG R
>R S
(S T
)T U
;U V
return!! 
userSession!! 
;!! 
}"" 	
return$$ 
null$$ 
;$$ 
}%% 
}&& ¥K
RC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Services\HolidaysService.cs
public 
class 
HolidaysService 
{ 
private		 
readonly		 

HttpClient		 
_httpClient		  +
;		+ ,
public 

HolidaysService 
( 

HttpClient 

httpClient 
) 
{ 
_httpClient 
= 

httpClient  
;  !
} 
public 

async 
Task 
< 
List 
< 
Appointment &
>& '
>' ("
GetZoneBVacationsAsync) ?
(? @
UserProfile@ K
userL P
)P Q
{ 
string 
zone 
= 
user 
. 
Zone 
[  
user  $
.$ %
Zone% )
.) *
Length* 0
-1 2
$num3 4
]4 5
.5 6
ToString6 >
(> ?
)? @
;@ A
var 
options 
= 
new !
JsonSerializerOptions /
{ 	'
PropertyNameCaseInsensitive '
=( )
true* .
} 	
;	 

var 
apiUrl 
= 
$" 
$str	 Ÿ
{
Ÿ ⁄
zone
⁄ ﬁ
}
ﬁ ﬂ
$str
ﬂ °
"
° ¢
;
¢ £
using   
var   
request   
=   
new   
HttpRequestMessage    2
(  2 3

HttpMethod  3 =
.  = >
Get  > A
,  A B
apiUrl  C I
)  I J
;  J K
var"" 
response"" 
="" 
await"" 
_httpClient"" (
.""( )
GetAsync"") 1
(""1 2
apiUrl""2 8
)""8 9
;""9 :
response## 
.## #
EnsureSuccessStatusCode## (
(##( )
)##) *
;##* +
var%% 
result%% 
=%% 
await%% 
response%% #
.%%# $
Content%%$ +
.%%+ ,
ReadFromJsonAsync%%, =
<%%= >
ApiResultResponse%%> O
>%%O P
(%%P Q
options%%Q X
)%%X Y
;%%Y Z
var(( 
distinctVacations(( 
=(( 
result((  &
.((& '
Results((' .
.)) 
Where)) 
()) 
r)) 
=>)) 
r)) 
.)) 
EndDate)) !
.))! "
Date))" &
!=))' )
new))* -
DateTime)). 6
())6 7
$num))7 ;
,)); <
$num))= >
,))> ?
$num))@ B
)))B C
)))C D
.** 
ToList** 
(** 
)** 
;** 
var-- 
appointments-- 
=-- 
new-- 
List-- #
<--# $
Appointment--$ /
>--/ 0
(--0 1
)--1 2
;--2 3
foreach// 
(// 
var// 
appointment//  
in//! #
distinctVacations//$ 5
)//6 7
{00 	
var11 
appt11 
=11 
new11 
Appointment11 &
(11& '
)11' (
;11( )
if33 
(33 
appointment33 
.33 
Description33 '
.33' (
Contains33( 0
(330 1
$str331 K
)33K L
)33L M
{44 
string66 
dateReprise202666 &
=66' (
$str66) D
;66D E
appt88 
=88 
new88 
Appointment88 &
{99 
Id:: 
=:: 
Guid:: 
.:: 
NewGuid:: %
(::% &
)::& '
.::' (
ToString::( 0
(::0 1
)::1 2
,::2 3
Start;; 
=;; 
appointment;; '
.;;' (
	StartDate;;( 1
,;;1 2
End<< 
=<< 
DateTime<< "
.<<" #
Parse<<# (
(<<( )
dateReprise2026<<) 8
)<<8 9
,<<9 :
Text== 
=== 
$str== +
,==+ ,
Color>> 
=>> 
$str>> %
,>>% &
	Recurring?? 
=?? 
false??  %
,??% &
IdRecurring@@ 
=@@  !
String@@" (
.@@( )
Empty@@) .
}AA 
;AA 
}BB 
elseCC 
{DD 
apptEE 
=EE 
newEE 
AppointmentEE &
{FF 
IdGG 
=GG 
GuidGG 
.GG 
NewGuidGG %
(GG% &
)GG& '
.GG' (
ToStringGG( 0
(GG0 1
)GG1 2
,GG2 3
StartHH 
=HH 
appointmentHH '
.HH' (
	StartDateHH( 1
,HH1 2
EndII 
=II 
appointmentII %
.II% &
EndDateII& -
,II- .
TextJJ 
=JJ 
appointmentJJ &
.JJ& '
DescriptionJJ' 2
,JJ2 3
ColorKK 
=KK 
$strKK %
,KK% &
	RecurringLL 
=LL 
falseLL  %
,LL% &
IdRecurringMM 
=MM  !
StringMM" (
.MM( )
EmptyMM) .
}NN 
;NN 
}PP 
appointmentsQQ 
.QQ 
AddQQ 
(QQ 
apptQQ !
)QQ! "
;QQ" #
}RR 	
returnSS 
appointmentsSS 
;SS 
}TT 
publicVV 

asyncVV 
TaskVV 
<VV 
ListVV 
<VV 
AppointmentVV &
>VV& '
>VV' (
GetPublicHolidayVV) 9
(VV9 :
UserProfileVV: E
userVVF J
)VVJ K
{WW 
varXX 
optionsXX 
=XX 
newXX !
JsonSerializerOptionsXX /
{YY 	'
PropertyNameCaseInsensitiveZZ '
=ZZ( )
trueZZ* .
}[[ 	
;[[	 

var]] 

apiUrlList]] 
=]] 
new]] 
List]] !
<]]! "
string]]" (
>]]( )
{^^ 	
$str__ M
,__M N
$str`` M
}aa 	
;aa	 

varcc 
appointmentscc 
=cc 
newcc 
Listcc #
<cc# $
Appointmentcc$ /
>cc/ 0
(cc0 1
)cc1 2
;cc2 3
foreachee 
(ee 
varee 
urlee 
inee 

apiUrlListee &
)ee& '
{ff 	
usinggg 
vargg 
requestgg 
=gg 
newgg  #
HttpRequestMessagegg$ 6
(gg6 7

HttpMethodgg7 A
.ggA B
GetggB E
,ggE F
urlggG J
)ggJ K
;ggK L
varii 
responseii 
=ii 
awaitii  
_httpClientii! ,
.ii, -
GetAsyncii- 5
(ii5 6
urlii6 9
)ii9 :
;ii: ;
responsejj 
.jj #
EnsureSuccessStatusCodejj ,
(jj, -
)jj- .
;jj. /
varll 
resultll 
=ll 
awaitll 
responsell '
.ll' (
Contentll( /
.ll/ 0
ReadFromJsonAsyncll0 A
<llA B

DictionaryllB L
<llL M
stringllM S
,llS T
stringllU [
>ll[ \
>ll\ ]
(ll] ^
optionsll^ e
)lle f
;llf g
foreachpp 
(pp 
varpp 
entrypp 
inpp !
resultpp" (
)pp( )
{qq 
varrr 
daterr 
=rr 
DateTimerr #
.rr# $
Parserr$ )
(rr) *
entryrr* /
.rr/ 0
Keyrr0 3
)rr3 4
;rr4 5
varss 
descriptionss 
=ss  !
entryss" '
.ss' (
Valuess( -
;ss- .
vartt 
appointmenttt 
=tt  !
newtt" %
Appointmenttt& 1
{uu 
Idvv 
=vv 
Guidvv 
.vv 
NewGuidvv %
(vv% &
)vv& '
.vv' (
ToStringvv( 0
(vv0 1
)vv1 2
,vv2 3
Startww 
=ww 
dateww  
.ww  !
Dateww! %
.ww% &
AddHoursww& .
(ww. /
$numww/ 0
)ww0 1
,ww1 2
Endxx 
=xx 
datexx 
.xx 
Datexx #
.xx# $
AddHoursxx$ ,
(xx, -
$numxx- /
)xx/ 0
,xx0 1
Textyy 
=yy 
$"yy 
$stryy *
{yy* +
descriptionyy+ 6
}yy6 7
"yy7 8
,yy8 9
Colorzz 
=zz 
$strzz %
,zz% &
	Recurring{{ 
={{ 
false{{  %
,{{% &
IdRecurring|| 
=||  !
string||" (
.||( )
Empty||) .
}}} 
;}} 
appointments 
. 
Add  
(  !
appointment! ,
), -
;- .
}
ÄÄ 
}
ÅÅ 	
return
ÉÉ 
appointments
ÉÉ 
;
ÉÉ 
}
ÑÑ 
}ÖÖ ®ƒ
YC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Controllers\SchedulerController.cs
	namespace 	
Service
 
. 
Database 
. 
Controllers &
;& '
[		 
ApiController		 
]		 
[

 
Route

 
(

 
$str

 
)

 
]

 
public 
class 
SchedulerController  
:! "
ControllerBase$ 2
{ 
private 
readonly 
UserService  
_userService! -
;- .
private 
readonly  
ISchedulerRepository ) 
_schedulerRepository* >
;> ?
private 
readonly 
HolidaysService $
_holidaysService% 5
;5 6
private 
readonly  
BlockVacationService )!
_blockVacationService* ?
;? @
public 

SchedulerController 
( 
UserService 
userService 
,   
ISchedulerRepository 
schedulerRepository 0
,0 1
HolidaysService 
holidaysService '
,' ( 
BlockVacationService  
blockVacationService 1
)1 2
{ 
_userService 
= 
userService "
;" # 
_schedulerRepository 
= 
schedulerRepository 2
;2 3
_holidaysService 
= 
holidaysService *
;* +!
_blockVacationService 
=  
blockVacationService  4
;4 5
} 
[ 
HttpPost 
] 
[ 
Route 

(
 
$str 
) 
] 
public   

async   
Task   
<   
IActionResult   #
>  # $
GetScheduler  % 1
(  1 2
CreateDataRequest  2 C
request  D K
)  K L
{!! 
var"" 
	scheduler"" 
="" 
await""  
_schedulerRepository"" 2
.""2 3
GetScheduler""3 ?
(""? @
request""@ G
.""G H
UserId""H N
)""N O
;""O P
if$$ 

($$ 
	scheduler$$ 
==$$ 
null$$ 
)$$ 
{%% 	
return&& 
NotFound&& 
(&& 
)&& 
;&& 
}'' 	
return)) 
Ok)) 
()) 
	scheduler)) 
))) 
;)) 
}** 
[,, 
HttpPost,, 
],, 
[-- 
Route-- 

(--
 
$str-- 
)-- 
]-- 
public.. 

async.. 
Task.. 
<.. 
IActionResult.. #
>..# $
GetManyScheduler..% 5
(..5 6
List..6 :
<..: ;
string..; A
>..A B
idsProfesseur..C P
)..P Q
{// 
var00 

schedulers00 
=00 
await00  
_schedulerRepository00 3
.003 4
GetManyScheduler004 D
(00D E
idsProfesseur00E R
)00R S
;00S T
if22 

(22 

schedulers22 
==22 
null22 
)22 
return22  &

BadRequest22' 1
(221 2
)222 3
;223 4
return44 
Ok44 
(44 

schedulers44 
)44 
;44 
}55 
[88 
HttpPost88 
]88 
[99 
Route99 

(99
 
$str99 
)99 
]99 
public:: 

async:: 
Task:: 
<:: 
IActionResult:: #
>::# $
AddScheduler::% 1
(::1 2
[::2 3
FromBody::3 ;
]::; <
CreateDataRequest::= N
request::O V
)::V W
{;; 
Console<< 
.<< 
	WriteLine<< 
(<< 
$"<< 
$str<< 3
"<<3 4
)<<4 5
;<<5 6
var>> 
newScheduler>> 
=>> 
await>>   
_schedulerRepository>>! 5
.>>5 6
AddScheduler>>6 B
(>>B C
request>>C J
.>>J K
UserId>>K Q
)>>Q R
;>>R S
return@@ 
Ok@@ 
(@@ 
newScheduler@@ 
)@@ 
;@@  
}AA 
[CC 
HttpPostCC 
]CC 
[DD 
RouteDD 

(DD
 
$strDD 
)DD 
]DD 
publicEE 

asyncEE 
TaskEE 
<EE 
IActionResultEE #
>EE# $
DeleteSchedulerEE% 4
(EE4 5
[EE5 6
FromBodyEE6 >
]EE> ?
DeleteUserRequestEE@ Q
requestEER Y
)EEY Z
{FF 
varGG 
deletedSchedulerGG 
=GG 
awaitGG $ 
_schedulerRepositoryGG% 9
.GG9 :
DeleteSchedulerGG: I
(GGI J
requestGGJ Q
.GGQ R
IdUserGGR X
)GGX Y
;GGY Z
ifII 

(II 
deletedSchedulerII 
==II 
nullII  $
)II$ %
returnII& ,
NotFoundII- 5
(II5 6
)II6 7
;II7 8
returnKK 
OkKK 
(KK 
deletedSchedulerKK "
)KK" #
;KK# $
}LL 
[NN 
HttpPostNN 
]NN 
[OO 
RouteOO 

(OO
 
$strOO 
)OO 
]OO 
publicPP 

asyncPP 
TaskPP 
<PP 
IActionResultPP #
>PP# $
AddAppointmentPP% 3
(PP3 4
[PP4 5
FromBodyPP5 =
]PP= >
SchedulerRequestPP? O
requestPPP W
)PPW X
{QQ 
varTT 
userSessionTT 
=TT 
awaitTT 
_userServiceTT  ,
.TT, -
GetUserByIdSessionTT- ?
(TT? @
requestTT@ G
.TTG H
	IdSessionTTH Q
)TTQ R
;TTR S
varWW 
existingAppointmentWW 
=WW  !
awaitWW" ' 
_schedulerRepositoryWW( <
.WW< =
GetOneAppointmentWW= N
(WWN O
userSessionXX 
.XX 
UserIdXX 
,XX 
requestXX  '
.XX' (
AppointmentXX( 3
)XX3 4
;XX4 5
var[[ 
newAppointment[[ 
=[[ 
new[[  
Appointment[[! ,
{\\ 	
Id]] 
=]] 
Guid]] 
.]] 
NewGuid]] 
(]] 
)]] 
.]]  
ToString]]  (
(]]( )
)]]) *
,]]* +
Start^^ 
=^^ 
request^^ 
.^^ 
Appointment^^ '
.^^' (
Start^^( -
,^^- .
End__ 
=__ 
request__ 
.__ 
Appointment__ %
.__% &
End__& )
,__) *
Text`` 
=`` 
request`` 
.`` 
Appointment`` &
.``& '
Text``' +
,``+ ,
Coloraa 
=aa 
requestaa 
.aa 
Appointmentaa '
.aa' (
Coloraa( -
,aa- .
	Recurringbb 
=bb 
requestbb 
.bb  
Appointmentbb  +
.bb+ ,
	Recurringbb, 5
,bb5 6
IdRecurringcc 
=cc 
requestcc !
.cc! "
Appointmentcc" -
.cc- .
	Recurringcc. 7
?dd 
Guiddd 
.dd 
NewGuiddd 
(dd 
)dd  
.dd  !
ToStringdd! )
(dd) *
)dd* +
:ee 
Stringee 
.ee 
Emptyee 
}ff 	
;ff	 

ifhh 

(hh 
!hh 
newAppointmenthh 
.hh 
	Recurringhh %
)hh% &
{ii 	
varjj 
addedAppointmentjj  
=jj! "
awaitjj# ( 
_schedulerRepositoryjj) =
.jj= >
AddAppointmentjj> L
(jjL M
userSessionkk 
.kk 
UserIdkk "
.kk" #
ToStringkk# +
(kk+ ,
)kk, -
,kk- .
newAppointmentkk/ =
)kk= >
;kk> ?
returnoo 
Okoo 
(oo 
addedAppointmentoo &
)oo& '
;oo' (
}pp 	
elseqq 
{rr 	
varuu 
newAppointmentsuu 
=uu  !
awaituu" ' 
_schedulerRepositoryuu( <
.uu< =
GetBlockVacationuu= M
(uuM N
userSessionuuN Y
.uuY Z
UserIduuZ `
,uu` a
newAppointmentuub p
)uup q
;uuq r
returnww 
Okww 
(ww 
newAppointmentsww %
)ww% &
;ww& '
}xx 	
}yy 
[|| 
HttpPost|| 
]|| 
[}} 
Route}} 

(}}
 
$str}} $
)}}$ %
]}}% &
public~~ 

async~~ 
Task~~ 
<~~ 
IActionResult~~ #
>~~# $!
AddHolidayToScheduler~~% :
(~~: ;
[~~; <
FromBody~~< D
]~~D E
UserProfile~~F Q
user~~R V
)~~V W
{ 
var
ÅÅ !
holidaysAppointment
ÅÅ 
=
ÅÅ  !
await
ÅÅ" '
_holidaysService
ÅÅ( 8
.
ÅÅ8 9$
GetZoneBVacationsAsync
ÅÅ9 O
(
ÅÅO P
user
ÅÅP T
)
ÅÅT U
;
ÅÅU V
var
ÉÉ 
holidaysPublic
ÉÉ 
=
ÉÉ 
await
ÉÉ "
_holidaysService
ÉÉ# 3
.
ÉÉ3 4
GetPublicHoliday
ÉÉ4 D
(
ÉÉD E
user
ÉÉE I
)
ÉÉI J
;
ÉÉJ K
var
ÖÖ 
allAppointments
ÖÖ 
=
ÖÖ !
holidaysAppointment
ÖÖ 1
.
ÖÖ1 2
Concat
ÖÖ2 8
(
ÖÖ8 9
holidaysPublic
ÖÖ9 G
)
ÖÖG H
.
ÖÖH I
OrderBy
ÖÖI P
(
ÖÖP Q
a
ÖÖQ R
=>
ÖÖS U
a
ÖÖV W
.
ÖÖW X
Start
ÖÖX ]
)
ÖÖ] ^
.
ÖÖ^ _
ToList
ÖÖ_ e
(
ÖÖe f
)
ÖÖf g
;
ÖÖg h
var
àà 
newScheduler
àà 
=
àà 
await
àà  "
_schedulerRepository
àà! 5
.
àà5 6 
AddListAppointment
àà6 H
(
ààH I
user
ààI M
.
ààM N
Id
ààN P
,
ààP Q
allAppointments
ààR a
)
ààa b
;
ààb c
if
ää 

(
ää 
newScheduler
ää 
==
ää 
null
ää  
)
ää  !
return
ää" (

BadRequest
ää) 3
(
ää3 4
)
ää4 5
;
ää5 6
return
åå 
Ok
åå 
(
åå 
newScheduler
åå 
)
åå 
;
åå  
}
çç 
[
èè 
HttpPost
èè 
]
èè 
[
êê 
Route
êê 

(
êê
 
$str
êê 
)
êê  
]
êê  !
public
ëë 

async
ëë 
Task
ëë 
<
ëë 
IActionResult
ëë #
>
ëë# $
UpdateAppointment
ëë% 6
(
ëë6 7
[
ëë7 8
FromBody
ëë8 @
]
ëë@ A
SchedulerRequest
ëëB R
request
ëëS Z
)
ëëZ [
{
íí 
var
îî 
userSession
îî 
=
îî 
await
îî 
_userService
îî  ,
.
îî, - 
GetUserByIdSession
îî- ?
(
îî? @
request
îî@ G
.
îîG H
	IdSession
îîH Q
)
îîQ R
;
îîR S
var
óó !
existingAppointment
óó 
=
óó  !
await
óó" '"
_schedulerRepository
óó( <
.
óó< =#
GetOneAppointmentById
óó= R
(
óóR S
userSession
òò 
.
òò 
UserId
òò 
,
òò 
request
òò  '
.
òò' (
Appointment
òò( 3
)
òò3 4
;
òò4 5
if
öö 

(
öö !
existingAppointment
öö 
==
öö  "
null
öö# '
)
öö' (
return
öö) /

BadRequest
öö0 :
(
öö: ;
)
öö; <
;
öö< =
if
üü 

(
üü 
request
üü 
.
üü 
Appointment
üü 
.
üü  
	Recurring
üü  )
&&
üü* ,
!
üü- .
string
üü. 4
.
üü4 5
IsNullOrEmpty
üü5 B
(
üüB C
request
üüC J
.
üüJ K
Appointment
üüK V
.
üüV W
IdRecurring
üüW b
)
üüb c
)
üüc d
{
†† 	
var
°° 
idRecurring
°° 
=
°° 
request
°° %
.
°°% &
Appointment
°°& 1
.
°°1 2
IdRecurring
°°2 =
;
°°= >
var
¢¢ 
	startDate
¢¢ 
=
¢¢ 
request
¢¢ #
.
¢¢# $
Appointment
¢¢$ /
.
¢¢/ 0
Start
¢¢0 5
;
¢¢5 6
var
§§ "
appointmenetsDeleted
§§ $
=
§§% &
await
•• "
_schedulerRepository
•• *
.
••* +#
DeleteListAppointment
••+ @
(
••@ A
userSession
••A L
.
••L M
UserId
••M S
,
••S T
idRecurring
••U `
,
••` a
	startDate
••b k
)
••k l
;
••l m
if
ßß 
(
ßß "
appointmenetsDeleted
ßß $
==
ßß% '
null
ßß( ,
)
ßß, -
return
ßß. 4

BadRequest
ßß5 ?
(
ßß? @
)
ßß@ A
;
ßßA B
var
©© 
newAppointments
©© 
=
©©  !
await
™™ "
_schedulerRepository
™™ *
.
™™* +
GetBlockVacation
™™+ ;
(
™™; <
userSession
™™< G
.
™™G H
UserId
™™H N
,
™™N O
request
™™P W
.
™™W X
Appointment
™™X c
)
™™c d
;
™™d e
if
¨¨ 
(
¨¨ 
newAppointments
¨¨ 
==
¨¨  "
null
¨¨# '
)
¨¨' (
return
¨¨) /

BadRequest
¨¨0 :
(
¨¨: ;
)
¨¨; <
;
¨¨< =
var
ØØ (
listAppointmentAfterDelete
ØØ *
=
ØØ+ ,
await
∞∞ "
_schedulerRepository
∞∞ *
.
∞∞* +
DeleteAppointment
∞∞+ <
(
∞∞< =
userSession
∞∞= H
.
∞∞H I
UserId
∞∞I O
,
∞∞O P
request
∞∞Q X
.
∞∞X Y
Appointment
∞∞Y d
)
∞∞d e
;
∞∞e f
if
≤≤ 
(
≤≤ (
listAppointmentAfterDelete
≤≤ *
==
≤≤+ -
null
≤≤. 2
)
≤≤2 3
return
≤≤4 :

BadRequest
≤≤; E
(
≤≤E F
)
≤≤F G
;
≤≤G H
return
µµ 
Ok
µµ 
(
µµ (
listAppointmentAfterDelete
µµ 0
)
µµ0 1
;
µµ1 2
}
∂∂ 	
if
∫∫ 

(
∫∫ 
request
∫∫ 
.
∫∫ 
Appointment
∫∫ 
.
∫∫  
	Recurring
∫∫  )
&&
∫∫* ,
string
∫∫- 3
.
∫∫3 4
IsNullOrEmpty
∫∫4 A
(
∫∫A B
request
∫∫B I
.
∫∫I J
Appointment
∫∫J U
.
∫∫U V
IdRecurring
∫∫V a
)
∫∫a b
)
∫∫b c
{
ªª 	
request
ΩΩ 
.
ΩΩ 
Appointment
ΩΩ 
.
ΩΩ  
IdRecurring
ΩΩ  +
=
ΩΩ, -
Guid
ΩΩ. 2
.
ΩΩ2 3
NewGuid
ΩΩ3 :
(
ΩΩ: ;
)
ΩΩ; <
.
ΩΩ< =
ToString
ΩΩ= E
(
ΩΩE F
)
ΩΩF G
;
ΩΩG H
var
¿¿ %
newRecurringAppointment
¿¿ '
=
¿¿( )
await
¡¡ #
_blockVacationService
¡¡ +
.
¡¡+ ,+
GetAppointmentWithoutVacation
¡¡, I
(
¡¡I J
userSession
¡¡J U
.
¡¡U V
UserId
¡¡V \
,
¡¡\ ]
request
¡¡^ e
.
¡¡e f
Appointment
¡¡f q
)
¡¡q r
;
¡¡r s
if
√√ 
(
√√ %
newRecurringAppointment
√√ '
==
√√( *
null
√√+ /
)
√√/ 0
return
√√1 7
null
√√8 <
;
√√< =
var
∆∆ '
recurringAppointmentAdded
∆∆ )
=
∆∆* +
await
«« "
_schedulerRepository
«« *
.
««* + 
AddListAppointment
««+ =
(
««= >
userSession
««> I
.
««I J
UserId
««J P
,
««P Q%
newRecurringAppointment
««R i
)
««i j
;
««j k
if
…… 
(
…… '
recurringAppointmentAdded
…… )
==
……* ,
null
……- 1
)
……1 2
return
……3 9
null
……: >
;
……> ?
var
ÃÃ (
listAppointmentAfterDelete
ÃÃ *
=
ÃÃ+ ,
await
ÕÕ "
_schedulerRepository
ÕÕ *
.
ÕÕ* +
DeleteAppointment
ÕÕ+ <
(
ÕÕ< =
userSession
ÕÕ= H
.
ÕÕH I
UserId
ÕÕI O
,
ÕÕO P
request
ÕÕQ X
.
ÕÕX Y
Appointment
ÕÕY d
)
ÕÕd e
;
ÕÕe f
if
œœ 
(
œœ (
listAppointmentAfterDelete
œœ *
==
œœ+ -
null
œœ. 2
)
œœ2 3
return
œœ4 :

BadRequest
œœ; E
(
œœE F
)
œœF G
;
œœG H
return
““ 
Ok
““ 
(
““ (
listAppointmentAfterDelete
““ 0
)
““0 1
;
““1 2
}
‘‘ 	
if
◊◊ 

(
◊◊ 
!
◊◊ 
request
◊◊ 
.
◊◊ 
Appointment
◊◊  
.
◊◊  !
	Recurring
◊◊! *
&&
◊◊+ -
!
◊◊. /
string
◊◊/ 5
.
◊◊5 6
IsNullOrEmpty
◊◊6 C
(
◊◊C D
request
◊◊D K
.
◊◊K L
Appointment
◊◊L W
.
◊◊W X
IdRecurring
◊◊X c
)
◊◊c d
)
◊◊d e
{
ÿÿ 	
var
ŸŸ 
idRecurring
ŸŸ 
=
ŸŸ 
request
ŸŸ %
.
ŸŸ% &
Appointment
ŸŸ& 1
.
ŸŸ1 2
IdRecurring
ŸŸ2 =
;
ŸŸ= >
var
⁄⁄ 
	startDate
⁄⁄ 
=
⁄⁄ 
request
⁄⁄ #
.
⁄⁄# $
Appointment
⁄⁄$ /
.
⁄⁄/ 0
Start
⁄⁄0 5
;
⁄⁄5 6
request
›› 
.
›› 
Appointment
›› 
.
››  
	Recurring
››  )
=
››* +
false
››, 1
;
››1 2
request
ﬁﬁ 
.
ﬁﬁ 
Appointment
ﬁﬁ 
.
ﬁﬁ  
IdRecurring
ﬁﬁ  +
=
ﬁﬁ, -
String
ﬁﬁ. 4
.
ﬁﬁ4 5
Empty
ﬁﬁ5 :
;
ﬁﬁ: ;
var
‡‡ "
appointmenetsDeleted
‡‡ $
=
‡‡% &
await
·· "
_schedulerRepository
·· *
.
··* +#
DeleteListAppointment
··+ @
(
··@ A
userSession
··A L
.
··L M
UserId
··M S
,
··S T
idRecurring
··U `
,
··` a
	startDate
··b k
)
··k l
;
··l m
if
„„ 
(
„„ "
appointmenetsDeleted
„„ $
==
„„% '
null
„„( ,
)
„„, -
return
„„. 4

BadRequest
„„5 ?
(
„„? @
)
„„@ A
;
„„A B
}
‰‰ 	
var
ËË  
updatedAppointment
ËË 
=
ËË  
await
ËË! &"
_schedulerRepository
ËË' ;
.
ËË; <
UpdateAppointment
ËË< M
(
ËËM N
userSession
ÈÈ 
.
ÈÈ 
UserId
ÈÈ 
,
ÈÈ 
request
ÈÈ  '
.
ÈÈ' (
Appointment
ÈÈ( 3
)
ÈÈ3 4
;
ÈÈ4 5
return
ÎÎ 
Ok
ÎÎ 
(
ÎÎ  
updatedAppointment
ÎÎ $
)
ÎÎ$ %
;
ÎÎ% &
}
ÏÏ 
[
ÓÓ 
HttpPost
ÓÓ 
]
ÓÓ 
[
ÔÔ 
Route
ÔÔ 

(
ÔÔ
 
$str
ÔÔ 
)
ÔÔ  
]
ÔÔ  !
public
 

async
 
Task
 
<
 
IActionResult
 #
>
# $
DeleteAppointment
% 6
(
6 7
[
7 8
FromBody
8 @
]
@ A
SchedulerRequest
B R
request
S Z
)
Z [
{
ÒÒ 
var
ÛÛ 
userSession
ÛÛ 
=
ÛÛ 
await
ÛÛ 
_userService
ÛÛ  ,
.
ÛÛ, - 
GetUserByIdSession
ÛÛ- ?
(
ÛÛ? @
request
ÛÛ@ G
.
ÛÛG H
	IdSession
ÛÛH Q
)
ÛÛQ R
;
ÛÛR S
var
ˆˆ !
existingAppointment
ˆˆ 
=
ˆˆ  !
await
ˆˆ" '"
_schedulerRepository
ˆˆ( <
.
ˆˆ< =#
GetOneAppointmentById
ˆˆ= R
(
ˆˆR S
userSession
˜˜ 
.
˜˜ 
UserId
˜˜ 
,
˜˜ 
request
˜˜  '
.
˜˜' (
Appointment
˜˜( 3
)
˜˜3 4
;
˜˜4 5
if
˘˘ 

(
˘˘ !
existingAppointment
˘˘ 
==
˘˘  "
null
˘˘# '
)
˘˘' (
return
˘˘) /

BadRequest
˘˘0 :
(
˘˘: ;
)
˘˘; <
;
˘˘< =
if
¸¸ 

(
¸¸ 
!
¸¸ 
request
¸¸ 
.
¸¸ 
Appointment
¸¸  
.
¸¸  !
	Recurring
¸¸! *
)
¸¸* +
{
˝˝ 	
var
˛˛  
deletedAppointment
˛˛ "
=
˛˛# $
await
˛˛% *"
_schedulerRepository
˛˛+ ?
.
˛˛? @
DeleteAppointment
˛˛@ Q
(
˛˛Q R
userSession
ˇˇ 
.
ˇˇ 
UserId
ˇˇ "
,
ˇˇ" #
request
ˇˇ$ +
.
ˇˇ+ ,
Appointment
ˇˇ, 7
)
ˇˇ7 8
;
ˇˇ8 9
if
ÅÅ 
(
ÅÅ  
deletedAppointment
ÅÅ "
==
ÅÅ# %
null
ÅÅ& *
)
ÅÅ* +
return
ÅÅ, 2

BadRequest
ÅÅ3 =
(
ÅÅ= >
)
ÅÅ> ?
;
ÅÅ? @
return
ÉÉ 
Ok
ÉÉ 
(
ÉÉ  
deletedAppointment
ÉÉ (
)
ÉÉ( )
;
ÉÉ) *
}
ÑÑ 	
else
ÖÖ 
{
ÜÜ 	
var
àà  
deletedAppointment
àà "
=
àà# $
await
ââ "
_schedulerRepository
ââ *
.
ââ* +
DeleteAppointment
ââ+ <
(
ââ< =
userSession
ââ= H
.
ââH I
UserId
ââI O
,
ââO P
request
ââQ X
.
ââX Y
Appointment
ââY d
)
ââd e
;
ââe f
if
ãã 
(
ãã  
deletedAppointment
ãã "
==
ãã# %
null
ãã& *
)
ãã* +
return
ãã, 2

BadRequest
ãã3 =
(
ãã= >
)
ãã> ?
;
ãã? @
var
éé 
idRecurring
éé 
=
éé 
request
éé %
.
éé% &
Appointment
éé& 1
.
éé1 2
IdRecurring
éé2 =
;
éé= >
var
èè 
	startDate
èè 
=
èè 
request
èè #
.
èè# $
Appointment
èè$ /
.
èè/ 0
Start
èè0 5
;
èè5 6
var
ëë !
deletedAppointments
ëë #
=
ëë$ %
await
íí "
_schedulerRepository
íí *
.
íí* +#
DeleteListAppointment
íí+ @
(
íí@ A
userSession
ííA L
.
ííL M
UserId
ííM S
,
ííS T
idRecurring
ííU `
,
íí` a
	startDate
ííb k
)
íík l
;
ííl m
if
îî 
(
îî !
deletedAppointments
îî #
.
îî# $
Count
îî$ )
==
îî* ,
$num
îî- .
)
îî. /
return
îî0 6

BadRequest
îî7 A
(
îîA B
)
îîB C
;
îîC D
var
ôô 
	scheduler
ôô 
=
ôô 
await
ôô !"
_schedulerRepository
ôô" 6
.
ôô6 7
GetScheduler
ôô7 C
(
ôôC D
userSession
ôôD O
.
ôôO P
UserId
ôôP V
)
ôôV W
;
ôôW X
if
õõ 
(
õõ 
	scheduler
õõ 
==
õõ 
null
õõ !
)
õõ! "
return
õõ# )

BadRequest
õõ* 4
(
õõ4 5
)
õõ5 6
;
õõ6 7
return
ùù 
Ok
ùù 
(
ùù 
	scheduler
ùù 
.
ùù  
Appointments
ùù  ,
)
ùù, -
;
ùù- .
}
ûû 	
}
üü 
}†† ‰D
WC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Services\BlockVacationService.cs
	namespace 	
Service
 
. 
Database 
. 
Services #
;# $
public 
class  
BlockVacationService !
{		 
private

 
readonly

 
MongoDbContext

 #
_mongoDbContext

$ 3
;

3 4
public 
 
BlockVacationService 
(  
MongoDbContext  .
mongoDbContext/ =
)= >
{ 
_mongoDbContext 
= 
mongoDbContext (
;( )
} 
public 

async 
Task 
< 
List 
< 
Appointment &
>& '
>' ()
GetAppointmentWithoutVacation) F
(F G
string 
userId 
, 
Appointment 
appointment 
,  
CancellationToken 
ct 
= 
default &
)& '
{ 
int 
terminatedYear 
= 
appointment (
.( )
Start) .
.. /
Month/ 4
>=5 7
$num8 9
?: ;
appointment< G
.G H
StartH M
.M N
YearN R
+S T
$numU V
:W X
appointmentY d
.d e
Starte j
.j k
Yeark o
;o p
var 
	scheduler 
= 
await 
_mongoDbContext -
.- .

Schedulers. 8
. 
Find 
( 
Builders 
< 
	Scheduler $
>$ %
.% &
Filter& ,
., -
Eq- /
(/ 0
s0 1
=>2 4
s5 6
.6 7
IdUser7 =
,= >
userId? E
)E F
)F G
. 
FirstOrDefaultAsync  
(  !
ct! #
)# $
;$ %
var 
appointmentList 
= 
	scheduler '
.' (
Appointments( 4
;4 5
var!! 
vacationSummer!! 
=!! 
appointmentList!! ,
."" 
Select"" 
("" 
list"" 
=>"" 
list"" 
)""  
.## 
FirstOrDefault## 
(## 
a## 
=>##  
a$$ 
.$$ 
Text$$ 
.$$ 
Contains$$ 
($$  
$str$$  0
,$$0 1
StringComparison$$2 B
.$$B C
OrdinalIgnoreCase$$C T
)$$T U
&&$$V X
a%% 
.%% 
Start%% 
.%% 
Year%% 
==%% 
terminatedYear%%  .
)%%. /
;%%/ 0
var'' 
blockingList'' 
='' 
appointmentList'' *
.(( 
Select(( 
((( 
list(( 
=>(( 
list((  
)((  !
.)) 
Where)) 
()) 
a)) 
=>)) 
a** 
!=** 
null** 
&&** 
!++ 
string++ 
.++ 
IsNullOrEmpty++ %
(++% &
a++& '
.++' (
Text++( ,
)++, -
&&++. 0
a,, 
.,, 
Start,, 
!=,, 
default,, "
&&,,# %
vacationSummer-- 
!=-- !
null--" &
&&--' )
vacationSummer.. 
... 
Start.. $
!=..% '
default..( /
&&..0 2
(// 
a00 
.00 
Text00 
.00 
Contains00 #
(00# $
$str00$ -
,00- .
StringComparison00/ ?
.00? @
OrdinalIgnoreCase00@ Q
)00Q R
||00S U
a11 
.11 
Text11 
.11 
Contains11 #
(11# $
$str11$ *
,11* +
StringComparison11, <
.11< =
OrdinalIgnoreCase11= N
)11N O
||11P R
a22 
.22 
Text22 
.22 
Contains22 #
(22# $
$str22$ +
,22+ ,
StringComparison22- =
.22= >
OrdinalIgnoreCase22> O
)22O P
)33 
&&33 
a44 
.44 
Start44 
<44 
vacationSummer44 (
.44( )
Start44) .
)44. /
.55 
ToList55 
(55 
)55 
;55 
var77  
appointmentListFinal77  
=77! "
await77# (&
GenerateWeeklyMondaysAsync77) C
(77C D
userId77D J
,77J K
appointment77L W
,77W X
vacationSummer77Y g
,77g h
blockingList77i u
)77u v
;77v w
return99  
appointmentListFinal99 #
;99# $
}:: 
public<< 

async<< 
Task<< 
<<< 
List<< 
<<< 
Appointment<< &
><<& '
><<' (&
GenerateWeeklyMondaysAsync<<) C
(<<C D
string== 
userId== 
,== 
Appointment>> 
	prototype>> 
,>> 
Appointment?? 
vacationSummer?? "
,??" #
List@@ 
<@@ 
Appointment@@ 
>@@ 
blockingList@@ &
)@@& '
{AA 
varBB 
resultsBB 
=BB 
newBB 
ListBB 
<BB 
AppointmentBB *
>BB* +
(BB+ ,
)BB, -
;BB- .
varEE 
	targetDowEE 
=EE 
	prototypeEE !
.EE! "
StartEE" '
.EE' (
	DayOfWeekEE( 1
;EE1 2
varHH 
firstHH 
=HH 
	prototypeHH 
.HH 
StartHH #
;HH# $
intII 
	daysToAddII 
=II 
(II 
(II 
intII 
)II 
	targetDowII '
-II( )
(II* +
intII+ .
)II. /
firstII/ 4
.II4 5
	DayOfWeekII5 >
+II? @
$numIIA B
)IIB C
%IID E
$numIIF G
;IIG H
varJJ 

occurrenceJJ 
=JJ 
firstJJ 
.JJ 
AddDaysJJ &
(JJ& '
	daysToAddJJ' 0
)JJ0 1
;JJ1 2
whileMM 
(MM 

occurrenceMM 
<MM 
vacationSummerMM *
.MM* +
StartMM+ 0
)MM0 1
{NN 	
boolPP 
	isBlockedPP 
=PP 
blockingListPP )
.PP) *
AnyPP* -
(PP- .
bPP. /
=>PP0 2

occurrenceQQ 
>=QQ 
bQQ 
.QQ  
StartQQ  %
&&QQ& (

occurrenceRR 
<=RR 
bRR  
.RR  !
EndRR! $
)VV 
;VV 
ifXX 
(XX 
!XX 
	isBlockedXX 
)XX 
{YY 
var[[ 
appt[[ 
=[[ 
new[[ 
Appointment[[ *
{\\ 
Id]] 
=]]! "
Guid]]# '
.]]' (
NewGuid]]( /
(]]/ 0
)]]0 1
.]]1 2
ToString]]2 :
(]]: ;
)]]; <
,]]< =
Start^^ 
=^^! "

occurrence^^# -
,^^- .
End__ 
=__! "

occurrence__# -
+__. /
(__0 1
	prototype__1 :
.__: ;
End__; >
-__? @
	prototype__A J
.__J K
Start__K P
)__P Q
,__Q R
Text`` 
=``! "
	prototype``# ,
.``, -
Text``- 1
,``1 2
Coloraa 
=aa! "
	prototypeaa# ,
.aa, -
Coloraa- 2
,aa2 3
	Recurringbb 
=bb! "
truebb# '
,bb' (
IdRecurringcc 
=cc! "
	prototypecc# ,
.cc, -
IdRecurringcc- 8
}dd 
;dd 
resultsgg 
.gg 
Addgg 
(gg 
apptgg  
)gg  !
;gg! "
}hh 

occurrencekk 
=kk 

occurrencekk #
.kk# $
AddDayskk$ +
(kk+ ,
$numkk, -
)kk- .
;kk. /
}ll 	
returnnn 
resultsnn 
;nn 
}oo 
}pp ¥ß
ZC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Repositories\SchedulerRepository.cs
	namespace		 	
Service		
 
.		 
Database		 
.		 
Repositories		 '
;		' (
public 
class 
SchedulerRepository  
:! " 
ISchedulerRepository# 7
{ 
private 
readonly 
MongoDbContext #
_mongoDbContext$ 3
;3 4
private 
readonly  
BlockVacationService )!
_blockVacationService* ?
;? @
public 

SchedulerRepository 
( 
MongoDbContext 
mongoDbContext %
,% & 
BlockVacationService  
blockVacationService 1
)1 2
{ 
_mongoDbContext 
= 
mongoDbContext (
;( )!
_blockVacationService 
=  
blockVacationService  4
;4 5
} 
public 

async 
Task 
< 
	Scheduler 
>  
GetScheduler! -
(- .
string. 4
userId5 ;
); <
{ 
var 
existingScheduler 
= 
await  %
_mongoDbContext& 5
.5 6

Schedulers6 @
. 
Find 
( 
Builders 
< 
	Scheduler $
>$ %
.% &
Filter& ,
., -
Eq- /
(/ 0
x 
=> 
x 
. 
IdUser 
, 
userId %
)% &
)& '
. 
FirstOrDefaultAsync  
(  !
)! "
;" #
if 

( 
existingScheduler 
!=  
null! %
)% &
return' -
existingScheduler. ?
;? @
return!! 
null!! 
;!! 
}"" 
public$$ 

async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
	Scheduler$$ $
>$$$ %
>$$% &
GetManyScheduler$$' 7
($$7 8
List$$8 <
<$$< =
string$$= C
>$$C D
idsProfesseur$$E R
)$$R S
{%% 
var&& 

schedulers&& 
=&& 
await&& 
_mongoDbContext&& .
.&&. /

Schedulers&&/ 9
.'' 
Find'' 
('' 
Builders(( 
<(( 
	Scheduler(( "
>((" #
.((# $
Filter(($ *
.((* +
In((+ -
(((- .
s((. /
=>((0 2
s((3 4
.((4 5
IdUser((5 ;
,((; <
idsProfesseur((= J
)((J K
)((K L
.)) 
ToListAsync)) 
()) 
))) 
;)) 
if++ 

(++ 

schedulers++ 
.++ 
Count++ 
==++ 
$num++  !
)++! "
return++# )
null++* .
;++. /
return-- 

schedulers-- 
;-- 
}.. 
public00 

async00 
Task00 
<00 
	Scheduler00 
>00  
AddScheduler00! -
(00- .
string00. 4
userId005 ;
)00; <
{11 
var22 
newScheduler22 
=22 
new22 
	Scheduler22 (
{33 	
IdScheduler44 
=44 
ObjectId44 "
.44" #
GenerateNewId44# 0
(440 1
)441 2
.442 3
ToString443 ;
(44; <
)44< =
,44= >
IdUser55 
=55 
userId55 
,55 
Appointments66 
=66 
new66 
List66 #
<66# $
Appointment66$ /
>66/ 0
(660 1
)661 2
,662 3
	CreatedAt77 
=77 
DateTime77  
.77  !
UtcNow77! '
,77' (
	UpdatedAt88 
=88 
DateTime88  
.88  !
UtcNow88! '
}99 	
;99	 

await;; 
_mongoDbContext;; 
.;; 

Schedulers;; (
.;;( )
InsertOneAsync;;) 7
(;;7 8
newScheduler;;8 D
);;D E
;;;E F
return== 
newScheduler== 
;== 
}>> 
public@@ 

async@@ 
Task@@ 
<@@ 
	Scheduler@@ 
>@@  
DeleteScheduler@@! 0
(@@0 1
string@@1 7
userId@@8 >
)@@> ?
{AA 
varBB 
deletedSchedulerBB 
=BB 
awaitBB $
_mongoDbContextBB% 4
.BB4 5

SchedulersBB5 ?
.BB? @!
FindOneAndDeleteAsyncBB@ U
(BBU V
BuildersCC 
<CC 
	SchedulerCC 
>CC 
.CC  
FilterCC  &
.CC& '
EqCC' )
(CC) *
sCC* +
=>CC, .
sCC/ 0
.CC0 1
IdUserCC1 7
,CC7 8
userIdCC9 ?
)CC? @
)CC@ A
;CCA B
returnEE 
deletedSchedulerEE 
;EE  
}FF 
publicHH 

asyncHH 
TaskHH 
<HH 
AppointmentHH !
>HH! "
GetOneAppointmentHH# 4
(HH4 5
stringHH5 ;
userIdHH< B
,HHB C
AppointmentHHD O
appointmentHHP [
)HH[ \
{II 
varJJ 
existingSchedulerJJ 
=JJ 
awaitJJ! &
_mongoDbContextJJ' 6
.JJ6 7

SchedulersJJ7 A
.KK 
FindKK 
(KK 
sKK 
=>KK 
sKK 
.KK 
IdUserKK 
==KK  "
userIdKK# )
)KK) *
.LL 
FirstOrDefaultAsyncLL  
(LL  !
)LL! "
;LL" #
ifNN 

(NN 
existingSchedulerNN 
==NN  
nullNN! %
)NN% &
returnNN' -
nullNN. 2
;NN2 3
varPP 
existingAppointmentPP 
=PP  !
existingSchedulerPP" 3
.PP3 4
AppointmentsPP4 @
.QQ 
FirstOrDefaultQQ 
(QQ 
aQQ 
=>QQ  
aQQ! "
.QQ" #
StartQQ# (
==QQ) +
appointmentQQ, 7
.QQ7 8
StartQQ8 =
&&QQ> @
aQQA B
.QQB C
EndQQC F
==QQG I
appointmentQQJ U
.QQU V
EndQQV Y
)QQY Z
;QQZ [
ifSS 

(SS 
existingAppointmentSS 
==SS  "
nullSS# '
)SS' (
returnSS) /
nullSS0 4
;SS4 5
returnUU 
existingAppointmentUU "
;UU" #
}VV 
publicXX 

asyncXX 
TaskXX 
<XX 
AppointmentXX !
>XX! "!
GetOneAppointmentByIdXX# 8
(XX8 9
stringXX9 ?
userIdXX@ F
,XXF G
AppointmentXXH S
appointmentXXT _
)XX_ `
{YY 
varZZ 
existingSchedulerZZ 
=ZZ 
awaitZZ! &
_mongoDbContextZZ' 6
.ZZ6 7

SchedulersZZ7 A
.[[ 
Find[[ 
([[ 
s[[ 
=>[[ 
s[[ 
.[[ 
IdUser[[ 
==[[  "
userId[[# )
)[[) *
.\\ 
FirstOrDefaultAsync\\  
(\\  !
)\\! "
;\\" #
if^^ 

(^^ 
existingScheduler^^ 
==^^  
null^^! %
)^^% &
return^^' -
null^^. 2
;^^2 3
var`` 
existingAppointment`` 
=``  !
existingScheduler``" 3
.``3 4
Appointments``4 @
.aa 
FirstOrDefaultaa 
(aa 
aaa 
=>aa  
aaa! "
.aa" #
Idaa# %
==aa& (
appointmentaa) 4
.aa4 5
Idaa5 7
)aa7 8
;aa8 9
ifcc 

(cc 
existingAppointmentcc 
==cc  "
nullcc# '
)cc' (
returncc) /
nullcc0 4
;cc4 5
returnee 
existingAppointmentee "
;ee" #
}ff 
publichh 

asynchh 
Taskhh 
<hh 
Listhh 
<hh 
Appointmenthh &
>hh& '
>hh' (
AddAppointmenthh) 7
(hh7 8
stringhh8 >
userIdhh? E
,hhE F
AppointmenthhG R
appointmenthhS ^
)hh^ _
{ii 
varjj 
updatedSchedulerjj 
=jj 
awaitjj $
_mongoDbContextjj% 4
.jj4 5

Schedulersjj5 ?
.kk !
FindOneAndUpdateAsynckk "
(kk" #
Buildersll 
<ll 
	Schedulerll "
>ll" #
.ll# $
Filterll$ *
.ll* +
Eqll+ -
(ll- .
sll. /
=>ll0 2
sll3 4
.ll4 5
IdUserll5 ;
,ll; <
userIdll= C
)llC D
,llD E
Buildersmm 
<mm 
	Schedulermm "
>mm" #
.mm# $
Updatemm$ *
.mm* +
Pushmm+ /
(mm/ 0
smm0 1
=>mm2 4
smm5 6
.mm6 7
Appointmentsmm7 C
,mmC D
appointmentmmE P
)mmP Q
,mmQ R
newnn #
FindOneAndUpdateOptionsnn +
<nn+ ,
	Schedulernn, 5
>nn5 6
{oo 
ReturnDocumentpp "
=pp# $
ReturnDocumentpp% 3
.pp3 4
Afterpp4 9
}qq 
)qq 
;qq 
ifss 

(ss 
updatedSchedulerss 
!=ss 
nullss  $
)ss$ %
returnss& ,
updatedSchedulerss- =
.ss= >
Appointmentsss> J
;ssJ K
returnuu 
nulluu 
;uu 
}vv 
publicxx 

asyncxx 
Taskxx 
<xx 
	Schedulerxx 
>xx  
AddListAppointmentxx! 3
(xx3 4
stringxx4 :
userIdxx; A
,xxA B
ListxxC G
<xxG H
AppointmentxxH S
>xxS T
appointmentsxxU a
)xxa b
{yy 
var|| 
updatedScheduler|| 
=|| 
await|| $
_mongoDbContext||% 4
.||4 5

Schedulers||5 ?
.}} !
FindOneAndUpdateAsync}} "
(}}" #
Builders~~ 
<~~ 
	Scheduler~~ "
>~~" #
.~~# $
Filter~~$ *
.~~* +
Eq~~+ -
(~~- .
s~~. /
=>~~0 2
s~~3 4
.~~4 5
IdUser~~5 ;
,~~; <
userId~~= C
)~~C D
,~~D E
Builders 
< 
	Scheduler "
>" #
.# $
Update$ *
.* +
PushEach+ 3
(3 4
s4 5
=>6 8
s9 :
.: ;
Appointments; G
,G H
appointmentsI U
)U V
,V W
new
ÄÄ %
FindOneAndUpdateOptions
ÄÄ +
<
ÄÄ+ ,
	Scheduler
ÄÄ, 5
>
ÄÄ5 6
{
ÅÅ 
ReturnDocument
ÇÇ "
=
ÇÇ# $
ReturnDocument
ÇÇ% 3
.
ÇÇ3 4
After
ÇÇ4 9
}
ÉÉ 
)
ÉÉ 
;
ÉÉ 
return
ÖÖ 
updatedScheduler
ÖÖ 
;
ÖÖ  
}
ÜÜ 
public
àà 

async
àà 
Task
àà 
<
àà 
List
àà 
<
àà 
Appointment
àà &
>
àà& '
>
àà' (
UpdateAppointment
àà) :
(
àà: ;
string
àà; A
userId
ààB H
,
ààH I
Appointment
ààJ U
appointment
ààV a
)
ààa b
{
ââ 
var
ää 
updatedScheduler
ää 
=
ää 
await
ää $
_mongoDbContext
ää% 4
.
ää4 5

Schedulers
ää5 ?
.
ãã #
FindOneAndUpdateAsync
ãã "
(
ãã" #
Builders
åå 
<
åå 
	Scheduler
åå "
>
åå" #
.
åå# $
Filter
åå$ *
.
åå* +
And
åå+ .
(
åå. /
Builders
çç 
<
çç 
	Scheduler
çç 
>
çç  
.
çç  !
Filter
çç! '
.
çç' (
Eq
çç( *
(
çç* +
s
éé 
=>
éé 
s
éé 
.
éé 
IdUser
éé %
,
éé% &
userId
éé' -
)
éé- .
,
éé. /
Builders
èè  
<
èè  !
	Scheduler
èè! *
>
èè* +
.
èè+ ,
Filter
èè, 2
.
èè2 3
	ElemMatch
èè3 <
(
èè< =
s
êê 
=>
êê 
s
êê 
.
êê 
Appointments
êê +
,
êê+ ,
a
êê- .
=>
êê/ 1
a
êê2 3
.
êê3 4
Id
êê4 6
==
êê7 9
appointment
êê: E
.
êêE F
Id
êêF H
)
êêH I
)
êêI J
,
êêJ K
Builders
ëë 
<
ëë 
	Scheduler
ëë "
>
ëë" #
.
ëë# $
Update
ëë$ *
.
íí 
Set
íí 
(
íí 
$str
íí ,
,
íí, -
appointment
íí. 9
.
íí9 :
Id
íí: <
)
íí< =
.
ìì 
Set
ìì 
(
ìì 
$str
ìì /
,
ìì/ 0
appointment
ìì1 <
.
ìì< =
Start
ìì= B
)
ììB C
.
îî 
Set
îî 
(
îî 
$str
îî -
,
îî- .
appointment
îî/ :
.
îî: ;
End
îî; >
)
îî> ?
.
ïï 
Set
ïï 
(
ïï 
$str
ïï .
,
ïï. /
appointment
ïï0 ;
.
ïï; <
Text
ïï< @
)
ïï@ A
.
ññ 
Set
ññ 
(
ññ 
$str
ññ /
,
ññ/ 0
appointment
ññ1 <
.
ññ< =
Color
ññ= B
)
ññB C
.
óó 
Set
óó 
(
óó 
$str
óó 3
,
óó3 4
appointment
óó5 @
.
óó@ A
	Recurring
óóA J
)
óóJ K
.
òò 
Set
òò 
(
òò 
$str
òò 5
,
òò5 6
appointment
òò7 B
.
òòB C
IdRecurring
òòC N
)
òòN O
,
òòO P
new
öö %
FindOneAndUpdateOptions
öö +
<
öö+ ,
	Scheduler
öö, 5
>
öö5 6
{
õõ 
ReturnDocument
úú "
=
úú# $
ReturnDocument
úú% 3
.
úú3 4
After
úú4 9
}
ùù 
)
ûû 
;
ûû 
if
°° 

(
°° 
updatedScheduler
°° 
==
°° 
null
°°  $
)
°°$ %
return
°°& ,
null
°°- 1
;
°°1 2
return
££ 
updatedScheduler
££ 
.
££  
Appointments
££  ,
;
££, -
}
•• 
public
ßß 

async
ßß 
Task
ßß 
<
ßß 
List
ßß 
<
ßß 
Appointment
ßß &
>
ßß& '
>
ßß' (
DeleteAppointment
ßß) :
(
ßß; <
string
ßß< B
userId
ßßC I
,
ßßI J
Appointment
ßßK V
appointment
ßßW b
)
ßßb c
{
®® 
var
©© 
deletedScheduler
©© 
=
©© 
await
©© $
_mongoDbContext
©©% 4
.
©©4 5

Schedulers
©©5 ?
.
™™ #
FindOneAndUpdateAsync
™™ "
(
™™" #
Builders
´´ 
<
´´ 
	Scheduler
´´ "
>
´´" #
.
´´# $
Filter
´´$ *
.
´´* +
Eq
´´+ -
(
´´- .
s
´´. /
=>
´´0 2
s
´´3 4
.
´´4 5
IdUser
´´5 ;
,
´´; <
userId
´´= C
)
´´C D
,
´´D E
Builders
¨¨ 
<
¨¨ 
	Scheduler
¨¨ "
>
¨¨" #
.
¨¨# $
Update
¨¨$ *
.
¨¨* +

PullFilter
¨¨+ 5
(
¨¨5 6
s
≠≠ 
=>
≠≠ 
s
≠≠ 
.
≠≠ 
Appointments
≠≠ '
,
≠≠' (
a
ÆÆ 
=>
ÆÆ 
a
ÆÆ 
.
ÆÆ 
Id
ÆÆ 
==
ÆÆ  
appointment
ÆÆ! ,
.
ÆÆ, -
Id
ÆÆ- /
)
ÆÆ/ 0
,
ÆÆ0 1
new
ØØ %
FindOneAndUpdateOptions
ØØ +
<
ØØ+ ,
	Scheduler
ØØ, 5
>
ØØ5 6
{
∞∞ 
ReturnDocument
±± "
=
±±# $
ReturnDocument
±±% 3
.
±±3 4
After
±±4 9
}
≤≤ 
)
≤≤ 
;
≤≤ 
if
¥¥ 

(
¥¥ 
deletedScheduler
¥¥ 
!=
¥¥ 
null
¥¥  $
)
¥¥$ %
return
¥¥& ,
deletedScheduler
¥¥- =
.
¥¥= >
Appointments
¥¥> J
;
¥¥J K
return
∂∂ 
null
∂∂ 
;
∂∂ 
}
∑∑ 
public
ππ 

async
ππ 
Task
ππ 
<
ππ 
List
ππ 
<
ππ 
Appointment
ππ &
>
ππ& '
>
ππ' (#
DeleteListAppointment
ππ) >
(
ππ> ?
string
ππ? E
idUser
ππF L
,
ππL M
string
ππN T
idRecurring
ππU `
,
ππ` a
DateTime
ππb j
	startDate
ππk t
)
ππt u
{
∫∫ 
var
ºº 
filterScheduler
ºº 
=
ºº 
Builders
ºº &
<
ºº& '
	Scheduler
ºº' 0
>
ºº0 1
.
ºº1 2
Filter
ºº2 8
.
ºº8 9
Eq
ºº9 ;
(
ºº; <
s
ºº< =
=>
ºº> @
s
ººA B
.
ººB C
IdUser
ººC I
,
ººI J
idUser
ººK Q
)
ººQ R
;
ººR S
var
ææ 
	scheduler
ææ 
=
ææ 
await
ææ 
_mongoDbContext
ææ -
.
ææ- .

Schedulers
ææ. 8
.
ææ8 9
Find
ææ9 =
(
ææ= >
filterScheduler
ææ> M
)
ææM N
.
ææN O!
FirstOrDefaultAsync
ææO b
(
ææb c
)
ææc d
;
ææd e
var
¡¡ !
deletedAppointments
¡¡ 
=
¡¡  !
	scheduler
¡¡" +
.
¡¡+ ,
Appointments
¡¡, 8
.
¬¬ 
Where
¬¬ 
(
¬¬ 
a
¬¬ 
=>
¬¬ 
a
¬¬ 
.
¬¬ 
IdRecurring
¬¬ %
==
¬¬& (
idRecurring
¬¬) 4
&&
¬¬5 7
a
¬¬8 9
.
¬¬9 :
Start
¬¬: ?
>
¬¬@ A
	startDate
¬¬B K
)
¬¬K L
.
√√ 
ToList
√√ 
(
√√ 
)
√√ 
;
√√ 
await
≈≈ 
_mongoDbContext
≈≈ 
.
≈≈ 

Schedulers
≈≈ (
.
≈≈( )
UpdateOneAsync
≈≈) 7
(
≈≈7 8
filterScheduler
∆∆ 
,
∆∆ 
Builders
«« 
<
«« 
	Scheduler
«« 
>
«« 
.
««  
Update
««  &
.
««& '

PullFilter
««' 1
(
««1 2
s
««2 3
=>
««4 6
s
««7 8
.
««8 9
Appointments
««9 E
,
««E F
a
»» 
=>
»» 
a
»» 
.
»» 
IdRecurring
»» "
==
»»# %
idRecurring
»»& 1
&&
»»2 4
a
»»5 6
.
»»6 7
Start
»»7 <
>
»»= >
	startDate
»»? H
)
»»H I
)
»»I J
;
»»J K
return
   !
deletedAppointments
   "
;
  " #
}
ÀÀ 
public
ÕÕ 

async
ÕÕ 
Task
ÕÕ 
<
ÕÕ 
List
ÕÕ 
<
ÕÕ 
Appointment
ÕÕ &
>
ÕÕ& '
>
ÕÕ' (
GetBlockVacation
ÕÕ) 9
(
ÕÕ9 :
string
ÕÕ: @
userId
ÕÕA G
,
ÕÕG H
Appointment
ÕÕI T
appointment
ÕÕU `
)
ÕÕ` a
{
ŒŒ 
var
œœ 
appointmentList
œœ 
=
œœ 
await
œœ ##
_blockVacationService
œœ$ 9
.
œœ9 :+
GetAppointmentWithoutVacation
œœ: W
(
œœW X
userId
œœX ^
,
œœ^ _
appointment
œœ` k
)
œœk l
;
œœl m
var
““ 
newScheduler
““ 
=
““ 
await
““   
AddListAppointment
““! 3
(
““3 4
userId
““4 :
,
““: ;
appointmentList
““< K
)
““K L
;
““L M
if
‘‘ 

(
‘‘ 
newScheduler
‘‘ 
==
‘‘ 
null
‘‘  
)
‘‘  !
return
‘‘" (
null
‘‘) -
;
‘‘- .
return
÷÷ 
newScheduler
÷÷ 
.
÷÷ 
Appointments
÷÷ (
;
÷÷( )
}
ÿÿ 
}ŸŸ ‹À
WC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Repositories\LessonRepository.cs
	namespace

 	
Service


 
.

 
Database

 
.

 
Repositories

 '
;

' (
public 
class 
LessonRepository 
: 
ILessonRepository  1
{ 
private 
readonly 
MongoDbContext #
_mongoDbContext$ 3
;3 4
private 
readonly 
ILogger 
< 
LessonRepository -
>- .
_logger/ 6
;6 7
public 

LessonRepository 
( 
MongoDbContext *
mongoDbContext+ 9
,9 :
ILogger; B
<B C
LessonRepositoryC S
>S T
loggerU [
)[ \
{ 
_mongoDbContext 
= 
mongoDbContext (
;( )
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
Lesson 
> 
	GetLesson '
(' (
string( .
idAppointment/ <
,< =
string> D
idUserE K
)K L
{ 
var 
existingLessonBook 
=  
await! &
_mongoDbContext' 6
.6 7
LessonBooks7 B
. 
Find 
( 
l 
=> 
l 
. 
IdUser 
==  "
idUser# )
)) *
. 
FirstOrDefaultAsync  
(  !
)! "
;" #
if 

( 
existingLessonBook 
== !
null" &
)& '
return( .
null/ 3
;3 4
var 
existingLesson 
= 
existingLessonBook /
./ 0
Lessons0 7
.   
FirstOrDefault   
(   
l   
=>    
l  ! "
.  " #
IdAppointment  # 0
==  1 3
idAppointment  4 A
)  A B
;  B C
if"" 

("" 
existingLesson"" 
=="" 
null"" "
)""" #
return""$ *
null""+ /
;""/ 0
return$$ 
existingLesson$$ 
;$$ 
}%% 
public'' 

async'' 
Task'' 
<'' 
Lesson'' 
>'' 
	AddLesson'' '
(''' (
Lesson''( .
lesson''/ 5
,''5 6
string''7 =
idUser''> D
)''D E
{(( 
lesson)) 
.)) 
IdLesson)) 
=)) 
ObjectId)) "
.))" #
GenerateNewId))# 0
())0 1
)))1 2
.))2 3
ToString))3 ;
()); <
)))< =
;))= >
lesson** 
.** 
	CreatedAt** 
=** 
DateTime** #
.**# $
Now**$ '
;**' (
lesson++ 
.++ 
	UpdatedAt++ 
=++ 
DateTime++ #
.++# $
Now++$ '
;++' (
var-- 
existingLessonBook-- 
=--  
await--! &
_mongoDbContext--' 6
.--6 7
LessonBooks--7 B
... 
Find.. 
(.. 
l.. 
=>.. 
l.. 
... 
IdUser.. 
==..  "
idUser..# )
)..) *
.// 
FirstOrDefaultAsync//  
(//  !
)//! "
;//" #
if11 

(11 
existingLessonBook11 
==11 !
null11" &
)11& '
return11( .
null11/ 3
;113 4
existingLessonBook33 
.33 
Lessons33 "
.33" #
Add33# &
(33& '
lesson33' -
)33- .
;33. /
var55 
result55 
=55 
await55 
_mongoDbContext55 *
.55* +
LessonBooks55+ 6
.66 
UpdateOneAsync66 
(66 
l66 
=>66  
l66! "
.66" #
IdUser66# )
==66* ,
idUser66- 3
,663 4
Builders77 
<77 

LessonBook77 #
>77# $
.77$ %
Update77% +
.77+ ,
Set77, /
(77/ 0
l88 
=>88 
l88 
.88 
Lessons88 "
,88" #
existingLessonBook88$ 6
.886 7
Lessons887 >
)88> ?
)88? @
;88@ A
if:: 

(:: 
result:: 
.:: 
ModifiedCount::  
==::! #
$num::$ %
)::% &
return::' -
null::. 2
;::2 3
return<< 
lesson<< 
;<< 
}== 
public?? 

async?? 
Task?? 
<?? 
Lesson?? 
>?? 
UpdateLesson?? *
(??* +
Lesson??+ 1
lesson??2 8
,??8 9
string??: @
idUser??A G
)??G H
{@@ 
varCC 
	lessonLogCC 
=CC 
JsonSerializerCC &
.CC& '
	SerializeCC' 0
(CC0 1
lessonCC1 7
,CC7 8
newCC9 <!
JsonSerializerOptionsCC= R
{DD 	
WriteIndentedEE
 
=EE 
trueEE 
,EE 
IgnoreNullValuesFF
 
=FF 
falseFF "
}GG 	
)GG	 

;GG
 
ConsoleHH 
.HH 
	WriteLineHH 
(HH 
$strHH A
+HHB C
	lessonLogHHD M
)HHM N
;HHN O
varKK 
existingLessonBookKK 
=KK  
awaitKK! &
_mongoDbContextKK' 6
.KK6 7
LessonBooksKK7 B
.LL 
FindLL 
(LL 
lLL 
=>LL 
lLL 
.LL 
IdUserLL 
==LL  "
idUserLL# )
)LL) *
.MM 
FirstOrDefaultAsyncMM  
(MM  !
)MM! "
;MM" #
ifOO 

(OO 
existingLessonBookOO 
==OO !
nullOO" &
)OO& '
returnOO( .
nullOO/ 3
;OO3 4
varQQ 
existingLessonIndexQQ 
=QQ  !
existingLessonBookQQ" 4
.QQ4 5
LessonsQQ5 <
.RR 
	FindIndexRR 
(RR 
lRR 
=>RR 
lRR 
.RR 
IdLessonRR &
==RR' )
lessonRR* 0
.RR0 1
IdLessonRR1 9
)RR9 :
;RR: ;
ifTT 

(TT 
existingLessonIndexTT 
==TT  "
-TT# $
$numTT$ %
)TT% &
returnTT' -
nullTT. 2
;TT2 3
lessonVV 
.VV 
	UpdatedAtVV 
=VV 
DateTimeVV #
.VV# $
NowVV$ '
;VV' (
existingLessonBookXX 
.XX 
LessonsXX "
[XX" #
existingLessonIndexXX# 6
]XX6 7
=XX8 9
lessonXX: @
;XX@ A
tryZZ 
{[[ 	
var\\
 
result\\ 
=\\ 
await\\ 
_mongoDbContext\\ ,
.\\, -
LessonBooks\\- 8
.]] 
UpdateOneAsync]] )
(]]) *
l]]* +
=>]], .
l]]/ 0
.]]0 1
IdUser]]1 7
==]]8 :
idUser]]; A
,]]A B
Builders^^ &
<^^& '

LessonBook^^' 1
>^^1 2
.^^2 3
Update^^3 9
.^^9 :
Set^^: =
(^^= >
l__" #
=>__$ &
l__' (
.__( )
Lessons__) 0
,__0 1
existingLessonBook__2 D
.__D E
Lessons__E L
)__L M
)__M N
;__N O
ifaa 
(aa 
resultaa 
.aa 
ModifiedCountaa $
==aa% '
$numaa( )
)aa) *
returnaa+ 1
nullaa2 6
;aa6 7
returncc 
lessoncc 
;cc 
}dd 	
catchee 
(ee 
	Exceptionee 
eee 
)ee 
{ff 	
Consolegg 
.gg 
	WriteLinegg 
(gg 
egg 
)gg  
;gg  !
throwhh 
;hh 
}ii 	
}jj 
publicll 

asyncll 
Taskll 
<ll 
Lessonll 
>ll 
DeleteLessonll *
(ll* +
Lessonll+ 1
lessonll2 8
,ll8 9
stringll: @
idUserllA G
)llG H
{mm 
varnn 
resultnn 
=nn 
awaitnn 
_mongoDbContextnn *
.nn* +
LessonBooksnn+ 6
.oo 
UpdateOneAsyncoo 
(oo 
Builderspp 
<pp 

LessonBookpp #
>pp# $
.pp$ %
Filterpp% +
.pp+ ,
Eqpp, .
(pp. /
lpp/ 0
=>pp1 3
lpp4 5
.pp5 6
IdUserpp6 <
,pp< =
idUserpp> D
)ppD E
,ppE F
Buildersqq 
<qq 

LessonBookqq #
>qq# $
.qq$ %
Updateqq% +
.qq+ ,

PullFilterqq, 6
(qq6 7
lqq7 8
=>qq9 ;
lqq< =
.qq= >
Lessonsqq> E
,qqE F
Buildersrr 
<rr 
Lessonrr #
>rr# $
.rr$ %
Filterrr% +
.rr+ ,
Eqrr, .
(rr. /
lrr/ 0
=>rr1 3
lrr4 5
.rr5 6
IdLessonrr6 >
,rr> ?
lessonrr@ F
.rrF G
IdLessonrrG O
)rrO P
)rrP Q
)rrQ R
;rrR S
iftt 

(tt 
resulttt 
.tt 
ModifiedCounttt  
==tt! #
$numtt$ %
)tt% &
returntt' -
nulltt. 2
;tt2 3
returnvv 
lessonvv 
;vv 
}ww 
publicyy 

asyncyy 
Taskyy 
<yy 
Documentyy 
>yy 
GetDocumentInLessonyy  3
(yy3 4
RequestLessonyy4 A
requestyyB I
,yyI J
stringyyK Q
idUseryyR X
)yyX Y
{zz 
var{{ 
existingLessonBook{{ 
={{  
await{{! &
_mongoDbContext{{' 6
.{{6 7
LessonBooks{{7 B
.|| 
Find|| 
(|| 
Builders|| 
<|| 

LessonBook|| %
>||% &
.||& '
Filter||' -
.||- .
Eq||. 0
(||0 1
l}} 
=>}} 
l}} 
.}} 
IdUser}} 
,}} 
idUser}} %
)}}% &
)}}& '
.~~ 
FirstOrDefaultAsync~~  
(~~  !
)~~! "
;~~" #
if
ÄÄ 

(
ÄÄ  
existingLessonBook
ÄÄ 
==
ÄÄ !
null
ÄÄ" &
)
ÄÄ& '
return
ÄÄ( .
null
ÄÄ/ 3
;
ÄÄ3 4
var
ÇÇ 
existingDocument
ÇÇ 
=
ÇÇ  
existingLessonBook
ÇÇ 1
.
ÇÇ1 2
Lessons
ÇÇ2 9
.
ÉÉ 
FirstOrDefault
ÉÉ 
(
ÉÉ 
l
ÉÉ 
=>
ÉÉ  
l
ÉÉ! "
.
ÉÉ" #
IdLesson
ÉÉ# +
==
ÉÉ, .
request
ÉÉ/ 6
.
ÉÉ6 7
Lesson
ÉÉ7 =
.
ÉÉ= >
IdLesson
ÉÉ> F
)
ÉÉF G
.
ÑÑ 
	Documents
ÑÑ 
.
ÑÑ 
FirstOrDefault
ÑÑ %
(
ÑÑ% &
d
ÑÑ& '
=>
ÑÑ( *
d
ÑÑ+ ,
.
ÑÑ, -

IdDocument
ÑÑ- 7
==
ÑÑ8 :
request
ÑÑ; B
.
ÑÑB C
Document
ÑÑC K
.
ÑÑK L

IdDocument
ÑÑL V
)
ÑÑV W
;
ÑÑW X
if
ÜÜ 

(
ÜÜ 
existingDocument
ÜÜ 
==
ÜÜ 
null
ÜÜ  $
)
ÜÜ$ %
return
ÜÜ& ,
null
ÜÜ- 1
;
ÜÜ1 2
return
àà 
existingDocument
àà 
;
àà  
}
ââ 
public
ãã 

async
ãã 
Task
ãã 
<
ãã 
UpdateResult
ãã "
>
ãã" #$
DeleteDocumentInLesson
ãã$ :
(
ãã: ;
string
ãã; A
idUser
ããB H
,
ããH I
string
ããJ P
idLesson
ããQ Y
,
ããY Z
string
ãã[ a

IdDocument
ããb l
)
ããl m
{
åå 
var
çç 
result
çç 
=
çç 
await
çç 
_mongoDbContext
çç *
.
çç* +
LessonBooks
çç+ 6
.
çç6 7
UpdateOneAsync
çç7 E
(
ççE F
Builders
éé 
<
éé 

LessonBook
éé 
>
éé  
.
éé  !
Filter
éé! '
.
éé' (
And
éé( +
(
éé+ ,
Builders
èè 
<
èè 

LessonBook
èè #
>
èè# $
.
èè$ %
Filter
èè% +
.
èè+ ,
Eq
èè, .
(
èè. /
lb
èè/ 1
=>
èè2 4
lb
èè5 7
.
èè7 8
IdUser
èè8 >
,
èè> ?
idUser
èè@ F
)
èèF G
,
èèG H
Builders
êê 
<
êê 

LessonBook
êê #
>
êê# $
.
êê$ %
Filter
êê% +
.
êê+ ,
	ElemMatch
êê, 5
(
êê5 6
lb
êê6 8
=>
êê9 ;
lb
êê< >
.
êê> ?
Lessons
êê? F
,
êêF G
l
êêH I
=>
êêJ L
l
êêM N
.
êêN O
IdLesson
êêO W
==
êêX Z
idLesson
êê[ c
)
êêc d
)
êêd e
,
êêe f
Builders
ëë 
<
ëë 

LessonBook
ëë 
>
ëë  
.
ëë  !
Update
ëë! '
.
ëë' (

PullFilter
ëë( 2
(
ëë2 3
$str
íí %
,
íí% &
Builders
íí' /
<
íí/ 0
Document
íí0 8
>
íí8 9
.
íí9 :
Filter
íí: @
.
íí@ A
Eq
ííA C
(
ííC D
d
ìì 
=>
ìì 
d
ìì 
.
ìì 

IdDocument
ìì %
,
ìì% &

IdDocument
ìì' 1
)
ìì1 2
)
ìì2 3
)
ìì3 4
;
ìì4 5
if
ïï 

(
ïï 
result
ïï 
.
ïï 
ModifiedCount
ïï  
==
ïï! #
$num
ïï$ %
)
ïï% &
return
ïï' -
null
ïï. 2
;
ïï2 3
return
óó 
result
óó 
;
óó 
}
òò 
public
öö 

async
öö 
Task
öö 
<
öö 
Document
öö 
>
öö 
GetDocument
öö  +
(
öö+ ,
string
öö, 2

idDocument
öö3 =
,
öö= >
string
öö? E
idUser
ööF L
)
ööL M
{
õõ 
Document
úú 
doc
úú 
=
úú 
null
úú 
;
úú 
var
ûû 
userLessons
ûû 
=
ûû 
await
ûû 
_mongoDbContext
ûû  /
.
ûû/ 0
LessonBooks
ûû0 ;
.
üü 
Find
üü 
(
üü 
lb
üü 
=>
üü 
lb
üü 
.
üü 
IdUser
üü !
==
üü" $
idUser
üü% +
)
üü+ ,
.
†† !
FirstOrDefaultAsync
††  
(
††  !
)
††! "
;
††" #
foreach
¢¢ 
(
¢¢ 
var
¢¢ 
lesson
¢¢ 
in
¢¢ 
userLessons
¢¢ *
.
¢¢* +
Lessons
¢¢+ 2
)
¢¢2 3
{
££ 	
doc
§§ 
=
§§ 
lesson
§§ 
.
§§ 
	Documents
§§ "
.
•• 
FirstOrDefault
•• 
(
••  
d
••  !
=>
••" $
d
••% &
.
••& '

IdDocument
••' 1
==
••2 4

idDocument
••5 ?
)
••? @
;
••@ A
foreach
ßß 
(
ßß 
var
ßß 
d
ßß 
in
ßß 
lesson
ßß $
.
ßß$ %
	Documents
ßß% .
)
ßß. /
{
®® 
_logger
©© 
.
©© 
LogInformation
©© &
(
©©& '
$str
©©' t
,
©©t u
d
©©v w
.
©©w x

IdDocument©©x Ç
,©©Ç É

idDocument©©Ñ é
)©©é è
;©©è ê
if
´´ 
(
´´ 
d
´´ 
.
´´ 

IdDocument
´´  
==
´´! #

idDocument
´´$ .
)
´´. /
{
¨¨ 
return
≠≠ 
d
≠≠ 
;
≠≠ 
}
ÆÆ 
}
ØØ 
if
±± 
(
±± 
doc
±± 
!=
±± 
null
±± 
)
±± 
continue
±± %
;
±±% &
}
≤≤ 	
if
¥¥ 

(
¥¥ 
doc
¥¥ 
!=
¥¥ 
null
¥¥ 
)
¥¥ 
{
µµ 	
_logger
∂∂ 
.
∂∂ 
LogInformation
∂∂ "
(
∂∂" #
$str
∂∂# p
,
∂∂p q
doc
∂∂r u
.
∂∂u v

IdDocument∂∂v Ä
,∂∂Ä Å

idDocument∂∂Ç å
)∂∂å ç
;∂∂ç é
return
∏∏ 
doc
∏∏ 
;
∏∏ 
}
ππ 	
return
ºº 
null
ºº 
;
ºº 
}
ΩΩ 
public
øø 

async
øø 
Task
øø 
<
øø 
UpdateResult
øø "
>
øø" #$
UpdateDocumentInLesson
øø$ :
(
øø: ;
string
øø; A
idUser
øøB H
,
øøH I
string
øøJ P
idLesson
øøQ Y
,
øøY Z
Document
øø[ c
document
øød l
)
øøl m
{
¿¿ 
var
¡¡ 
doc
¡¡ 
=
¡¡ 
await
¡¡ 
_mongoDbContext
¡¡ '
.
¡¡' (
LessonBooks
¡¡( 3
.
¡¡3 4
Find
¡¡4 8
(
¡¡8 9
lb
¡¡9 ;
=>
¡¡< >
lb
¡¡? A
.
¡¡A B
IdUser
¡¡B H
==
¡¡I K
idUser
¡¡L R
)
¡¡R S
.
¡¡S T!
FirstOrDefaultAsync
¡¡T g
(
¡¡g h
)
¡¡h i
;
¡¡i j
var
√√ 
arrayFilters
√√ 
=
√√ 
new
√√ 
List
√√ #
<
√√# $#
ArrayFilterDefinition
√√$ 9
>
√√9 :
{
ƒƒ 	
new
∆∆ /
!BsonDocumentArrayFilterDefinition
∆∆ 1
<
∆∆1 2
BsonDocument
∆∆2 >
>
∆∆> ?
(
∆∆? @
new
∆∆@ C
BsonDocument
∆∆D P
(
∆∆P Q
$str
∆∆Q ]
,
∆∆] ^
new
∆∆^ a
ObjectId
∆∆b j
(
∆∆j k
idLesson
∆∆k s
)
∆∆s t
)
∆∆t u
)
∆∆u v
,
∆∆v w
new
«« /
!BsonDocumentArrayFilterDefinition
«« 1
<
««1 2
BsonDocument
««2 >
>
««> ?
(
««? @
new
««@ C
BsonDocument
««D P
(
««P Q
$str
««Q Z
,
««Z [
new
««[ ^
ObjectId
««_ g
(
««g h
document
««h p
.
««p q

IdDocument
««q {
)
««{ |
)
««| }
)
««} ~
}
»» 	
;
»»	 

var
   
result
   
=
   
await
   
_mongoDbContext
   *
.
  * +
LessonBooks
  + 6
.
ÀÀ 
UpdateOneAsync
ÀÀ 
(
ÀÀ 
Builders
ÃÃ 
<
ÃÃ 

LessonBook
ÃÃ #
>
ÃÃ# $
.
ÃÃ$ %
Filter
ÃÃ% +
.
ÃÃ+ ,
Eq
ÃÃ, .
(
ÃÃ. /
lb
ÃÃ/ 1
=>
ÃÃ2 4
lb
ÃÃ5 7
.
ÃÃ7 8
IdUser
ÃÃ8 >
,
ÃÃ> ?
idUser
ÃÃ@ F
)
ÃÃF G
,
ÃÃG H
Builders
ÕÕ 
<
ÕÕ 

LessonBook
ÕÕ #
>
ÕÕ# $
.
ÕÕ$ %
Update
ÕÕ% +
.
ŒŒ 
Set
ŒŒ 
(
ŒŒ 
$str
ŒŒ B
,
ŒŒB C
document
ŒŒD L
.
ŒŒL M
Name
ŒŒM Q
)
ŒŒQ R
.
œœ 
Set
œœ 
(
œœ 
$str
œœ J
,
œœJ K
document
œœL T
.
œœT U
IdCloudinary
œœU a
)
œœa b
,
œœb c
new
–– 
UpdateOptions
–– !
{
––" #
ArrayFilters
––$ 0
=
––1 2
arrayFilters
––3 ?
}
––@ A
)
––A B
;
––B C
if
““ 

(
““ 
result
““ 
.
““ 
ModifiedCount
““  
==
““! #
$num
““$ %
)
““% &
return
““' -
null
““. 2
;
““2 3
return
‘‘ 
result
‘‘ 
;
‘‘ 
}
’’ 
public
◊◊ 

async
◊◊ 
Task
◊◊ 
<
◊◊ 

LessonBook
◊◊  
>
◊◊  !
GetLessonBook
◊◊" /
(
◊◊/ 0
string
◊◊0 6
userId
◊◊7 =
)
◊◊= >
{
ÿÿ 
return
ŸŸ 
null
ŸŸ 
;
ŸŸ 
}
⁄⁄ 
public
‹‹ 

async
‹‹ 
Task
‹‹ 
<
‹‹ 

LessonBook
‹‹  
>
‹‹  !
AddLessonBook
‹‹" /
(
‹‹/ 0
string
‹‹0 6
userId
‹‹7 =
)
‹‹= >
{
›› 
var
ﬁﬁ 
newLessonBook
ﬁﬁ 
=
ﬁﬁ 
new
ﬁﬁ 

LessonBook
ﬁﬁ  *
{
ﬂﬂ 	
IdLessonBook
‡‡ 
=
‡‡ 
ObjectId
‡‡ #
.
‡‡# $
GenerateNewId
‡‡$ 1
(
‡‡1 2
)
‡‡2 3
.
‡‡3 4
ToString
‡‡4 <
(
‡‡< =
)
‡‡= >
,
‡‡> ?
IdUser
·· 
=
·· 
userId
·· 
,
·· 
Lessons
‚‚ 
=
‚‚ 
new
‚‚ 
List
‚‚ 
<
‚‚ 
Lesson
‚‚ %
>
‚‚% &
(
‚‚& '
)
‚‚' (
}
„„ 	
;
„„	 

await
ÂÂ 
_mongoDbContext
ÂÂ 
.
ÂÂ 
LessonBooks
ÂÂ )
.
ÂÂ) *
InsertOneAsync
ÂÂ* 8
(
ÂÂ8 9
newLessonBook
ÂÂ9 F
)
ÂÂF G
;
ÂÂG H
return
ÁÁ 
newLessonBook
ÁÁ 
;
ÁÁ 
}
ËË 
public
ÍÍ 

async
ÍÍ 
Task
ÍÍ 
<
ÍÍ 

LessonBook
ÍÍ  
>
ÍÍ  !
DeleteLessonBook
ÍÍ" 2
(
ÍÍ2 3
string
ÍÍ3 9
userId
ÍÍ: @
)
ÍÍ@ A
{
ÎÎ 
var
ÏÏ 
deletedLessonBook
ÏÏ 
=
ÏÏ 
await
ÏÏ  %
_mongoDbContext
ÏÏ& 5
.
ÏÏ5 6
LessonBooks
ÏÏ6 A
.
ÏÏA B#
FindOneAndDeleteAsync
ÏÏB W
(
ÏÏW X
Builders
ÌÌ 
<
ÌÌ 

LessonBook
ÌÌ 
>
ÌÌ  
.
ÌÌ  !
Filter
ÌÌ! '
.
ÌÌ' (
Eq
ÌÌ( *
(
ÌÌ* +
l
ÌÌ+ ,
=>
ÌÌ- /
l
ÌÌ0 1
.
ÌÌ1 2
IdUser
ÌÌ2 8
,
ÌÌ8 9
userId
ÌÌ: @
)
ÌÌ@ A
)
ÌÌA B
;
ÌÌB C
return
ÔÔ 
deletedLessonBook
ÔÔ  
;
ÔÔ  !
}
 
public
ÚÚ 

async
ÚÚ 
Task
ÚÚ 
<
ÚÚ 

Dictionary
ÚÚ  
<
ÚÚ  !
string
ÚÚ! '
,
ÚÚ' (
string
ÚÚ) /
>
ÚÚ/ 0
>
ÚÚ0 1$
GetLessonsByIdDocument
ÚÚ2 H
(
ÚÚH I
Document
ÚÚI Q
document
ÚÚR Z
,
ÚÚZ [
string
ÚÚ\ b
idUser
ÚÚc i
)
ÚÚi j
{
ÛÛ 
var
ÙÙ 
result
ÙÙ 
=
ÙÙ 
new
ÙÙ 

Dictionary
ÙÙ #
<
ÙÙ# $
string
ÙÙ$ *
,
ÙÙ* +
string
ÙÙ, 2
>
ÙÙ2 3
(
ÙÙ3 4
)
ÙÙ4 5
;
ÙÙ5 6
var
˜˜ 

lessonBook
˜˜ 
=
˜˜ 
await
˜˜ 
_mongoDbContext
˜˜ .
.
˜˜. /
LessonBooks
˜˜/ :
.
¯¯ 
Find
¯¯ 
(
¯¯ 
lb
¯¯ 
=>
¯¯ 
lb
¯¯ 
.
¯¯ 
IdUser
¯¯ !
==
¯¯" $
idUser
¯¯% +
)
¯¯+ ,
.
˘˘ !
FirstOrDefaultAsync
˘˘  
(
˘˘  !
)
˘˘! "
;
˘˘" #
if
˚˚ 

(
˚˚ 

lessonBook
˚˚ 
==
˚˚ 
null
˚˚ 
)
˚˚ 
return
˚˚  &
result
˚˚' -
;
˚˚- .
foreach
˛˛ 
(
˛˛ 
var
˛˛ 
lesson
˛˛ 
in
˛˛ 

lessonBook
˛˛ )
.
˛˛) *
Lessons
˛˛* 1
)
˛˛1 2
{
ˇˇ 	
var
ÄÄ 
matchingDocument
ÄÄ  
=
ÄÄ! "
lesson
ÄÄ# )
.
ÄÄ) *
	Documents
ÄÄ* 3
.
ÅÅ 
FirstOrDefault
ÅÅ 
(
ÅÅ  
d
ÅÅ  !
=>
ÅÅ" $
d
ÅÅ% &
.
ÅÅ& '

IdDocument
ÅÅ' 1
==
ÅÅ2 4
document
ÅÅ5 =
.
ÅÅ= >

IdDocument
ÅÅ> H
)
ÅÅH I
;
ÅÅI J
if
ÉÉ 
(
ÉÉ 
matchingDocument
ÉÉ  
!=
ÉÉ! #
null
ÉÉ$ (
)
ÉÉ( )
{
ÑÑ 
result
ÖÖ 
[
ÖÖ 
lesson
ÖÖ 
.
ÖÖ 
IdLesson
ÖÖ &
]
ÖÖ& '
=
ÖÖ( )
matchingDocument
ÖÖ* :
.
ÖÖ: ;

IdDocument
ÖÖ; E
;
ÖÖE F
}
ÜÜ 
}
áá 	
return
ââ 
result
ââ 
;
ââ 
}
ää 
}ãã ∆ 
AC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
builder

 
.

 
Logging

 
.

 
ClearProviders

 
(

 
)

  
;

  !
builder 
. 
Logging 
. 

AddConsole 
( 
) 
; 
builder 
. 
Services 
. 
AddDataProtection "
(" #
)# $
. #
PersistKeysToFileSystem 
( 
new  
DirectoryInfo! .
(. /
$str/ :
): ;
); <
. 
SetApplicationName 
( 
$str ,
), -
;- .
builder 
. 
Services 
. 
AddHttpClient 
( 
)  
;  !
builder 
. 
Services 
. 
AddHttpClient 
< 
HolidaysService .
>. /
(/ 0
)0 1
;1 2
builder 
. 
Services 
. 
	AddScoped 
< 
UserService &
>& '
(' (
)( )
;) *
builder 
. 
Services 
. 
	AddScoped 
< 
HolidaysService *
>* +
(+ ,
), -
;- .
builder 
. 
Services 
. 
	AddScoped 
<  
BlockVacationService /
>/ 0
(0 1
)1 2
;2 3
builder 
. 
Services 
. 
AddSingleton 
< 
MongoDbContext ,
>, -
(- .
). /
;/ 0
builder 
. 
Services 
. 
	AddScoped 
<  
ISchedulerRepository /
,/ 0
SchedulerRepository1 D
>D E
(E F
)F G
;G H
builder 
. 
Services 
. 
	AddScoped 
< 
ILessonRepository ,
,, -
LessonRepository. >
>> ?
(? @
)@ A
;A B
builder!! 
.!! 
Services!! 
.!! 
AddCors!! 
(!! 
options!!  
=>!!! #
{"" 
options## 
.## 
	AddPolicy## 
(## 
$str##  
,##  !
policy##" (
=>##) +
{$$ 
policy%% 
.%% 
AllowAnyOrigin%% 
(%% 
)%% 
.&& 
AllowAnyMethod&& 
(&& 
)&& 
.'' 
AllowAnyHeader'' 
('' 
)'' 
;'' 
}(( 
)(( 
;(( 
})) 
))) 
;)) 
builder++ 
.++ 
Services++ 
.++ 
AddControllers++ 
(++  
)++  !
;++! "
var// 
app// 
=// 	
builder//
 
.// 
Build// 
(// 
)// 
;// 
app11 
.11 
UseHttpsRedirection11 
(11 
)11 
;11 
app22 
.22 
UseCors22 
(22 
$str22 
)22 
;22 
app44 
.44 
MapControllers44 
(44 
)44 
;44 
app66 
.66 
Run66 
(66 
)66 	
;66	 

	namespace88 	
Service88
 
.88 
Database88 
{99 
public:: 

partial:: 
class:: 
Program::  
{::! "
}::# $
};; ÷
YC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Interfaces\ISchedulerRepository.cs
	namespace 	
Service
 
. 
Database 
. 

Interfaces %
;% &
public 
	interface  
ISchedulerRepository %
{ 
Task		 
<		 	
	Scheduler			 
>		 
GetScheduler		  
(		  !
string		! '
userId		( .
)		. /
;		/ 0
Task

 
<

 	
List

	 
<

 
	Scheduler

 
>

 
>

 
GetManyScheduler

 *
(

* +
List

+ /
<

/ 0
string

0 6
>

6 7
idsProfesseur

8 E
)

E F
;

F G
Task 
< 	
	Scheduler	 
> 
AddScheduler  
(  !
string! '
userId( .
). /
;/ 0
Task 
< 	
	Scheduler	 
> 
DeleteScheduler #
(# $
string$ *
userId+ 1
)1 2
;2 3
Task 
< 	
Appointment	 
> 
GetOneAppointment '
(' (
string( .
userId/ 5
,5 6
Appointment7 B
appointmentC N
)N O
;O P
Task 
< 	
Appointment	 
> !
GetOneAppointmentById +
(+ ,
string, 2
userId3 9
,9 :
Appointment; F
appointmentG R
)R S
;S T
Task 
< 	
List	 
< 
Appointment 
> 
> 
AddAppointment *
(* +
string+ 1
userId2 8
,8 9
Appointment: E
appointmentF Q
)Q R
;R S
Task 
< 	
	Scheduler	 
> 
AddListAppointment &
(& '
string' -
userId. 4
,4 5
List6 :
<: ;
Appointment; F
>F G
appointmentsH T
)T U
;U V
Task 
< 	
List	 
< 
Appointment 
> 
> 
UpdateAppointment -
(- .
string. 4
userId5 ;
,; <
Appointment= H
appointmentI T
)T U
;U V
Task 
< 	
List	 
< 
Appointment 
> 
> 
DeleteAppointment -
(- .
string. 4
userId5 ;
,; <
Appointment= H
appointmentI T
)T U
;U V
Task 
< 	
List	 
< 
Appointment 
> 
> !
DeleteListAppointment 1
(1 2
string2 8
idUser9 ?
,? @
stringA G
idRecurringH S
,S T
DateTimeU ]
	startDate^ g
)g h
;h i
Task 
< 	
List	 
< 
Appointment 
> 
> 
GetBlockVacation ,
(, -
string- 3
userId4 :
,: ;
Appointment< G
appointmentH S
)S T
;T U
} º
VC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Interfaces\ILessonRepository.cs
	namespace 	
Service
 
. 
Database 
. 

Interfaces %
;% &
public 
	interface 
ILessonRepository "
{ 
Task		 
<		 	
Lesson			 
>		 
	GetLesson		 
(		 
string		 !
idAppointment		" /
,		/ 0
string		1 7
idUser		8 >
)		> ?
;		? @
Task

 
<

 	
Lesson

	 
>

 
	AddLesson

 
(

 
Lesson

 !
lesson

" (
,

( )
string

* 0
idUser

1 7
)

7 8
;

8 9
Task 
< 	
Lesson	 
> 
UpdateLesson 
( 
Lesson $
lesson% +
,+ ,
string- 3
idUser4 :
): ;
;; <
Task 
< 	
Lesson	 
> 
DeleteLesson 
( 
Lesson $
lesson% +
,+ ,
string- 3
idUser4 :
): ;
;; <
Task 
< 	
Document	 
> 
GetDocumentInLesson &
(& '
RequestLesson' 4
request5 <
,< =
string> D
idUserE K
)K L
;L M
Task 
< 	
UpdateResult	 
> "
DeleteDocumentInLesson -
(- .
string. 4
idUser5 ;
,; <
string= C
idLessonD L
,L M
stringN T

IdDocumentU _
)_ `
;` a
Task 
< 	
UpdateResult	 
> "
UpdateDocumentInLesson -
(- .
string. 4
idUser5 ;
,; <
string= C
idLessonD L
,L M
DocumentN V
documentW _
)_ `
;` a
Task 
< 	
Document	 
> 
GetDocument 
( 
string %

idDocument& 0
,0 1
string2 8
idUser9 ?
)? @
;@ A
Task 
< 	

LessonBook	 
> 
GetLessonBook "
(" #
string# )
userId* 0
)0 1
;1 2
Task 
< 	

LessonBook	 
> 
DeleteLessonBook %
(% &
string& ,
userId- 3
)3 4
;4 5
Task 
< 	

LessonBook	 
> 
AddLessonBook "
(" #
string# )
userId* 0
)0 1
;1 2
Task 
< 	

Dictionary	 
< 
string 
, 
string "
>" #
># $"
GetLessonsByIdDocument% ;
(; <
Document< D
documentE M
,M N
stringO U
idUserV \
)\ ]
;] ^
} ∂
QC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Database\MongoDbContext.cs
	namespace 	
Service
 
. 
Database 
. 
Database #
;# $
public 
class 
MongoDbContext 
{		 
private

 
readonly

 
IMongoDatabase

 #
	_database

$ -
;

- .
public 

MongoDbContext 
( 
IConfiguration (
configuration) 6
)6 7
{ 
var 
mongoClient 
= 
new 
MongoClient )
() *
configuration* 7
.7 8
GetConnectionString8 K
(K L
$strL \
)\ ]
)] ^
;^ _
	_database 
= 
mongoClient 
.  
GetDatabase  +
(+ ,
configuration, 9
[9 :
$str: j
]j k
)k l
;l m
} 
public 

IMongoCollection 
< 
	Scheduler %
>% &

Schedulers' 1
=>2 4
	_database5 >
.> ?
GetCollection? L
<L M
	SchedulerM V
>V W
(W X
$strX d
)d e
;e f
public 

IMongoCollection 
< 

LessonBook &
>& '
LessonBooks( 3
=>4 6
	_database7 @
.@ A
GetCollectionA N
<N O

LessonBookO Y
>Y Z
(Z [
$str[ h
)h i
;i j
} ‘ë
VC:\Users\bapti\RiderProjects\MaClasse\Service.Database\Controllers\LessonController.cs
	namespace		 	
Service		
 
.		 
Database		 
.		 
Controllers		 &
;		& '
[ 
ApiController 
] 
[ 
Route 
( 
$str 
) 
] 
public 
class 
LessonController 
: 
ControllerBase  .
{ 
private 
readonly 
ILessonRepository &
_lessonRepository' 8
;8 9
private 
readonly 
UserService  
_userService! -
;- .
private 
readonly 
ILogger 
< 
LessonController -
>- .
_logger/ 6
;6 7
public 

LessonController 
( 
ILessonRepository 
lessonRepository *
,* +
UserService 
userService 
,  
ILogger 
< 
LessonController  
>  !
logger" (
)( )
{ 
_lessonRepository 
= 
lessonRepository ,
;, -
_userService 
= 
userService "
;" #
_logger 
= 
logger 
; 
} 
[ 
HttpPost 
] 
[   
Route   

(  
 
$str   
)   
]   
public!! 

async!! 
Task!! 
<!! 
IActionResult!! #
>!!# $
GetLessonBook!!% 2
(!!2 3
CreateDataRequest!!3 D
request!!E L
)!!L M
{"" 
var## 
existingLessonBook## 
=##  
_lessonRepository##! 2
.##2 3
GetLessonBook##3 @
(##@ A
request##A H
.##H I
UserId##I O
)##O P
;##P Q
if%% 

(%% 
existingLessonBook%% 
==%% !
null%%" &
)%%& '
return%%( .
NotFound%%/ 7
(%%7 8
)%%8 9
;%%9 :
return'' 
Ok'' 
('' 
existingLessonBook'' $
)''$ %
;''% &
}(( 
[** 
HttpPost** 
]** 
[++ 
Route++ 

(++
 
$str++ 
)++ 
]++ 
public,, 

async,, 
Task,, 
<,, 
IActionResult,, #
>,,# $$
GetLessonByAppointmentId,,% =
(,,= >
LessonRequest,,> K
request,,L S
),,S T
{-- 
string.. 
idUser.. 
;.. 
if00 

(00 
request00 
.00 
UserLessonDisplayed00 '
==00( *
null00+ /
)00/ 0
{11 	
idUser22 
=22 
_userService22 !
.22! "
GetUserByIdSession22" 4
(224 5
request225 <
.22< =
	IdSession22= F
)22F G
.22G H
Result22H N
.22N O
UserId22O U
;22U V
}33 	
else44 
{55 	
idUser66 
=66 
request66 
.66 
UserLessonDisplayed66 0
;660 1
}77 	
var99 
existingLesson99 
=99 
await99 "
_lessonRepository99# 4
.994 5
	GetLesson995 >
(99> ?
request:: 
.:: 
Appointment:: 
.::  
Id::  "
,::" #
idUser::$ *
)::* +
;::+ ,
if<< 

(<< 
existingLesson<< 
==<< 
null<< "
)<<" #
return<<$ *
NotFound<<+ 3
(<<3 4
null<<4 8
)<<8 9
;<<9 :
return>> 
Ok>> 
(>> 
existingLesson>>  
)>>  !
;>>! "
}?? 
[AA 
HttpPostAA 
]AA 
[BB 
RouteBB 

(BB
 
$strBB 
)BB 
]BB 
publicCC 

asyncCC 
TaskCC 
<CC 
IActionResultCC #
>CC# $
	AddLessonCC% .
(CC. /
RequestLessonCC/ <
requestCC= D
)CCD E
{DD 
varEE 
idUserEE 
=EE 
_userServiceEE "
.EE" #
GetUserByIdSessionEE# 5
(EE5 6
requestEE6 =
.EE= >
	IdSessionEE> G
)EEG H
.EEH I
ResultEEI O
.EEO P
UserIdEEP V
;EEV W
ifHH 

(HH 
requestHH 
.HH 
LessonHH 
.HH 
IdLessonHH #
==HH$ &
nullHH' +
)HH+ ,
{II 	
varJJ 
	newLessonJJ 
=JJ 
awaitJJ !
_lessonRepositoryJJ" 3
.JJ3 4
	AddLessonJJ4 =
(JJ= >
requestJJ> E
.JJE F
LessonJJF L
,JJL M
idUserJJN T
)JJT U
;JJU V
ifLL 
(LL 
	newLessonLL 
==LL 
nullLL !
)LL! "
returnLL# )

BadRequestLL* 4
(LL4 5
)LL5 6
;LL6 7
returnNN 
OkNN 
(NN 
	newLessonNN 
)NN  
;NN  !
}OO 	
elsePP 
{QQ 	
varRR 
updatedLessonRR 
=RR 
awaitRR  %
_lessonRepositoryRR& 7
.RR7 8
UpdateLessonRR8 D
(RRD E
requestRRE L
.RRL M
LessonRRM S
,RRS T
idUserRRU [
)RR[ \
;RR\ ]
ifTT 
(TT 
updatedLessonTT 
==TT  
nullTT! %
)TT% &
returnTT' -

BadRequestTT. 8
(TT8 9
)TT9 :
;TT: ;
returnVV 
OkVV 
(VV 
updatedLessonVV #
)VV# $
;VV$ %
}WW 	
}YY 
[[[ 
HttpPost[[ 
][[ 
[\\ 
Route\\ 

(\\
 
$str\\ 
)\\ 
]\\ 
public]] 

async]] 
Task]] 
<]] 
IActionResult]] #
>]]# $
DeleteLesson]]% 1
(]]1 2
RequestLesson]]2 ?
request]]@ G
)]]G H
{^^ 
Lesson__ 
existingLesson__ 
;__ 
varaa 
idUseraa 
=aa 
_userServiceaa !
.aa! "
GetUserByIdSessionaa" 4
(aa4 5
requestaa5 <
.aa< =
	IdSessionaa= F
)aaF G
.aaG H
ResultaaH N
.aaN O
UserIdaaO U
;aaU V
ifcc 

(cc 
requestcc 
.cc 
IdAppointementcc "
!=cc# %
nullcc& *
)cc* +
{dd 	
existingLessonee 
=ee 
awaitee "
_lessonRepositoryee# 4
.ee4 5
	GetLessonee5 >
(ee> ?
requestee? F
.eeF G
IdAppointementeeG U
,eeU V
idUsereeW ]
)ee] ^
;ee^ _
}ff 	
elsegg 
{hh 	
existingLessonii 
=ii 
awaitii "
_lessonRepositoryii# 4
.ii4 5
	GetLessonii5 >
(ii> ?
requestii? F
.iiF G
LessoniiG M
.iiM N
IdAppointmentiiN [
,ii[ \
idUserii] c
)iic d
;iid e
}jj 	
ifll 

(ll 
existingLessonll 
==ll 
nullll "
)ll" #
returnll$ *
NotFoundll+ 3
(ll3 4
)ll4 5
;ll5 6
varnn 
deletedLessonnn 
=nn 
awaitnn !
_lessonRepositorynn" 3
.nn3 4
DeleteLessonnn4 @
(nn@ A
existingLessonnnA O
,nnO P
idUsernnQ W
)nnW X
;nnX Y
ifpp 

(pp 
deletedLessonpp 
==pp 
nullpp !
)pp! "
returnpp# )
NotFoundpp* 2
(pp2 3
)pp3 4
;pp4 5
returnrr 
Okrr 
(rr 
deletedLessonrr 
)rr  
;rr  !
}ss 
[uu 
HttpPostuu 
]uu 
[vv 
Routevv 

(vv
 
$strvv 
)vv 
]vv 
publicww 

asyncww 
Taskww 
<ww 
IActionResultww #
>ww# $
GetDocumentww% 0
(ww0 1
[ww1 2
FromBodyww2 :
]ww: ;!
FileRequestToDatabaseww< Q
requestwwR Y
)wwY Z
{xx 
_loggeryy 
.yy 
LogInformationyy 
(yy 
$stryy m
)yym n
;yyn o
_loggerzz 
.zz 
LogInformationzz 
(zz 
$str	zz Ö
,
zzÖ Ü
request
zzá é
?
zzé è
.
zzè ê
Document
zzê ò
?
zzò ô
.
zzô ö

IdDocument
zzö §
,
zz§ •
request
zz¶ ≠
?
zz≠ Æ
.
zzÆ Ø
IdUser
zzØ µ
)
zzµ ∂
;
zz∂ ∑
var|| 
existingDocument|| 
=|| 
await|| $
_lessonRepository||% 6
.||6 7
GetDocument||7 B
(||B C
request||C J
.||J K
Document||K S
.||S T

IdDocument||T ^
,||^ _
request||` g
.||g h
IdUser||h n
)||n o
;||o p
if~~ 

(~~ 
existingDocument~~ 
==~~ 
null~~  $
)~~$ %
return~~& ,
NotFound~~- 5
(~~5 6
)~~6 7
;~~7 8
_logger
ÄÄ 
.
ÄÄ 
LogInformation
ÄÄ 
(
ÄÄ 
$str
ÄÄ W
,
ÄÄW X
existingDocument
ÄÄY i
.
ÄÄi j
Name
ÄÄj n
)
ÄÄn o
;
ÄÄo p
return
ÉÉ 
Ok
ÉÉ 
(
ÉÉ 
existingDocument
ÉÉ "
)
ÉÉ" #
;
ÉÉ# $
}
ÑÑ 
[
ÜÜ 
HttpPost
ÜÜ 
]
ÜÜ 
[
áá 
Route
áá 

(
áá
 
$str
áá &
)
áá& '
]
áá' (
public
àà 

async
àà 
Task
àà 
<
àà 
IActionResult
àà #
>
àà# $$
DeleteDocumentInLesson
àà% ;
(
àà; <
[
àà< =
FromBody
àà= E
]
ààE F
RequestLesson
ààG T
request
ààU \
)
àà\ ]
{
ââ 
var
ää 
idUser
ää 
=
ää 
_userService
ää !
.
ää! " 
GetUserByIdSession
ää" 4
(
ää4 5
request
ää5 <
.
ää< =
	IdSession
ää= F
)
ääF G
.
ääG H
Result
ääH N
.
ääN O
UserId
ääO U
;
ääU V
var
åå 
existingDocument
åå 
=
åå 
await
åå $
_lessonRepository
åå% 6
.
åå6 7!
GetDocumentInLesson
åå7 J
(
ååJ K
request
ååK R
,
ååR S
idUser
ååT Z
)
ååZ [
;
åå[ \
if
éé 

(
éé 
existingDocument
éé 
==
éé 
null
éé  $
)
éé$ %
return
éé& ,
NotFound
éé- 5
(
éé5 6
)
éé6 7
;
éé7 8
var
êê #
resultDeletedDocument
êê !
=
êê" #
await
êê$ )
_lessonRepository
êê* ;
.
êê; <$
DeleteDocumentInLesson
êê< R
(
êêR S
idUser
ëë 
,
ëë 
request
ëë 
.
ëë 
Lesson
ëë "
.
ëë" #
IdLesson
ëë# +
,
ëë+ ,
existingDocument
ëë- =
.
ëë= >

IdDocument
ëë> H
)
ëëH I
;
ëëI J
if
ìì 

(
ìì #
resultDeletedDocument
ìì !
==
ìì" $
null
ìì% )
)
ìì) *
return
ìì+ 1
NotFound
ìì2 :
(
ìì: ;
)
ìì; <
;
ìì< =
return
ïï 
Ok
ïï 
(
ïï 
request
ïï 
.
ïï 
Document
ïï "
)
ïï" #
;
ïï# $
}
ññ 
[
òò 
HttpPost
òò 
]
òò 
[
ôô 
Route
ôô 

(
ôô
 
$str
ôô &
)
ôô& '
]
ôô' (
public
öö 

async
öö 
Task
öö 
<
öö 
IActionResult
öö #
>
öö# $$
UploadDocumentInLesson
öö% ;
(
öö; <
[
öö< =
FromBody
öö= E
]
ööE F
RequestLesson
ööG T
request
ööU \
)
öö\ ]
{
õõ 
var
úú 
idUser
úú 
=
úú 
_userService
úú !
.
úú! " 
GetUserByIdSession
úú" 4
(
úú4 5
request
úú5 <
.
úú< =
	IdSession
úú= F
)
úúF G
.
úúG H
Result
úúH N
.
úúN O
UserId
úúO U
;
úúU V
var
ûû 
existingDocument
ûû 
=
ûû 
await
ûû $
_lessonRepository
ûû% 6
.
ûû6 7!
GetDocumentInLesson
ûû7 J
(
ûûJ K
request
ûûK R
,
ûûR S
idUser
ûûT Z
)
ûûZ [
;
ûû[ \
if
†† 

(
†† 
existingDocument
†† 
==
†† 
null
††  $
)
††$ %
return
††& ,
NotFound
††- 5
(
††5 6
)
††6 7
;
††7 8
var
¢¢ #
resultUpdatedDocument
¢¢ !
=
¢¢" #
await
¢¢$ )
_lessonRepository
¢¢* ;
.
¢¢; <$
UpdateDocumentInLesson
¢¢< R
(
¢¢R S
idUser
££ 
,
££ 
request
££ 
.
££ 
Lesson
££ "
.
££" #
IdLesson
££# +
,
££+ ,
request
££- 4
.
££4 5
Document
££5 =
)
££= >
;
££> ?
if
•• 

(
•• #
resultUpdatedDocument
•• !
==
••" $
null
••% )
)
••) *
return
••+ 1
NotFound
••2 :
(
••: ;
)
••; <
;
••< =
return
ßß 
Ok
ßß 
(
ßß 
request
ßß 
.
ßß 
Document
ßß "
)
ßß" #
;
ßß# $
}
®® 
[
™™ 
HttpPost
™™ 
]
™™ 
[
´´ 
Route
´´ 

(
´´
 
$str
´´ 
)
´´ 
]
´´ 
public
¨¨ 

async
¨¨ 
Task
¨¨ 
<
¨¨ 
IActionResult
¨¨ #
>
¨¨# $
AddLessonBook
¨¨% 2
(
¨¨2 3
[
¨¨3 4
FromBody
¨¨4 <
]
¨¨< =
CreateDataRequest
¨¨> O
request
¨¨P W
)
¨¨W X
{
≠≠ 
var
ÆÆ 
newLessonBook
ÆÆ 
=
ÆÆ 
await
ÆÆ !
_lessonRepository
ÆÆ" 3
.
ÆÆ3 4
AddLessonBook
ÆÆ4 A
(
ÆÆA B
request
ÆÆB I
.
ÆÆI J
UserId
ÆÆJ P
)
ÆÆP Q
;
ÆÆQ R
if
∞∞ 

(
∞∞ 
newLessonBook
∞∞ 
==
∞∞ 
null
∞∞ !
)
∞∞! "
return
∞∞# )
NotFound
∞∞* 2
(
∞∞2 3
)
∞∞3 4
;
∞∞4 5
return
≤≤ 
Ok
≤≤ 
(
≤≤ 
newLessonBook
≤≤ 
)
≤≤  
;
≤≤  !
}
≥≥ 
[
µµ 
HttpPost
µµ 
]
µµ 
[
∂∂ 
Route
∂∂ 

(
∂∂
 
$str
∂∂ 
)
∂∂ 
]
∂∂  
public
∑∑ 

async
∑∑ 
Task
∑∑ 
<
∑∑ 
IActionResult
∑∑ #
>
∑∑# $
DeleteLessonBook
∑∑% 5
(
∑∑5 6
[
∑∑6 7
FromBody
∑∑7 ?
]
∑∑? @
DeleteUserRequest
∑∑A R
request
∑∑S Z
)
∑∑Z [
{
∏∏ 
var
ππ 
deletedLessonBook
ππ 
=
ππ 
await
ππ  %
_lessonRepository
ππ& 7
.
ππ7 8
DeleteLessonBook
ππ8 H
(
ππH I
request
ππI P
.
ππP Q
IdUser
ππQ W
)
ππW X
;
ππX Y
if
ªª 

(
ªª 
deletedLessonBook
ªª 
==
ªª  
null
ªª! %
)
ªª% &
return
ªª' -
NotFound
ªª. 6
(
ªª6 7
)
ªª7 8
;
ªª8 9
return
ΩΩ 
Ok
ΩΩ 
(
ΩΩ 
deletedLessonBook
ΩΩ #
)
ΩΩ# $
;
ΩΩ$ %
}
ææ 
[
¿¿ 
HttpPost
¿¿ 
]
¿¿ 
[
¡¡ 
Route
¡¡ 

(
¡¡
 
$str
¡¡ &
)
¡¡& '
]
¡¡' (
public
¬¬ 

async
¬¬ 
Task
¬¬ 
<
¬¬ 
IActionResult
¬¬ #
>
¬¬# $'
GetLessonBookByIdDocument
¬¬% >
(
¬¬> ?
[
¬¬? @
FromBody
¬¬@ H
]
¬¬H I
RequestLesson
¬¬J W
request
¬¬X _
)
¬¬_ `
{
√√ 
var
≈≈ 
idUser
≈≈ 
=
≈≈ 
_userService
≈≈ !
.
≈≈! " 
GetUserByIdSession
≈≈" 4
(
≈≈4 5
request
≈≈5 <
.
≈≈< =
	IdSession
≈≈= F
)
≈≈F G
.
≈≈G H
Result
≈≈H N
.
≈≈N O
UserId
≈≈O U
;
≈≈U V
var
«« !
lessonsWithDocument
«« 
=
««  !
await
««" '
_lessonRepository
««( 9
.
««9 :$
GetLessonsByIdDocument
««: P
(
««P Q
request
««Q X
.
««X Y
Document
««Y a
,
««a b
idUser
««c i
)
««i j
;
««j k
if
…… 

(
……
 !
lessonsWithDocument
…… 
==
…… !
null
……" &
)
……& '
return
……( .
NotFound
……/ 7
(
……7 8
)
……8 9
;
……9 :
return
ÀÀ 
Ok
ÀÀ 
(
ÀÀ !
lessonsWithDocument
ÀÀ %
)
ÀÀ% &
;
ÀÀ& '
}
ÃÃ 
}ÕÕ 