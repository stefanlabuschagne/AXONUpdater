using System;
using System.Diagnostics;   // Writes the eventlog
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;           // XML Reader
using System.IO;            // filestream
using System.Reflection;    // This is to get the Application "Path" / "Directory"
using System.Net;           // webrequest objects
using System.Security.Cryptography.X509Certificates;   // Accepts all certificates over https:\\ 
using System.Security.Principal;
using Ionic.Zip;
using System.Threading;



namespace AutoUpdater
{

    public partial class Form1 : Form
    {
        // NOW THIS IS WHERE THE CURRENT UPDATER-APP STORES ITS SETTINGS
        // Decided to make these fixed constants

        // This On is old - This creates the admin rights issue because it writes in "HKEY_LOCAL_MACHINE"
        // public string REGISTRY_HKEY_PATH = "HKEY_LOCAL_MACHINE\\SOFTWARE\\AXON\\UPDATER";

        // 02 March 2011 -> This the way to go HKLM (Put the settings HERE!!!)
        public string REGISTRY_HKEY_PATH_UPDATER = "SOFTWARE\\AXON\\UPDATER";

        public string REGISTRY_HKEY_PATH_F2F = "SOFTWARE\\AXON\\Face2Face";

        //This one writes in HKEY_CURRENT_USER therefore the admin rights issue is no more (So i Hope!)
        // public string UPDATER_REGISTRY_HKEY_PATH = "Software\\AXON Wireless\\Axon Updater";

        // Just Test and Download (Read From The Registry or Setup.txt)
        private string PollingUrl = ""; // "http://face2face.axonwireless.co.za/autoupdate/auto.jsp";
        private string DownloadURL =""; //  "http://face2face.axonwireless.co.za/autoupdate/package/update.zip";

        private string PollingUrl2 = ""; // "http://face2face.axonwireless.co.za/autoupdate/auto.jsp";
        private string DownloadURL2 = ""; //  "http://face2face.axonwireless.co.za/autoupdate/package/update.zip";

        private double PollingIntervalSeconds = 0;

        // 22 Jun 2011
        private string UrlExtention = "user=[windows username]&machine=[windows machine]&version=1.2.2.14";

        // This is also Fine - Registry settings of the JAVA Application that needs to be whacked
        private string BaseRegistrySettings = "Software\\JavaSoft\\Prefs\\za";

        // Temporary directory for downloading stuff
        private string DefaultDownloadDirectory = "c:\\downloads\\axonf2f\\";

        private string DownloadDirectory = "";

        private string DownloadDirectoryRegistryKey = "Software\\Axon\\Face2Face";

        private string InjectorDirectory = "";
        private string Injectorfile = "injector.exe";

        // Extract the entire downloaded application file to here 
        // Confirmed with Justin 100% 22 Des 2010
        private string DefaultTargetDirectory = "c:\\Program Files\\";
        private string TargetDirectory = "";

        // private string m_sLastCommand;
        private string m_sLastReturnString;

        private bool Restarted = false;


        public Form1()
        {
            InitializeComponent();
        }

        private Boolean IsNewerVersion(string AvailableForDownloadVersion, string InstalledVersion)
        {

            bool ret = false;

            string Pv = InstalledVersion + "";
            string Dv = AvailableForDownloadVersion + "";

            string[] DaInstalledVersion = Pv.Split(new char[] { '.' }, 4);

            string[] DaDownloadVersion = Dv.Split(new char[] {'.'}, 4);

            for (int i = 0; i <= 3; i++)
            {

                if (Convert.ToDouble(DaInstalledVersion[i]) < Convert.ToDouble(DaDownloadVersion[i]))
                {
                    // Its Nerwer
                    return true;
                }
                else
                {

                    if (Convert.ToDouble(DaInstalledVersion[i]) > Convert.ToDouble(DaDownloadVersion[i]))
                    {
                        return false;
                    }


                }

                // If we got here
                // We must loop for the next Number!

                if (i == 3)
                {
                    // The numbers are the same!!!!
                    return false;
                }
                   

            }


            return ret;
        }




//        static void Main()
  //      {            
           
    //    }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Disable Manual Update Checks
            updateNowToolStripMenuItem.Enabled = false;

            // This is the timer event that triggers for an update on the loader.


            // Sort out which URL to USE
            for (int i = 1; i <= 2; i++)
            {

                switch (i)
                {
                    case 1:
                        DoThePollingAndDownload(PollingUrl, DownloadURL);
                        break;
                    case 2:
                        if (PollingUrl2.Length > 0)
                        {
                            DoThePollingAndDownload(PollingUrl2, DownloadURL2);
                        }
                        break;

                } // Switch 



            } // For

    } // Timer


        public void DoThePollingAndDownload(string LocalPollingURL, string LocalDownloadURL)
        {


            // Check  for a NEWER version than the current app
            string UpgradeInfo = PostRequest(LocalPollingURL, 60);

            notifyIcon1.Text = "Axon Updater" + Environment.NewLine + "Last Poll: " + System.DateTime.Now.ToString();

            if (UpgradeInfo.ToUpper().Contains("ERROR"))
            {

                // 03 March 2011 - Change as per Hofmeyer Retief.
                // When the internet disconnects 
                // The updater must not give the following error (When the pc is offline the updater must just be idle). 
                // THE BOTTYOM LINE IS WE MUST NOT DISPLAY AN ERROR MESSAGE

                // Display baloon with the connection error or whatever\
                // notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                // notifyIcon1.BalloonTipText = UpgradeInfo;
                // notifyIcon1.ShowBalloonTip(100);

                // We Quit
                updateNowToolStripMenuItem.Enabled = true;

            }
            else if (!ISFileReadable(UpgradeInfo.ToString()))
            {
                //WE need to know if we got a normal well-formed XML packet back!
                // So we can parse it

                // Display baloon with the connection error or whatever\
                // notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                // notifyIcon1.BalloonTipText = "Invalid XML Returned";
                // notifyIcon1.ShowBalloonTip(100);


                // We Quit
                updateNowToolStripMenuItem.Enabled = true;


            }

            else
            {
                // Business As Usual

                // Process the UpgradeInfo packet
                // With an XML Reader

                // Create an XmlReader
                using (XmlReader reader = XmlReader.Create(new StringReader(UpgradeInfo)))
                {

                    // Reading the XML FILE HAS ITS PROBLEMS!!
                    // 15 March 2011

                    String CurrentAvailableVersion = "";
                    bool FlushRegistry = false;
                    try
                    {

                        reader.ReadToFollowing("update");
                        reader.ReadToFollowing("version");

                        CurrentAvailableVersion = reader.ReadElementContentAsString();

                        FlushRegistry = false;
                        reader.ReadToFollowing("cleareg");
                        if (reader.ReadElementContentAsString() == "yes")
                        {
                            FlushRegistry = true;
                        }

                    }
                    catch (Exception ex)
                    {

                        // Just default to the min settingts so the flow can proceed and nothing happens!
                        CurrentAvailableVersion = "0.0.0.0";
                        FlushRegistry = false;

                    }

                    // notifyIcon1.BalloonTipTitle = "AXON Updater - Last Check: " + System.DateTime.Now.ToString();
                    // notifyIcon1.

                    // if (!(CurrentAvailableVersion == GetLastDownloadedVersion()))

                    if (IsNewerVersion(CurrentAvailableVersion, GetLastDownloadedVersion()))
                    {

                        // IF WE ARE HERE WE NEED ELEVATION (RUNNING AS ADMINISTRATOR!)

                        // Stop Polling and show Dialog.
                        timer1.Enabled = false;

                        if (true) //  (RunElevated(Application.ExecutablePath))
                        {
                            //Already runs elevated! so continue then with elevated code!!


                            // If the process was restarted we need to quit fast
                            if (true) // (!Restarted)
                            {


                                // Stop Triggers
                                timer1.Enabled = false;

                                // Download a new app and copy the files
                                // WriteEventlog("Downloading f2f application.");

                                DownloadDirectory = DefaultDownloadDirectory; // "c:\\downloads\\axonf2f\\"

                                // Add the current available version to indicate for AXON we are downloading THIS VERSION!!!
                                if (Downloadfile(LocalDownloadURL + "&version=" + CurrentAvailableVersion.ToString(), "update.zip", DownloadDirectory))
                                {
                                    // Download went 100%
                                    // Unzip the zipped file witrh the un-zip 3rd party utility
                                    // http://dotnetzip.codeplex.com/

                                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                                    notifyIcon1.BalloonTipText = "Extracting Files";
                                    notifyIcon1.ShowBalloonTip(100);

                                    string zipToUnpack = DownloadDirectory + "update.zip";
                                    string unpackDirectory = GetRegistryValueCurrentProgramnDirectory("InstallDir", DefaultTargetDirectory); ; //  DownloadDirectory;

                                    // Hows this - Unpack the files just as is 
                                    // to the program's directory, directly                       

                                    Boolean Upgaded = false;
                                    DialogResult DR = DialogResult.Retry;

                                    // Kill The F2F Process if he is there
                                    // New Spec by Hof Retief 
                                    // SL 4 Oct 2013
                                   //

                                    foreach (System.Diagnostics.Process myProc in System.Diagnostics.Process.GetProcesses())
                                    {

                                        // MessageBox.Show(myProc.ProcessName.ToString());

                                        if (myProc.ProcessName == "javaw")
                                        {
                                            myProc.Kill();

                                            // Pause for 5 Seconds, so the killed process can release resources
                                            // Otherwise the Pause screen pops up again
                                            Stopwatch stopwatch = Stopwatch.StartNew();
                                            while (true)
                                            {
                                                //some other processing to do STILL POSSIBLE
                                                if (stopwatch.ElapsedMilliseconds >= 5000)
                                                {
                                                    break;
                                                }
                                                Thread.Sleep(1); //so processor can rest for a while
                                            }

                                        }
                                    }

                                  
                                    // Code as usual..............                                    
                                    
                                    // SL 22 October 2013 -> If the Zip file is corrupt 
                                    // then the files will not be able to extract anyway. 
                                    // So if this message pops up, check weather the ZIP file is extractable.

                                    while ((!Upgaded) && (DR == DialogResult.Retry))
                                    {
                                        try
                                        {

                                            using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
                                            {
                                                // here, we extract every entry, but we could extract conditionally
                                                // based on entry name, size, date, checkbox status, etc.  
                                                foreach (ZipEntry Ze in zip1)
                                                {
                                                    Ze.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);

                                                }
                                            }

                                            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                                            notifyIcon1.BalloonTipText = "Extracting Complete";
                                            notifyIcon1.ShowBalloonTip(100);

                                            // We did IT.                        
                                            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                                            notifyIcon1.BalloonTipText = "Upgrade Sucsessfull";
                                            notifyIcon1.ShowBalloonTip(1000);

                                            // Okay - Here we need to reset the registry 
                                            // Only if we downloaded it Sucsessfully.

                                            // Yeah thanks only delete the registry if you do the download 
                                            // and the Reg flag is marked 

                                            // By Reading the previous setting, we 
                                            if (FlushRegistry)
                                            {

                                                // If the AXONF2F app is not installed 
                                                // then deleting nonexistant subkeys throws an error!
                                                try
                                                {
                                                    // Whack the resgistry settings of the JAVA app
                                                    // Registry settings of the JAVA Application that needs to be whacked
                                                    Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(BaseRegistrySettings);
                                                }
                                                catch
                                                {
                                                    // If the registry Subtree does not exist
                                                    // We get an Error!!!!
                                                    // But we can Ignore this!!

                                                }

                                                finally
                                                {
                                                    //Create an EMPTY Root Key Again
                                                    //  Registry settings of the JAVA Application that needs to be whacked
                                                    Microsoft.Win32.Registry.CurrentUser.CreateSubKey(BaseRegistrySettings);

                                                    // Whack the entry that indicates the LastDownloadedVersion
                                                    // To trigger a new Download next time
                                                    SetLastDownloadedVersion("");

                                                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                                                    notifyIcon1.BalloonTipText = "Application registry cleared.";
                                                    notifyIcon1.ShowBalloonTip(100);

                                                    // WriteEventlog("Reset Axon Registry settings.");
                                                }

                                            } // FLUSH-REGISTRY

                                            // Save the version that we INSTALLED
                                            // So we can know not to download this one again
                                            SetLastDownloadedVersion(CurrentAvailableVersion);


                                            // 15 March 2011 - AXON Hofmeyr
                                            InjectorDirectory = unpackDirectory + Injectorfile;
                                            RunInjector(InjectorDirectory);

                                            Upgaded = true;

                                            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                                            notifyIcon1.BalloonTipText = "Update Complete: Ver " + CurrentAvailableVersion.ToString();
                                            notifyIcon1.ShowBalloonTip(100);

                                            // Start Polling.
                                            timer1.Enabled = true;

                                            // ENABLE manual Update
                                            updateNowToolStripMenuItem.Enabled = true;

                                        }
                                        catch (Exception ex)
                                        {
                                            // Please ensure the application is not running.

                                            // Show the user a decent Exception "retry \ cancel

                                            // MessageBox.Show("Failed to Extract Zip File." + "\n" + ex.Message + "\n" + "Please ensure the F2F application is NOT running", "AXON Updater", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                                            // An update for Axon F2F is available. Can you please close the F2F application to install the update

                                            // 03 March 2011 - Hofmeyer Retief.
                                            // Change the Message That is Displayed: "An update for Axon F2F is available. Can you please close the F2F application to install the update"
                                            // DR =  MessageBox.Show("Failed to Extract Zip File." + "\n" + ex.Message + "\n" + "Please ensure the F2F application is NOT running", "AXON Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                                            string exm = ex.Message.ToString().Trim();

                                            // DR = MessageBox.Show("An update for Axon F2F is available.\nCan you please close the F2F application to install the update", "AXON Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                                            // 22 Oct 2013
                                            // SL -> Makes much more sense this way, as we FORCED / killed the F2F Application with code anyway. 

                                            DR = MessageBox.Show("Failed to Extract Zip File." + "\n" + exm, "AXON Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                                            notifyIcon1.ShowBalloonTip(100);

                                            // Attempt another Extract Action.
                                            Upgaded = false;

                                        }
                                        finally
                                        {



                                        }


                                    } // WHILE NOT UPGRADED or cancelled.

                                    if (DR == DialogResult.Cancel)
                                    {
                                        // Cancelled the Download / Extract

                                        // Download failed                        
                                        notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                                        notifyIcon1.BalloonTipText = "Upgrade Cancelled";
                                        notifyIcon1.ShowBalloonTip(1000);

                                        // Start Polling.
                                        timer1.Enabled = true;

                                    }
                                    else
                                    {
                                        // We Upgraded Okay

                                        // Download failed                        
                                        notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                                        notifyIcon1.BalloonTipText = "Upgrade Successful";
                                        notifyIcon1.ShowBalloonTip(1000);

                                        // Start Polling.
                                        timer1.Enabled = true;
                                    }
                                    // 



                                }
                                else
                                {
                                    // Download failed                        
                                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                                    notifyIcon1.BalloonTipText = "Download failed - System will retry later.";
                                    notifyIcon1.ShowBalloonTip(1000);

                                    // Start Polling.
                                    timer1.Enabled = true;

                                    // ENABLE manual Update
                                    updateNowToolStripMenuItem.Enabled = true;

                                }

                            } // Restarted!!

                        } // WE NEED TO BE AN ADMINISTRATOR!!!!!
                        else
                        {
                            // We did not get the okay to be an administrator!!!
                            // Keep on polling
                            timer1.Enabled = true;

                        }

                    } // If we must upgrade
                    else
                    {
                        // No Need to Upgrade -
                        // WE HAVE THE LATEST STUFF INSTALLED

                        // Start Polling.
                        timer1.Enabled = true;

                        // ENABLE manual Update
                        updateNowToolStripMenuItem.Enabled = true;

                    }

                }

            } // There was no error returned from the URL Connection!!!
        
        
        }

#region functions for stuff


        private bool ISFileReadable(string XMLString)
        {

            // XMLString = "";

            bool retval = false;
            string ss = "";

            try
            {

                // Create an XmlReader
                using (XmlReader reader = XmlReader.Create(new StringReader(XMLString)))
                {

                    // DO THE STUFF WITH TEH STRING WE NEED TO DO

                    reader.ReadToFollowing("update");
                    reader.ReadToFollowing("version");

                    string cav = reader.ReadElementContentAsString();

                    bool fr = false;
                    reader.ReadToFollowing("cleareg");
                    if (reader.ReadElementContentAsString() == "yes")
                    {
                        fr = true;
                    }
                
                    // If we got here it is a well-formed XML-String!
                   retval = true;

                }

            }

            catch (Exception ex)
            {
                // Throw an exception!!!
                ss = ex.Message.ToString();
                retval = false;
            }

            // Rteurn the result!!
            return retval;
        
        }


        private bool RunInjector(String InjectorApplication)
        {

            // WE need to test whether a exe file is 
            // present in the installed directory AND RUN IT!!!

            bool retval = false;

            try
            {

                if (File.Exists(InjectorApplication))
                {


                    // Starts a timer that will kill it after 5 seconds.
                    InjectorTimer.Enabled = true;

                    // Run the app
                    System.Diagnostics.Process.Start(InjectorApplication);

                    retval = true;

                }

            }
            catch(Exception ex)            
            {

                retval = false;
            
            }

            return retval;
        
        }

        private string GetLastDownloadedVersion()
        {


            // string ret = Microsoft.Win32.Registry.GetValue(REGISTRY_HKEY_PATH, "LastDownloadedVersion", "").ToString();

           Microsoft.Win32.RegistryKey SK = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(REGISTRY_HKEY_PATH_UPDATER);
           string ret = SK.GetValue("LastDownloadedVersion", "").ToString();

            // string ret = Microsoft.Win32.Registry.CurrentUser.GetValue(UPDATER_REGISTRY_HKEY_PATH+ "\\LastDownloadedVersion").ToString();
            //UPDATER_REGISTRY_HKEY_PATH
            return ret;
        }

       void SetLastDownloadedVersion(String LastDownloadedVersion)
        {


            // Microsoft.Win32.Registry.SetValue(REGISTRY_HKEY_PATH, "LastDownloadedVersion", LastDownloadedVersion.ToString());

            Microsoft.Win32.RegistryKey SK = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(REGISTRY_HKEY_PATH_UPDATER);
            SK.SetValue("LastDownloadedVersion", LastDownloadedVersion.ToString());

           // Microsoft.Win32.Registry.CurrentUser.SetValue(UPDATER_REGISTRY_HKEY_PATH + "\\LastDownloadedVersion", LastDownloadedVersion.ToString());
           // Microsoft.Win32.Registry.LocalMachine
        }


        private bool WriteEventlog(string sEvent)
        {

            // Write to the eventlog

            string sSource = "Axon Updater";
            string sLog = "Application";
            if (sEvent=="") sEvent = "Sample Event";

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);

            EventLog.WriteEntry(sSource, sEvent);
            // EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Warning, 234);

            return true;
        
        }

        // This Contacts the server and sends xml fwd and back
        private string PostRequest(string RequestURL, int iTimeout)
        {

            // POSTS a request to the server

            string result = "";
            StreamWriter myWriter = null;

            HttpWebRequest objRequest;
            objRequest = (HttpWebRequest)WebRequest.Create(RequestURL);
            objRequest.AllowWriteStreamBuffering = false;
            objRequest.Method = "POST";
            objRequest.ContentLength = 0;             
            objRequest.ContentType = "application/x-www-form-urlencoded";
            objRequest.KeepAlive = false;

            objRequest.Timeout = (1000 * iTimeout);  // 1 minute

            // This sets the packet to send to the server.

            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                // We DONT send ANY packet.
                myWriter.Write("");
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message.ToString(), "");
                return "error=" + ex.Message;
            }
            finally
            {
                if (myWriter == null)
                {
                }
                else
                {
                    myWriter.Close();
                }
            }


            // Get the response packet back

            try
            {
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                StreamReader sr;
                sr = new StreamReader(objResponse.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {

                // This will also return some errors like "404 - Not Found"
                // MessageBox.Show(ex.Message.ToString(), "");
                return ex.Message;
            }
            finally
            {
                if (!(myWriter == null))
                {
                    myWriter.Close();
                }
            }

            // m_sLastCommand = "";
            m_sLastReturnString = result;

            // MessageBox.Show(result, "");

            return result;

        }

        private Boolean DownloadApp(String downloadURL, String DownloadLocation)
        {

            // POSTS a request to the server

            string result = "";
            StreamWriter myWriter = null;

            HttpWebRequest objRequest;
            objRequest = (HttpWebRequest)WebRequest.Create(downloadURL);
            objRequest.AllowWriteStreamBuffering = false;

            // Figue this out
            /*
            if (UseProxy)
            {
                objRequest.Proxy = ProxyURL;
            }
            */

            objRequest.Method = "GET";
            objRequest.ContentLength = 0;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            objRequest.KeepAlive = false;

            int iTimeout = 1;

            objRequest.Timeout = (1000 * iTimeout);  // 1 minute

            // This sets the packet to send to the server.

            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                // We DONT send ANY packet.
                // myWriter.Write(TheRequestPacket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "");
                return false;
            }
            finally
            {
                if (myWriter == null)
                {
                }
                else
                {
                    myWriter.Close();
                }
            }


            // Get the response packet back

            try
            {
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                StreamReader sr;
                sr = new StreamReader(objResponse.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();

                // Write this to the downloaddirectory and save as a file 

            }
            catch (Exception ex)
            {

                // This will also return some errors like "404 - Not Found"
                MessageBox.Show(ex.Message.ToString(), "");
                return false;
            }
            finally
            {
                if (!(myWriter == null))
                {
                    myWriter.Close();
                }
            }

            // m_sLastCommand = "";
            m_sLastReturnString = result;

            // MessageBox.Show(result, "");

            return true;
                
        }


        private bool Downloadfile(String DaURL, String DaFilename, String DaTargetPaTH)
        {

            // DOWNLOADS A SPECIFIED FILE 
            // TO A SPECIFIED TARGET DESTINATIION
            if (!Directory.Exists(DaTargetPaTH))
            {
                Directory.CreateDirectory(DaTargetPaTH);
            }

            // See if the target directory already exists
            if (File.Exists(DaTargetPaTH + DaFilename))
            {
                File.Delete(DaTargetPaTH + DaFilename);
            }


            try
            {
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipText = "Downloading started.";
                notifyIcon1.ShowBalloonTip(100000);

                WebRequest objRequest = System.Net.HttpWebRequest.Create(DaURL);
                objRequest.Timeout = 1000 * 30;  // 30 Seconds???

                WebResponse objResponse = objRequest.GetResponse();
                byte[] buffer = new byte[32768];
                using (Stream input = objResponse.GetResponseStream())
                {
                    using (FileStream output = new FileStream(DaTargetPaTH + DaFilename, FileMode.CreateNew))
                    {
                        int bytesRead;

                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);


                            // Update the balloon that we are downloading...
                            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                            notifyIcon1.BalloonTipText = "Downloading " + bytesRead + " bytes";
                            notifyIcon1.ShowBalloonTip(100);

                        }
                    }
                }

                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipText = "Downloading Complete";
                notifyIcon1.ShowBalloonTip(100);

            }
            catch (Exception EX)
            {

                // Fact of the matter is, there is a GAZILLION reasons for a download problem.
                // In Most of the cases it is because the connection between the server and the client pc drops.

                // Show a messagebox, asking the user to "retry" or "cancel" to try later.

                // MessageBox.Show(EX.Message, "Download Exception");


                // MessageBox.Show(EX.Message, "An Error has occured during the download /n ");
                return false;

            }

            // Happiness if we got here!!
            return true;

        }

        public bool LoadSetupTXT()
        {


            // Clear registry of possible OLD values
            SetRegistryValue("PollingUrl", "");
            SetRegistryValue("PollingUrl2", "");

            SetRegistryValue("DownloadURL", "");
            SetRegistryValue("DownloadURL2", "");

            SetRegistryValue("PollingIntervalSeconds", "");

            // Load Setup.txt
            StreamReader f = new StreamReader(ApplicationDirectory() + "\\setup.txt");
            string s = f.ReadLine();
            int iCount = 0;
            while (s != null)
            {
                iCount += 1;
                if (s.StartsWith("DownloadURL="))
                {
                    SetRegistryValue("DownloadURL", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("DownloadURL2="))
                {
                    SetRegistryValue("DownloadURL2", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("PollingUrl="))
                {
                    SetRegistryValue("PollingUrl", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("PollingUrl2="))
                {
                    SetRegistryValue("PollingUrl2", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("PollingIntervalSeconds="))
                {
                    SetRegistryValue("PollingIntervalSeconds", s.Split('=')[1].Trim());
                }
               
                s = f.ReadLine();
            }
            f.Close();

            return true;

        }
        
        // This Saves the values to the Registry
        public void SetRegistryValue(string KeyName, string Value)
        {
            //Microsoft.Win32.Registry.SetValue(REGISTRY_HKEY_PATH, KeyName, Value);

            //Microsoft.Win32.RegistryKey SK = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(UPDATER_REGISTRY_HKEY_PATH);
            // SK.SetValue(KeyName, Value);

            // 02 March 20111
            Microsoft.Win32.RegistryKey SK = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(REGISTRY_HKEY_PATH_UPDATER);
            SK.SetValue(KeyName,Value);

        }


        // This Retrieves Values from the Registry LOCAL MACHINE 
        public string GetRegistryValueCurrentProgramnDirectory(string KeyName, string DefaultValue)
        {
            // Read it from "one Source"
            // string ret = Microsoft.Win32.Registry.GetValue(REGISTRY_HKEY_PATH, KeyName, DefaultValue).ToString();

            Microsoft.Win32.RegistryKey SK = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(DownloadDirectoryRegistryKey);
            string ret = SK.GetValue(KeyName, DefaultValue).ToString();

            // Add a doubl;e shlash at the end of the returned direectory!
            ret = ret + "\\";
            ret = ret.Replace("\\\\", "\\");

            if (ret == null)
            {
                return DefaultValue;
            }
            else
            {
                return ret;
            }

        }


        // This Retrieves Values from the Registry
        public string GetRegistryValue(string KeyName, string DefaultValue)
        {
            // Read it from "one Source"
            // string ret = Microsoft.Win32.Registry.GetValue(REGISTRY_HKEY_PATH, KeyName, DefaultValue).ToString();

            //Microsoft.Win32.RegistryKey SK = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(UPDATER_REGISTRY_HKEY_PATH);
            //string ret = SK.GetValue(KeyName,DefaultValue).ToString();

            // 02 March 2011
            Microsoft.Win32.RegistryKey SK = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(REGISTRY_HKEY_PATH_UPDATER);
            string ret = SK.GetValue(KeyName, DefaultValue).ToString();

            if (ret == null)
            {
                return DefaultValue;
            }
            else
            {
                return ret;
            }

        }




# endregion



        public Boolean IsFirstTimeUser()
        {
            // Check for first Time.
            // Check if the setup.txt file exists in the application Directry 

            if (System.IO.File.Exists(ApplicationDirectory() + "\\Setup.txt"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        // This returns the current Application Directory
        public String ApplicationDirectory()
        {
            Module[] Mod;
            Mod = Assembly.GetExecutingAssembly().GetModules();
            return Path.GetDirectoryName(Mod[0].FullyQualifiedName);
        }

        private bool IsGuest()
        {
            // Check for admin rights and dont elevate it again!
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return pricipal.IsInRole(WindowsBuiltInRole.Guest);
        }


        private bool RunElevated(string fileName)
        {

            // Returns TRUE if the 
            bool Retval = false;

            // Check for admin rights and dont elevate it again!
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
        
            if (!hasAdministrativeRight)
            {

                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.Verb = "runas";
                processInfo.FileName = fileName;
                try
                {
                    // START A NEW APP ON AN ELEVATED BASIS
                    // AND TERMINATE THIS ONE DEAD
                    Process.Start(processInfo);                    
                    Restarted = true;
                    Retval = true;
                }
                catch (Win32Exception ex)
                {
                    //Do nothing. Probably the user canceled the UAC window
                    // MessageBox.Show(ex.Message);
                    Retval = false;
                }

                // Set a flag to quicken the quit.
                if (Restarted)
                {
                
                    this.Close();                   
                    Application.Exit();
                    Restarted = true;
                    Retval = true;
                
                }


            }

            else
            {
                Retval = true;
            }

            return Retval;

        }


        private void Form1_Load(object sender, EventArgs e)
        {

            // This stuff that happens the first time            
            // Check whether the setup.txt exists and write values to the registry           

            if  (false)// (IsGuest())
            {

                // Prompt for elevation
                if (true) // (RunElevated(Application.ExecutablePath))
                {


                }

                // If we restarted, quit, otherwise, quit.
                this.Close();

            }

            else

             // We come in as Administrator or Std User;
            {

                this.ShowInTaskbar = false;


                RunFirstrTime(sender,e);
            
            }
                       
        }

        private void RunFirstrTime(object sender, EventArgs e)
        {


            if (IsFirstTimeUser())
            {
                // Read the Setup.txt into the Registry for the current app

                // MessageBox.Show("Here1");

                LoadSetupTXT();

                // If we are here we can delete the Setup.txt file
                // because the startup process completed 100%
                // The file is a flag to indicate a new stratup cycle

                // MessageBox.Show("Here2");


                // The "normal user" will not be able to delete this file
                // So then we will just READ the setup.txt every time 
                try
                {
                    System.IO.File.Delete(ApplicationDirectory() + "\\setup.txt");
                }
                catch (Exception ex)
                { 
                

                }
                finally
                {

                }

            }

            // Read the polling interval time form the registry 
            // and set the timer.

            // Get the stuff to be send to the server
            UrlExtention = "?user=[" + GetUsername().ToString() + "]&machine=[" + GetComputerIPAddress().ToString() + "]";
            
            // SL 22 Oct 2013
            // If there is no pollingURL in the registry, then it should be left blank
            // Otherwise, after adding the urlextention, we poll an invalid url and the thing crash

            if ((GetRegistryValue("PollingUrl", "").ToString().Trim()).Length > 0)
            {
                PollingUrl = GetRegistryValue("PollingUrl", "") + UrlExtention + "&version=" + GetLastDownloadedVersion();
            }
            else
            {
                PollingUrl = "";
            }

            if ((GetRegistryValue("PollingUrl2", "").ToString().Trim()).Length > 0)
            {
                PollingUrl2 = GetRegistryValue("PollingUrl2", "") + UrlExtention + "&version=" + GetLastDownloadedVersion();
            }
            else
            {
                PollingUrl2 = "";
            }
            
            DownloadURL = GetRegistryValue("DownloadURL", "")+UrlExtention;
            DownloadURL2 = GetRegistryValue("DownloadURL2", "")+UrlExtention;
            PollingIntervalSeconds = Convert.ToDouble(GetRegistryValue("PollingIntervalSeconds", "60"));

            //set thge timer to trigger the updates 1000miliseconds = 1 second
            timer1.Interval = (int)(1000 * PollingIntervalSeconds);
            timer1.Enabled = true; // Start the clock!

            // Set Defaults for the Notifier Balloon
            notifyIcon1.Visible = true;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipTitle = "Axon Updater";
            notifyIcon1.BalloonTipText = "Updater Started";
            notifyIcon1.ShowBalloonTip(100);

            // Trigger the first poll!
            timer1_Tick(sender, e);
        
        
        }

        // Get the Usrenbame and Computername

        public string GetUsername()
        {

            // Returns the current User;
            return WindowsIdentity.GetCurrent().Name.ToString();
           
        
        }

        public string GetComputername()
        {

            // Returns the current Machinename;
            return System.Environment.MachineName.ToString();

        }

        public string GetComputerIPAddress()
        {

            // This code comes from the internet

            string ComputerIPAddress = "";
            // Returns the current Machine IP Address;
            System.Net.IPAddress[] a =
            System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());

            for (int i = 0; i < a.Length; i++)
            {

                try 
                    {
                    // This triggers the possible exception
                    string ValidAddress = a[i].Address.ToString();

                    // If we are here we sucsesfully retrieved the address.
                    ComputerIPAddress = a[i].ToString();                

                    }
                catch {
                
                
                         }
                finally{
                
                }

            }

            return ComputerIPAddress;
        }




        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            // Clicked on the balloon
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void updateNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Trigger the timer
            timer1_Tick(sender, e);

            // Just show the user some UI that we did the update.
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipText = "Update Complete: Ver " + GetLastDownloadedVersion();
            notifyIcon1.ShowBalloonTip(100);

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void InjectorTimer_Tick(object sender, EventArgs e)
        {

            // This is the timer for the injector Process.
            // This Timer is enabled when the file is executed.
            // It deletes the file and stops the timef
            // If the file is not deleted then the timer tries again after 5 seconds

            InjectorTimer.Enabled = false;

            try 
            {

                if (File.Exists(InjectorDirectory))
                {
                    // This will NOT throw an exception if it does not exists!
                    File.Delete(InjectorDirectory);

                }

            }

            catch(Exception ex)
            {
            
                // No exception to Catch!!

            }

            finally
            {

                if (File.Exists(InjectorDirectory))
                {
                    // We did NOT manage to delete this file this time round!
                    // So we try in 5 seconds again!
                    InjectorTimer.Enabled = true;

                }
            
            }


        }


    }
}
