Imports System
Imports System.Net
Imports System.IO
Public Class Form1
    ' ESP8266 webserver lekérdezése HTTP-n keresztül  LEDControlwithWEB_VB_sima.ino Ardunio fileshez igazítva
    ' 2018.07.08 Teszt sikeres! Miki
    Dim din As Boolean
    Dim Ain As Integer
    Dim valt_zold As Boolean = True
    Dim valt_piros As Boolean = True
    Dim led As String
    Dim sURL As String
    Dim valtas As Boolean = True
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
        Timer1.Stop()
        RadioButton2.Select()
    End Sub
    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            NotifyIcon1.Visible = True
            NotifyIcon1.Icon = SystemIcons.Application
            NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
            NotifyIcon1.BalloonTipTitle = "Kapu értesítések"
            NotifyIcon1.BalloonTipText = "Kapu állapot"
            NotifyIcon1.ShowBalloonTip(200)
            'Me.Hide()
            ShowInTaskbar = False
            Timer1.Start()
        End If
    End Sub
    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.DoubleClick
        'Me.Show()
        'Timer1.Stop()
        ShowInTaskbar = True
        Me.WindowState = FormWindowState.Normal
        '  NotifyIcon1.Visible = False

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        tcp_elker()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ListBox1.Items.Clear()
    End Sub
    Private Sub tcp_elker()
        Dim webcim As String
        webcim = sURL & led
        Label1.Text = webcim
        Dim wrGETURL As WebRequest
        Try
            wrGETURL = WebRequest.Create(webcim)
        Catch ex As Exception
            Timer1.Stop()
            CheckBox4.Checked = False
            MsgBox(webcim & "-hoz nem lehet kapcsolódni")

            Exit Sub
        End Try


        Dim myProxy As New WebProxy("myproxy", 80)
        myProxy.BypassProxyOnLocal = True

        'wrGETURL.Proxy = myProxy
        ' wrGETURL.Proxy = WebProxy.GetDefaultProxy()
        Try
            Dim objStream As Stream
            objStream = wrGETURL.GetResponse.GetResponseStream()

            Dim objReader As New StreamReader(objStream)
            Dim sLine As String = ""
            Dim i As Integer = 0

            Do While Not sLine Is Nothing
                i += 1
                sLine = objReader.ReadLine
                If Not sLine Is Nothing Then
                    ListBox1.Items.Add(i & " " & sLine)
                    Select Case i
                        Case 2
                            If sLine = "0" Then
                                Label2.Text = "Din1: Low"
                            ElseIf sLine = "1" Then
                                Label2.Text = "Din1: High"
                            End If
                        Case 3
                            If sLine = "0" Then
                                Label3.Text = "Din2: Low"
                            ElseIf sLine = "1" Then
                                Label3.Text = "Din2: High"
                            End If
                        Case 4
                            If sLine = "0" Then
                                Label4.Text = "Din3: Low"
                            ElseIf sLine = "1" Then
                                Label4.Text = "Din3: High"
                            End If
                        Case 5
                            If sLine = "0" Then
                                din = False
                                Label5.Text = "Din4: Low"
                            ElseIf sLine = "1" Then
                                Label5.Text = "Din4: High"
                                din = True
                            End If
                        Case 7
                            Ain = Val(sLine)
                            Label6.Text = "Analóg: " & Ain
                    End Select

                End If
            Loop
        Catch ex As Exception
            MsgBox("Nem lehet kapcsolódni a kiszolgálóhoz")
            Timer1.Stop()
            CheckBox4.Checked = False
        End Try

    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        tcp_elker()

        If din = Not valtas Then

            NotifyIcon1.Visible = False
            If din = True Then

                NotifyIcon1.Icon = My.Resources.zöld
                NotifyIcon1.BalloonTipText = "Nyitás"
                Label7.Text = "Kapu Nyit"
                ' NotifyIcon1.ShowBalloonTip(500)
                NotifyIcon1.Text = "Nyitás"
            Else
                NotifyIcon1.Icon = My.Resources.piros
                NotifyIcon1.BalloonTipText = "Zárás"
                Label7.Text = "Kapu Zár"
                NotifyIcon1.Text = "Zárás"
            End If
            NotifyIcon1.Visible = True
            NotifyIcon1.ShowBalloonTip(500)
        End If
        valtas = din
    End Sub


    Protected Sub PostTo(url As String, postData As String)
        'Dim cim As String = "http://192.168.0.26"
        ' PostTo(cim, "szöveg")

        Dim myWebRequest As HttpWebRequest = TryCast(WebRequest.Create(url), HttpWebRequest)
        myWebRequest.Method = "POST"
        Dim byteArray As Byte() = System.Text.Encoding.[Default].GetBytes(postData)
        myWebRequest.ContentType = "application/x-www-form-urlencoded"
        myWebRequest.ContentLength = byteArray.Length
        Dim dataStream As System.IO.Stream = myWebRequest.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()

        'Dim myWebResponse As WebResponse = myWebRequest.GetResponse()
        'myWebResponse.Write((DirectCast(myWebResponse, HttpWebResponse)).StatusDescription)
        ' dataStream = myWebResponse.GetResponseStream()
        ' Dim reader As New System.IO.StreamReader(dataStream)
        ' Dim responseFromServer As String = reader.ReadToEnd()
        ' Response.Write(responseFromServer + ":")
        'reader.Close()
        ' dataStream.Close()
        'myWebResponse.Close()

    End Sub



    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            led = "/LED1=ON"
        Else
            led = "/LED1=OFF"
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked = True Then
            led = "/LED2=ON"
        Else
            led = "/LED2=OFF"
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            led = "/ALL=ON"
        Else
            led = "/ALL=OFF"
        End If
    End Sub



    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        sURL = "http://188.36.38.165:1418"
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        sURL = "http://192.168.0.26"
    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged

        If TextBox1.Text.Trim.Length > 0 Then
            sURL = TextBox1.Text
        Else
            MsgBox("No String specified")
        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked = True Then
            Timer1.Start()
        Else
            Timer1.Stop()
        End If
    End Sub


End Class
