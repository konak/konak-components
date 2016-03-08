Befor using HttpBrowser component, it is necessary to add some changes to application configuration file (web.config or app.config).

1. Open configuration file to edit.
4. Find the section:
	<configuration>
		...
		<configSections>
			...
		</configSections>
		...
	</configuration>
	If there is no one, create it.
5. Add the folowing line to section <configSections>:
	<section name="Konak.HttpBrowser" type="Konak.HttpBrowser.Configuration.ConfigSection, Konak.HttpBrowser, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
4. Create a subsection named "Konak.HttpBrowser" inside of section <configuration>:
	<configuration>
		...
		<Konak.HttpBrowser>
			...
		</Konak.HttpBrowser>
		...
	</configuration>
5. In created section add the folowing subsection:
	<configuration>
		...
		<Konak.HttpBrowser>
			<settings>
			...
			</settings>
		</Konak.HttpBrowser>
		...
	</configuration>
6. In <settings> section add <browser> subsection:
	<configuration>
		...
		<Konak.HttpBrowser>
			<settings>
				<browser user_agent="Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko" accept_language="ru,en-US;q=0.8,en;q=0.6,hy;q=0.4" request_timeout="60" response_timeout="60" />
			</settings>
		</Konak.HttpBrowser>
		...
	</configuration>

	where:
		user_agent			- the value, that will be used in HTTP headers during page request process
		accept_language		- list of languages we prefere to receive data in
		request_timeout		- connection timeout in seconds during request process
		response_timeout	- connection timeout in seconds during data receive process (after receiving response from server)

If no configuration will be created in configuration files, then default configuration will be loaded accordig the following lines:

	<Konak.HttpBrowser>
		<settings>
			<browser user_agent="Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko" accept_language="ru,en-US;q=0.8,en;q=0.6,hy;q=0.4" request_timeout="60" response_timeout="60" />
		</settings>
	</Konak.HttpBrowser>

That is all.
