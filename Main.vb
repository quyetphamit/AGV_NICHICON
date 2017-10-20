Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic
Imports System.IO.Ports

Public Class Main
    Public PathApplycation As String = Application.StartupPath.ToString
    Public PathSetting As String = PathApplycation & "\Setting.ini"
    Public PathSetupLine As String = PathApplycation & "\Setting Line.ini"
    Private PathSetupModel As String = PathApplycation & "\Setting Model.ini"
    Dim ComDk As String
    Dim COMPort As String = 1                   '# COM Port
    Dim AlarmLight As Boolean = False
    Dim CountTime As Integer = 0
    Dim chochuong2 As Integer = 0
    Dim TimeInBuzzer As Integer = 0
    Dim AlarmLine(16) As Integer
    Dim dictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)
    Dim countAlarm As Integer = 0
    '                     LINE  10  11   12   13   14   15   16   17
    Dim MangUp9() As String = {"X", "Y", "Z", "W", "S", "T", "U", "V"}

    Private Sub CommPortSetup()                  '# setup COM
        With SerialPort1
            .PortName = COMPort
            .BaudRate = 9600
            .DataBits = 8
            .Parity = Parity.None
            .StopBits = StopBits.One
            .Handshake = Handshake.None
        End With
        Try
            SerialPort1.Open()
        Catch ex As Exception
            MessageBox.Show("COM RF: " & COMPort & " NOT CONNECT", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        With SerialPort2
            .PortName = ComDk
            .BaudRate = 9600
            .DataBits = 8
            .Parity = Parity.None
            .StopBits = StopBits.One
            .Handshake = Handshake.None
        End With
        Try
            SerialPort2.Open()
        Catch ex As Exception
            MessageBox.Show(ComDk & " CONTROL 3 COLOR LAMP NOT CONNECT", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'End
        End Try

    End Sub

    Private Sub Load_SettingFile()                                '# Load file Setting
        AxWindowsMediaPlayer1.URL = ReadTextFile(PathSetting, 16)
        AxWindowsMediaPlayer1.settings.volume = 100
        AxWindowsMediaPlayer1.Ctlcontrols.stop()
        If System.IO.File.Exists(PathSetting) = True Then
            COMPort = ReadTextFile(PathSetting, 2)
            ComDk = ReadTextFile(PathSetting, 4)
            TimeInBuzzer = Val(ReadTextFile(PathSetting, 6))
            chochuong2 = Val(ReadTextFile(PathSetting, 8))
            Timer1.Interval = ReadTextFile(PathSetting, 10)

            Timer3.Interval = ReadTextFile(PathSetting, 14)
        Else
            MsgBox(PathSetting & " Not found")
            End
        End If
        If System.IO.File.Exists(PathSetupLine) = True Then

            For index As Integer = 1 To CounterlineTextFile(PathSetupLine)
                GroupBox2.Controls("Button" & index).Text = ReadTextFile(PathSetupLine, index)
            Next
        Else
            MsgBox(PathSetupLine & " Not found")
            End
        End If
    End Sub

    Private Sub comcontrol_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Receivedtext_rs232(SerialPort1.ReadExisting())

    End Sub
    Delegate Sub SetTextCallback(ByVal [text] As String)
    Private Sub Receivedtext_rs232(ByVal [text] As String) '# input from ReadExisting
        If Serial_Receive.InvokeRequired Then
            Dim x As New SetTextCallback(AddressOf Receivedtext_rs232)
            Me.Invoke(x, New Object() {(text)})
        Else
            Serial_Receive.Text += [text]
        End If

        If Len(Serial_Receive.Text) > 5 Then
            Serial_Receive.Text = ""
        End If
    End Sub

    Private Sub Main_program()                                    '# Chuong trinh chinh
        Serial_Receive.Text = ""
        dictionary = MapModel(PathSetupModel)
        '# khoi tao chuong trinh quet du lieu
    End Sub

    Private Sub Main_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If SerialPort2.IsOpen = True Then
            SerialPort2.Write("F")
            SerialPort2.Write("X")
        End If

    End Sub
    Private Sub Main_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load     '# Form Load
        Load_SettingFile()      '# load data from setting file
        CommPortSetup()         '# Setup COM
        Main_program()          '# Chuong trinh chay
        Timer1.Enabled = True
        Timer3.Enabled = True
        Timer4.Enabled = True

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        'Thoigian.Text = Now
        TextBox1.Text = ""
        TextBox2.Text = ""
        For i As Integer = 1 To AlarmLine.Length - 1                                           '# tao nhap nhay cho vi tri line goi Scan tu tren xuong
            If GroupBox2.Controls("Button" & i).BackColor = Color.Red Then
                GroupBox2.Controls("Button" & i).BackColor = Color.Blue

            ElseIf GroupBox2.Controls("Button" & i).BackColor = Color.Blue Then
                GroupBox2.Controls("Button" & i).BackColor = Color.Red
            End If
        Next
        For index = 1 To AlarmLine.Length - 1
            If AlarmLine(index) <> 0 Then
                Dim txt = GroupBox2.Controls("Button" & AlarmLine(index)).Text
                Dim model = dictionary(txt)
                TextBox2.Text = TextBox2.Text & index & ": " & GroupBox2.Controls("Button" & AlarmLine(index)).Text & "-" & model & Chr(13) & Chr(10)
            Else
                If index > 1 Then
                    TextBox1.Text = GroupBox2.Controls("Button" & AlarmLine(index - 1)).Text
                Else
                    TextBox1.Clear()
                End If
                Exit For
            End If
        Next

        'For i As Integer = 1 To 10                                            '# tao nhap nhay cho vi tri AGV
        '    If GroupBox2.Controls("AGV_VT" & i).BackColor = Color.Red Then
        '        GroupBox2.Controls("AGV_VT" & i).BackColor = Color.Blue
        '    ElseIf GroupBox2.Controls("AGV_VT" & i).BackColor = Color.Blue Then
        '        GroupBox2.Controls("AGV_VT" & i).BackColor = Color.Red
        '    End If
        'Next
    End Sub


    Private Sub Timer3_Tick(sender As System.Object, e As System.EventArgs) Handles Timer3.Tick
        For index = 1 To 13
            If GroupBox2.Controls("Button" & index).BackColor = Color.Red Or GroupBox2.Controls("Button" & index).BackColor = Color.Blue Then

                AlarmLight = True
                Exit For
            Else
                If SerialPort2.IsOpen = True Then
                    SerialPort2.Write("X")
                End If
                AlarmLight = False
            End If
        Next
        If SerialPort2.IsOpen = True And AlarmLight = True Then SerialPort2.Write("V")
    End Sub

    Private Sub Timer4_Tick(sender As System.Object, e As System.EventArgs) Handles Timer4.Tick
        If AlarmLight = True Then
            CountTime = CountTime + 1
            '((TimeInBuzzer * 1000) / 500) + 1
            If CountTime < 17 Then ' cho phep coi bip trong 8 s
                If CountTime Mod 2 = 0 Then
                    If SerialPort2.IsOpen = True Then SerialPort2.Write("F")
                Else
                    If SerialPort2.IsOpen = True Then SerialPort2.Write("B")
                End If
            ElseIf CountTime = 77 Then
                CountTime = 0
                If SerialPort2.IsOpen = True Then SerialPort2.Write("F")
            End If
        Else
            CountTime = 0
            TextBox1.Text = ""
            TextBox2.Text = ""
            If SerialPort2.IsOpen = True Then
                SerialPort2.Write("F")
                SerialPort2.Write("X")
            End If

        End If
    End Sub

    Private Sub Button32_Click(sender As System.Object, e As System.EventArgs) Handles Button32.Click
        Dim MANG As String = ""
        For index = 1 To AlarmLine.Length - 1
            MANG = MANG & AlarmLine(index) & "_"
        Next
        MsgBox(MANG)
    End Sub

    Private Sub Serial_Receive_TextChanged(sender As System.Object, e As System.EventArgs) Handles Serial_Receive.TextChanged
        '# xu ly tin hieu goi tu cac line
        If Len(Serial_Receive.Text) > 1 Then
            If Serial_Receive.Text.Contains("CL") = True Then
                CountTime = 0
                For j As Integer = 1 To 16

                    GroupBox2.Controls("Button" & j).BackColor = Color.LightGray
                    Serial_Receive.Text = ""
                    AlarmLine(j) = 0
                    SerialPort2.Write("F")

                Next
            End If
            '------------------------------------------------------------------
            For i As Integer = 1 To AlarmLine.Length - 1
                If i < 10 Then
                    If Serial_Receive.Text.Contains("N" & i) = True And GroupBox2.Controls("Button" & i).BackColor = Color.LightGray Then
                        CountTime = 0
                        GroupBox2.Controls("Button" & i).BackColor = Color.Red
                        SerialPort1.WriteLine("N" & i)
                        AxWindowsMediaPlayer1.Ctlcontrols.play()
                        TextBox1.Text = i
                        For j = 1 To AlarmLine.Length - 1
                            If AlarmLine(j) = 0 Then
                                AlarmLine(j) = i
                                Exit For
                            End If
                        Next

                    ElseIf Serial_Receive.Text.Contains("F" & i) = True And GroupBox2.Controls("Button" & i).BackColor <> Color.LightGray Then
                        GroupBox2.Controls("Button" & i).BackColor = Color.LightGray
                        SerialPort1.WriteLine("F" & i)
                        For j = 1 To 13
                            If AlarmLine(j) = i Then
                                For K = j To AlarmLine.Length - 2
                                    AlarmLine(K) = AlarmLine(K + 1)
                                Next
                                AlarmLine(AlarmLine.Length - 1) = 0
                                Exit For
                            End If
                        Next
                    End If
                Else
                    If Serial_Receive.Text.Contains("L" & MangUp9(i - 10)) = True And GroupBox2.Controls("Button" & i).BackColor = Color.LightGray Then
                        GroupBox2.Controls("Button" & i).BackColor = Color.Red
                        TextBox1.Text = i
                        CountTime = 0
                        For j = 1 To AlarmLine.Length - 1
                            If AlarmLine(j) = 0 Then
                                AlarmLine(j) = i
                                Exit For
                            End If
                        Next

                    ElseIf Serial_Receive.Text.Contains("O" & MangUp9(i - 10)) = True And GroupBox2.Controls("Button" & i).BackColor <> Color.LightGray Then
                        GroupBox2.Controls("Button" & i).BackColor = Color.LightGray
                        For j = 1 To 13
                            If AlarmLine(j) = i Then
                                For K = j To AlarmLine.Length - 2
                                    AlarmLine(K) = AlarmLine(K + 1)
                                Next
                                AlarmLine(AlarmLine.Length - 1) = 0
                                Exit For
                            End If
                        Next
                    End If
                End If
            Next
            '============================================================================

            '# xu ly tin hieu vi tri AGV
            'Start_digit.Text = Len(Serial_Receive.Text) - 4
            'For i As Integer = 1 To 9
            '    If InStr(Len(Serial_Receive.Text) - 4, Serial_Receive.Text, "A" & i) > 0 Then
            '        For j As Integer = 1 To 9
            '            GroupBox2.Controls("AGV_VT" & j).BackColor = Color.White
            '            GroupBox2.Controls("AGV1_L" & j).Visible = False
            '        Next
            '        GroupBox2.Controls("AGV_VT" & i).BackColor = Color.Red
            '        GroupBox2.Controls("AGV1_L" & i).Visible = True
            '        GroupBox2.Controls("AGV1_L" & i).Text = "AGV" & 1
            '    Else
            '        GroupBox2.Controls("AGV_VT" & i).BackColor = Color.White
            '        GroupBox2.Controls("AGV1_L" & i).Visible = False
            '    End If
            'Next

            'For i As Integer = 1 To 9
            '    If InStr(Len(Serial_Receive.Text) - 4, Serial_Receive.Text, "B" & i) > 0 Then
            '        For j As Integer = 1 To 9
            '            GroupBox2.Controls("AGV_VT" & j).BackColor = Color.White
            '            GroupBox2.Controls("AGV2_L" & j).Visible = False
            '        Next
            '        GroupBox2.Controls("AGV_VT" & i).BackColor = Color.Red
            '        GroupBox2.Controls("AGV2_L" & i).Visible = True
            '        GroupBox2.Controls("AGV2_L" & i).Text = "AGV" & 2
            '    Else
            '        GroupBox2.Controls("AGV_VT" & i).BackColor = Color.White
            '        GroupBox2.Controls("AGV2_L" & i).Visible = False
            '    End If
            'Next

            'For i As Integer = 1 To 9
            '    If InStr(Len(Serial_Receive.Text) - 4, Serial_Receive.Text, "C" & i) > 0 Then
            '        For j As Integer = 1 To 9
            '            GroupBox2.Controls("AGV_VT" & j).BackColor = Color.White
            '            GroupBox2.Controls("AGV3_L" & j).Visible = False
            '        Next
            '        GroupBox2.Controls("AGV_VT" & i).BackColor = Color.Red
            '        GroupBox2.Controls("AGV3_L" & i).Visible = True
            '        GroupBox2.Controls("AGV3_L" & i).Text = "AGV" & 3
            '    Else
            '        GroupBox2.Controls("AGV_VT" & i).BackColor = Color.White
            '        GroupBox2.Controls("AGV3_L" & i).Visible = False
            '    End If
            'Next
            Serial_Receive.Text = ""
        End If
    End Sub


    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        Dim data = txtTest.Text.Trim
        If SerialPort1.IsOpen Then
            SerialPort1.Write(data)

        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Button2.BackColor <> Color.LightGray Then
            SerialPort1.WriteLine("F2")
            Serial_Receive.Text = "F2"
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Button3.BackColor <> Color.LightGray Then
            SerialPort1.WriteLine("F3")
            Serial_Receive.Text = "F3"
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Button4.BackColor <> Color.LightGray Then
            SerialPort1.WriteLine("F4")
            Serial_Receive.Text = "F4"
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If Button5.BackColor <> Color.LightGray Then
            SerialPort1.WriteLine("F5")
            Serial_Receive.Text = "F5"
        End If
    End Sub
End Class

