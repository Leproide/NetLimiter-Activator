<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla mediante l'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.txtDllPath = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.btnStopServices = New System.Windows.Forms.Button()
        Me.btnBackup = New System.Windows.Forms.Button()
        Me.btnRestore = New System.Windows.Forms.Button()
        Me.btnPatch = New System.Windows.Forms.Button()
        Me.txtRegName = New System.Windows.Forms.TextBox()
        Me.chkChangeName = New System.Windows.Forms.CheckBox()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.btnStartServices = New System.Windows.Forms.Button()
        Me.chkMusic = New System.Windows.Forms.CheckBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtDllPath
        '
        Me.txtDllPath.Location = New System.Drawing.Point(12, 391)
        Me.txtDllPath.Name = "txtDllPath"
        Me.txtDllPath.ReadOnly = True
        Me.txtDllPath.Size = New System.Drawing.Size(185, 20)
        Me.txtDllPath.TabIndex = 0
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(12, 362)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(185, 23)
        Me.btnBrowse.TabIndex = 1
        Me.btnBrowse.Text = "Browse dll path"
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'btnStopServices
        '
        Me.btnStopServices.Location = New System.Drawing.Point(12, 79)
        Me.btnStopServices.Name = "btnStopServices"
        Me.btnStopServices.Size = New System.Drawing.Size(185, 23)
        Me.btnStopServices.TabIndex = 2
        Me.btnStopServices.Text = "Stop Service"
        Me.btnStopServices.UseVisualStyleBackColor = True
        '
        'btnBackup
        '
        Me.btnBackup.Location = New System.Drawing.Point(12, 417)
        Me.btnBackup.Name = "btnBackup"
        Me.btnBackup.Size = New System.Drawing.Size(87, 23)
        Me.btnBackup.TabIndex = 3
        Me.btnBackup.Text = "Backup dll"
        Me.btnBackup.UseVisualStyleBackColor = True
        '
        'btnRestore
        '
        Me.btnRestore.Location = New System.Drawing.Point(110, 417)
        Me.btnRestore.Name = "btnRestore"
        Me.btnRestore.Size = New System.Drawing.Size(87, 23)
        Me.btnRestore.TabIndex = 4
        Me.btnRestore.Text = "Restore dll"
        Me.btnRestore.UseVisualStyleBackColor = True
        '
        'btnPatch
        '
        Me.btnPatch.Enabled = False
        Me.btnPatch.Location = New System.Drawing.Point(12, 108)
        Me.btnPatch.Name = "btnPatch"
        Me.btnPatch.Size = New System.Drawing.Size(185, 23)
        Me.btnPatch.TabIndex = 5
        Me.btnPatch.Text = "Patch dll"
        Me.btnPatch.UseVisualStyleBackColor = True
        '
        'txtRegName
        '
        Me.txtRegName.Location = New System.Drawing.Point(12, 37)
        Me.txtRegName.Name = "txtRegName"
        Me.txtRegName.ReadOnly = True
        Me.txtRegName.Size = New System.Drawing.Size(185, 20)
        Me.txtRegName.TabIndex = 6
        Me.txtRegName.Text = "Leprechaun"
        Me.txtRegName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'chkChangeName
        '
        Me.chkChangeName.AutoSize = True
        Me.chkChangeName.Location = New System.Drawing.Point(50, 14)
        Me.chkChangeName.Name = "chkChangeName"
        Me.chkChangeName.Size = New System.Drawing.Size(114, 17)
        Me.chkChangeName.TabIndex = 7
        Me.chkChangeName.Text = "Change RegName"
        Me.chkChangeName.UseVisualStyleBackColor = True
        '
        'txtLog
        '
        Me.txtLog.Location = New System.Drawing.Point(203, 14)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ReadOnly = True
        Me.txtLog.Size = New System.Drawing.Size(241, 426)
        Me.txtLog.TabIndex = 8
        '
        'btnStartServices
        '
        Me.btnStartServices.Enabled = False
        Me.btnStartServices.Location = New System.Drawing.Point(12, 137)
        Me.btnStartServices.Name = "btnStartServices"
        Me.btnStartServices.Size = New System.Drawing.Size(185, 23)
        Me.btnStartServices.TabIndex = 9
        Me.btnStartServices.Text = "Start Service"
        Me.btnStartServices.UseVisualStyleBackColor = True
        '
        'chkMusic
        '
        Me.chkMusic.AutoSize = True
        Me.chkMusic.Checked = True
        Me.chkMusic.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkMusic.Location = New System.Drawing.Point(390, 446)
        Me.chkMusic.Name = "chkMusic"
        Me.chkMusic.Size = New System.Drawing.Size(54, 17)
        Me.chkMusic.TabIndex = 11
        Me.chkMusic.Text = "Music"
        Me.chkMusic.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox1.BackgroundImage = Global.NetLimiter_Patcher.My.Resources.Resources.LeprePirate
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PictureBox1.InitialImage = Nothing
        Me.PictureBox1.Location = New System.Drawing.Point(12, 166)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(185, 185)
        Me.PictureBox1.TabIndex = 10
        Me.PictureBox1.TabStop = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(455, 465)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.chkMusic)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnStartServices)
        Me.Controls.Add(Me.chkChangeName)
        Me.Controls.Add(Me.txtRegName)
        Me.Controls.Add(Me.btnPatch)
        Me.Controls.Add(Me.btnRestore)
        Me.Controls.Add(Me.btnBackup)
        Me.Controls.Add(Me.btnStopServices)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.txtDllPath)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "NetLimiter Patcher - Leprechaun"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtDllPath As TextBox
    Friend WithEvents btnBrowse As Button
    Friend WithEvents btnStopServices As Button
    Friend WithEvents btnBackup As Button
    Friend WithEvents btnRestore As Button
    Friend WithEvents btnPatch As Button
    Friend WithEvents txtRegName As TextBox
    Friend WithEvents chkChangeName As CheckBox
    Friend WithEvents txtLog As TextBox
    Friend WithEvents btnStartServices As Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents chkMusic As CheckBox
End Class
