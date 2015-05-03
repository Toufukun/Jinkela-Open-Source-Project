Imports System.Console
Imports System.IO

Module ULyricsProcessor
    Public ifs As StreamReader, ofs As StreamWriter
    Sub Main()
        Dim arg() As String = Environment.GetCommandLineArgs
        WriteLine("Which do you want to add, prefix or suffix?")
        WriteLine("Input p for prefix / s for suffix:")
        Dim selection As String = ReadLine()
        WriteLine("")
        WriteLine("Input what you want to add:")
        Dim source As String = ReadLine()
        ifs = New StreamReader(arg(1))
        ofs = New StreamWriter(arg(1) & "2")
        Dim buf As String = ifs.ReadLine()
        Dim shouldSkip As Boolean = False
        While Not ifs.EndOfStream
            If buf.StartsWith("[#PREV]") Or buf.StartsWith("[#NEXT]") Then
                shouldSkip = True
            End If
            If buf.Split("=")(0) = "Lyric" Then
                If shouldSkip OrElse buf.Split("=")(1).ToUpper = "R" Then
                    shouldSkip = False
                Else
                    Select Case selection
                        Case "p", "P"
                            ofs.WriteLine("Lyric=" & source & buf.Split("=")(1))
                        Case "s", "S"
                            ofs.WriteLine("Lyric=" & buf.Split("=")(1) & source)
                        Case Else
                            'Nothing
                    End Select
                End If
            Else
                ofs.WriteLine(buf)
            End If
            buf = ifs.ReadLine()
        End While
        ifs.Close()
        ofs.Close()
        File.Delete(arg(1))
        File.Move(arg(1) & "2", arg(1))
    End Sub
    
End Module
