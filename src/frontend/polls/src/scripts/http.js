import axios from 'axios';
const baseUrl ='https://localhost:7288/api/';

export const httpRequest = async (url, method, data) => {
	try {
		const requestData = await axios({
			url: baseUrl + url,
			method: method,
			data: data,
		});
		return requestData;
	} catch (error) {
		console.log(error);
		return null;
	}
};
export const httpRequestStartStopStrategy = async (url, method, data) => {
	try {
		const requestData = await axios({
			url: baseUrl + url,
			headers: {
				'Content-Type': 'application/json-patch+json',
			},
			method: method,
			data: data,
		});
		return requestData;
	} catch (error) {
		console.log(error);
		return null;
	}
};
