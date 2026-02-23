# NetLimiter Patcher

Sets the license to 99999 days and forces license registration, it currently works with all available versions.

![NetLimiter Patcher](https://muninn.ovh/Patcher/NetLimiter/NetLimiterPatcher.png)

[**DOWNLOAD**](https://muninn.ovh/Patcher/NetLimiter/NetLimiter%20Patcher.exe)

<small>SHA512: c5043a80b7ea5b31fd302f1df2d11d5e6ca3407b1b5fa4a40cbd312575c985250b65efb67272275f84b7f385452a56b28d7f387f1f5778317be7ead4c45d34a9</small>


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

Virustotal [ClickMe](https://www.virustotal.com/gui/file/20e8cf4b9428d7fcb3ef538a19f6a0e4422f0daad89171abe1a5ba5bdb74f47c/detection)

Logs are in Italian. I’m too lazy to translate them ¯\\\_(ツ)\_/¯


![NetLimiter Activated](https://muninn.ovh/Patcher/NetLimiter/NetLimiterActivated.png)
