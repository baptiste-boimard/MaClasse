Ù
XC:\Users\bapti\RiderProjects\MaClasse\Service.Cloudinary\Services\VerifyDeleteService.cs
	namespace 	
Service
 
. 
Database 
. 
Services #
;# $
public 
class 
VerifyDeleteService  
{ 
private 	
readonly
 

HttpClient 
_httpClient )
;) *
private		 	
readonly		
 
IConfiguration		 !
_configuration		" 0
;		0 1
private

 	
readonly


 
UserCloudService

 #
_userCloudService

$ 5
;

5 6
public 
VerifyDeleteService	 
( 

HttpClient 

httpClient 
, 
IConfiguration 
configuration  
,  !
UserCloudService 
userCloudService %
)% &
{ 
_httpClient 
= 

httpClient 
; 
_configuration 
= 
configuration "
;" #
_userCloudService 
= 
userCloudService (
;( )
} 
public 
async	 
Task 
< 
List 
< 
Document !
>! "
>" #
VerifyDeleteFiles$ 5
(5 6
RequestLesson6 C
requestD K
)K L
{ 
List 
< 	
Document	 
> 
documentsToDelete $
=% &
new' *
List+ /
</ 0
Document0 8
>8 9
(9 :
): ;
;; <
foreach 
( 
var 
document 
in 
request $
.$ %
Lesson% +
.+ ,
	Documents, 5
)5 6
{ 
var 	
documentRequest
 
= 
new 
RequestLesson  -
{   
Document!! 
=!! 
document!! 
,!! 
	IdSession"" 
="" 
request"" 
."" 
	IdSession"" %
}## 
;## 
var&& 	
response&&
 
=&& 
await&& 
_httpClient&& &
.&&& '
PostAsJsonAsync&&' 6
(&&6 7
$"'' 

{''
 
_configuration'' 
['' 
$str'' *
]''* +
}''+ ,
$str'', S
"''S T
,''T U
documentRequest''V e
)''e f
;''f g
if)) 
())	 

response))
 
.)) 
IsSuccessStatusCode)) &
)))& '
{** 
var++ 

resultDict++ 
=++ 
await++ 
response++ '
.++' (
Content++( /
.++/ 0
ReadFromJsonAsync++0 A
<++A B

Dictionary++B L
<++L M
string++M S
,++S T
string++U [
>++[ \
>++\ ]
(++] ^
)++^ _
;++_ `
if-- 

(-- 

resultDict-- 
.-- 
Count-- 
==-- 
$num--  !
)--! "
{.. 	
documentsToDelete// 
.// 
Add// !
(//! "
document//" *
)//* +
;//+ ,
}00 	
}11 
}22 
return44 

documentsToDelete44 
;44 
}55 
}66 °
UC:\Users\bapti\RiderProjects\MaClasse\Service.Cloudinary\Services\UserCloudService.cs
	namespace 	
Service
 
. 
Database 
. 
Services #
;# $
public 
class 
UserCloudService 
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
UserCloudService 
( 

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
}&& Œ
SC:\Users\bapti\RiderProjects\MaClasse\Service.Cloudinary\Services\SlugifyService.cs
	namespace 	
Service
 
. 
Database 
. 
Services #
;# $
public 
class 
SlugifyService 
{ 
public 
string	 
SlugifyFileName 
(  
string  &
name' +
)+ ,
{		 
return

 

Path

 
.

 '
GetFileNameWithoutExtension

 +
(

+ ,
name

, 0
)

0 1
. 
	Normalize 
( 
NormalizationForm "
." #
FormD# (
)( )
. 
Where 
( 
c 
=> 
char 
. 
GetUnicodeCategory )
() *
c* +
)+ ,
!=- /
UnicodeCategory0 ?
.? @
NonSpacingMark@ N
)N O
. 
	Aggregate 
( 
$str 
, 
( 
a 
, 
c 
) 
=> 
a  
+! "
c# $
)$ %
. 
Replace 
( 
$str 
, 
$str 
) 
. 
ToLowerInvariant 
( 
) 
; 
} 
} ¨A
XC:\Users\bapti\RiderProjects\MaClasse\Service.Cloudinary\Repositories\CloudRepository.cs
	namespace 	
Service
 
. 

Cloudinary 
. 
Repositories )
;) *
public 
class 
CloudRepository 
: 
ICloudRepository /
{		 
private

 	
readonly


 
CloudinaryDotNet

 #
.

# $

Cloudinary

$ .
_cloudinary

/ :
;

: ;
private 	
readonly
 
SlugifyService !
_slugifyService" 1
;1 2
private 	
ICloudRepository
 *
_cloudRepositoryImplementation 9
;9 :
public 
CloudRepository	 
( 
CloudinaryDotNet 
. 

Cloudinary 

cloudinary  *
,* +
SlugifyService 
slugifyService !
)! "
{ 
_cloudinary 
= 

cloudinary 
; 
_slugifyService 
= 
slugifyService $
;$ %
} 
public 
async	 
Task 
< 
UploadResult  
>  !
UploadFileAsync" 1
(1 2
	IFormFile2 ;
file< @
,@ A
stringB H
idUserI O
)O P
{ 
if 
( 
file 
== 
null 
|| 
file 
. 
Length #
==$ &
$num' (
)( )
throw 
new 
ArgumentException !
(! "
$str" 4
)4 5
;5 6
await 	
using
 
var 
memoryStream  
=! "
new# &
MemoryStream' 3
(3 4
)4 5
;5 6
await 	
file
 
. 
CopyToAsync 
( 
memoryStream '
)' (
;( )
memoryStream 
. 
Position 
= 
$num 
; 
var!! 
nameWithoutExt!! 
=!! 
Path!! 
.!! '
GetFileNameWithoutExtension!! 9
(!!9 :
file!!: >
.!!> ?
FileName!!? G
)!!G H
;!!H I
var"" 
	extension"" 
="" 
Path"" 
."" 
GetExtension"" %
(""% &
file""& *
.""* +
FileName""+ 3
)""3 4
?""4 5
.""5 6
ToLowerInvariant""6 F
(""F G
)""G H
;""H I
var## 
finalFileName## 
=## 
_slugifyService## '
.##' (
SlugifyFileName##( 7
(##7 8
$"##8 :
{##: ;
nameWithoutExt##; I
}##I J
{##J K
	extension##K T
}##T U
"##U V
)##V W
;##W X
var%% 
imageLikeExtensions%% 
=%% 
new%% !
[%%! "
]%%" #
{%%$ %
$str%%& ,
,%%, -
$str%%. 4
,%%4 5
$str%%6 =
,%%= >
$str%%? E
,%%E F
$str%%G M
,%%M N
$str%%O V
}%%W X
;%%X Y
if(( 
((( 
imageLikeExtensions(( 
.(( 
Contains(( $
((($ %
	extension((% .
)((. /
)((/ 0
{)) 
var** 	
imageParams**
 
=** 
new** 
ImageUploadParams** -
{++ 
File,, 
=,, 
new,, 
FileDescription,, "
(,," #
finalFileName,,# 0
,,,0 1
memoryStream,,2 >
),,> ?
,,,? @
Folder-- 
=-- 
idUser-- 
,-- 
UseFilename.. 
=.. 
true.. 
,.. 
UniqueFilename// 
=// 
true// 
,// 
	Overwrite00 
=00 
false00 
,00 

AccessMode11 
=11 
$str11 
,11 
Type22 
=22 
$str22 
,22 
UploadPreset33 
=33 
$str33 
}55 
;55 
var77 	
uploadResult77
 
=77 
await77 
_cloudinary77 *
.77* +
UploadAsync77+ 6
(776 7
imageParams777 B
)77B C
;77C D
return99 
uploadResult99 
;99 
};; 
return<< 

null<< 
;<< 
}== 
public?? 
async??	 
Task?? 
<?? 
GetResourceResult?? %
>??% &&
GetFileAsyncByIdCloudinary??' A
(??A B
string??B H
idCloudinary??I U
)??U V
{@@ 
varAA 
existingDocumentAA 
=AA 
awaitAA  
_cloudinaryAA! ,
.AA, -
GetResourceAsyncAA- =
(AA= >
newAA> A
GetResourceParamsAAB S
(AAS T
idCloudinaryAAT `
)AA` a
)AAa b
;AAb c
ifCC 
(CC 
existingDocumentCC 
==CC 
nullCC  
)CC  !
returnCC" (
nullCC) -
;CC- .
returnEE 

existingDocumentEE 
;EE 
}FF 
publicHH 
TaskHH	 
<HH 
ImageUploadResultHH 
>HH  
UpdateFileAsyncHH! 0
(HH0 1
)HH1 2
{II 
returnJJ 
*
_cloudRepositoryImplementationJJ )
.JJ) *
UpdateFileAsyncJJ* 9
(JJ9 :
)JJ: ;
;JJ; <
}KK 
publicMM 
asyncMM	 
TaskMM 
<MM 
DelResResultMM  
>MM  !
DeleteFileAsyncMM" 1
(MM1 2
stringMM2 8
idCloudinaryMM9 E
)MME F
{NN 
varOO 
deletedDocumentOO 
=OO 
awaitOO 
_cloudinaryOO  +
.OO+ , 
DeleteResourcesAsyncOO, @
(OO@ A
idCloudinaryOOA M
)OOM N
;OON O
ifQQ 
(QQ 
!QQ 	
deletedDocumentQQ	 
.QQ 
DeletedQQ  
.QQ  !
TryGetValueQQ! ,
(QQ, -
idCloudinaryQQ- 9
,QQ9 :
outQQ; >
varQQ? B
statusQQC I
)QQI J
)QQJ K
returnQQL R
nullQQS W
;QQW X
returnSS 

deletedDocumentSS 
;SS 
}TT 
publicVV 
asyncVV	 
TaskVV 
<VV 
RenameResultVV  
>VV  !
RenameFileAsyncVV" 1
(VV1 2
stringVV2 8
oldPublicIdVV9 D
,VVD E
stringVVF L
newPublicIdVVM X
)VVX Y
{WW 
varYY 
slugNewPublicIdYY 
=YY 
_slugifyServiceYY )
.YY) *
SlugifyFileNameYY* 9
(YY9 :
newPublicIdYY: E
)YYE F
;YYF G
var[[ 
renameParams[[ 
=[[ 
new[[ 
RenameParams[[ '
([[' (
oldPublicId[[( 3
,[[3 4
slugNewPublicId[[5 D
)[[D E
;[[E F
var]] 
result]] 
=]] 
await]] 
_cloudinary]] "
.]]" #
RenameAsync]]# .
(]]. /
renameParams]]/ ;
)]]; <
;]]< =
if__ 
(__ 
result__ 
.__ 

StatusCode__ 
!=__ 
System__ #
.__# $
Net__$ '
.__' (
HttpStatusCode__( 6
.__6 7
OK__7 9
)__9 :
return__; A
null__B F
;__F G
returnaa 

resultaa 
;aa 
}bb 
publicdd 
asyncdd	 
Taskdd 
<dd 
ImageUploadResultdd %
>dd% &
GetFilesAsyncdd' 4
(dd4 5
)dd5 6
{ee 
returnff 

nullff 
;ff 
}gg 
}hh ◊
CC:\Users\bapti\RiderProjects\MaClasse\Service.Cloudinary\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
builder		 
.		 
Logging		 
.		 
ClearProviders		 
(		 
)		  
;		  !
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
 

AddConsole

 
(

 
)

 
;

 
builder 
. 
Services 
. 
	AddScoped 
< 
UserCloudService +
>+ ,
(, -
)- .
;. /
builder 
. 
Services 
. 
	AddScoped 
< 
SlugifyService )
>) *
(* +
)+ ,
;, -
builder 
. 
Services 
. 
	AddScoped 
< 
VerifyDeleteService .
>. /
(/ 0
)0 1
;1 2
builder 
. 
Services 
. 
	AddScoped 
< 
ICloudRepository +
,+ ,
CloudRepository- <
>< =
(= >
)> ?
;? @
builder 
. 
Services 
. 
AddHttpClient 
( 
)  
;  !
builder 
. 
Services 
. 
AddSingleton 
( 
x 
=>  "
{ 
var 
account 
= 
new 
Account 
( 
builder 
. 
Configuration 
[ 
$str 4
]4 5
,5 6
builder 
. 
Configuration 
[ 
$str 1
]1 2
,2 3
builder 
. 
Configuration 
[ 
$str 4
]4 5
)5 6
;6 7
return 

new 

Cloudinary 
( 
account !
)! "
;" #
} 
) 
; 
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
builder,, 
.,, 
Services,, 
.,, 
AddControllers,, 
(,,  
),,  !
;,,! "
var.. 
app.. 
=.. 	
builder..
 
... 
Build.. 
(.. 
).. 
;.. 
app00 
.00 
UseHttpsRedirection00 
(00 
)00 
;00 
app11 
.11 
UseCors11 
(11 
$str11 
)11 
;11 
app22 
.22 
MapControllers22 
(22 
)22 
;22 
app44 
.44 
Run44 
(44 
)44 	
;44	 

	namespace66 	
Service66
 
.66 

Cloudinary66 
{77 
public88 

partial88 
class88 
Program88  
{88! "
}88# $
}99 ˆ
WC:\Users\bapti\RiderProjects\MaClasse\Service.Cloudinary\Interfaces\ICloudRepository.cs
	namespace 	
Service
 
. 

Cloudinary 
. 

Interfaces '
;' (
public 
	interface 
ICloudRepository !
{ 
Task 
< 
UploadResult 
> 
UploadFileAsync $
($ %
	IFormFile% .
file/ 3
,3 4
string5 ;
iduser< B
)B C
;C D
Task		 
<		 
GetResourceResult		 
>		 &
GetFileAsyncByIdCloudinary		 4
(		4 5
string		5 ;
idCloudinary		< H
)		H I
;		I J
Task

 
<

 
ImageUploadResult

 
>

 
UpdateFileAsync

 )
(

) *
)

* +
;

+ ,
Task 
< 
DelResResult 
> 
DeleteFileAsync $
($ %
string% +
idCloudinary, 8
)8 9
;9 :
Task 
< 
RenameResult 
> 
RenameFileAsync $
($ %
string% +
oldPublicId, 7
,7 8
string9 ?
newPublicId@ K
)K L
;L M
Task 
< 
ImageUploadResult 
> 
GetFilesAsync '
(' (
)( )
;) *
} ïÇ
WC:\Users\bapti\RiderProjects\MaClasse\Service.Cloudinary\Controllers\CloudController.cs
	namespace 	
Service
 
. 

Cloudinary 
. 
Controllers (
;( )
[ 
ApiController 
] 
[ 
Route 
( 
$str 
) 
] 
public 
class 
CloudController 
: 
ControllerBase -
{ 
private 	
readonly
 
UserCloudService #
_userCloudService$ 5
;5 6
private 	
readonly
 
ICloudRepository #
_fileRepository$ 3
;3 4
private 	
readonly
 
CloudinaryDotNet #
.# $

Cloudinary$ .
_cloudinary/ :
;: ;
private 	
readonly
 
VerifyDeleteService & 
_verifyDeleteService' ;
;; <
public 
CloudController	 
( 
UserCloudService 
userCloudService %
,% &
ICloudRepository 
fileRepository #
,# $
CloudinaryDotNet 
. 

Cloudinary 

cloudinary  *
,* +
VerifyDeleteService 
verifyDeleteService +
)+ ,
{ 
_userCloudService 
= 
userCloudService (
;( )
_fileRepository 
= 
fileRepository $
;$ %
_cloudinary 
= 

cloudinary 
;  
_verifyDeleteService 
= 
verifyDeleteService .
;. /
}   
["" 
HttpPost"" 
]"" 
[## 
Route## 
(## 	
$str##	 
)## 
]## 
public$$ 
async$$	 
Task$$ 
<$$ 
IActionResult$$ !
>$$! "
AddFile$$# *
($$* +
[%% 
FromForm%% 
]%% 
	IFormFile%% 
file%% 
,%% 
[%%  
FromForm%%  (
]%%( )
string%%* 0
filerequest%%2 =
)%%= >
{&& 
string(( 

originalFilename(( 
=(( 
$str((  
;((  !
DateTime)) 
	createdAt)) 
=)) 
DateTime)) !
.))! "
Now))" %
;))% &
var** 
format** 
=** 
Path** 
.** 
GetExtension** "
(**" #
file**# '
.**' (
FileName**( 0
)**0 1
?**1 2
.**2 3
	TrimStart**3 <
(**< =
$char**= @
)**@ A
.**A B
ToLowerInvariant**B R
(**R S
)**S T
;**T U
string++ 

url++ 
=++ 
$str++ 
;++ 
var-- 
	idSession-- 
=-- 
JsonSerializer-- "
.--" #
Deserialize--# .
<--. /
FileRequest--/ :
>--: ;
(--; <
filerequest--< G
)--G H
;--H I
var// 
idUser// 
=// 
_userCloudService00 
.00 
GetUserByIdSession00 *
(00* +
	idSession00+ 4
.004 5
	IdSession005 >
)00> ?
.00? @
Result00@ F
.00F G
UserId00G M
;00M N
var22 
newFileResult22 
=22 
await33 
_fileRepository33 
.33 
UploadFileAsync33 +
(33+ ,
file33, 0
,330 1
idUser332 8
)338 9
;339 :
Console55 
.55 
	WriteLine55 
(55 
JsonSerializer55 $
.55$ %
	Serialize55% .
(55. /
newFileResult55/ <
)55< =
)55= >
;55> ?
if88 
(88 
newFileResult88 
is88 
RawUploadResult88 (
	rawResult88) 2
)882 3
{99 
originalFilename:: 
=:: 
	rawResult:: "
.::" #
OriginalFilename::# 3
;::3 4
	createdAt;; 
=;; 
	rawResult;; 
.;; 
	CreatedAt;; %
;;;% &
}<< 
else== 
if==	 
(== 
newFileResult== 
is== 
ImageUploadResult== /
imageResult==0 ;
)==; <
{>> 
originalFilename?? 
=?? 
imageResult?? $
.??$ %
OriginalFilename??% 5
;??5 6
	createdAt@@ 
=@@ 
imageResult@@ 
.@@ 
	CreatedAt@@ '
;@@' (
}AA 
varDD 
newDocumentDD 
=DD 
newDD 
DocumentDD "
{EE 

IdDocumentFF 
=FF 
ObjectIdFF 
.FF 
GenerateNewIdFF )
(FF) *
)FF* +
.FF+ ,
ToStringFF, 4
(FF4 5
)FF5 6
,FF6 7
IdCloudinaryGG 
=GG 
newFileResultGG "
.GG" #
PublicIdGG# +
,GG+ ,
NameHH 

=HH 
$strHH 
,HH 
UrlII 	
=II
 
newFileResultII 
.II 
	SecureUrlII #
?II# $
.II$ %
ToStringII% -
(II- .
)II. /
??II0 2
newFileResultII3 @
.II@ A
UrlIIA D
?IID E
.IIE F
ToStringIIF N
(IIN O
)IIO P
??IIQ S
$strIIT V
,IIV W
ThumbnailUrlJJ 
=JJ 
$strJJ 
,JJ 
FormatKK 
=KK 
$strKK 
,KK 
	CreatedAtLL 
=LL 
DateTimeLL 
.LL 
NowLL 
}MM 
;MM 
ifOO 
(OO 
formatOO 
.OO 
ToLowerOO 
(OO 
)OO 
==OO 
$strOO !
)OO! "
{PP 
newDocumentQQ 
.QQ 
ThumbnailUrlQQ 
=QQ  
_cloudinaryQQ! ,
.QQ, -
ApiQQ- 0
.QQ0 1
UrlImgUpQQ1 9
.RR 	
	TransformRR	 
(RR 
newRR 
TransformationRR %
(RR% &
)RR& '
.SS
 
PageSS 
(SS 
$strSS 
)SS 
.TT
 
FetchFormatTT 
(TT 
$strTT 
)TT 
.UU
 
WidthUU 
(UU 
$numUU 
)UU 
.VV
 
CropVV 
(VV 
$strVV 
)VV 
)VV 
.WW 	
BuildUrlWW	 
(WW 
$"WW 
{WW 
newFileResultWW "
.WW" #
PublicIdWW# +
}WW+ ,
"WW, -
)WW- .
;WW. /
}XX 
elseYY 
{ZZ 
newDocument[[ 
.[[ 
ThumbnailUrl[[ 
=[[  
_cloudinary[[! ,
.[[, -
Api[[- 0
.[[0 1
UrlImgUp[[1 9
.\\ 	
	Transform\\	 
(\\ 
new\\ 
Transformation\\ %
(\\% &
)\\& '
.]]
 
Width]] 
(]] 
$num]] 
)]] 
.^^
 
Crop^^ 
(^^ 
$str^^ 
)^^ 
)^^ 
.__ 	
BuildUrl__	 
(__ 
$"__ 
{__ 
newFileResult__ "
.__" #
PublicId__# +
}__+ ,
$str__, -
{__- .
newFileResult__. ;
.__; <
Format__< B
}__B C
"__C D
)__D E
;__E F
newDocumentaa 
.aa 
Urlaa 
=aa 
newFileResultaa %
.aa% &
	SecureUrlaa& /
?aa/ 0
.aa0 1
ToStringaa1 9
(aa9 :
)aa: ;
??aa< >
newFileResultaa? L
.aaL M
UrlaaM P
?aaP Q
.aaQ R
ToStringaaR Z
(aaZ [
)aa[ \
??aa] _
$straa` b
;aab c
}bb 
newDocumentdd 
.dd 
Namedd 
=dd 
originalFilenamedd '
;dd' (
newDocumentee 
.ee 
Formatee 
=ee 
formatee 
;ee  
newDocumentff 
.ff 
	CreatedAtff 
=ff 
	createdAtff %
;ff% &
returnhh 

Okhh 
(hh 
newDocumenthh 
)hh 
;hh 
}ii 
[kk 
HttpPostkk 
]kk 
[ll 
Routell 
(ll 	
$strll	 
)ll 
]ll 
publicmm 
asyncmm	 
Taskmm 
<mm 
IActionResultmm !
>mm! "
GetFilemm# *
(mm* +
)mm+ ,
{nn 
returnoo 

Okoo 
(oo 
)oo 
;oo 
}pp 
[rr 
HttpPostrr 
]rr 
[ss 
Routess 
(ss 	
$strss	 
)ss 
]ss 
publictt 
asynctt	 
Tasktt 
<tt 
IActionResulttt !
>tt! "

UpdateFilett# -
(tt- .
[tt. /
FromBodytt/ 7
]tt7 8
Documenttt9 A
documentttB J
)ttJ K
{uu 
varww 
existingDocumentww 
=ww 
awaitww  
_fileRepositoryww! 0
.ww0 1&
GetFileAsyncByIdCloudinaryww1 K
(wwK L
documentwwL T
.wwT U
IdCloudinarywwU a
)wwa b
;wwb c
ifyy 
(yy 
existingDocumentyy 
==yy 
nullyy  
)yy  !
returnyy" (
NotFoundyy) 1
(yy1 2
)yy2 3
;yy3 4
var|| 

folderName|| 
=|| 
existingDocument|| %
.||% &
PublicId||& .
.||. /
	Substring||/ 8
(||8 9
$num||9 :
,||: ;
existingDocument||< L
.||L M
PublicId||M U
.||U V
LastIndexOf||V a
(||a b
$char||b e
)||e f
)||f g
;||g h
var}} 
fileName}} 
=}} 
existingDocument}} #
.}}# $
PublicId}}$ ,
.}}, -
	Substring}}- 6
(}}6 7
existingDocument}}7 G
.}}G H
PublicId}}H P
.}}P Q
LastIndexOf}}Q \
(}}\ ]
$char}}] `
)}}` a
+}}b c
$num}}d e
)}}e f
;}}f g
var~~ 
suffix~~ 
=~~ 
fileName~~ 
.~~ 
	Substring~~ #
(~~# $
fileName~~$ ,
.~~, -
LastIndexOf~~- 8
(~~8 9
$char~~9 <
)~~< =
)~~= >
;~~> ?
var 
newFileName 
= 
$" 
{ 

folderName #
}# $
$str$ %
{% &
document& .
.. /
Name/ 3
}3 4
{4 5
suffix5 ;
}; <
"< =
;= >
var
ÇÇ 
RenamedDocument
ÇÇ 
=
ÇÇ 
await
ÇÇ 
_fileRepository
ÇÇ  /
.
ÇÇ/ 0
RenameFileAsync
ÇÇ0 ?
(
ÇÇ? @
existingDocument
ÇÇ@ P
.
ÇÇP Q
PublicId
ÇÇQ Y
,
ÇÇY Z
newFileName
ÇÇ[ f
)
ÇÇf g
;
ÇÇg h
if
ÑÑ 
(
ÑÑ 
RenamedDocument
ÑÑ 
==
ÑÑ 
null
ÑÑ 
)
ÑÑ  
return
ÑÑ! '

BadRequest
ÑÑ( 2
(
ÑÑ2 3
)
ÑÑ3 4
;
ÑÑ4 5
document
ÜÜ 
.
ÜÜ 
IdCloudinary
ÜÜ 
=
ÜÜ 
newFileName
ÜÜ '
;
ÜÜ' (
return
àà 

Ok
àà 
(
àà 
document
àà 
)
àà 
;
àà 
}
ââ 
[
ãã 
HttpPost
ãã 
]
ãã 
[
åå 
Route
åå 
(
åå 	
$str
åå	 
)
åå 
]
åå 
public
çç 
async
çç	 
Task
çç 
<
çç 
IActionResult
çç !
>
çç! "

DeleteFile
çç# -
(
çç- .
[
çç. /
FromBody
çç/ 7
]
çç7 8
RequestLesson
çç9 F
request
ççG N
)
ççN O
{
éé 
var
êê 
existingDocument
êê 
=
êê 
await
êê  
_fileRepository
êê! 0
.
êê0 1(
GetFileAsyncByIdCloudinary
êê1 K
(
êêK L
request
êêL S
.
êêS T
Document
êêT \
.
êê\ ]
IdCloudinary
êê] i
)
êêi j
;
êêj k
if
íí 
(
íí 
existingDocument
íí 
==
íí 
null
íí  
)
íí  !
return
íí" (
NotFound
íí) 1
(
íí1 2
)
íí2 3
;
íí3 4
var
ïï 
documentsToDelete
ïï 
=
ïï 
await
ïï !"
_verifyDeleteService
ïï" 6
.
ïï6 7
VerifyDeleteFiles
ïï7 H
(
ïïH I
request
ïïI P
)
ïïP Q
;
ïïQ R
if
òò 
(
òò 
documentsToDelete
òò 
.
òò 
Count
òò 
==
òò !
$num
òò" #
)
òò# $
{
ôô 
return
õõ 
Ok
õõ 
(
õõ 
)
õõ 
;
õõ 
}
úú 
var
üü 
deletedDocument
üü 
=
üü 
await
üü 
_fileRepository
üü  /
.
üü/ 0
DeleteFileAsync
üü0 ?
(
üü? @
request
üü@ G
.
üüG H
Document
üüH P
.
üüP Q
IdCloudinary
üüQ ]
)
üü] ^
;
üü^ _
return
†† 

Ok
†† 
(
†† 
existingDocument
†† 
)
†† 
;
††  
}
°° 
[
££ 
HttpPost
££ 
]
££ 
[
§§ 
Route
§§ 
(
§§ 	
$str
§§	 
)
§§ 
]
§§ 
public
•• 
async
••	 
Task
•• 
<
•• 
IActionResult
•• !
>
••! "
GetFiles
••# +
(
••+ ,
)
••, -
{
¶¶ 
return
ßß 

Ok
ßß 
(
ßß 
)
ßß 
;
ßß 
}
®® 
[
™™ 
HttpPost
™™ 
]
™™ 
[
´´ 
Route
´´ 
(
´´ 	
$str
´´	 
)
´´ 
]
´´ 
public
¨¨ 
async
¨¨	 
Task
¨¨ 
<
¨¨ 
IActionResult
¨¨ !
>
¨¨! "
DeleteFiles
¨¨# .
(
¨¨. /
[
¨¨/ 0
FromBody
¨¨0 8
]
¨¨8 9
RequestLesson
¨¨: G
request
¨¨H O
)
¨¨O P
{
≠≠ 
var
ÆÆ 
documentsToDelete
ÆÆ 
=
ÆÆ 
await
ÆÆ !"
_verifyDeleteService
ÆÆ" 6
.
ÆÆ6 7
VerifyDeleteFiles
ÆÆ7 H
(
ÆÆH I
request
ÆÆI P
)
ÆÆP Q
;
ÆÆQ R
foreach
∞∞ 
(
∞∞ 
var
∞∞ 
document
∞∞ 
in
∞∞ 
documentsToDelete
∞∞ .
)
∞∞. /
{
±± 
var
≤≤ 	
existingDocument
≤≤
 
=
≤≤ 
await
≤≤ "
_fileRepository
≤≤# 2
.
≤≤2 3(
GetFileAsyncByIdCloudinary
≤≤3 M
(
≤≤M N
document
≤≤N V
.
≤≤V W
IdCloudinary
≤≤W c
)
≤≤c d
;
≤≤d e
if
¥¥ 
(
¥¥	 

existingDocument
¥¥
 
==
¥¥ 
null
¥¥ "
)
¥¥" #
return
¥¥$ *
NotFound
¥¥+ 3
(
¥¥3 4
)
¥¥4 5
;
¥¥5 6
await
∂∂ 
_fileRepository
∂∂ 
.
∂∂ 
DeleteFileAsync
∂∂ +
(
∂∂+ ,
document
∂∂, 4
.
∂∂4 5
IdCloudinary
∂∂5 A
)
∂∂A B
;
∂∂B C
}
∏∏ 
return
∫∫ 

Ok
∫∫ 
(
∫∫ 
)
∫∫ 
;
∫∫ 
}
ªª 
}ºº 