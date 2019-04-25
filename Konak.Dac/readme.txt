Befor using DAC component, it is necessary to add some changes to application configuration file (web.config or app.config).

1. Open configuration file to edit.
2. Find the section:
	<configuration>
		...
		<connectionStrings>
			...
		</connectionStrings>
		...
	</configuration>
	If there is no any one, create it.
3. Add one or more connection strings to that section. Something like this:
	<add name="DefaultConnection" connectionString="Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\mdf_file_name;Integrated Security=True" providerName="System.Data.SqlClient" />
	or
	<add name="Production" connectionString="Database=database_name;Server=server_name; User ID=user_id; Password=password;" providerName="System.Data.SqlClient"/>
	or
	<add name="TestConnection" connectionString="Data Source=server_name;Initial Catalog=database_name;Integrated Security=True" providerName="System.Data.SqlClient"/>
	etc.
4. Find the section:
	<configuration>
		<configSections>
			...
		</configSections>
		...
	</configuration>
	If there is no one create it. Please note, if you are just creating that section, it must be the first section.
5. Add the folowing line to section <configSections>:
	<section name="Konak.Dac" type="Konak.Dac.Configuration.ConfigSection, Konak.DAC.Standard, Culture=neutral, PublicKeyToken=null" />
4. Create a subsection named "Konak.Dac" inside of section <configuration>:
	<configuration>
		...
		<Konak.Dac>
			...
		</Konak.Dac>
		...
	</configuration>
5. In created section add the folowing line:
	<settings default_connection_string="DefaultConnection" />
	where the value of the "default_connection_string" property must point to the default connection string used to connect to database.
6. Call extension method to initialyse component.
	if it is a web application, open Global.asa file and in Application_Start method call:
	this.UseKonakDac();

	In other non web applications, you can add to startup Main function this init command:
	Konak.Dac.Extensions.Configuration.UseKonakDac();


That is all.
