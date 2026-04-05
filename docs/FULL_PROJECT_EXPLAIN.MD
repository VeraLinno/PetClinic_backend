# Pet Clinic backend projekt

## Projekti ülevaade

Pet Clinic on veterinaarkliiniku süsteem. Selle eesmärk on hoida ühes kohas koos kliiniku põhiandmed: omanike kontod, lemmikloomad, ajad, visiidid, retseptid, arved, inventar, tõlked ja administraatori haldus.

Süsteemi kasutavad kolm peamist rolli:

- **Owner** ehk lemmiku omanik. Tema saab lisada ja hallata oma lemmikuid, vaadata aegasid, arveid ja oma andmeid.
- **Vet** ehk veterinaar. Tema näeb talle määratud või temaga seotud patsiente, saab kinnitada aegu, lõpetada visiite, teha retsepte ja hallata inventari.
- **Admin** ehk süsteemi administraator. Tema saab teha kõrgema õigusega haldustoiminguid, näiteks kasutajate, veterinaaride, tõlgete ja auditiga seotud tegevusi.

Peamine probleem, mida süsteem lahendab, on see, et kliiniku töö ei ole seotud ainult ühe ekraaniga. Kliinikus peab sama andmestik olema nähtav eri kasutajarollidele erinevalt. Omanik peab nägema ainult oma andmeid, veterinaar peab nägema oma tööga seotud andmeid ja admin peab nägema kogu süsteemi ülevaadet. Selle pärast on rakendus ehitatud rollipõhiselt.

Frontend ja backend töötavad koos nii, et frontend on kasutaja jaoks nähtav osa ja backend on andmete ning reeglite keskne osa. Frontend saadab backendile HTTP päringuid, näiteks login, lemmikute nimekiri või aja broneerimine. Backend kontrollib õigused, loeb ja kirjutab andmebaasi ning tagastab vastuse kujul JSON-i või mõnel juhul ka view. Lisaks kasutab frontend JWT access tokenit, et tõestada, kes kasutaja on.

## Arhitektuur ja kaustastruktuur

Projekt on jagatud kihtideks, et kood ei muutuks kiiresti segaseks ja et vastutus oleks selgelt eraldatud. Põhiidee on see, et iga kiht teeb ainult oma tööd.

### Lahenduse põhiosad

- `src/PetClinic.Domain` sisaldab domeeni ehk põhilisi äriloogika objekte.
- `src/PetClinic.Application` sisaldab DTO-sid, liideseid ja AutoMapperi profiili.
- `src/PetClinic.Infrastructure` sisaldab andmebaasi, teenuste teostusi ja püsivusega seotud loogikat.
- `src/PetClinic.Api` sisaldab kontrollereid, middleware’i, käivitust ja Swaggerit.
- `tests/PetClinic.Tests` sisaldab ühik- ja integratsiooniteste.

### Miks projekt on niimoodi jaotatud

Selline jaotus on kasulik mitmel põhjusel:

- Domeenimudel jääb puhtaks ja ei sõltu HTTP-st ega andmebaasi tehnilistest detailidest.
- Rakenduskihi DTO-d kaitsevad mind selle eest, et ma ei hakkaks otse andmebaasi objekte klienti saatma.
- Infrastructure saab tegeleda EF Core, PostgreSQL, auth’i, tõlgete ja muude tehniliste asjadega.
- API kiht jääb õhukeseks. Kontroller võtab päringu vastu, kutsub teenust ja tagastab vastuse.
- Testimine on lihtsam, sest teenuseid saab testida eraldi andmebaasita või testandmebaasiga.

### Kas projekt kasutab MVC-d

See projekt ei ole klassikaline täis-MVC veebirakendus, kus mudel, vaade ja kontroller töötavad ühes serveripoolses UI-s. Põhiosa on pigem REST API.

Kui seletada lihtsalt, siis:

- **Model** on andmemudel või domeeniobjekt, näiteks `Owner`, `Pet` või `Appointment`.
- **Controller** võtab HTTP päringu vastu ja otsustab, mida edasi teha.
- **View** on siin peamiselt eraldi frontend Vue rakenduses, mitte klassikaline serveripoolne Razor vaade.

Siiski on projektis ka MVC-laadne osa. `ClientController` tagastab vaateid ja kasutab `ViewBag`-i, seega see osa käitub rohkem nagu klassikaline MVC. Aga üldine arhitektuur on ikkagi API-keskne ja enamik loogikast käib läbi kontrolleri, teenuse ja andmebaasi töövoo.

## Domeenimudel

Domeenimudel on projekti kõige tähtsam loogiline osa. Siin on kirjeldatud, millised andmeobjektid süsteemis olemas on ja kuidas need omavahel seotud on.

### Peamised entiteedid

#### `Owner`

`Owner` on kasutaja konto mudel. See hoiab endas e-posti, paroolihashi, ees- ja perekonnanime ning rolle. Sama tabel hoiab ka refresh tokenite infot.

Miks see on olemas:

- omanik peab saama sisse logida,
- rollid peavad olema seotud konkreetse kontoga,
- sama inimene võib olla süsteemis erinevas rollis, näiteks omanik või veterinaar.

#### `Pet`

`Pet` on lemmikloom. Sellel on nimi, liik, tõug, sünniaeg ja omaniku ID.

Miks see on olemas:

- kliinik töötab lemmikloomade, mitte ainult kasutajakontodega,
- iga lemmik peab kuuluma kindlale omanikule,
- broneeringud ja arved on seotud lemmiku kaudu omaniku andmetega.

#### `Veterinarian`

`Veterinarian` hoiab veterinaari profiili: nimi, email, telefon, litsentsinumber, aktiivsus ja auditiga seotud väljad.

Miks see on olemas:

- veterinaar ei ole ainult login kontoga kasutaja, vaid ka kliinilise töö profiil,
- litsentsinumber peab olema unikaalne,
- veterinaari saab aktiveerida ja deaktiveerida.

#### `Appointment`

`Appointment` on aeg ehk broneering. Sellel on `PetId`, `VeterinarianId`, algus ja lõpp ning staatus.

Miks see on olemas:

- see on süsteemi keskne tööobjekt,
- omaniku, veterinaari ja visiidi vahel tekib konkreetne ajakirje,
- selle kaudu saab kontrollida, kas aeg on vaba või juba hõivatud.

#### `Visit`

`Visit` on päris visiit, mis on seotud ühe broneeringuga. Seal hoitakse märkusi, loomise aega ja lõpetamise aega.

Miks see on olemas:

- broneering ja tegelik visiit ei ole alati sama asi,
- visiit tekib siis, kui veterinaar reaalselt tegeleb patsiendiga,
- visiidi sulgemine käivitab retseptid ja arve loomise.

#### `Prescription`

`Prescription` hoiab visiidi ajal määratud ravimi, doosi ja koguse.

Miks see on olemas:

- kliinik peab nägema, mida patsiendile määrati,
- ravimite kogus peab vähendama laoseisu,
- hiljem saab seda kasutada aruandluses.

#### `Invoice`

`Invoice` ehk arve on seotud ühe visiidiga. Seal on summa, väljastamise aeg, makse staatus ja maksetähtaeg.

Miks see on olemas:

- omanik peab nägema, mida ta peab maksma,
- visiidiga seotud rahaline info peab olema eraldi,
- arvet saab märgitud tasutuks.

#### `MedicationStock`

`MedicationStock` on ravimite laoseis. Seal on nimi, kategooria, ühiku hind, kogus ja miinimumtase.

Miks see on olemas:

- veterinaar peab nägema, kui palju ravimeid on alles,
- süsteem peab suutma anda märku madalast laoseisust,
- ravimite tellimused on seotud laoga.

#### `InventoryReorder`

`InventoryReorder` hoiab ravimite juurde tellimist. Seal on ravim, kogus, planeeritud kohaletoimetamise aeg ja kättesaamise aeg.

Miks see on olemas:

- kliinik saab teha sisemise laotellimuse,
- süsteem teab, millal kaup peaks kohale jõudma,
- saab eristada ootel ja juba saabunud tellimusi.

#### `RefreshToken`

`RefreshToken` hoiab uuendamise tokeni hash’i, aegumist ja tühistamise infot.

Miks see on olemas:

- access token on lühiajaline,
- refresh token aitab kasutajal uuesti sisse saada ilma uut parooli küsimata,
- token on andmebaasis hash’ina, mitte lihttekstina.

#### `Translation`

`Translation` hoiab tõlkekirjeid. Igal kirjel on võtme nimi, keelekood, väärtus ja kategooria.

Miks see on olemas:

- rakendus peab toetama mitut keelt,
- tõlked ei pea olema ainult frontendis, osa neist saab tulla ka andmebaasist,
- administraator saab tõlkeid hallata ilma koodi muutmata.

#### `AdminAuditEvent`

`AdminAuditEvent` logib admini tegevusi. Seal on tegevuse nimi, sihtobjekti tüüp, kasutaja, aeg ja lisainfo.

Miks see on olemas:

- admini tegevused peavad olema jälgitavad,
- hiljem saab vaadata, kes mida tegi,
- see aitab turvalisuse ja auditiga.

#### `VetUnavailability`

See entiteet hoiab veterinaari kättesaamatuid ajavahemikke.

Miks see on olemas:

- veterinaar saab märkida, millal ta ei tööta,
- aegade broneerimisel saab vältida valesid valikuid.

### Suhted entiteetide vahel

Lihtsustatult liiguvad suhted nii:

- üks `Owner` võib omada mitu `Pet`-i,
- üks `Pet` võib omada mitu `Appointment`-i,
- üks `Appointment` kuulub ühele lemmikule ja võib olla seotud ühe veterinaariga,
- üks `Appointment` võib viia ühele `Visit`-ile,
- üks `Visit` võib omada mitu `Prescription`-it ja ühte `Invoice`-i,
- üks `MedicationStock` võib omada mitu `InventoryReorder`-it,
- üks `Owner` võib omada mitu `RefreshToken`-it.

See struktuur on loogiline, sest kliinikus ei ole aeg või arve kunagi täiesti eraldiseisev asi. Kõik liigub lõpuks omaniku, lemmiku ja visiidi ümber.

### Kuidas andmed süsteemis liiguvad

Tüüpiline voog on selline:

1. Kasutaja logib sisse.
2. Backend kontrollib parooli ja loob JWT access tokeni.
3. Kasutaja teeb järgmise päringu tokeniga.
4. Kontroller loeb kasutaja ID ja rollid tokenist.
5. Teenus kontrollib, kas kasutajal on õigus seda andmestikku näha või muuta.
6. Kui kõik on korras, loetakse või muudetakse andmeid andmebaasis.
7. Tulemus mapitakse DTO-ks ja saadetakse vastusena tagasi.

See tähendab, et frontend ei otsusta kunagi üksi, mida kasutaja tohib teha. Viimane kontroll peab alati olema backendis.

## REST API ja kontrollerid

REST API on viis, kuidas backend pakub andmeid HTTP kaudu. Iga ressurss on oma URL-i all ja kasutatakse tavalisi HTTP meetodeid:

- `GET` loeb andmeid,
- `POST` loob uusi andmeid,
- `PUT` või `PATCH` muudab andmeid,
- `DELETE` eemaldab andmeid.

Selles projektis on kontrollerid API keskmes. Kontroller ei tohiks teha kogu äriloogikat ise. Ta võtab päringu vastu, kontrollib põhilise sisendi ja kutsub teenust.

### Peamised kontrollerid

#### `AuthController`

Haldab sisselogimist, registreerimist, refresh tokeni uuendamist ja logout’i.

Olulised endpointid:

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout`
- `POST /api/v1/auth/register-vet`

See kontroller kasutab ka rate limitingut, et login- ja auth-päringuid ei saaks liiga palju teha.

#### `OwnersController`

Haldab omaniku vaadet ja tema enda andmeid.

Olulised endpointid:

- `GET /api/v1/owners/me`
- `PUT /api/v1/owners/me`
- `GET /api/v1/owners/me/pets`
- `POST /api/v1/owners/me/pets`
- `PUT /api/v1/owners/me/pets/{petId}`
- `DELETE /api/v1/owners/me/pets/{petId}`
- `GET /api/v1/owners/me/invoices`
- `POST /api/v1/owners/me/invoices/{invoiceId}/mark-paid`

#### `AppointmentsController`

Haldab broneeringuid.

Olulised endpointid:

- `POST /api/v1/appointments`
- `GET /api/v1/appointments`
- `GET /api/v1/appointments/{id}`
- `PATCH /api/v1/appointments/{id}/confirm`
- `PATCH /api/v1/appointments/{id}/cancel`

#### `VisitsController`

Haldab visiite ja visiidi lõpetamist.

Olulised endpointid:

- `GET /api/v1/visits/{id}`
- `PATCH /api/v1/visits/{id}/complete`

#### `InventoryController`

Haldab ravimite laoseisu ja tellimusi.

Olulised endpointid:

- `GET /api/v1/inventory`
- `GET /api/v1/inventory/low-stock`
- `PUT /api/v1/inventory/{medicationId}`
- `POST /api/v1/inventory/{medicationId}/reorder`
- `GET /api/v1/inventory/incoming`
- `GET /api/v1/inventory/delivered`

#### `VetsController`

Haldab veterinaari kontosid.

Olulised endpointid:

- `GET /api/v1/vets/accounts`
- `POST /api/v1/vets/accounts`
- `PUT /api/v1/vets/accounts/{id}`
- `DELETE /api/v1/vets/accounts/{id}`

#### `TranslationsController`

Pakub tõlgete lugemist ja adminile ka haldamist.

Olulised endpointid:

- `GET /api/v1/translations/language/{languageCode}`
- `GET /api/v1/translations/language/{languageCode}/category/{category}`
- `GET /api/v1/translations/language/{languageCode}/category/{category}/key/{key}`
- `GET /api/v1/translations/languages`
- `GET /api/v1/translations/categories`
- `POST /api/v1/translations/search`
- `POST /api/v1/translations`
- `PUT /api/v1/translations/{id}`
- `DELETE /api/v1/translations/{id}`
- `POST /api/v1/translations/bulk`

#### `HealthController`

Lihtne tervisekontroll. Kasutatakse selleks, et näha, kas teenus vastab.

#### `ClientController`

See on serveripoolne MVC-laadne controller, mis tagastab vaateid. See osa on rohkem seotud omanikupoolse kasutajaliidese loogikaga. Põhifrontend on siiski eraldi Vue rakendus.

### Miks on API versioonimine vajalik

Kõik API kontrollerid kasutavad vormi `api/v{version:apiVersion}/...`, näiteks `/api/v1/auth/login`.

Versioonimine on kasulik, sest:

- hiljem saab teha uue versiooni ilma vana katki tegemata,
- frontend ja backend saavad areneda järk-järgult,
- vana kliendi tugi ei kao kohe ära,
- muudatused on paremini hallatavad.

See projekt kasutab URL-i põhist versioonimist. See tähendab, et versioon on aadressi sees, mitte ainult peidetud headeris.

### Miks DTO-sid kasutatakse

DTO tähendab Data Transfer Object. See on lihtne andmeobjekt, mida kasutatakse päringu või vastuse jaoks.

Ma ei saada otse andmebaasi entiteete kliendile, sest:

- entiteedid sisaldavad sageli rohkem infot kui klient vajab,
- domeeni sisemised väljad ei pea väljapoole minema,
- DTO-ga saab vastust lihtsustada,
- DTO aitab vältida tsüklilisi viiteid ja liigseid navigation property andmeid,
- DTO teeb API lepingu selgemaks.

Näiteks `PetDto` ei ole sama mis `Pet`. DTO-s on lisaks ka `SpeciesLocalized`, `BreedLocalized` ja `OwnerName`, mis on juba vastuse jaoks mugavaks tehtud.

### Näited päringu ja vastuse voost

#### Näide 1: login

1. Frontend saadab `POST /api/v1/auth/login` koos emaili ja parooliga.
2. `AuthController` kutsub `AuthService.LoginAsync`.
3. Teenus otsib kasutaja andmebaasist, kontrollib BCrypt parooli ja rollid.
4. Kui admini MFA on vajalik, kontrollitakse ka TOTP kood.
5. Edu korral luuakse access token ja refresh token.
6. Access token saadetakse JSON vastuses, refresh token salvestatakse httpOnly cookie’sse.

#### Näide 2: aja broneerimine

1. Frontend saadab `POST /api/v1/appointments` koos `CreateAppointmentDto` andmetega.
2. `AppointmentsController` kutsub `AppointmentService.CreateAsync`.
3. Teenus kontrollib, kas aeg on tulevikus, kas lõpp on pärast algust, kas kasutaja võib selle lemmiku jaoks aja teha ja kas veterinaar on vaba.
4. Kui kõik on korras, salvestatakse broneering andmebaasi.
5. Vastuseks tagastatakse loodud `AppointmentDto`.

### Swagger

Swagger on automaatne API dokumentatsioon ja testimisvaade. See näitab, millised endpointid on olemas, millist body-t nad ootavad ja millise vastuse nad annavad.

Minu projektis on Swagger arenduskeskkonnas sisse lülitatud. See on kasulik, sest:

- saan endpoint’e kiiresti testida ilma frontendita,
- näen kohe, mis DTO-d või parameetrid on vajalikud,
- saan kontrollida, kas autentimine, versioning ja vastused töötavad.

Kaitsmisel saan öelda, et Swagger on minu jaoks nagu API töölaud. Kui midagi ei tööta, saan sealt kiiresti näha, kas viga on päringus, tokenis või backendis.

## Autentimine ja JWT

JWT tähendab JSON Web Tokenit. Lihtsas keeles on see allkirjastatud andmeplokk, millega backend ütleb: see kasutaja on juba sisse loginud ja tal on need õigused.

Süsteemis on kaks tokenit:

- **access token** on lühiajaline ja kasutatakse igas API päringus,
- **refresh token** on pikema elueaga ja sellega saab access tokenit uuendada.

### Login

Kui kasutaja logib sisse:

1. backend kontrollib emaili ja parooli,
2. kui parool on õige, luuakse JWT access token,
3. refresh token salvestatakse andmebaasi hash’ina,
4. refresh token läheb browseris httpOnly cookie’sse.

### Miks refresh token on httpOnly cookie’s

See on oluline turvameede. `HttpOnly` tähendab, et JavaScript ei saa seda cookie’t otse lugeda. Nii on refresh token kaitstud näiteks XSS rünnaku vastu, kus pahatahtlik skript prooviks tokenit brauserist varastada.

Lisaks on cookie’ga mugavam teha automaatset tokeni uuendamist. Frontend ei pea refresh tokenit ise käsitsi kuskil hoidma.

### Miks tokenipõhine autentimine on parem kui lihtne session auth

Selles projektis on tokenipõhine lahendus mõistlik, sest frontend on SPA-laadne rakendus ja backend on eraldi API.

Tokenipõhise lahenduse eelised:

- backend saab olla rohkem staatiline,
- frontend ja backend saavad eraldi skaleeruda,
- access tokeni saab saata erinevatest klientidest,
- õigused on tokeni sees ja neid saab kontrollida iga päringu puhul.

Lihtsa session-authiga oleks rohkem serveripoolset seisu ja see sobiks siin vähem hästi.

### Refresh tokeni rotatsioon

Refresh tokenit ei kasutata lõpmatult. Kui kasutaja teeb refresh päringu, luuakse uus refresh token ja vana märgitakse tühistatuks. See vähendab riski, et varastatud token töötaks kaua aega.

### Rollid

Rollid on süsteemis väga tähtsad. Tokenisse lisatakse rollid ja neid kasutatakse autoriseerimisel.

Peamised rollid on:

- `Owner`
- `Vet`
- `Admin`

Backendis on lisaks policy-d, näiteks:

- `Vet`
- `Owner`
- `Admin`

`Admin` policy on eriti range, sest lisaks rollile kontrollitakse ka seda, kas kasutaja email vastab konfiguratsioonis määratud admin emailile.

### Miks admini jaoks on MFA

Admini sisselogimisel võib olla nõutud MFA ehk mitmeastmeline autentimine. Selles projektis kasutatakse TOTP-koodi loogikat.

See on oluline, sest admini konto õigused on kõige suuremad. Kui keegi saaks selle konto kätte, oleks mõju väga suur.

## Rollid ja õigused

Rollipõhine autoriseerimine tähendab, et mitte kõik kasutajad ei näe samu asju.

### Omanik

Omanik saab:

- vaadata oma profiili,
- hallata oma lemmikuid,
- broneerida aegu,
- vaadata oma arveid,
- märkida arve tasutuks.

### Veterinaar

Veterinaar saab:

- vaadata talle seotud patsiente,
- kinnitada ja lõpetada aegu,
- lõpetada visiite,
- kirjutada retsepte,
- vaadata ja hallata inventari,
- hallata veterinaari kontosid vastavalt õigustele.

### Admin

Admin saab:

- hallata kasutajaid,
- muuta rolle,
- vaadata veterinaare,
- hallata tõlkeid,
- teha auditit ja aruandlust,
- teha kõrgema õigusega operatsioone.

### Miks rollid on eraldi

Rollide eraldamine hoiab UI ja backend’i lihtsamana. Iga kasutaja näeb ainult seda, mida tal on vaja. See teeb nii kasutaja kogemuse paremaks kui ka turvalisuse tugevamaks.

## Kasutajaliides: client ja admin

Kuigi see dokument räägib backendist, on oluline mõista ka UI-vaadet, sest backend on ehitatud selle järgi, kuidas erinevad rollid kasutavad süsteemi.

### Owner vaade

Omanik näeb tavaliselt:

- oma lemmikuid,
- tulevasi ja mineviku aegu,
- arveid,
- tervise- või ajaloovaateid,
- broneerimise samm-sammulist vaadet.

Frontend routeris on selle jaoks eraldi owner-lehed.

### Vet vaade

Veterinaar näeb tavaliselt:

- tänaseid või talle määratud aegu,
- patsiente,
- inventari,
- veterinaari kontosid,
- kättesaamatuse ajavahemikke.

### Admin vaade

Admin näeb süsteemi ülevaate vaadet ja halduslehti. Admin võib frontendis kasutada ka eri töörežiime, näiteks vaadata asju veti või omaniku vaates, kuid backend kontrollib ikkagi lõplikult, mida päriselt teha tohib.

### Miks UI on rolliti eraldi

See on lihtsam kasutajale ja vähendab segadust. Omanik ei pea nägema veterinaari tööriistu ja veterinaar ei pea nägema kõiki admini aruandeid kogu aeg. Lisaks aitab see hoida ekraanid väiksemad ja mõistlikumad.

### Olulised kasutajavoogude näited

- Omanik logib sisse, lisab lemmiku, broneerib aja ja hiljem maksab arve.
- Veterinaar logib sisse, näeb oma aega, kinnitab selle, lõpetab visiidi ja kirjutab retsepti.
- Admin logib sisse, kontrollib kasutajaid, muudab rolle, vaatab tõlkeid ja auditilogisid.

## Tõlked UI-s ja andmebaasis

Süsteem toetab mitut keelt. Praegu on backendis toetatud vähemalt inglise, eesti ja vene keel.

### UI tõlked

Frontend kasutab lokaliseerimist, et nuppude, menüüde ja vaadete tekstid oleksid kasutaja keeles. Osa tõlgetest võib olla lokaalses frontend failis, osa võib tulla backendist.

### Andmebaasi tõlked

Backendis on `Translation` tabel, kus iga kirje on kindla keele, kategooria ja võtme all.

Näiteks võib võtme nimi olla midagi sellist nagu `common.save` või `auth.login`.

### Miks tõlked on kasulikud

- rakendus on kasutajasõbralikum,
- erinevad kliiniku töötajad võivad eelistada eri keeli,
- tõlkeid saab muuta ilma, et peaks kogu rakendust ümber kirjutama.

### Kuidas backend tõlkeid kasutab

`LocalizationService` vaatab `Accept-Language` headerit ja valib vastava keele. Seejärel tõlgitakse näiteks:

- lemmiku liik,
- lemmiku tõug,
- broneeringu staatus,
- visiidi staatus.

`TranslationsController` võimaldab frontendil tõlked otse andmebaasist küsida, näiteks kogu keele või kindla kategooria kaupa.

## Turvalisus ja IDOR

IDOR tähendab Insecure Direct Object Reference. Lihtsas keeles tähendab see olukorda, kus kasutaja saab otse mingi ID kaudu ligi andmetele, mida ta tegelikult näha ei tohiks.

Näiteks halb lahendus oleks see, kui omanik muudab URL-is lihtsalt ID ära ja saab teise inimese arve kätte.

### Kuidas projekt seda väldib

Ma kontrollin andmeid mitmes kohas:

- omanik näeb ainult oma lemmikuid ja arveid,
- veterinaar näeb ainult oma või talle seotud aegu ja visiite,
- adminil on eraldi policy,
- objekti kuuluvus kontrollitakse enne muutmist või lugemist.

Näiteks:

- `OwnersController` kontrollib, et lemmik kuulub sisse loginud omanikule,
- `AppointmentsController` kasutab kasutaja ID-d ja rolle, et otsustada, millised ajad üldse nähtavad on,
- `VisitsController` kontrollib, kas veterinaar on selle visiidi juures seotud või kas kasutaja on vastava lemmiku omanik,
- `InventoryController` lubab laoseisu muuta ainult veterinaaril või adminil.

### Miks backend peab kõike ise kontrollima

Frontend ei ole turvapiir. Kasutaja võib brauserist päringut muuta, Postmani kasutada või arendustööriistadega requesti võltsida. Selle pärast peab backend alati ise kontrollima:

- kas kasutaja on sisse loginud,
- mis roll tal on,
- kas see objekt kuulub talle,
- kas see tegevus on üldse lubatud.

Frontend võib aidata kasutajal õigeid nuppe näha, aga see ei asenda backend’i kontrolli.

### Lisa kaitsed

Projektis on veel mitu turvameedet:

- JWT allkirja kontroll,
- tokeni eluea kontroll,
- refresh tokeni hash andmebaasis,
- auth endpointide rate limiting,
- CORS lubab ainult sobivaid origin’e,
- logimine aitab kahtlase tegevuse tuvastamisel.

## CI/CD ja deploy

CI/CD tähendab Continuous Integration ja Continuous Deployment. Lihtsalt öeldes tähendab see, et build, testimine ja deployimine ei toimu käsitsi iga kord nullist, vaid võimalusel automaatselt.

### Kuidas see projektis töötab

Projekt on Dockeri põhine. See tähendab, et kogu süsteemi saab käivitada konteineritena:

- PostgreSQL andmebaas,
- backend API,
- frontend.

`docker-compose.yml` seob need kokku. API töötab konteineris, andmebaas samuti ning frontend kasutab API aadressi, mis on Compose’i kaudu ette antud.

### Miks Docker on kasulik

- keskkond on ühesugune nii arenduses kui deploy’l,
- sõltuvusi on lihtsam hallata,
- backend, frontend ja andmebaas saavad koos käivituda,
- on lihtsam näidata projekti õpetajale või teises masinas.

### Build, test ja deploy

Tüüpiline töövoog on selline:

1. Build: backend builditakse `dotnet` abil ja frontend builditakse eraldi.
2. Test: käivitatakse `dotnet test` ning kontrollitakse teenuseid ja integratsiooni.
3. Deploy: käivitatakse Docker Compose või ehitatakse konteinerid serveris uuesti.

### Kas selles repoos on täielik CI pipeline

Selles kaustas ma ei näe eraldi pipeline konfiguratsiooni faili, seega ma ei ütleks, et siin on täispikk automaatne CI/CD pipeline juba eraldi kirjeldatud. Aga tehniline alus selleks on olemas: Docker, testid, migrations ja konfigureeritavad keskkonnamuutujad.

### Konfiguratsioon ja keskkonnamuutujad

Deploy ja käivitamine sõltuvad mitmest seadest:

- andmebaasi connection string,
- JWT secret,
- admini email,
- MFA secret,
- CORS origins,
- ASPNETCORE_URLS ja ASPNETCORE_ENVIRONMENT.

Need on paremad keskkonnamuutujatena või konfiguratsioonifailides, mitte koodis kõvasti sees.

## Andmebaas, migrations ja seed data

Projekt kasutab PostgreSQL-i ja Entity Framework Core’i.

### `PetClinicDbContext`

`PetClinicDbContext` koondab kõik `DbSet`-id ja seosed. Seal on näiteks:

- owners,
- pets,
- veterinarians,
- appointments,
- visits,
- prescriptions,
- medication stocks,
- inventory reorders,
- invoices,
- refresh tokens,
- vet unavailabilities,
- translations,
- admin audit events.

### Miks migrations on olulised

Migration on andmebaasi skeemi versioon. Kui ma lisan uue välja või suhte, siis ma ei muuda andmebaasi käsitsi, vaid teen migration’i.

See projektis on oluline, sest skeem on arenenud mitmes etapis. Näiteks olemas on migration’id tõlgete, veterinaari auditiväljade ja admini tracking’i jaoks.

### Automaatne migration startupis

Rakenduse käivitamisel kutsutakse `Database.MigrateAsync()`. See tähendab, et backend proovib skeemi ise jooksvalt uuendada enne, kui ta päringuid teenindama hakkab.

See on arenduses mugav, sest ma ei pea iga kord käsitsi migration’eid käivitama.

### Seed data

Startupis lisatakse ka algandmed, kui andmebaas on tühi või osaliselt tühi.

Seotud algandmed võivad sisaldada näiteks:

- admin kasutaja,
- vet kasutaja,
- omanike kontod,
- näidispetsid,
- broneeringud,
- inventari kirjed,
- tõlked.

See on kasulik, sest pärast esimest käivitamist on kohe midagi vaadata ja testida.

### Olulised andmebaasi reeglid

`OnModelCreating`-us on näiteks:

- unikaalsed emailid ownerile ja veterinaarile,
- veterinaari litsentsinumbri unikaalsus,
- seosed `Owner` → `Pet`, `Pet` → `Appointment`, `Appointment` → `Visit`, `Visit` → `Invoice`,
- enumite salvestamine stringina,
- `Owner.Roles` salvestamine stringi kujul,
- UTC kuupäeva teisendused.

UTC kasutamine on tähtis, sest kliinikus on ajad väga olulised ja aega ei tohi salvestada eri ajavööndites juhuslikult erinevalt.

## Olulised meetodid ja töövoog

Allpool on lühidalt lahti seletatud peamised teenused ja meetodid, mida kaitsel tasub kindlasti osata kirjeldada.

### `AuthService`

`RegisterAsync`

- registreerib uue omaniku,
- kontrollib emaili,
- ei luba tavalise registreerimise kaudu vet rolli luua,
- hashib parooli BCryptiga.

`CreateVetAccountAsync`

- loob veterinaari konto,
- kontrollib emaili ja litsentsinumbri unikaalsust,
- kontrollib nime, parooli ja telefoninumbri formaati,
- kasutab transaktsiooni, sest korraga tuleb luua nii `Owner` kui `Veterinarian`.

`LoginAsync`

- leiab kasutaja emaili järgi,
- kontrollib parooli,
- vajadusel uuendab vana paroolihashi,
- kontrollib admini allowlist’i,
- vajadusel kontrollib MFA koodi,
- loob access tokeni ja refresh tokeni.

`RefreshTokenAsync`

- loeb refresh tokeni hash’i andmebaasist,
- kontrollib aegumist,
- teeb tokeni rotatsiooni,
- tagastab uue access tokeni ja uue refresh tokeni.

`LogoutAsync`

- märgib refresh tokeni tühistatuks,
- lõpetab kasutaja seansi refresh tokeni poole pealt.

### JWT tokeni loomise loogika

Access tokeni sees on:

- kasutaja ID ehk `sub`,
- email,
- rollid.

Token kehtib lühikest aega, umbes 5 minutit. See on teadlik valik, sest lühike eluiga vähendab riski, kui token peaks lekima.

### `AppointmentService`

`CreateAsync`

- kontrollib, et lõppaeg oleks algusajast hilisem,
- kontrollib, et aeg ei oleks minevikus,
- kontrollib, kas kasutaja tohib selle lemmiku jaoks broneeringu teha,
- kontrollib veterinaari olemasolu,
- kontrollib kattuvaid aegu,
- salvestab broneeringu transaktsioonis.

`GetUserAppointmentsAsync`

- tagastab kasutajale sobivad ajad vastavalt rollile,
- omanik näeb oma lemmikute aegu,
- vet näeb oma aegu ja mõnes vaates ka ootel järjekorda,
- admin võib näha kõiki.

`ConfirmAsync`

- lubab veterinaaril või adminil aja kinnitada,
- ei luba kinnitada tühistatud või lõpetatud aega,
- vajadusel seob veterinaari broneeringuga.

`CancelAsync`

- lubab tühistada ainult õigel kasutajal,
- ei luba tühistada juba lõpetatud aega,
- muudab staatuse `Cancelled`-iks.

### `VisitService`

`CompleteVisitAsync`

- kontrollib, et kasutaja oleks vet või admin,
- leiab visiidi või loob selle, kui UI saatis vana tüüpi ID,
- kontrollib, kas vet on selle visiidiga seotud,
- lõpetab visiidi,
- loob retseptid,
- vähendab ravimite laoseisu,
- loob arve,
- kasutab transaktsiooni, et kõik toimuks koos.

See on üks tähtsamaid meetodeid, sest siin on korraga mitu ärireeglit: õigused, laoseis, arve ja visiidi seisund.

### `TranslationService`

See teenus tegeleb tõlgete lugemise ja haldamisega.

Peamised ülesanded:

- tõlgete saamine keele kaupa,
- tõlgete saamine kategooria kaupa,
- konkreetse võtme leidmine,
- uue tõlke lisamine,
- olemasoleva muutmine,
- kustutamine,
- bulk-operatsioonid,
- keelte ja kategooriate loetelu tagastamine,
- tõlgete seedimine algandmetest.

### `LocalizationService`

See teenus loeb kasutaja keele ja teeb mõne väärtuse inimloetavaks.

Näiteks tõlgitakse:

- lemmiku liik,
- lemmiku tõug,
- broneeringu staatus,
- visiidi staatus.

Kui keeleks on inglise keel, siis väärtust üldiselt ei muudeta. Kui keel on eesti või vene, siis kasutatakse sõnastikku.

### `UserContextService`

See teenus loeb praeguse kasutaja ID ja rollid JWT-st.

See on tähtis, sest JWT claimid ei tule alati ühesuguse kujuga. Mõnes kohas võib roll olla `roles`, mõnes `role`, mõnes `ClaimTypes.Role`. Sama kehtib kasutaja ID kohta. Selle teenuse töö on need erinevad variandid ühtseks muuta.

### `AdminService`

AdminService koondab administraatori haldusloogika.

Selles teenuses on näiteks:

- kasutajate haldus,
- veterinaaride haldus,
- broneeringute ülevaade,
- arvete ülevaade,
- finantsaruanded,
- inventari aruanded,
- auditilogid,
- süsteemi tervise ja kokkuvõtete vaatamine.

### Töövoog: controller -> service -> database -> response

See on projekti peamine loogika. Näiteks `AppointmentsController` ei tee ise kogu tööd. Ta:

1. võtab päringu vastu,
2. loeb kasutaja tokenist,
3. kutsub teenust,
4. teenus teeb äriloogika ja andmebaasi töö,
5. kontroller mapib tulemuse DTO-ks,
6. vastus läheb kliendile tagasi.

See hoiab kontrolleri lihtsana ja teenuse testitavana.

### Valideerimine ja veahaldus

Projektis on valideerimist mitmes kihis:

- DTO-del on `DataAnnotations`, näiteks `[Required]`, `[MaxLength]`, `[Range]`,
- `CreatePetDto` kasutab ka `IValidatableObject`-i, et kontrollida sünniaega,
- teenused kontrollivad ärireegleid, näiteks aegade kattumist või laoseisu,
- kontrollerid püüavad vead kinni ja tagastavad sobiva HTTP staatuse.

Veahaldus on tehtud nii, et kasutaja saaks võimalikult selge vastuse:

- `NotFound` kui kirjet pole,
- `BadRequest` kui sisend on vale,
- `Unauthorized` või `Forbid` kui õigust pole,
- `Conflict` kui email või muu unikaalne väärtus on juba kasutusel.

### Transaktsioonid

Transaktsioon on kasutusel kohtades, kus mitu sammu peavad õnnestuma koos.

Näiteks:

- veterinaari konto loomisel tekivad korraga `Owner` ja `Veterinarian`,
- visiidi lõpetamisel tekivad retseptid, arve ja laoseisu muutus,
- broneeringu loomisel ja muid seotud muudatusi võib olla vaja teha ühes tervikus.

Kui midagi läheb keskel valesti, tehakse rollback, et andmebaas ei jääks poolikusse seisu.

### Logimine

Projekt kasutab Serilogit ja ka tavalist `ILogger`-it. Logitakse näiteks:

- login õnnestus või ebaõnnestus,
- translatsioonide lugemine,
- admini tegevused,
- vigade detailid,
- inventari ja broneeringute muudatused.

See on kasulik nii arenduses kui ka hiljem vigade otsimisel.

### CORS ja rate limiting

**CORS** tähendab Cross-Origin Resource Sharing. See määrab, millistelt frontend aadressidelt võib brauser backendile päringuid teha.

Selles projektis on lubatud lokaalsed aadressid ja arenduses ka LAN aadressid. See on oluline, sest frontend võib joosta eraldi porti või isegi teises masinas.

**Rate limiting** piirab päringute arvu. Auth endpointide jaoks on piirang rangem, et login’i ei saaks brute force’ida.

## Testimine

Testid on projekti oluline osa, sest see aitab kontrollida, et ärireeglid ei läheks katki.

### Kasutatud testitüübid

- **Unit testid** kontrollivad üksikuid teenuseid.
- **Integration testid** kontrollivad, kas API töötab koos päris serveri ja andmebaasiga.

### Kasutatud tööriistad

- `xUnit` testiraamistikuna,
- `FluentAssertions` kontrollide mugavamaks kirjutamiseks,
- `Moq` mock-objektide jaoks,
- `Microsoft.EntityFrameworkCore.InMemory` mõnede teenusetestide jaoks,
- `Testcontainers.PostgreSql` integratsioonitestides päris PostgreSQL konteineri jaoks.

### Mida testid kontrollivad

`AppointmentServiceTests` kontrollivad näiteks:

- et aeg saab loodud, kui veterinaar on vaba,
- et kattuva aja korral tekib viga,
- et kasutaja ei saa teise inimese lemmiku eest broneerida,
- et minevikus aega ei saa luua,
- et vale kuupäeva filter ei rikuks päringut.

`VisitServiceTests` kontrollivad näiteks:

- et visiidi lõpetamine muudab staatuse õigesti,
- et retseptid vähendavad stocki,
- et invoice tekib,
- et liiga väikese laoseisu korral tehakse rollback,
- et mitteveterinaar ei saa visiiti lõpetada.

`IntegrationTests` kontrollivad näiteks:

- login õnnestub õige parooliga,
- vale parool annab unauthorized,
- SQL injection laadne sisend ei tööta,
- health endpoint vastab.

### Miks testimine on oluline

Kaitsmisel saan öelda, et testid ei ole siin lihtsalt formaalsus. Nad kaitsevad eriti just äriloogikat: õigused, ajad, stock, arved ja autentimine. Need on kohad, kus viga oleks päriselus kõige kallim.

## Kokkuvõte

Pet Clinic backend on ehitatud nii, et kliiniku andmed oleksid turvaliselt, selgelt ja rollipõhiselt hallatavad. Süsteemi keskmes on REST API, JWT autentimine, EF Core andmebaasikiht, DTO-dega eraldatud vastused ja teenuste kaudu hoitud äriloogika.

Suurimad tugevused selles projektis on minu jaoks:

- selge kihiline arhitektuur,
- rollipõhine juurdepääs,
- IDOR-i vältimine omandi kontrolliga,
- refresh tokeni turvaline hoidmine cookie’s,
- tõlgete tugi,
- automaatsed migrations ja seed data,
- Dockeriga lihtne käivitamine,
- testid, mis katavad kõige tähtsama loogika.

Tegelik eesmärk on teha kliiniku töövoog kasutajatele lihtsaks ja samal ajal hoida backend piisavalt turvaline ja hallatav.
