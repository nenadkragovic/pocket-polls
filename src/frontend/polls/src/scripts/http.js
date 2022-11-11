import axios from 'axios';

const BASE_URL ='http://192.168.1.211:5000/api/';

export const request = async (url, method, data) => {
	return await axios({
		url: BASE_URL + url,
		method: method,
		data: data,
		timeout: 5000,
		headers: {
			'Content-Type': 'application/json-patch+json',
			'Authorization': 'Bearer ' + localStorage.getItem('user-token')
		},
	});
};
