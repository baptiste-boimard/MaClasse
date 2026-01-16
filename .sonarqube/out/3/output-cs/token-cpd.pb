„
RC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\ValidateGoogleToken.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
;  
public 
class &
ValidateGoogleTokenService '
:( )'
IValidateGoogleTokenService* E
{ 
private 
readonly 
IConfiguration #
_configuration$ 2
;2 3
public

 
&
ValidateGoogleTokenService

 %
(

% &
IConfiguration

& 4
configuration

5 B
)

B C
{ 
_configuration 
= 
configuration &
;& '
} 
public 

async 
Task 
< "
GoogleJsonWebSignature ,
., -
Payload- 4
?4 5
>5 6
ValidateGoogleToken7 J
(J K
stringK Q
?Q R
tokenS X
)X Y
{ 
try 
{ 	
var 
settings 
= 
new "
GoogleJsonWebSignature 5
.5 6
ValidationSettings6 H
{ 
Audience 
= 
new 
[ 
]  
{! "
$"# %
{% &
_configuration& 4
[4 5
$str5 U
]U V
}V W
"W X
}Y Z
} 
; 
var 
payload 
= 
await "
GoogleJsonWebSignature  6
.6 7
ValidateAsync7 D
(D E
tokenE J
,J K
settingsL T
)T U
;U V
return 
payload 
; 
} 	
catch 
( 
	Exception 
ex 
) 
{ 	
Console 
. 
	WriteLine 
( 
$"  
$str  H
{H I
exI K
.K L
MessageL S
}S T
"T U
)U V
;V W
return 
null 
; 
} 	
}   
}!! ¯
UC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\UserServiceRattachment.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
;  
public

 
class

 "
UserServiceRattachment

 #
:

$ %#
IUserServiceRattachment

& =
{ 
private 
readonly 
IAuthRepository $
_authRepository% 4
;4 5
public 
"
UserServiceRattachment !
(! "
IAuthRepository 
authRepository &
)' (
{ 
_authRepository 
= 
authRepository (
;( )
} 
public 

async 
Task 
< 

AuthReturn  
?  !
>! ""
GetUserWithRattachment# 9
(9 :
UserProfile 
user 
, 
bool 
	isNewUser 
, 
string 
	idSession 
, 
string 
? 

AccesToken 
, 
	Scheduler 
	scheduler 
) 
{ 
var 
rattachments 
= 
await  
_authRepository! 0
.0 1"
GetRattachmentByIdRole1 G
(G H
userH L
.L M
IdRoleM S
)S T
;T U
var 
userWithRattachment 
=  !
new" %
UserWithRattachment& 9
{ 	
UserProfile 
= 
user 
, 
AsDirecteur   
=   
rattachments   &
.  & '
Where  ' ,
(  , -
r  - .
=>  / 1
r  2 3
.  3 4
IdDirecteur  4 ?
==  @ B
user  C G
.  G H
IdRole  H N
)  N O
.  O P
ToList  P V
(  V W
)  W X
,  X Y
AsProfesseur!! 
=!! 
rattachments!! '
.!!' (
Where!!( -
(!!- .
r!!. /
=>!!0 2
r!!3 4
.!!4 5
IdProfesseur!!5 A
==!!B D
user!!E I
.!!I J
IdRole!!J P
)!!P Q
.!!Q R
ToList!!R X
(!!X Y
)!!Y Z
,!!Z [
AccessToken"" 
="" 

AccesToken"" $
}## 	
;##	 

var%% 

authReturn%% 
=%% 
new%% 

AuthReturn%% '
{&& 	
	IsNewUser'' 
='' 
	isNewUser'' !
,''! "
UserWithRattachment(( 
=((  !
userWithRattachment((" 5
,((5 6
	IdSession)) 
=)) 
	idSession)) !
,))! "
	Scheduler** 
=** 
	scheduler** !
,**! "
}++ 	
;++	 

return-- 

authReturn-- 
;-- 
}.. 
}// Ç
dC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\Interface\IValidateGoogleTokenService.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
.  
	Interface  )
;) *
public 
	interface '
IValidateGoogleTokenService ,
{ 
Task 
< "
GoogleJsonWebSignature 
. 
Payload %
?% &
>& '
ValidateGoogleToken( ;
(; <
string< B
?B C
tokenD I
)I J
;J K
} ¿
`C:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\Interface\IUserServiceRattachment.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
.  
	Interface  )
;) *
public 
	interface #
IUserServiceRattachment (
{ 
Task 
< 

AuthReturn 
? 
> "
GetUserWithRattachment *
(* +
UserProfile+ 6
user7 ;
,; <
bool< @
	isNewUserA J
,J K
stringK Q
	idSessionR [
,[ \
string\ b
?b c

AccesTokend n
,n o
	Schedulero x
	scheduler	y Ç
)
Ç É
;
É Ñ
}		 Ë
XC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\Interface\IGenerateIdRole.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
.  
	Interface  )
;) *
public 
	interface 
IGenerateIdRole  
{ 
Task 
< 
string 
> 
GenerateIdAsync 
( 
int "
length# )
=* +
$num, -
)- .
;. /
string 
Generate	 
( 
int 
length 
) 
; 
Task 
< 
bool 
>  
VerifyExistingIdRole !
(! "
string" (
idRole) /
)/ 0
;0 1
} Ó
[C:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\Interface\IDeleteUserService.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
.  
	Interface  )
;) *
public 
	interface 
IDeleteUserService #
{ 
Task 
DeleteLessonBook 
( 
string 
userId %
)% &
;& '
Task 
DeleteScheduler 
( 
string 
userId $
)$ %
;% &
} ›
[C:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\Interface\ICreateDataService.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
.  
	Interface  )
;) *
public 
	interface 
ICreateDataService #
{ 
Task		 
<		 
	Scheduler		 
>		 
CreateDataScheduler		 %
(		% &
string		& ,
userId		- 3
)		3 4
;		4 5
Task

 
<

 
	Scheduler

 
>

 
GetDataScheduler

 "
(

" #
string

# )
userId

* 0
)

0 1
;

1 2
Task 
< 
	Scheduler 
> !
AddHolidayToScheduler '
(' (
UserProfile( 3
user4 8
)8 9
;9 :
Task 
< 

LessonBook 
>  
CreateDateLessonBook '
(' (
string( .
userId/ 5
)5 6
;6 7
} ª
MC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\GenerateIdRole.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
;  
public 
class 
GenerateIdRole 
: 
IGenerateIdRole -
{ 
private		 
readonly		 
IAuthRepository		 $
_authRepository		% 4
;		4 5
public 

GenerateIdRole 
( 
IAuthRepository )
authRepository* 8
)8 9
{ 
_authRepository 
= 
authRepository (
;( )
} 
public 

async 
Task 
< 
string 
> 
GenerateIdAsync -
(- .
int. 1
length2 8
=9 :
$num; <
)< =
{ 
while 
( 
true 
) 
{ 	
var 
	candidate 
= 
Generate $
($ %
length% +
)+ ,
;, -
var 
isFree 
= 
await  
VerifyExistingIdRole 3
(3 4
	candidate4 =
)= >
;> ?
if 
( 
isFree 
) 
return 
	candidate  
;  !
} 	
} 
public 

string 
Generate 
( 
int 
length %
)% &
{ 
const 
string 
chars 
= 
$str C
;C D
var 
random 
= 
new 
Random 
(  
)  !
;! "
return   
new   
string   
(   

Enumerable   $
.  $ %
Repeat  % +
(  + ,
chars  , 1
,  1 2
length  3 9
)  9 :
.!! 
Select!! 
(!! 
s!! 
=>!! 
s!! 
[!! 
random!! !
.!!! "
Next!!" &
(!!& '
s!!' (
.!!( )
Length!!) /
)!!/ 0
]!!0 1
)!!1 2
.!!2 3
ToArray!!3 :
(!!: ;
)!!; <
)!!< =
;!!= >
}"" 
public$$ 

async$$ 
Task$$ 
<$$ 
bool$$ 
>$$  
VerifyExistingIdRole$$ 0
($$0 1
string$$1 7
idRole$$8 >
)$$> ?
{%% 
var&& 
existing&& 
=&& 
await&& 
_authRepository&& ,
.&&, -
CheckIdRole&&- 8
(&&8 9
idRole&&9 ?
)&&? @
;&&@ A
if(( 

((( 
!(( 
existing(( 
)(( 
return(( 
true(( "
;((" #
return** 
false** 
;** 
}++ 
},, Ê6
PC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\CreateDataService.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
;  
public		 
class		 
CreateDataService		 
:		  
ICreateDataService		! 3
{

 
private 
readonly 

HttpClient 
_httpClient  +
;+ ,
private 
readonly 
IConfiguration #
_configuration$ 2
;2 3
public 

CreateDataService 
( 

HttpClient 

httpClient 
, 
IConfiguration 
configuration $
)$ %
{ 
_httpClient 
= 

httpClient  
;  !
_configuration 
= 
configuration &
;& '
} 
public 

async 
Task 
< 
	Scheduler 
>  
CreateDataScheduler! 4
(4 5
string5 ;
userId< B
)B C
{ 
var 
response 
= 
await 
_httpClient (
.( )
PostAsJsonAsync) 8
(8 9
$" 
{ 
_configuration 
[ 
$str .
]. /
}/ 0
$str0 K
"K L
,L M
newN Q
CreateDataRequestR c
{ 
UserId 
= 
userId 
} 
) 
; 
if 

( 
response 
. 
IsSuccessStatusCode (
)( )
{ 	
var   
newScheduler   
=   
await   $
response  % -
.  - .
Content  . 5
.  5 6
ReadFromJsonAsync  6 G
<  G H
	Scheduler  H Q
>  Q R
(  R S
)  S T
;  T U
if"" 
("" 
newScheduler"" 
!="" 
null""  $
)""$ %
{## 
return$$ 
newScheduler$$ #
;$$# $
}%% 
}&& 	
var'' 
error'' 
='' 
await'' 
response'' "
.''" #
Content''# *
.''* +
ReadAsStringAsync''+ <
(''< =
)''= >
;''> ?
Console(( 
.(( 
	WriteLine(( 
((( 
$"(( 
$str(( )
{(() *
response((* 2
.((2 3

StatusCode((3 =
}((= >
$str((> I
{((I J
error((J O
}((O P
"((P Q
)((Q R
;((R S
return** 
null** 
;** 
}++ 
public-- 

async-- 
Task-- 
<-- 
	Scheduler-- 
>--  
GetDataScheduler--! 1
(--1 2
string--2 8
userId--9 ?
)--? @
{.. 
var// 
response// 
=// 
await// 
_httpClient// (
.//( )
PostAsJsonAsync//) 8
(//8 9
$"00 
{00 
_configuration00 
[00 
$str00 .
]00. /
}00/ 0
$str000 K
"00K L
,00L M
new00N Q
CreateDataRequest00R c
{11 
UserId22 
=22 
userId22 
}33 
)33 
;33 
if55 

(55 
response55 
.55 
IsSuccessStatusCode55 (
)55( )
{66 	
var77 
newScheduler77 
=77 
await77 $
response77% -
.77- .
Content77. 5
.775 6
ReadFromJsonAsync776 G
<77G H
	Scheduler77H Q
>77Q R
(77R S
)77S T
;77T U
if99 
(99 
newScheduler99 
!=99 
null99  $
)99$ %
{:: 
return;; 
newScheduler;; #
;;;# $
}<< 
}== 	
return?? 
null?? 
;?? 
}@@ 
publicBB 

asyncBB 
TaskBB 
<BB 
	SchedulerBB 
>BB  !
AddHolidayToSchedulerBB! 6
(BB6 7
UserProfileBB7 B
userBBC G
)BBG H
{CC 
varDD 
responseDD 
=DD 
awaitDD 
_httpClientDD (
.DD( )
PostAsJsonAsyncDD) 8
(DD8 9
$"EE 
{EE 
_configurationEE 
[EE 
$strEE .
]EE. /
}EE/ 0
$strEE0 U
"EEU V
,EEV W
userFF 
)FF 
;FF 
ifHH 

(HH 
responseHH 
.HH 
IsSuccessStatusCodeHH (
)HH( )
{II 	
varKK 
	schedulerKK 
=KK 
awaitKK !
responseKK" *
.KK* +
ContentKK+ 2
.KK2 3
ReadFromJsonAsyncKK3 D
<KKD E
	SchedulerKKE N
>KKN O
(KKO P
)KKP Q
;KKQ R
returnLL 
	schedulerLL 
;LL 
}MM 	
returnNN 
newNN 
	SchedulerNN 
(NN 
)NN 
;NN 
}OO 
publicQQ 

asyncQQ 
TaskQQ 
<QQ 

LessonBookQQ  
>QQ  ! 
CreateDateLessonBookQQ" 6
(QQ6 7
stringQQ7 =
userIdQQ> D
)QQD E
{RR 
varSS  
newCreateDateRequestSS  
=SS! "
newSS# &
CreateDataRequestSS' 8
{TT 	
UserIdUU 
=UU 
userIdUU 
}VV 	
;VV	 

varXX 
responseXX 
=XX 
awaitXX 
_httpClientXX (
.XX( )
PostAsJsonAsyncXX) 8
(XX8 9
$"YY 
{YY 
_configurationYY 
[YY 
$strYY .
]YY. /
}YY/ 0
$strYY0 L
"YYL M
,YYM N 
newCreateDateRequestYYO c
)YYc d
;YYd e
if[[ 

([[ 
response[[ 
.[[ 
IsSuccessStatusCode[[ (
)[[( )
{\\ 	
var]] 
newLessonBook]] 
=]] 
await]]  %
response]]& .
.]]. /
Content]]/ 6
.]]6 7
ReadFromJsonAsync]]7 H
<]]H I

LessonBook]]I S
>]]S T
(]]T U
)]]U V
;]]V W
if__ 
(__ 
newLessonBook__ 
!=__  
null__! %
)__% &
return__' -
newLessonBook__. ;
;__; <
}aa 	
returncc 
nullcc 
;cc 
}dd 
}ee ˛ 
UC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Repositories\SessionRepository.cs
	namespace 	
Service
 
. 
OAuth 
. 
Repositories $
;$ %
public 
class 
SessionRepository 
:  
ISessionRepository! 3
{		 
private

 
readonly

 
PostgresDbContext

 &
_postgresDbContext

' 9
;

9 :
public 

SessionRepository 
( 
PostgresDbContext .
postgresDbContext/ @
)@ A
{ 
_postgresDbContext 
= 
postgresDbContext .
;. /
} 
public 

async 
Task 
< 
SessionData !
>! "
GetUserIdByCookies# 5
(5 6
string6 <
token= B
)B C
{ 
var 
user 
= 
await 
_postgresDbContext +
.+ ,
SessionDatas, 8
.8 9
FirstOrDefaultAsync9 L
(L M
s 
=> 
s 
. 
Token 
== 
token !
)! "
;" #
if 

( 
user 
!= 
null 
) 
return  
user! %
;% &
return 
null 
; 
} 
public 

async 
Task 
< 
SessionData !
>! "
SaveNewSession# 1
(1 2
SessionData2 =
sessionData> I
)I J
{ 
_postgresDbContext 
. 
SessionDatas '
.' (
Add( +
(+ ,
sessionData, 7
)7 8
;8 9
await 
_postgresDbContext  
.  !
SaveChangesAsync! 1
(1 2
)2 3
;3 4
return 
sessionData 
; 
} 
public!! 

async!! 
Task!! 
<!! 
SessionData!! !
>!!! "
DeleteSessionData!!# 4
(!!4 5
SessionData!!5 @
sessionData!!A L
)!!L M
{"" 
_postgresDbContext## 
.## 
SessionDatas## '
.##' (
Remove##( .
(##. /
sessionData##/ :
)##: ;
;##; <
await$$ 
_postgresDbContext$$  
.$$  !
SaveChangesAsync$$! 1
($$1 2
)$$2 3
;$$3 4
return&& 
sessionData&& 
;&& 
}'' 
public)) 

async)) 
Task)) 
<)) 
SessionData)) !
>))! "
UpdateSession))# 0
())0 1
SessionData))1 <
sessionData))= H
)))H I
{** 
_postgresDbContext++ 
.++ 
Update++ !
(++! "
sessionData++" -
)++- .
;++. /
await,, 
_postgresDbContext,,  
.,,  !
SaveChangesAsync,,! 1
(,,1 2
),,2 3
;,,3 4
return.. 
sessionData.. 
;.. 
}// 
public11 

async11 
Task11 
<11 
SessionData11 !
>11! "
GetSessionByUserId11# 5
(115 6
string116 <
userId11= C
)11C D
{22 
var33 
existingSession33 
=33 
await33 #
_postgresDbContext33$ 6
.336 7
SessionDatas337 C
.33C D
FirstOrDefaultAsync33D W
(33W X
s44 
=>44 
s44 
.44 
UserId44 
==44 
userId44 #
)44# $
;44$ %
if66 

(66 
existingSession66 
==66 
null66 #
)66# $
return66% +
null66, 0
;660 1
return88 
existingSession88 
;88 
}99 
};; ◊%
ZC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Repositories\RattachementRepository.cs
	namespace 	
Service
 
. 
OAuth 
. 
Repositories $
;$ %
public 
class "
RattachementRepository #
:$ %"
IRattachmentRepository& <
{		 
private

 
readonly

 
PostgresDbContext

 &
_postgresDbContext

' 9
;

9 :
public 
"
RattachementRepository !
(! "
PostgresDbContext 
postgresDbContext +
)+ ,
{ 
_postgresDbContext 
= 
postgresDbContext .
;. /
} 
public 

async 
Task 
< 
List 
< 
Rattachment &
>& '
>' (
GetRattachmentProf) ;
(; <
string< B

idRoleUserC M
)M N
{ 
var 
listRattachment 
= 
await #
_postgresDbContext$ 6
.6 7
Rattachments7 C
. 
Where 
( 
r 
=> 
r 
. 
IdDirecteur %
==& (

idRoleUser) 3
)3 4
. 
ToListAsync 
( 
) 
; 
return 
listRattachment 
; 
} 
public 

async 
Task 
< 
List 
< 
Rattachment &
>& '
>' ( 
GetRattachmentDirect) =
(= >
string> D

IdRoleUserE O
)O P
{ 
var 
listRattachment 
= 
await #
_postgresDbContext$ 6
.6 7
Rattachments7 C
. 
Where 
( 
r 
=> 
r 
. 
IdProfesseur &
==' )

IdRoleUser* 4
)4 5
. 
ToListAsync 
( 
) 
; 
return!! 
listRattachment!! 
;!! 
}"" 
public$$ 

async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
Rattachment$$ &
>$$& '
>$$' (
GetRattachment$$) 7
($$7 8
Rattachment$$8 C
rattachment$$D O
)$$O P
{%% 
var&& 
existingRattachment&& 
=&&  !
await&&" '
_postgresDbContext&&( :
.&&: ;
Rattachments&&; G
.'' 
Where'' 
('' 
r'' 
=>'' 
r'' 
.'' 
IdProfesseur'' &
==''' )
rattachment''* 5
.''5 6
IdProfesseur''6 B
&&''C E
r''F G
.''G H
IdDirecteur''H S
==''T V
rattachment''W b
.''b c
IdDirecteur''c n
)''n o
.(( 
ToListAsync(( 
((( 
)(( 
;(( 
return** 
existingRattachment** "
;**" #
}++ 
public-- 

async-- 
Task-- 
<-- 
Rattachment-- !
>--! "
AddRattachment--# 1
(--1 2
Rattachment--2 =
rattachment--> I
)--I J
{.. 
rattachment00 
.00 
IdGuid00 
=00 
Guid00 !
.00! "
NewGuid00" )
(00) *
)00* +
;00+ ,
_postgresDbContext22 
.22 
Rattachments22 '
.22' (
Add22( +
(22+ ,
rattachment22, 7
)227 8
;228 9
await33 
_postgresDbContext33  
.33  !
SaveChangesAsync33! 1
(331 2
)332 3
;333 4
return55 
rattachment55 
;55 
}66 
public88 

async88 
Task88 
<88 
Rattachment88 !
>88! "
DeleteRattachment88# 4
(884 5
Rattachment885 @
rattachment88A L
)88L M
{99 
_postgresDbContext?? 
.?? 
Rattachments?? +
.??+ ,
Remove??, 2
(??2 3
rattachment??3 >
)??> ?
;??? @
await@@ 
_postgresDbContext@@ $
.@@$ %
SaveChangesAsync@@% 5
(@@5 6
)@@6 7
;@@7 8
returnBB 
rattachmentBB 
;BB 
returnEE 
nullEE 
;EE 
}FF 
}HH ﬁ
PC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Service\DeleteUserService.cs
	namespace 	
Service
 
. 
OAuth 
. 
Service 
;  
public 
class 
DeleteUserService 
:  
IDeleteUserService  2
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

DeleteUserService 
( 

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
DeleteLessonBook &
(& '
string' -
userId. 4
)4 5
{ 
var  
newDeleteUserRequest  
=! "
new# &
DeleteUserRequest' 8
{ 	
IdUser 
= 
userId 
, 
	IdSession 
= 
null 
} 	
;	 

var 
response 
= 
await 
_httpClient (
.( )
PostAsJsonAsync) 8
(8 9
$" 
{ 
_configuration 
[ 
$str .
]. /
}/ 0
$str0 O
"O P
,P Q 
newDeleteUserRequestR f
)f g
;g h
if 

( 
response 
. 
IsSuccessStatusCode (
)( )
{ 	
}!! 	
}"" 
public$$ 

async$$ 
Task$$ 
DeleteScheduler$$ %
($$% &
string$$& ,
userId$$- 3
)$$3 4
{%% 
var&&  
newDeleteUserRequest&&  
=&&! "
new&&# &
DeleteUserRequest&&' 8
{'' 	
IdUser(( 
=(( 
userId(( 
})) 	
;))	 

var++ 
respones++ 
=++ 
await++ 
_httpClient++ (
.++( )
PostAsJsonAsync++) 8
(++8 9
$",, 
{,, 
_configuration,, 
[,, 
$str,, .
],,. /
},,/ 0
$str,,0 N
",,N O
,,,O P 
newDeleteUserRequest,,Q e
),,e f
;,,f g
if.. 

(.. 
respones.. 
... 
IsSuccessStatusCode.. (
)..( )
{// 	
}11 	
}22 
}33 í:
RC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Repositories\AuthRepository.cs
	namespace 	
Service
 
. 
OAuth 
. 
Repositories $
;$ %
public 
class 
AuthRepository 
: 
IAuthRepository -
{		 
private

 
readonly

 
PostgresDbContext

 &
_postgresDbContext

' 9
;

9 :
public 

AuthRepository 
( 
PostgresDbContext +
postgresDbContext, =
)= >
{ 
_postgresDbContext 
= 
postgresDbContext .
;. /
} 
public 

async 
Task 
< 
UserProfile !
?! "
>" # 
GetOneUserByGoogleId$ 8
(8 9
string9 ?
googleId@ H
)H I
{ 
var 
user 
= 
await 
_postgresDbContext +
.+ ,
UserProfiles, 8
.8 9
FirstOrDefaultAsync9 L
(L M
u 
=> 
u 
. 
Id 
== 
googleId !
)! "
;" #
if 

( 
user 
!= 
null 
) 
{ 	
return 
user 
; 
} 	
return 
null 
; 
} 
public 

async 
Task 
< 
UserProfile !
>! "
AddUser# *
(* +
UserProfile+ 6
user7 ;
); <
{ 
var   
newUser   
=   
new   
UserProfile   %
{!! 	
Id"" 
="" 
user"" 
."" 
Id"" 
,"" 
IdRole## 
=## 
user## 
.## 
IdRole##  
,##  !
Email$$ 
=$$ 
user$$ 
.$$ 
Email$$ 
,$$ 
Name%% 
=%% 
user%% 
.%% 
Name%% 
,%% 
	GivenName&& 
=&& 
user&& 
.&& 
	GivenName&& &
,&&& '

FamilyName'' 
='' 
user'' 
.'' 

FamilyName'' (
,''( )
Picture(( 
=(( 
user(( 
.(( 
Picture(( "
,((" #
	CreatedAt)) 
=)) 
DateTime))  
.))  !
UtcNow))! '
,))' (
	UpdatedAt** 
=** 
DateTime**  
.**  !
UtcNow**! '
}++ 	
;++	 

_postgresDbContext-- 
.-- 
UserProfiles-- '
.--' (
Add--( +
(--+ ,
newUser--, 3
)--3 4
;--4 5
await.. 
_postgresDbContext..  
...  !
SaveChangesAsync..! 1
(..1 2
)..2 3
;..3 4
return// 
newUser// 
;// 
}00 
public22 

async22 
Task22 
<22 
UserProfile22 !
>22! "

UpdateUser22# -
(22- .
UserProfile22. 9
user22: >
)22> ?
{33 
_postgresDbContext44 
.44 
UserProfiles44 '
.44' (
Update44( .
(44. /
user44/ 3
)443 4
;444 5
await55 
_postgresDbContext55  
.55  !
SaveChangesAsync55! 1
(551 2
)552 3
;553 4
return77 
user77 
;77 
}88 
public:: 

async:: 
Task:: 
<:: 
bool:: 
>:: 
CheckIdRole:: '
(::' (
string::( .
idRole::/ 5
)::5 6
{;; 
var<< 
existingIdRole<< 
=<< 
await== 
_postgresDbContext== $
.==$ %
UserProfiles==% 1
.==1 2
FirstOrDefaultAsync==2 E
(==E F
u>> 
=>>> 
u>> 
.>> 
IdRole>> 
==>>  
idRole>>! '
)>>' (
;>>( )
if@@ 

(@@
 
existingIdRole@@ 
==@@ 
null@@ !
)@@! "
{AA 	
returnBB 
falseBB 
;BB 
}CC 	
returnDD 
trueDD 
;DD 
}EE 
publicGG 

asyncGG 
TaskGG 
<GG 
ListGG 
<GG 
RattachmentGG &
>GG& '
>GG' ("
GetRattachmentByIdRoleGG) ?
(GG? @
stringGG@ F
IdRoleGGG M
)GGM N
{HH 
varII 
rattachmentsII 
=II 
awaitII  
_postgresDbContextII! 3
.II3 4
RattachmentsII4 @
.JJ 
WhereJJ 
(JJ 
rJJ 
=>JJ 
rJJ 
.JJ 
IdDirecteurJJ %
==JJ& (
IdRoleJJ) /
||JJ0 2
rJJ3 4
.JJ4 5
IdProfesseurJJ5 A
==JJB D
IdRoleJJE K
)JJK L
.KK 
ToListAsyncKK 
(KK 
)KK 
;KK 
returnMM 
rattachmentsMM 
;MM 
}NN 
publicPP 

asyncPP 
TaskPP 
<PP 
UserProfilePP !
>PP! "

DeleteUserPP# -
(PP- .
UserProfilePP. 9
userPP: >
)PP> ?
{QQ 
_postgresDbContextRR 
.RR 
UserProfilesRR '
.RR' (
RemoveRR( .
(RR. /
userRR/ 3
)RR3 4
;RR4 5
awaitSS 
_postgresDbContextSS  
.SS  !
SaveChangesAsyncSS! 1
(SS1 2
)SS2 3
;SS3 4
returnUU 
userUU 
;UU 
}VV 
publicXX 

asyncXX 
TaskXX 
<XX 
ListXX 
<XX 
UserProfileXX &
>XX& '
>XX' (
GetUsersByIdRolesXX) :
(XX: ;
ListXX; ?
<XX? @
RattachmentXX@ K
>XXK L
listRattachmentsXXM ]
)XX] ^
{YY 
var[[ 
ids[[ 
=[[ 
listRattachments[[ "
.\\ 
Select\\ 
(\\ 
r\\ 
=>\\ 
r\\ 
.\\ 
IdProfesseur\\ '
)\\' (
.]] 
Distinct]] 
(]] 
)]] 
.^^ 
ToList^^ 
(^^ 
)^^ 
;^^ 
var`` 
userProfiles`` 
=`` 
await``  
_postgresDbContext``! 3
.``3 4
UserProfiles``4 @
.aa 
Whereaa 
(aa 
uaa 
=>aa 
idsaa 
.aa 
Containsaa $
(aa$ %
uaa% &
.aa& '
IdRoleaa' -
)aa- .
)aa. /
.bb 
ToListAsyncbb 
(bb 
)bb 
;bb 
returndd 
userProfilesdd 
;dd 
}ee 
}ff ÷2
>C:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Program.cs
var

 
builder

 
=

 
WebApplication

 
.

 
CreateBuilder

 *
(

* +
args

+ /
)

/ 0
;

0 1
builder 
. 
Logging 
. 
ClearProviders 
( 
)  
;  !
builder 
. 
Logging 
. 

AddConsole 
( 
) 
; 
builder 
. 
Services 
. 
AddDataProtection "
(" #
)# $
. #
PersistKeysToFileSystem 
( 
new  
DirectoryInfo! .
(. /
$str/ :
): ;
); <
. 
SetApplicationName 
( 
$str ,
), -
;- .
builder 
. 
Services 
. 
	AddScoped 
< 
IAuthRepository *
,* +
AuthRepository, :
>: ;
(; <
)< =
;= >
builder 
. 
Services 
. 
	AddScoped 
< 
ISessionRepository -
,- .
SessionRepository/ @
>@ A
(A B
)B C
;C D
builder 
. 
Services 
. 
	AddScoped 
< "
IRattachmentRepository 1
,1 2"
RattachementRepository3 I
>I J
(J K
)K L
;L M
builder 
. 
Services 
. 
	AddScoped 
< 
ICreateDataService -
,- .
CreateDataService/ @
>@ A
(A B
)B C
;C D
builder 
. 
Services 
. 
	AddScoped 
< '
IValidateGoogleTokenService 6
,6 7&
ValidateGoogleTokenService8 R
>R S
(S T
)T U
;U V
builder 
. 
Services 
. 
	AddScoped 
< 
IGenerateIdRole *
,* +
GenerateIdRole, :
>: ;
(; <
)< =
;= >
builder 
. 
Services 
. 
	AddScoped 
< #
IUserServiceRattachment 2
,2 3"
UserServiceRattachment4 J
>J K
(K L
)L M
;M N
builder 
. 
Services 
. 
	AddScoped 
< 
IDeleteUserService -
,- .
DeleteUserService/ @
>@ A
(A B
)B C
;C D
builder"" 
."" 
Services"" 
."" 
AddCors"" 
("" 
options""  
=>""! #
{## 
options$$ 
.$$ 
	AddPolicy$$ 
($$ 
$str$$  
,$$  !
policy$$" (
=>$$) +
{%% 
policy&& 
.&& 
AllowAnyOrigin&& 
(&& 
)&& 
.'' 
AllowAnyMethod'' 
('' 
)'' 
.(( 
AllowAnyHeader(( 
((( 
)(( 
;(( 
})) 
))) 
;)) 
}** 
)** 
;** 
builder-- 
.-- 
Services-- 
.-- 
AddAuthentication-- "
(--" #
options--# *
=>--+ -
{.. 
options// 
.// %
DefaultAuthenticateScheme// )
=//* +(
CookieAuthenticationDefaults//, H
.//H I 
AuthenticationScheme//I ]
;//] ^
}00 
)00 
.11 
	AddCookie11 
(11 
options11 
=>11 
{22 
options33 
.33 
	LoginPath33 
=33 
$str33 
;33  
options44 
.44 
Cookie44 
.44 
HttpOnly44 
=44  !
true44" &
;44& '
options55 
.55 
Cookie55 
.55 
SecurePolicy55 #
=55$ %
CookieSecurePolicy55& 8
.558 9
Always559 ?
;55? @
}66 
)66 
;66 
builder99 
.99 
Services99 
.99 
AddDbContext99 
<99 
PostgresDbContext99 /
>99/ 0
(990 1
options991 8
=>999 ;
options:: 
.:: 
	UseNpgsql:: 
(:: 
builder:: 
.:: 
Configuration:: +
[::+ ,
$str::, Q
]::Q R
)::R S
)::S T
;::T U
builder<< 
.<< 
Services<< 
.<< 
AddAuthorization<< !
(<<! "
)<<" #
;<<# $
builder>> 
.>> 
Services>> 
.>> 
AddControllers>> 
(>>  
)>>  !
;>>! "
builderAA 
.AA 
ServicesAA 
.AA 
AddRazorComponentsAA #
(AA# $
)AA$ %
;AA% &
builderCC 
.CC 
ServicesCC 
.CC 
AddHttpClientCC 
(CC 
)CC  
;CC  !
varEE 
appEE 
=EE 	
builderEE
 
.EE 
BuildEE 
(EE 
)EE 
;EE 
appGG 
.GG 
UseCorsGG 
(GG 
$strGG 
)GG 
;GG 
appHH 
.HH 
UseHttpsRedirectionHH 
(HH 
)HH 
;HH 
appJJ 
.JJ 

UseRoutingJJ 
(JJ 
)JJ 
;JJ 
appLL 
.LL 
UseAuthenticationLL 
(LL 
)LL 
;LL 
appMM 
.MM 
UseAuthorizationMM 
(MM 
)MM 
;MM 
appOO 
.OO 
MapControllersOO 
(OO 
)OO 
;OO 
appQQ 
.QQ 
RunQQ 
(QQ 
)QQ 	
;QQ	 
Ä
nC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Migrations\20250406153619_Ajout IdRole dans UserProfile.cs
	namespace 	
Service
 
. 
OAuth 
. 

Migrations "
{ 
public		 

partial		 
class		 &
AjoutIdRoledansUserProfile		 3
:		4 5
	Migration		6 ?
{

 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 
	AddColumn &
<& '
string' -
>- .
(. /
name 
: 
$str 
, 
table 
: 
$str %
,% &
type 
: 
$str .
,. /
	maxLength 
: 
$num 
, 
nullable 
: 
false 
,  
defaultValue 
: 
$str  
)  !
;! "
migrationBuilder 
. 
CreateTable (
(( )
name 
: 
$str $
,$ %
columns 
: 
table 
=> !
new" %
{ 
IdGuid 
= 
table "
." #
Column# )
<) *
Guid* .
>. /
(/ 0
type0 4
:4 5
$str6 <
,< =
nullable> F
:F G
falseH M
)M N
,N O
IdDirecteur 
=  !
table" '
.' (
Column( .
<. /
string/ 5
>5 6
(6 7
type7 ;
:; <
$str= C
,C D
nullableE M
:M N
falseO T
)T U
,U V
IdProfesseur  
=! "
table# (
.( )
Column) /
</ 0
string0 6
>6 7
(7 8
type8 <
:< =
$str> D
,D E
nullableF N
:N O
falseP U
)U V
} 
, 
constraints 
: 
table "
=># %
{ 
table   
.   

PrimaryKey   $
(  $ %
$str  % 6
,  6 7
x  8 9
=>  : <
x  = >
.  > ?
IdGuid  ? E
)  E F
;  F G
}!! 
)!! 
;!! 
}"" 	
	protected%% 
override%% 
void%% 
Down%%  $
(%%$ %
MigrationBuilder%%% 5
migrationBuilder%%6 F
)%%F G
{&& 	
migrationBuilder'' 
.'' 
	DropTable'' &
(''& '
name(( 
:(( 
$str(( $
)(($ %
;((% &
migrationBuilder** 
.** 

DropColumn** '
(**' (
name++ 
:++ 
$str++ 
,++ 
table,, 
:,, 
$str,, %
),,% &
;,,& '
}-- 	
}.. 
}// ≠
oC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Migrations\20250405193247_Ajout de Zone dans UserProfile.cs
	namespace 	
Service
 
. 
OAuth 
. 

Migrations "
{ 
public 

partial 
class &
AjoutdeZonedansUserProfile 3
:4 5
	Migration6 ?
{		 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 
	AddColumn &
<& '
string' -
>- .
(. /
name 
: 
$str 
, 
table 
: 
$str %
,% &
type 
: 
$str .
,. /
	maxLength 
: 
$num 
, 
nullable 
: 
false 
,  
defaultValue 
: 
$str  
)  !
;! "
migrationBuilder 
. 
AlterColumn (
<( )
string) /
>/ 0
(0 1
name 
: 
$str 
, 
table 
: 
$str %
,% &
type 
: 
$str 
, 
nullable 
: 
true 
, 

oldClrType 
: 
typeof "
(" #
string# )
)) *
,* +
oldType 
: 
$str 
)  
;  !
} 	
	protected 
override 
void 
Down  $
($ %
MigrationBuilder% 5
migrationBuilder6 F
)F G
{   	
migrationBuilder!! 
.!! 

DropColumn!! '
(!!' (
name"" 
:"" 
$str"" 
,"" 
table## 
:## 
$str## %
)##% &
;##& '
migrationBuilder%% 
.%% 
AlterColumn%% (
<%%( )
string%%) /
>%%/ 0
(%%0 1
name&& 
:&& 
$str&& 
,&& 
table'' 
:'' 
$str'' %
,''% &
type(( 
:(( 
$str(( 
,(( 
nullable)) 
:)) 
false)) 
,))  
defaultValue** 
:** 
$str**  
,**  !

oldClrType++ 
:++ 
typeof++ "
(++" #
string++# )
)++) *
,++* +
oldType,, 
:,, 
$str,, 
,,,  
oldNullable-- 
:-- 
true-- !
)--! "
;--" #
}.. 	
}// 
}00 ›
oC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Migrations\20250328180321_Ajout de Role dans UserProfile.cs
	namespace 	
Service
 
. 
OAuth 
. 

Migrations "
{ 
public 

partial 
class &
AjoutdeRoledansUserProfile 3
:4 5
	Migration6 ?
{		 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 
DropPrimaryKey +
(+ ,
name 
: 
$str &
,& '
table 
: 
$str $
)$ %
;% &
migrationBuilder 
. 
RenameTable (
(( )
name 
: 
$str #
,# $
newName 
: 
$str '
)' (
;( )
migrationBuilder 
. 
	AddColumn &
<& '
string' -
>- .
(. /
name 
: 
$str 
, 
table 
: 
$str %
,% &
type 
: 
$str .
,. /
	maxLength 
: 
$num 
, 
nullable 
: 
false 
,  
defaultValue 
: 
$str  
)  !
;! "
migrationBuilder 
. 
AddPrimaryKey *
(* +
name 
: 
$str '
,' (
table 
: 
$str %
,% &
column   
:   
$str   
)    
;    !
}!! 	
	protected$$ 
override$$ 
void$$ 
Down$$  $
($$$ %
MigrationBuilder$$% 5
migrationBuilder$$6 F
)$$F G
{%% 	
migrationBuilder&& 
.&& 
DropPrimaryKey&& +
(&&+ ,
name'' 
:'' 
$str'' '
,''' (
table(( 
:(( 
$str(( %
)((% &
;((& '
migrationBuilder** 
.** 

DropColumn** '
(**' (
name++ 
:++ 
$str++ 
,++ 
table,, 
:,, 
$str,, %
),,% &
;,,& '
migrationBuilder.. 
... 
RenameTable.. (
(..( )
name// 
:// 
$str// $
,//$ %
newName00 
:00 
$str00 &
)00& '
;00' (
migrationBuilder22 
.22 
AddPrimaryKey22 *
(22* +
name33 
:33 
$str33 &
,33& '
table44 
:44 
$str44 $
,44$ %
column55 
:55 
$str55 
)55  
;55  !
}66 	
}77 
}88 è
nC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Migrations\20250328172930_Ajout de la table SessionData.cs
	namespace 	
Service
 
. 
OAuth 
. 

Migrations "
{ 
public		 

partial		 
class		 %
AjoutdelatableSessionData		 2
:		3 4
	Migration		5 >
{

 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 
CreateTable (
(( )
name 
: 
$str #
,# $
columns 
: 
table 
=> !
new" %
{ 
Token 
= 
table !
.! "
Column" (
<( )
string) /
>/ 0
(0 1
type1 5
:5 6
$str7 =
,= >
nullable? G
:G H
falseI N
)N O
,O P
UserId 
= 
table "
." #
Column# )
<) *
string* 0
>0 1
(1 2
type2 6
:6 7
$str8 >
,> ?
nullable@ H
:H I
falseJ O
)O P
,P Q
Role 
= 
table  
.  !
Column! '
<' (
string( .
>. /
(/ 0
type0 4
:4 5
$str6 <
,< =
nullable> F
:F G
falseH M
)M N
,N O

Expiration 
=  
table! &
.& '
Column' -
<- .
DateTime. 6
>6 7
(7 8
type8 <
:< =
$str> X
,X Y
nullableZ b
:b c
falsed i
)i j
} 
, 
constraints 
: 
table "
=># %
{ 
table 
. 

PrimaryKey $
($ %
$str% 5
,5 6
x7 8
=>9 ;
x< =
.= >
Token> C
)C D
;D E
} 
) 
; 
} 	
	protected 
override 
void 
Down  $
($ %
MigrationBuilder% 5
migrationBuilder6 F
)F G
{ 	
migrationBuilder   
.   
	DropTable   &
(  & '
name!! 
:!! 
$str!! #
)!!# $
;!!$ %
}"" 	
}## 
}$$ Í#
XC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Migrations\20250323142048_Initial.cs
	namespace 	
Service
 
. 
OAuth 
. 

Migrations "
{ 
public		 

partial		 
class		 
Initial		  
:		! "
	Migration		# ,
{

 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 
CreateTable (
(( )
name 
: 
$str $
,$ %
columns 
: 
table 
=> !
new" %
{ 
Id 
= 
table 
. 
Column %
<% &
string& ,
>, -
(- .
type. 2
:2 3
$str4 L
,L M
	maxLengthN W
:W X
$numY \
,\ ]
nullable^ f
:f g
falseh m
)m n
,n o
Email 
= 
table !
.! "
Column" (
<( )
string) /
>/ 0
(0 1
type1 5
:5 6
$str7 O
,O P
	maxLengthQ Z
:Z [
$num\ _
,_ `
nullablea i
:i j
falsek p
)p q
,q r
Name 
= 
table  
.  !
Column! '
<' (
string( .
>. /
(/ 0
type0 4
:4 5
$str6 N
,N O
	maxLengthP Y
:Y Z
$num[ ^
,^ _
nullable` h
:h i
falsej o
)o p
,p q
	GivenName 
= 
table  %
.% &
Column& ,
<, -
string- 3
>3 4
(4 5
type5 9
:9 :
$str; S
,S T
	maxLengthU ^
:^ _
$num` c
,c d
nullablee m
:m n
falseo t
)t u
,u v

FamilyName 
=  
table! &
.& '
Column' -
<- .
string. 4
>4 5
(5 6
type6 :
:: ;
$str< T
,T U
	maxLengthV _
:_ `
$numa d
,d e
nullablef n
:n o
falsep u
)u v
,v w
Picture 
= 
table #
.# $
Column$ *
<* +
string+ 1
>1 2
(2 3
type3 7
:7 8
$str9 Q
,Q R
	maxLengthS \
:\ ]
$num^ a
,a b
nullablec k
:k l
falsem r
)r s
,s t
	CreatedAt 
= 
table  %
.% &
Column& ,
<, -
DateTime- 5
>5 6
(6 7
type7 ;
:; <
$str= W
,W X
	maxLengthY b
:b c
$numd g
,g h
nullablei q
:q r
trues w
)w x
,x y
	UpdatedAt 
= 
table  %
.% &
Column& ,
<, -
DateTime- 5
>5 6
(6 7
type7 ;
:; <
$str= W
,W X
	maxLengthY b
:b c
$numd g
,g h
nullablei q
:q r
trues w
)w x
} 
, 
constraints 
: 
table "
=># %
{ 
table 
. 

PrimaryKey $
($ %
$str% 6
,6 7
x8 9
=>: <
x= >
.> ?
Id? A
)A B
;B C
} 
) 
; 
} 	
	protected"" 
override"" 
void"" 
Down""  $
(""$ %
MigrationBuilder""% 5
migrationBuilder""6 F
)""F G
{## 	
migrationBuilder$$ 
.$$ 
	DropTable$$ &
($$& '
name%% 
:%% 
$str%% $
)%%$ %
;%%% &
}&& 	
}'' 
}(( Ñ

TC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Interfaces\ISessionRepository.cs
	namespace 	
Service
 
. 
OAuth 
. 

Interfaces "
;" #
public 
	interface 
ISessionRepository #
{ 
Task 
< 	
SessionData	 
> 
GetUserIdByCookies (
(( )
string) /
token0 5
)5 6
;6 7
Task 
< 	
SessionData	 
> 
SaveNewSession $
($ %
SessionData% 0
sessionData1 <
)< =
;= >
Task		 
<		 	
SessionData			 
>		 
DeleteSessionData		 '
(		' (
SessionData		( 3
sessionData		4 ?
)		? @
;		@ A
Task

 
<

 	
SessionData

	 
>

 
UpdateSession

 #
(

# $
SessionData

$ /
sessionData

0 ;
)

; <
;

< =
Task 
< 	
SessionData	 
> 
GetSessionByUserId (
(( )
string) /
userId0 6
)6 7
;7 8
} ®
XC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Interfaces\IRattachmentRepository.cs
	namespace 	
Service
 
. 
OAuth 
. 

Interfaces "
;" #
public 
	interface "
IRattachmentRepository '
{ 
Task 
< 	
List	 
< 
Rattachment 
> 
> 
GetRattachmentProf .
(. /
string/ 5

idRoleUser6 @
)@ A
;A B
Task 
< 	
List	 
< 
Rattachment 
> 
>  
GetRattachmentDirect 0
(0 1
string1 7

idRoleUser8 B
)B C
;C D
Task		 
<		 	
List			 
<		 
Rattachment		 
>		 
>		 
GetRattachment		 *
(		* +
Rattachment		+ 6
rattachment		7 B
)		B C
;		C D
Task

 
<

 	
Rattachment

	 
>

 
AddRattachment

 $
(

$ %
Rattachment

% 0
rattachment

1 <
)

< =
;

= >
Task 
< 	
Rattachment	 
> 
DeleteRattachment '
(' (
Rattachment( 3
rattachment4 ?
)? @
;@ A
} Ì
QC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Interfaces\IAuthRepository.cs
	namespace 	
Service
 
. 
OAuth 
. 

Interfaces "
;" #
public 
	interface 
IAuthRepository  
{ 
Task 
< 	
UserProfile	 
>  
GetOneUserByGoogleId *
(* +
string+ 1
googleId2 :
): ;
;; <
Task 
< 	
UserProfile	 
> 
AddUser 
( 
UserProfile )
user* .
). /
;/ 0
Task		 
<		 	
UserProfile			 
>		 

UpdateUser		  
(		  !
UserProfile		! ,
user		- 1
)		1 2
;		2 3
Task

 
<

 	
bool

	 
>

 
CheckIdRole

 
(

 
string

 !
idRole

" (
)

( )
;

) *
Task 
< 	
List	 
< 
Rattachment 
> 
> "
GetRattachmentByIdRole 2
(2 3
string3 9
idRole: @
)@ A
;A B
Task 
< 	
UserProfile	 
> 

DeleteUser  
(  !
UserProfile! ,
user- 1
)1 2
;2 3
Task 
< 	
List	 
< 
UserProfile 
> 
> 
GetUsersByIdRoles -
(- .
List. 2
<2 3
Rattachment3 >
>> ?
listRattachments@ P
)P Q
;Q R
} ‰1
QC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Database\PostgresDBContext.cs
	namespace 	
Service
 
. 
OAuth 
. 
Database  
;  !
public

 
class

 
PostgresDbContext

 
:

  
	DbContext

! *
{ 
public 

PostgresDbContext 
( 
DbContextOptions -
<- .
PostgresDbContext. ?
>? @
optionsA H
)H I
:J K
baseL P
(P Q
optionsQ X
)X Y
{ 
} 
public 

DbSet 
< 
UserProfile 
> 
UserProfiles *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
public 

DbSet 
< 
SessionData 
> 
SessionDatas *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
public 

DbSet 
< 
Rattachment 
> 
Rattachments *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
	protected 
override 
void 
OnModelCreating +
(+ ,
ModelBuilder, 8
modelBuilder9 E
)E F
{ 
base 
. 
OnModelCreating 
( 
modelBuilder )
)) *
;* +
modelBuilder 
. 
Entity 
< 
UserProfile '
>' (
(( )
entity) /
=>0 2
{ 	
entity 
. 
HasKey 
( 
a 
=> 
a  
.  !
Id! #
)# $
;$ %
entity 
. 
Property 
( 
a 
=>  
a! "
." #
IdRole# )
)) *
.* +

IsRequired+ 5
(5 6
)6 7
;7 8
entity 
. 
Property 
( 
a 
=>  
a! "
." #
Email# (
)( )
.) *

IsRequired* 4
(4 5
)5 6
;6 7
entity 
. 
Property 
( 
a 
=>  
a! "
." #
Name# '
)' (
.( )

IsRequired) 3
(3 4
)4 5
;5 6
entity 
. 
Property 
( 
a 
=>  
a! "
." #
Role# '
)' (
.( )

IsRequired) 3
(3 4
)4 5
;5 6
entity   
.   
Property   
(   
a   
=>    
a  ! "
.  " #
Zone  # '
)  ' (
.  ( )

IsRequired  ) 3
(  3 4
)  4 5
;  5 6
entity!! 
.!! 
Property!! 
(!! 
a!! 
=>!!  
a!!! "
.!!" #
	GivenName!!# ,
)!!, -
.!!- .

IsRequired!!. 8
(!!8 9
)!!9 :
;!!: ;
entity"" 
."" 
Property"" 
("" 
a"" 
=>""  
a""! "
.""" #

FamilyName""# -
)""- .
."". /

IsRequired""/ 9
(""9 :
)"": ;
;""; <
entity## 
.## 
Property## 
(## 
a## 
=>##  
a##! "
.##" #
Picture### *
)##* +
.##+ ,

IsRequired##, 6
(##6 7
)##7 8
;##8 9
entity$$ 
.$$ 
Property$$ 
($$ 
a$$ 
=>$$  
a$$! "
.$$" #
	CreatedAt$$# ,
)$$, -
;$$- .
entity%% 
.%% 
Property%% 
(%% 
a%% 
=>%%  
a%%! "
.%%" #
	UpdatedAt%%# ,
)%%, -
;%%- .
}&& 	
)&&	 

;&&
 
modelBuilder(( 
.(( 
Entity(( 
<(( 
SessionData(( '
>((' (
(((( )
entity(() /
=>((0 2
{)) 	
entity** 
.** 
HasKey** 
(** 
a** 
=>** 
a**  
.**  !
Token**! &
)**& '
;**' (
entity++ 
.++ 
Property++ 
(++ 
a++ 
=>++  
a++! "
.++" #
UserId++# )
)++) *
.++* +

IsRequired+++ 5
(++5 6
)++6 7
;++7 8
entity,, 
.,, 
Property,, 
(,, 
a,, 
=>,,  
a,,! "
.,," #
Role,,# '
),,' (
;,,( )
entity-- 
.-- 
Property-- 
(-- 
a-- 
=>--  
a--! "
.--" #

Expiration--# -
)--- .
;--. /
}.. 	
)..	 

;..
 
modelBuilder00 
.00 
Entity00 
<00 
Rattachment00 '
>00' (
(00( )
entity00) /
=>000 2
{11 	
entity22 
.22 
HasKey22 
(22 
a22 
=>22 
a22  
.22  !
IdGuid22! '
)22' (
;22( )
entity33 
.33 
Property33 
(33 
a33 
=>33  
a33! "
.33" #
IdDirecteur33# .
)33. /
.33/ 0

IsRequired330 :
(33: ;
)33; <
;33< =
entity44 
.44 
Property44 
(44 
a44 
=>44  
a44! "
.44" #
IdProfesseur44# /
)44/ 0
.440 1

IsRequired441 ;
(44; <
)44< =
;44= >
}55 	
)55	 

;55
 
}66 
}77 ô
SC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Controller\SessionController.cs
	namespace 	
Service
 
. 
OAuth 
. 

Controller "
;" #
[ 
ApiController 
] 
[		 
Route		 
(		 
$str		 
)		 
]		 
public

 
class

 
SessionController

 
:

  
ControllerBase

! /
{ 
private 
readonly 
ISessionRepository '
_sessionRepository( :
;: ;
public 

SessionController 
( 
ISessionRepository /
sessionRepository0 A
)A B
{ 
_sessionRepository 
= 
sessionRepository .
;. /
} 
[ 
HttpPost 
] 
[ 
Route 

(
 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
LogoutSession% 2
(2 3
[3 4
FromBody4 <
]< =
LogoutRequest> K
requestL S
)S T
{ 
var 
existingSession 
= 
await #
_sessionRepository$ 6
.6 7
GetUserIdByCookies7 I
(I J
requestJ Q
.Q R
	IdSessionR [
)[ \
;\ ]
if 

( 
existingSession 
!= 
null #
)# $
{ 	
await 
_sessionRepository $
.$ %
DeleteSessionData% 6
(6 7
existingSession7 F
)F G
;G H
return 
Ok 
( 
) 
; 
} 	
return 
NotFound 
( 
) 
; 
}   
["" 
HttpPost"" 
]"" 
[## 
Route## 

(##
 
$str## 
)## 
]## 
public$$ 

async$$ 
Task$$ 
<$$ 
IActionResult$$ #
>$$# $
GetUser$$% ,
($$, -
[$$- .
FromBody$$. 6
]$$6 7 
UserBySessionRequest$$8 L
request$$M T
)$$T U
{%% 
var&& 
existingUser&& 
=&& 
await&&  
_sessionRepository&&! 3
.&&3 4
GetUserIdByCookies&&4 F
(&&F G
request&&G N
.&&N O
	IdSession&&O X
)&&X Y
;&&Y Z
if(( 

((( 
existingUser(( 
!=(( 
null((  
)((  !
return((" (
Ok(() +
(((+ ,
existingUser((, 8
)((8 9
;((9 :
return** 
Unauthorized** 
(** 
)** 
;** 
}++ 
},, Üw
WC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Controller\RattachmentController.cs
	namespace 	
Service
 
. 
OAuth 
. 

Controller "
;" #
[ 
ApiController 
] 
[		 
Route		 
(		 
$str		 
)		 
]		 
public

 
class

 !
RattachmentController

 "
:

" #
ControllerBase

$ 2
{ 
private 
readonly 
ISessionRepository '
_sessionRepository( :
;: ;
private 
readonly 
IAuthRepository $
_authRepository% 4
;4 5
private 
readonly "
IRattachmentRepository +"
_rattachmentRepository, B
;B C
public 
!
RattachmentController  
(  !
ISessionRepository 
sessionRepository ,
,, -
IAuthRepository 
authRepository &
,& '"
IRattachmentRepository !
rattachmentRepository 4
)4 5
{ 
_sessionRepository 
= 
sessionRepository .
;. /
_authRepository 
= 
authRepository (
;( )"
_rattachmentRepository 
=  !
rattachmentRepository! 6
;6 7
} 
[ 
HttpPost 
] 
[ 
Route 

(
 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
AddRattachment% 3
(3 4
[4 5
FromBody5 =
]= >
RattachmentRequest? Q
requestR Y
)Y Z
{ 
var 
existingSession 
= 
await #
_sessionRepository$ 6
.6 7
GetUserIdByCookies7 I
(I J
requestJ Q
.Q R
	IdSessionR [
)[ \
;\ ]
if   

(   
existingSession   
!=   
null   #
)  # $
{!! 	
var"" 
user"" 
="" 
await"" 
_authRepository"" ,
."", - 
GetOneUserByGoogleId""- A
(""A B
existingSession""B Q
.""Q R
UserId""R X
)""X Y
;""Y Z
if$$ 
($$ 
user$$ 
!=$$ 
null$$ 
)$$ 
{%% 
if&& 
(&& 
!&& 
string&& 
.&& 
IsNullOrWhiteSpace&& .
(&&. /
request&&/ 6
.&&6 7
IdDirecteur&&7 B
)&&B C
)&&C D
{'' 
var)) 
rattachment)) #
=))$ %
new))& )
Rattachment))* 5
{** 
IdProfesseur++ $
=++% &
user++' +
.+++ ,
IdRole++, 2
,++2 3
IdDirecteur,, #
=,,$ %
request,,& -
.,,- .
IdDirecteur,,. 9
}-- 
;-- 
var00 
isexistingDirect00 (
=00) *
await00+ 0
_authRepository001 @
.00@ A
CheckIdRole00A L
(00L M
request00M T
.00T U
IdDirecteur00U `
)00` a
;00a b
if22 
(22 
!22 
isexistingDirect22 )
)22) *
{33 
return44 
Conflict44 '
(44' (
$str44( e
)44e f
;44f g
}55 
var88 
existingRattachment88 +
=88, -
await88. 3"
_rattachmentRepository884 J
.88J K
GetRattachment88K Y
(88Y Z
rattachment88Z e
)88e f
;88f g
if:: 
(:: 
existingRattachment:: +
.::+ ,
Count::, 1
>::2 3
$num::4 5
)::5 6
{;; 
return== 
Conflict== '
(==' (
$str==( R
)==R S
;==S T
}?? 
varAA 
addRattachmentAA &
=AA' (
awaitAA) ."
_rattachmentRepositoryAA/ E
.AAE F
AddRattachmentAAF T
(AAT U
rattachmentAAU `
)AA` a
;AAa b
ifCC 
(CC 
addRattachmentCC &
!=CC' )
nullCC* .
)CC. /
{DD 
varFF 
listRattachmentFF +
=FF, -
awaitFF. 3"
_rattachmentRepositoryFF4 J
.FFJ K 
GetRattachmentDirectFFK _
(FF_ `
userFF` d
.FFd e
IdRoleFFe k
)FFk l
;FFl m
returnHH 
OkHH !
(HH! "
listRattachmentHH" 1
)HH1 2
;HH2 3
}II 
}JJ 
ifLL 
(LL 
!LL 
stringLL 
.LL 
IsNullOrWhiteSpaceLL .
(LL. /
requestLL/ 6
.LL6 7
IdProfesseurLL7 C
)LLC D
)LLD E
{MM 
{PP 
varQQ 
rattachementQQ (
=QQ) *
newQQ+ .
RattachmentQQ/ :
{RR 
IdProfesseurSS (
=SS) *
requestSS+ 2
.SS2 3
IdProfesseurSS3 ?
,SS? @
IdDirecteurTT '
=TT( )
userTT* .
.TT. /
IdRoleTT/ 5
}UU 
;UU 
varXX 
isexistingProfXX *
=XX+ ,
awaitXX- 2
_authRepositoryXX3 B
.XXB C
CheckIdRoleXXC N
(XXN O
requestXXO V
.XXV W
IdProfesseurXXW c
)XXc d
;XXd e
ifZZ 
(ZZ 
!ZZ 
isexistingProfZZ +
)ZZ+ ,
{[[ 
return\\ "
Conflict\\# +
(\\+ ,
$str\\, b
)\\b c
;\\c d
}]] 
var`` 
existingRattachment`` /
=``0 1
await``2 7"
_rattachmentRepository``8 N
.``N O
GetRattachment``O ]
(``] ^
rattachement``^ j
)``j k
;``k l
ifbb 
(bb 
existingRattachmentbb /
.bb/ 0
Countbb0 5
>bb6 7
$numbb8 9
)bb9 :
{cc 
returnee "
Conflictee# +
(ee+ ,
$stree, V
)eeV W
;eeW X
}gg 
varii 
addRattachmentii *
=ii+ ,
awaitii- 2"
_rattachmentRepositoryii3 I
.iiI J
AddRattachmentiiJ X
(iiX Y
rattachementiiY e
)iie f
;iif g
ifkk 
(kk 
addRattachmentkk *
!=kk+ -
nullkk. 2
)kk2 3
{ll 
varmm 
listRattachmentmm  /
=mm0 1
awaitmm2 7"
_rattachmentRepositorymm8 N
.mmN O
GetRattachmentProfmmO a
(mma b
usermmb f
.mmf g
IdRolemmg m
)mmm n
;mmn o
returnoo "
Okoo# %
(oo% &
listRattachmentoo& 5
)oo5 6
;oo6 7
}pp 
}qq 
returnss 
Unauthorizedss '
(ss' (
)ss( )
;ss) *
}tt 
returnvv 
Unauthorizedvv #
(vv# $
)vv$ %
;vv% &
}ww 
elsexx 
{yy 
returnzz 
Unauthorizedzz #
(zz# $
)zz$ %
;zz% &
}{{ 
}|| 	
return}} 
Unauthorized}} 
(}} 
)}} 
;}} 
}~~ 
[
ÄÄ 
HttpPost
ÄÄ 
]
ÄÄ 
[
ÅÅ 
Route
ÅÅ 

(
ÅÅ
 
$str
ÅÅ 
)
ÅÅ  
]
ÅÅ  !
public
ÇÇ 

async
ÇÇ 
Task
ÇÇ 
<
ÇÇ 
IActionResult
ÇÇ #
>
ÇÇ# $
DeleteRattachment
ÇÇ% 6
(
ÇÇ6 7
[
ÇÇ7 8
FromBody
ÇÇ8 @
]
ÇÇ@ A 
RattachmentRequest
ÇÇB T
request
ÇÇU \
)
ÇÇ\ ]
{
ÉÉ 
var
ÑÑ 
existingSession
ÑÑ 
=
ÑÑ 
await
ÑÑ # 
_sessionRepository
ÑÑ$ 6
.
ÑÑ6 7 
GetUserIdByCookies
ÑÑ7 I
(
ÑÑI J
request
ÑÑJ Q
.
ÑÑQ R
	IdSession
ÑÑR [
)
ÑÑ[ \
;
ÑÑ\ ]
if
ÜÜ 

(
ÜÜ 
existingSession
ÜÜ 
!=
ÜÜ 
null
ÜÜ #
)
ÜÜ# $
{
áá 	
var
àà 
user
àà 
=
àà 
await
àà 
_authRepository
àà ,
.
àà, -"
GetOneUserByGoogleId
àà- A
(
ààA B
existingSession
ààB Q
.
ààQ R
UserId
ààR X
)
ààX Y
;
ààY Z
if
ää 
(
ää 
user
ää 
!=
ää 
null
ää 
)
ää 
{
ãã 
if
åå 
(
åå 
request
åå 
.
åå 
IdDirecteur
åå '
!=
åå( *
null
åå+ /
)
åå/ 0
{
çç 
var
éé 
rattachment
éé #
=
éé$ %
new
éé& )
Rattachment
éé* 5
{
èè 
IdProfesseur
êê $
=
êê% &
user
êê' +
.
êê+ ,
IdRole
êê, 2
,
êê2 3
IdDirecteur
ëë #
=
ëë$ %
request
ëë& -
.
ëë- .
IdDirecteur
ëë. 9
}
íí 
;
íí 
var
ïï !
existingRattachment
ïï +
=
ïï, -
await
ïï. 3$
_rattachmentRepository
ïï4 J
.
ïïJ K
GetRattachment
ïïK Y
(
ïïY Z
rattachment
ïïZ e
)
ïïe f
;
ïïf g
if
óó 
(
óó !
existingRattachment
óó +
.
óó+ ,
Count
óó, 1
==
óó2 4
$num
óó5 6
)
óó6 7
{
òò 
return
ôô 
Conflict
ôô '
(
ôô' (
$str
ôô( F
)
ôôF G
;
ôôG H
}
öö 
var
úú 
deleteRattachment
úú )
=
úú* +
await
úú, 1$
_rattachmentRepository
úú2 H
.
úúH I
DeleteRattachment
úúI Z
(
úúZ [
rattachment
úú[ f
)
úúf g
;
úúg h
if
ûû 
(
ûû 
deleteRattachment
ûû )
!=
ûû* ,
null
ûû- 1
)
ûû1 2
{
üü 
var
†† 
listRattachment
†† +
=
††, -
await
††. 3$
_rattachmentRepository
††4 J
.
††J K 
GetRattachmentProf
††K ]
(
††] ^
user
††^ b
.
††b c
IdRole
††c i
)
††i j
;
††j k
return
¢¢ 
Ok
¢¢ !
(
¢¢! "
listRattachment
¢¢" 1
)
¢¢1 2
;
¢¢2 3
}
££ 
}
§§ 
if
¶¶ 
(
¶¶ 
request
¶¶ 
.
¶¶ 
IdProfesseur
¶¶ (
!=
¶¶) +
null
¶¶, 0
)
¶¶0 1
{
ßß 
var
®® 
rattachement
®® $
=
®®% &
new
®®' *
Rattachment
®®+ 6
{
©© 
IdProfesseur
™™ $
=
™™% &
request
™™' .
.
™™. /
IdProfesseur
™™/ ;
,
™™; <
IdDirecteur
´´ #
=
´´$ %
user
´´& *
.
´´* +
IdRole
´´+ 1
}
¨¨ 
;
¨¨ 
var
ØØ !
existingRattachment
ØØ +
=
ØØ, -
await
ØØ. 3$
_rattachmentRepository
ØØ4 J
.
ØØJ K
GetRattachment
ØØK Y
(
ØØY Z
rattachement
ØØZ f
)
ØØf g
;
ØØg h
if
±± 
(
±± !
existingRattachment
±± +
.
±±+ ,
Count
±±, 1
==
±±2 4
$num
±±5 6
)
±±6 7
{
≤≤ 
return
≥≥ 
Conflict
≥≥ '
(
≥≥' (
$str
≥≥( F
)
≥≥F G
;
≥≥G H
}
¥¥ 
var
∂∂ 
deleteRattachment
∂∂ )
=
∂∂* +
await
∂∂, 1$
_rattachmentRepository
∂∂2 H
.
∂∂H I
DeleteRattachment
∂∂I Z
(
∂∂Z [!
existingRattachment
∂∂[ n
.
∂∂n o
FirstOrDefault
∂∂o }
(
∂∂} ~
)
∂∂~ 
)∂∂ Ä
;∂∂Ä Å
if
∏∏ 
(
∏∏ 
deleteRattachment
∏∏ )
!=
∏∏* ,
null
∏∏- 1
)
∏∏1 2
{
ππ 
var
∫∫ 
listRattachment
∫∫ +
=
∫∫, -
await
∫∫. 3$
_rattachmentRepository
∫∫4 J
.
∫∫J K 
GetRattachmentProf
∫∫K ]
(
∫∫] ^
user
∫∫^ b
.
∫∫b c
IdRole
∫∫c i
)
∫∫i j
;
∫∫j k
return
ºº 
Ok
ºº !
(
ºº! "
listRattachment
ºº" 1
)
ºº1 2
;
ºº2 3
}
ΩΩ 
}
ææ 
return
¿¿ 
Unauthorized
¿¿ #
(
¿¿# $
)
¿¿$ %
;
¿¿% &
}
¡¡ 
return
√√ 
Unauthorized
√√ 
(
√√  
)
√√  !
;
√√! "
}
ƒƒ 	
return
∆∆ 
Unauthorized
∆∆ 
(
∆∆ 
)
∆∆ 
;
∆∆ 
}
«« 
[
…… 
HttpPost
…… 
]
…… 
[
   
Route
   

(
  
 
$str
   #
)
  # $
]
  $ %
public
ÀÀ 

async
ÀÀ 
Task
ÀÀ 
<
ÀÀ 
IActionResult
ÀÀ #
>
ÀÀ# $
GetRattachments
ÀÀ% 4
(
ÀÀ4 5
[
ÀÀ5 6
FromBody
ÀÀ6 >
]
ÀÀ> ?"
ViewDashboardRequest
ÀÀ@ T
request
ÀÀU \
)
ÀÀ\ ]
{
ÃÃ 
var
ŒŒ 
existingSession
ŒŒ 
=
ŒŒ 
await
œœ  
_sessionRepository
œœ $
.
œœ$ % 
GetUserIdByCookies
œœ% 7
(
œœ7 8
request
œœ8 ?
.
œœ? @
	IdSession
œœ@ I
)
œœI J
;
œœJ K
if
—— 

(
—— 
existingSession
—— 
==
—— 
null
—— #
)
——# $
return
——% +
Unauthorized
——, 8
(
——8 9
)
——9 :
;
——: ;
var
‘‘ 
rattachmentsInfos
‘‘ 
=
‘‘ 
await
‘‘  %
_authRepository
‘‘& 5
.
‘‘5 6
GetUsersByIdRoles
‘‘6 G
(
‘‘G H
request
‘‘H O
.
‘‘O P
AsDirecteur
‘‘P [
)
‘‘[ \
;
‘‘\ ]
if
÷÷ 

(
÷÷ 
rattachmentsInfos
÷÷ 
==
÷÷  
null
÷÷! %
)
÷÷% &
return
÷÷' -

BadRequest
÷÷. 8
(
÷÷8 9
)
÷÷9 :
;
÷÷: ;
return
ÿÿ 
Ok
ÿÿ 
(
ÿÿ 
rattachmentsInfos
ÿÿ #
)
ÿÿ# $
;
ÿÿ$ %
}
ŸŸ 
}⁄⁄ ©∑
PC:\Users\bapti\RiderProjects\MaClasse\Service.OAuth\Controller\AuthController.cs
	namespace		 	
Service		
 
.		 
OAuth		 
.		 

Controller		 "
;		" #
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
class 
AuthController 
: 
ControllerBase +
{ 
private 
readonly 
IConfiguration #
_configuration$ 2
;2 3
private 
readonly '
IValidateGoogleTokenService 0'
_validateGoogleTokenService1 L
;L M
private 
readonly 
IAuthRepository $
_authRepository% 4
;4 5
private 
readonly 
ISessionRepository '
_sessionRepository( :
;: ;
private 
readonly #
IUserServiceRattachment ,#
_userServiceRattachment- D
;D E
private 
readonly 
IGenerateIdRole $
_generateIdRole% 4
;4 5
private 
readonly 
ICreateDataService '
_createDataService( :
;: ;
private 
readonly 
IDeleteUserService '
_deleteUserService( :
;: ;
public 

AuthController 
( 
IConfiguration 
configuration $
,$ %'
IValidateGoogleTokenService #&
validateGoogleTokenService$ >
,> ?
IAuthRepository 
authRepository &
,& '
ISessionRepository 
sessionRepository ,
,, -#
IUserServiceRattachment "
userServiceRattachment  6
,6 7
IGenerateIdRole 
generateIdRole &
,& '
ICreateDataService 
createDataService ,
,, -
IDeleteUserService   
deleteUserService   ,
)  , -
{!! 
_configuration"" 
="" 
configuration"" &
;""& ''
_validateGoogleTokenService## #
=##$ %&
validateGoogleTokenService##& @
;##@ A
_authRepository$$ 
=$$ 
authRepository$$ (
;$$( )
_sessionRepository%% 
=%% 
sessionRepository%% .
;%%. /#
_userServiceRattachment&& 
=&&  !"
userServiceRattachment&&" 8
;&&8 9
_generateIdRole'' 
='' 
generateIdRole'' (
;''( )
_createDataService(( 
=(( 
createDataService(( .
;((. /
_deleteUserService)) 
=)) 
deleteUserService)) .
;)). /
}** 
private,, 

AuthReturn,, 
_returnResponse,, &
=,,' (
new,,) ,
(,,, -
),,- .
;,,. /
private-- 
	Scheduler-- 
newScheduler-- "
=--# $
new--% (
(--( )
)--) *
;--* +
private.. 

LessonBook.. 
newLessonBook.. $
=..% &
new..' *
(..* +
)..+ ,
;.., -
[00 
HttpPost00 
]00 
[11 
Route11 

(11
 
$str11 
)11 
]11 
public22 

async22 
Task22 
<22 
IActionResult22 #
>22# $
GoogleLogin22% 0
(220 1
GoogleTokenRequest221 C
request22D K
)22K L
{33 
var44 
payload44 
=44 
await44 '
_validateGoogleTokenService44 7
.447 8
ValidateGoogleToken448 K
(44K L
request44L S
.44S T
Token44T Y
)44Y Z
;44Z [
if55 

(55 
payload55 
==55 
null55 
)55 
{66 	
return77 
Unauthorized77 
(77  
$str77  1
)771 2
;772 3
}88 	
var:: 
user:: 
=:: 
new:: 
UserProfile:: "
{;; 	
Id<< 
=<< 
payload<< 
.<< 
Subject<<  
,<<  !
Email== 
=== 
payload== 
.== 
Email== !
,==! "
Name>> 
=>> 
payload>> 
.>> 
Name>> 
,>>  
	GivenName?? 
=?? 
payload?? 
.??  
	GivenName??  )
,??) *

FamilyName@@ 
=@@ 
payload@@  
.@@  !

FamilyName@@! +
,@@+ ,
PictureAA 
=AA 
payloadAA 
.AA 
PictureAA %
}BB 	
;BB	 

varEE 
existingUserEE 
=EE 
awaitEE  
_authRepositoryEE! 0
.EE0 1 
GetOneUserByGoogleIdEE1 E
(EEE F
userEEF J
.EEJ K
IdEEK M
)EEM N
;EEN O
ifGG 

(GG 
existingUserGG 
!=GG 
nullGG  
)GG  !
{HH 	
varJJ 
alreadySessionJJ 
=JJ  
awaitJJ! &
_sessionRepositoryJJ' 9
.JJ9 :
GetSessionByUserIdJJ: L
(JJL M
userJJM Q
.JJQ R
IdJJR T
)JJT U
;JJU V
ifLL 
(LL 
alreadySessionLL 
!=LL !
nullLL" &
)LL& '
{MM 
varOO 
deleteSessionOO !
=OO" #
awaitOO$ )
_sessionRepositoryOO* <
.OO< =
DeleteSessionDataOO= N
(OON O
alreadySessionOOO ]
)OO] ^
;OO^ _
ifQQ 
(QQ 
deleteSessionQQ !
==QQ" $
nullQQ% )
)QQ) *
returnQQ+ 1
UnauthorizedQQ2 >
(QQ> ?
)QQ? @
;QQ@ A
}RR 
varUU 
sessionTokenLoginUU !
=UU" #
GuidUU$ (
.UU( )
NewGuidUU) 0
(UU0 1
)UU1 2
.UU2 3
ToStringUU3 ;
(UU; <
)UU< =
;UU= >
varWW  
newSessionTokenLoginWW $
=WW% &
newWW' *
SessionDataWW+ 6
{XX 
TokenYY 
=YY 
sessionTokenLoginYY )
,YY) *
UserIdZZ 
=ZZ 
existingUserZZ %
.ZZ% &
IdZZ& (
,ZZ( )
Role[[ 
=[[ 
existingUser[[ #
.[[# $
Role[[$ (
,[[( )

Expiration\\ 
=\\ 
DateTime\\ %
.\\% &
UtcNow\\& ,
.\\, -
AddHours\\- 5
(\\5 6
$num\\6 7
)\\7 8
}]] 
;]] 
var`` 
sessionSaveLogin``  
=``! "
await``# (
_sessionRepository``) ;
.``; <
SaveNewSession``< J
(``J K 
newSessionTokenLogin``K _
)``_ `
;``` a
newSchedulerdd 
=dd 
awaitdd  
_createDataServicedd! 3
.dd3 4
GetDataSchedulerdd4 D
(ddD E
existingUserddE Q
.ddQ R
IdddR T
)ddT U
;ddU V
ifff 
(ff 
sessionSaveLoginff  
!=ff! #
nullff$ (
)ff( )
{gg 
_returnResponseii 
=ii  !
awaitii" '#
_userServiceRattachmentii( ?
.ii? @"
GetUserWithRattachmentii@ V
(iiV W
existingUserjj  
,jj  !
falsejj" '
,jj' (
sessionSaveLoginjj) 9
.jj9 :
Tokenjj: ?
,jj? @
requestjjA H
.jjH I
TokenjjI N
,jjN O
newSchedulerjjP \
)jj] ^
;jj^ _
returnll 
Okll 
(ll 
_returnResponsell )
)ll) *
;ll* +
}mm 
returnoo 
Unauthorizedoo 
(oo  
)oo  !
;oo! "
}pp 	
varuu 
idRoleuu 
=uu 
awaituu 
_generateIdRoleuu *
.uu* +
GenerateIdAsyncuu+ :
(uu: ;
)uu; <
;uu< =
userww 
.ww 
IdRoleww 
=ww 
idRoleww 
;ww 
varyy 
newUseryy 
=yy 
awaityy 
_authRepositoryyy +
.yy+ ,
AddUseryy, 3
(yy3 4
useryy4 8
)yy8 9
;yy9 :
newScheduler~~ 
=~~ 
await~~ 
_createDataService~~ /
.~~/ 0
CreateDataScheduler~~0 C
(~~C D
newUser~~D K
.~~K L
Id~~L N
)~~N O
;~~O P
newLessonBook 
= 
await 
_createDataService 0
.0 1 
CreateDateLessonBook1 E
(E F
newUserF M
.M N
IdN P
)P Q
;Q R
var
ÇÇ  
sessionTokenSignup
ÇÇ 
=
ÇÇ  
Guid
ÇÇ! %
.
ÇÇ% &
NewGuid
ÇÇ& -
(
ÇÇ- .
)
ÇÇ. /
.
ÇÇ/ 0
ToString
ÇÇ0 8
(
ÇÇ8 9
)
ÇÇ9 :
;
ÇÇ: ;
var
ÑÑ #
newSessionTokenSignup
ÑÑ !
=
ÑÑ" #
new
ÑÑ$ '
SessionData
ÑÑ( 3
{
ÖÖ 	
Token
ÜÜ 
=
ÜÜ  
sessionTokenSignup
ÜÜ &
,
ÜÜ& '
UserId
áá 
=
áá 
newUser
áá 
.
áá 
Id
áá 
,
áá  
Role
àà 
=
àà 
$str
àà 
,
àà 

Expiration
ââ 
=
ââ 
DateTime
ââ !
.
ââ! "
UtcNow
ââ" (
.
ââ( )
AddHours
ââ) 1
(
ââ1 2
$num
ââ2 3
)
ââ3 4
}
ää 	
;
ää	 

var
çç 
sessionSaveSignup
çç 
=
çç 
await
çç  % 
_sessionRepository
çç& 8
.
çç8 9
SaveNewSession
çç9 G
(
ççG H#
newSessionTokenSignup
ççH ]
)
çç] ^
;
çç^ _
if
èè 

(
èè 
sessionSaveSignup
èè 
!=
èè  
null
èè! %
)
èè% &
{
êê 	
_returnResponse
ëë 
=
ëë 
await
ëë #%
_userServiceRattachment
ëë$ ;
.
ëë; <$
GetUserWithRattachment
ëë< R
(
ëëR S
newUser
íí 
,
íí 
true
íí 
,
íí 
sessionSaveSignup
íí 0
.
íí0 1
Token
íí1 6
,
íí6 7
request
íí8 ?
.
íí? @
Token
íí@ E
,
ííE F
newScheduler
ííG S
)
ííS T
;
ííT U
return
îî 
Ok
îî 
(
îî 
_returnResponse
îî %
)
îî% &
;
îî& '
}
ïï 	
return
óó 
Unauthorized
óó 
(
óó 
)
óó 
;
óó 
}
ôô 
[
õõ 
HttpPost
õõ 
]
õõ 
[
úú 
Route
úú 

(
úú
 
$str
úú 
)
úú 
]
úú 
public
ùù 

async
ùù 
Task
ùù 
<
ùù 
IActionResult
ùù #
>
ùù# $
FinishedSignUp
ùù% 3
(
ùù3 4
[
ùù4 5
FromBody
ùù5 =
]
ùù= > 
SignupDialogResult
ùù? Q
result
ùùR X
)
ùùX Y
{
ûû 
var
†† 
userSession
†† 
=
†† 
await
††  
_sessionRepository
††  2
.
††2 3 
GetUserIdByCookies
††3 E
(
††E F
result
††F L
.
††L M
	IdSession
††M V
)
††V W
;
††W X
userSession
¢¢ 
.
¢¢ 
Role
¢¢ 
=
¢¢ 
result
¢¢ !
.
¢¢! "
Role
¢¢" &
;
¢¢& '
var
•• 
updateSession
•• 
=
•• 
await
•• ! 
_sessionRepository
••" 4
.
••4 5
UpdateSession
••5 B
(
••B C
userSession
••C N
)
••N O
;
••O P
if
ßß 

(
ßß 
updateSession
ßß 
==
ßß 
null
ßß !
)
ßß! "
return
ßß# )
Unauthorized
ßß* 6
(
ßß6 7
)
ßß7 8
;
ßß8 9
var
™™ 

updateUser
™™ 
=
™™ 
await
™™ 
_authRepository
™™ .
.
™™. /"
GetOneUserByGoogleId
™™/ C
(
™™C D
updateSession
™™D Q
.
™™Q R
UserId
™™R X
)
™™X Y
;
™™Y Z

updateUser
¨¨ 
.
¨¨ 
Role
¨¨ 
=
¨¨ 
updateSession
¨¨ '
.
¨¨' (
Role
¨¨( ,
;
¨¨, -

updateUser
≠≠ 
.
≠≠ 
Zone
≠≠ 
=
≠≠ 
result
≠≠  
.
≠≠  !
Zone
≠≠! %
;
≠≠% &

updateUser
ÆÆ 
.
ÆÆ 
	UpdatedAt
ÆÆ 
=
ÆÆ 
DateTime
ÆÆ '
.
ÆÆ' (
UtcNow
ÆÆ( .
;
ÆÆ. /
var
±± 
updatedUser
±± 
=
±± 
await
±± 
_authRepository
±±  /
.
±±/ 0

UpdateUser
±±0 :
(
±±: ;

updateUser
±±; E
)
±±E F
;
±±F G
newScheduler
µµ 
=
µµ 
await
µµ  
_createDataService
µµ /
.
µµ/ 0#
AddHolidayToScheduler
µµ0 E
(
µµE F
updatedUser
µµF Q
)
µµQ R
;
µµR S
if
∑∑ 

(
∑∑ 
updatedUser
∑∑ 
!=
∑∑ 
null
∑∑ 
&&
∑∑  "
newScheduler
∑∑# /
!=
∑∑0 2
null
∑∑3 7
)
∑∑7 8
{
∏∏ 	
_returnResponse
ππ 
=
ππ 
await
ππ #%
_userServiceRattachment
ππ$ ;
.
ππ; <$
GetUserWithRattachment
ππ< R
(
ππR S
updatedUser
∫∫ 
,
∫∫ 
false
∫∫ "
,
∫∫" #
userSession
∫∫$ /
.
∫∫/ 0
Token
∫∫0 5
,
∫∫5 6
result
∫∫7 =
.
∫∫= >
AccessToken
∫∫> I
,
∫∫I J
newScheduler
∫∫K W
)
∫∫W X
;
∫∫X Y
return
ºº 
Ok
ºº 
(
ºº 
_returnResponse
ºº %
)
ºº% &
;
ºº& '
}
ΩΩ 	
return
øø 
Unauthorized
øø 
(
øø 
)
øø 
;
øø 
}
¿¿ 
[
¬¬ 
HttpPost
¬¬ 
]
¬¬ 
[
√√ 
Route
√√ 

(
√√
 
$str
√√ 
)
√√ 
]
√√ 
public
ƒƒ 

async
ƒƒ 
Task
ƒƒ 
<
ƒƒ 
IActionResult
ƒƒ #
>
ƒƒ# $
GetUser
ƒƒ% ,
(
ƒƒ, -
[
ƒƒ- .
FromBody
ƒƒ. 6
]
ƒƒ6 7 
GoogleTokenRequest
ƒƒ8 J
request
ƒƒK R
)
ƒƒR S
{
≈≈ 
var
«« 
userSession
«« 
=
«« 
await
««  
_sessionRepository
««  2
.
««2 3 
GetUserIdByCookies
««3 E
(
««E F
request
««F M
.
««M N
Token
««N S
)
««S T
;
««T U
if
…… 

(
…… 
userSession
…… 
==
…… 
null
…… 
)
……  
return
……! '
Unauthorized
……( 4
(
……4 5
)
……5 6
;
……6 7
if
ÃÃ 

(
ÃÃ
 
userSession
ÃÃ 
.
ÃÃ 

Expiration
ÃÃ !
<
ÃÃ" #
DateTime
ÃÃ$ ,
.
ÃÃ, -
UtcNow
ÃÃ- 3
)
ÃÃ3 4
{
ÕÕ 	
return
ŒŒ 
Unauthorized
ŒŒ 
(
ŒŒ  
)
ŒŒ  !
;
ŒŒ! "
}
œœ 	
var
““ 
user
““ 
=
““ 
await
““ 
_authRepository
““ (
.
““( )"
GetOneUserByGoogleId
““) =
(
““= >
userSession
““> I
.
““I J
UserId
““J P
)
““P Q
;
““Q R
if
‘‘ 

(
‘‘ 
user
‘‘ 
==
‘‘ 
null
‘‘ 
)
‘‘ 
return
‘‘  
Unauthorized
‘‘! -
(
‘‘- .
)
‘‘. /
;
‘‘/ 0
newScheduler
÷÷ 
=
÷÷ 
await
÷÷  
_createDataService
÷÷ /
.
÷÷/ 0
GetDataScheduler
÷÷0 @
(
÷÷@ A
user
÷÷A E
.
÷÷E F
Id
÷÷F H
)
÷÷H I
;
÷÷I J
_returnResponse
ÿÿ 
=
ÿÿ 
await
ÿÿ %
_userServiceRattachment
ÿÿ  7
.
ÿÿ7 8$
GetUserWithRattachment
ÿÿ8 N
(
ÿÿN O
user
ŸŸ 
,
ŸŸ 
false
ŸŸ 
,
ŸŸ 
userSession
ŸŸ $
.
ŸŸ$ %
Token
ŸŸ% *
,
ŸŸ* +
request
ŸŸ, 3
.
ŸŸ3 4
Token
ŸŸ4 9
,
ŸŸ9 :
newScheduler
ŸŸ; G
)
ŸŸG H
;
ŸŸH I
return
€€ 
Ok
€€ 
(
€€ 
_returnResponse
€€ !
)
€€! "
;
€€" #
}
‹‹ 
[
ﬁﬁ 
HttpPost
ﬁﬁ 
]
ﬁﬁ 
[
ﬂﬂ 
Route
ﬂﬂ 

(
ﬂﬂ
 
$str
ﬂﬂ 
)
ﬂﬂ 
]
ﬂﬂ 
public
‡‡ 

async
‡‡ 
Task
‡‡ 
<
‡‡ 
IActionResult
‡‡ #
>
‡‡# $

ChangeRole
‡‡% /
(
‡‡/ 0
[
‡‡0 1
FromBody
‡‡1 9
]
‡‡9 :!
ChangeProfilRequest
‡‡; N
request
‡‡O V
)
‡‡V W
{
·· 
var
‚‚ 
existingSession
‚‚ 
=
‚‚ 
await
‚‚ # 
_sessionRepository
‚‚$ 6
.
‚‚6 7 
GetUserIdByCookies
‚‚7 I
(
‚‚I J
request
‚‚J Q
.
‚‚Q R
	IdSession
‚‚R [
)
‚‚[ \
;
‚‚\ ]
if
‰‰ 

(
‰‰ 
existingSession
‰‰ 
!=
‰‰ 
null
‰‰ #
)
‰‰# $
{
ÂÂ 	
var
ÊÊ 
user
ÊÊ 
=
ÊÊ 
await
ÊÊ 
_authRepository
ÊÊ ,
.
ÊÊ, -"
GetOneUserByGoogleId
ÊÊ- A
(
ÊÊA B
existingSession
ÊÊB Q
.
ÊÊQ R
UserId
ÊÊR X
)
ÊÊX Y
;
ÊÊY Z
if
ËË 
(
ËË 
user
ËË 
!=
ËË 
null
ËË 
)
ËË 
{
ÈÈ 
user
ÍÍ 
.
ÍÍ 
Zone
ÍÍ 
=
ÍÍ 
request
ÍÍ #
.
ÍÍ# $
Zone
ÍÍ$ (
;
ÍÍ( )
user
ÎÎ 
.
ÎÎ 
Role
ÎÎ 
=
ÎÎ 
request
ÎÎ #
.
ÎÎ# $
Role
ÎÎ$ (
;
ÎÎ( )
user
ÏÏ 
.
ÏÏ 
	UpdatedAt
ÏÏ 
=
ÏÏ  
DateTime
ÏÏ! )
.
ÏÏ) *
UtcNow
ÏÏ* 0
;
ÏÏ0 1
var
ÌÌ 
updatedUser
ÌÌ 
=
ÌÌ  !
await
ÌÌ" '
_authRepository
ÌÌ( 7
.
ÌÌ7 8

UpdateUser
ÌÌ8 B
(
ÌÌB C
user
ÌÌC G
)
ÌÌG H
;
ÌÌH I
if
ÔÔ 
(
ÔÔ 
updatedUser
ÔÔ 
!=
ÔÔ  "
null
ÔÔ# '
)
ÔÔ' (
{
 
return
ÒÒ 
Ok
ÒÒ 
(
ÒÒ 
updatedUser
ÒÒ )
)
ÒÒ) *
;
ÒÒ* +
}
ÚÚ 
return
ÙÙ 
Unauthorized
ÙÙ #
(
ÙÙ# $
)
ÙÙ$ %
;
ÙÙ% &
}
ıı 
return
˜˜ 
Unauthorized
˜˜ 
(
˜˜  
)
˜˜  !
;
˜˜! "
}
¯¯ 	
else
˘˘ 
{
˙˙ 	
return
˚˚ 
Unauthorized
˚˚ 
(
˚˚  
)
˚˚  !
;
˚˚! "
}
¸¸ 	
}
˝˝ 
[
ˇˇ 
HttpPost
ˇˇ 
]
ˇˇ 
[
ÄÄ 
Route
ÄÄ 

(
ÄÄ
 
$str
ÄÄ 
)
ÄÄ 
]
ÄÄ 
public
ÅÅ 

async
ÅÅ 
Task
ÅÅ 
<
ÅÅ 
IActionResult
ÅÅ #
>
ÅÅ# $

DeleteUser
ÅÅ% /
(
ÅÅ/ 0
[
ÅÅ0 1
FromBody
ÅÅ1 9
]
ÅÅ9 :
DeleteUserRequest
ÅÅ; L
request
ÅÅM T
)
ÅÅT U
{
ÇÇ 
var
ÉÉ 
existingSession
ÉÉ 
=
ÉÉ 
await
ÉÉ # 
_sessionRepository
ÉÉ$ 6
.
ÉÉ6 7 
GetUserIdByCookies
ÉÉ7 I
(
ÉÉI J
request
ÉÉJ Q
.
ÉÉQ R
	IdSession
ÉÉR [
)
ÉÉ[ \
;
ÉÉ\ ]
if
ÖÖ 

(
ÖÖ 
existingSession
ÖÖ 
!=
ÖÖ 
null
ÖÖ #
)
ÖÖ# $
{
ÜÜ 	
var
áá 
user
áá 
=
áá 
await
áá 
_authRepository
áá ,
.
áá, -"
GetOneUserByGoogleId
áá- A
(
ááA B
existingSession
ááB Q
.
ááQ R
UserId
ááR X
)
ááX Y
;
ááY Z
var
ää 
deleteSession
ää 
=
ää 
await
ää  % 
_sessionRepository
ää& 8
.
ää8 9
DeleteSessionData
ää9 J
(
ääJ K
existingSession
ääK Z
)
ääZ [
;
ää[ \
if
åå 
(
åå 
deleteSession
åå 
==
åå  
null
åå! %
)
åå% &
return
åå' -
Unauthorized
åå. :
(
åå: ;
)
åå; <
;
åå< =
if
éé 
(
éé 
user
éé 
!=
éé 
null
éé 
)
éé 
{
èè 
var
ëë 
deletedUser
ëë 
=
ëë  !
await
ëë" '
_authRepository
ëë( 7
.
ëë7 8

DeleteUser
ëë8 B
(
ëëB C
user
ëëC G
)
ëëG H
;
ëëH I
await
îî  
_deleteUserService
îî (
.
îî( )
DeleteLessonBook
îî) 9
(
îî9 :
user
îî: >
.
îî> ?
Id
îî? A
)
îîA B
;
îîB C
await
ïï  
_deleteUserService
ïï (
.
ïï( )
DeleteScheduler
ïï) 8
(
ïï8 9
user
ïï9 =
.
ïï= >
Id
ïï> @
)
ïï@ A
;
ïïA B
if
óó 
(
óó 
deletedUser
óó 
!=
óó  "
null
óó# '
)
óó' (
{
òò 
return
ôô 
Ok
ôô 
(
ôô 
deletedUser
ôô )
)
ôô) *
;
ôô* +
}
öö 
return
úú 
NotFound
úú 
(
úú  
)
úú  !
;
úú! "
}
ùù 
return
üü 
Unauthorized
üü 
(
üü  
)
üü  !
;
üü! "
}
†† 	
else
°° 
{
¢¢ 	
return
££ 
Unauthorized
££ 
(
££  
)
££  !
;
££! "
}
§§ 	
}
•• 
}¶¶ 