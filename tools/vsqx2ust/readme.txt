# VSQx2UST
Copyright 2012-2015 Toufukun under Apache License, Version 2.0

## 简体中文

### 注意
**因没有必要添加对更多参数的支持，本程序可能不再更新。**

### 快速入门
使用本工具，您需要 .Net Framework 2.0。
把 VSQx 拖到本工具的图标上就行了。
拖到 vsqx2ust.exe 上会使用默认设置。
拖到 vsqx2ust-noquantize.bat 上会禁用量化（当乐谱含有三连音的时候使用）。
拖到 vsqx2ust-noquantize-noparameter.bat 上会禁用量化和参数转换（此时会使用 UTAU 的 MODE2 滑音，并使用本程序提供的一个预置）。

### 支持的参数

 - PIT (滑音)
 - PBS (滑音范围)
 - VEL (力度)

### 更新日志

- 0.3.3 beta
新增若干个开关，详细请看程序的命令行说明
新增自动避开重名文件，防止覆盖的功能
readme改为markdown格式

- 0.3.0 beta
支持Vocaloid 4文件 (VSQ4)
改进了命令行界面

- 0.2.1 beta
音符会被强制量化到三十二分音符
Vocaloid中的VEL改为转换为辅音速度。（之前为音量）

- 0.2.0 beta
完全重写代码
支持PIT和PBS
取消了对极短“R”（UTAU 中的休止符）的保护

### 语言
Visual Basic .Net
源代码在另一个压缩包里。

-----

## English

### Note
**This program may be no longer updated, for there's not much necessity.**

### How to use?
You need .Net Framework 2.0 to run this utility.
Just drag the VSQx to the utility's icon.
If you drop on vsqx2ust.exe, the default setting will be used.
If you drop on vsqx2ust-noquantize.bat, quantize will NOT be used. Use this when there're triplets(tuplets) in your sequence.
If you drop on vsqx2ust-noquantize-noparameter.bat, quantize and control parameter conversion will be NOT be used.

### Supported Parameters
PIT (Pitch Bend)
PBS (Pitch Bend Sensitivity)
VEL (Velocity)

### What's new?

- 0.3.3 beta
Add several switches, please see the program's commandline help for more information
Add auto renaming to avoid overwriting
Readme is written in markdown

- 0.3.0 beta
Vocaloid 4 file (VSQ4) support
Commandline interface improvement

- 0.2.1 beta
Notes are forced to be quantized into 1/32 notes.
VEL in Vocaloid is converted into Consonant Velocity now. (It was converted into Intensity[Volume] before.)

- 0.2.0 beta
Fully rewrite the code
Support PIT and PBS now
Remove protection for very short "R"s (rest note in UST)

### Language
Visual Basic .Net
To get the code, you need to download separated archive.
