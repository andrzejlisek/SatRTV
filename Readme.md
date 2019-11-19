# SatRTV overview

The general purpose of this application is downloading information about geostationary satellite broadcasts (especially radio and television channels) and convert it into text files, which are more uniform and usable by people or other applications.

This application aquires data from three sources:
* [https://en.kingofsat.net/satellites.php](https://en.kingofsat.net/satellites.php)
* [https://www.lyngsat.com/](https://www.lyngsat.com/)
* [https://www.flysat.com/satlist.php](https://www.flysat.com/satlist.php)



All the sites are not official information about satellites, there are created for fans or end-users, so the information may be outdated, incorrect or incomplete. It is a good idea to compare information from several sources, there are differences either in transponder list or in channel list on specified transponder. SatRTV can create list of transponder and the channels, which has the same layout, so there are easier to compare or correct using your spreadsheet software. In the spreadsheet software, the transponder list or channel list can be addidionaly filtered or converted by user.

# Configuration file

This application uses the text file __Config.txt__ placed in executable file directory. The configuration is organized by __parameter=value__. The line order in file does not affect in program working.

Every parameter name starts with __Sat__ followed by satellite number (the first satellite has number 0). The satelite count is detected by first satellite number without name (lack of parameter has the same meaning as parameter with blank value).

## Basic parameters

For satellite 0, there are following parameters:
* __Sat0Name__ - Name of satellite displayed in application.
* __Sat0KingOfSat__ - Satellite data url from KingOfSat.
* __Sat0LyngSat__ - Satellite data url from LyngSat.
* __Sat0FlySat__ - Satellite data url from FlySat.
* __Sat0Selected__ - This satellite is selected by default.

For other satellites, the __0__ must be replaced by next numbers, without ommiting any number or there will be included satellites prior to ommited number. For example, for satellite 5, there will be following parameters: __Sat5Name__, __Sat5KingOfSat__, __Sat5LyngSat__, __Sat5FlySat__, __Sat5Selected__.

## Additional parameters

The LyngSat and FlySat table layout contains the column with several row span, which describes transponder. Sometimes, it is possible, that there is mistake in row span number or there are unreadable rows. In this cases, data parsing will be broken with exception or the parses data will be erronous. The parsed data from mentioned sources has row number, which identifies table row, the one row is the one transponder. In case of exception due to mentioned mistake, there will be displayed the current and previous row number and the parsed file will be saved.

In the __Config.txt__ file there is possible to change number of row span for specified row (read from parsed data text file).

* __Sat0LyngSatRowSpan0Row__ - The first number of row to change in LyngSat for satellite 0.
* __Sat0LyngSatRowSpan0Val__ - The change value (relative to original) for the first row in LyngSat for satellite 0.
* __Sat0FlySatRowSpan0Row__ - The first number of row to change in FlySat for satellite 0.
* __Sat0FlySatRowSpan0Val__ - The change value (relative to original) for the first row in FlySat for satellite 0.

The next necessary changes have next numbers after __Span__ element like this: __Sat0FlySatRowSpan1Row__, __Sat0FlySatRowSpan1Val__, __Sat0FlySatRowSpan2Row__, __Sat0FlySatRowSpan2Val__.

For other satellites, the number after __Sat__ element means satellite number.

## Default configuration parameters

You can specify the default configuration parameters used in creating transponder and channel lists. For each parameter, the value can be __0__ or __1__:

* __SetBand1__ - Include C band transponders (3400MHz -  4200MHz).
* __SetBand2__ - Include Ku band transponders (10700MHz - 12750MHz).
* __SetBand3__ - Include Ka band transponders (18200MHz - 22200MHz).
* __SetTypeR__ - Include R-type channels (audio only) in creating list.
* __SetTypeTV__ - Include TV-type channels (video and audio).
* __SetTypeIMG__ - Include IMG-type channels (video only).
* __SetTypeDATA__ - Include DATA-type channels (other than R, TV and IMG).
* __SetFTA__ - Include non-encrypted channels (free to air) only.
* __SetTransWithChan__ - Include only transponders in transponder list, which contains at least one channel included in channel list.

All above settings can be changed during application running.

## Transponder list and channel list layout

You can specify field layout in ultimate transponder list and channel list. For this parameter, the value is the list of fields separated by "|" character.

* __SetTransFields__ - Transponder list layout, available fields:
  * __Freq__ - Frequency
  * __Pol__ - Polarization
  * __SR__ - Symbol rate
  * __FEC__ - Forward error correction
  * __Txp__ - Identifier or name
  * __Beam__ - Beam name
  * __R__ - Number of R channels included on channel list
  * __TV__ - Number of TV channels included on channel list
  * __IMG__ - Number of IMG channels included on channel list
  * __DATA__ - Number of DATA channels included on channel list
  * __TOTAL__ - Total number channels included on channel list
* __SetChanFields__ - Channel list layout, available fields:
  * __Freq__ - Transponder frequency
  * __Pol__ - Transponder polarization
  * __Beam__ - Transponder beam name
  * __SID__ - Channel service identifier
  * __Type__ - Channel type (R/TV/IMG/DATA)
  * __Name__ - Channel name
  * __Lang__ - Channel languages
  * __FTA__ - Channel is not encrypted (free to air)

By modifying the list, you can specify order or include only some of all fields.

# Using application

This application runs in text console, displays satellites (the selected satellite will be indicated by __#__), current configuration and waits for command from user. Every command consists of one word (input is not case sensitive) or has word and parameter separated by space. If entered command is not correct or not supported, there wil be displayed list of all possible commands. After command executed, the satellite list and configuration will be redisplayed.

## Introduction

Before perform any operation, you can select or deselect any satellite using __SELECT N__ command, where __N__ is satellite number. The selected satellites are indicated by __#__ and included in downloading, parsing, creating list and creating image.

To close application, enter the __EXIT__ command.

## Downloading data

The first step is download current data. To preform this, execuse on of following commands, and data will be saved in subdirectory in application directory:
* __DOWNLOAD 1__ - Download data from KingOfSat and save in __Data1__ directory.
* __DOWNLOAD 2__ - Download data from LyngSat and save in __Data2__ directory.
* __DOWNLOAD 3__ - Download data from FlySat and save in __Data3__ directory.

There will be downloaded only HTML files, one per satellite, without additional files. Every file will be named as __DataN.html__ where __N__ is number of satellite.

## Parsing data

Where data are downloaded, you have to parse this data to create source list of transponders and channels. This will be performed using following commands:
* __PARSE 1__ - Parse KingOfSat data.
* __PARSE 2__ - Parse LyngSat data.
* __PARSE 3__ - Parse FlySat data.

The parsed data are saved in three text files per earch selected satellite (the __N__ is the satellite number):
* __TransDataN.txt__ - Transponder data.
* __ChanDataN.txt__ - Channel data.
* __BeamN.txt__ - All unique transponder beams used in the satellite, sorted alphabetically.

The first two files stores almost all available data. If files has incorrect layout or parsing process is broken due to incorrectness HTML file, you can manually correct this HTML file and repeat parse. For LyngSat and FlySat you can change row spanning for tables when necessary as described in configuration file desctiption. After hanging configuration file, you have to restart SatRTV application. Eventually, you can manually correct output text files.

The third file stores all beam names, which occured in this satellite.

At this step, each channel will be assigned to one of four types based on checking if contains audio or video stream identifier:
* __R__ - Audio stream only (usually radio channel).
* __TV__ - Audio and video streams (usually television channel).
* __IMG__ - Video stream only (usually test and advertisment static images).
* __DATA__ - Channel without audio and video stream (usually useless in ordinary satellite radio and television receiver).

## Creating lists

Creating list are the last operation in creating ultimate transponder list and channel list. This operation can be configured, after executing configuration command, the current configuration will be redisplayed.

You can configure creating list using following commands:
* __BAND N__ - Include or exclude specified band, __1__ for C band, __2__ for Ku band, __3__ for Ka band.
* __FTA__ - Include all channels or free-to-air (not encrypted) channels only.
* __TRANSCH__ - Include all transponders or transponders containing listed channels only.
* __TYPE X__ - Include or exclude channel type. The possible types are __R__, __TV__, __IMG__, __DATA__.

The creating list uses the __BeamN.txt__. You can specify, which beams will be included by editing the file. The beam names, which are in the file, but not exists in this satellite are ignored. By default, the file contains all available beams.

After change configuration and satellite selecting, you can create list using following commands:
* __LIST 1__ - Create list for KingOfSat.
* __LIST 2__ - Create list for LyngSat.
* __LIST 3__ - Create list for FlySat.

The transponder and channel list created by above commands has the same layout regardless data source, the layout can be configured in __Config.txt__ file.

This operation will create following files (the __N__ is the satellite number):
* __TransListN.txt__ - Transponder list based on __TransDataN.txt__, which transponders are sorted by frequency, polarization (order is __HVRL__) and beam. The transponder is identified by frequency rounded to 1 MHz and polarization, so if there are several transponders having the same frequency, polarization and beam, all of them will be treated as one transponder. This file also contains SR, FEC, Txp data and number of channel of each type. In this number, there are included listed channel, not all available channels.
* __ChanListN.txt__ - Channel list based on __ChanDataN.txt__, which channels are sorted by frequency, polarization (order is __HVRL__), beam and SID identifier. In file there is saved frequency, transponder, SID, type, name, languages (each language code between __[__ and __]__) and FTA information (__Yes__ or __No__).

You can use this files for further action, for example:
* Import into your own software or script, which process this files as you needed.
* Import into your spreadsheed software and filter language, type etc.
* Paste into spreadsheed software side by side and compare the same transponder list or channel list from miscellaneous sources.

## Creating transponder frequency picture

After generating lists, there is possible to visualize transponder frequencies. For example, this map allows check, if two satellites has the same transponder frequencies (very important when the salellites are very close to earch other).

To create these images, you have to select satellites to include in image and execute one of following commands:
* __TRANSIMG 1__ - Create image for KingOfSat.
* __TRANSIMG 2__ - Create image for LyngSat.
* __TRANSIMG 3__ - Create image for FlySat.

This operation will generate the following files:
* __TransFreq1.png__ - Image for C band
* __TransFreq2.png__ - Image for Ku band
* __TransFreq3.png__ - Image for Ka band

These images are based on information from __TransListN.txt__ (the __N__ is the satellite number) files for selected satellites.

The image height is the same as number of selected satellites. The image width is the same as band width in MHz with incremented by. The background is black, the pixels may have one of following colors:
* __Red__ - Transponder with horizontal or right polarization.
* __Green__ - Transponder with vertical or left polarization.
* __Yellow__ - Two transponders with opposite polarizations.

# Errors and exceptions

SatRTV was successfully tested with data about following orbital positions:
* __7.0E__
* __9.0E__ - FlySat: __Eutelsat 9B__ only
* __10.0E__
* __13.0E__ - FlySat: Radio non-FTA stations are on separate page, which was not tested and must be treated as separate satellite.
* __16.0E__
* __19.2E__ - FlySat: Radio non-FTA stations are on separate page, which was not tested and must be treated as separate satellite.
* __23.5E__
* __28.2E__

If you experience any uncommon errors in parsing, which are not due to errors or mistakes on source data pages, report it as bug providing satellite link page causing the problem.
