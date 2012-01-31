 using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

public class PortChat
{
static bool _continue;
static SerialPort _uniproPort, _witsPort;
static Configuration c = Configuration.Deserialize("u2wits.xml");

public static void Main()
 {

string message;

//Configuration c = new Configuration();


//Configuration c = Configuration.Deserialize("u2wits.xml");

 StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
 Thread readThread = new Thread(Read);

// Create a new SerialPort object with default settings.
 _uniproPort = new SerialPort();
 _witsPort = new SerialPort();

// Allow the user to set the appropriate properties.
 if (c.uniproName == "") c.uniproName = SetPortName(_uniproPort.PortName);
 _uniproPort.PortName = c.uniproName;

 if (c.uniproBaudRate == -1) c.uniproBaudRate = SetPortBaudRate(_uniproPort.BaudRate);
 _uniproPort.BaudRate = c.uniproBaudRate;

 if (c.uniproParity == "") c.uniproParity = SetPortParity(_uniproPort.Parity);
 _uniproPort.Parity = (Parity)Enum.Parse(typeof(Parity), c.uniproParity);

 if (c.uniproDataBits == -1) c.uniproDataBits = SetPortDataBits(_uniproPort.DataBits);
 _uniproPort.DataBits = c.uniproDataBits;

 if (c.uniproStopBits == "") c.uniproStopBits = SetPortStopBits(_uniproPort.StopBits);
 _uniproPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits),c.uniproStopBits);

 if (c.uniproHandshake == "") c.uniproHandshake = SetPortHandshake(_uniproPort.Handshake);
 _uniproPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), c.uniproHandshake);

 if (c.uniproPressure == -1) c.uniproPressure = SetPressure();

 if (c.uniproDensity == -1) c.uniproDensity = SetDensity();

 if (c.uniproRate == -1) c.uniproRate = SetRate();

 if (c.uniproVolume == -1) c.uniproVolume = SetVolume();

 if (c.witsName == "") c.witsName = SetPortName(_witsPort.PortName);
 _witsPort.PortName = c.witsName;

 if (c.witsBaudRate == -1) c.witsBaudRate = SetPortBaudRate(_witsPort.BaudRate);
 _witsPort.BaudRate = c.witsBaudRate;

 if (c.witsParity == "") c.witsParity = SetPortParity(_witsPort.Parity);
 _witsPort.Parity = (Parity)Enum.Parse(typeof(Parity), c.witsParity);

 if (c.witsDataBits == -1) c.witsDataBits = SetPortDataBits(_witsPort.DataBits);
 _witsPort.DataBits = c.witsDataBits;

 if (c.witsStopBits == "") c.witsStopBits = SetPortStopBits(_witsPort.StopBits);
 _witsPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), c.witsStopBits);

 if (c.witsHandshake == "") c.witsHandshake = SetPortHandshake(_witsPort.Handshake);
 _witsPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), c.witsHandshake);

 if (c.witsHeadder == "") c.witsHeadder = SetHeadder();

 if (c.witsFotter == "") c.witsFotter = SetFotter();


 Configuration.Serialize("u2wits.xml", c);


// Set the read/write timeouts
 _uniproPort.ReadTimeout = 500;
 _uniproPort.WriteTimeout = 500;
 _witsPort.ReadTimeout = 500;
 _witsPort.WriteTimeout = 500;

 _uniproPort.Open();
 _witsPort.Open();
 _continue = true;
 readThread.Start();

 Console.WriteLine("Type QUIT to exit");

while (_continue)
 {
 message = Console.ReadLine();

if (stringComparer.Equals("quit", message))
 {
 _continue = false;
 }
 }

 readThread.Join();
 _uniproPort.Close();
 }

public static void Read()
 {
while (_continue)
 {
try
 {
string message = _uniproPort.ReadLine();
Console.Clear();
Console.WriteLine("This program is made by Esp1 cmt nrwy,");
Console.WriteLine();
Console.WriteLine("UniproII input ("+ _uniproPort.PortName +"):");
 Console.WriteLine(message);
 String[] words = message.Split(' ');
 Console.WriteLine();
 Console.WriteLine("WITS output (" + _witsPort.PortName +"):");
 Console.WriteLine(c.witsHeadder);
 Console.WriteLine("1712"+words[c.uniproPressure]);//press
 Console.WriteLine("1716"+words[c.uniproRate]);//rate
 Console.WriteLine("1719"+words[c.uniproDensity]);//dens
 Console.WriteLine("1729"+words[c.uniproVolume]);//vol
 Console.WriteLine(c.witsFotter);

 _witsPort.WriteLine(c.witsHeadder);
 _witsPort.WriteLine("1712" + words[c.uniproPressure]);//press
 _witsPort.WriteLine("1716" + words[c.uniproRate]);//rate
 _witsPort.WriteLine("1719" + words[c.uniproDensity]);//dens
 _witsPort.WriteLine("1729" + words[c.uniproVolume]);//vol
 _witsPort.WriteLine(c.witsFotter);

 }
catch (TimeoutException) { }
 }
 }

public static string SetPortName(string defaultPortName)
 {
string portName;

 Console.WriteLine("Available Ports:");
foreach (string s in SerialPort.GetPortNames())
 {
 Console.WriteLine(" {0}", s);
 }

 Console.Write("COM port({0}): ", defaultPortName);
 portName = Console.ReadLine();

if (portName == "")
 {
 portName = defaultPortName;
 }
return portName;
 }

public static int SetPortBaudRate(int defaultPortBaudRate)
 {
string baudRate;

 Console.Write("Baud Rate({0}): ", defaultPortBaudRate);
 baudRate = Console.ReadLine();

if (baudRate == "")
 {
 baudRate = defaultPortBaudRate.ToString();
 }

return int.Parse(baudRate);
 }

public static string SetPortParity(Parity defaultPortParity)
 {
string parity;

 Console.WriteLine("Available Parity options:");
foreach (string s in Enum.GetNames(typeof(Parity)))
 {
 Console.WriteLine(" {0}", s);
 }

 Console.Write("Parity({0}):", defaultPortParity.ToString());
 parity = Console.ReadLine();

if (parity == "")
 {
 parity = defaultPortParity.ToString();
 }

//return (Parity)Enum.Parse(typeof(Parity), parity);
return parity;
 }

public static int SetPortDataBits(int defaultPortDataBits)
 {
string dataBits;

 Console.Write("Data Bits({0}): ", defaultPortDataBits);
 dataBits = Console.ReadLine();

if (dataBits == "")
 {
 dataBits = defaultPortDataBits.ToString();
 }

return int.Parse(dataBits);
 }

public static string SetPortStopBits(StopBits defaultPortStopBits)
 {
string stopBits;

 Console.WriteLine("Available Stop Bits options:");
foreach (string s in Enum.GetNames(typeof(StopBits)))
 {
 Console.WriteLine(" {0}", s);
 }

 Console.Write("Stop Bits({0}):", defaultPortStopBits.ToString());
 stopBits = Console.ReadLine();

if (stopBits == "")
 {
 stopBits = defaultPortStopBits.ToString();
 }

return stopBits;
//return (StopBits)Enum.Parse(typeof(StopBits), stopBits);
 }

public static string SetPortHandshake(Handshake defaultPortHandshake)
 {
string handshake;

 Console.WriteLine("Available Handshake options:");
foreach (string s in Enum.GetNames(typeof(Handshake)))
 {
 Console.WriteLine(" {0}", s);
 }

 Console.Write("Handshake({0}):", defaultPortHandshake.ToString());
 handshake = Console.ReadLine();

if (handshake == "")
 {
 handshake = defaultPortHandshake.ToString();
 }
return handshake;
//return (Handshake)Enum.Parse(typeof(Handshake), handshake);
 }

public static int SetPressure()
{
    string Pressure;

    Console.WriteLine("Pressure location:");
    

    Console.Write("(8): ");
    Pressure = Console.ReadLine();

    if (Pressure == "")
    {
        Pressure = "8";
    }
    return int.Parse(Pressure);
}

public static int SetDensity()
{
    string Density;

    Console.WriteLine("Density location:");


    Console.Write("(4): ");
    Density = Console.ReadLine();

    if (Density == "")
    {
        Density = "4";
    }
    return int.Parse(Density);
}

public static int SetRate()
{
    string Rate;

    Console.WriteLine("Rate location:");


    Console.Write("(18): ");
    Rate = Console.ReadLine();

    if (Rate == "")
    {
        Rate = "18";
    }
    return int.Parse(Rate);
}

public static int SetVolume()
{
    string Volume;

    Console.WriteLine("Volume location:");


    Console.Write("(19): ");
    Volume = Console.ReadLine();

    if (Volume == "")
    {
        Volume = "19";
    }
    return int.Parse(Volume);
}

public static string SetHeadder()
{
    string Headder;

    Console.WriteLine("WITS Headder:");


    Console.Write("(&&): ");
    Headder = Console.ReadLine();

    if (Headder == "")
    {
        Headder = "&&";
    }
    return Headder;
}

public static string SetFotter()
{
    string Fotter;

    Console.WriteLine("WITS Fotter:");


    Console.Write("(!!): ");
    Fotter = Console.ReadLine();

    if (Fotter == "")
    {
        Fotter = "!!";
    }
    return Fotter;
}



#region -- Configuration Class --
/// <summary>
/// static methods to manage the serialization to and deserialization from a simple XML file.
/// </summary>
[Serializable]
public class Configuration
{
    int _Version;
    
    string _uniproName;
    int _uniproBaudRate;
    string _uniproParity;
    int _uniproDataBits;
    string _uniproStopBits;
    string _uniproHandshake;

    string _witsName;
    int _witsBaudRate;
    string _witsParity;
    int _witsDataBits;
    string _witsStopBits;
    string _witsHandshake;

    int _uniproPressure;
    int _uniproDensity;
    int _uniproRate;
    int _uniproVolume;

    string _witsHeadder;
    string _witsFotter;

    public Configuration()
    {
        _Version = 1;
        _uniproName = "";
        _uniproBaudRate = -1;
        _uniproParity = "";
        _uniproDataBits = -1;
        _uniproStopBits = "";
        _uniproHandshake = "";
        _uniproPressure = -1;
        _uniproDensity = -1;
        _uniproRate = -1;
        _uniproVolume = -1;
        _witsName = "";
        _witsBaudRate = -1;
        _witsParity = "";
        _witsDataBits = -1;
        _witsStopBits = "";
        _witsHandshake = "";
        _witsHeadder = "";
        _witsFotter = "";
    }
    public static void Serialize(string file, Configuration c)
    {
        System.Xml.Serialization.XmlSerializer xs
           = new System.Xml.Serialization.XmlSerializer(c.GetType());
        StreamWriter writer = File.CreateText(file);
        xs.Serialize(writer, c);
        writer.Flush();
        writer.Close();
    }
    public static Configuration Deserialize(string file)
    {
        System.Xml.Serialization.XmlSerializer xs
           = new System.Xml.Serialization.XmlSerializer(
              typeof(Configuration));
        StreamReader reader = File.OpenText(file);
        Configuration c = (Configuration)xs.Deserialize(reader);
        reader.Close();
        return c;
    }
    public int Version
    {
        get { return _Version; }
        set { _Version = value; }
    }
    public string uniproName
    {
        get { return _uniproName; }
        set { _uniproName = value; }
    }
    public int uniproBaudRate
    {
        get { return _uniproBaudRate; }
        set { _uniproBaudRate = value; }
    }
    public string uniproParity
    {
        get { return _uniproParity; }
        set { _uniproParity = value; }
    }
    public int uniproDataBits
    {
        get { return _uniproDataBits; }
        set { _uniproDataBits = value; }
    }
    public string uniproStopBits
    {
        get { return _uniproStopBits; }
        set { _uniproStopBits = value; }
    }
    public string uniproHandshake
    {
        get { return _uniproHandshake; }
        set { _uniproHandshake = value; }
    }
    public int uniproPressure
    {
        get { return _uniproPressure; }
        set { _uniproPressure = value; }
    }
    public int uniproDensity
    {
        get { return _uniproDensity; }
        set { _uniproDensity = value; }
    }
    public int uniproRate
    {
        get { return _uniproRate; }
        set { _uniproRate = value; }
    }
    public int uniproVolume
    {
        get { return _uniproVolume; }
        set { _uniproVolume = value; }
    }

    public string witsName
    {
        get { return _witsName; }
        set { _witsName = value; }
    }
    public int witsBaudRate
    {
        get { return _witsBaudRate; }
        set { _witsBaudRate = value; }
    }
    public string witsParity
    {
        get { return _witsParity; }
        set { _witsParity = value; }
    }
    public int witsDataBits
    {
        get { return _witsDataBits; }
        set { _witsDataBits = value; }
    }
    public string witsStopBits
    {
        get { return _witsStopBits; }
        set { _witsStopBits = value; }
    }
    public string witsHandshake
    {
        get { return _witsHandshake; }
        set { _witsHandshake = value; }
    }
    public string witsHeadder
    {
        get { return _witsHeadder; }
        set { _witsHeadder = value; }
    }
    public string witsFotter
    {
        get { return _witsFotter; }
        set { _witsFotter = value; }
    }


}
#endregion
}