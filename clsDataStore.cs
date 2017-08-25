using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;            // filestream
using System.Reflection;    // This is to get the Application "Path" / "Directory"
using System.Net;           // webrequest objects
using System.Windows.Forms; // messageboxes!!
using System.Security.Cryptography.X509Certificates;   // Accepts all certificates over https:\\ 

namespace SmartDeviceProject4
{
        public enum ImageTypes 
{
        PrincipalIdentity = 1,
        PrincipalPhoto = 2,
        PrincipalResidance = 3,
        PrincipalCohabitation = 4
/*
 * This according to Sergei 
    
    public final static int IMAGE_PRINCIPAL_ID = 1;
    public final static int IMAGE_PRINCIPAL_PROOF = 2;
    public final static int IMAGE_PRINCIPAL_PHOTO = 3;
    public final static int IMAGE_GUARDIAN_ID = 4;
    public final static int IMAGE_GUARDIAN_PROOF = 5;
    public final static int IMAGE_GUARDIAN_PHOTO = 6;
    public final static int IMAGE_COHABITATION = 7;
*/

}
    /*  This is a fix to accept all types of certificates */
    /*  http://weblogs.asp.net/jan/archive/2003/12/04/41154.aspx */
    public class TrustAllCertificatePolicy : System.Net.ICertificatePolicy
    {
        public TrustAllCertificatePolicy()
        { }

        public bool CheckValidationResult(ServicePoint sp,
         X509Certificate cert, WebRequest req, int problem)
        {
            return true;
        }
    }


    class datastore
    {
        // was the application send by SMS?
        private string _SMS;

        private string _agentID;
        private string _agentMSISDN;
        private string _agentDI;  // Device ID
        private string _agentPASSWORD;

/*      private string _appVersion;
        private string _applanguage;
*/

        private string _PrincipalName;
        private string _PrincipalSurname;
        private string _PrincipalNationality;
        private string _PrincipalID;
        private string _Principalidtype;
        private string _Principalbirthdate;
        private string _Principalmsisdn;
        private string _Principalemail;
        private string _Principalbank;
        private string _PrincipalAccountPurpose;
        private string _Principalnextofkin;
        private string _PrincipalnextofkinNumber;
        private string _PrincipalAddress1;
        private string _PrincipalAddress2;
        private string _PrincipalAddress3;

        private bool _newCustomer;    // "1" or "0"
        private bool _simswap;        // "1" or "0"
        private bool _simreg;         // "1" or "0"
        private bool _mmoney;         // "1" or "0"

        private string _sim;
        private string _puk;

        private string _idImageData;
        private string _idImageType;

        private string _appImageData;
        private string _appImageType;

        private string _custImageData;
        private string _custImageType;

        private string _gaurdID;
        private string _gaurdIDtype;

        /* Constants for the different lists GROUPLISTS */
        public enum GROUP_LIST
        {
            INCSOURCE = 0,
            IDTYPE = 1,
            NATIONALITY = 2,
            ACCPURPOSE = 3,
            BANK = 4,
            GENDER = 5,
            RELIGION = 6,
            PRODUCT = 7,
            PROFESSIONAL_STATUS = 8,
            PHONE_EXPENDITURE = 9,
            CHOICE_AGREE = 10,
            STATE = 11,
            PROFESSION = 12,
            RELATIONSHIP = 13,
            NUMBER_PREFIX = 14
        }
        
        private string m_sLastCommand;
        private string m_sLastReturnString;
        private string Groupversion;

        // Decided to make these fixed constants
        public string REGISTRY_HKEY_PATH = "HKEY_LOCAL_MACHINE\\SOFTWARE\\AXON\\RICS";

        private string AXONServerURL = "";
        private string UpgradeURL = "";
        private string ProxyURL = "";
        private string UseProxy = "";


        // This is the interface to set to make the xml packages

        public string SMS
        {
            get
            {
                return _SMS;
            }
            set
            {
                _SMS = value;
            }
        }

        public string PrincipalName
        {
            get
            {
                return _PrincipalName;
            }
            set
            {
                _PrincipalName = value;
            }
        }

        public string PrincipalSurName
        {
            get
            {
                return _PrincipalSurname;
            }
            set
            {
                _PrincipalSurname = value;
            }
        }


        public string PrincipalNationality
        {
            get
            {
                return _PrincipalNationality;
            }
            set
            {
                _PrincipalNationality = value;
            }
        }

        public string PrincipalID
        {
            get
            {
                return _PrincipalID;
            }
            set
            {
                _PrincipalID = value;
            }
        }

        public string PrincipalIDtype
        {
            get
            {
                return _Principalidtype;
            }
            set
            {
                _Principalidtype = value;
            }
        }

        public string PrincipalBirthDate
        {
            get
            {
                return _Principalbirthdate;
            }
            set
            {
                _Principalbirthdate = value;
            }
        }

        public string PrincipalMSISDN
        {
            get
            {
                return _Principalmsisdn;
            }
            set
            {
                _Principalmsisdn = value;
            }
        }

        public string PrincipalEmail
        {
            get
            {
                return _Principalemail;
            }
            set
            {
                _Principalemail = value;
            }
        }

        public string PrincipalBank
        {
            get
            {
                return _Principalbank;
            }
            set
            {
                _Principalbank = value;
            }
        }

        public string PrincipalAccountPurpose
        {
            get
            {
                return _PrincipalAccountPurpose;
            }
            set
            {
                _PrincipalAccountPurpose = value;
            }
        }

        public string PrincipalNextOfKin
        {
            get
            {
                return _Principalnextofkin;
            }
            set
            {
                _Principalnextofkin = value;
            }
        }

        public string PrincipalNextOfKinNumber
        {
            get
            {
                return _PrincipalnextofkinNumber;
            }
            set
            {
                _PrincipalnextofkinNumber = value;
            }
        }

        public string PrincipalAddress1
        {
            get
            {
                return _PrincipalAddress1;
            }
            set
            {
                _PrincipalAddress1 = value;
            }
        }

        public string PrincipalAddress2
        {
            get
            {
                return _PrincipalAddress2;
            }
            set
            {
                _PrincipalAddress2 = value;
            }
        }

        public string PrincipalAddress3
        {
            get
            {
                return _PrincipalAddress3;
            }
            set
            {
                _PrincipalAddress3 = value;
            }
        }

        public bool newCustomer // "1" or "0"
        {
            get
            {
                return _newCustomer;
            }
            set
            {
                _newCustomer = value;
            }
        }

        public bool SimReg // "1" or "0"
        {
            get
            {
                return _simreg;
            }
            set
            {
                _simreg = value;
            }
        }

        public bool SimSwap// "1" or "0"
        {
            get
            {
                return _simswap;
            }
            set
            {
                _simswap = value;
            }
        }

        public bool MMoney  // "1" or "0"
        {
            get
            {
                return _mmoney;
            }
            set
            {
                _mmoney = value;
            }
        }

        public string Sim
        {
            get
            {
                return _sim;
            }
            set
            {
                _sim  = value;
            }
        }

        public string Puk
        {
            get
            {
                return _puk;
            }
            set
            {
                _puk = value;
            }
        }

        // IMAGES!

        // Copy of the ID Doc
        public string IDImageData
        {
            get
            {
                return _idImageData;
            }
            set
            {
                _idImageData = value;
            }
        }

        public string IDImageType
        {
            get
            {
                return _idImageType;
            }
            set
            {
                _idImageType = value;
            }
        }

        // Photo of the Application Form
        public string AppImageData
        {
            get
            {
                return _appImageData;
            }
            set
            {
                _appImageData = value;
            }
        }

        public string AppImageType
        {
            get
            {
                return _appImageType;
            }
            set
            {
                _appImageType = value;
            }
        }


        // Photo of the Customer
        public string CustImageType
        {
            get
            {
                return _custImageType;
            }
            set
            {
                _custImageType = value;
            }
        }

        public string CustImageData
        {
            get
            {
                return _custImageData;
            }
            set
            {
                _custImageData = value;
            }
        }

        // Gaurdian Stuff

        public string GaurdID
        {
            get
            {
                return _gaurdID;
            }
            set
            {
                _gaurdID = value;
            }
        }

        public string GaurdIDType
        {
            get
            {
                return _gaurdIDtype;
            }
            set
            {
                _gaurdIDtype = value;
            }
        }

        public string DeviceIdentifier
        {
            get
            {
                return _agentDI;
            }
            set
            {
                _agentDI = value;
            }                    
        }

        public string AgentID
        {
            get
            {
                return _agentID;
            }
            set
            {
                _agentID = value;
            }
        }

        private string AgentPassword
        {
            get
            {
                return _agentPASSWORD;
            }
            set
            {
                _agentPASSWORD = value;
            }
        }


        public string AgentMSISDN
        {
            get
            {
                return _agentMSISDN;
            }
            set
            {
                _agentMSISDN = value;
            }
        }


        #region 

        private void LoadServerSettings()
        {
            // Loads the registry settings into the class variables
            // This is to replace the hardcoded values
            // The values are read into the registry with the setup.txt file
            AXONServerURL = GetRegistryValue("server_url","");
            // UpgradeURL = GetRegistryValue("upgrade_url", "");
            ProxyURL = GetRegistryValue("proxy_url","");
            UseProxy = GetRegistryValue("useproxy","");
            AgentMSISDN = GetRegistryValue("agentmsisdn", "");

        }

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

        public Boolean RegistryHasValues()
        {
            // Check if registry values exist 
            // This is used to detect a failed UPGRADE

            if (GetRegistryValue("server_url","").ToString()=="")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public bool IsSerevrAvailable()
        {
            // Just Post Someting to the serevr
            string reponse = PostRequest( "", 10);
            if (reponse.Contains("<mdas><error>Document has no root element!</error></mdas>"))
            {
                return true;

            }
            else
            {
                MessageBox.Show("Server not available" + reponse);
                return false;
            }
            
        }

        private void ClearRegisty()
        {
           // This is called on loading new "setup.txt "
           // and just before an UPGRADE takes place

            SetRegistryValue("agentid", "");
            SetRegistryValue("deviceidentifier", "");
            SetRegistryValue("agentpassword", "");
            SetRegistryValue("grpver", "");
            SetRegistryValue("server_url", "");
            SetRegistryValue("proxy_url", "");
            SetRegistryValue("useproxy", "");

            //Wipe the grouplists (ID Type not used anymore)
            SetRegistryValue("f2f_grp_idtype_SIM", "");
            SetRegistryValue("f2f_grp_idtype_SIM_1", "");
            SetRegistryValue("f2f_grp_idtype_SIM_2", "");

            SetRegistryValue("f2f_grp_idtype_MM", "");
            SetRegistryValue("f2f_grp_idtype_MM_1", "");
            SetRegistryValue("f2f_grp_idtype_MM_2", "");

            SetRegistryValue("f2f_grp_nationality", "");
            SetRegistryValue("f2f_grp_nationality_1", "");
            SetRegistryValue("f2f_grp_nationality_2", "");

            SetRegistryValue("f2f_grp_accountpurpose", "");
            SetRegistryValue("f2f_grp_accountpurpose_1", "");
            SetRegistryValue("f2f_grp_accountpurpose_2", "");

            SetRegistryValue("f2f_grp_bank", "");
            SetRegistryValue("f2f_grp_bank_1", "");
            SetRegistryValue("f2f_grp_bank_2", "");

        }


        // This cleans-up the registry
        // This load the setup.txt file into the registry
        // THIS saves the msisdn into the regisrtry.
        public bool LoadSetupTXT()
        {       

            // Clear registry
            ClearRegisty();

            // Load Setup.txt

            datastore ds = new datastore();

            StreamReader f = new StreamReader(ApplicationDirectory() + "\\setup.txt");
            string s  = f.ReadLine();
            int iCount = 0;
            while ( s != null)
            {
                iCount += 1;
                if (s.StartsWith("AMSISDN")) 
                {
                    // Read ther MSISDN in on first read.
                    ds.SetRegistryValue("agentmsisdn", s.Split('=')[1].Trim());

                    // string a  = s.Split('=')[1].Trim();

                }
                if (s.StartsWith("PinWaitTimeout"))
                {
                    ds.SetRegistryValue("PinWaitTimeout", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("PollingInterval"))
                {
                    ds.SetRegistryValue("PollingInterval", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("ServerURL"))
                {
                    ds.SetRegistryValue("server_url", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("UpgradeURL"))
                {
                    ds.SetRegistryValue("upgrade_url", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("ProxyURL"))
                {
                    ds.SetRegistryValue("proxy_url", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("useproxy"))
                {
                    ds.SetRegistryValue("useproxy", s.Split('=')[1].Trim());
                }

                if (s.StartsWith("ApplicationTitle") )
                {
                    ds.SetRegistryValue("ApplicationTitle", s.Split('=')[1].Trim());
                }               

                s = f.ReadLine();
            }
            f.Close();

            return true;
                
        }

        // This is callexd fior sacreen 2 where the AGENT has to enter and confirm his password (PIN)
        // And we authenticate him and get his data from the server.
        public bool AuthenticateAgentFirstTime(string agentpassword)
        {

            string AgentMSISDN = GetRegistryValue("agentmsisdn", "");
        
            // Do the first startup Stuff
            return GET_AGENT_DATA(agentpassword, AgentMSISDN, AXONServerURL);
                 
        }


        // This is run FIRSTTIME to connect to the server and register the agent //
        // And download registry settings                                        //
        private bool GET_AGENT_DATA(string AgentPassword, string AgentMSISDN, string AXONServerURL)
        {
            // This passes the MSISDN and new PASSWORD enterd by the user.
            // And returns the informtion abouty the AGENT

            // Command must (always?) be "set"

            string XMLPACKET = getXmlPassword("set", "", AgentPassword, AgentMSISDN);

            PostRequest(XMLPACKET, 60);

            // THIS HANDLES POSSIBLE ERRORS AND UPGRADE
            if (CheckErrorStatus(m_sLastReturnString))

            {
                // There were no Upgrade or Failed or Locked
           

                // Now write the stuff into the registry!!!
                if (m_sLastReturnString.ToString().Contains("error"))
                {
                    MessageBox.Show("Agent Authentication failed.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    MessageBox.Show(m_sLastReturnString.ToString(), "");
                    return false;             

                }
                else
                {  
                    // Create an XmlReader
                    using (XmlReader reader = XmlReader.Create(new StringReader(m_sLastReturnString)))
                    {
                       
                            reader.ReadToFollowing("status");
                            if (reader.ReadElementContentAsString() == "new")
                            {

                                // the AgentID is the next (Current) node.
                                // and it was advanced automatically here by the Prev "ReadElementContentAsString"
                                AgentID = reader.ReadElementContentAsString();

                                reader.ReadToFollowing("deviceid");
                                DeviceIdentifier = reader.ReadElementContentAsString();

                                reader.ReadToFollowing("groupversion");
                                Groupversion = reader.ReadElementContentAsString();


                                // Write THIS TO THE REGISTRY FOR FURTHER REFERENCE
                                SetRegistryValue("agentid", AgentID);
                                SetRegistryValue("deviceidentifier", DeviceIdentifier);
                                SetRegistryValue("agentpassword", AgentPassword);         // This is the new password weve just set
                                SetRegistryValue("grpver", Groupversion);

                                SetRegistryValue("agentmsisdn", AgentMSISDN);  // This we get from the registry in the first place


                                // Update the REGISTRY lists as this is the first time the device is used
                                UpdateLists();

                                // If we are here we can delete the Setup.txt file
                                // because the startup process completed 100%
                                // The file is a flag to indicate a new stratup cycle

                                System.IO.File.Delete(ApplicationDirectory() + "\\setup.txt");

                                return true;

                            }
                            else
                            {
                                MessageBox.Show("Agent Authentication failed.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                                MessageBox.Show(m_sLastReturnString.ToString(), "");
                                Application.Exit();
                                return false;
                            }
                       
                    } // end using the XML READER

                    // "<mdas><command>password</command><device>intermec</device><agent><id></id><msisdn>27835551234</msisdn><deviceid></deviceid><password></password></agent><password><command>set</command><old></old><new>syntax10</new></password><system><version></version><language></language><imei></imei><firmware></firmware></system></mdas>"

                    //"<mdas><error>Agent already authenticated</error></mdas>"
                    // If there is an error message, authentication fails, and it will exit the application.

                    // Typical return
                    // "<mdas><agent><status>new</status><id>7202105029081</id><type>2</type><deviceid>-2138272020</deviceid></agent><groupversion>3</groupversion></mdas>"
                    //"<mdas><agent><status>new</status><id>7202105029081</id><type>2</type><deviceid>-2109734261</deviceid></agent><groupversion>3</groupversion></mdas>"

                    // And update the registry with the group informartion!!
                    // No Need to test cos the groups atre always the newest ones
            
            }

            }
            else
            {
                // There WERE an Upgrade or Failed or Locked errors
                // and they were reportes on already
                return false; 
            
            }

        }   // FIN GET AGENT DATA


        // This is called fior sacreen 2 where the AGENT has to enter and confirm his password (PIN)
        // And we authenticate him and get his data from the server.
        public bool AuthenticateAgentSecondTime(string agentpassword)
        {

            // Get the stored MSISDN in the registry
            string AgentMSISDN = GetRegistryValue("agentmsisdn", "");

            // Do the Second startup Stuff
            return GET_AGENT_STATUS(agentpassword,AgentMSISDN);

          
        }

        private bool GET_AGENT_STATUS(string AgentPassword, string AgentMSISDN)
        {
            // This is when the user log in for a SECOND time
            // We need to confirm that the user LEGIT to use the device.
            //
            // Lets say the device is stolen so we block teh agent on the server.
            // When the agent logs in it will terminate the session if the user does check-out

            string AgentID = GetRegistryValue("agentid", "");
            string DeviceIdentifier = GetRegistryValue("deviceidentifier", "");

            string XMLPACKET = getXmlAgentStatus(AgentPassword, AgentMSISDN,AgentID,DeviceIdentifier);
            PostRequest( XMLPACKET, 60);

            // m_sLastCommand = m_sLastCommand;

            // THIS HANDLES POSSIBLE ERRORS AND UPGRADE
            if (CheckErrorStatus(m_sLastReturnString))
            {

                // Now we can do a XML reader to determine 
                // If the ou was authenticated.

                if (m_sLastReturnString.Contains("<status>authenticated</status>"))
                {
                    //  User was Authenticated OKAY!!
                    //  MessageBox.Show(m_sLastReturnString,"");

                    // Get the version of the last list and determine if we need to load a new user 

                    using (XmlReader reader = XmlReader.Create(new StringReader(m_sLastReturnString)))
                    {

                        reader.ReadToFollowing("status");
                        if (reader.ReadElementContentAsString() == "authenticated")
                        {
                            reader.ReadToFollowing("groupversion");
                            Groupversion = reader.ReadElementContentAsString();

                            string RegistryGroupVersion = GetRegistryValue("Groupversion", "");

                            // the version cant be lower so if they are != me must load!
                            if (Groupversion != RegistryGroupVersion)
                            {
                                // Load ALL the lists into the registry.
                                UpdateLists();

                            }

                        }

                    }

                    // User was authenticated and the new lists were loaded in the registry
                    return true;

                }
                else
                {
                    // The user was not authenticated 
                    // Probably returned an <ERROR></ERROR>

                    MessageBox.Show("PIN Verification FAILED.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                    return false;
                }


            }  // Check_error_status close;

            else 
            
            {

                // There were errors or an UPGRADE or an FAILED or a LOCKED returned.
                // INFO Messages was already displayed for this.
                return false;            
            }

        }

        private bool CheckErrorStatus(string m_sLastReturnString)
        {

            // THIS WAS ADDED LAST BY ME 25 Oct 2010
            if (m_sLastReturnString.ToString().Contains("<status>upgrade</status>"))
            {
                // Call The Upgrade Screen
                UPGRADE(m_sLastReturnString);
                return false;

            }

            // you can only get upgrade status once I think. it then resets the agent hence you will get failed as it expects a new application to connect next time.
            if (m_sLastReturnString.ToString().Contains("<status>failed</status>"))
            {
                // Call The Upgrade Screen
                MessageBox.Show("FAILED Returned", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                Application.DoEvents();
                Application.Exit();
                return false;

            }

            // yeah, I think if you try 3 times or something
            if (m_sLastReturnString.ToString().Contains("<status>locked</status>"))
            {
                // Call The Upgrade Screen
                MessageBox.Show("Agent Locked", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                Application.DoEvents();
                Application.Exit();
                return false;

            }  
    
            // This retuirns true
            // As we have NO messages returned that is BAD
            return true;        
        
        }

        public bool UPGRADE(string upgradePacket)
        {

            // This attempts to upgrade the application 
            // if the packet result indicates an "<status>upgrade</status>"

            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

            // SL 12 November 2010
            // DELETES ALL REGISRTY ENTRIES TO TERMINATE THE CURRENT APPS FUNCTIONALLITY
            // If the upgrade went well, 
            // the setup.txt will load new settings in the registry

            // If the upgrade failed for some or other reason
            // There will be no SETUP.TXT AND NO registry entries to connect to the server
            // The App will prompt then for a manual download.

            // Clear registry
            ClearRegisty();


            // SL 01 November 2010
            // Gets the URL for the upgrade from the XML PAcket returned that flagged the upgrade
            double Startpos = upgradePacket.LastIndexOf("<url>")+5;
            double Endpos = upgradePacket.IndexOf("</url>");

            // EXTRACT THE URL FROM THIS
            int Dalength = (int)(Endpos - Startpos);
            UpgradeURL = upgradePacket.Substring((int)Startpos,Dalength ).Replace("#","=");
          
            // Just Redirect to explorer to let the user Accept the EULA run the app
            System.Diagnostics.Process.Start("iexplore.exe", UpgradeURL);

            // Run the files that we downloaded
            // System.Diagnostics.Process.Start(TargertDestination + TargetFile1, "");

            // Quit this App
            Application.Exit();

            // We dont use this value anyways
            return true;

        }


        private bool Downloadfile(String DaURL, String DaFilename, String DaTargetPaTH)
        {

            // DOWNLOADS A SPECIFIED FILE 
            // TO A SPECIFIED TARGET DESTINATIION

            try
            {

                WebRequest objRequest = System.Net.HttpWebRequest.Create(DaURL);

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
                        }
                    }
                }
            }
            catch (Exception EX)
            {

                MessageBox.Show(EX.Message,"Download Exception");

                return false;



            }

            // Happiness if we got here!!
            return true;

        }




        private void UpdateLists()
        {
            // This Traverses through the Groups of lists.
            // Queries the server and updates them in the registry.

            // Send a request to AXON
            // IDTYPE = 1  (THIS GETS HARDCODED NOW !!!!!!!)
            // NATIONALITY = 2 (THIS IS THE ONLY LIST WE read / Update NOW)
            // Language = en

            // The calling procedure mustr decide whether to update all the lists or not
            // based on the value of the grpver in the registry

            for (int groupIdentifier = 2; groupIdentifier < 5; groupIdentifier++)
            {

                string XMLPACKET = getXmlGroupList(groupIdentifier, "en");
                PostRequest(XMLPACKET, 60);

                // Now write the stuff into the registry!!!
                // m_sLastCommand = m_sLastCommand;

                if (m_sLastReturnString.ToString().Contains("error"))
                {
                    // Decide what to do 
                    MessageBox.Show(m_sLastReturnString.ToString(), "");
                    Application.Exit();
                }
                else
                {
                    // Create an XmlReader
                    using (XmlReader reader = XmlReader.Create(new StringReader(m_sLastReturnString)))
                    {
                        string Grouplist;
                        reader.ReadToFollowing("value");
                        Grouplist = reader.ReadElementContentAsString();

                        // GET the new group version from the xml-packet
                        reader.ReadToFollowing("groupversion");
                        Groupversion = reader.ReadElementContentAsString();

                        // Save the new group version
                        SetRegistryValue("grpver", Groupversion);

                        // Save the Grouplists in the Registry
                        switch (groupIdentifier)
                        {
                            case 1:
                                // This is obsolete now 12 Nov 2010
                                SetRegistryValue("f2f_grp_idtype_SIM", Grouplist);
                                break;
                            case 2:
                                SetRegistryValue("f2f_grp_nationality", Grouplist);
                                break;

                            // Added for the MM Stuff
                            case 3:
                                SetRegistryValue("f2f_grp_accountpurpose", Grouplist);
                                break;
                            case 4:
                                SetRegistryValue("f2f_grp_bank", Grouplist);
                                break;
                        }
                    }

                } // end using the XML READER

            } // end next Group Identifier 

        }

        // This is Called to update the comco boxes in the 
        public bool LOADContriols(ComboBox Cbb, int GroupItentifier)
        {
            try
            {

                // Update the combobox controls with the lists from the registry
                string[] idlist;

                switch (GroupItentifier)
                {
                    case 1:

                        // 12 Nov 2010
                        // We must HARDCODE the values for the id types 
                        // cos they differ for SIM and MMONEY
                        // We dont read them from or into the registry any more
                        // as per Sergei
                        // idlist = GetRegistryValue("f2f_grp_idtype_SIM", "").Split(':');
                        idlist = "Voter ID:Drivers Licence:NIA Card:Passport:NHIS Card".Split(':');
                        break;

                    case 2:
                        idlist = GetRegistryValue("f2f_grp_nationality", "").Split(':');
                        break;

                    case 3:
                        idlist = GetRegistryValue("f2f_grp_accountpurpose", "").Split(':');
                        break;

                    case 4:
                        idlist = GetRegistryValue("f2f_grp_bank", "").Split(':');
                        break;

                    case 100:

                        // 12 Nov 2010
                        // We must HARDCODE the values for the id types 
                        // cos they differ for SIM and MMONEY
                        // We dont read them from or into the registry any more
                        // as per Sergei
                        // idlist = GetRegistryValue("f2f_grp_idtype_MM", "").Split(':');
                        idlist = "National ID:Passport:Drivers Licence:Other:Voter ID".Split(':');
                        break;


                    default:
                        idlist = GetRegistryValue("default", "").Split(':');
                        break;

                }

                Cbb.Items.Clear();
                for (int i = 0; i < idlist.Count() ; i++)
                {

                    Cbb.Items.Add(idlist[i]);

                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        
        }



        // This Contacts the server and sends xml fwd and back
        private string PostRequest(string TheRequestPacket, int iTimeout)
        {

            // Load the connection Setting from the registry in to the class
            if (AXONServerURL == "") 
            {
                LoadServerSettings();
            }

            // POSTS a request to the server

            string result = "";
            StreamWriter myWriter = null;

            HttpWebRequest objRequest;
            objRequest = (HttpWebRequest)WebRequest.Create(AXONServerURL);
            objRequest.AllowWriteStreamBuffering = false;           

            // Figue this out
            /*
            if (UseProxy)
            {
                objRequest.Proxy = ProxyURL;
            }
            */

            objRequest.Method = "POST";
            objRequest.ContentLength = TheRequestPacket.Length; //  TheRequestPacket.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            objRequest.KeepAlive = false;

            objRequest.Timeout = (1000 * iTimeout);  // 1 minute

            // This sets the packet to send to the server.

            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(TheRequestPacket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "");
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
                MessageBox.Show(ex.Message.ToString(), "");
                return ex.Message;
            }
            finally
            {
                if (!(myWriter == null))
                {
                    myWriter.Close();
                }
            }

            m_sLastCommand = TheRequestPacket;
            m_sLastReturnString = result;

            // MessageBox.Show(result, "");

            return result;
        }


        /**
        * <p>Password manipulation commands.</p>
        * @param command
        * @param oldpass
        * @param newpass
        * @return
        */
        /* WE USE THIS AT FIRST-STARTUP TO SET THE PASSWORD OF THE AGENT*/
        /* IT TAKES AN MSISDN and new-PASSWORD */
        /* AND RETURNS XML WITH THE AGENT'S INFORMATION*/
        /* ITS A MULTI TASKING METHOD! */
        public String getXmlPassword(
                String command,
                String oldpass,
                String newpass,
                String MSISDN)
        {
            /* Extract the Version Data from the assembly*/
            Version vrs = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;           
            
            String xml =
                    "<mdas>"
                    + "<command>password</command>"
                    + "<device>intermec</device>"
                    + "<agent>"
                    + "<id></id>"
                    + "<msisdn>" + MSISDN + "</msisdn>"
                    + "<deviceid></deviceid>"
                    + "<password></password>"
                    + "</agent>"
                    + "<password>"
                    + "<command>" + command + "</command>"
                    + "<old>" + oldpass + "</old>"
                    + "<new>" + newpass + "</new>"
                    + "</password>"
                    + "<system>"
                    + "<version>" + vrs.ToString().Substring(0, vrs.ToString().Length - 0) + "</version>"
                    + "<language></language>"
                    + "<imei></imei>"
                    + "<firmware></firmware>"
                    + "</system>"
                    + "</mdas>";
            return xml;

            /*  This does the version thing.  */

            /*
            Version vrs = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show("Major: " + vrs.Major + "\r\nMinor: " + vrs.Minor);
            vrs.ToString()
            */


        }
        

        public String getXmlGroupList(int type, String lang)
        {
            // identification type = 1
            // nationality = 2
            // Language = en

            /*
            String message = "debug="+Constants.debug_r+
            ";cmd=grplst;type="+type+";dev=hs";
             */
            String xml =
                    "<mdas>"
                    + "<command>grouplist</command>"
                    + "<device>intermec</device>"
                    + "<grouplist>"
                    + "<type>" + type + "</type>"
                    + "<language>" + lang + "</language>"
                    + "</grouplist>"
                    + "</mdas>";
            return xml;
        }

        public String getXmlAgentStatus(string AgentPassword, string AgentMSISDN, string AgentID, string DeviceIdentifier)
        {
            /*
            String message = "debug="+Constants.debug_r+
            ";cmd=status;dev=hs;aid="+agent.id+
            ";amsisdn="+agent.msisdn+";adi="+
            agent.di+";imei="+Constants.PHONE_IMEI+
            ";ver="+Constants.APP_VERSION+
            ";grpver="+agent.grpver;
             */
            // this is what get agent status request looks like in XML

            /* Extract the Version Data from the assembly*/
            Version vrs = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            
            String xml =
                    "<mdas>"
                    + "<command>status</command>"
                    + "<device>intermec</device>"
                    + "<agent>"
                    + "<id>"+AgentID+"</id>"
                    + "<msisdn>" + AgentMSISDN + "</msisdn>"
                    + "<deviceid>"+ DeviceIdentifier + "</deviceid>"
                    + "<password>"+AgentPassword+"</password>"
                    + "</agent>"
                    + "<system>"
                    + "<version>"+ vrs.ToString().Substring(0, vrs.ToString().Length - 0) +"</version>"
                    + "<language></language>"
                    + "</system>"
                    + "</mdas>";
            
            return xml;
        }

        public String getXmlApplicationRef()
        {
            /*
             * 23-Sept-2010 Sergei:
             * Need to know on the backend what type of applicaiton this is
             * during getref command because this is when all kinds of checks
             * are performed to determine whether this applicaiton can be submitted
             */
            // convert boolean to 0 or 1

            /*
            String simswap = "0";
            if (principal.simswap)
            {
                simswap = "1";
            }
            String mmoney = "0";
            if (principal.mmoney)
            {
                mmoney = "1";
            }
            String simreg = "0";
            if (principal.simreg)
            {
                simreg = "1";
            }
            */

            // Read from the registry 
            // (Could have created an AGENT OBJECT FOR THIS!!!)
            AgentID = GetRegistryValue("agentid", "");
            AgentMSISDN = GetRegistryValue("agentmsisdn", "");
            DeviceIdentifier = GetRegistryValue("deviceidentifier", "");
            AgentPassword = GetRegistryValue("agentpassword", "");

            /* Extract the Version Data from the assembly*/
            Version vrs = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            
            // AS hardcoded for the handheld device
            String xml =
                    "<mdas>"
                    + "<command>getref</command>"
                    + "<device>intermec</device>"
                    + "<agent>"
                    + "<id>" + AgentID + "</id>"
                    + "<msisdn>" + AgentMSISDN + "</msisdn>"
                    + "<deviceid>" + DeviceIdentifier + "</deviceid>"
                    + "<password>" + AgentPassword + "</password>"
                    + "</agent>"
                /*
                 * 10-Jan-2010 Sergei:
                 * We need to offer the option to reject application if it is
                 * a duplicate (i.e. same ID number). It would be a waste to
                 * upload the data first (images, etc.) and only then reject.
                 * Hence add some customer information in the first message
                 * to the server which is used to obtain reference.
                 * Image buffering starts straight after this command followed
                 * eventually by the customer info.
                 *
                 * Note: I have also added the MSISDN in case we need to use
                 * the combination of ID/MSISDN to detemine a duplicate
                 */
                    + "<application>"
                    + "<principal>"
                    + "<id>"+ PrincipalID+ "</id>"
                    + "<msisdn>"+PrincipalMSISDN+"</msisdn>"
                /*
                 * See notes above. We need this information to kick off
                 * all kinds of checks on the backed before applicaiton
                 * is submitted.
                 */
                    + "<indicators>"
                    + "<mmoney>" + MMoney + "</mmoney>"
                    + "<simreg>" + SimReg + "</simreg>"
                    + "<simswap>" + SimSwap + "</simswap>"
                    + "</indicators>"
                    + "</principal>"
                    + "</application>"
                    + "<system>"
                    + "<version>" + vrs.ToString().Substring(0, vrs.ToString().Length - 0) + "</version>"
                    + "<language></language>"
                    + "</system>"
                    + "</mdas>";
            return xml;
        }

        /**
     * <p>XML for image upload.</p>
     * <p>Each image is processed and uploaded separately because
     * of the limitations of the mobile device. Encoded, individual image
     * can reach 100k which will put a lot of strain on the memory of the
     * handset.
     * </p>
     * @param ref
     * @param image
     * @param type
     * @return
     */
        public String getXmlUploadImage(String DaRef, string BASE_64_IMAGESTRING, ImageTypes type)
        {
            // base64 encode image so we can strap it onto XML message
            // String eimage = ImagetoStringBase64(imagePathAndName);
            // compose the image upload XML

            // Read from teh registry
            AgentID = GetRegistryValue("agentid", "");
            AgentMSISDN = GetRegistryValue("agentmsisdn", "");
            DeviceIdentifier = GetRegistryValue("deviceidentifier", "");
            AgentPassword = GetRegistryValue("agentpassword", "");

            /* Extract the Version Data from the assembly*/
            Version vrs = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;            

            String xml =
                    "<mdas>"
                    + "<command>submit</command>"
                    + "<device>intermec</device>"
                    + "<agent>"
                    + "<id>" + AgentID + "</id>"
                    + "<msisdn>" + AgentMSISDN + "</msisdn>"
                    + "<deviceid>" + DeviceIdentifier + "</deviceid>"
                    + "<password>" + AgentPassword + "</password>"
                    + "</agent>"
                    + "<application>"
                    + // reference for application submission. this must be obtained
                // from the server using the getref command.
                    "<reference>" + DaRef + "</reference>"
                    + // application type
                    "<type>document</type>"
                    + "<image>"
                    + "<type>" + (int)type + "</type>"
                    + "<data>" + BASE_64_IMAGESTRING + "</data>"
                    + "</image>"
                    + "</application>"
                    + "<system>"
                    + "<version>" + vrs.ToString().Substring(0, vrs.ToString().Length - 0) + "</version>"
                    + "<language></language>"
                    + "</system>"
                    + "</mdas>";
            return xml;
        }

        /**
    * <p>Command to upload customer application to the server.
    * </p>
    * @param ref	reference number for application submission.
    * @return
    */
        public String getXmlUploadApplication(String DaRef)
        {


            // convert boolean to 0 or 1
            /*
            String simswap = "0";
            if (principal.simswap) {
                simswap = "1";
            }
            String mmoney = "0";
            if (principal.mmoney) {
                mmoney = "1";
            }
            String simreg = "0";
            if (principal.simreg) {
                simreg = "1";
            }
            String newcust = "0";
            if (principal.newCustomer) {
                newcust = "1";
            }
             */
        
            /*
            String mmoney = "1";
            String simswap = "0";
            String simreg = "0";

            String newcust = "1";
            */
          

            // Read from the registry
            AgentID = GetRegistryValue("agentid", "");
            AgentMSISDN = GetRegistryValue("agentmsisdn", "");
            DeviceIdentifier = GetRegistryValue("deviceidentifier", "");
            AgentPassword = GetRegistryValue("agentpassword", "");

            /* Extract the Version Data from the assembly*/
            Version vrs = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            
            String xml =
                    "<mdas>"
                    + "<command>submit</command>"
                    + "<device>intermec</device>"
                    + "<agent>"
                    + "<id>" + AgentID + "</id>"
                    + "<msisdn>" + AgentMSISDN + "</msisdn>"
                    + "<deviceid>" + DeviceIdentifier + "</deviceid>"
                    + "<password>" + AgentPassword + "</password>"
                    + "</agent>"
                    + "<application>"
                    + // reference for application submission. this must be obtained
                // from the server using the getref command.
                    "<reference>" + DaRef + "</reference>"
                    + "<type>application</type>"
                    + "<principal>"
                    + "<id>"+PrincipalID+"</id>"
                    + "<idtype>"+PrincipalIDtype+"</idtype>"
                    + "<birthdate>"+PrincipalBirthDate+"</birthdate>"  // yyyy-mm-dd Format!!!
                    + "<name>"+ PrincipalName+"</name>"
                    + "<surname>"+PrincipalSurName+"</surname>"
                    + "<nationality>"+PrincipalNationality+"</nationality>"
                    + "<msisdn>"+PrincipalMSISDN+"</msisdn>"
                    + "<nok>"+PrincipalNextOfKin+"</nok>"
                    + "<nokmsisdn>"+PrincipalNextOfKinNumber+"</nokmsisdn>"
                // sim registration additions
                    + "<new_customer>" + Convert.ToInt32(newCustomer) + "</new_customer>"
                    + "<indicators>"
                    + "<mmoney>" + Convert.ToInt32(MMoney) + "</mmoney>"
                    + "<simreg>" + Convert.ToInt32(SimReg) + "</simreg>"
                    + "<simswap>" + Convert.ToInt32(SimSwap) + "</simswap>"
                    + "</indicators>"
                    + // 30-03-2009: address added on request.
                    "<address>"
                    + "<line1>"+PrincipalAddress1+"</line1>"
                    + "<line2>" + PrincipalAddress2 + "</line2>"
                    + "<line3>" + PrincipalAddress3 + "</line3>"
                    + "</address>"
                    + "<sim>"+Sim+"</sim>"
                    + "<puk>"+Puk+"</puk>"
                    + /*
                 * 13-Dec-2008 Sergei: adding account purpose
                 */
                        "<account_purpose>"+ PrincipalAccountPurpose +"</account_purpose>"
                    + /*
                 * 14-Jun-2009 Sergei: adding customer bank
                 */
                        "<bank>"+PrincipalBank+"</bank>"
                    + "</principal>"
                    + "<guardian>"
                    + /*
                 * 08-Dec-2008 Sergei: removing name and contacts from guardian
                 *
                "<name>" + (guardian.name == null ? "" : guardian.name) + "</name>" +
                "<contacts>" + (guardian.contacts == null ? "" : guardian.contacts) + "</contacts>" +
                 */
                        "<id>"+ GaurdID +"</id>"
                    + "<idtype>"+GaurdIDType+"</idtype>"
                    + "</guardian>";
            xml +=
                    "</application>"
                    + "<system>"
                    + "<version>"+ vrs.ToString().Substring(0, vrs.ToString().Length - 0) +"</version>"
                    + "<language></language>"
                    + "</system>"
                    + "</mdas>";
            return xml;
        }



        #endregion 

        ///////////////////////////////////////////////////////////////
        // MMM                                                       //
        // Acceopts an xml file and does the entire thing-a-mejiekie //
        //                                                           //
        // get a ref                                                 //
        // send the images                                           //
        // send the data                                             //
        // Pee in the bath.                                          //
        //                                                           //
        // 1 Func to call from the click button in the UI            //
        ///////////////////////////////////////////////////////////////

        public bool UpoadXMLFILE(string ThexmlFilePath) 
        {

            if (IsSerevrAvailable())
            {

                // open the xml file wit the XML READER
                using (XmlReader reader = XmlReader.Create(ThexmlFilePath))
                {
                    reader.ReadToFollowing("name");
                    PrincipalName = reader.ReadElementContentAsString();

                    //reader.ReadToFollowing("surname");
                    PrincipalSurName = reader.ReadElementContentAsString();

                    //reader.ReadToFollowing("nationality");
                    PrincipalNationality = reader.ReadElementContentAsString();

                    // needed for getXmlApplicationRef
                    //reader.ReadToFollowing("id");
                    PrincipalID = reader.ReadElementContentAsString();

                    //reader.ReadToFollowing("idtype");
                    PrincipalIDtype = reader.ReadElementContentAsString();

                    //reader.ReadToFollowing("birthdate");
                    PrincipalBirthDate = reader.ReadElementContentAsString();

                    // needed for getXmlApplicationRef
                    //reader.ReadToFollowing("msisdn");
                    PrincipalMSISDN = reader.ReadElementContentAsString();

                    //reader.ReadToFollowing("email");
                    PrincipalEmail = reader.ReadElementContentAsString();

                    //reader.ReadToFollowing("bank");
                    PrincipalBank = reader.ReadElementContentAsString();

                    // Added SL 25 Nov 2010
                    PrincipalAccountPurpose = reader.ReadElementContentAsString();

                    PrincipalNextOfKin = reader.ReadElementContentAsString();

                    PrincipalNextOfKinNumber = reader.ReadElementContentAsString();

                    // Read the address

                    reader.ReadToFollowing("line1");
                    PrincipalAddress1 = reader.ReadElementContentAsString();

                    //reader.ReadToFollowing("line2");
                    PrincipalAddress2 = reader.ReadElementContentAsString();

                    //reader.ReadToFollowing("line3");
                    PrincipalAddress3 = reader.ReadElementContentAsString();

                    reader.ReadToFollowing("new");

                    newCustomer = StringToBool(reader.ReadElementContentAsString());

                    // needed for getXmlApplicationRef
                    // reader.ReadToFollowing("simswap");
                    SimSwap = StringToBool(reader.ReadElementContentAsString());

                    // needed for getXmlApplicationRef
                    // reader.ReadToFollowing("simreg");
                    SimReg = StringToBool(reader.ReadElementContentAsString());

                    // needed for getXmlApplicationRef
                    // reader.ReadToFollowing("mmoney");
                    MMoney = StringToBool(reader.ReadElementContentAsString());

                    // reader.ReadToFollowing("sim");
                    Sim = reader.ReadElementContentAsString();

                    // reader.ReadToFollowing("puk");
                    Puk = reader.ReadElementContentAsString();

                    // We need to read the gaurdian details at the end of the file
                    // Here we skip the Data Packets 

                    reader.ReadToFollowing("guardian");
                    reader.ReadToFollowing("id");
                    GaurdID = reader.ReadElementContentAsString();
                    GaurdIDType = reader.ReadElementContentAsString();

                } // USING

                // get xml SUBMISSION Ref!!!!!!!!!
                string XMLPACKET = getXmlApplicationRef();
                string xmlAppref = PostRequest(XMLPACKET, 60);

                if (xmlAppref.Contains("<status>success</status>"))
                {
                    // Open the XML READER TO GET THE REF

                    // Create an XmlReader
                    string DaTransactionRef;
                    using (XmlReader RefReader = XmlReader.Create(new StringReader(xmlAppref)))
                    {
                        RefReader.ReadToFollowing("reference");
                        DaTransactionRef = RefReader.ReadElementContentAsString();
                    }

                    // #1 Send Da Images 

                    // Traverse all the images till they are gone.
                    // Read until; you get a null node or something
                    using (XmlReader reader = XmlReader.Create(ThexmlFilePath))
                    {

                        while (reader.ReadToFollowing("image"))
                        {

                            // Get the image data and send it
                            // Send up Image
                            if (reader.ReadToFollowing("data"))
                            {
                                // read the image and teh type
                                string DaBASE64Image = reader.ReadElementContentAsString();
                                int DaImageType = Convert.ToInt32(reader.ReadElementContentAsString());

                                XMLPACKET = getXmlUploadImage(DaTransactionRef, DaBASE64Image, (ImageTypes)DaImageType);
                                string ImageSendStatus = PostRequest(XMLPACKET, 60);
                                if (!ImageSendStatus.Contains("<status>success</status>"))
                                {
                                    // Throw exception                            
                                    MessageBox.Show("Image Send failed " + ImageSendStatus, "");
                                    Application.DoEvents();
                                    return false;
                                }
                            }

                        }

                    } // Using for the images

                    // Send the data here

                    // #2 SEND THE APPLICATION DATA
                    XMLPACKET = getXmlUploadApplication(DaTransactionRef);
                    string dataSendStatus = PostRequest(XMLPACKET, 60);

                    if (dataSendStatus.Contains("<error>"))
                    {
                        MessageBox.Show("Sending data Failed!" + ThexmlFilePath + dataSendStatus, "");
                        Application.DoEvents();
                        return false;
                    }
                    else
                    {
                        // MessageBox.Show("Sending data OKAY!!!!" + ThexmlFilePath, "");
                        Application.DoEvents();
                        return true;
                    }

                }
                else
                {
                    // No Ref here

                    MessageBox.Show("No Ref returned for upload");
                    Application.DoEvents();
                    return false;
                }

            }
            else
            { 
                // Server not available
                // MessageBox.Show("SAN", "Proc UpoadXMLFILE");

                Application.DoEvents();
                return false;
            }

        }

        // MMM Writes the data to XML File!!
        public void SaveUserRecord(string WorkingDirectory)
        {

            // Check if the dir exist bedfore we save!!!
            // This created the problem with the firsttime onsave that did not work at all!
            // This should be sorted now
            // SL 23 Nov 2010

            // Create a reference to the current directory.
            DirectoryInfo di = new DirectoryInfo(WorkingDirectory);

            // If the xml-file directory does not exist, create it
            if (!di.Exists)
            {
                di.Create();
            }

            XmlWriter DaWriter = XmlWriter.Create(WorkingDirectory + _PrincipalID + ".F2F");

        using (DaWriter)
        {

            // THIS IS THE FORMAT OF THE XML FILE THAT GETS SAVED ON THE DISK.

            DaWriter.WriteStartElement("application");

            DaWriter.WriteElementString("sms", "");

            DaWriter.WriteStartElement("principal");
            DaWriter.WriteElementString("name", _PrincipalName);
            DaWriter.WriteElementString("surname", _PrincipalSurname);
            DaWriter.WriteElementString("nationality", _PrincipalNationality);
            DaWriter.WriteElementString("id", _PrincipalID);
            DaWriter.WriteElementString("idtype", _Principalidtype);
            DaWriter.WriteElementString("birthdate", _Principalbirthdate);  // FOR TESTING!!
            DaWriter.WriteElementString("msisdn", _Principalmsisdn);
            DaWriter.WriteElementString("email", _Principalemail);
            DaWriter.WriteElementString("bank", _Principalbank);
            DaWriter.WriteElementString("accountpurpose", _PrincipalAccountPurpose);
            DaWriter.WriteElementString("nextofkin", _Principalnextofkin);
            DaWriter.WriteElementString("nextofkinnumber", _PrincipalnextofkinNumber);
            DaWriter.WriteStartElement("address");
                DaWriter.WriteElementString("line1", _PrincipalAddress1);
                DaWriter.WriteElementString("line2", _PrincipalAddress2);
                DaWriter.WriteElementString("line3", _PrincipalAddress3);
            DaWriter.WriteEndElement();  // address
            DaWriter.WriteElementString("new", BooltoString(_newCustomer));
            DaWriter.WriteElementString("simswap", BooltoString(_simswap));
            DaWriter.WriteElementString("simreg", BooltoString(_simreg));
            DaWriter.WriteElementString("mmoney", BooltoString(_mmoney));

            Application.DoEvents();

            // Only for SimSwap
            DaWriter.WriteElementString("sim", _sim);
            DaWriter.WriteElementString("puk", _puk);

            Application.DoEvents();

            // Id Photo
            DaWriter.WriteStartElement("image");
            DaWriter.WriteElementString("data", ImagetoStringBase64(_idImageData));
                DaWriter.WriteElementString("type", _idImageType.ToString());
            DaWriter.WriteEndElement();  // image

            Application.DoEvents();
        
            // Application Proof
            DaWriter.WriteStartElement("image", "");
            DaWriter.WriteElementString("data", ImagetoStringBase64(_appImageData));
                DaWriter.WriteElementString("type", _appImageType.ToString());
            DaWriter.WriteEndElement();  // image

            Application.DoEvents();

            // Customer Photo
            DaWriter.WriteStartElement("image");
            DaWriter.WriteElementString("data", ImagetoStringBase64(_custImageData));
                DaWriter.WriteElementString("type", _custImageType.ToString());
            DaWriter.WriteEndElement();  // image

            Application.DoEvents();

            DaWriter.WriteEndElement();  // PRINCIPAL

            DaWriter.WriteStartElement("guardian");
                DaWriter.WriteElementString("id", _gaurdID);
                DaWriter.WriteElementString("idtype", GaurdIDType);
                DaWriter.WriteEndElement();  // guardian

            Application.DoEvents();

            DaWriter.WriteStartElement("agent");
                DaWriter.WriteElementString("id", _agentID);
                DaWriter.WriteElementString("msisdn", _agentMSISDN);
            DaWriter.WriteEndElement();  // agent

            Application.DoEvents();

            DaWriter.WriteEndElement();  // Application

            DaWriter.Close();
        }
                
        }        
         
        private string ImagetoStringBase64(string Imagename)
        {
            byte[] b;
            // b = new byte[100];
            FileStream f = null;

            try
            {
                f = new System.IO.FileStream(Imagename, FileMode.Open);
                b = new byte[f.Length - 1];
                int l = (int)f.Length;
                f.Read(b, 0, l - 1);
                return System.Convert.ToBase64String(b);               
            }
            catch (Exception eX)
            {
                return "";
            }
            finally
            {
                if (f == null)
                {
                    // Fine
                }
                else
                {
                    f.Close();
                }
            }

        }

        public string GetRegistryValue(string KeyName, string DefaultValue)
        {
            // Read it from "one Source"
            string ret = Microsoft.Win32.Registry.GetValue(REGISTRY_HKEY_PATH, KeyName, DefaultValue).ToString();

            if (ret == "")
            {   // try Concatinated Values if a Single value failed
                ret = Microsoft.Win32.Registry.GetValue(REGISTRY_HKEY_PATH, KeyName + "_1", DefaultValue).ToString();
                ret = ret + Microsoft.Win32.Registry.GetValue(REGISTRY_HKEY_PATH, KeyName + "_2", DefaultValue).ToString();
            }


            if (ret == null)
            {
                return DefaultValue;
            }
            else
            {
                return ret;
            }
        }

        public void SetRegistryValue(string KeyName, string Value)
        {
            // One character takes up 2 bytes 
            // And the max size for a registry value is 4096 bytes
            if ((Value.Length * 2) > 4096)
            {   // C# is zero based
                Microsoft.Win32.Registry.SetValue(REGISTRY_HKEY_PATH, KeyName + "_1", Value.Substring(0, 2047));
                Microsoft.Win32.Registry.SetValue(REGISTRY_HKEY_PATH, KeyName + "_2", Value.Substring(2048));
            }
            else
            {  // Write it "Normally"
                Microsoft.Win32.Registry.SetValue(REGISTRY_HKEY_PATH, KeyName, Value);
            }
        }
        
        // This returns the current Application Directory
       public String ApplicationDirectory()
       {
           Module[] Mod;
           Mod = Assembly.GetExecutingAssembly().GetModules();
           return Path.GetDirectoryName(Mod[0].FullyQualifiedName);


       }

       // This converts the date entered by the user DD/MON/YYYY
       // to a date in the format DD-MM-YYYY for the backend
       // Bug Fix 12 November 2010
       public string DateUIToDateBackend(string FrontendDate)
       {

        String DaYear = FrontendDate.Substring(7,4);
        String DaDay = FrontendDate.Substring(0,2);
        String DaMonth = "";

        switch (FrontendDate.Substring(3, 3))
        {
            case "JAN":
                DaMonth = "01";
                break;

            case "FEB":
                DaMonth = "02";
                break;

            case "MAR":
                DaMonth = "03";
                break;

            case "APR":
                DaMonth = "04";
                break;

            case "MAY":
                DaMonth = "05";
                break;

            case "JUN":
                DaMonth = "06";
                break;

            case "JUL":
                DaMonth = "07";
                break;

            case "AUG":
                DaMonth = "08";
                break;

            case "SEP":
                DaMonth = "09";
                break;

            case "OCT":
                DaMonth = "10";
                break;

            case "NOV":
                DaMonth = "11";
                break;

            case "DEC":
                DaMonth = "12";
                break;
        }

        return (DaDay + "-" + DaMonth + "-" + DaYear);
   
       }


       // This converts the date entered by the user DD/MON/YYYY
       // to a date in the format MM-DD-YYYY for the "AGE COMARISON STUFF"
       // Bug Fix 26 November 2010
       public string DateUIToDateBackendMMDDYYYY(string FrontendDate)
       {

           String DaYear = FrontendDate.Substring(7, 4);
           String DaDay = FrontendDate.Substring(0, 2);
           String DaMonth = "";

           switch (FrontendDate.Substring(3, 3))
           {
               case "JAN":
                   DaMonth = "01";
                   break;

               case "FEB":
                   DaMonth = "02";
                   break;

               case "MAR":
                   DaMonth = "03";
                   break;

               case "APR":
                   DaMonth = "04";
                   break;

               case "MAY":
                   DaMonth = "05";
                   break;

               case "JUN":
                   DaMonth = "06";
                   break;

               case "JUL":
                   DaMonth = "07";
                   break;

               case "AUG":
                   DaMonth = "08";
                   break;

               case "SEP":
                   DaMonth = "09";
                   break;

               case "OCT":
                   DaMonth = "10";
                   break;

               case "NOV":
                   DaMonth = "11";
                   break;

               case "DEC":
                   DaMonth = "12";
                   break;
           }

           return (DaMonth + "-" + DaDay + "-" + DaYear);

       }

       public string DateXMLTOUI(string XMLDATE)
       { 
        // Does exactly the opposite of the ABOVE
        // Converts 10-02-1972 to 10/FEB/1972
        // Bug Fix 12 November 2010
       
        string daDay = "";
        string daMonth = "";
        string daYear = "";

        daDay = XMLDATE.Substring(0, 2);
        daMonth = XMLDATE.Substring(3, 2);
        daYear = XMLDATE.Substring(6, 4);

        switch (daMonth)
        {
            case "01":
                daMonth = "JAN";
                break;

            case "02":
                daMonth = "FEB";
                break;

            case "03":
                daMonth = "MAR";
                break;

            case "04":
                daMonth = "APR";
                break;

            case "05":
                daMonth = "MAY";
                break;

            case "06":
                daMonth = "JUN";
                break;

            case "07":
                daMonth = "JUL";
                break;

            case "08":
                daMonth = "AUG";
                break;

            case "09":
                daMonth = "SEP";
                break;

            case "10":
                daMonth = "OCT";
                break;

            case "11":
                daMonth = "NOV";
                break;

            case "12":
                daMonth = "DES";
                break;

        }
                
        // Return the vales stated here

        return (daDay + "/" + daMonth + "/" + daYear);
       }



       // MY PRIVATE STUFF - Stupid Code
       public bool StringToBool(string BooleanString)
       {
           if (BooleanString.StartsWith("1"))
           {
               return true;
           }
           else
           {
               return false;
           }
       }

       public string BooltoString(bool B)
       {
           if (B)
           { return "1"; }
           else
           { return "0"; }

       }

        // Releases this object from memory 
        public void dispose()
        {
        this.dispose();
        }

    }
}
