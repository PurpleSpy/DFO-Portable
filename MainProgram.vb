﻿Module MainProgram
    Dim olddfoloc As String = ""
    Dim dfowasinstalled As Boolean = False
    Dim leaveInstalled As Boolean = False
    Dim language As String = "EN"
    Dim Shortcuts As String = "2"
    Dim curdrive As String = My.Application.Info.DirectoryPath
    Dim allowmorethanone As Boolean = False
    Dim errorfound As Boolean = False
    Dim appdatafolder As IO.DirectoryInfo = New IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).Parent
    Sub Main()
        errorfound = CHeckforerrors()
        If errorfound Then
            Console.WriteLine("Minimums to run Dungeon Fighter online Failed")
            Exit Sub
        Else
            Console.WriteLine("Minimums to run Dungeon Fighter online Passed")
        End If

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

            If IO.File.Exists(curdrive & "\steam_appid.txt") Then
                Console.WriteLine("Portable Version cannot be Steam version of game")
                Console.WriteLine("Go to https://www.dfoneople.com/")
                Console.WriteLine("Download game through the installer then copy that folder on removable media")

                Try
                    Process.Start("https://www.dfoneople.com/")
                Catch ex As Exception

                End Try
                Exit Sub
            End If

            If My.Computer.Registry.CurrentUser.OpenSubKey("Software\Neople_DFO") IsNot Nothing Then
                    dfowasinstalled = True
                    olddfoloc = My.Computer.Registry.CurrentUser.OpenSubKey("Software\Neople_DFO", True).GetValue("Path")
                    My.Computer.Registry.CurrentUser.OpenSubKey("Software\Neople_DFO", True).SetValue("Path", curdrive)
                    Console.WriteLine("Dfo was already installed temp changing install location")
                    If My.Application.CommandLineArgs.Contains("/c") Or My.Application.CommandLineArgs.Contains("-c") Then
                        Console.Write("Dfo Install location changed")
                        Exit Sub
                    End If

                    If IO.File.Exists(curdrive & "\DFO_UNIV.cfg") Then
                        If IO.File.Exists(appdatafolder.FullName & "\locallow\dnf\DFO_UNIV.cfg") Then
                            If IO.File.Exists(appdatafolder.FullName & "\locallow\dnf\DFO_UNIVBACK.BACK") Then
                                IO.File.Delete(appdatafolder.FullName & "\locallow\dnf\DFO_UNIVBACK.BACK")
                            End If
                            My.Computer.FileSystem.RenameFile(appdatafolder.FullName & "\locallow\dnf\DFO_UNIV.cfg", "DFO_UNIVBACK.BACK")
                        End If
                        Try
                            My.Computer.FileSystem.CopyFile(curdrive & "\DFO_UNIV.cfg", appdatafolder.FullName & "\locallow\dnf\DFO_UNIV.cfg", True)
                        Catch ex As Exception

                        End Try
                    End If
                Else
                    Dim xs As Microsoft.Win32.RegistryKey = My.Computer.Registry.CurrentUser.OpenSubKey("Software", True).CreateSubKey("Neople_DFO", True)
                    xs.SetValue("Language", language)
                    xs.SetValue("NumShortcuts", Shortcuts)
                    xs.SetValue("Path", curdrive)

                    If IO.File.Exists(curdrive & "\DFO_UNIV.cfg") Then
                        If Not IO.Directory.Exists(appdatafolder.FullName & "\locallow\dnf") Then
                            IO.Directory.CreateDirectory(appdatafolder.FullName & "\locallow\dnf")
                        End If
                        Try
                            My.Computer.FileSystem.CopyFile(curdrive & "\DFO_UNIV.cfg", appdatafolder.FullName & "\locallow\dnf\DFO_UNIV.cfg", True)
                        Catch ex As Exception

                        End Try

                    End If
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
            Console.WriteLine("Go to https://www.dfoneople.com/")
            Console.WriteLine("Download game through the installer then copy that folder on removable media")

            Try
                Process.Start("https://www.dfoneople.com/")
            Catch ex As Exception

            End Try
        End If



    End Sub

    Function CHeckforerrors() As Boolean
        Dim viddetail As New System.Management.ManagementObjectSearcher("select * from Win32_VideoController")
        Dim retval As Boolean = False

        Try
            Dim cx As Management.ManagementObjectCollection = viddetail.Get()
            Dim cvid As Management.ManagementObject = cx(0)

            If cvid.Properties.Item("AdapterRAM").Value < (512 * Math.Pow(1024, 2)) Then
                Console.WriteLine(cvid.Properties.Item("Name").Value & " only has " & (cvid.Properties.Item("AdapterRAM").Value / Math.Pow(1024, 2)) & " MB available ram" & vbCrLf & "512 MB required to run game ")
                retval = True
            End If


        Catch ex As Exception

        End Try




        Try
                Dim cx As String = My.Computer.Info.OSFullName
                If cx.ToUpper.IndexOf("MICROSOFT WINDOWS") = -1 Then
                    Console.WriteLine("Found os : " & My.Computer.Info.OSFullName)
                    Console.WriteLine("Game cannot run under this os")
                    retval = True
                End If
            Catch ex As Exception

            End Try


            Try
                Dim cx As String() = My.Computer.Info.OSVersion.Split(".")

                If Integer.Parse(cx(2)) < 7601 Then
                    Console.WriteLine("Found os : " & My.Computer.Info.OSFullName)
                    Console.WriteLine("Game cannot run under this os")
                    retval = True
                End If
            Catch ex As Exception

            End Try



            If My.Computer.Info.TotalPhysicalMemory < (2 * Math.Pow(1024, 3)) Then
                Console.WriteLine("Ram : " & My.Computer.Info.TotalPhysicalMemory / Math.Pow(1024, 3) & " Gb")
                Console.WriteLine("You Need at least 2 Gb ram to run game")
                retval = True
            End If


        If My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\DirectX").GetValue("Version") IsNot Nothing Then
            Dim dvin As String = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\DirectX").GetValue("Version")
            Dim bsr As String() = dvin.Split(".")
            Dim bx As Integer = Integer.Parse(bsr(0) & bsr(1))

            If bx < 409 Then
                Console.WriteLine("You Need at least Direct X 9 to run")
                retval = True
            End If

        End If

        If My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Internet Explorer").GetValue("svcVersion") IsNot Nothing Then
            Dim dvin As String = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Internet Explorer").GetValue("svcVersion")
            Dim bsr As String() = dvin.Split(".")
            Dim bx As Integer = Integer.Parse(bsr(0))

            If bx < 10 Then
                Console.WriteLine("You Need at least Internet Explorer 10 to run")
                retval = True
            End If
        End If



        Return retval
    End Function

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
        If IO.File.Exists(appdatafolder.FullName & "\locallow\dnf\DFO_UNIV.cfg") Then

            Try
                My.Computer.FileSystem.CopyFile(appdatafolder.FullName & "\locallow\dnf\DFO_UNIV.cfg", curdrive & "\DFO_UNIV.cfg", True)

            Catch ex As Exception

            End Try
        End If
        If dfowasinstalled Then
            If olddfoloc <> "" Then

                Try
                    If IO.File.Exists(appdatafolder.FullName & "\DFO_UNIV.cfg") Then
                        IO.File.Delete(appdatafolder.FullName & "\DFO_UNIV.cfg")
                    End If

                    My.Computer.FileSystem.RenameFile(appdatafolder.FullName & "\locallow\dnf\DFO_UNIVBACK.BACK", "DFO_UNIV.cfg")

                    If IO.File.Exists(appdatafolder.FullName & "\locallow\dnf\DFO_UNIVBACK.BACK") Then
                        IO.File.Delete(appdatafolder.FullName & "\locallow\dnf\DFO_UNIVBACK.BACK")
                    End If
                Catch ex As Exception

                End Try


                My.Computer.Registry.CurrentUser.OpenSubKey("Software\Neople_DFO", True).SetValue("Path", olddfoloc)
            End If

        Else
            If Not leaveInstalled Then
                If uninstallDFOregkey() Then
                    My.Computer.FileSystem.DeleteDirectory(appdatafolder.FullName & "\locallow\dnf", FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Console.Write("DFO removed")
                Else
                    Console.Write("DFO error removing")
                End If
            End If
        End If

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

