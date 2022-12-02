// ATRD 

var V = Sistem.GrafikVerileri;
var VG = Sistem.GrafikVerileriniOku(Sistem.Sembol,"G");
var CG = Sistem.GrafikFiyatOku(VG,"Kapanis");
var ATRG = Sistem.AverageTrueRange(VG, 14);
var ATRD = Sistem.DonemCevir(V,VG,ATRG);
ATRD = Sistem.Ref(ATRD,-1);

// SİSTEM TANIMLAMALARI

var C = Sistem.GrafikFiyatSec("Kapanis");
var L = Sistem.GrafikFiyatSec("Dusuk");
var H = Sistem.GrafikFiyatSec("Yuksek");
var O = Sistem.GrafikFiyatSec("Acilis");
var SonYon ="F";
var Sinyal = "";
double  alım = 0 ;
double  satım = 0 ;

// HAREKETLİ ORTALAMALAR

var EMA8 = Sistem.MA(C, "Variable", 60);
var EMA3 = Sistem.MA(C, "Variable", 19);
var EMAATRD = Sistem.MA(ATRD, "Exp",8);
var EMAATRDD = Sistem.MA(ATRD, "Exp",5);
var ATR = Sistem.AverageTrueRange(C , 14);
var ema3ATR = Sistem.MA(ATR,"Simple",20);
var ema1ATR = Sistem.MA(ATR,"Simple",10);

// TEMP ALIM SATIM DEĞERLERİ

double  tempAlim =0;
double  tempSatim =0;

// HİGHEST - LOWEST DEĞERLERİ

double Highest = 1;
double Lowest= 1;

//ALT ÜST BAR ÇİZDİRME

double highesttemp = 1;
double lowesttemp = 1;
int highestUpdateSignal = 0;
int lowestUpdateSignal = 0;

// OTT GRAF DEĞERLERİ

var q1 = Sistem.Liste(0);
var q2 = Sistem.Liste(0);
var ottgraf= Sistem.Liste(0);
var opt = 3f;
var per = 8f;
var C1 = Sistem.MA(C, "Variable", 8);
var q0 = Sistem.Ref(C1,-2);

// TREND ÇİZDİRME

var Pe = 50;
var pe1 = Pe - 1;
var ebar = Sistem.BarSayisi - 10;
var evalue = Sistem.LinearReg(C, Pe)[ebar - 1];
var slope = Sistem.LinearRegSlope(C, Pe)[ebar - 1];
var barsince = 0;
var center = Sistem.Liste(0);

// GRAFİK ELEMANLARI

var Gosterge = Sistem.Liste(1);
var AnlikOran = Sistem.Liste(1);
var tempAlimGosterge= Sistem.Liste(1);
var tempSatimGosterge = Sistem.Liste(1);
var longstop= Sistem.Liste(1);
var karal = Sistem.Liste(1);
var shortstop= Sistem.Liste(1);
var karsat = Sistem.Liste(1);
var ustgrafik= Sistem.Liste(1);
var altgrafik = Sistem.Liste(1);

// EN YÜKSEK VE EN DÜŞÜK DEĞER

var HighestAlSat=Sistem.HHV(4, "Kapanis");
var LowestAlSat=Sistem.LLV(4, "Kapanis");

//SİSTEM 

for ( var i=5 ; i<Sistem.BarSayisi ; i++)
{

//--------------- AL VE SAT KOŞULU ---------------------

  if(((EMA3[i-1]<EMA8[i-1] && EMA3[i]>EMA8[i])   )|| ((SonYon == "F" && C[i] > EMA3[i-2]+ATRD[i-1]/2 &&EMA3[i]>EMA8[i] ) && ATR[i] > ema3ATR[i] && ema1ATR[i]>0 ) ) Sinyal ="A"; // ALIM KOŞULU

//************

  if(((EMA3[i-1]>EMA8[i-1] && EMA3[i]<EMA8[i])  ) || ((SonYon == "F" && C[i] < EMA3[i-2]-ATRD[i-1]/2 && EMA3[i]<EMA8[i]  ) && ATR[i] > ema3ATR[i] ) )  Sinyal = "S"; // SATIM KOŞULU

//------------------ STOP KURALLARI ------------------------

// LONG STOP 
if (SonYon =="A" && C[i] <= tempAlim - ATRD[i-1]/2  ) {
Sinyal="F";
Gosterge[i] = 25;
} 

//************

//SHORT STOP

if (SonYon =="S" && C[i] >= (tempSatim + ATRD[i-1]/2 ) ) {

Sinyal="F" ; 
Gosterge[i+1] =50; 
} 

//------------------ EMNİYET STOPLARI------------------------

// EMNİYET LONG STOP 
if (SonYon =="A" && L[i] <= tempAlim - ATRD[i-1]* 5/2  ) {
Sinyal="F";
Gosterge[i] = 125;
} 

//************

// EMNİYET SHORT STOP

if (SonYon =="S" && H[i] >= (tempSatim + ATRD[i-1]* 5/2 ) ) {
Sinyal="F" ; 
Gosterge[i+1] =150; 
} 

//--------------- TEMP ALIM SATIM GÜNCELLENMESİ ---------------------

// TEMP ALIM
if (C[i] > tempAlim + ATRD[i-1]/2 && SonYon == "A"){
 tempAlim += ATRD[i]/2;
}

//************

// TEMP SATIM
if (C[i] < tempSatim - ATRD[i-1]/2 && SonYon == "S"){
 tempSatim -= ATRD[i]/2;
}

tempAlimGosterge[i] = (float) tempAlim; 
tempSatimGosterge[i] = (float) tempSatim; 

//--------------- HİGHEST VE LOWEST GÜNCELLENMESİ ---------------------

// HİGHEST

if (C[i] > Highest && SonYon== "A"){
Highest = C[i];
}

//************

// LOWEST

if (C[i] < Lowest && SonYon== "S"){
Lowest = C[i];
}

//----------------ALT VE ÜST BAR ÇİZDİRME-------------------------

//--------------------YÜKSELİŞ----------------

//YÜKSELİŞ SİNYALİ YAKALAMA
if(C[i] > highesttemp ) {

highestUpdateSignal = 1;
highesttemp = C[i];
}

// TEPE DEĞERDEN ATRD * X KADAR DÜŞÜŞ SİNYALİ YAKALAMA
if(C[i] < highesttemp - ATRD[i] * 3) {

highestUpdateSignal = 1;
}

//YAKALANAN TEPE NOKTASINI GRAFİĞE VERME
if(highestUpdateSignal == 1){

highesttemp = C[i];
highestUpdateSignal = 0;
}

//----------------------DÜŞÜŞ-------------------

// DÜŞÜŞ SİNYALİ YAKALAMA
if(C[i] < lowesttemp ) {

lowestUpdateSignal = 1;
lowesttemp = C[i];
Gosterge[i] = 5000;
}

// DİP DEĞERDEN ATRD * X KADAR YÜKSELİŞ SİNYALİ YAKALAMA
if(C[i] > lowesttemp + ATRD[i] * 3) {

lowestUpdateSignal = 1;
}

// YAKALANAN DİP NOKTASINI GRAFİĞE VERME
if(lowestUpdateSignal == 1){
lowesttemp = C[i];
lowestUpdateSignal = 0;
}

// YAKALANAN TEPE VE DİP VERİLERİNİ EKRANA YAZDIRMA
altgrafik[i] = (float) lowesttemp ;
ustgrafik[i] = (float) highesttemp ;

//--------------- YÖN YAPILANMALARI ---------------------

if ( SonYon !="A" && Sinyal =="A" ) // AL
   {
   Sistem.Yon[i] = "A"; 
   SonYon = Sistem.Yon[i];
   alım = C[i];
   tempAlim = alım;
   Highest = alım; 
   }

//************

else if ( SonYon !="S" && Sinyal =="S") // SAT
   {
   Sistem.Yon[i] = "S"; 
   SonYon = Sistem.Yon[i];
   satım = C[i];
   tempSatim=satım;
   Lowest = satım;
}

//************

else if ( SonYon != "F" && Sinyal =="F" ) // FLAT
   {
   Sistem.Yon[i] = "F"; 
   SonYon = Sistem.Yon[i];
   }

//--------------- KAR AL VE KAR SAT ---------------------

// KAR AL
if(SonYon =="A" && C[i] < Highest - ATRD[i-1]/2 && C[i]>alım ){ 
Sinyal ="F" ;
Gosterge[i+1] =75; 
Highest = 0;
}

//************

// KAR SAT
if(SonYon =="S" && C[i] > Lowest + ATRD[i-1]/2 && C[i]<satım) {
Sinyal ="F";
Gosterge[i+1] =100;
Lowest = 0;
}

//--------------- OTT GRAFİĞİ ---------------------

q1[i]=C[i-2]*(1+opt/200);
q2[i]=C[i-2]*(1-opt/200);
ottgraf[i]=i==2?C1[i]:q1[i]<=ottgraf[i-1]?q1[i]:q2[i]>=ottgraf[i-1]?q2[i]:ottgraf[i-1];

// STOP NOKTALARINI GÖRME
if(SonYon == "A" ) longstop[i] = (float)  tempAlim - ATRD[i-1]/2;
if(SonYon == "S" ) shortstop[i] = (float)  tempSatim + ATRD[i-1]/2;

// KAR AL NOKTALARINI GÖRME

if(SonYon == "A" ) karal[i] = (float) Highest - ATRD[i-1]/2 ;
if(SonYon == "S" ) karsat[i] = (float) Lowest + ATRD[i-1]/2 ;

// ANLIK ATRD ORANI
AnlikOran[i] = (float) (ATRD[i] / C[i] *100);
}

//SİSTEM BİTİŞİ

//--------------- GRAFİKLER ---------------------

// HAREKETLİ ORTALAMA GRAFİKLERİ
Sistem.Cizgiler[0].Deger = EMA3;
Sistem.Cizgiler[1].Deger = EMA8;

// OTT GRAFİĞİ
Sistem.Cizgiler[2].Deger = ottgraf;

// ATRD GRAFİĞİ
Sistem.Cizgiler[3].Deger = ATRD;

// DEĞİŞKEN GÖSTERGE
Sistem.Cizgiler[4].Deger = Gosterge;

// ATRD ORANI 
Sistem.Cizgiler[5].Deger = AnlikOran;

// TEMP ALIM SATIM GÖSTERGESİ
Sistem.Cizgiler[6].Deger = tempAlimGosterge;
Sistem.Cizgiler[7].Deger = tempSatimGosterge;

//KARAL VE STOPLAR
Sistem.Cizgiler[12].Deger = longstop;
Sistem.Cizgiler[13].Deger = karal;
Sistem.Cizgiler[14].Deger = shortstop;
Sistem.Cizgiler[15].Deger = karsat;

// KAR ZARAR GRAFİĞİ
Sistem.GetiriHesapla("23/08/2021",0.000);
Sistem.Cizgiler[8].Deger = Sistem.GetiriKZ;

// ATRD GRAFİĞİ
Sistem.Cizgiler[9].Deger = ATR;
Sistem.Cizgiler[10].Deger = ema3ATR;
Sistem.Cizgiler[11].Deger = ema1ATR;

// ALT VE ÜST GRAFİK
Sistem.Cizgiler[12].Deger = ustgrafik;
Sistem.Cizgiler[13].Deger = altgrafik;