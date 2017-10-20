Imports System
Imports System.IO
Module Module1
    Public model As String
    Function WriteTextFile(ByVal filePath As String, ByVal textline As String)               'ghi tiep du lieu vao file
        If System.IO.File.Exists(filePath) = True Then
            Dim w As StreamWriter
            w = File.AppendText(filePath)
            w.WriteLine(textline)
            w.Flush()
            w.Close()
            End
        End If
        Return 0
    End Function
    Function ReadTextFile(ByVal filePath As String, ByVal lineNumber As Integer) As String   ' nhap duong dan file va dong can doc
        Using file As New StreamReader(filePath)
            Dim line As String
            ' doc nhung Line trong text file khong can truy nhap'
            For i As Integer = 1 To lineNumber - 1
                If file.ReadLine() Is Nothing Then
                    line = " "
                End If
            Next
            'doc Line trong text file can truy nhap
            line = file.ReadLine()
            ' Succeded!
            Return line
            file.Close()
        End Using
    End Function

    Function CounterlineTextFile(ByVal File_Path As String) As Integer    ' nhap duong dan file va dong can doc
        Dim counterLine As String = 0
        If System.IO.File.Exists(File_Path) = True Then                              ' xac nhan duong dan ton tai hay khong
            Dim objReader As New System.IO.StreamReader(File_Path)                   ' mo file theo duong dan
            While (objReader.ReadLine <> "")
                counterLine = counterLine + 1                                        ' doc theo tung dong file text
            End While
            objReader.Close()                                                        ' dong file text da mo
        Else
            MsgBox(File_Path & " Not found")
            End
        End If
        Return counterLine
    End Function

    Function FileExist(ByVal file_Path As String) As Boolean                      ' kiem tra file co ton tai hay khong?
        If System.IO.File.Exists(file_Path) = True Then
            Return True
        Else : Return False
        End If
    End Function

    Function CheckExistChar(ByVal startNo As Integer, ByVal line As String, ByVal check_Char As String) As Boolean   'Kiem tra trong chuoi ky tu [Line] co su xuat hien cua ky tu [check_char] hay khong? 
        startNo = 1
        If InStr(startNo, line, check_Char) > 0 Then   '# tra ve vi tri xuat hien 'check_char' trong chuoi'Line' tinh tu StarNo
            Return True
        Else : Return False
        End If
    End Function


    Function findNameApplication(ByVal SetNameSoftware As String) As String         '# Tim ten chuong trinh dang chay theo ten tuong doi
        Dim p As Process
        Dim temp As String
        Dim strnamesoft As String = ""
        Dim i As Boolean
        For Each p In Process.GetProcesses
            temp = p.MainWindowTitle.ToString()
            If temp.Length <> 0 Then
                i = InStr(temp, SetNameSoftware, 0)
                If i = True Then
                    strnamesoft = temp
                End If
            End If
        Next
        Return strnamesoft
    End Function
    Function MapModel(ByVal path As String) As Dictionary(Of String, String)
        Dim result As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Dim line As List(Of String) = File.ReadAllLines(path).ToList
        For Each item In line
            Dim col() As String = item.Split(",")
            result.Add(col(0), col(1))
        Next
        Return result
    End Function
End Module

