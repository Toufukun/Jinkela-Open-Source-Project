'VSQx2UST
'Copyright 2012-2013 Toufukun

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

Module vsqx2ust
    Public ofs As StreamWriter
    Public progName As String = "VSQx2UST"
    Public progVersion As String = "0.3.3 beta"
    Public progAuthor As String = "Toufukun"
    Structure Parameter
        Public Position As Integer
        Public Value As Integer
    End Structure
    Public arrPit() As Parameter, arrPBS() As Parameter
    Dim noPit As Integer = 1, noPBS As Integer = 1
    Dim pPit As Integer = 0, pPBS As Integer = 0
    Dim posAccuracy As Integer = 15
    Dim DoPause = True, ConvParam = True, VelToVol = False
    Dim activeNS As String = Nothing

    Sub PrintInfo()
        WriteLine(progName & " " & progVersion & " by " & progAuthor)
    End Sub
    Sub PrintUsage()
        WriteLine("USAGE: Just drop your VSQx on the program's icon")
        WriteLine("       or use commandline: vsqx2ust [options] <inputVSQx>")
        WriteLine()
        WriteLine("OPTIONS:")
        WriteLine("  -q n   Force quantize the notes to n ticks")
        WriteLine("         Notes will be quantized to 15 ticks (32nd note) by default.")
        WriteLine("         This is to avoid very short ""R""s in the UST,")
        WriteLine("         especially when the VSQ is imported from MIDI. ")
        WriteLine("         It will bring error when there are tuplets, use -q 1 or -p to prevent.")
        WriteLine("  -p     Prevent quantization. Same as -q 1.")
        WriteLine("  --v3   Force read as VSQ3.")
        WriteLine("  --v4   Force read as VSQ4.")
        WriteLine("  -c     Don't convert control parameters (VEL, PIT,PBS)")
        WriteLine("         and use MODE2 by default.")
        WriteLine("  -v     Convert VEL into Volume(Intensity).")
        WriteLine("  -x     Don't pause when the conversion finishes.")
        WriteLine("         Use this when you're doing a batch work.")
    End Sub
    Sub WriteHeader(ByVal tempo As Integer, ByVal name As String)
        ofs.WriteLine("[#SETTING]")
        ofs.WriteLine("Tempo=" & Format(tempo, "0.00"))
        ofs.WriteLine("Tracks=1")
        ofs.WriteLine("ProjectName=" & name)
        If ConvParam Then
            ofs.WriteLine("Mode2=False")
        Else
            ofs.WriteLine("Mode2=True")
        End If
    End Sub
    Sub WriteNote(ByRef num As Integer, ByVal dur As Integer, ByVal key As Integer, ByVal lyric As String, ByVal vel As Integer, ByVal pos As Integer)
        Static LastLyirc = "R"
        ofs.WriteLine("[#" & Format(num, "0000") & "]")
        ofs.WriteLine("Length=" & dur)
        ofs.WriteLine("Lyric=" & lyric)
        ofs.WriteLine("NoteNum=" & key)
        If Not ConvParam Then
            ofs.WriteLine("Velocity=" & 100)
            ofs.WriteLine("Intensity=" & 100)
        Else
            If VelToVol Then
                ofs.WriteLine("Velocity=" & 100)
                ofs.WriteLine("Intensity=" & vel)
            Else
                ofs.WriteLine("Velocity=" & vel)
                ofs.WriteLine("Intensity=" & 100)
            End If
        End If
        ofs.WriteLine("Moduration=0")
        If lyric <> "R" Then
            If ConvParam Then
                ofs.Write("PitchBend=")
                For i = 1 To Int(dur / 480 * 100)
                    'Tsukkomi here: And and Or won't let VB do a short-circuit evaluation
                    While pPit + 1 < noPit AndAlso arrPit(pPit + 1).Position <= pos + i / 100 * 480
                        pPit += 1
                    End While
                    While pPBS + 1 < noPBS AndAlso arrPBS(pPBS + 1).Position <= pos + i / 100 * 480
                        pPBS += 1
                    End While
                    ofs.Write(Int(arrPit(pPit).Value / 8192 * arrPBS(pPBS).Value * 100) & ",")
                    'WriteLine("pPit = " & pPit & " position = " & arrPit(pPit).Position &
                    '          " pPBS = " & pPBS & " position = " & arrPit(pPBS).Position &
                    '          " pos = " & pos) 'Debug
                Next
                ofs.WriteLine("0")
            Else
                If LastLyirc = "R" Then
                    ofs.WriteLine("PBW=100,50")
                    ofs.WriteLine("PBS=-50")
                Else
                    ofs.WriteLine("PBW=200,50")
                    ofs.WriteLine("PBS=-100;-50")
                End If
            End If
        End If
        num += 1
        LastLyirc = lyric
    End Sub
    Sub Main()
        'Process arguments from terminal
        Dim arg() As String = Environment.GetCommandLineArgs
        If arg.Length = 1 Then
            PrintInfo()
            WriteLine()
            PrintUsage()
            Exit Sub
        End If
        For i As Integer = 1 To arg.Length - 2
            Select Case arg(i)
                Case "-q"
                    If Val(arg(i + 1)) > 0 Then
                        posAccuracy = Val(arg(i + 1))
                    ElseIf Val(arg(i + 1)) = 0 Then
                        posAccuracy = 1
                    Else
                        Throw New Exception
                    End If
                    i += 1
                Case "-p"
                    posAccuracy = 1
                Case "--v3"
                    activeNS = "v3"
                Case "--v4"
                    activeNS = "v4"
                Case "-c"
                    ConvParam = False
                Case "-v"
                    VelToVol = True
                Case "-x"
                    DoPause = False
                Case Else
                    PrintInfo()
                    WriteLine()
                    PrintUsage()
                    Exit Sub
            End Select
        Next
        Dim vsqxDir As String = arg(arg.Length - 1)
        Dim ustDir As String ' directory of the dest. ust without ".ust" extension
        Dim ustPre As String = Left(vsqxDir, Len(vsqxDir) - 5)
        ' Open the vsqx file
        Dim vsqx As XmlDocument = New XmlDocument
        Try
            vsqx.Load(vsqxDir)
        Catch
            WriteLine("Invalid XML document.")
            GoTo BeforeExit
        End Try
        Dim ns As XmlNamespaceManager = New XmlNamespaceManager(vsqx.NameTable)
        ns.AddNamespace("v3", "http://www.yamaha.co.jp/vocaloid/schema/vsq3/")
        ns.AddNamespace("v4", "http://www.yamaha.co.jp/vocaloid/schema/vsq4/")
        ' Detect the file format (vsq3 or vsq4)
        Dim root As XmlNode = Nothing
        If activeNS Is Nothing Then
            Try
                root = vsqx.SelectSingleNode("v3:vsq3", ns)
            Catch
            End Try
            If root Is Nothing Then
                Try
                    root = vsqx.SelectSingleNode("v4:vsq4", ns)
                Catch
                    WriteLine("Unsupported file format.")
                    GoTo BeforeExit
                End Try
                activeNS = "v4"
                WriteLine("This is a VSQ4 Document, trying to read as VSQ4.")
            End If
        Else
            root = vsqx.SelectSingleNode(activeNS & ":vsq" & activeNS(activeNS.Length - 1))
        End If
        Try
            'Set tempo
            Dim tempoNode As XmlNode
            If activeNS = "v4" Then
                tempoNode = root.SelectSingleNode(activeNS + ":masterTrack/" + activeNS + ":tempo/" + activeNS + ":v", ns)
            Else
                tempoNode = root.SelectSingleNode(activeNS + ":masterTrack/" + activeNS + ":tempo/" + activeNS + ":bpm", ns)
            End If
            Dim tempo As Double = Val(tempoNode.InnerText) / 100
            Dim vsTracks As XmlNodeList = root.SelectNodes(activeNS + ":vsTrack", ns)
            Dim vsTrack As XmlNode
            For Each vsTrack In vsTracks
                Dim vsTrackNoNode As XmlNode
                If activeNS = "v4" Then
                    vsTrackNoNode = vsTrack.SelectSingleNode(activeNS + ":tNo", ns)
                Else
                    vsTrackNoNode = vsTrack.SelectSingleNode(activeNS + ":vsTrackNo", ns)
                End If
                Dim vsTrackNo As Integer = Val(vsTrackNoNode.InnerText)
                Dim trackName As String
                If activeNS = "v4" Then
                    trackName = vsTrack.SelectSingleNode(activeNS + ":name", ns).InnerText
                Else
                    trackName = vsTrack.SelectSingleNode(activeNS + ":trackName", ns).InnerText
                End If
                Dim musicalParts As XmlNodeList
                If activeNS = "v4" Then
                    musicalParts = vsTrack.SelectNodes(activeNS + ":vsPart", ns)
                Else
                    musicalParts = vsTrack.SelectNodes(activeNS + ":musicalPart", ns)
                End If
                Dim musicalPart As XmlNode
                Dim partNo As Integer = 0
                For Each musicalPart In musicalParts
                    partNo += 1
                    Dim partName As String
                    If activeNS = "v4" Then
                        partName = musicalPart.SelectSingleNode(activeNS + ":name", ns).InnerText
                    Else
                        partName = musicalPart.SelectSingleNode(activeNS + ":partName", ns).InnerText
                    End If
                    ustDir = ustPre & "_t" & Format(vsTrackNo, "00") & "_p" & Format(partNo, "00") & ".ust"
                    Dim fileCounter = 1
                    While File.Exists(ustDir)
                        ustDir = ustPre & "_t" & Format(vsTrackNo, "00") & "_p" & Format(partNo, "00") & "_" & Format(fileCounter, "000") & ".ust"
                        fileCounter += 1
                    End While
                    WriteLine("* Track " & vsTrackNo & " (" & trackName & ") , Part " & partNo & " (" & partName & ")" & _
                        vbCrLf & "  --> " & ustDir)
                    'Codepage 932 - Shift_JIS
                    ofs = New StreamWriter(ustDir, False, System.Text.Encoding.GetEncoding(932))
                    WriteHeader(tempo, "VSQx2UST Converted Sequence")
                    'Preprocess vsqx' parameters
                    Dim mCtrls As XmlNodeList
                    If activeNS = "v4" Then
                        mCtrls = musicalPart.SelectNodes(activeNS + ":cc", ns)
                    Else
                        mCtrls = musicalPart.SelectNodes(activeNS + ":mCtrl", ns)
                    End If
                    Dim mCtrl As XmlNode
                    ReDim arrPit(mCtrls.Count)
                    ReDim arrPBS(mCtrls.Count)
                    arrPit(0).Position = 0
                    arrPit(0).Value = 0
                    arrPBS(0).Position = 0
                    arrPBS(0).Value = 2
                    noPit = 1
                    noPBS = 1
                    pPit = 0
                    pPBS = 0
                    Dim posTick, attr As XmlNode
                    For Each mCtrl In mCtrls
                        If activeNS = "v4" Then
                            attr = mCtrl.SelectSingleNode(activeNS + ":v", ns)
                            posTick = mCtrl.SelectSingleNode(activeNS + ":t", ns)
                        Else
                            attr = mCtrl.SelectSingleNode(activeNS + ":attr", ns)
                            posTick = mCtrl.SelectSingleNode(activeNS + ":posTick", ns)
                        End If
                        Select Case attr.Attributes.ItemOf("id").InnerText
                            Case "PIT", "P"
                                arrPit(noPit).Position = posTick.InnerText
                                arrPit(noPit).Value = attr.InnerText
                                noPit += 1
                            Case "PBS", "S"
                                arrPBS(noPBS).Position = posTick.InnerText
                                arrPBS(noPBS).Value = attr.InnerText
                                noPBS += 1
                            Case Else
                                'Nothing here :P
                        End Select
                    Next
                    Dim notes As XmlNodeList = musicalPart.SelectNodes(activeNS + ":note", ns)
                    Dim note As XmlNode
                    Dim noteNo As Integer = 0
                    Dim lastNoteEndPos As Integer = 0
                    Dim isFirstNote As Boolean = True
                    Dim noteDur, notePos, noteVel, noteKey As Integer, noteLyric As String
                    For Each note In notes
                        'Force be quantized, to 1/32 note (15 ticks) by default
                        'To prevent some very short "R"s in dest. UST
                        'especially when the VSQx is converted from MIDI.

                        If activeNS = "v4" Then
                            notePos = Val(note.SelectSingleNode(activeNS + ":t", ns).InnerText)
                        Else
                            notePos = Val(note.SelectSingleNode(activeNS + ":posTick", ns).InnerText)
                        End If
                        notePos = Math.Round(notePos / posAccuracy) * posAccuracy
                        'If lastNoteEndPos <> notePos And Not isFirstNote Then
                        If lastNoteEndPos <> notePos Then
                            WriteNote(noteNo, notePos - lastNoteEndPos, 60, "R", 100, notePos)
                        End If
                        If isFirstNote Then
                            isFirstNote = False
                        End If
                        If activeNS = "v4" Then
                            noteDur = Val(note.SelectSingleNode(activeNS + ":dur", ns).InnerText)
                        Else
                            noteDur = Val(note.SelectSingleNode(activeNS + ":durTick", ns).InnerText)
                        End If
                        noteDur = Math.Round(noteDur / posAccuracy) * posAccuracy
                        If activeNS = "v4" Then
                            noteVel = Val(note.SelectSingleNode(activeNS + ":v", ns).InnerText) / 64 * 100
                        Else
                            noteVel = Val(note.SelectSingleNode(activeNS + ":velocity", ns).InnerText) / 64 * 100
                        End If
                        If activeNS = "v4" Then
                            noteLyric = note.SelectSingleNode(activeNS + ":y", ns).InnerText
                        Else
                            noteLyric = note.SelectSingleNode(activeNS + ":lyric", ns).InnerText
                        End If
                        If activeNS = "v4" Then
                            noteKey = Val(note.SelectSingleNode(activeNS + ":n", ns).InnerText)
                        Else
                            noteKey = Val(note.SelectSingleNode(activeNS + ":noteNum", ns).InnerText)
                        End If
                        WriteNote(noteNo, noteDur, noteKey, noteLyric, noteVel, notePos)
                        lastNoteEndPos = notePos + noteDur
                    Next
                    ofs.WriteLine("[#TRACKEND]")
                    ofs.Close()
                    WriteLine("  " & noteNo & " notes converted.")
                Next
            Next
            WriteLine()
            Write("All Tracks and Parts has been converted.")
            If DoPause Then
                WriteLine(" Please press enter to exit.")
            Else
                WriteLine()
            End If
        Catch e As Exception
            WriteLine("Some error occured.")
            WriteLine("Maybe beacuse the file format is invalid.")
            WriteLine()
            WriteLine("Stack trace: (You can send this to the developer to help debug.)")
            WriteLine(e.StackTrace)
        End Try
BeforeExit:
        If DoPause Then
            ReadLine()
        End If
    End Sub
End Module

