# SatRTV overview

The general purpose of this application is downloading information about geostationary satellite broadcasts \(especially radio and television channels\) and convert it into text files, which are more uniform and usable by people or other applications\.

This application aquires data from three sources:


* [https://en\.kingofsat\.net/satellites\.php](https://en.kingofsat.net/satellites.php "https://en.kingofsat.net/satellites.php")
* [https://www\.lyngsat\.com](https://www.lyngsat.com/ "https://www.lyngsat.com/")
* [https://www\.flysat\.com/satlist\.php](https://www.flysat.com/satlist.php "https://www.flysat.com/satlist.php")
* [https://www\.satbeams\.com/channels](https://www.satbeams.com/channels "https://www.satbeams.com/channels")

All the sites are not official information about satellites, there are created for fans or end\-users, so the information may be outdated, incorrect or incomplete\. It is a good idea to compare information from several sources, there are differences either in transponder list or in channel list on specified transponder\. SatRTV can create list of transponder and the channels, which has the same layout, so there are easier to compare or correct using your spreadsheet software\. In the spreadsheet software, the transponder list or channel list can be addidionaly filtered or converted by user\.

# Configuration file

This application uses the text file **Config\.txt** placed in executable file directory\. The configuration is organized by **parameter=value**\. The line order in file does not affect in program working\.

Every parameter name starts with **Sat** followed by satellite number \(the first satellite has number 0\)\. The satelite count is detected by first satellite number without name \(lack of parameter has the same meaning as parameter with blank value\)\.

## Basic parameters

For satellite **0**, there are following parameters:


* **Sat0Name** \- Name of satellite displayed in application\.
* **Sat0KingOfSat** \- Satellite data URL from KingOfSat\.
* **Sat0LyngSat** \- Satellite data URL from LyngSat\.
* **Sat0FlySat** \- Satellite data URL from FlySat\.
* **Sat0SatBeams** \- Satellite data URL from SatBeams\.
* **Sat0Selected** \- This satellite is selected by default\.

For other satellites, the **0** must be replaced by next numbers, without ommiting any number or there will be included satellites prior to ommited number\. For example, for satellite **5**, there will be following parameters: **Sat5Name**, **Sat5KingOfSat**, **Sat5LyngSat**, **Sat5FlySat**, **Sat5SatBeams**, **Sat5Selected**\.

## Additional parameters

The LyngSat and FlySat table layout contains the column with several row span, which describes transponder\. Sometimes, it is possible, that there is mistake in row span number or there are unreadable rows\. In this cases, data parsing will be broken with exception or the parses data will be erronous\. The parsed data from mentioned sources has row number, which identifies table row, the one row is the one transponder\. In case of exception due to mentioned mistake, there will be displayed the current and previous row number and the parsed file will be saved\.

In the **Config\.txt** file there is possible to change number of row span for specified row \(read from parsed data text file\)\.


* **Sat0LyngSatRowSpan0Row** \- The first number of row to change in LyngSat for satellite **0**\.
* **Sat0LyngSatRowSpan0Val** \- The change value \(relative to original\) for the first row in LyngSat for satellite **0**\.
* **Sat0FlySatRowSpan0Row** \- The first number of row to change in FlySat for satellite **0**\.
* **Sat0FlySatRowSpan0Val** \- The change value \(relative to original\) for the first row in FlySat for satellite **0**\.

The next necessary changes have next numbers after **Span** element like this: **Sat0FlySatRowSpan1Row**, **Sat0FlySatRowSpan1Val**, **Sat0FlySatRowSpan2Row**, **Sat0FlySatRowSpan2Val**\.

For other satellites, the number after **Sat** element means satellite number\.

## Default configuration parameters

You can specify the default configuration parameters used in creating transponder and channel lists\. For each parameter, the value can be **0** or **1**:


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

## Transponder list and channel list layout

You can specify field layout in ultimate transponder list and channel list\. For this parameter, the value is the list of fields separated by **&#124;** character\.


* **SetTransFields** \- Transponder list layout, available fields:
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
  * **Freq** \- Transponder frequency
  * **Pol** \- Transponder polarization
  * **Beam** \- Transponder beam name
  * **SID** \- Channel service identifier
  * **Type** \- Channel type \(R/TV/IMG/DATA\)
  * **Name** \- Channel name
  * **Lang** \- Channel languages
  * **FTA** \- Channel is not encrypted \(free to air\)

By modifying the list, you can specify order or include only some of all fields\.

# Using application

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

The first two files stores almost all available data\. If files has incorrect layout or parsing process is broken due to incorrectness HTML file, you can manually correct this HTML file and repeat parse\. For LyngSat and FlySat you can change row spanning for tables when necessary as described in configuration file desctiption\. After hanging configuration file, you have to restart SatRTV application\. Eventually, you can manually correct output text files\.

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

The image height is the same as number of selected satellites\. The image width is the same as band width in MHz with incremented by\. The background is black, the pixels may have one of following colors:


* **Red** \- Transponder with horizontal or right polarization\.
* **Green** \- Transponder with vertical or left polarization\.
* **Yellow** \- Two transponders with opposite polarizations\.

# Errors and exceptions

SatRTV was successfully tested with data about following satellites \(orbital positions\):


* **13\.0E \(Hotbird 13B/13C/13E\)** \- FlySat: Radio non\-FTA stations are on separate page, which was not tested and must be treated as separate satellite\.
* **16\.0E \(Eutelsat 16A\)**
* **19\.2E \(Astra 1 group\)** \- FlySat: Radio non\-FTA stations are on separate page, which was not tested and must be treated as separate satellite\.

If you experience any uncommon errors in parsing, which are not due to errors or mistakes on source data pages, report it as bug providing satellite link page causing the problem\.


