Befor using DAC component, it is necessary to initialize component.
If you are using appsettings.json configuration file for your application you will need:

1. Open Startup.cs file of your application
2. Add folowing using declaration:

	using Konak.Dac.Core.Extensions;

3. In Configure method of Startup class call UseKonakDac extension method by providing configuration object:

	app.UseKonakDac(Configuration);

4. Add necessary connection strings to ConnectionStrings section of configuration file:

	"ConnectionStrings": {
		"DefaultConnection": "Server=server_name; Database=database_name; User ID=user_id; Password=password;"
		"OtherDataStore": "Server=server_name; Database=database_name; User ID=user_id; Password=password;"
	},

5. Add Konak.Dac section to settings file and define default_connection_string property, so component will know which connection to use by default:

  "Konak.Dac": {
    "default_connection_string": "DefaultConnection"
  }

if Konak.Dac section wil not be defined, component will use first connection string as default one.

If your application if using web.config or app.config as configuration source you will need to do folowing steps:

1. From package manager please add the System.Configuration.ConfigurationManager package
2. Open configuration file to edit.
3. Find the section:
	<configuration>
		...
		<connectionStrings>
			...
		</connectionStrings>
		...
	</configuration>
	If there is no any one, create it.
4. Add one or more connection strings to that section. Something like this:
	<add name="DefaultConnection" connectionString="Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\mdf_file_name;Integrated Security=True" providerName="System.Data.SqlClient" />
	or
	<add name="Production" connectionString="Database=database_name;Server=server_name; User ID=user_id; Password=password;" providerName="System.Data.SqlClient"/>
	or
	<add name="TestConnection" connectionString="Data Source=server_name;Initial Catalog=database_name;Integrated Security=True" providerName="System.Data.SqlClient"/>
	etc.
5. Find the section:
	<configuration>
		<configSections>
			...
		</configSections>
		...
	</configuration>
	If there is no one create it. Please note, if you are just creating that section, it must be the first section.
6. Add the folowing line to section <configSections>:
	<section name="Konak.Dac" type="Konak.Dac.Configuration.ConfigSection, Konak.DAC.Standard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
7. Create a subsection named "Konak.Dac" inside of section <configuration>:
	<configuration>
		...
		<Konak.Dac>
			...
		</Konak.Dac>
		...
	</configuration>
8. In created section add the folowing line:
	<settings default_connection_string="DefaultConnection" />
	where the value of the "default_connection_string" property must point to the default connection string used to connect to database.

That is all.
