Imports System.IO
Imports System.ServiceProcess
Imports System.ComponentModel

Public Class Form1
    Private bw As New BackgroundWorker
    Private originalDllPath As String = ""
    Private backupPath As String = ""

    ' Dichiarazione API per riproduzione MP3
    Private Declare Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Integer, ByVal hwndCallback As Integer) As Integer

    ' Variabile per tenere traccia del file temporaneo
    Private musicTempPath As String
    Private Const musicAlias As String = "KEYGENMUSIC"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Controlla se è in esecuzione come amministratore
        If Not IsAdministrator() Then
            MessageBox.Show("Questo programma deve essere eseguito come amministratore.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Environment.Exit(1)
        End If

        ' Chiede subito la posizione della DLL
        SelectDllFile()

        ' Configura BackgroundWorker
        bw.WorkerReportsProgress = True
        bw.WorkerSupportsCancellation = False
        AddHandler bw.DoWork, AddressOf Bw_DoWork
        AddHandler bw.RunWorkerCompleted, AddressOf Bw_RunWorkerCompleted
        AddHandler bw.ProgressChanged, AddressOf Bw_ProgressChanged

        musicTempPath = System.IO.Path.GetTempPath() & Guid.NewGuid().ToString() & ".mp3"
        System.IO.File.WriteAllBytes(musicTempPath, My.Resources.BackgroundMusic) ' <-- Sostituisci "BackgroundMusic" con il nome della tua risorsa

        ' Apri il file con l'API
        mciSendString("open """ & musicTempPath & """ type mpegvideo alias " & musicAlias, Nothing, 0, 0)

        ' Se la checkbox è attiva (default), avvia la musica in loop
        If chkMusic.Checked Then
            mciSendString("play " & musicAlias & " repeat", Nothing, 0, 0)
        End If
    End Sub

    Private Sub SelectDllFile()
        Using ofd As New OpenFileDialog()
            ofd.Filter = "NetLimiter.dll|NetLimiter.dll|Tutti i file|*.*"
            ofd.Title = "Seleziona il file NetLimiter.dll originale"
            ofd.InitialDirectory = "C:\Program Files\Locktime Software\NetLimiter"
            If ofd.ShowDialog() = DialogResult.OK Then
                txtDllPath.Text = ofd.FileName
                originalDllPath = ofd.FileName
                backupPath = originalDllPath & ".backup"
            Else
                MessageBox.Show("Nessun file selezionato. Il programma verrà chiuso.", "Uscita", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Environment.Exit(0)
            End If
        End Using
    End Sub

    Private Function IsAdministrator() As Boolean
        Dim identity = Security.Principal.WindowsIdentity.GetCurrent()
        Dim principal = New Security.Principal.WindowsPrincipal(identity)
        Return principal.IsInRole(Security.Principal.WindowsBuiltInRole.Administrator)
    End Function

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Using ofd As New OpenFileDialog()
            ofd.Filter = "NetLimiter.dll|NetLimiter.dll|Tutti i file|*.*"
            ofd.Title = "Seleziona NetLimiter.dll"
            If ofd.ShowDialog() = DialogResult.OK Then
                txtDllPath.Text = ofd.FileName
                originalDllPath = ofd.FileName
                backupPath = originalDllPath & ".backup"
            End If
        End Using
    End Sub

    Private Sub btnStopServices_Click(sender As Object, e As EventArgs) Handles btnStopServices.Click
        Try
            Log("Arresto processi e servizio NetLimiter...")
            ' Kill NLClientApp.exe
            For Each proc In Process.GetProcessesByName("NLClientApp")
                proc.Kill()
                Log("Processo NLClientApp terminato.")
            Next
            ' Ferma servizio nlsvc
            Dim sc As New ServiceController("nlsvc")
            If sc.Status = ServiceControllerStatus.Running Then
                sc.Stop()
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10))
                Log("Servizio nlsvc fermato.")
                btnStopServices.Enabled = False
                btnPatch.Enabled = True
            Else
                Log("Servizio nlsvc già fermato.")
                btnStopServices.Enabled = False
                btnPatch.Enabled = True
            End If
        Catch ex As Exception
            Log("Errore durante l'arresto: " & ex.Message)
        End Try
    End Sub

    Private Sub btnStartServices_Click(sender As Object, e As EventArgs) Handles btnStartServices.Click
        Try
            Log("Avvio servizio nlsvc...")
            Dim sc As New ServiceController("nlsvc")
            If sc.Status <> ServiceControllerStatus.Running Then
                sc.Start()
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10))
                Log("Servizio nlsvc avviato.")
            Else
                Log("Servizio nlsvc già in esecuzione.")
            End If
            ' Avvia il client (opzionale)
            Dim clientPath = Path.Combine(Path.GetDirectoryName(originalDllPath), "NLClientApp.exe")
            If File.Exists(clientPath) Then
                Process.Start(clientPath)
                Log("NLClientApp avviato.")
                btnStopServices.Enabled = True
                btnStartServices.Enabled = False
                btnPatch.Enabled = False
            End If
        Catch ex As Exception
            Log("Errore durante l'avvio: " & ex.Message)
        End Try
    End Sub

    Private Sub btnBackup_Click(sender As Object, e As EventArgs) Handles btnBackup.Click
        If Not File.Exists(originalDllPath) Then
            Log("Seleziona un file DLL valido prima.")
            Return
        End If
        Try
            File.Copy(originalDllPath, backupPath, True)
            Log("Backup creato: " & backupPath)
        Catch ex As Exception
            Log("Errore backup: " & ex.Message)
        End Try
    End Sub

    Private Sub btnRestore_Click(sender As Object, e As EventArgs) Handles btnRestore.Click
        If Not File.Exists(backupPath) Then
            Log("Nessun backup trovato. Esegui prima un backup.")
            Return
        End If
        Try
            ' Assicurarsi che i servizi siano fermi
            btnStopServices_Click(Nothing, Nothing)
            File.Copy(backupPath, originalDllPath, True)
            Log("Ripristino completato da: " & backupPath)
        Catch ex As Exception
            Log("Errore ripristino: " & ex.Message)
        End Try
    End Sub

    Private Sub btnPatch_Click(sender As Object, e As EventArgs) Handles btnPatch.Click
        If Not File.Exists(originalDllPath) Then
            Log("File DLL non trovato. Selezionare un percorso valido.")
            Return
        End If

        ' Crea backup se non esiste già
        If Not File.Exists(backupPath) Then
            Try
                File.Copy(originalDllPath, backupPath)
                Log("Backup automatico creato: " & backupPath)
            Catch ex As Exception
                Log("Impossibile creare il backup: " & ex.Message)
                Return
            End Try
        End If

        ' Avvia il patching in background
        btnPatch.Enabled = False
        bw.RunWorkerAsync()
    End Sub

    Private Sub Bw_DoWork(sender As Object, e As DoWorkEventArgs)
        Dim worker = CType(sender, BackgroundWorker)
        Try
            worker.ReportProgress(0, "Avvio modifica della DLL...")
            Using patcher As New Patcher(originalDllPath)
                ' Prima modifica: IsRegistered
                patcher.SetLicenseRegistered(True, worker)

                ' Seconda modifica: AddDays
                patcher.ExtendTrial(99999, worker)

                ' Terza modifica: gestore eccezioni
                patcher.FixExceptionHandler(worker)

                ' Opzionale: nome registrazione
                If chkChangeName.Checked AndAlso Not String.IsNullOrWhiteSpace(txtRegName.Text) Then
                    patcher.SetRegistrationName(txtRegName.Text.Trim(), worker)
                Else
                    patcher.SetRegistrationName("Leprechaun".Trim(), worker)
                End If

                ' Salva
                patcher.Save(worker)
            End Using
            worker.ReportProgress(100, "Patch completata con successo!")
        Catch ex As Exception
            worker.ReportProgress(-1, "ERRORE: " & ex.ToString())
        End Try
    End Sub

    Private Sub Bw_ProgressChanged(sender As Object, e As ProgressChangedEventArgs)
        Log(e.UserState.ToString())
    End Sub

    Private Sub Bw_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        btnPatch.Enabled = True
        btnStartServices.Enabled = True
        If e.Error IsNot Nothing Then
            Log("Errore durante il patching: " & e.Error.Message)
        ElseIf e.Cancelled Then
            Log("Operazione annullata.")
        End If
    End Sub

    Private Sub Log(message As String)
        If txtLog.InvokeRequired Then
            txtLog.Invoke(New Action(Of String)(AddressOf Log), message)
        Else
            txtLog.AppendText(message & Environment.NewLine)
        End If
    End Sub

    Private Sub TxtRegName_TextChanged(sender As Object, e As EventArgs) Handles txtRegName.TextChanged

    End Sub

    Private Sub ChkChangeName_CheckedChanged(sender As Object, e As EventArgs) Handles chkChangeName.CheckedChanged
        If chkChangeName.Checked = True Then
            txtRegName.ReadOnly = False
        Else
        txtRegName.ReadOnly = True
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Process.Start("https://github.com/leproide")
    End Sub

    Private Sub ChkMusic_CheckedChanged(sender As Object, e As EventArgs) Handles chkMusic.CheckedChanged
        If chkMusic.Checked Then
            ' Attiva: riproduci in loop
            mciSendString("play " & musicAlias & " repeat", Nothing, 0, 0)
        Else
            ' Disattiva: ferma
            mciSendString("stop " & musicAlias, Nothing, 0, 0)
        End If
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ' Ferma e chiudi il file audio
        mciSendString("stop " & musicAlias, Nothing, 0, 0)
        mciSendString("close " & musicAlias, Nothing, 0, 0)

        ' Elimina il file temporaneo
        If System.IO.File.Exists(musicTempPath) Then
            System.IO.File.Delete(musicTempPath)
        End If
    End Sub
End Class