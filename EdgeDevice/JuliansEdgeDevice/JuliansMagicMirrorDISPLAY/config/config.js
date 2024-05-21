let config = {
	address: "localhost", 	// Address to listen on, can be:
							// - "localhost", "127.0.0.1", "::1" to listen on loopback interface
							// - another specific IPv4/6 to listen on a specific interface
							// - "0.0.0.0", "::" to listen on any interface
							// Default, when address config is left out or empty, is "localhost"
	port: 8080,
	basePath: "/", 	// The URL path where MagicMirror is hosted. If you are using a Reverse proxy
					// you must set the sub path here. basePath must end with a /
	ipWhitelist: ["127.0.0.1", "::ffff:127.0.0.1", "::1"], 	// Set [] to allow all IP addresses

	useHttps: false, 		// Support HTTPS or not, default "false" will use HTTP
	httpsPrivateKey: "", 	// HTTPS private key path, only require when useHttps is true
	httpsCertificate: "", 	// HTTPS Certificate path, only require when useHttps is true

	language: "en",
	locale: "en-US",
	logLevel: ["INFO", "LOG", "WARN", "ERROR"], // Add "DEBUG" for even more logging
	timeFormat: 24,
	units: "metric",
	serverOnly:  false,

	modules: [
		{
			module: "clock",
			position: "top_left",
			config: {
				timeFormat: 12,
				clockBold: true,
				timezone: "Australia/Melbourne"
			}
		},
		{
			module: "MMM-sensordata",
			position: "middle_center",
			config: {
				updateInterval: 5000,
				apiUrl: "http://localhost:5000/api/sensordata"
			}
		},
		{
		    module: "newsfeed",
		    position: "bottom_bar",
		    config: {
		      feeds: [
				{
				  title: "New York Times",
				  url: "https://www.nytimes.com/services/xml/rss/nyt/HomePage.xml",
				},
				{
				  title: "Hacker News",
				  url: "https://hnrss.org/frontpage",
				},
		      ],
		    },
		  },
		{
			module: "weather",
			position: "top_right",
			config: {
				weatherProvider: "openweathermap",
				units: "metric",
				roundTemp: true,
				degreeLabel: true,
				showPrecipitationAmount: true,
				showPrecipitationProbability: true,
				type: "current",
				location: "Melbourne",
				locationID: "2158177",
				apiKey: "7c294714c2a1ed6ba29f8c4a61816e19"
			}
		}
	]
};

/*************** DO NOT EDIT THE LINE BELOW ***************/
if (typeof module !== "undefined") {module.exports = config;}
