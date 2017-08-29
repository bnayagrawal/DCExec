Imports System.ComponentModel
Imports System.IO
Imports System.Text.RegularExpressions
Public Class Form1
    Dim exCount As Integer = 0
    Dim exTime As Date = System.DateTime.Now
    Dim bgColorCode As Integer
    Dim bgColorValue As String
    Dim notified As Boolean
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
    'Accepts arguments
    Public Sub NewArgumentsReceived(args As ObjectModel.ReadOnlyCollection(Of String))
        'to hold the first argument
        Dim aArg As String = Nothing
        'if a file path is treated as multiple arguments then it will concatenate them
        For Each item As String In args
            If item.EndsWith(".class") Then
                If IO.File.Exists(item) Then
                    aArg = item
                    Label3.Text = aArg.Trim()
                    If CheckBox5.Checked Then
                        executeFile()
                    Else
                        Me.Focus()
                    End If
                    Exit For
                Else
                    aArg += " " + item
                    If IO.File.Exists(aArg) Then
                        Label3.Text = aArg.Trim()
                        If CheckBox5.Checked Then
                            executeFile()
                        Else
                            Me.Focus()
                        End If
                        Exit For
                    End If
                End If
            Else
                aArg += " " + item
            End If
        Next
        'If args.Length > 0 Then
        'Label3.Text = args(0)
        'End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Label3.Text = "..." Or Label3.Text = "" Then
            MsgBox("Please select a file.", vbOKOnly + vbInformation, "No File Selected")
            Exit Sub
        End If
        executeFile()
        If CheckBox1.Checked Then
            Close()
        End If
    End Sub
    Public Sub executeFile()
        If IO.File.Exists(Label3.Text) Then
            Using fw As StreamWriter = New StreamWriter(My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData + "\DCExec.bat")
                Dim Fname As String
                Fname = New String(Label3.Text.Substring(Label3.Text.LastIndexOf("\") + 1))
                Fname = Fname.Substring(0, Fname.LastIndexOf("."))

                fw.WriteLine("echo off")
                fw.WriteLine(Label3.Text.Substring(0, 2))
                fw.WriteLine("cd " + Label3.Text.Substring(0, Label3.Text.LastIndexOf("\")))
                fw.WriteLine("cls")
                If RadioButton4.Checked Then
                    fw.WriteLine("color " + bgColorValue + "0")
                Else
                    fw.WriteLine("color " + bgColorValue + "f")
                End If
                If txtParameters.Text <> "" Then
                    txtParameters.Text = txtParameters.Text.Trim()
                    fw.WriteLine("java " + Fname + " " + txtParameters.Text)
                Else
                    fw.WriteLine("java " + Fname)
                End If
                If CheckBox2.Checked Then
                    fw.WriteLine("pause")
                End If
                fw.Close()
            End Using
            If CheckBox6.Checked Then
                Me.Hide()
                Me.ShowInTaskbar = False
                NotifyIcon1.BalloonTipText = "You can access me here"
                If Not notified Then
                    NotifyIcon1.ShowBalloonTip(250)
                    notified = True
                End If
            End If
            Process.Start(My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData + "\DCExec.bat")
            exCount += 1
            My.Settings.TotalExecuted += 1
            My.Settings.LastUsedFile = Label3.Text
        Else
            MsgBox("File Not found !", vbOKOnly + vbExclamation)
            ToolStripStatusLabel1.Text = "Selected File Not Found..."
            ToolStripStatusLabel1.ForeColor = Color.DarkOrange
            Exit Sub
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        OpenFileDialog1.Title = "Select a .class file"
        OpenFileDialog1.ShowDialog()

        If My.Computer.FileSystem.FileExists(OpenFileDialog1.FileName) Then
            Label3.Text = OpenFileDialog1.FileName
            ToolStripStatusLabel1.Text = "Ready to execute..."
            ToolStripStatusLabel1.ForeColor = Color.Green
            If CheckBox5.Checked Then
                executeFile()
            End If
        End If
    End Sub
    Private Sub Label3_MouseHover(sender As Object, e As EventArgs) Handles Label3.MouseHover
        ToolTip1.Show(Label3.Text, Label3)
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked Then
            RadioButton4.Checked = True
            bgColorCode = 1
            bgColorValue = "f"
        End If
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked Then
            RadioButton3.Checked = True
            bgColorCode = 0
            bgColorValue = "0"
        End If
    End Sub

    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged
        If RadioButton4.Checked Then
            If RadioButton1.Checked Then
                RadioButton2.Checked = True
            End If
        End If
    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        If RadioButton3.Checked Then
            If RadioButton2.Checked Then
                RadioButton1.Checked = True
            End If
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'to detect java path from environment variable
        'below statement splits the strings with delemeter ';'
        Dim pathArray() As String = Strings.Split(Environment.GetEnvironmentVariable("path"), ";")
        TextBox1.Clear()
        TextBox1.BackColor = Color.DarkSlateGray
        TextBox1.ForeColor = Color.Lime
        TextBox1.Text = "Detected java Path " + " @ " + Environment.NewLine
        Dim found As Boolean = False
        For Each item As String In pathArray
            If item.EndsWith("\") Then
                If IO.File.Exists(item & "java.exe") Then
                    TextBox1.AppendText(item + Environment.NewLine)
                    found = True
                End If
            Else
                If IO.File.Exists(item & "\java.exe") Then
                    TextBox1.AppendText(item + Environment.NewLine)
                    found = True
                End If
            End If
        Next

        If Not found Then
            TextBox1.TextAlign = HorizontalAlignment.Center
            TextBox1.BackColor = Color.LightGray
            TextBox1.ForeColor = Color.Red
            TextBox1.Text = "Could not detect java path!" + Environment.NewLine + "-------------------------------------" + Environment.NewLine + "Execution of file will not work unless you set proper path to java."
        End If
        'end of the code to detect java path

        'Displays app's assembly version information
        Me.Text = "DCExec v" + My.Application.Info.Version.ToString()

        'Increments appliaction executed count
        My.Settings.ApplicationExecuted += 1

        'Load last saved application settings
        CheckBox1.Checked = My.Settings.CloseProgramOnExec
        CheckBox2.Checked = My.Settings.PauseAppOnExec
        CheckBox3.Checked = My.Settings.AutoExecOnDrop
        CheckBox4.Checked = My.Settings.ExecOnStart
        CheckBox5.Checked = My.Settings.AutoExecOnNewFileOpen
        CheckBox6.Checked = My.Settings.DontShowWindow
        txtParameters.Text = My.Settings.LastUsedArgument
        RadioButton4.Checked = My.Settings.ColorPrefFg
        bgColorCode = My.Settings.ColorPrefBg
        If bgColorCode = 0 Then
            RadioButton1.Checked = True
            bgColorValue = "0"
        ElseIf bgColorCode = 1 Then
            RadioButton2.Checked = True
            bgColorValue = "f"
        ElseIf bgColorCode = 2 Then
            RadioButton5.Checked = True
            bgColorValue = "4"
        Else
            RadioButton6.Checked = True
            bgColorValue = "1"
        End If

        Label3.Text = My.Settings.LastUsedFile
        If IO.File.Exists(Label3.Text) Then
            ToolStripStatusLabel1.Text = "Ready to execute..."
            ToolStripStatusLabel1.ForeColor = Color.Green
            If My.Settings.ExecOnStart Then
                If My.Settings.DontShowWindow Then
                    Me.Hide()
                    Me.ShowInTaskbar = False
                    executeFile()
                    'NotifyIcon1.ShowBalloonTip(200)
                Else
                    Me.WindowState = FormWindowState.Minimized
                    executeFile()
                End If
            End If
        End If

        'Loads the command line argument (the file passed or used to open this app)
        Dim args() As String = Environment.GetCommandLineArgs
        If args.Length > 1 Then
            Label3.Text = args(1)
            ToolStripStatusLabel1.Text = "Ready to execute"
            ToolStripStatusLabel1.ForeColor = Color.Green

            'if space between the file path is treated as multiple args
            'it will concatenate them and make the actual path string
            If args.Length > 2 Then
                Dim tmp As String
                tmp = ""
                For i As Integer = 1 To args.Length - 1
                    tmp = tmp + " " + args(i)
                Next

                Label3.Text = tmp
                ToolStripStatusLabel1.Text = "Ready to execute"
                ToolStripStatusLabel1.ForeColor = Color.Green
            End If
        End If
    End Sub


    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        'Saves application settings
        My.Settings.CloseProgramOnExec = CheckBox1.Checked
        My.Settings.LastUsedFile = Label3.Text
        My.Settings.LastUsedArgument = txtParameters.Text
        My.Settings.PauseAppOnExec = CheckBox2.Checked
        My.Settings.ColorPrefFg = RadioButton4.Checked
        My.Settings.AutoExecOnDrop = CheckBox3.Checked
        My.Settings.AutoExecOnNewFileOpen = CheckBox5.Checked
        My.Settings.ExecOnStart = CheckBox4.Checked
        My.Settings.DontShowWindow = CheckBox6.Checked
        My.Settings.ColorPrefBg = bgColorCode
        My.Settings.LastRun = exTime
    End Sub

    Private Sub MenuItem2_Click(sender As Object, e As EventArgs) Handles MenuItem2.Click
        OpenFileDialog1.Title = "Select a .class file"
        OpenFileDialog1.ShowDialog()
        Label3.Text = OpenFileDialog1.FileName
        If My.Computer.FileSystem.FileExists(Label3.Text) = True Then
            ToolStripStatusLabel1.Text = "Ready to execute..."
            ToolStripStatusLabel1.ForeColor = Color.Green
        Else
            ToolStripStatusLabel1.Text = "Select a (.Class) file"
            ToolStripStatusLabel1.ForeColor = Color.Purple
        End If
    End Sub

    Private Sub MenuItem4_Click(sender As Object, e As EventArgs) Handles MenuItem4.Click
        Close()
    End Sub

    Private Sub MenuItem6_Click(sender As Object, e As EventArgs) Handles MenuItem6.Click
        frmAbout.ShowDialog()
    End Sub

    Private Sub txtParameters_TextChanged(sender As Object, e As EventArgs) Handles txtParameters.TextChanged
        Label6.Text = txtParameters.Text.Length
        Label8.Text = CountWords(txtParameters.Text)
    End Sub

    Public Function CountWords(ByVal value As String) As Integer
        ' Count matches.
        Dim collection As MatchCollection = Regex.Matches(value, "\S+")
        Return collection.Count
    End Function


    Private Sub Form1_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub Form1_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Dim srcFile() As String = e.Data.GetData(DataFormats.FileDrop)
        If srcFile.Length = 1 Then
            Dim finfo As IO.FileInfo
            finfo = My.Computer.FileSystem.GetFileInfo(srcFile(0))
            If finfo.Extension.ToLower <> ".class" Then
                MsgBox("Not a .class file !", vbCritical)
            Else
                Label3.Text = srcFile(0)
                ToolStripStatusLabel1.Text = "Ready to execute..."
                ToolStripStatusLabel1.ForeColor = Color.Green
                If CheckBox3.Checked Then
                    executeFile()
                End If
            End If
                ElseIf srcFile.Length > 1 Then
            MsgBox("Please drop only a single file", vbInformation)
        End If
        'srcFile() = e.Data.GetData(DataFormats.FileDrop)
    End Sub

    Private Sub ToolStripStatusLabel4_Click(sender As Object, e As EventArgs) Handles ToolStripStatusLabel4.Click
        Try
            Process.Start("http://www.sub1n4y.com")
        Catch
            Exit Sub
        End Try
    End Sub
    Private Sub Label3_TextChanged(sender As Object, e As EventArgs) Handles Label3.TextChanged
        Button1.Focus()
    End Sub
    Private Sub Form1_DoubleClick(sender As Object, e As EventArgs) Handles Me.DoubleClick
        Dim fd As New FontDialog
        fd.ShowDialog()
        Me.Font = fd.Font
    End Sub

    Private Sub MenuItem8_Click(sender As Object, e As EventArgs) Handles MenuItem8.Click
        MsgBox("Files Executed : " & exCount & " (This run)" & Chr(13) & "Files Executed : " & My.Settings.TotalExecuted & " (Total)" & Chr(13) & "Application Used : " & My.Settings.ApplicationExecuted & " Time(s) " & Chr(13) & "Last Run : " & My.Settings.LastRun.ToString & Chr(13) & "Last File : " & My.Settings.LastUsedFile, vbInformation)
    End Sub

    Private Sub RadioButton5_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton5.CheckedChanged
        If RadioButton5.Checked Then
            bgColorCode = 2
            bgColorValue = "4"
        End If
    End Sub

    Private Sub RadioButton6_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton6.CheckedChanged
        If RadioButton6.Checked Then
            bgColorCode = 3
            bgColorValue = "1"
        End If
    End Sub

    Private Sub OpenProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenProgramToolStripMenuItem.Click
        Me.Show()
        Me.ShowInTaskbar = True
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub NotifyIcon1_DoubleClick(sender As Object, e As EventArgs) Handles NotifyIcon1.DoubleClick
        Me.Show()
        Me.ShowInTaskbar = True
    End Sub

    Private Sub MenuItem10_Click(sender As Object, e As EventArgs) Handles MenuItem10.Click
        Me.Hide()
        Me.ShowInTaskbar = False
    End Sub

End Class
