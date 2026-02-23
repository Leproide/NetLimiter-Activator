Imports Mono.Cecil
Imports Mono.Cecil.Cil
Imports System.IO
Imports System.Threading

Public Class Patcher
    Implements IDisposable

    Private _originalPath As String
    Private _tempPath As String
    Private _assembly As AssemblyDefinition
    Private _disposed As Boolean = False

    Public Sub New(originalDllPath As String)
        _originalPath = originalDllPath

        ' Legge tutto il file originale in un array di byte
        Dim rawBytes As Byte() = File.ReadAllBytes(_originalPath)

        ' Prepara il resolver per eventuali dipendenze
        Dim resolver As New DefaultAssemblyResolver()
        resolver.AddSearchDirectory(Path.GetDirectoryName(_originalPath))

        ' Carica l'assembly da un MemoryStream (non tiene il file aperto)
        Dim memStream As New MemoryStream(rawBytes)
        Dim parameters As New ReaderParameters() With {
            .ReadWrite = True,
            .AssemblyResolver = resolver,
            .InMemory = True
        }

        _assembly = AssemblyDefinition.ReadAssembly(memStream, parameters)
    End Sub

    ' Modifica 1: this.IsRegistered = false -> true (cerca il backing field)
    Public Sub SetLicenseRegistered(registered As Boolean, worker As ComponentModel.BackgroundWorker)
        Dim licenseType = _assembly.MainModule.GetType("NetLimiter.Service.NLLicense")
        If licenseType Is Nothing Then Throw New Exception("Tipo NLLicense non trovato.")
        worker.ReportProgress(0, "Trovato tipo NLLicense.")

        Dim ctor As MethodDefinition = Nothing
        For Each m In licenseType.Methods
            If m.IsConstructor AndAlso Not m.IsStatic Then ctor = m : Exit For
        Next
        If ctor Is Nothing Then Throw New Exception("Costruttore di NLLicense non trovato.")
        worker.ReportProgress(0, "Trovato costruttore.")

        worker.ReportProgress(0, "Analisi istruzioni del costruttore...")
        Dim found As Boolean = False

        ' Prima cerca chiamate a set_IsRegistered
        For Each inst In ctor.Body.Instructions
            If (inst.OpCode = OpCodes.Call OrElse inst.OpCode = OpCodes.Callvirt) AndAlso inst.Operand IsNot Nothing Then
                Dim methodRef = TryCast(inst.Operand, MethodReference)
                If methodRef IsNot Nothing AndAlso methodRef.Name = "set_IsRegistered" Then
                    worker.ReportProgress(0, "Trovata chiamata a set_IsRegistered.")
                    ' Cerca l'istruzione precedente che carica il valore (ldc.i4.0 o ldc.i4.1)
                    Dim prev = GetPreviousInstruction(ctor.Body.Instructions, inst)
                    If prev IsNot Nothing AndAlso (prev.OpCode = OpCodes.Ldc_I4_0 OrElse prev.OpCode = OpCodes.Ldc_I4_1) Then
                        Dim oldValue = If(prev.OpCode = OpCodes.Ldc_I4_0, "false", "true")
                        worker.ReportProgress(0, $"Trovato valore {oldValue} prima della chiamata. Imposto a {If(registered, "true", "false")}.")
                        prev.OpCode = If(registered, OpCodes.Ldc_I4_1, OpCodes.Ldc_I4_0)
                        found = True
                        Exit For
                    End If
                End If
            End If
        Next

        ' Se non trovata, cerca stfld su campo contenente "IsRegistered"
        If Not found Then
            worker.ReportProgress(0, "Nessuna chiamata a set_IsRegistered trovata. Cerco stfld su campi...")
            For Each inst In ctor.Body.Instructions
                If inst.OpCode = OpCodes.Stfld AndAlso inst.Operand IsNot Nothing Then
                    Dim field = TryCast(inst.Operand, FieldDefinition)
                    If field IsNot Nothing AndAlso field.Name.IndexOf("IsRegistered", StringComparison.OrdinalIgnoreCase) >= 0 Then
                        worker.ReportProgress(0, $"Trovato stfld su campo: {field.Name}")
                        Dim prev = GetPreviousInstruction(ctor.Body.Instructions, inst)
                        If prev IsNot Nothing AndAlso (prev.OpCode = OpCodes.Ldc_I4_0 OrElse prev.OpCode = OpCodes.Ldc_I4_1) Then
                            Dim oldValue = If(prev.OpCode = OpCodes.Ldc_I4_0, "false", "true")
                            worker.ReportProgress(0, $"Trovata assegnazione a {field.Name} con valore {oldValue}. Imposto a {If(registered, "true", "false")}.")
                            prev.OpCode = If(registered, OpCodes.Ldc_I4_1, OpCodes.Ldc_I4_0)
                            found = True
                            Exit For
                        End If
                    End If
                End If
            Next
        End If

        If Not found Then
            ' Debug: elenca tutte le chiamate a metodi e stfld
            worker.ReportProgress(0, "ATTENZIONE: Nessuna assegnazione a IsRegistered trovata. Elenco operazioni nel costruttore:")
            For Each inst In ctor.Body.Instructions
                If inst.OpCode = OpCodes.Stfld Then
                    Dim field = TryCast(inst.Operand, FieldDefinition)
                    If field IsNot Nothing Then worker.ReportProgress(0, $"  stfld campo: {field.Name}")
                ElseIf inst.OpCode = OpCodes.Call OrElse inst.OpCode = OpCodes.Callvirt Then
                    Dim methodRef = TryCast(inst.Operand, MethodReference)
                    If methodRef IsNot Nothing Then worker.ReportProgress(0, $"  call metodo: {methodRef.Name}")
                End If
            Next
            Throw New Exception("Non è stato possibile trovare l'assegnazione a IsRegistered nel costruttore.")
        End If
    End Sub

    ' Modifica 2: DateTime expiration = installTime.AddDays(28.0) -> AddDays(99999.0)
    Public Sub ExtendTrial(days As Integer, worker As ComponentModel.BackgroundWorker)
        Dim serviceTempType = _assembly.MainModule.GetType("NetLimiter.Service.NLServiceTemp")
        If serviceTempType Is Nothing Then Throw New Exception("Tipo NLServiceTemp non trovato.")
        worker.ReportProgress(0, "Trovato tipo NLServiceTemp.")

        Dim initMethod As MethodDefinition = Nothing
        For Each m In serviceTempType.Methods
            If m.Name = "InitLicense" Then initMethod = m : Exit For
        Next
        If initMethod Is Nothing Then Throw New Exception("Metodo InitLicense non trovato.")
        worker.ReportProgress(0, "Trovato metodo InitLicense.")

        Dim found As Boolean = False
        For Each inst In initMethod.Body.Instructions
            If (inst.OpCode = OpCodes.Call OrElse inst.OpCode = OpCodes.Callvirt) AndAlso inst.Operand IsNot Nothing Then
                Dim methodRef = TryCast(inst.Operand, MethodReference)
                If methodRef IsNot Nothing AndAlso methodRef.Name = "AddDays" Then
                    worker.ReportProgress(0, "Trovata chiamata a AddDays.")
                    Dim prev = GetPreviousInstruction(initMethod.Body.Instructions, inst)
                    If prev IsNot Nothing AndAlso prev.OpCode = OpCodes.Ldc_R8 Then
                        Dim oldValue = Convert.ToDouble(prev.Operand)
                        worker.ReportProgress(0, $"Trovato valore AddDays: {oldValue}. Imposto a {days}.")
                        prev.Operand = Convert.ToDouble(days)
                        found = True
                        Exit For
                    End If
                End If
            End If
        Next

        If Not found Then
            worker.ReportProgress(0, "ATTENZIONE: Nessuna chiamata a AddDays trovata. Elenco COMPLETO delle istruzioni di InitLicense:")
            Dim idx As Integer = 0
            For Each inst In initMethod.Body.Instructions
                Dim oper As String = If(inst.Operand IsNot Nothing, inst.Operand.ToString(), "")
                worker.ReportProgress(0, $"  [{idx}] {inst.OpCode} {oper}")
                idx += 1
            Next
            Throw New Exception("Non è stato possibile trovare l'istruzione AddDays nel metodo InitLicense.")
        End If
    End Sub

    ' Modifica 3: catch (Exception exception) -> catch (System.Exception exception)
    Public Sub FixExceptionHandler(worker As ComponentModel.BackgroundWorker)
        Dim serviceTempType = _assembly.MainModule.GetType("NetLimiter.Service.NLServiceTemp")
        If serviceTempType Is Nothing Then Return

        Dim initMethod As MethodDefinition = Nothing
        For Each m In serviceTempType.Methods
            If m.Name = "InitLicense" Then
                initMethod = m
                Exit For
            End If
        Next
        If initMethod Is Nothing Then Return

        Dim found As Boolean = False
        For Each handler In initMethod.Body.ExceptionHandlers
            If handler.HandlerType = ExceptionHandlerType.Catch Then
                Dim catchType = handler.CatchType
                If catchType IsNot Nothing AndAlso catchType.Name = "Exception" Then
                    worker.ReportProgress(0, "Trovato gestore catch per Exception, sostituisco con System.Exception.")
                    Dim systemException = _assembly.MainModule.ImportReference(GetType(System.Exception))
                    handler.CatchType = systemException
                    found = True
                End If
            End If
        Next
        If Not found Then
            worker.ReportProgress(0, "Nessun gestore catch per Exception trovato (potrebbe non essere necessario).")
        End If
    End Sub

    ' Modifica opzionale: get_RegName restituisce un nome fisso
    Public Sub SetRegistrationName(name As String, worker As ComponentModel.BackgroundWorker)
        Dim licenseType = _assembly.MainModule.GetType("NetLimiter.Service.NLLicense")
        If licenseType Is Nothing Then Return

        Dim getRegName As MethodDefinition = Nothing
        For Each m In licenseType.Methods
            If m.Name = "get_RegName" Then
                getRegName = m
                Exit For
            End If
        Next
        If getRegName Is Nothing Then
            worker.ReportProgress(0, "Metodo get_RegName non trovato.")
            Return
        End If

        worker.ReportProgress(0, $"Modifico get_RegName per restituire '{name}'.")
        Dim il = getRegName.Body.GetILProcessor()
        il.Body.Instructions.Clear()
        il.Append(il.Create(OpCodes.Ldstr, name))
        il.Append(il.Create(OpCodes.Ret))
    End Sub

    ' Funzione per ottenere l'istruzione precedente
    Private Function GetPreviousInstruction(instructions As Mono.Collections.Generic.Collection(Of Instruction), current As Instruction) As Instruction
        For i As Integer = 1 To instructions.Count - 1
            If instructions(i) Is current Then
                Return instructions(i - 1)
            End If
        Next
        Return Nothing
    End Function

    Public Sub Save(worker As ComponentModel.BackgroundWorker)
        If _disposed Then Throw New ObjectDisposedException("Patcher")

        worker.ReportProgress(0, "Scrittura assembly modificato in memoria...")
        ' Scrive l'assembly in un MemoryStream
        Dim outStream As New MemoryStream()
        _assembly.Write(outStream)
        _assembly.Dispose()
        _assembly = Nothing

        ' Crea un file temporaneo e scrive i byte
        _tempPath = Path.GetTempFileName()
        File.WriteAllBytes(_tempPath, outStream.ToArray())
        outStream.Close()
        outStream.Dispose()

        worker.ReportProgress(0, $"File temporaneo creato: {_tempPath}")

        ' Ora sovrascrivi l'originale con il temporaneo
        Dim retryCount As Integer = 3
        While retryCount > 0
            Try
                File.Copy(_tempPath, _originalPath, True)
                worker.ReportProgress(0, "File originale sovrascritto con successo.")
                Exit While
            Catch ex As IOException
                retryCount -= 1
                If retryCount = 0 Then
                    Throw New Exception("Impossibile sovrascrivere il file originale dopo diversi tentativi. Assicurati che NetLimiter sia fermato.", ex)
                End If
                worker.ReportProgress(0, $"Tentativo fallito, riprovo tra 500ms... ({retryCount} tentativi rimasti)")
                Thread.Sleep(500)
            End Try
        End While

        ' Elimina il temporaneo
        Try
            File.Delete(_tempPath)
            worker.ReportProgress(0, "File temporaneo eliminato.")
        Catch
        End Try
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposed Then
            If disposing Then
                If _assembly IsNot Nothing Then
                    _assembly.Dispose()
                    _assembly = Nothing
                End If
                If _tempPath IsNot Nothing AndAlso File.Exists(_tempPath) Then
                    Try
                        File.Delete(_tempPath)
                    Catch
                    End Try
                End If
            End If
            _disposed = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
    End Sub
End Class