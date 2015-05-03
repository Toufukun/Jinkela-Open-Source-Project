'UTAU Parameters Processing Module
'(as part of umulens)
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

Module utauParam
    Class b64pb
        Private pb() As Integer
        Private Length As Integer
        Public Const err As Integer = -65535
        Public Function getLength() As Integer
            Return Me.Length
        End Function
        Private Function b64ToInt(ByVal b64 As String) As Integer
            Dim res = 0
            For i = 0 To 1
                res *= 64
                Dim ascNow As Integer = Asc(b64(i)), ascBigA As Integer = Asc("A"),
                    ascBigZ As Integer = Asc("Z"), asca As Integer = Asc("a"),
                    ascz As Integer = Asc("z"), asc0 As Integer = Asc("0")
                If ascNow >= ascBigA And ascNow <= ascBigZ Then
                    res += ascNow - ascBigA
                ElseIf ascNow >= asca And ascNow <= ascz Then
                    res += ascNow - asca + 26
                ElseIf b64(i) = "/" Then
                    res += 63
                ElseIf b64(i) = "+" Then
                    res += 62
                Else
                    res += ascNow - asc0 + 52
                End If
            Next
            If (res And (1 << 11)) > 0 Then
                res = (res - 1) Xor ((1 << 12) - 1)
                res = -res
            End If
            Return res
        End Function
        Public Sub New(ByVal opb As String)
            Dim buf As String = ""
            Dim readingRepeat As Boolean = False
            For i = 0 To opb.Length - 1
                If opb(i) = "#" Then
                    If readingRepeat Then
                        readingRepeat = False
                        Length += Val(buf)
                    Else
                        buf = ""
                        readingRepeat = True
                    End If
                Else
                    If readingRepeat Then
                        buf += opb(i)
                    Else
                        If buf = "" Then
                            buf = opb(i)
                        Else
                            'buf += opb(i)
                            Length += 1
                            buf = ""
                        End If
                    End If
                End If
            Next
            ReDim pb(Length)
            Dim ptrPb As Integer = 0
            buf = ""
            For i = 0 To opb.Length - 1
                If opb(i) = "#" Then
                    If readingRepeat Then
                        readingRepeat = False
                        For j = 1 To Val(buf)
                            pb(ptrPb) = pb(ptrPb - 1)
                            ptrPb += 1
                        Next
                        buf = ""
                    Else
                        buf = ""
                        readingRepeat = True
                    End If
                Else
                    If readingRepeat Then
                        buf += opb(i)
                    Else
                        If buf = "" Then
                            buf = opb(i)
                        Else
                            buf += opb(i)
                            pb(ptrPb) = b64ToInt(buf)
                            ptrPb += 1
                            buf = ""
                        End If
                    End If
                End If
            Next
        End Sub
        Public Function getValue(ByVal pos As Integer) As Integer
            If pos >= Length Then
                Return err
            End If
            Return pb(pos)
        End Function
        Public Function setValue(ByVal pos As Integer, ByVal value As Integer) As Integer
            If pos >= Length OrElse value >= 1 << 12 OrElse value <= -(1 << 12) Then
                Return err
            End If
            pb(pos) = value
            Return 0
        End Function
    End Class

    Class MIDIKey
        Private Key As Integer
        Private disFromC() As Integer = {9, 11, 0, 2, 4, 5, 7}
        Private letterOfKey() As String = {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"}
        Private Octave, KeyOfOctave As Integer
        Public Const err As Integer = -1
        Private Function isLetterFlat(ByVal keyInLetter As String) As Boolean
            For i = 0 To keyInLetter.Length - 1
                If keyInLetter(i) = "#" OrElse keyInLetter(i) = "b" Then
                    Return False
                End If
            Next
            Return True
        End Function
        Public Sub New(ByVal keyInLetter As String)
            Me.setValue(keyInLetter)
        End Sub
        Public Sub New(ByVal keyInInt As Integer)
            Me.setValue(keyInInt)
        End Sub
        Public Function getValueInInt() As Integer
            Return Key
        End Function
        Public Function getValueInLetter() As String
            Dim buf As String
            buf = letterOfKey(Key Mod 12) & Octave
            Return buf
        End Function
        Public Function setValue(ByVal keyInLetter As String) As Integer
            If keyInLetter.Length > 3 OrElse keyInLetter.Length < 2 Then
                Key = err
                Return err
            End If
            Key = 0
            If isLetterFlat(keyInLetter) Then
                Octave = Val(keyInLetter(1))
                KeyOfOctave = disFromC(Asc(keyInLetter(0)) - Asc("A"))
                Key = (Octave - 5) * 12 + 72 + KeyOfOctave
            Else
                Octave = Val(keyInLetter(2))
                KeyOfOctave = disFromC(Asc(keyInLetter(0)) - Asc("A"))
                If keyInLetter(1) = "#" Then
                    KeyOfOctave += 1
                Else
                    KeyOfOctave -= 1
                End If
                Key = (Octave - 5) * 12 + 72 + KeyOfOctave
            End If
            Return 0
        End Function
        Public Function setValue(ByVal keyInInt As Integer) As String
            If keyInInt < 0 OrElse keyInInt > 127 Then
                Key = err
                Return err
            Else
                Key = keyInInt
            End If
            Octave = Math.Floor(Key / 12) - 1
            KeyOfOctave = Key - Octave * 12
            Return 0
        End Function
    End Class
End Module
