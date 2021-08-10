package mono.android.app;

public class ApplicationRegistration {

	public static void registerApplications ()
	{
				// Application and Instrumentation ACWs must be registered first.
		mono.android.Runtime.register ("TestApp.Droid.MainApplication, TestApp.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", crc64c7fff72bd74cd533.MainApplication.class, crc64c7fff72bd74cd533.MainApplication.__md_methods);
		
	}
}
