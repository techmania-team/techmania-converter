# TECHMANIA Converter
A tool to convert .bms and .pt (work in progress) to .tech (TECHMANIA's format).

Code in [TechmaniaConverter/TechmaniaConverter/DJMax](TechmaniaConverter/TechmaniaConverter/DJMax) are reused from [DJMax-Editor](https://github.com/hsreina/DJMax-Editor) to load and parse .pt files. DJMax-Editor is originally released under [Apache License 2.0](TechmaniaConverter/TechmaniaConverter/DJMax/LICENSE). Among these files,
* [PtLoader.cs](TechmaniaConverter/TechmaniaConverter/DJMax/PtLoader.cs) contains 1 method from [PTOpenFile.cs](https://github.com/hsreina/DJMax-Editor/blob/master/DJMaxEditor/Files/pt/PTOpenFile.cs). I modified the method to remove logging code and change output type.
* I removed an unused line in [TracksList.cs](TechmaniaConverter/TechmaniaConverter/DJMax/TracksList.cs) to improve performance.
* Other files are not modified.
