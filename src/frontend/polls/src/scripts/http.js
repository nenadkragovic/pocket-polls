import axios from 'axios';

const { API_BASE_URL ='https://api.pocket-polls.click:5000' } = process.env

export const request = async (url, method, data) => {
	return await axios({
		url: `${API_BASE_URL}/api/${url}`,
		method: method,
		data: data,
		timeout: 5000,
		headers: {
			'Content-Type': 'application/json-patch+json',
			'Authorization': 'Bearer ' + localStorage.getItem('user-token')
		},
	});
};
