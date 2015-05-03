'Another UTAU Multi-Engine Selection Tool (umulens)
'Copyright 2013 Toufukun

'Licensed under the Apache License, Version 2.0 (the "License");
'you may not use this file except in compliance with the License.
'You may obtain a copy of the License at

'    http://www.apache.org/licenses/LICENSE-2.0

'Unless required by applicable law or agreed to in writing, software
'distributed under the License is distributed on an "AS IS" BASIS,
'WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'See the License for the specific language governing permissions and
'limitations under the License.

Imports System.IO
Imports System.Console
Imports System.Xml
Imports umulensb.utauParam

Module umulens
    Private Const version As String = "0.1.3", author As String = "Toufukun"
    Sub b64pbDebug()
        While True
            Dim buf = ReadLine()
            Dim pb As b64pb = New b64pb(buf)
            For i = 0 To pb.getLength - 1
                Write(pb.getValue(i) & " ")
            Next
            WriteLine()
        End While
    End Sub
    Sub midikeyDebug()
        Dim midikey As MIDIKey = New MIDIKey(0)
        For i = 0 To 127
            midikey.setValue(i)
            WriteLine(i & " " & midikey.getValueInLetter & " ")
        Next
        WriteLine()
        While True
            Dim buf = ReadLine()
            'midikey.setValue(buf)
            midikey = New MIDIKey(buf)
            WriteLine(midikey.getValueInInt)
        End While
    End Sub
    Private Const argcUtauWithPitch As Integer = 14, argcUtauWithoutPitch As Integer = 12
    Function getDir(ByVal fullDir As String) As String
        Dim i As Integer
        For i = fullDir.Length - 1 To 0 Step -1
            If fullDir(i) = "\" Then
                Exit For
            End If
        Next
        Return Left(fullDir, i + 1)
    End Function
    Function isSelectionOn(ByVal flag As String) As Integer
        Dim i As Integer
        For i = flag.Length - 1 To 0 Step -1
            If flag(i) = "/" Then
                Exit For
            End If
        Next
        'If flag(i) = "/" Then
        Return i
        'End If
        'Return -1
    End Function
    Sub Main()
        Dim arg() As String = Environment.GetCommandLineArgs
        WriteLine("Another UTAU Multi-Engine Selection Tool (VB.Net)")
        WriteLine("Version " & version & " by " & author)
        WriteLine()
        If arg.Length <> argcUtauWithPitch AndAlso arg.Length <> argcUtauWithoutPitch Then
            WriteLine("Bad arguments!")
            Exit Sub
        End If

        Dim inWav As String = arg(1), outWav As String = arg(2),
            orgFlag As String = arg(5), newFlag As String = "",
            orgPB As String = "AA", orgTempo As String = "!120.0",
            destKeyLetter As String = arg(3)
        If (arg.Length = argcUtauWithPitch) Then
            orgPB = arg(13)
            orgTempo = arg(12)
        End If
        Dim destKey As MIDIKey = New MIDIKey(destKeyLetter)
        Dim PB As b64pb = New b64pb(orgPB)

        Dim consSpd As Integer = Val(arg(4)), head As Integer = Val(arg(6)),
            destDur As Integer = Val(arg(7)), cons As Integer = Val(arg(8)),
            tail As Integer = Val(arg(9)), volume As Integer = Val(arg(10)),
            modulation As Integer = Val(arg(11)), tempo As Double = Val(Right(orgTempo, orgTempo.Length - 1))

        Dim engines(10) As String, ptrEngine As Integer = 0
        engines(0) = "resampler.exe"

        Dim meDir As String = getDir(arg(0))

        If Not File.Exists(meDir & "umulens.ini") Then
            WriteLine("ERROR: Config file (umulens.ini) doesn't exist!")
        Else
            Dim ifs As StreamReader = New StreamReader(meDir & "umulens.ini")
            While Not ifs.EndOfStream
                ptrEngine += 1
                engines(ptrEngine) = ifs.ReadLine()
            End While
        End If

        Dim slashPos As Integer = isSelectionOn(orgFlag)
        Dim selectionOn As Boolean = (slashPos >= 0)
        If orgFlag.Length < 2 OrElse Not selectionOn Then
            newFlag = orgFlag
            ptrEngine = 0
        Else
            ptrEngine = Val(orgFlag(slashPos + 1))
            newFlag = Left(orgFlag, slashPos) & Mid(orgFlag, slashPos + 3, orgFlag.Length - slashPos - 2)
        End If

        WriteLine("Input WAV: " & inWav)
        WriteLine("Output WAV: " & outWav)
        WriteLine("Destination Key: " & destKey.getValueInLetter & "(" & destKey.getValueInInt & ")" &
                  ", Destination Duration: " & Format(destDur / 1000.0, "#.###") & "s")
        WriteLine("Volume(Intensity): " & volume & "%, Modulation: " & modulation & "%")
        WriteLine("Head: " & head & "ms, Tail: " & tail & "ms, Length-fixed:" & cons & "ms")
        WriteLine("Tempo: " & tempo & "bpm, Cons. Speed: " & consSpd & ", Pitch Bend Length: " & PB.getLength())
        WriteLine("Flags: " & orgFlag)
        WriteLine("Pitch Bend: " & orgPB)
        For i = 0 To PB.getLength
            Write(PB.getValue(i) & " ")
        Next
        WriteLine()

        ' "%windir%\system32\cmd.exe", """" & meDir & engines(ptrEngine) & """ " &
        Dim pEngine As Process = Process.Start(meDir & engines(ptrEngine),
                                                """" & inWav & """ " & """" & outWav & """ " &
                                                destKeyLetter & " " & consSpd & " """ & newFlag & """ " &
                                                head & " " & destDur & " " & cons & " " & tail & " " &
                                                volume & " " & modulation & " " & orgTempo & " " &
                                                orgPB)
        pEngine.WaitForExit()
        pEngine.Close()
    End Sub

End Module
