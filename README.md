**WARNING**

This repo is published for educational purposes only. 
No one is being encouraged to use unlicensed software, consider it an example of how not to protect an application.
This system is publicly available and has been working for years across multiple versions, including the latest one, and it doesn’t seem like the developer is interested in patching it.
If you like the software, buy it!

# NetLimiter Patcher

Sets the license to 99999 days and forces license registration, it currently works with all available versions.

![NetLimiter Patcher](https://muninn.ovh/Patcher/NetLimiter/NetLimiterPatcher.png)

[**DOWNLOAD**](https://muninn.ovh/Patcher/NetLimiter/NetLimiter%20Patcher.exe)

<small>SHA256: 468cf5da9b45b45114dc9e9a3d0b4aca3ef0d415324084e15dfa29cef546c961</small>

**How it works is simple:**

- Select the DLL at startup
- Stop the service using the dedicated button
- Click the “Patch DLL” button
- Restart the service with the last button
- Verify that activation was successful

At startup, the patcher creates a backup of NetLimiter.dll in the software’s folder in case of issues.

**Permanent registration:**  
Modifies the constructor of the NLLicense class by setting IsRegistered = true instead of false.

**Extended trial:**  
In NLServiceTemp.InitLicense, replaces installTime.AddDays(28.0) with AddDays(99999.0) for an infinite expiration.

**Exception fix:**  
In the same method, changes the catch block type from Exception to System.Exception to avoid errors.

**Custom name (optional):**  
Modifies the get_RegName method of the NLLicense class to return the user‑chosen name instead of the backing field.

Logs are in Italian. I’m too lazy to translate them ¯\\\_(ツ)\_/¯

![NetLimiter Activated](https://muninn.ovh/Patcher/NetLimiter/NetLimiterActivated.png)
