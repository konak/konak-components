Befor using logging component, it is necessary to add some changes to application configuration file (web.config or app.config).

1. Open configuration file to edit.
2. Find the section:
	<configuration>
		...
		<configSections>
			...
		</configSections>
		...
	</configuration>
	If there is no one create it.
5. Add the folowing line to section <configSections>:
	<section name="Konak.Logging" type="Konak.Logging.Config.ConfigSection, Konak.Logging, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
4. Create a subsection named "Konak.Dac" inside of section <configuration>:
	<configuration>
		...
		<Konak.Logging>
			...
		</Konak.Logging>
		...
	</configuration>
5. In created section add the folowing line:
	<FileWriter enabled="true" folder="C:\Temp" file_name_prefix="AppName" file_extension="log" max_size="2048000"  />
	where the value of the "default_connection_string" property must point to the default connection string used to connect to database.

That is all.
