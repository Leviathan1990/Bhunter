*.bin extractor tool V1.0
Designed for the game "Bhunter"
Moddb: https://www.moddb.com/games/bhunter
By: Krisztian Kispeti.

A simple extractor tool that extracts the contents of the *.bin (.dat) and *.lev archives.

Fileformats stored in archive:
=
.BSP (? Source engine files [map editor files?]) //investigation needed!
.TXT (text files)
.Wav (audio files)
.PCX (old image format)
.LEV (map editor files are stored in it)
.PTH (?)
.TGA 

The GUI version of the extractor can be downloaded from Moddb site.

File structure for *.bin:
=
//Archive header
4 - Number of files
//Details directory for each files:
248 - Filename (null terminated, filled with junk and nulls)
4 - File Offset
4 - End File Offset
//File Data
//for each files
X - File Data

File structure for *.lev:
=
//Archive header
4 - Number of files  (There are exactly 5815 files in the *.test.lev file)
//Details directory for each files:
24 - Filename (null terminated, filled with junk and nulls)
4 - File Offset
4 - End File Offset
//File Data
//for each files
X - File Data

File structure for *.bsp file:
=
Special thanks to watto for the fileformats (structure).

TODO: Gather info about the .bsp file (thats are the model/level/animation files?!)
