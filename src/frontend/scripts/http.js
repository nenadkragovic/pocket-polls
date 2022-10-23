import axios from 'axios';

export const httpRequest = async (url, method, data) => {
	try {
		const requestData = await axios({
			url: url,
			method: method,
			data: data,
		});
		return requestData;
	} catch (error) {
		console.log(error);
	}
};
export const httpRequestStartStopStrategy = async (url, method, data) => {
	try {
		const requestData = await axios({
			url: url,
			headers: {
				'Content-Type': 'application/json-patch+json',
			},
			method: method,
			data: data,
		});
		return requestData;
	} catch (error) {
		console.log(error);
	}
};
