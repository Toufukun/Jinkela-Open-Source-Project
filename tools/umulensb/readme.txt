Another UTAU Multi-Engine Selection Tool
Copyright 2013 Toufukun
v0.1.3

Umulens is free software (both in free speech and free beer) licensed under the Apache License, Version 2.0.

umulens 是在 Apache License 第二版下发布的免费自由软件。

[Notice]
Two .vb files are source codes. If it's not necessary for you, just delete it.

[注意]
两个 .vb 文件为源代码。如果您用不上，删掉就行了。

[How to Use]
1. Put umulensb.exe into the same folder as your engines', whose relationship with UTAU's directory is not important.
2. Create a file called "umulens.ini" at the folder referred in Step 1, and write one of the engines you want to use in relative path per line. Here's an example:
resampler.exe
fresamp.exe
TIPS.exe
3. Open project settings in UTAU, change the tool2 (resample) into umulens.exe.
4. Add "/n" to your notes' flag anywhere. "n" represents the number of the line where the engine to be invoked is in the setting file created in the Step 2 (i.e. the engine's number). Note: The number starts at 1, not 0, while 0 is reserved for "resampler.exe".
5. Use UTAU as usual. Have fun!

[使用方法]
1. 将 umulensb.exe 放在与您的引擎相同的目录下，这个目录与 UTAU 目录的关系是无所谓的。
2. 在第 1 步的那个目录中建立一个名为“umulens.ini”的文件，每行写一个您想使用的引擎。例如：
resampler.exe
fresamp.exe
TIPS.exe
3. 打开 UTAU 的工程设置，把 tool2 (resample) 改为 umulens.exe。
4. 在欲改变所用引擎的音符属性的 Flag 中任意位置添加形如“/n”的 Flag。其中 n 代表您想使用的引擎在于第 2 步中创建的设置文件的行数。注意，第一个引擎是 1（不是 0），0 是保留给 resampler.exe 的。
5. 像往常一样使用 UTAU 吧。

[Known Issues]
Though engines patched to run multi-thread are supported, they can only run in single-thread mode when you use this utility.

[已知问题]
虽然本工具支持打过多线程高速化补丁的引擎，但是它们只能运行在单线程模式下。
