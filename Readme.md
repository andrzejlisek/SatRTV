# SatRTV overview

The general purpose of this application is downloading information about geostationary satellite broadcasts \(especially radio and television channels\) and convert it into text files, which are more uniform and usable by people or other applications\.

This application aquires data from three sources, which are enumerated from 1 to 4:


1. [https://en\.kingofsat\.net/satellites\.php](https://en.kingofsat.net/satellites.php "https://en.kingofsat.net/satellites.php")
2. [https://www\.lyngsat\.com](https://www.lyngsat.com/ "https://www.lyngsat.com/") 
3. [https://www\.flysat\.com/satlist\.php](https://www.flysat.com/satlist.php "https://www.flysat.com/satlist.php")
4. [https://www\.satbeams\.com/channels](https://www.satbeams.com/channels "https://www.satbeams.com/channels")

All the sites are not official information about satellites, there are created for fans or end\-users, so the information may be outdated, incorrect or incomplete\. It is a good idea to compare information from several sources, there are differences either in transponder list or in channel list on specified transponder\. SatRTV can create list of transponder and the channels, which has the same layout, so there are easier to compare or correct using your spreadsheet software\. In the spreadsheet software, the transponder list or channel list can be addidionaly filtered or converted by user\.

The other ability and purpose is the create enumerated list of stations, which is corresponds to channel numbers in your receiver\. It can merge chanell names and languages from several or all four sources\.

SatRTV requires \.NET or MONO library to run and use, it was implemented and tested on Windows and Ubuntu Linux\.

## Enigma 2 receivers

The additional ability of SatRTV is creating bouquet list for satellite receivers based on Enigma 2 system and compatible\.

This program does not modify or remove any Enigma 2 configuration files\. It creates additional files and you have to rename and replace original files manually\.

in some cases, tuner erroneously classifies some channels, for example:


* Encrypted channel as FTA channel\.
* Radio channel as TV channel\.
* Data channel as radio channel\.

The internet data sources has usually good classified channels by encryption \(FTA or encrypted\) and type \(radio or TV or data\)\. The SatRTV makes creating bouquet list mach easier than creating them manually\.

# Configuration file

This application uses the text file **Config\.txt** placed in executable file directory\. The configuration is organized by **parameter=value**\. The line order in file does not affect in program working\.

## Default configuration parameters

You can specify the default configuration parameters used in creating transponder and channel lists\. For each parameter, the value can be **0** or **1**:


* **DataPath** \- Path for data files, which will be downloaded and created**\.**
* **SetBand1** \- Include C band transponders \(3400MHz \- 4200MHz\)\.
* **SetBand2** \- Include Ku band transponders \(10700MHz \- 12750MHz\)\.
* **SetBand3** \- Include Ka band transponders \(18200MHz \- 22200MHz\)\.
* **SetTypeR** \- Include R\-type channels \(audio only\) in creating list\.
* **SetTypeTV** \- Include TV\-type channels \(video and audio\)\.
* **SetTypeIMG** \- Include IMG\-type channels \(video only\)\.
* **SetTypeDATA** \- Include DATA\-type channels \(other than R, TV and IMG\)\.
* **SetFTA** \- Include non\-encrypted channels \(free to air\) only\.
* **SetTransWithChan** \- Include only transponders in transponder list, which contains at least one channel included in channel list\.

All above settings can be changed during application running\.

## Enigma 2 related settings

The additional settings, which are related to Enigma2, are following:


* **EnigmaPath** \- Path, where E2 setting files are placed\. You must download files from receiver using appropriate software\.
* **EnigmaDatabase** \- Name of file, which consists of transponder and channel informations, usually **lamedb**\.
* **EnigmaFrequency** \- Transponder frequency tolerance used while matching channel list to E2 database items\.

## Transponder list and channel list layout

You can specify field layout in ultimate transponder list and channel list\. For this parameter, the value is the list of fields separated by **&#124;** character\.


* **SetTransFields** \- Transponder list layout, available fields:
  * **No** \- Channel number \(generated as blank field\)
  * **Freq** \- Frequency
  * **Pol** \- Polarization
  * **SR** \- Symbol rate
  * **FEC** \- Forward error correction
  * **Txp** \- Identifier or name
  * **Beam** \- Beam name
  * **R** \- Number of R channels included on channel list
  * **TV** \- Number of TV channels included on channel list
  * **IMG** \- Number of IMG channels included on channel list
  * **DATA** \- Number of DATA channels included on channel list
  * **TOTAL** \- Total number channels included on channel list
* **SetChanFields** \- Channel list layout, available fields:
  * **No** \- Channel number \(generated as blank field\)
  * **Freq** \- Transponder frequency
  * **Pol** \- Transponder polarization
  * **Beam** \- Transponder beam name
  * **SID** \- Channel service identifier
  * **Type** \- Channel type \(R/TV/IMG/DATA\)
  * **Name** \- Channel name
  * **Lang** \- Channel languages
  * **FTA** \- Channel is not encrypted \(free to air\)

By modifying the list, you can specify order or include only some of all fields\.

## Enumerated list

After creating the transponder list and channel list, you can create the enumerated list\. The function uses the following parameters:


* **SetTransNoListFields** \- Field list in enumerated transponder list, separated by **&#124;**\.
* **SetTransNoListMode** \- Field value aquiring mode corresponding to fields in **SetTransNoListFields**, separated by **&#124;**, for each field possible value is **0** or **1**, described below\.
* **SetChanNoListFields** \- Field list in enumerated transponder list, separated by **&#124;**\.
* **SetChanNoListMode** \- Field value aquiring mode corresponding to fields in **SetChanNoListFields**, separated by **&#124;**, for each field possible value is **0** or **1** or **2**, described below\.

The **SetTransNoListMode** and **SetChanNoListMode** is the digital value list separated by **&#124;**\. These value are the following meaning:


* **0** \- Write the first occurence
* **1** \- Write all occurences separated by **&#124;**, the exactly repeated occurences will not written\.
* **2** \- Reserved to **Lang** field, where language codes are in **\[\]**\. Merge language codes without repeated language codes and sort alphabetically \(usable only in **Lang** field in channel list\)\.

The value is not applicable to **No** field, because the list is generated based on the **No** field, so the value for the field does not impact the list creating\.

## Sattellite parameters

Every parameter name starts with **Sat** followed by satellite number \(the first satellite has number 0\)\. The satelite count is detected by first satellite number without name \(lack of parameter has the same meaning as parameter with blank value\)\.

For satellite **0**, there are following parameters:


* **Sat0Name** \- Name of satellite displayed in application\.
* **Sat0KingOfSat** \- Satellite data URL from KingOfSat\.
* **Sat0LyngSat** \- Satellite data URL from LyngSat\.
* **Sat0FlySat** \- Satellite data URL from FlySat\.
* **Sat0SatBeams** \- Satellite data URL from SatBeams\.
* **Sat0Selected** \- This satellite is selected by default\.

For other satellites, the **0** must be replaced by next numbers, without ommiting any number or there will be included satellites prior to ommited number\. For example, for satellite **5**, there will be following parameters: **Sat5Name**, **Sat5KingOfSat**, **Sat5LyngSat**, **Sat5FlySat**, **Sat5SatBeams**, **Sat5Selected**\.

## Enigma 2 parameters

If you want to create or sort Enigma 2 bouquet, you have to set additional parameters for satellites, which you want to support:


* **Sat0EnigmaId** \- The internal E2 satellite identifier, usually cretated from position, for example, the Astra satellite placed on 19\.2E is identified as 192\.
* **Sat0EnigmaBouquetTv** \- The file name containing TV channel bouquet\.
* **Sat0EnigmaBouquetRadio** \- The file name containing radio channel bouquet\.

For other satellites, the number after **Sat** element means satellite number\.

# Creating channel list

This application runs in text console, displays satellites \(the selected satellite will be indicated by **\#**\), current configuration and waits for command from user\. Every command consists of one word \(input is not case sensitive\) or has word and parameter separated by space\. If entered command is not correct or not supported, there wil be displayed list of all possible commands\. After command executed, the satellite list and configuration will be redisplayed\.

## Introduction

Before perform any operation, you can select or deselect any satellite using **SELECT N** command, where **N** is satellite number\. The selected satellites are indicated by **\#** and included in downloading, parsing, creating list and creating image\.

To close application, enter the **EXIT** command\.

## Downloading data

The first step is download current data\. To preform this, execuse on of following commands, and data will be saved in subdirectory in application directory:


* **DOWNLOAD 1** \- Download data from KingOfSat and save in **Data1** directory\.
* **DOWNLOAD 2** \- Download data from LyngSat and save in **Data2** directory\.
* **DOWNLOAD 3** \- Download data from FlySat and save in **Data3** directory\.
* **DOWNLOAD 4** \- Download data from SatBeams and save in **Data4** directory\.

There will be downloaded only HTML files, one per satellite, without additional files\. Every file will be named as **DataNNN\.html** where **NNN** is number of satellite in configuration file, which is presented as three characters \(leaded with **0** if necessay\)\.

## Parsing data

Where data are downloaded, you have to parse this data to create source list of transponders and channels\. This will be performed using following commands:


* **PARSE 1** \- Parse KingOfSat data\.
* **PARSE 2** \- Parse LyngSat data\.
* **PARSE 3** \- Parse FlySat data\.
* **PARSE 4** \- Parse SatBeams data\.

The parsed data are saved in three text files per earch selected satellite \(the **NNN** is the satellite number\):


* **TransDataNNN\.txt** \- Transponder data\.
* **ChanDataNNN\.txt** \- Channel data\.
* **BeamNNN\.txt** \- All unique transponder beams used in the satellite, sorted alphabetically\.

The first two files stores almost all available data\. If files has incorrect layout or parsing process is broken due to incorrectness HTML file, you have manually correct the HTML file and repeat parse\. Eventually, you can manually correct output text files\.

The third file stores all beam names, which occured in this satellite\.

At this step, each channel will be assigned to one of four types based on checking if contains audio or video stream identifier:


* **R** \- Audio stream only \(usually radio channel\)\.
* **TV** \- Audio and video streams \(usually television channel\)\.
* **IMG** \- Video stream only \(usually test and advertisment static images\)\.
* **DATA** \- Channel without audio and video stream \(usually useless in ordinary satellite radio and television receiver\)\.

## APID and language code parse

One channel can contain several audio streams, every stream should have APID identifier and can have specified none or one or several language codes\. In the parsed channel data, the every language code is specified between **\[** and **\]** characters\. For example:


* **1\[eng\]** \- One audio stream available, in **eng** language\.
* **2&#124;3\[ger\]** \- Two available audio stream, the **2** stream has not specified language and the **3** stream has specified **ger** language\.
* **4\[ara\]\[hun\]&#124;5\[deu\]\[esp\]** \- Two audio streams, the **4** is in **ara** or **hun** language, the **5** is in **deu** or **esp** language\.
* **6&#124;7&#124;8** \- Three audio streams available, the language of this streams are not specified\.

In some cases, the audio data from source data can not be automatically parsed, so the conversion is impossible\. In such case there will be displayed the text from source data and you will have to manually input the parse equivalent, which meets the convention mentioned above\.

## Creating lists

Creating list are the last operation in creating ultimate transponder list and channel list\. This operation can be configured, after executing configuration command, the current configuration will be redisplayed\.

You can configure creating list using following commands:


* **BAND N** \- Include or exclude specified band, **1** for C band, **2** for Ku band, **3** for Ka band\.
* **FTA** \- Include all channels or free\-to\-air \(not encrypted\) channels only\.
* **TRANSCH** \- Include all transponders or transponders containing listed channels only\.
* **TYPE X** \- Include or exclude channel type\. The possible types are **R**, **TV**, **IMG**, **DATA**\.

The creating list uses the **BeamNNN\.txt**\. You can specify, which beams will be included by editing the file\. The beam names, which are in the file, but not exists in this satellite are ignored\. By default, the file contains all available beams\.

After change configuration and satellite selecting, you can create list using following commands:


* **LIST 1** \- Create list for KingOfSat\.
* **LIST 2** \- Create list for LyngSat\.
* **LIST 3** \- Create list for FlySat\.
* **LIST 4** \- Create list for SatBeams\.

The transponder and channel list created by above commands has the same layout regardless data source, the layout can be configured in **Config\.txt** file\.

This operation will create following files \(the **NNN** is the satellite number\):


* **TransListNNN\.txt** \- Transponder list based on **TransDataNNN\.txt**, which transponders are sorted by frequency, polarization \(order is **HVRL**\) and beam\. The transponder is identified by frequency rounded to 1 MHz and polarization, so if there are several transponders having the same frequency, polarization and beam, all of them will be treated as one transponder\. This file also contains SR, FEC, Txp data and number of channel of each type\. In this number, there are included listed channel, not all available channels\.
* **ChanListNNN\.txt** \- Channel list based on **ChanDataNNN\.txt**, which channels are sorted by frequency, polarization \(order is **HVRL**\), beam and SID identifier\. In file there is saved frequency, transponder, SID, type, name, languages \(each language code between **\[** and **\]**\) and FTA information \(**Yes** or **No**\)\.

You can use this files for further action, for example:


* Import into your own software or script, which process this files as you needed\.
* Import into your spreadsheed software and filter language, type etc\.
* Paste into spreadsheed software side by side and compare the same transponder list or channel list from miscellaneous sources\.

## Creating transponder frequency picture

After generating lists, there is possible to visualize transponder frequencies\. For example, this map allows check, if two satellites has the same transponder frequencies \(very important when the salellites are very close to earch other\)\.

To create these images, you have to select satellites to include in image and execute one of following commands:


* **TRANSIMG 1** \- Create image for KingOfSat\.
* **TRANSIMG 2** \- Create image for LyngSat\.
* **TRANSIMG 3** \- Create image for FlySat\.
* **TRANSIMG 4** \- Create image for SatBeams\.

This operation will generate the following files:


* **TransFreq1\.png** \- Image for C band
* **TransFreq2\.png** \- Image for Ku band
* **TransFreq3\.png** \- Image for Ka band

These images are based on information from **TransListNNN\.txt** \(the **NNN** is the satellite number\) files for selected satellites\.

The image height is the same as number of selected satellites\. The image width is the same as band width in MHz with incremented by 1\. The background is black, the pixels may have one of following colors:


* **Red** \- Transponder with horizontal or right polarization\.
* **Green** \- Transponder with vertical or left polarization\.
* **Yellow** \- Two transponders with opposite polarizations\.

## Creating enumerated channel list

The last ability is to create the enumerated list for every satellite\. This list is usable for example, for search the item number in your receiver by station name or language\. If you have tuned your receiver and you have generated channel list \(**ChanListNNN\.txt** file\) with the **No** field, which is blank, You have to manually input item number, in which the channel is stored\. At the items, which are not stored in your receiver, leave blank the **No** field\. The **No** can not be repeated or input out of order\. In case, where the item order does not meet the item order in your receiver, reorder the items in the file\. The limitations are introduced to detect some mistakes in item number entering\.

After fill in the **No** field, you have to save the file and run **CHANNO S** command\. In this command, the **S** means the concatenated source numbers in priority order\. The characters other than digits from **1** to **4** will be ignored\. The repeated **1**\-**4** characters will be also ignored\. For example:


* **CHANNO 1234** \- Get channels from all sources in this order: KingOfSat, LyngSat, FlySat and SatBeams\.
* **CHANNO 1** \- Get channels from KingOfSat only\.
* **CHANNO 31** \- Get channels from FlySat followed by KingOfSat\.

This command will create **ChanListNNN\.txt** file in the application directory, where there is the **Config\.txt** file\. The field list for this list is defined by the **SetChanNoListFields** parameter\. The all fields mentioned as the value must be exist in the every used **ChanListNNN\.txt** file in **DataS** directory\.

The **SetChanNoListMode** parameter corresponds with the **SetChanNoListFields** parameter and determines the value getting type for each field:


* **0** \- Write the first occurence
* **1** \- Write all occurences separated by **&#124;**, the exactly repeated occurences will not written\.
* **2** \- Reserved to **Lang** field, where language codes are in **\[\]**\. Merge language codes without repeated language codes and sort alphabetically\.

The **SetChanNoListMode** value is not applicable to **No** field, because the list is generated based on the **No** field\.

## Creating enumerated transponder list

You can create the enumerated transponder list by exactly the same principle as the enumerated thannel list creating\. It uses the **TransListNNN\.txt** file from **DataS** directory and creates the **TransListNNN\.txt** file\. To to this you have to execute the **TRANSNO S** command\. It uses the **SetTransNoListFields** and **SetTransNoListMode** parameters from Config\.txt, but the 2 value for **SetTransNoListMode** is not usable in the transponder list\.

# Creating bouquets for E2 receiver

If you create channel list using **LIST S** command, you can create the buquet, whis is usable in your satellite receiver\. SatRTV does not creating channel list itself, it only creates bouquet using existing E2 channel database and channel list created by SatRTV\.

SatRTV does not modify any of E2 configuration fies\. It will create files and you will have to appropriate remove original file and rename created file to use in Enigma 2 receiver\.

## Preparing E2 channel database

At the first, you have to fully scan satellite for all channels\. It is recommended to include non\-FTA channels and data channels\. You can perform blind scan or use transponder list\. If you want to receive from several satellites, repeat this scaning for every satellite\.

Then, download setting files from receiver to computer\. For every satellite, you have to create at least one bouquet file \(for radio channels only or TV channels only\), you can leave the file blank\. If you want to have both radio and TV channels, you have to create two bouquet files\.

## Configuring SatRTV

You have to configure parameters related to Enigma 2, at the first, set the **EnigmaPath** to path, where you placed files downloaded from tuner\. Then, for every satellite, you have to configure **Sat0EnigmaId**\. The **Sat0EnigmaBouquetTv** and **Sat0EnigmaBouquetRadio** are optional\. For example, if you want to create on bouquet, which contains TV channels and you do not want to have radio channels, leave the **Sat0EnigmaBouquetRadio** parameter blank or unset\.

Usually, the bouquet files has the **\.tv** or **\.radio** extension\. The **Sat0EnigmaId** value van be viewed in **EnigmaDatabase** file \(usually **lamedb**\)\. In the **lamedb** file, from `transponders` word to first occurence of `end` word, there are defined transponders\. Each transponder is defined by three lines, the second lines begins with `c` or `s` character\. The identifier is between fourth and fifth colon\.

Also, you have to check the **SetChanFields** parameter\. The bouquet creator uses channel list and channel list must contain at least following fields: **Freq**, **Pol**, **SID**, **Type**\. Other fields can exist, but values of them will not be used\.

## Creating channel list

At the next step, create the channel list as described in **Creating channel list** chapter\. You can also create or edit the cannel list manually\. Values of fields other than **Freq**, **Pol**, **SID** and **Type** will not be user, so there are not important\. The file must have the name **ChanListXXX\.txt** in **DataS** subdirectory, where **XXX** is the satellite number and **S** is the number of source, from **1** to **4**\.

## Creating the bouquets

To create bouquet, perform the **ENIGMALIST S** command, where S is a number from 1 to 4\. This operation will read the bouquet name from provided bouquet file \(the name is saved inside the file\) and will create two files in the E2 settins directory, but with additional extensions:


* **\.txt** \- The new bouqued file, compatible as original file replacement\.
* **\.log** \- The log file\.

For every channel in **ChanListXXX\.txt**, the SatRTV will attempt to match the nearest channel from E2 database\. Found channel must match all the following conditions:


* The same SID number\.
* The same transponder polarity\.
* The matching channel type:
  * In TV bouquet: **TV**, **IMG**\.
  * In radio bouquet: **R**\.
* The transponder frequency differs at most the value set as **EnigmaFrequency**\.

If more than one channel matches this conditions, there will be selected this channel, to which the frequency difference is the smallest\. If there two channels have the same frequency difference, the channel will not be selected and the fact will be denoted as **abiguous channel** in log file\.

The log file will have the following fields: **Freq**, **Pol**, **SID**, **Type**, **Comment**\. The first four field value will be copied from **ChanListXXX\.txt**\. The last is the comment about matching channel from database and can have the following values:


* **Channel found** \- For this item there is found matching channel\.
* **Type mismatch** \- The channel type denoted in **ChanListXXX\.txt** file does not match the bouquet type, the channel type denoted in E2 database will not matter\. It occurs in the following cases:
  * In the TV bouquet: for **R** and **DATA** channels\.
  * In the radio bouquet: for **TV**, **IMG** and **DATA** channels\.
* **Channel not exists** \- For the item there is not exist the matching channel in E2 database\.
* **Ambiguous channel** \- There are more than one matching channel\.

The bouquet will be sorted by the following order:


* Transponder frequency\.
* Polarity \(H, V, L, R\), if there are several transponders with the same frequency,
* Channel SID within one transponder\.

## Uploading to receiver

The new bouquet will have the **\.txt** extension\. You can remove the **\.log** file\. To finish, remove the original bouquet file \(provided as **Sat0EnigmaBouquetTv** or **Sat0EnigmaBouquetRadio** parameter\) and remove the **\.txt** extension from the new file\. The new file will have the same name as old file\.

Then, you can upload the setting files into your satellite receiver\.

# Sorting the bouquets

If you have the bouquet created manually or using the other software, you can order this bouquet by frequency, polarity and SID\. To do this, prepare settingand E2 files in the same way as for creating bouquets\.

Insteat of download and make the channel list, perform the **ENIGMASORT** command\. There will be created the additional file with **\.txt** extension\. You have to remove the origial bouqued file and remove the **\.txt** extenstion from new file name\.

Then, you can upload E2 files into receiver\.

# Errors and exceptions

SatRTV was successfully tested with data about following satellites \(orbital positions\):


* **13\.0E \(Hotbird 13B/13C/13E\)** \- FlySat: Radio non\-FTA stations are on separate page, which was not tested and must be treated as separate satellite\.
* **16\.0E \(Eutelsat 16A\)**
* **19\.2E \(Astra 1 group\)** \- FlySat: Radio non\-FTA stations are on separate page, which was not tested and must be treated as separate satellite\.

If you experience any uncommon errors in parsing, which are not due to errors or mistakes on source data pages, report it as bug providing satellite link page causing the problem\.




