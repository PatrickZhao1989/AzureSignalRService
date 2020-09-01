const axios = require("axios").default;
const signalR = require("@microsoft/signalr");

const apiBaseUrl = "http://localhost:7071";

console.log("hello world");

getConnectionInfo()
	.then((info) => {
		const options = {
			accessTokenFactory: () => info.accessToken,
		};
		const connection = new signalR.HubConnectionBuilder()
			.withUrl(info.url, options)
			.configureLogging(signalR.LogLevel.Information)
			.build();

		connection.on("newMessage", (message) => {
			console.log(JSON.stringify(message));
		});
		connection.onclose(() => console.log("disconnected"));

		console.log("connecting...");
		connection
			.start()
			.then(() => console.log("connected!"))
			.catch(console.error);
	})
	.catch((error) => {
		console.log(error);
	});

function getConnectionInfo() {
	return axios
		.get(`${apiBaseUrl}/api/negotiate`, getAxiosConfig())
		.then((resp) => resp.data);
}

function sendMessage(sender, messageText) {
	return axios
		.post(
			`${apiBaseUrl}/api/messages`,
			{
				sender: sender,
				text: messageText,
			},
			getAxiosConfig()
		)
		.then((resp) => resp.data);
}

function getAxiosConfig() {
	const config = {
		headers: {},
	};
	return config;
}
