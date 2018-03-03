Module MainProgram
    Dim olddfoloc As String = ""
    Dim dfowasinstalled As Boolean = False
    Dim leaveInstalled As Boolean = False
    Dim language As String = "EN"
    Dim Shortcuts As String = "2"
    Dim curdrive As String = My.Application.Info.DirectoryPath
    Dim allowexit As Boolean = False
    Dim allowmorethanone As Boolean = False
    Sub Main()


        If My.Application.CommandLineArgs.Count > 0 Then
            If My.Application.CommandLineArgs.Contains("/?") Or My.Application.CommandLineArgs.Contains("-?") Or My.Application.CommandLineArgs.Contains("/h") Or My.Application.CommandLineArgs.Contains("-h") Then
                Console.WriteLine(My.Application.Info.AssemblyName)
                Console.WriteLine(My.Application.Info.Description)
                Console.WriteLine("Usage : " & My.Application.Info.AssemblyName & " -p ""basepath of dfo"" , sets the containing folder of the dfolauncher")
                Console.WriteLine("Usage : " & My.Application.Info.AssemblyName & " -u , uninstall regkeys")
                Console.WriteLine("Usage : " & My.Application.Info.AssemblyName & " -l , leaves regkeys installed")
                Console.WriteLine("Usage : " & My.Application.Info.AssemblyName & " -? or -h, shows this help")
                Console.WriteLine("Usage : " & My.Application.Info.AssemblyName & " -c -p ""path of dfo"", Changes Installed Path of dfo")
                Exit Sub
            End If
            If My.Application.CommandLineArgs.Contains("/u") Or My.Application.CommandLineArgs.Contains("-u") Then
                If uninstallDFOregkey() Then
                    Console.WriteLine("Dfo Uninstalled")
                Else
                    Console.WriteLine("Uninstall Failed")
                End If

                Exit Sub
            End If
            If My.Application.CommandLineArgs.Contains("/l") Or My.Application.CommandLineArgs.Contains("-l") Then
                leaveInstalled = True
            End If
            If My.Application.CommandLineArgs.Contains("-p") Or My.Application.CommandLineArgs.Contains("/p") Then
                If My.Application.CommandLineArgs.Contains("-p") Then
                    curdrive = My.Application.CommandLineArgs(My.Application.CommandLineArgs.IndexOf("-p") + 1)
                End If
                If My.Application.CommandLineArgs.Contains("/p") Then
                    curdrive = My.Application.CommandLineArgs(My.Application.CommandLineArgs.IndexOf("/p") + 1)
                End If
            End If

            If Not IO.Directory.Exists(curdrive) Then
                curdrive = My.Application.Info.DirectoryPath
            End If

        End If

        If IO.File.Exists(curdrive & "\NeopleLauncher.exe") Then
            If My.Computer.Registry.CurrentUser.OpenSubKey("Software\Neople_DFO") IsNot Nothing Then
                dfowasinstalled = True
                olddfoloc = My.Computer.Registry.CurrentUser.OpenSubKey("Software\Neople_DFO", True).GetValue("Path")
                My.Computer.Registry.CurrentUser.OpenSubKey("Software\Neople_DFO", True).SetValue("Path", curdrive)
                Console.WriteLine("Dfo was already installed temp changing install location")
                If My.Application.CommandLineArgs.Contains("/c") Or My.Application.CommandLineArgs.Contains("-c") Then
                    Console.Write("Dfo Install location changed")
                    Exit Sub
                End If
            Else
                Dim xs As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("Software", True).CreateSubKey("Neople_DFO", True)
                xs.SetValue("Language", language)
                xs.SetValue("NumShortcuts", Shortcuts)
                xs.SetValue("Path", curdrive)
                If My.Application.CommandLineArgs.Contains("/c") Or My.Application.CommandLineArgs.Contains("-c") Then
                    Console.Write("Dfo Installed")
                    Exit Sub
                Else
                    Console.Write("Dfo Installed Temporarily")
                End If

            End If

            If Not allowmorethanone Then
                cancelExistingDFO()
            End If

            Dim bxe As New Process
            bxe.StartInfo.FileName = curdrive & "\NeopleLauncher.exe"
            bxe.Start()
            Console.WriteLine("Starting DFO")
            Threading.Thread.Sleep(5000)
            EndCLeanup()

        Else
            Console.WriteLine("DFO launcher not found")
            allowexit = True
        End If

        While Not allowexit

        End While

    End Sub

    Sub cancelExistingDFO()
        If Process.GetProcessesByName("NeopleLauncher").Count > 0 Then
            Console.WriteLine("Closing previous instance of neople launcher")

            While Process.GetProcessesByName("NeopleLauncher").Count > 0
                For Each prc As Process In Process.GetProcessesByName("NeopleLauncher")
                    If Not prc.HasExited Then
                        prc.Kill()
                    End If
                Next
            End While

        End If
        If Process.GetProcessesByName("dfo").Count > 0 Then
            Console.WriteLine("Closing previous instance of dfo")
            While Process.GetProcessesByName("dfo").Count > 0
                For Each prc As Process In Process.GetProcessesByName("dfo")
                    If Not prc.HasExited Then
                        prc.Kill()
                    End If
                Next
            End While
        End If
    End Sub

    Sub EndCLeanup()
        Console.Write("Cleanup Phase")
        If dfowasinstalled Then
            If olddfoloc <> "" Then
                My.Computer.Registry.CurrentUser.OpenSubKey("Software\Neople_DFO", True).SetValue("Path", olddfoloc)
            End If

        Else
            If Not leaveInstalled Then
                If uninstallDFOregkey() Then
                    Console.Write("DFO removed")
                Else
                    Console.Write("DFO error removing")
                End If
            End If
        End If
        allowexit = True
    End Sub
    Function uninstallDFOregkey() As Boolean
        If My.Computer.Registry.CurrentUser.OpenSubKey("Software\Neople_DFO") IsNot Nothing Then
            Try
                Dim xs As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("Software", True)
                xs.DeleteSubKeyTree("Neople_DFO")

                Return True
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return False
            End Try
        Else
            Console.Write("DFO Not Currently Installed")
        End If
        Return False
    End Function

End Module

