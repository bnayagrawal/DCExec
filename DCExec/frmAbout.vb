Public Class frmAbout
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub frmAbout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label2.Text = "Version : " & My.Application.Info.Version.ToString
        TextBox1.Text = "Description : " & My.Application.Info.Description +
                        Environment.NewLine + Environment.NewLine +
                        My.Application.Info.Copyright +
                        Environment.NewLine + Environment.NewLine +
                        My.Application.Info.CompanyName
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Close()
    End Sub
End Class